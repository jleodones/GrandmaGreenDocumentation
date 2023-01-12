using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using GrandmaGreen.Garden;

namespace GrandmaGreen.Entities
{
    public class GolemCrowdController : MonoBehaviour
    {
        [Header("Golem Management")]
        public GolemManager golemManager;

        [Header("Golem References")]
        public GardenAreaController gardenArea;

        public void Awake()
        {
            golemManager.Initialize();
            EventManager.instance.EVENT_ASSIGN_TASK += AssignGolemAction;
        }

        [Header("Debug Options")]
        public CharacterId theGolem = CharacterId.PumpkinGolem;
        public int value = 0;

        [Button(ButtonSizes.Medium)]
        public void UpdateGolemHappiness()
        {
            EventManager.instance.HandleEVENT_GOLEM_HAPPINESS_UPDATE((ushort)theGolem, value);
        }

        [Button(ButtonSizes.Medium)]
        public void SpawnGolem()
        {
            ushort id = (ushort)CharacterId.TulipGolem;
            Vector3 pos = new Vector3(0,0,0);
            golemManager.OnGolemSpawn(id, pos);
        }

        public void AssignGolemAction(ushort id)
        {
            Debug.Log("Task Assigned");
            List<PlantState> plants = gardenArea.gardenManager.GetPlants(gardenArea.areaIndex);
            if(plants.Count != 0)
            {
                int randIndex = Random.Range(0, plants.Count - 1);
                Debug.Log(plants[randIndex].cell);
                golemManager.UpdateGolemTask(id, plants[randIndex].cell);
            }

        }

        public void GolemDoAction()
        {
            foreach(GolemState golem in golemManager.golemStateTable)
            {
                if (golem.assignedWatering)
                {
                    EventManager.instance.HandleEVENT_GOLEM_DO_TASK();
                }
            }
        }

        [Button(ButtonSizes.Medium)]
        public void WaterGolem()
        {
            GolemDoAction();
        }
    }
}
