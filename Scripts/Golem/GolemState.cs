using GrandmaGreen.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    [System.Serializable]
    public struct GolemState
    {
        public CharacterId golemID;
        public int happiness;
        public Vector3Int assignedCell;
        public bool isSpawned;
        public bool isMature;
        public bool isTravelling;
        public bool assignedWatering;
    }
}
