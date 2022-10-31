using System.Collections;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UIElements;
using Yarn.Unity;

namespace GrandmaGreen.Dialogue
{
    /// <summary>
    /// Contains coroutine methods that apply visual effects. This class is used
    /// by <see cref="LineView"/> to handle animating the presentation of lines.
    /// </summary>
    public static class Effects
    {
        /// <summary>
        /// An object that can be used to signal to a coroutine that it should
        /// terminate early.
        /// </summary>
        /// <remarks>
        /// <para>
        /// While coroutines can be stopped by calling <see
        /// cref="MonoBehaviour.StopCoroutine"/> or <see
        /// cref="MonoBehaviour.StopAllCoroutines"/>, this has the side effect
        /// of also stopping any coroutine that was waiting for the now-stopped
        /// coroutine to finish.
        /// </para>
        /// <para>
        /// Instances of this class may be passed as a parameter to a coroutine
        /// that they can periodically poll to see if they should terminate
        /// earlier than planned.
        /// </para>
        /// <para>
        /// To use this class, create an instance of it, and pass it as a
        /// parameter to your coroutine. In the coroutine, call <see
        /// cref="Start"/> to mark that the coroutine is running. During the
        /// coroutine's execution, periodically check the <see
        /// cref="WasInterrupted"/> property to determine if the coroutine
        /// should exit. If it is <see langword="true"/>, the coroutine should
        /// exit (via the <c>yield break</c> statement.) At the normal exit of
        /// your coroutine, call the <see cref="Complete"/> method to mark that the
        /// coroutine is no longer running. To make a coroutine stop, call the
        /// <see cref="Interrupt"/> method.
        /// </para>
        /// <para>
        /// You can also use the <see cref="CanInterrupt"/> property to
        /// determine if the token is in a state in which it can stop (that is,
        /// a coroutine that's using it is currently running.)
        /// </para>
        /// </remarks>
        public class CoroutineInterruptToken {

            /// <summary>
            /// The state that the token is in.
            /// </summary>
            enum State {
                NotRunning,
                Running,
                Interrupted,
            }
            private State state = State.NotRunning;

            public bool CanInterrupt => state == State.Running;
            public bool WasInterrupted => state == State.Interrupted;
            public void Start() => state = State.Running;
            public void Interrupt()
            {
                if (CanInterrupt == false) {
                    throw new InvalidOperationException($"Cannot stop {nameof(CoroutineInterruptToken)}; state is {state} (and not {nameof(State.Running)}");
                }
                state = State.Interrupted;
            }

            public void Complete() => state = State.NotRunning;
        }

        /// <summary>
        /// A coroutine that gradually reveals the text in a <see
        /// cref="TextMeshProUGUI"/> object over time.
        /// </summary>
        /// <remarks>
        /// <para>This method works by adjusting the value of the <paramref name="text"/> parameter's <see cref="TextMeshProUGUI.maxVisibleCharacters"/> property. This means that word wrapping will not change half-way through the presentation of a word.</para>
        /// <para style="note">Depending on the value of <paramref name="lettersPerSecond"/>, <paramref name="onCharacterTyped"/> may be called multiple times per frame.</para>
        /// <para>Due to an internal implementation detail of TextMeshProUGUI, this method will always take at least one frame to execute, regardless of the length of the <paramref name="text"/> parameter's text.</para>
        /// </remarks>
        /// <param name="text">A TextMeshProUGUI object to reveal the text
        /// of.</param>
        /// <param name="lettersPerSecond">The number of letters that should be
        /// revealed per second.</param>
        /// <param name="onCharacterTyped">An <see cref="Action"/> that should be called for each character that was revealed.</param>
        /// <param name="stopToken">A <see cref="CoroutineInterruptToken"/> that
        /// can be used to interrupt the coroutine.</param>
        public static IEnumerator Typewriter(Label displayText, LocalizedLine finalText, float lettersPerSecond, Action<int> onCharacterTyped, CoroutineInterruptToken stopToken = null)
        {
            stopToken?.Start();

            int maxVisibleCharacters = 0;

            // Wait a single frame to let the text component process its
            // content, otherwise text.textInfo.characterCount won't be
            // accurate
            yield return null;

            // How many visible characters are present in the text?
            var characterCount = finalText.TextWithoutCharacterName.Text.Length;

            // Early out if letter speed is zero, text length is zero
            if (lettersPerSecond <= 0 || characterCount == 0)
            {
                // Show everything and return
                displayText.text = finalText.TextWithoutCharacterName.Text;
                stopToken?.Complete();
                yield break;
            }

            // Convert 'letters per second' into its inverse
            float secondsPerLetter = 1.0f / lettersPerSecond;

            // If lettersPerSecond is larger than the average framerate, we
            // need to show more than one letter per frame, so simply
            // adding 1 letter every secondsPerLetter won't be good enough
            // (we'd cap out at 1 letter per frame, which could be slower
            // than the user requested.)
            //
            // Instead, we'll accumulate time every frame, and display as
            // many letters in that frame as we need to in order to achieve
            // the requested speed.
            var accumulator = Time.deltaTime;

            while (maxVisibleCharacters < characterCount)
            {
                if (stopToken?.WasInterrupted ?? false) {
                    yield break;
                }

                // We need to show as many letters as we have accumulated
                // time for.
                while (accumulator >= secondsPerLetter)
                {
                    if (maxVisibleCharacters < finalText.TextWithoutCharacterName.Text.Length)
                    {
                        displayText.text = finalText.TextWithoutCharacterName.Text.Substring(0, maxVisibleCharacters);
                        char currChar = finalText.TextWithoutCharacterName.Text[maxVisibleCharacters];
                        if (Char.IsLetter(currChar))
                        {
                            int charIndex = (int)(currChar) % 32;
                            Debug.Log(maxVisibleCharacters + " " + currChar + " " + charIndex);
                            onCharacterTyped?.Invoke(charIndex);
                        }
                    }
                    accumulator -= secondsPerLetter;
                    maxVisibleCharacters += 1;
                }
                accumulator += Time.deltaTime;

                yield return null;
            }

            // We either finished displaying everything, or were
            // interrupted. Either way, display everything now.
            displayText.text = finalText.TextWithoutCharacterName.Text;

            stopToken?.Complete();
        }
    }
}
