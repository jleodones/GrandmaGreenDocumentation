using System.Collections.Generic;
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
        public void AddTrait()
        {
            Allele[] arr = {(Allele) 1, (Allele) 0};
            var rand = new System.Random();

            Trait t;
            t.allele1 = arr[rand.Next() % 2];
            t.allele2 = arr[rand.Next() % 2];

            m_objectSavers[0].AddComponent<Trait>(-1, t);
        }

        [Button()]
        public void AddToInventory(int type, int id, int quantity)
        {
            switch (type)
            {
                case 0: // Tool
                    EventManager.instance.HandleEVENT_INVENTORY_ADD(new GrandmaGreen.Collections.Tool(id, null, 0), quantity);
                    break;
                case 1: // Plant
                    EventManager.instance.HandleEVENT_INVENTORY_ADD(new GrandmaGreen.Collections.Plant(id, null, 0, new Trait()), quantity);
                    break;
                case 2: // Seed
                    EventManager.instance.HandleEVENT_INVENTORY_ADD(new GrandmaGreen.Collections.Seed(id, null, 0), quantity);
                    break;
                case 3: // Decor
                    EventManager.instance.HandleEVENT_INVENTORY_ADD(new GrandmaGreen.Collections.Decor(id, null, 0), quantity);
                    break;
            }
        }

        [Button()]
        public void RemoveFromInventory(int type, int id, int quantity)
        {
            switch (type)
            {
                case 0: // Tool
                    EventManager.instance.HandleEVENT_INVENTORY_REMOVE(new GrandmaGreen.Collections.Tool(id, null, 0), quantity);
                    break;
                case 1: // Plant
                    EventManager.instance.HandleEVENT_INVENTORY_REMOVE(new GrandmaGreen.Collections.Plant(id, null, 0, new Trait()), quantity);
                    break;
                case 2: // Seed
                    EventManager.instance.HandleEVENT_INVENTORY_REMOVE(new GrandmaGreen.Collections.Seed(id, null, 0), quantity);
                    break;
                case 3: // Decor
                    EventManager.instance.HandleEVENT_INVENTORY_REMOVE(new GrandmaGreen.Collections.Decor(id, null, 0), quantity);
                    break;
            }
        }
        
        public int SampleTest()
        {
            return 3;
        }
    }
}
