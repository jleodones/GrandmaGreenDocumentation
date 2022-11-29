using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Garden;

namespace GrandmaGreen {
    public class GameSystemDummy : MonoBehaviour
    {
        [SerializeField]
        private List<ObjectSaver> m_objectSavers = new List<ObjectSaver>();

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
        
        public int SampleTest()
        {
            return 3;
        }
    }
}
