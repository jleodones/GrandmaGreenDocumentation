using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [CreateAssetMenu(fileName = "New Inventory")]
    public class Inventory : ScriptableObject
    {
        public List<string> items = new List<string>();
    }
}