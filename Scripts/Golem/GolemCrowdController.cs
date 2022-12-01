using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GrandmaGreen.Entities
{
    public class GolemCrowdController : MonoBehaviour
    {
        [Header("Golem Management")]
        public GolemManager golemManager;
        
        public void Awake()
        {
            golemManager.Initialize();
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
            ushort id = (ushort)CharacterId.PumpkinGolem;
            Vector3 pos = new Vector3(0,0,0);
            golemManager.OnGolemSpawn(id, pos);
        }

    }
}
