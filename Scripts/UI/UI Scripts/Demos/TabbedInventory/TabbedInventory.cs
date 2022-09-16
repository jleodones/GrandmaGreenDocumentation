// This script attaches the tabbed menu logic to the game.
using UnityEngine;
using UnityEngine.UIElements;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
public class TabbedInventory : MonoBehaviour
{
    private TabbedInventoryController controller;

    private void OnEnable()
    {
        UIDocument inventory = GetComponent<UIDocument>();
        VisualElement root = inventory.rootVisualElement;

        controller = new(root);

        controller.RegisterTabCallbacks();
        controller.RegisterExitCallback();
    }
}