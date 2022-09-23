// This script defines the tab selection logic.
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class HUDController
{
    /*
    To-Do:
    - exit buttons: hide the current element, 
        show HUD element
    - buttons -> open element with same name
    */
    
    /* Define member variables*/

    private readonly VisualElement root;

    public HUDController(VisualElement root)
    {
        this.root = root;
    }

}