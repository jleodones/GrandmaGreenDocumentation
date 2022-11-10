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
        /// Function to populate and creates the Collections SO by reading the CSV file
        /// </summary>
        [Button()]
        public void GenerateCollectionsSO() {

            string[] allLines = File.ReadAllLines(Application.dataPath + inventoryCSVPath);
            CollectionsSO collections = ScriptableObject.CreateInstance<CollectionsSO>();
            collections.ItemLookup = new Dictionary<Id, ItemProperties>();
            collections.PlantLookup = new Dictionary<PlantId, PlantProperties>();
            collections.CharacterLookup = new Dictionary<CharacterId, CharacterProperties>();
            collections.SeedLookup = new Dictionary<SeedId, SeedProperties>();
            for(int i=2; i<allLines.Length; i++)
            {
                //current line in file
                var line = allLines[i].Split(',');
                int csvID = Int32.Parse(line[0]);
                string name = line[2];
                string description = line[3];
                string tag = line[4];
                int baseCost = Int32.Parse(line[5]);
                string spritePath = "";
                ItemProperties itemProps = new ItemProperties();;
                itemProps.name = name;
                itemProps.description = description;
                itemProps.baseCost = baseCost;
                //plant
                if(csvID/1000 == 1)
                {
                    PlantId plantId = (PlantId)csvID;
                    PlantProperties plantProps = new PlantProperties();
                    plantProps.name = name;
                    plantProps.description = description;
                    //csv does not provide growth stages
                    plantProps.spriteBasePath = "PLA_" + name;
                    collections.PlantLookup.Add(plantId, plantProps);
                    collections.ItemLookup.Add(plantId, itemProps);
                }
                //seed
                else if(csvID/1000 == 2)
                {
                    SeedId seedId = (SeedId)csvID;
                    spritePath = "GAR_" + name;
                    itemProps.spritePath = spritePath;
                    itemProps.baseCost = baseCost;
                    itemProps.cycleCooldown = 0;
                    collections.ItemLookup.Add(seedId, itemProps);
                    SeedProperties seedProps = new SeedProperties();
                    seedProps.name = name;
                    seedProps.description = description;
                    seedProps.spritePath = spritePath;
                    seedProps.baseCost = baseCost;
                    seedProps.cycleCooldown = 0;
                    //flower
                    if(2001 <= csvID && csvID <= 2007)
                    {
                        seedProps.seedType = (SeedType)1;
                    }
                    //veggie
                    else if(2008 <= csvID && csvID <= 2014)
                    {
                        seedProps.seedType = (SeedType)2;
                    }
                    //fruit
                    else if(2015 <= csvID && csvID <= 2021)
                    {
                        seedProps.seedType = (SeedType)3;
                    }
                    collections.SeedLookup.Add(seedId, seedProps);
                }
                //tool
                else if(csvID/1000 == 3)
                {
                    ToolId toolId = (ToolId)csvID;
                    itemProps.spritePath = "GAR_" + name;
                    itemProps.baseCost = baseCost;
                    collections.ItemLookup.Add(toolId, itemProps);
                }
                //decor
                else if(csvID/1000 == 4)
                {
                    DecorationId decorId = (DecorationId)csvID;
                    itemProps.spritePath = "DEC_" + name;
                    itemProps.cycleCooldown = 0;
                    itemProps.baseCost = baseCost;
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
                    spritePath = "CHA_" + name;
                    characterProps.spritePaths.Add(spritePath);
                    collections.CharacterLookup.Add(characterId, characterProps);
                    collections.ItemLookup.Add(characterId, itemProps);
                }
            }
            AssetDatabase.CreateAsset(collections, $"Assets/_GrandmaGreen/Scripts/Collections/CollectionsSystemData.asset");
            AssetDatabase.SaveAssets();
        }

    }
}
#endif