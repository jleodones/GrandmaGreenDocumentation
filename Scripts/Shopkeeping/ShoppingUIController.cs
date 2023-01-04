using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using Sirenix.OdinInspector;
using Codice.CM.Common.Merge;

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

        int currCycle; //after the 4th cycle, reset the gardening controller. increment each cycle.

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
            AllSeedsList = collections.PlantGenotypeMasterList;

            currCycle = 1;
            //initial ratios
            numFlowers = 3;
            numVeggies = 3;
            numFruits = 2;

            //GardenList = new List<ShopItem>();
            //use CollectionsSO GetItem(Id) to retrieve ItemProperties of each item, set each attribute
            //2001-21 seed ids
            //look at list of seed items in collectionsSO and randomly pick 3 seed types per flower/veggie/fruit
            //int flowerSeed1, flowerSeed2, flowerSeed3;
            //int veggieSeed1, veggieSeed2, veggieSeed3;
            //int fruitSeed1, fruitSeed2, fruitSeed3;

            //Seeds:
            //for (int i = 1001; i <= 1009; i++)
            //{
            //    ushort id = (ushort) i;
            //    ItemProperties thisItem = collectionsinput.GetItem(id); //essentially getting the plant

            //    ShopItem itemProps = new ShopItem();
            //    itemProps.quantity = 3;
            //    itemProps.name = thisItem.name;

            //    // Get genotype of seed.
            //    // TODO: Change this so it "randomly" picks a genotype for the seed packets.
            //    // For now this defaults to heterozygous plant.
            //    Genotype myGenotype = new Genotype("AaBb");

            //    //Determine the mega value based on genotype
            //    string genoString = myGenotype.ToString();

            //    itemProps.megaValue = GetMegaValue(genoString);

            //    itemProps.baseCost = thisItem.baseCost * itemProps.megaValue;

            //    // Get sprite.
            //    itemProps.sprite = collectionsinput.GetSprite((PlantId)i, myGenotype);
            //    itemProps.myItem = new Seed((ushort)i, thisItem.name, myGenotype);
            //    GardenList.Add(itemProps);
            //}
        }

        /// <summary>
        /// Each new cycle, call this function to update the gardening shopkeeping system's cycle count
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
                    genotype = plant.genotypes[0];
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
            //else return collections.GetItem(item.itemID).baseCost / 2;

            //can only sell plants and seeds in the garden shop
            else return 0;
        }

    }

    /// <summary>
    /// Creates List of each item that will show up this cycle for the Decor Shop UI to pull from
    /// </summary>
    public class DecorShopUIController
    {
        List<ShopItem> DecorList;
        CollectionsSO collections;

        /// <summary>
        /// Pass in the collections SO. List will need to retrieve base costs of each item from the collections
        /// </summary>
        /// <param name="collections"></param>
        public DecorShopUIController(CollectionsSO collections)
        {
            DecorList = new List<ShopItem>();

            //Decor (non garden expansion):
            for (int i = 4001; i <= 4100; i++)
            {
                DecorationId id = (DecorationId)i;
                ShopItem itemProps = new ShopItem();
                itemProps.quantity = 1;
                // itemProps.id = id;
                DecorList.Add(itemProps);
            }
            //Garden Exp:
            //uhh idk yet

        }

        /// <summary>
        /// Get List containing all items in the decor shop this cycle
        /// </summary>
        /// <returns></returns>
        public List<ShopItem> GetDecorList()
        {
            return DecorList;
        }

        /// <summary>
        /// Get the base cost of an item in the decor shop, pass in the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetBaseCostById(ushort id)
        {
            return collections.GetItem(id).baseCost;
        }

        /// <summary>
        /// Get the selling price of an item in player's inventory in the decor shop (for now is just half the base cost)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetSellingPriceById(ushort id)
        {
            return collections.GetItem(id).baseCost / 2;
        }
    }
}