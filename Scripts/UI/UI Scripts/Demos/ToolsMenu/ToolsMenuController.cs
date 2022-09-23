using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolsMenuController
{
    // Member Variables
    private ToolTest toolTestScript = null;
    private readonly VisualElement root;

    // ToolsMenuController: Assigns visual element root
    public ToolsMenuController(VisualElement root)
    {
        this.root = root;
    }

    // SetToolTest: reads in backend tool script 
    public void SetToolTest(ToolTest toolScript){
        toolTestScript = toolScript;
    }

    // RegisterToolCallbacks: Assigns a function to every tool button
    public void RegisterToolCallbacks(){
        UQueryBuilder<Button> tools = GetAllTools();
        tools.ForEach((Button tool) => {
            tool.RegisterCallback<ClickEvent>(ToolOnClick);
        });
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
        toolTestScript.SetTools(clickedTool.name);
        HideToolsMenu();
    }

    // ShowToolsMenu: displays the root visual element
    public void ShowToolsMenu(){
        root.style.display = DisplayStyle.Flex;
    }

    // HideToolsMenu: hides the root visual element
    public void HideToolsMenu(){
        root.style.display = DisplayStyle.None;
    }
}
