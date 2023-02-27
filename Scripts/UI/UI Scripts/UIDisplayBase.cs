using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI
{
    
    [System.Flags]
    public enum UITag
    {
        None = 0,
        HUD = 1,
        INVENTORY = 2,
        ENTITY_MENU = 4,
        SHOPPING = 8,
        SELLING = 16,
        CUSTOMIZATION = 32,
        SETTINGS = 64,
        CULTIVISION = 128,
        DIALOGUE = 256,
        COLLECTIONS = 512,
        TUTORIAL = 1024,
        MAILBOX = 2048,
        BULLETIN = 4096,
        START_SCREEN = 8192,
        SPLASH_SCREEN = 16384,

        ALL = HUD | INVENTORY | ENTITY_MENU | SHOPPING | SELLING | CUSTOMIZATION | SETTINGS | CULTIVISION | DIALOGUE | COLLECTIONS | TUTORIAL | MAILBOX | BULLETIN | START_SCREEN | SPLASH_SCREEN
    }
    
    public class UIDisplayBase : MonoBehaviour
    {
        [field: Header("Display References")]
        [field: SerializeField] public virtual UIDocument UIDocument { get; protected set; }

        [field: Header("Display Settings")]
        [field: SerializeField] public virtual UITag panelTags { get; protected set; }

        [field: Header("Display Variables")]
        // [field: SerializeField] public virtual BaseUIController Controller { get; protected set; }
        [field: SerializeField] public virtual bool displayOpen { get; protected set; } = false;
        [field: SerializeField] public virtual bool displayEnabled { get; protected set; } = true;
        [field: SerializeField] public virtual bool displayInteractive { get; protected set; } = true;

        public event System.Action onPanelOpened;
        public event System.Action onPanelClosed;

        public event System.Action onPanelEnabled;
        public event System.Action onPanelDisabled;
        
        public VisualElement m_rootVisualElement;

        private Dictionary<string, List<Action>> m_callbackDictionary;
        protected virtual void Awake()
        {
            UIDocument ??= GetComponent<UIDocument>();
            m_rootVisualElement = UIDocument.rootVisualElement.Q<VisualElement>("rootVisualElement");
            m_callbackDictionary = new Dictionary<string, List<Action>>();
            
            RegisterButtonCallback("exitButton", CloseUI);
            UIDisplayTracker.AddPanel(this);

            if (!displayOpen)
            {
                m_rootVisualElement.style.display = DisplayStyle.None;
            }
            
            Load();
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<string, List<Action>> entry in m_callbackDictionary)
            {
                foreach (Action action in entry.Value)
                {
                    DeregisterButtonCallback(entry.Key, action);
                }
            }
            UIDisplayTracker.RemovePanel(this);
            
            Unload();
        }

        public virtual void Load()
        {
        }

        public virtual void Unload()
        {
        }

        [ContextMenu("Open")]
        public virtual void OpenUI()
        {
            if (!displayEnabled || displayOpen)
            {
                return;
            }

            UIDisplayTracker.ProcessOpenDisplayRule(panelTags);
            onPanelOpened?.Invoke();
        }

        public virtual void UIOpenLogic()
        {
            displayOpen = true;
            m_rootVisualElement.style.display = DisplayStyle.Flex;
        }

        [ContextMenu("Close")]
        public virtual void CloseUI()
        {
            if (!displayOpen)
            {
                return;
            }

            UIDisplayTracker.ProcessCloseDisplayRule(panelTags);
            onPanelClosed?.Invoke();
        }

        public virtual void UICloseLogic()
        {
            displayOpen = false;
            m_rootVisualElement.style.display = DisplayStyle.None;
        }

        public void EnableUI()
        {
            if (displayEnabled)
            {
                return;
            }
            displayEnabled = true;
            onPanelEnabled?.Invoke();
        }

        public void DisableUI()
        {
            if (!displayEnabled)
            {
                return;
            }
            displayEnabled = false;
            onPanelDisabled?.Invoke();
        }
        
        public void EnableInteraction()
        {
            if (displayInteractive)
            {
                return;
            }

            displayInteractive = true;
            InteractionEnabledLogic();
        }
        
        protected virtual void InteractionEnabledLogic()
        {
            //TODO: enable controller interaction
        }
        
        public void DisableInteraction()
        {
            if (!displayInteractive) return;

            displayInteractive = false;

            InteractionDisabledLogic();
        }
        
        protected virtual void InteractionDisabledLogic()
        {
            //TODO: disable controller interaction
        }
        
        #region utility
        public void RegisterButtonCallback(string buttonName, Action callbackFunction)
        {
            Button button;
            try
            { 
                button = m_rootVisualElement.Q<Button>(buttonName);
                button.clicked += callbackFunction;
            }
            catch (NullReferenceException e)
            {
                return;
            }

            // If this button does not already exist in the callback dictionary, add it with a new list entry.
            if (!m_callbackDictionary.ContainsKey(buttonName))
            {
                m_callbackDictionary.Add(buttonName, new List<Action>() { callbackFunction });
            }
            // Else, add the callback to the list of callbacks.
            else
            {
                m_callbackDictionary[buttonName].Add(callbackFunction);
            }
        }
        
        public void RegisterButtonCallbackWithClose(string buttonName, Action callbackFunction)
        {
            Button button;
            try
            { 
                button = m_rootVisualElement.Q<Button>(buttonName);
                button.clicked += CloseUI;
                button.clicked += callbackFunction;
            }
            catch (NullReferenceException e)
            {
                return;
            }
            
            // If this button does not already exist in the callback dictionary, add it with a new list entry.
            if (!m_callbackDictionary.ContainsKey(buttonName))
            {
                m_callbackDictionary.Add(buttonName, new List<Action>() { callbackFunction });
                m_callbackDictionary[buttonName].Add(CloseUI);
            }
            // Else, add the callback to the list of callbacks.
            else
            {
                m_callbackDictionary[buttonName].Add(callbackFunction);
            }
        }
        
        private void DeregisterButtonCallback(string buttonName, Action callbackFunction)
        {
            m_rootVisualElement.Q<Button>(buttonName).clicked -= callbackFunction;
        }
        #endregion
    }
}
