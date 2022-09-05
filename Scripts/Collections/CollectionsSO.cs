using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [CreateAssetMenu(fileName = "New Collections SO")]
    public class CollectionsSO : ScriptableObject
    {
        //public List<List<string>> items = new List<List<string>>();
        public List<Item> items = new List<Item>();
    }
}