using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Garden;
using GrandmaGreen.UI.HUD;

namespace GrandmaGreen {
    public class GameSystemDummy : MonoBehaviour
    {
        public HUD hud;
        
        [Button(ButtonSizes.Medium)]
        public void ResourcesLoadTest()
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("PLA_Tulip");

            Sprite sprite = sprites.Single(s => s.name == "PLA_Tulip_Seedling");
        }
        
        [Button(ButtonSizes.Medium)]
        public void AddTrait()
        {
            //Allele[] arr = {(Allele) 1, (Allele) 0};
            //var rand = new System.Random();

            //Trait t;
            //t.allele1 = arr[rand.Next() % 2];
            //t.allele2 = arr[rand.Next() % 2];

            //m_objectSavers[0].AddComponent<Trait>(-1, t);
        }

        [Button()]
        public void AddSampleSeedToInventory()
        {
            EventManager.instance.HandleEVENT_INVENTORY_ADD_SEED(1001, new Genotype("AaBb"));
        }

        [Button()]
        public void RemoveSampleSeedFromInventory()
        {
            EventManager.instance.HandleEVENT_INVENTORY_REMOVE_SEED(1001, new Genotype("AaBb"));
        }

        [Button()]
        public void AddDecorItem()
        {
            // Adds some decor items (fountain and flamingo ?)
            EventManager.instance.HandleEVENT_INVENTORY_ADD_DECOR(4048);
            EventManager.instance.HandleEVENT_INVENTORY_ADD_DECOR(4029);
        }
        
        public int SampleTest()
        {
            return 3;
        }
        
        [Button()]
        public void CloseHUD()
        {
            hud.CloseUI();
        }

        [Button()]
        public void OpenHUD()
        {
            hud.OpenUI();
        }
    }
}
