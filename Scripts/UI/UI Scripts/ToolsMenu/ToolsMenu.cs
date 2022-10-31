using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Garden;
using GrandmaGreen;

public class ToolsMenu : MonoBehaviour
{	
	
    [SerializeField] public PlayerToolData toolData;
    [SerializeField] public CameraZoom zoom;
    private ToolsMenuController controller;

    void OnEnable()
    {
        UIDocument inventory = GetComponent<UIDocument>();
        VisualElement root = inventory.rootVisualElement;

        controller = new(root);
        controller.toolData = toolData;
        controller.zoom = zoom;

        controller.RegisterToolCallbacks();
    }
}
