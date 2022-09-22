using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTool : MonoBehaviour
{
    public static Action interactInput;
    public static Action swapInput;
    public InputActionReference toolAction;
    public InputActionReference swapAction;

    private void Start()
    {
        toolAction.action.Enable();
        swapAction.action.Enable();
    }

    private void Update()
    {
        if(toolAction.action.triggered)
        {
            interactInput?.Invoke();
        }

        if(swapAction.action.triggered)
        {
            swapInput?.Invoke();
        }
    }
}
