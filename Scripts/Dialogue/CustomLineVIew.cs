using Yarn.Unity;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine.EventSystems;

namespace GrandmaGreen.Dialogue
{
    public class CustomLineView : DialogueViewBase
    {
        /// <summary>
        /// The UI document it uses to display text.
        /// </summary>
        public UIDocument dialogueUI;

        /// <summary>
        /// The dialogueSFX for the golem that is currently speaking.
        /// </summary>
        /// TODO: Change this to list so that it can support multiple golem voices.
        public ListContainer dialogueSFX;

        /// <summary>
        /// Defines whether the dialogue text should auto advance.
        /// </summary>
        public bool autoAdvance = false;

        /// <summary>
        /// Used to determine if the dialogue should use the typewriter effect.
        /// </summary>
        public bool useTypewriterEffect = false;

        /// <summary>
        /// The number of characters per second that should appear during the typewriter effect.
        /// </summary>
        public float typewriterEffectSpeed = 5.0f;

        /// <summary>
        /// Action that occurs when character is typed.
        /// </summary>
        public Action<int> onCharacterTyped;

        /// <summary>
        /// Grabs the name text from the dialogue UI for displaying the name of the speaker.
        /// </summary>
        private Label m_nameTextUI = null;

        /// <summary>
        /// Grabs the current line text from the dialogue UI for displaying the current line.
        /// </summary>
        private Label m_currentLineTextUI = null;

        /// <summary>
        /// The line that is currently being displayed.
        /// </summary>
        private LocalizedLine m_currentLine = null;

        /// <summary>
        /// A stop token that is used to interrupt the current animation.
        /// </summary>
        private Effects.CoroutineInterruptToken m_currentStopToken = new Effects.CoroutineInterruptToken();
        
        /// <summary>
        /// A list of the option views used to represent options in the dialogue UI.
        /// </summary>
        private List<CustomOptionView> m_optionViews;

        public Action<int> OnOptionSelected;
        void Awake()
        {
            // Grab the name UI and the current line text UI.
            m_nameTextUI = dialogueUI.rootVisualElement.Q<Label>("nameText");
            m_currentLineTextUI = dialogueUI.rootVisualElement.Q<Label>("dialogueText");

            // Set the dialogue SFX callback.
            onCharacterTyped += dialogueSFX.PlayIndex;

            // Register dialogue bubble clickable callback.
            dialogueUI.rootVisualElement.Q<Button>("dialogueContainer").RegisterCallback<ClickEvent>(OnContinueTriggered);
            
            // Set up the option buttons.
            m_optionViews = new List<CustomOptionView>();
            List<Button> optionButtons = dialogueUI.rootVisualElement.Query<Button>(className: "option-button")
                .ToList();

            for (int i = 0; i < optionButtons.Count; i++)
            {
                CustomOptionView optionView = new CustomOptionView();
                optionView.SetButton(optionButtons[i]);
                m_optionViews.Add(optionView);
            }
        }

        /// <summary>
        /// Display the dialogue bubble upon node start.
        /// </summary>
        public void StartDialogueNode()
        {
            // Clear any leftover text.
            m_currentLineTextUI.text = "";

            // Display.
            dialogueUI.rootVisualElement.Q<VisualElement>("rootVisualElement").style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Hide the dialogue bubble upon node end.
        /// </summary>
        public void EndDialogueNode()
        {
            // Hide.
            dialogueUI.rootVisualElement.Q<VisualElement>("rootVisualElement").style.display = DisplayStyle.None;
        }

        public void OnContinueTriggered(ClickEvent clickEvent)
        {
            UserRequestedViewAdvancement();
        }

        public override void UserRequestedViewAdvancement()
        {
            if (m_currentLine == null)
            {
                return;
            }

            if (m_currentStopToken.CanInterrupt)
            {
                m_currentStopToken.Interrupt();
            }

            requestInterrupt?.Invoke();
        }

        /// <summary>
        /// Called by the DialogueRunner to signal that a line should be displayed to the user.
        /// </summary>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            StopAllCoroutines();

            // Start RunLine coroutine.
            StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
        }

        private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // Present the line on the UI.
            IEnumerator PresentLine()
            {
                // Set the character name.
                m_nameTextUI.text = dialogueLine.CharacterName;

                // If using typewriter effect, start the typewriter coroutine.
                if (useTypewriterEffect)
                {
                    yield return StartCoroutine(
                        Effects.Typewriter(
                            m_currentLineTextUI,
                            dialogueLine,
                            typewriterEffectSpeed,
                            onCharacterTyped,
                            m_currentStopToken
                        )
                    );
                    if (m_currentStopToken.WasInterrupted)
                    {
                        m_currentLineTextUI.text = dialogueLine.TextWithoutCharacterName.Text;
                        yield break;
                    }
                }
                // Else, display it at once.
                else
                {
                    m_currentLineTextUI.text = dialogueLine.TextWithoutCharacterName.Text;
                    yield break;
                }
            }

            m_currentLine = dialogueLine;

            // Call to present the line.
            yield return StartCoroutine(PresentLine());

            m_currentStopToken.Complete();

            // If we are not auto advancing, pause here while we wait for the player to ask for more dialogue.
            if (!autoAdvance)
            {
                yield break;
            }

            // Presentation complete; call the completion handler.
            onDialogueLineFinished();
        }

        /// <summary>
        /// Called by the DialogueRunner to signal that a line has been interrupted, and that the Dialogue View should finish presenting its line as quickly as possible.
        /// </summary>
        public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
        {
            StopAllCoroutines();

            m_currentLine = dialogueLine;

            m_nameTextUI.text = dialogueLine.CharacterName;
            m_currentLineTextUI.text = dialogueLine.TextWithoutCharacterName.Text;

            if (m_currentStopToken.WasInterrupted)
            {
                m_currentStopToken.Complete();
            }
            else
            {
                onInterruptLineFinished();
            }
        }

        private void DisableOptions(int i)
        {
            // Enabling and disabling normal styles.
            dialogueUI.rootVisualElement.Q<VisualElement>("nameTextBox").style.display = DisplayStyle.Flex;
            dialogueUI.rootVisualElement.Q<VisualElement>("dialogueTextBox").style.display = DisplayStyle.Flex;
            dialogueUI.rootVisualElement.Q<VisualElement>("optionsTextBox").style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Called by the DialogueRunner to signal that the view should dismiss its current line from display, and clean up.
        /// </summary>
        public override void DismissLine(Action onDismissalComplete)
        {
            m_currentLine = null;

            onDismissalComplete();
        }

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            // DISPLAY.
            // Disable the name and the other dialogue box.
            dialogueUI.rootVisualElement.Q<VisualElement>("nameTextBox").style.display = DisplayStyle.None;
            dialogueUI.rootVisualElement.Q<VisualElement>("dialogueTextBox").style.display = DisplayStyle.None;
            dialogueUI.rootVisualElement.Q<VisualElement>("optionsTextBox").style.display = DisplayStyle.Flex;
            
            // Set up based on 2 or 3 choices.
            if (dialogueOptions.Length == 2)
            {
                dialogueUI.rootVisualElement.Q<VisualElement>("two-option-spacer").style.display = DisplayStyle.Flex;
                m_optionViews[2].button.style.display = DisplayStyle.None;
            }
            else // 3 choices.
            {
                dialogueUI.rootVisualElement.Q<VisualElement>("two-option-spacer").style.display = DisplayStyle.None;
                m_optionViews[2].button.style.display = DisplayStyle.Flex;
            }
            
            // RUNNING OPTIONS.
            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                var optionView = m_optionViews[i];
                var option = dialogueOptions[i];
                optionView.option = option;
                
                optionView.DisplayText(option);
                optionView.OnOptionSelected = OptionViewWasSelected;
            }

            OnOptionSelected = null;
            OnOptionSelected += DisableOptions;
            OnOptionSelected += onOptionSelected;

            void OptionViewWasSelected(DialogueOption selectedOption)
            {
                OnOptionSelected(selectedOption.DialogueOptionID);
            }
        }
    }
    
    public class CustomOptionView
    {
        public Action<DialogueOption> OnOptionSelected;

        public DialogueOption option;

        public bool hasSubmittedOptionSelection = false;

        public Button button;
        
        public DialogueOption Option
        {
            get => option;

            set
            {
                option = value;
                hasSubmittedOptionSelection = false;
            }
        }

        public void SetButton(Button _button)
        {
            button = _button;
            button.clicked += InvokeOptionSelected;
        }

        public void DisplayText(DialogueOption dialogueOption)
        {
            // Retrieve the text on this button.
            Label buttonText = button.Q<Label>("text");
            buttonText.text = dialogueOption.Line.TextWithoutCharacterName.Text;
        }

        public void InvokeOptionSelected()
        {
            if (!hasSubmittedOptionSelection)
            {
                OnOptionSelected.Invoke(Option);
                hasSubmittedOptionSelection = true;
            }
        }
    }
}
