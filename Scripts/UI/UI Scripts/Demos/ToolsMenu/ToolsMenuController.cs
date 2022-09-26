using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Garden;

public class ToolsMenuController
{
    // Member Variables
    public ToolStateData toolState;
    public IPanel panel;
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

        toolState.onToolSelectionStart += ShowToolsMenu;
        toolState.onToolSelectionEnd += HideToolsMenu;

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
        toolState.SetTool(clickedTool.name);
        HideToolsMenu();
    }

    // ShowToolsMenu: displays the root visual element
    public void ShowToolsMenu()
    {
        root.style.display = DisplayStyle.Flex;

        SetToolMenuPosition(toolState.target.entity.CurrentPos());

        toolState.target.entity.onEntityMove += SetToolMenuPosition;
    }

    // HideToolsMenu: hides the root visual element
    public void HideToolsMenu()
    {
        root.style.display = DisplayStyle.None;

        toolState.target.entity.onEntityMove -= SetToolMenuPosition;
    }

    void SetToolMenuPosition(Vector3 worldSpace)
    {
        root.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, worldSpace, Camera.main);
    }
}
