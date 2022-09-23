// This script attaches the tabbed menu logic to the game.
using UnityEngine;
using UnityEngine.UIElements;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
public class HUD : MonoBehaviour
{
    private HUDController controller;

    private void OnEnable()
    {
        UIDocument HUD = GetComponent<UIDocument>();
        VisualElement root = HUD.rootVisualElement;

        controller = new(root);

        // controller.RegisterTabCallbacks();
        // controller.RegisterExitCallback();
    }
}