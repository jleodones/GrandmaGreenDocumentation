using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Core.Input;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap tiles;

    //private void Awake()
    //{
    //    Debug.Log("TileManager is Awake");
    //    InputAction click = new InputAction(binding: "<Mouse>/leftButton");
    //    click.performed += ctx =>
    //    {
    //        //RaycastHit hit;
    //        //Vector3 coor = Mouse.current.position.ReadValue();
    //        //if (Physics.Raycast(camera.ScreenPointToRay(coor), out hit))
    //        //{
    //        //    //hit.collider.GetComponent<IClickable>()?.OnClick();
    //        //    print("Hit" + hit);
    //        //}
    //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector3Int gridPosition = tiles.WorldToCell(mousePosition);
    //        TileBase clickedTile = tiles.GetTile(gridPosition);
    //        Debug.Log("Clicked position " + gridPosition + " with tile type " + clickedTile);
    //    };
    //    click.Enable();
    //}

    //private void Awake()
    //{
    //    leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
    //    leftMouseClick.performed += ctx => LeftMouseClicked();
    //    leftMouseClick.Enable();
    //}

    //private void LeftMouseClicked()
    //{
    //    Debug.Log("LeftMouseClicked");
    //}

    //private void Update()
    //{
    //    if (TouchInteraction.OnInteraction)
    //    {
    //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector3Int gridPosition = tiles.WorldToCell(mousePosition);
    //        TileBase clickedTile = tiles.GetTile(gridPosition);
    //        Debug.Log("Clicked position " + gridPosition + " with tile type " + clickedTile);
    //    }
    //}

    void Update()
     {
        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray))
                {
                    Debug.Log("Screen was touched");
                }
            }
        }
    }
}
