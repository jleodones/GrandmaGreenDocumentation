using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if (UNITY_EDITOR)

/*
* This class populates and creates the Collections SO by reading the CSV file
*/
namespace GrandmaGreen.Collections {
    public class CSVtoSO
    {
        private static string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/Temp CSV File.csv";
        //temporarily calling this function via menu item, get rid of this later
        [MenuItem("Utilities/Generate Collections SO")]
        public static void GenerateCollectionsSO() {
            string[] allLines = File.ReadAllLines(Application.dataPath + inventoryCSVPath);
            CollectionsSO collections = ScriptableObject.CreateInstance<CollectionsSO>();
            for(int i=1; i<allLines.Length; i++)
            {
                //current line in file
                var line = allLines[i].Split(',');
                // List<string> attributes = new List<string>();
                // for(int j=0; j<line.Length; j++)
                // {
                //     //line[j] is an attribute of the item (ie name, id, description)
                //     attributes.Add(line[j]);
                // }
                // collections.items.Add(attributes);
                Item t = new Item();
                t.id = line[0];
                t.name = line[1];
                t.description = line[2];

                collections.items.Add(t);
            }
            //might want to change where this gets saved
            AssetDatabase.CreateAsset(collections, $"Assets/_GrandmaGreen/Scripts/Collections/CollectionsSO.asset");
            AssetDatabase.SaveAssets();
        }

    }
}
#endif