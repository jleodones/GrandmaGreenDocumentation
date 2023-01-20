using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using Sirenix.OdinInspector;

namespace GrandmaGreen.Shopkeeping
{
    using Id = System.Enum;
    using Random = System.Random;

    [Serializable]
    public struct ShopItem
    {
        public Sprite sprite;
        public int quantity;
        public int baseCost; //this is the original item's base cost multiplied by the mega value. AKA buying cost
        public string name;
        public IInventoryItem myItem;
    }

    /// <summary>
    /// Creates Dictionary<Id, ShopItem> of each item that will show up this cycle for the Gardening Shop UI to pull from
    /// </summary>
    public class GardeningShopUIController
    {
        List<ShopItem> GardenList;
        CollectionsSO collections;
        List<Seed> AllSeedsList; //copy of the plant genotype master list

        int currCycle; //after the 4th cycle, reset allseedslist. increment each cycle.

        //ratio of flowers/veggies/fruits to rotate each cycle
        int numFlowers;
        int numVeggies;
        int numFruits;

        /// <summary>
        /// Helper function to determine mega value given the genotype in string form
        /// </summary>
        /// <param name="genoString"></param>
        /// <returns></returns>
        private int GetMegaValue(string genoString)
        {
            switch (genoString)
            {
                case "AABB":
                case "aaBB":
                case "AAbb":
                case "aabb":
                    return 12;
                    break;
                case "AABb":
                case "aaBb":
                    return 4;
                    break;
                case "AaBB":
                case "Aabb":
                    return 3;
                    break;
                case "AaBb":
                    return 1;
                    break;
                default:
                    return 0;
                    break;
            }
        }


        /// <summary>
        /// Pass in the collections SO.
        /// </summary>
        public GardeningShopUIController(CollectionsSO collectionsinput)
        {
            collections = collectionsinput;
            AllSeedsList = new List<Seed>(collections.PlantGenotypeMasterList);

            currCycle = 1;
            //initial ratios
            numFlowers = 3;
            numVeggies = 3;
            numFruits = 2;
        }

        /// <summary>
        /// Each new cycle, call this function to update the gardening shopkeeping system's cycle count
        /// If exceeds 
        /// </summary>
        public void UpdateCycle()
        {
            currCycle++;
            //rotate the ratios
            switch (currCycle)
            {
                case 2:
                    numFlowers = 2;
                    numVeggies = 3;
                    numFruits = 3;
                    break;
                case 3:
                    numFlowers = 3;
                    numVeggies = 2;
                    numFruits = 3;
                    break;
                case 4:
                    numFlowers = 3;
                    numVeggies = 3;
                    numFruits = 2;
                    break;
            }
            //reset every 4 cycles
            if(currCycle > 4)
            {
                currCycle = 1;
                AllSeedsList = new List<Seed>(collections.PlantGenotypeMasterList);
            }
        }

        /// <summary>
        /// Generate and get List<ShopItem> containing all items in the gardening shop this cycle
        /// Call this once per cycle. Reset the garden shop after the 4th cycle
        /// </summary>
        /// <returns></returns>
        public List<ShopItem> GetGardenList()
        {
            List<Seed> tempSeedList = new List<Seed>();
            List<string> genotypeList = new List<string> { "AABB", "AABb", "AAbb", "AaBB", "AaBb", "Aabb", "aaBB", "aaBb", "aabb" };
            //extract 8 out of the list
            //after 4 cycles, reset the shop and thus the AllSeedsList
            int currNumFlowers = 0;
            int currNumVeggies = 0;
            int currNumFruits = 0;
            for (int i=0; i<8; i++)
            {
                // cannot have the same plant type with different genotype in the store at the same time
                //  (if plant type (ie rose, tulip) exists in gardenlist already, skip it)

                //ensure that there is a 3:3:2 ratio of plant seed types per cycle (e.g. 3 flower-type seeds, 3 veggie-type seeds, 2 type-fruit seeds)
                //the ratio should strictly rotate each shop cycle (3veggie:3fruit:2flower → 3fruit:3flower:2veggie → 3flower:3veggie:2fruit)

                bool validIndexFound = false;
                while(!validIndexFound)
                {
                    Random rnd = new Random();
                    int ind = rnd.Next(AllSeedsList.Count);
                    Seed currSeed = AllSeedsList[ind];
                    if(!tempSeedList.Contains(currSeed)) //make sure seed is not already in our list
                    {
                        bool typeAlreadyExists = false;
                        //make sure seed type (rose, tulip, etc) is not already in our list (they will have the same ID)
                        foreach(Seed tempSeed in tempSeedList)
                        {
                            if(tempSeed.itemID == currSeed.itemID)
                            {
                                typeAlreadyExists = true;
                            }
                        }
                        if (!typeAlreadyExists)
                        {
                            //check whether it's flower,veggie, or fruit. if limit of specific type is exceeded, skip this seed
                            Collections.PlantType plantType = collections.GetPlant((PlantId)currSeed.itemID).plantType;
                            if(plantType == (Collections.PlantType)1) //flower
                            {
                                currNumFlowers++;
                                if(currNumFlowers > numFlowers)
                                {
                                    continue;
                                }
                            }
                            else if(plantType == (Collections.PlantType)2) //veggie
                            {
                                currNumVeggies++;
                                if(currNumVeggies > numVeggies)
                                {
                                    continue;
                                }
                            }
                            else if (plantType == (Collections.PlantType)3) //fruit
                            {
                                currNumFruits++;
                                if(currNumFruits > numFruits)
                                {
                                    continue;
                                }
                            }
                            //if seed is valid, add to tempSeedList, remove from AllSeedsList
                            tempSeedList.Add(currSeed);
                            AllSeedsList.RemoveAt(ind);
                            validIndexFound = true;
                        }
                    }
                }
            }
            
            //generate garden list -- convert tempseedlist to gardenlist -- for each seed, set values and add to gardenlist
            GardenList = new List<ShopItem>();
            for(int i=0; i<tempSeedList.Count; i++)
            {
                Seed seed = tempSeedList[i];
                ShopItem item = new ShopItem();
                item.sprite = collections.GetSprite((PlantId)seed.itemID, seed.seedGenotype);
                item.quantity = 3;
                item.name = seed.itemName;
                int megaValue = GetMegaValue(seed.seedGenotype.ToString());
                item.baseCost = collections.GetItem(seed.itemID).baseCost * megaValue;
                item.myItem = new Seed((ushort)seed.itemID, seed.itemName, seed.seedGenotype);
                GardenList.Add(item);
            }

            return GardenList;
        }

        /// <summary>
        /// Get the selling price of an item in player's inventory in the gardening shop
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetSellingPriceById(IInventoryItem item)
        {
            if (item.itemID >= 1000 && item.itemID < 4000)
            {
                PlantProperties plantProps = collections.GetPlant((PlantId)item.itemID);
                /* 
                 * Use plantProps item to get the baseGoldPerTimeUnit and multiply by mega value, then multiply by growthTime (also from plantprops), times 2
                 */
                int megaValue = 0;
                Genotype genotype;
                if (item.itemType == ItemType.Plant)
                {
                    Plant plant = (Plant)item;
                    //set the genotype
                    genotype = plant.plantGenotype;
                    megaValue = GetMegaValue(genotype.ToString());
                }
                else if (item.itemType == ItemType.Seed)
                {
                    Seed seed = (Seed)item;
                    genotype = seed.seedGenotype;
                    megaValue = GetMegaValue(genotype.ToString());
                }

                return (int)(plantProps.baseGoldPerTimeUnit * megaValue * plantProps.growthTime * 2);
            }

            //can only sell plants and seeds in the garden shop
            else return 0;
        }

        /// <summary>
        /// Returns if the given item is sellable or not. If seed or plant, returns true. otherwise false.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsSellable(IInventoryItem item)
        {
            //can only sell seeds and plants
            if(item.itemType == ItemType.Seed || item.itemType == ItemType.Plant)
            {
                return true;
            }
            else return false;
        }

    }

    /// <summary>
    /// Creates List of each item that will show up this cycle for the Decor Shop UI to pull from
    /// </summary>
    public class DecorShopUIController
    {
        List<ShopItem> DecorList; //all items that will show up in decor shop this cycle
        CollectionsSO collections;
        List<Decor> AllDecorList;
        List<Decor> AllFixturesList;
        int currCycleDecor; //after the 2nd cycle, reset the all decor items list. increment each cycle.
        int currCycleFixture; //after the 4th cycle, reset the all fixture decor items list. increment each cycle.
        int currGardenExpansion; //the new garden expansion the player has get to unlock (default to 1, which means player only has a base garden and wants expansion 1)

        /// <summary>
        /// Pass in the collections SO. List will need to retrieve base costs of each item from the collections
        /// </summary>
        /// <param name="collections"></param>
        public DecorShopUIController(CollectionsSO collectionsinput)
        {
            collections = collectionsinput;
            AllDecorList = new List<Decor>(collections.DecorList);
            AllFixturesList = new List<Decor>(collections.FixtureList);

            DecorList = new List<ShopItem>();

            currCycleDecor = 1;
            currCycleFixture = 1;

            currGardenExpansion = 1;
        }

        /// <summary>
        /// Each new cycle, call this function to update the decor shopkeeping system's cycle count
        /// </summary>
        public void UpdateCycle()
        {
            currCycleDecor++;
            currCycleFixture++;
            if(currCycleDecor > 2)
            {
                currCycleDecor = 1;
                AllDecorList = new List<Decor>(collections.DecorList);
            }
            if(currCycleFixture > 4)
            {
                currCycleFixture = 1;
                AllFixturesList = new List<Decor>(collections.FixtureList);
            }
        }

        /// <summary>
        /// Get List containing all items in the decor shop this cycle
        /// </summary>
        /// <returns></returns>
        public List<ShopItem> GetDecorList()
        {
            /*
             * randomly pick 6 decor items but making sure not to have duplicate decor types,
             * pop from the master list, and have some way to track which garden expansion
             * the player has unlocked, and then the other slot is fixture
             */
            List<Decor> tempDecorList = new List<Decor>();
            //regular decor items:
            for (int i = 0; i < 6; i++)
            {
                bool validIndexFound = false;
                while (!validIndexFound)
                {
                    Random rnd = new Random();
                    int ind = rnd.Next(AllDecorList.Count);
                    Decor currDecor = AllDecorList[ind];
                    if (!tempDecorList.Contains(currDecor) && collections.ItemLookup[currDecor.itemID].decorType != "Fixture") //make sure item is not already in our list
                    {
                        //make sure decor type does not already exist in our list
                        bool typeAlreadyExists = false;
                        foreach (Decor item in tempDecorList)
                        {
                            if (collections.ItemLookup[currDecor.itemID].decorType == collections.ItemLookup[item.itemID].decorType)
                            {
                                typeAlreadyExists = true;
                            }
                        }
                        if (!typeAlreadyExists)
                        {
                            tempDecorList.Add(currDecor);
                            AllDecorList.RemoveAt(ind);
                            validIndexFound = true;
                        }
                    }
                }
            }
            //fixture:
            int fixtureSize = 1;
            if(currGardenExpansion > 3)
            {
                fixtureSize = 2;
            }
            for (int i = 0; i < fixtureSize; i++)
            {
                bool validIndexFound = false;
                while (!validIndexFound)
                {
                    Random rnd = new Random();
                    int ind = rnd.Next(AllFixturesList.Count);
                    Decor currFixture = AllFixturesList[ind];
                    if (!tempDecorList.Contains(currFixture)) //make sure item is not already in our list
                    {
                        tempDecorList.Add(currFixture);
                        AllFixturesList.RemoveAt(ind);
                        validIndexFound = true;
                    }
                }
            }

            DecorList = new List<ShopItem>();

            //garden expansion:
            if (currGardenExpansion <= 3)
            {
                //add garden expansion to list
                ShopItem gardenExp = new ShopItem();
                gardenExp.sprite = null; //there is no sprite associated with garden expansion -- just display name "garden expansion"
                gardenExp.quantity = 1;
                gardenExp.name = "Garden Expansion";
                gardenExp.baseCost = 100; //TODO: manually update here once design team figures out prices
                DecorList.Add(gardenExp);
            }

            //generate decor list -- convert tempdecorlist to decorlist -- for each item, set values and add to decorlist
            for (int i = 0; i < tempDecorList.Count; i++)
            {
                Decor decor = tempDecorList[i];
                ShopItem item = new ShopItem();
                item.sprite = collections.GetSprite(decor.itemID);
                item.quantity = 1;
                item.name = decor.itemName;
                item.baseCost = collections.GetItem(decor.itemID).baseCost; //change when design team figures out costs
                item.myItem = new Decor((ushort)decor.itemID, decor.itemName);
                DecorList.Add(item);
            }

            return DecorList;
        }

        /// <summary>
        /// Call this function whenever the player buys a garden expansion. Updates so that the next cycle will have the next garden expansion,
        /// or if all expansions have been unlocked the slot will be replaced by a fixture.
        /// </summary>
        /// <param name="item"></param>
        public void UnlockGardenExpansion()
        {
            currGardenExpansion++;
            //TODO: is there anywhere to update player's info for garden expansion?
        }

        /// <summary>
        /// Get the selling price of an item in player's inventory in the decor shop
        /// </summary>
        /// <returns></returns>
        public int GetSellingPriceById(IInventoryItem item)
        {
            if (item.itemID >= 4000 && item.itemID <= 4100)
            {
                ItemProperties itemProps = collections.GetItem((ushort)item.itemID);
                //TODO: update selling price once the design team figures it out

                return (int)(itemProps.baseCost / 2);
            }
            //can only sell decor in the decor shop
            else return 0;
        }

        /// <summary>
        /// Returns true if item is a decor item, otherwise returns false
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsSellable(IInventoryItem item)
        {
            //can only sell decor items
            if (item.itemType == ItemType.Decor)
            {
                return true;
            }
            else return false;
        }
    }
}