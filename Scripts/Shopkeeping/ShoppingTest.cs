using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using Sirenix.OdinInspector;
using GrandmaGreen.Garden;

#if (UNITY_EDITOR)

namespace GrandmaGreen.Shopkeeping
{
    using Id = System.Enum;
    [CreateAssetMenu(menuName = "Utilities/ShopTester", fileName = "ShopTester")]
    public class ShoppingTest : ScriptableObject
    {
        [SerializeField]
        CollectionsSO collection;
        [Button()]
        public void TestShop()
        {
            GardeningShopUIController gc = new GardeningShopUIController(collection);
            PlantId id = (PlantId)1002;
            ToolId idt = (ToolId)1;
            //int num = gc.GetSeedBaseCostById(id);
            //Debug.Log(num);
            //Sprite sprite = collection.GetSprite(idt);
            Genotype g = new Genotype("aabb");
            g.size = Genotype.Size.Big;
            g.trait = Genotype.Trait.Heterozygous;
            //Sprite sprite = collection.GetSprite(id, g, 2);
            Sprite sprite = collection.GetSprite(id, g); //get seed packet
        }
    }
}

#endif