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
            EventManager.instance.EVENT_ASSIGN_TASK += AssignGolemAction;
            golemManager.golemWorkTimer.Pause();
        }

        void OnEnable() {
            golemManager.LoadGolemData();
            golemManager.golemWorkTimer.Resume(true);
            golemManager.golemWorkTimer.onTick += GolemDoAction;
        }

        void OnDisable() {
            golemManager.SaveGolemData();
            golemManager.golemWorkTimer.Pause();
            golemManager.golemWorkTimer.onTick -= GolemDoAction;
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
            List<PlantState> plants = gardenArea.wiltedPlantList;//gardenManager.GetPlants(gardenArea.areaIndex);
            if(plants.Count != 0)
            {
                //Debug.Log("Watering Task");
                golemManager.UpdateGolemTask(id);
            }

        }

        public void GolemDoAction(int value)
        {
            List<PlantState> plants = gardenArea.wiltedPlantList;
            int wiltedIndex = 0;
            bool fireEvent = false;

            foreach(GolemState golem in golemManager.golemStateTable)
            {
                // Add a check for Golem Happiness here once we get this sorted out
                if((ushort)golem.golemID != 0){
                    if ((golem.assignedWatering && golem.happiness > 0) && wiltedIndex < plants.Count)
                    {
                        fireEvent = true;
                        //Debug.Log("Assigned Cell: " + plants[wiltedIndex].cell);
                        golemManager.UpdateTaskCell((ushort)golem.golemID, plants[wiltedIndex].cell);
                        
                        wiltedIndex++;
                    } else {
                        golemManager.UpdateGolemTask((ushort)golem.golemID);
                    }
                }       
            }
            
            if(fireEvent) EventManager.instance.HandleEVENT_GOLEM_DO_TASK();
        }

        [Button(ButtonSizes.Medium)]
        public void WaterGolem()
        {
            GolemDoAction(1);
        }
    }
}
