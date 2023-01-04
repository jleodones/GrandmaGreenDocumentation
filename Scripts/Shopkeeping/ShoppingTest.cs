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
            //PlantId id = (PlantId)1002;
            //ToolId idt = (ToolId)1;
            //Debug.Log(num);
            //Sprite sprite = collection.GetSprite(idt);
            //Genotype g = new Genotype("aabb");
            //g.size = Genotype.Size.Big;
            //g.trait = Genotype.Trait.Heterozygous;

            //test the selling cost function
            Seed seed = new Seed((ushort)1001, "Rose", new Genotype("AABB"));
            int s = gc.GetSellingPriceById(seed); //should be 173

            //test the getgardenlist function
            List<ShopItem> list1 = gc.GetGardenList(); //should return 8 seeds: 3 flowers, 3 veggies, 2 fruits

            //test the updatecycle function
            gc.UpdateCycle(); //should rotate the ratios correctly
            gc.UpdateCycle();

            List<ShopItem> list2 = gc.GetGardenList();
        }
    }
}

#endif