using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace GrandmaGreen
{
    public class NewItemUI : MonoBehaviour
    {
        [SerializeField] CollectionsSO collectionsSO;
        [SerializeField] CanvasGroup parentCanvas;
        [SerializeField] TextMeshProUGUI textObj;
        [SerializeField] Image image;

        List<(string, Sprite)> newItemQueue = new List<(string, Sprite)>();

        Sequence itemAnimSeq;

        void OnEnable()
        {
            EventManager.instance.EVENT_INVENTORY_ADD_SEED += QueueNewPlant;
            EventManager.instance.EVENT_INVENTORY_ADD_PLANT += QueueNewSeed;
        }

        void OnDisable()
        {
            EventManager.instance.EVENT_INVENTORY_ADD_SEED -= QueueNewPlant;
            EventManager.instance.EVENT_INVENTORY_ADD_PLANT -= QueueNewSeed;
        }


        void QueueNewPlant(ushort id, Garden.Genotype genotype)
        {
            string name = collectionsSO.GetPlant((PlantId)id).name;
            Sprite sprite = collectionsSO.GetInventorySprite((PlantId)id, genotype);


            QueueNewItem(name, sprite);
        }

        void QueueNewSeed(ushort id, Garden.Genotype genotype)
        {
            string name = collectionsSO.GetPlant((PlantId)id).name;
            Sprite sprite = collectionsSO.GetSprite((PlantId)id, genotype);

            QueueNewItem(name, sprite);
        }

        void QueueNewItem(string itemName, Sprite itemSprite)
        {
            newItemQueue.Add((itemName, itemSprite));

            if (newItemQueue.Count == 1)
                PlayNewitemAnimation();
        }

        Vector3 localPosOrigin;
        void PlayNewitemAnimation()
        {
            parentCanvas.alpha = 1.0f;
            localPosOrigin = parentCanvas.transform.localPosition;
            SetNewItemValues();

            itemAnimSeq = DOTween.Sequence()
            .Append(parentCanvas.transform.DOLocalMoveY(2.5f, 1.0f))
            .Insert(0.5f, DOTween.To(() => parentCanvas.alpha, x => parentCanvas.alpha = x, 0, 1.5f))
            .AppendCallback(CheckItemQueue);
        }

        void SetNewItemValues()
        {
            textObj.text = newItemQueue[0].Item1;
            image.sprite = newItemQueue[0].Item2;
        }


        void CheckItemQueue()
        {
            parentCanvas.transform.localPosition = localPosOrigin;
            newItemQueue.RemoveAt(0);
            if (newItemQueue.Count > 0)
            {
                PlayNewitemAnimation();
            }
        }
    }
}
