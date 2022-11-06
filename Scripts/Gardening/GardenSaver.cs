using GrandmaGreen.SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Collections;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GardenSaver")]
    public class GardenSaver : ObjectSaver
    {
        private readonly int plantKey = 0;
        private readonly int plantValues = 1;
        private readonly int tiles = 2;
        private readonly int decorKey = 3;
        private readonly int decorValues = 4;

        public void Initialize()
        {
            if (componentStores.Count == 0)
            {
                CreateNewStore(typeof(Vector3Int));
                CreateNewStore(typeof(PlantState));
                CreateNewStore(typeof(TileState));
                //CreateNewStore(typeof(Vector3));
                //CreateNewStore(typeof(DecorationId));
            }
        }

        public bool ContainsKey(Vector3Int k)
        {
            return ((ComponentStore<Vector3Int>)componentStores[plantKey]).components.Contains(k);
        }

        public bool Remove(Vector3Int k)
        {
            if (ContainsKey(k))
            {
                int i = ((ComponentStore<Vector3Int>)componentStores[plantKey]).components.IndexOf(k);
                ((ComponentStore<Vector3Int>)componentStores[plantKey]).components.RemoveAt(i);
                ((ComponentStore<PlantState>)componentStores[plantValues]).components.RemoveAt(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            ((ComponentStore<Vector3Int>)componentStores[plantKey]).components.Clear();
            ((ComponentStore<PlantState>)componentStores[plantValues]).components.Clear();
        }

        public List<PlantState> Values()
        {
            return ((ComponentStore<PlantState>)componentStores[plantValues]).components;
        }

        public PlantState this[Vector3Int k]
        {
            get
            {
                return ((ComponentStore<PlantState>)componentStores[plantValues])
                    .components[((ComponentStore<Vector3Int>)componentStores[plantKey])
                    .components.IndexOf(k)];
            }
            set
            {
                if (ContainsKey(k))
                {
                    ((ComponentStore<PlantState>)componentStores[plantValues])
                        .components[((ComponentStore<Vector3Int>)componentStores[plantKey])
                        .components.IndexOf(k)] = value;
                }
                else
                {
                    ((ComponentStore<Vector3Int>)componentStores[plantKey])
                        .components.Add(k);
                    ((ComponentStore<PlantState>)componentStores[plantValues])
                        .components.Add(value);
                }
            }
        }

        public void SetTileState(TileState tileState)
        {
            if (componentStores.Count <= tiles )
                CreateNewStore(typeof(TileState));

            if (((ComponentStore<TileState>)componentStores[tiles]).components.Contains(tileState))
            {
                int index = ((ComponentStore<TileState>)componentStores[tiles]).components.IndexOf(tileState);
                ((ComponentStore<TileState>)componentStores[tiles]).components[index] = tileState;
            }
            else
                ((ComponentStore<TileState>)componentStores[tiles]).components.Add(tileState);

        }

        public List<TileState> Tiles()
        {
            return ((ComponentStore<TileState>)componentStores[tiles]).components;
        }

        public void AddDecor(DecorationId decorID, Vector3 position)
        {
            if (((ComponentStore<DecorationId>)componentStores[decorKey]).components.Contains(decorID))
            {

            }
            else
            {
                ((ComponentStore<DecorationId>)componentStores[decorKey]).components.Add(decorID);
                ((ComponentStore<DecorationId>)componentStores[decorKey]).components.Add(decorID);
            }
        }


    }
}

