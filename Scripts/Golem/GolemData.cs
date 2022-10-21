using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/GolemData")]
    public class GolemData : ScriptableObject
    {
        public int golemID;
        public string golemName;
        public int happiness;
        public int stages;
        public int spawnChance;
        public GameObject[] golemPrefabs;
    }
}
