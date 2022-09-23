using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolsMenu : MonoBehaviour
{	
	
    [SerializeField] public ToolTest toolScript;
    private ToolsMenuController controller;

    void OnEnable()
    {
        UIDocument inventory = GetComponent<UIDocument>();
        VisualElement root = inventory.rootVisualElement;

        controller = new(root);
        controller.SetToolTest(toolScript);
        controller.RegisterToolCallbacks();
    }
}
