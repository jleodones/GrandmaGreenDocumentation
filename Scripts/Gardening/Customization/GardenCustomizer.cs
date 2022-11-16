using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Core.Input;
using DG.Tweening;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GardenCustomizer")]
    public class GardenCustomizer : ScriptableObject
    {
        [Header("References")]
        [SerializeField] GardenDecorItem decorItemPrefab;
        [SerializeField] Collections.DecorationId debugDecor;
        [SerializeField] Collections.CollectionsSO collections;
        [SerializeField] TileStore tileStore;
        [SerializeField] PointerState pointerState;

        [Header("Settings")]
        [SerializeField] float colliderSizeModifier = 1.05f;
        [SerializeField] float validCheckTime = 0.05f;
        [SerializeField] LayerMask decorLayerMask;
        [SerializeField] Color validColor;
        [SerializeField] Color invalidColor;

        public GardenDecorItem GenerateDecorItem() => GenerateDecorItem(debugDecor);

        public GardenDecorItem GenerateDecorItem(Collections.DecorationId decorID)
        {
            GardenDecorItem decorItem = Instantiate(decorItemPrefab);

            decorItem.decorID = decorID;
            Sprite decorSprite = collections.GetSprite(decorID);
            decorItem.GetComponentInChildren<SpriteRenderer>().sprite = decorSprite;

            Vector3 colliderSize = decorItem.boundsCollider.size;
            colliderSize.x = decorSprite.bounds.size.x * colliderSizeModifier;
            decorItem.boundsCollider.size = colliderSize;

            return decorItem;
        }


        public bool CheckValidState(BoxCollider decorItem, GardenAreaController decorArea)
        {
            Vector3Int tileBlockOrigin = Vector3Int.zero;
            tileBlockOrigin.x = (int)(decorItem.bounds.min.x / decorArea.tilemap.cellSize.x);
            tileBlockOrigin.y = (int)(decorItem.bounds.min.y / decorArea.tilemap.cellSize.y);

            Vector3Int tileBlockSize = Vector3Int.one;
            tileBlockSize.x = (int)(decorItem.bounds.size.x / decorArea.tilemap.cellSize.x);
            tileBlockSize.y = (int)(decorItem.bounds.size.y / decorArea.tilemap.cellSize.y);

            BoundsInt colliderBounds = new BoundsInt(tileBlockOrigin, tileBlockSize);

            TileBase[] m_tileBlock = decorArea.tilemap.GetTilesBlock(colliderBounds);

            foreach (TileBase tileBase in m_tileBlock)
            {
                if (!tileStore[tileBase].pathable || tileStore[tileBase].occupied || tileStore[tileBase].plantable)
                    return false;
            }

            foreach (Collider coll in Physics.OverlapBox(decorItem.bounds.center, decorItem.bounds.extents, Quaternion.identity, decorLayerMask))
            {
                if (coll != decorItem)
                    return false;
            }

            return true;
        }

        Coroutine customizationState;

        public void EnterCustomizationState(GardenAreaController decorArea, GardenDecorItem decorItem)
        {
            customizationState = decorArea.StartCoroutine(CustomizationStateHandler(decorArea, decorItem));
        }

        public void ForceExitCustomizationState(GardenAreaController decorArea)
        {
            decorArea.StopCoroutine(customizationState);
        }

        /// <summary>
        /// TODO: Dont calculate per frame
        /// TODO: Color sprite
        /// </summary>
        /// <param name="decorArea"></param>
        /// <param name="decorItem"></param>
        /// <returns></returns>
        IEnumerator CustomizationStateHandler(GardenAreaController decorArea, GardenDecorItem decorItem)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(validCheckTime);

            Vector3 destination = decorArea.lastSelectedPosition;
            destination.z = 0;

            decorItem.transform.position = destination;

            decorItem.DisableInteraction();
            Debug.Log("Customization start");
            bool isValid = false;
            do
            {
                destination = decorArea.lastDraggedPosition;
                destination.z = 0;

                decorItem.transform.position = destination;

                Physics.SyncTransforms();

                isValid = CheckValidState(decorItem.boundsCollider, decorArea);

                yield return null;

            } while (decorItem && pointerState.phase != PointerState.Phase.NONE);

            decorItem.EnableInteraction();

            EventManager.instance.HandleEVENT_CUSTOMIZATION_END(isValid);

            Debug.Log("Customization end");
        }
    }
}
