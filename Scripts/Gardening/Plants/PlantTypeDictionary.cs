using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Types/PlantTypeDictionary")]
    public class PlantTypeDictionary : ScriptableObject
    {
        private readonly Dictionary<string, PlantType> dictionary = new Dictionary<string, PlantType>();
        public PlantType[] plantTypes;
    
        public PlantType this[int plantIndex]
        {
            get { return plantTypes[plantIndex]; }
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
