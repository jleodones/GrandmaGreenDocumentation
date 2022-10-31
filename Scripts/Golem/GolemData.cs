using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    public enum GolemStage
    {
        baby = 0,
        growUp = 1
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/GolemData")]
    public class GolemData : ScriptableObject
    {
        public int golemID;
        public string golemName;
        public int happiness;
        public int isSpawned;
        public bool isTravelling;
        public GolemStage stages;
        public int spawnChance;
        public string info;

        public GameObject[] golemPrefabs;
    }
}
