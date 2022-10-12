using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryListController
{
    // UXML template for list entries
    VisualTreeAsset m_ListEntryTemplate;
    VisualTreeAsset m_EntryTemplate;

    // UI element references
    ListView m_InventoryList;
    Label m_InvClassLabel;
    VisualElement m_InvImageElement;
    VisualElement m_InvIcon;

    public void InitializeInventoryList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        EnumerateAllInventoryItems();

        // Store a reference to the template for the list entries
        m_ListEntryTemplate = listElementTemplate;

        // Store a reference to the character list element
        m_InventoryList = root.Q<ListView>("character-list");

        // Store references to the selected character info elements
        // m_InvClassLabel = root.Q<Label>("character-class");
        // m_InvImageElement = root.Q<VisualElement>("inventory-image");
        // m_InvIcon = root.Q<VisualElement>("character-portrait");

        FillInventoryList();
    }

    List<InventoryData> m_AllInventoryItems;

    void EnumerateAllInventoryItems()
    {
        m_AllInventoryItems = new List<InventoryData>();
        m_AllInventoryItems.AddRange(Resources.LoadAll<InventoryData>("Inventory"));
    }

    void FillInventoryList()
    {
        // Set up a make item function for a list entry
        m_InventoryList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = m_ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new InventoryListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };
        // Set up bind function for a specific list entry
        m_InventoryList.bindItem = (item, index) =>
        {
            (item.userData as InventoryListEntryController).SetInventoryData(m_AllInventoryItems[index]);
        };

        // Set a fixed item height
        m_InventoryList.fixedItemHeight = 70;

        // // Set the actual item's source list/array
        m_InventoryList.itemsSource = m_AllInventoryItems;
    }
}