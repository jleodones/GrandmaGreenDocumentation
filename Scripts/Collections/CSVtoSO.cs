using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.SaveSystem;
using Sirenix.OdinInspector;

#if (UNITY_EDITOR)

/*
* This class populates and creates the Collections SO by reading the CSV file
*/
namespace GrandmaGreen.Collections {
    
    [CreateAssetMenu(menuName = "Utilities/CSV Generator", fileName = "CSVGenerator")]
    public class CSVtoSO : ScriptableObject
    {
        public ObjectSaver InventoryData;
        private string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/Temp CSV File.csv";
        
        [Button()]
        public void GenerateCollectionsSO() {
            InventoryData.CreateNewStore<InventoryItem>();
            
            string[] allLines = File.ReadAllLines(Application.dataPath + inventoryCSVPath);
            CollectionsSO collections = ScriptableObject.CreateInstance<CollectionsSO>();
            for(int i=1; i<allLines.Length; i++)
            {
                //current line in file
                var line = allLines[i].Split(',');
                
                Item t = new Item();
                t.serialID = i - 1;
                t.id = line[0];
                t.name = line[1];
                t.description = line[2];

                collections.items.Add(t);
                
                // For every item that gets loaded, also add it to the InventoryData SO.
                InventoryData.AddComponent<InventoryItem>(-1, new InventoryItem()
                {
                    ID = t.id,
                    serialID = t.serialID,
                    quantity = 0
                });
            }
            //might want to change where this gets saved
            AssetDatabase.CreateAsset(collections, $"Assets/_GrandmaGreen/Scripts/Collections/CollectionsSO.asset");
            AssetDatabase.SaveAssets();
        }

    }
}
#endif