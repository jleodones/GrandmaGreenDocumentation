using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Types/PlantTypeDictionary")]
    public class PlantTypeDictionary : ScriptableObject
    {
        private readonly Dictionary<string, PlantType> dictionary = new Dictionary<string, PlantType>();
        public PlantType[] plantTypes;

        void Awake()
        {
            LoadPlantTypes();
        }

        [ContextMenu("LoadPlantTypes")]
        /// <summary>
        /// Initialize PlantType dictionary
        /// </summary>
        public void LoadPlantTypes()
        {
            foreach (PlantType type in plantTypes)
            {
                dictionary[type.plantName] = type;
            }
        }

        public PlantType this[string name]
        {
            get { return dictionary[name]; }
        }

        [ContextMenu("TestPlantTypeLookup")]
        public void TestPlantTypeLookup()
        {
            foreach (string typename in new List<string> { "Rose", "Tulip", "Undefined" })
            {
                try
                {
                    Debug.Log(string.Format("dictionary[{0}]: {1}", typename, dictionary[typename]));
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log(string.Format("dictionary[{0}]: {1}", typename, e));
                }
            }
        }
    }
}
