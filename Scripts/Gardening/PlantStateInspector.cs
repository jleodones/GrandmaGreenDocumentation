using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/PlantStateInspector")]
    public class PlantStateInspector : ScriptableObject
    {
        public PlantStateManager plantStateManager;
        [SerializeField]
        List<Plant> plants;

        [ContextMenu("InspectGardenData")]
        public void InspectGardenData()
        {
            plants = new List<Plant>();
            foreach (Plant plant in plantStateManager.plantData[0])
            {
                if (plant.type != null)
                {
                    plants.Add(plant);
                }
            }
        }
    }
}
