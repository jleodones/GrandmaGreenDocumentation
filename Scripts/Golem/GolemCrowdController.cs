using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Entities;
using UnityEngine;

namespace GardenmaGreen.Entities
{
    public class GolemCrowdController : MonoBehaviour
    {
        [Header("Golem Management")]
        public GolemManager golemManager;
        
        public void Awake()
        {
            golemManager.Initialize();
        }

        //For debugging
        [ContextMenu("Spawn Golem")]
        public void spawnAGolem()
        {
            ushort id = (ushort)CharacterId.PumpkinGolem;
            Vector3 pos = new Vector3(0,0,0);
            golemManager.OnGolemSpawn(id, pos);
        }

    }
}
