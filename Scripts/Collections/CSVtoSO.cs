using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.SaveSystem;
using Sirenix.OdinInspector;
using System.Globalization;

//#if (UNITY_EDITOR)


/// <summary>
/// This class populates and creates the Collections SO by reading the CSV file.
/// </summary>
namespace GrandmaGreen.Collections
{
    using Id = System.Enum;

    //TODO: Expoand csv for plant entries to include seeds + extra stats
    public class CSVtoSO
    {
        private static readonly string inventoryCSVPath = "/_GrandmaGreen/Scripts/Collections/CollectionsDatabase.csv";
        public const ushort SEED_ID_OFFSET = 1000;
        /// <summary>
        /// Function to populate and creates the Collections SO by reading the CSV file
        /// </summary>
        public static void GenerateCollectionsSO(CollectionsSO collections, TextAsset dataSheet)
        {
            string[] allLines = dataSheet.text.Split('\n');
            //File.ReadAllLines(Application.dataPath + inventoryCSVPath, System.Text.Encoding.Default);

            collections.ItemLookup = new Dictionary<ushort, ItemProperties>();
            collections.PlantLookup = new Dictionary<ushort, PlantProperties>();
            collections.CharacterLookup = new Dictionary<ushort, CharacterProperties>();
            collections.SeedLookup = new Dictionary<ushort, SeedProperties>();

            for (int i = 2; i < allLines.Length; i++)
            {
                //current line in file
                var line = allLines[i].Split(',');

                if (line == null)
                    continue;
                if (!ushort.TryParse(line[0], out ushort csvID))
                    continue;

                string entryType = line[1];
                string name = line[2];
                string description = line[3];
                string tag = line[4];
                // int baseCost = Int32.Parse(line[5]);
                ushort.TryParse(line[5], out ushort baseCost);
                string seedDescription = line[6];
                //int seedBaseCost = 0;
                ushort.TryParse(line[7], out ushort seedBaseCost);
                //if (line[7].Length != 0)
                //{
                //    seedBaseCost = Int32.Parse(line[7]);
                //}
                string plantType = line[8];
                string spritePath = "";
                ItemProperties itemProps = new ItemProperties();
                itemProps.name = name;
                itemProps.description = description;
                itemProps.itemType = entryType;
                itemProps.baseCost = baseCost;
                itemProps.spritePath = spritePath;

                switch (entryType)
                {
                    //TODO: fill up other plant properties
                    case "Plant":
                        //Plant data
                        PlantProperties plantProps = new PlantProperties();

                        plantProps.name = name;
                        plantProps.description = description;
                        plantProps.spriteBasePath = "PLA_" + name.Replace(" ", "");
                        spritePath = "PLA_" + name.Replace(" ", "");
                        itemProps.spritePath = spritePath;
                        
                        // TODO: Please fix this later.
                        itemProps.baseCost = seedBaseCost;

                        //plant stats
                        ushort.TryParse(line[9], out ushort growthStages);
                        plantProps.growthStages = growthStages;
                        ushort.TryParse(line[10], out ushort growthTime);
                        plantProps.growthTime = growthTime;
                        ushort.TryParse(line[11], out ushort waterPerStage);
                        plantProps.waterPerStage = waterPerStage;

                        //flower
                        if (plantType == "Flower")
                        {
                            plantProps.plantType = (PlantType)1;
                        }
                        //veggie
                        else if (plantType == "Vegetable")
                        {
                            plantProps.plantType = (PlantType)2;
                        }
                        //fruit
                        else if (plantType == "Fruit")
                        {
                            plantProps.plantType = (PlantType)3;
                        }

                        collections.PlantLookup.Add(csvID, plantProps);
                        collections.ItemLookup.Add(csvID, itemProps);

                        //Seed Data
                        spritePath = "SEED_" + name.Replace(" ", "");
                        itemProps.spritePath = spritePath;

                        collections.ItemLookup.Add((ushort)(csvID + SEED_ID_OFFSET), itemProps);
                        SeedProperties seedProps = new SeedProperties();
                        seedProps.name = name;
                        seedProps.description = seedDescription;
                        seedProps.spritePath = spritePath;
                        seedProps.baseCost = seedBaseCost;
                        seedProps.plantType = plantProps.plantType;

                        collections.SeedLookup.Add((ushort)(csvID + SEED_ID_OFFSET), seedProps);

                        break;

                    case "Tool":
                        itemProps.spritePath = "GAR_" + name.Replace(" ", "_");
                        itemProps.baseCost = baseCost;
                        collections.ItemLookup.Add(csvID, itemProps);
                        break;

                    case "Decor":
                        itemProps.spritePath = "DEC_" + name.Replace(" ", "_");
                        itemProps.baseCost = baseCost;
                        collections.ItemLookup.Add(csvID, itemProps);
                        break;

                    case "Character":
                        CharacterProperties characterProps = new CharacterProperties();
                        characterProps.name = name;
                        characterProps.description = description;
                        characterProps.spritePaths = new List<string>();
                        spritePath = "CHA_" + name.Replace(" ", "_");
                        characterProps.spritePaths.Add(spritePath);
                        itemProps.spritePath = spritePath;
                        collections.CharacterLookup.Add(csvID, characterProps);
                        collections.ItemLookup.Add(csvID, itemProps);
                        break;

                    default:
                        Debug.Log("Entry Type not valid");
                        break;

                }
            }

        }

    }
}
//#endif