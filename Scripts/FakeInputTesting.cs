using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
public class FakeInputTesting : MonoBehaviour
{
    InputEventPtr eventPtr;

    [ContextMenu("Enter Fake Input")]
    void EnterFakeInput()
    {
        using (StateEvent.From(Keyboard.current, out eventPtr))
        {
            (Keyboard.current.spaceKey).WriteValueIntoEvent(1.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }
}
