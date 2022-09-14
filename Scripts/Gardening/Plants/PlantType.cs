using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Types/PlantType")]
    public class PlantType : ScriptableObject
    {
        public string plantName;
        public GameObject[] growthStagePrefabs;
        public int growthTimeMins;
        public int growthStages;

        /// <summary>
        /// Get growth stage by age in seconds (1<=growthStage<=growthStages)
        /// </summary>
        public int GetGrowthStage(float timeSecs)
        {
            int currentStage = (int)((timeSecs / 60) / growthTimeMins * growthStages) + 1;
            // Clamp 1 <= growthStage <= growthStages
            return (currentStage < 1 ? 1 : currentStage) > growthStages ?
                growthStages : currentStage;
        }

        /// <summary>
        /// Get prefab at current growth stage by age in seconds
        /// </summary>
        public GameObject GetPrefab(float timeSecs) =>
            growthStagePrefabs[GetGrowthStage(timeSecs) - 1];

        [ContextMenu("TestGrowthStage")]
        public void TestCurrentGrowthStage()
        {
            for (int i = 0; i <= 60 * growthTimeMins; i += (60 * growthTimeMins / growthStages))
            {
                Debug.Log(string.Format("Growth stage at {0}: {1}", i,
                    GetGrowthStage(i)));
            }
        }

        [ContextMenu("TestGrowthStagePrefabs")]
        public void TestCurrentGrowthStagePrefab()
        {
            for (int i = 0; i <= 60 * growthTimeMins; i += (60 * growthTimeMins / growthStages))
            {
                Debug.Log(string.Format("Growth prefab at {0}: {1}", i,
                    GetPrefab(i).name));
            }
        }
    }
}
