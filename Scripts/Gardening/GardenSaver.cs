using GrandmaGreen.SaveSystem;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GardenSaver")]
    public class GardenSaver : ObjectSaver
    {
        private readonly int key = 0;
        private readonly int values = 1;

        public void Initialize()
        {
            if (componentStores.Count == 0)
            {
                CreateNewStore(typeof(Vector3Int));
                CreateNewStore(typeof(PlantState));
            }
        }

        public bool ContainsKey(Vector3Int k)
        {
            return ((ComponentStore<Vector3Int>)componentStores[key]).components.Contains(k);
        }

        public bool Remove(Vector3Int k)
        {
            if (ContainsKey(k))
            {
                int i = ((ComponentStore<Vector3Int>)componentStores[key]).components.IndexOf(k);
                ((ComponentStore<Vector3Int>)componentStores[key]).components.RemoveAt(i);
                ((ComponentStore<PlantState>)componentStores[values]).components.RemoveAt(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            ((ComponentStore<Vector3Int>)componentStores[key]).components.Clear();
            ((ComponentStore<PlantState>)componentStores[values]).components.Clear();
        }

        public List<PlantState> Values()
        {
            return ((ComponentStore<PlantState>)componentStores[values]).components;
        }

        public PlantState this[Vector3Int k]
        {
            get
            {
                return ((ComponentStore<PlantState>)componentStores[values])
                    .components[((ComponentStore<Vector3Int>)componentStores[key])
                    .components.IndexOf(k)];
            }
            set
            {
                if (ContainsKey(k))
                {
                    ((ComponentStore<PlantState>)componentStores[values])
                        .components[((ComponentStore<Vector3Int>)componentStores[key])
                        .components.IndexOf(k)] = value;
                }
                else
                {
                    ((ComponentStore<Vector3Int>)componentStores[key])
                        .components.Add(k);
                    ((ComponentStore<PlantState>)componentStores[values])
                        .components.Add(value);
                }
            }
        }
    }
}

