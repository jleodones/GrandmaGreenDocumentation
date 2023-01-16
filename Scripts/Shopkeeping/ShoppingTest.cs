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
            //test the garden shop:

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

            //test the decor shop:

            DecorShopUIController dc = new DecorShopUIController(collection);
            //test the getdecorlist function
            List<ShopItem> list3 = dc.GetDecorList();
            //test the updatecycle function (should increment both cycle counters and reset respective master lists accordingly)
            dc.UpdateCycle();
            dc.UpdateCycle(); //now the cycle for regular decor should be set back to 1 and decor master list should be re copied
            dc.UpdateCycle();
            dc.UpdateCycle(); //now the cycle for fixtures should be set back to 1 and fixtures master list should be re copied
            //test the unlockgardenexpansion function (should just increment the garden expansion, and maybe later u can test if it updates the proper player info
            dc.UnlockGardenExpansion();
            //test getsellingpricebyid (for now its just half the base cost) -- should return 0 if non decor item is inputted
            Decor decor = new Decor((ushort)4001, "Torch");
            int d = dc.GetSellingPriceById(decor); //should return 50
            int s2 = dc.GetSellingPriceById(seed); //should return 0
        }
    }
}

#endif