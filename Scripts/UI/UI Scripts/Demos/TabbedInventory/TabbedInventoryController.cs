// This script defines the tab selection logic.
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class TabbedInventoryController
{
    /* Define member variables*/
    private const string tabClassName = "tab-button";
    private const string currentlySelectedTabClassName = "active-tab";
    private const string unselectedContentClassName = "inactive-content";
    private const string exitButton = "exit-button";
    private const string inventoryElement = "inventory-element";
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "-tab";
    // Define the suffix of the tab content name
    private const string contentNameSuffix = "-content";

    private ToolTest toolTestScript = null;


    private readonly VisualElement root;

    public void SetToolTest(ToolTest toolScript){
        toolTestScript = toolScript;
    }

    public TabbedInventoryController(VisualElement root)
    {
        this.root = root;
    }

    public void RegisterTabCallbacks()
    {
        UQueryBuilder<Button> tabs = GetAllTabs();
        tabs.ForEach((Button tab) => {
            tab.RegisterCallback<ClickEvent>(TabOnClick);
        });
    }
    // Register the hide function to the event: when exit button is clicked
    public void RegisterExitCallback()
    {
        root.Q<Button>(exitButton).RegisterCallback<ClickEvent>(ExitOnClick);
    }

    // This hides the entire inventory panel when the exit button is clicked
    private void ExitOnClick(ClickEvent evt){
        root.Q(inventoryElement).style.display = DisplayStyle.None;
    }

    /* Method for the tab on-click event: 

       - If it is not selected, find other tabs that are selected, unselect them 
       - Then select the tab that was clicked on
    */
    private void TabOnClick(ClickEvent evt)
    {
        Button clickedTab = evt.currentTarget as Button;
        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
    }
    //Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(Button tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Button> GetAllTabs()
    {
        return root.Query<Button>(className: tabClassName);
    }

    /* Method for the selected tab: 
       -  Takes a tab as a parameter and adds the currentlySelectedTab class
       -  Then finds the tab content and removes the unselectedContent class */
    private void SelectTab(Button tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.RemoveFromClassList(unselectedContentClassName);
        // Sending tab name to Ashley
        toolTestScript.SetTools(tab.name);
    }

    /* Method for the unselected tab: 
       -  Takes a tab as a parameter and removes the currentlySelectedTab class
       -  Then finds the tab content and adds the unselectedContent class */
    private void UnselectTab(Button tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.AddToClassList(unselectedContentClassName);
    }

    // Method to generate the associated tab content name by for the given tab name
    private static string GenerateContentName(Button tab) =>
        tab.name.Replace(tabNameSuffix, contentNameSuffix);

    // Method that takes a tab as a parameter and returns the associated content element
    private VisualElement FindContent(Button tab)
    {
        return root.Q(GenerateContentName(tab));
    }
}