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
/// This class populates and creates the Collections SO by reading the CSV file.
/// Also contains GetSpriteById(int id) to retrieve a sprite by id.
/// </summary>
namespace GrandmaGreen.Collections {
    using Id = System.Enum;
    
    [CreateAssetMenu(menuName = "Utilities/CSV Generator", fileName = "CSVGenerator")]
    public class CSVtoSO : ScriptableObject
    {
        private string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/CollectionsDatabase.csv";

        /// <summary>
        /// Function to retrieve sprite by id. Returns null if there does not exist a sprite for the id.
        /// </summary>
        // [Button()]
        // public Sprite GetSpriteById(int id)
        // {
        //     CollectionsSO collection = (CollectionsSO)AssetDatabase.LoadAssetAtPath("Assets/_GrandmaGreen/Scripts/Collections/CollectionsSystemData.asset", typeof(CollectionsSO));
        //     if(collection)
        //     {
        //         List<Item> items = collection.items;
        //         foreach(Item i in items)
        //         {
        //             if(i.id == id)
        //             {
        //                 return i.GetImage();
        //             }
        //         }
        //     }
        //     return null;
        // }
        
        /// <summary>
        /// Function to populate and creates the Collections SO by reading the CSV file
        /// </summary>
        [Button()]
        public void GenerateCollectionsSO() {
            // EventManager.instance.EVENT_INVENTORY_GETSPRITE += GetSpriteById;

            string[] allLines = File.ReadAllLines(Application.dataPath + inventoryCSVPath);
            CollectionsSO collections = ScriptableObject.CreateInstance<CollectionsSO>();
            collections.ItemLookup = new Dictionary<Id, ItemProperties>();
            collections.PlantLookup = new Dictionary<PlantId, PlantProperties>();
            collections.CharacterLookup = new Dictionary<CharacterId, CharacterProperties>();
            // SpriteMapSO spriteMap = ScriptableObject.CreateInstance<SpriteMapSO>();
            for(int i=2; i<allLines.Length; i++)
            {
                //current line in file
                var line = allLines[i].Split(',');
                // t.id = Int32.Parse(line[0]);
                int csvID = Int32.Parse(line[0]);
                string name = line[2];
                string description = line[3];
                string tag = line[4];
                //csv only gives one file path per item, add this to the data(list, dict, whatever) containing filepaths for each item
                string spritePath = line[5];
                ItemProperties itemProps = new ItemProperties();;
                itemProps.name = name;
                itemProps.description = description;
                itemProps.spritePath = spritePath;
                //plant
                if(csvID/1000 == 1)
                {
                    PlantId plantId = (PlantId)csvID;
                    PlantProperties plantProps = new PlantProperties();
                    plantProps.name = name;
                    plantProps.description = description;
                    //csv does not provide growth stages
                    plantProps.spritePaths = new List<string>();
                    plantProps.spritePaths.Add(spritePath);
                    collections.PlantLookup.Add(plantId, plantProps);
                    collections.ItemLookup.Add(plantId, itemProps);
                }
                //seed
                else if(csvID/1000 == 2)
                {
                    SeedId seedId = (SeedId)csvID;
                    collections.ItemLookup.Add(seedId, itemProps);
                }
                //tool
                else if(csvID/1000 == 3)
                {
                    ToolId toolId = (ToolId)csvID;
                    collections.ItemLookup.Add(toolId, itemProps);

                }
                //decor
                else if(csvID/1000 == 4)
                {
                    DecorationId decorId = (DecorationId)csvID;
                    collections.ItemLookup.Add(decorId, itemProps);
                }
                //character
                else if(csvID/1000 == 5)
                {
                    CharacterId characterId = (CharacterId)csvID;
                    CharacterProperties characterProps = new CharacterProperties();
                    characterProps.name = name;
                    characterProps.description = description;
                    characterProps.spritePaths = new List<string>();
                    characterProps.spritePaths.Add(spritePath);
                    collections.CharacterLookup.Add(characterId, characterProps);
                    collections.ItemLookup.Add(characterId, itemProps);
                }

                //figure out if we need to manually put filepath in csv or if its possible to automatically figure out the filepath here

                // string type = line[1];
                // string imgName = "";
                // t.name = line[2];
                // //idk what default variant each naming convention should be so i'll add the variant part later
                // if(type == "Tool")
                // {
                //     t.itemType = itemTypes.Tool;
                //     imgName = "GAR_" + t.name;
                // }
                // else if(type == "Seed")
                // {
                //     t.itemType = itemTypes.Seed;
                //     imgName = "GAR_" + t.name;
                // }
                // else if(type == "Plant")
                // {
                //     t.itemType = itemTypes.Plant;
                //     imgName = "PLA_" + t.name;
                // }
                // else if(type == "Decor")
                // {
                //     t.itemType = itemTypes.Decor;
                //     t.tag = line[4];
                //     imgName = "DEC_" + t.name;
                // }
                // else if(type == "Character")
                // {
                //     t.itemType = itemTypes.Character;
                //     imgName = "CHA_" + t.name;
                // }
                // t.description = line[3];

                // //add to the SpriteMapSO at the index corresponding to item id
                // List<Sprite> sprites = new List<Sprite>();
                // sprites.Add(Resources.Load(imgName, typeof(Sprite)) as Sprite);
                // spriteMap.itemSprites.Insert(t.id, sprites);

                // // t.SetImage(Resources.Load(imgName, typeof(Sprite)) as Sprite);

                // collections.items.Add(t);
            }
            //might want to change where this gets saved
            AssetDatabase.CreateAsset(collections, $"Assets/_GrandmaGreen/Scripts/Collections/CollectionsSystemData.asset");
            AssetDatabase.SaveAssets();
        }

    }
}
#endif