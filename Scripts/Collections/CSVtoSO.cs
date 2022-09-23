using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.SaveSystem;
using Sirenix.OdinInspector;

#if (UNITY_EDITOR)


/// <summary>
/// This class populates and creates the Collections SO by reading the CSV file
/// </summary>
namespace GrandmaGreen.Collections {
    
    [CreateAssetMenu(menuName = "Utilities/CSV Generator", fileName = "CSVGenerator")]
    public class CSVtoSO : ScriptableObject
    {
        private string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/Temp CSV File.csv";
        
        /// <summary>
        /// Function to populate and creates the Collections SO by reading the CSV file
        /// </summary>
        [Button()]
        public void GenerateCollectionsSO() {
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
                string imgName = t.name;
                //Sprite img;
                ItemSpritesSO spritesSO = Resources.Load("Item Sprites SO", typeof(ItemSpritesSO)) as ItemSpritesSO;
                for(int j=0; j<spritesSO.itemSprites.Count; j++)
                {
                    if(imgName == spritesSO.itemSprites[j].name)
                    {
                        t.SetImage(spritesSO.itemSprites[j].img);
                    }
                }

                collections.items.Add(t);
            }
            //might want to change where this gets saved
            AssetDatabase.CreateAsset(collections, $"Assets/_GrandmaGreen/Scripts/Collections/CollectionsSO.asset");
            AssetDatabase.SaveAssets();
        }

    }
}
#endif