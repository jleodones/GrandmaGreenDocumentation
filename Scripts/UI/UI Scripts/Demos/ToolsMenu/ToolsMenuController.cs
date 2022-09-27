using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Garden;
using GrandmaGreen;

public class ToolsMenuController
{
    // Member Variables
    public PlayerToolData toolData;
    public IPanel panel;
    public CameraZoom zoom;
    private readonly VisualElement root;

    // ToolsMenuController: Assigns visual element root
    public ToolsMenuController(VisualElement root)
    {
        this.root = root;
    }


    // RegisterToolCallbacks: Assigns a function to every tool button
    public void RegisterToolCallbacks()
    {
        UQueryBuilder<Button> tools = GetAllTools();
        tools.ForEach((Button tool) =>
        {
            tool.RegisterCallback<ClickEvent>(ToolOnClick);
        });

        toolData.onToolSelectionStart += ShowToolsMenu;
        toolData.onToolSelectionEnd += HideToolsMenu;

        root.style.display = DisplayStyle.None;
    }

    // GetAllTools: returns all buttons (which are all tools)
    private UQueryBuilder<Button> GetAllTools()
    {
        return root.Query<Button>();
    }

    // When tool is clicked, send Ashley name of tool and hide ToolMenu
    private void ToolOnClick(ClickEvent evt)
    {
        Button clickedTool = evt.currentTarget as Button;

        toolData.ToolSelection(int.Parse(clickedTool.text));
        //HideToolsMenu();
    }

    // ShowToolsMenu: displays the root visual element
    public void ShowToolsMenu()
    {
        root.style.display = DisplayStyle.Flex;

        SetToolMenuPosition(toolData.playerController.entity.CurrentPos());

        toolData.playerController.entity.onEntityMove += SetToolMenuPosition;

        zoom.ZoomCameraRequest(4.2f, 0.5f);
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
        root.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, worldSpace, Camera.main);
    }
}
