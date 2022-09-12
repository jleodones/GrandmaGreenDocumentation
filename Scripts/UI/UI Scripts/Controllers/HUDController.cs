using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public Button collectionsButton;
    public Button cameraButton;
    public Button settingsButton;
    public Button inventoryButton;
    public Button customButton;
    public Button visionButton;
    public Label currencyText;
    public Label fertilizerText;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Buttons
        collectionsButton = root.Q<Button>("collections-button");
        cameraButton = root.Q<Button>("camera-button");
        settingsButton = root.Q<Button>("settings-button");
        inventoryButton = root.Q<Button>("inventory-button");
        customButton = root.Q<Button>("custom-button");
        visionButton = root.Q<Button>("vision-button");

        // Text Labels
        currencyText = root.Q<Label>("currency-text");
        fertilizerText = root.Q<Label>("ferilizer-text");
    }
}
