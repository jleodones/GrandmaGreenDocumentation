using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System;
using Sirenix.OdinInspector;

namespace GrandmaGreen.YarnOverrides
{
    public class CustomLineView : DialogueViewBase
    {
        /// <summary>
        /// Dictates where to place the root of the dialogue bubble.
        /// </summary>
        private Vector3 spawnPosition;

        /// <summary>
        /// The line that is currently being displayed.
        /// </summary>
        private LocalizedLine currentLine = null;

        /// <summary>
        /// Called upon by speaking entities to indicate where the dialogue bubble should spawn.
        /// </summary>
        public void SetLocation(Vector3 entityPosition)
        {
            spawnPosition = entityPosition;
        }
        
        /// <summary>
        /// Called by the DialogueRunner to signal that a line should be displayed to the user.
        /// </summary>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // If this is the start of 
            if (currentLine == null)
            {
                
            }
        }
        
        /// <summary>
        /// Called by the DialogueRunner to signal that a line has been interrupted, and that the Dialogue View should finish presenting its line as quickly as possible.
        /// </summary>

        public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            
        }

        /// <summary>
        /// Called by the DialogueRunner to signal that the view should dismiss its current line from display, and clean up.
        /// </summary>
        public override void DismissLine(Action onDismissalComplete)
        {
            
        }
    }
}
