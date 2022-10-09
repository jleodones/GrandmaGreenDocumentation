using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System;
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
        private string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/CollectionsDatabase.csv";
        [SerializeField] SpriteAtlas atlas;
        
        /// <summary>
        /// Function to populate and creates the Collections SO by reading the CSV file
        /// </summary>
        [Button()]
        public void GenerateCollectionsSO() {
            string[] allLines = File.ReadAllLines(Application.dataPath + inventoryCSVPath);
            CollectionsSO collections = ScriptableObject.CreateInstance<CollectionsSO>();
            for(int i=2; i<allLines.Length; i++)
            {
                //current line in file
                var line = allLines[i].Split(',');
                Item t = new Item();
                t.id = Int32.Parse(line[0]);
                string type = line[1];
                string imgName = "";
                t.name = line[2];
                //idk what default variant each naming convention should be so i'll add the variant part later
                if(type == "Tool")
                {
                    t.itemType = itemTypes.Tool;
                    imgName = "GAR_" + t.name;
                }
                else if(type == "Seed")
                {
                    t.itemType = itemTypes.Seed;
                    imgName = "GAR_" + t.name;
                }
                else if(type == "Plant")
                {
                    t.itemType = itemTypes.Plant;
                    imgName = "PLA_" + t.name;
                }
                else if(type == "Decor")
                {
                    t.itemType = itemTypes.Decor;
                    t.tag = line[4];
                    imgName = "DEC_" + t.name;
                }
                else if(type == "Character")
                {
                    t.itemType = itemTypes.Character;
                    imgName = "CHA_" + t.name;
                }
                t.description = line[3];

                //currently trying to get this to work:
                // Sprite sp = atlas.GetSprite(imgName);
                // t.SetImage(sp);
                
                //dummy image naming convention (not the actual naming convention lol)
                //t.SetImage(Resources.Load("image" + t.id, typeof(Sprite)) as Sprite);

                t.SetImage(Resources.Load(imgName, typeof(Sprite)) as Sprite);

                // ItemSpritesSO spritesSO = Resources.Load("Item Sprites SO", typeof(ItemSpritesSO)) as ItemSpritesSO;
                // for(int j=0; j<spritesSO.itemSprites.Count; j++)
                // {
                //     if(t.id == spritesSO.itemSprites[j].id)
                //     {
                //         t.SetImage(spritesSO.itemSprites[j].img);
                //     }
                // }

                collections.items.Add(t);
            }
            //might want to change where this gets saved
            AssetDatabase.CreateAsset(collections, $"Assets/_GrandmaGreen/Scripts/Collections/CollectionsSystemData.asset");
            AssetDatabase.SaveAssets();
        }

    }
}
#endif