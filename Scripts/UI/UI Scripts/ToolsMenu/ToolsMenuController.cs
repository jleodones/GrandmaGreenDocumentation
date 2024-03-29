using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Garden;
using GrandmaGreen;

[System.Serializable]
public class ToolsMenuController
{
    // Member Variables
    public PlayerToolData toolData;
    public IPanel panel;
    public CameraZoom zoom;
    [SerializeField] private readonly VisualElement root;
    [SerializeField] private VisualElement bloomStart;

    // ToolsMenuController: Assigns visual element root
    public ToolsMenuController(VisualElement root)
    {
        this.root = root;
        panel = root.panel;
        Debug.Log(root);
    }


    // RegisterToolCallbacks: Assigns a function to every tool button
    public void RegisterToolCallbacks()
    {
        ToolCallbacks();

        toolData.onToolSelectionStart += ShowToolsMenu;
        toolData.onToolSelectionEnd += HideToolsMenu;
        root.style.display = DisplayStyle.None;
    }

    //ToolCallbacks: Assigns functions to tool buttons (seperated for reusability)
    public void ToolCallbacks(){
        UQueryBuilder<Button> tools = GetAllTools();
        tools.ForEach((Button tool) =>
        {
            tool.RegisterCallback<ClickEvent>(ToolOnClick);
        });
    }

    // GetAllTools: returns all buttons (which are all tools)
    private UQueryBuilder<Button> GetAllTools()
    {
        // Debug.Log(root.Q<Button>()); 
        return root.Query<Button>();
    }

    // When tool is clicked send tool data to back and play animation
    private void ToolOnClick(ClickEvent evt)
    {
        Button clickedTool = evt.currentTarget as Button;
        clickedTool.UnregisterCallback<ClickEvent>(ToolOnClick);
        toolData.ToolSelection(int.Parse(clickedTool.text));

        // Start bloom animation
        bloomStart = root.Q(clickedTool.name + "-bloom");
        bloomStart.RegisterCallback<TransitionEndEvent>(OnBloom);
        bloomStart.style.transitionProperty = new List<StylePropertyName> { "scale", "rotate" };
        bloomStart.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.EaseInOut, EasingMode.EaseInOutSine };
        bloomStart.style.transitionDuration = new List<TimeValue> { new TimeValue(200, TimeUnit.Millisecond), new TimeValue(400, TimeUnit.Millisecond) };
        bloomStart.style.transitionDelay = new List<TimeValue> { new TimeValue(0, TimeUnit.Millisecond) };
        bloomStart.style.scale = new Scale(new Vector2(2, 2));
        bloomStart.style.rotate = new Rotate(-25);
    }

    // On end bloom animation, start unbloom animation
    private void OnBloom(TransitionEndEvent evt)
    {
        bloomStart.UnregisterCallback<TransitionEndEvent>(OnBloom);
        bloomStart.RegisterCallback<TransitionEndEvent>(OnUnbloom);
        bloomStart.style.transitionDelay = new List<TimeValue> { new(200, TimeUnit.Millisecond) };
        bloomStart.style.scale = new Scale(new Vector2(1, 1)); 
        bloomStart.style.rotate = new Rotate(25);
    }

    private void OnUnbloom(TransitionEndEvent evt)
    {
        ToolCallbacks();
        toolData.EndToolSelection();
        bloomStart.UnregisterCallback<TransitionEndEvent>(OnUnbloom);
        
    }

    // ShowToolsMenu: displays the root visual element
    public void ShowToolsMenu()
    {
        // root.style.transitionDuration = new List<TimeValue>{ new TimeValue(300, TimeUnit.Millisecond) };
        root.style.display = DisplayStyle.Flex;

        SetToolMenuPosition(toolData.playerController.entity.CurrentPos());

        toolData.playerController.entity.onEntityMove += SetToolMenuPosition;

        zoom.ZoomCameraRequest(3.85f, 0.5f);
    }

    // HideToolsMenu: hides the root visual element
    public void HideToolsMenu()
    {
        root.style.display = DisplayStyle.None;
        toolData.playerController.entity.onEntityMove -= SetToolMenuPosition;

        zoom.ZoomCameraRequest(5.0f, 0.5f);
    }

    void SetToolMenuPosition(Vector3 worldSpace)
    {
        root.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(panel, worldSpace, Camera.main);
    }
}
