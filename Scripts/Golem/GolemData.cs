using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Collections;

namespace GrandmaGreen.Entities
{

    public class GolemData : ScriptableObject
    {
        public CharacterId golemID;
        public string golemName;
        public int spawnChance;
        public string info;
    }
}
