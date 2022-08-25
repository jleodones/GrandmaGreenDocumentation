using UnityEngine;
using UnityEditor;
using System.IO;

namespace GrandmaGreen.Collections {
    public class CSVtoSO
    {
        private static string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/Temp CSV File.csv";
        [MenuItem("Utilities/Generate Inventory")]
        public static void GenerateInventory() {
            string[] allLines = File.ReadAllLines(Application.dataPath + inventoryCSVPath);
            Inventory inventory = ScriptableObject.CreateInstance<Inventory>();
            foreach(string s in allLines)
            {
                //if csv structure gets modified, this part needs to be changed
                inventory.items.Add(s);
            }
            //might want to change where this gets saved
            AssetDatabase.CreateAsset(inventory, $"Assets/_GrandmaGreen/Scripts/Collections/Inventory.asset");
            AssetDatabase.SaveAssets();
        }

    }
}