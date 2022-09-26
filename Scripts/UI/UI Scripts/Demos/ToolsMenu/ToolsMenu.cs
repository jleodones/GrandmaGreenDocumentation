using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Garden;

public class ToolsMenu : MonoBehaviour
{	
	
    [SerializeField] public ToolTest toolScript;
    [SerializeField] public ToolStateData toolData;
    private ToolsMenuController controller;

    void OnEnable()
    {
        UIDocument inventory = GetComponent<UIDocument>();
        VisualElement root = inventory.rootVisualElement;

        controller = new(root);
        controller.toolState = toolData;
        
        controller.RegisterToolCallbacks();
    }
}
