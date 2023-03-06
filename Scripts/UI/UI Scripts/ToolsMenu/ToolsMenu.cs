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
    [SerializeField] private ToolsMenuController controller;

    UIDocument document;

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }
    
    void Start()
    {
        VisualElement root = document.rootVisualElement;

        controller = new(root);
        controller.toolData = toolData;
        controller.zoom = zoom;

        controller.RegisterToolCallbacks();
    }

    public void EnableToolButton(string button)
    {
        document.rootVisualElement.Q<VisualElement>(button).SetEnabled(true);
    }


    public void DisableToolButton(string button)
    {
        document.rootVisualElement.Q<VisualElement>(button).SetEnabled(false);
    }
}
