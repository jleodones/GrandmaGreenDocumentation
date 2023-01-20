using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public interface IDraggableContainer
    {
        IDraggable draggable { get; set; }
        VisualElement threshold { get; set; }
        Vector3 pointerStartPosition { get; set; }

        bool enabled { get; set; }
        bool handled { get; set; }

        void PointerDownHandler(PointerDownEvent evt);
    }
    
    public interface IDraggable
    {
        // This button.
        Button button { get; set; }
        
        // Draggable item's starting position.
        Vector3 startingPosition { get; set; }
    }
}
