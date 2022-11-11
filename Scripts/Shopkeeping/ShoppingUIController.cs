using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using Sirenix.OdinInspector;

namespace GrandmaGreen.Shopkeeping
{
    using Id = System.Enum;

    public struct ShopItem
    {
        public int baseCost;
        public string type;
        //quantity 3 for seeds, 1 for tools, etc
        public int quantity;
        //test
        public string spritePath;
    }

    /// <summary>
    /// Creates Dictionary<Id, ShopItem> of each item that will show up this cycle for the Gardening Shop UI to pull from
    /// </summary>
    public class GardeningShopUIController
    {
        Dictionary<Id, ShopItem> GardenList;
        CollectionsSO collections;
        /// <summary>
        /// Pass in the collections SO. List will need to retrieve base costs of each item from the collections
        /// </summary>
        public GardeningShopUIController(CollectionsSO collectionsinput)
        {
            collections = collectionsinput;
            GardenList = new Dictionary<Id, ShopItem>();
            //use CollectionsSO GetItem(Id) to retrieve ItemProperties of each item, set each attribute
            //2001-21 seed ids
            //look at list of seed items in collectionsSO and randomly pick 3 seed types per flower/veggie/fruit
            int flowerSeed1, flowerSeed2, flowerSeed3;
            int veggieSeed1, veggieSeed2, veggieSeed3;
            int fruitSeed1, fruitSeed2, fruitSeed3;
            //Dictionary<SeedId, SeedProperties> SeedLookup = collections.SeedLookup;
            // Random rnd = new Random();
            // flowerSeed1 = rnd.Next(2001,2021);

            //Seeds:
            for (int i = 2001; i <= 2009; i++)
            {
                SeedId id = (SeedId)i;
                ShopItem itemProps = new ShopItem();
                itemProps.baseCost = collections.GetItem(id).baseCost;
                itemProps.type = "Seed";
                itemProps.quantity = 3;
                itemProps.spritePath = collections.GetItem(id).spritePath;
                GardenList.Add(id, itemProps);
            }

            //Tools:
            for (int i = 3001; i <= 3006; i++)
            {
                if (i != 3003)
                {
                    ToolId id = (ToolId)i;
                    ShopItem itemProps = new ShopItem();
                    itemProps.baseCost = collections.GetItem(id).baseCost;
                    itemProps.type = "Tool";
                    itemProps.quantity = 1;
                    itemProps.spritePath = collections.GetItem(id).spritePath;
                    GardenList.Add(id, itemProps);
                }
            }
        }
        /// <summary>
        /// Get Dictionary<Id, ShopItem> containing all items in the gardening shop this cycle
        /// </summary>
        public Dictionary<Id, ShopItem> GetGardenList()
        {
            return GardenList;
        }
        /// <summary>
        /// Get the base cost of an item in the gardening shop, pass in the id
        /// </summary>
        public int GetBaseCostById(Id id)
        {
            return GardenList[id].baseCost;
        }
        /// <summary>
        /// Get the selling price of an item in player's inventory in the gardening shop (for now is just half the base cost)
        /// </summary>
        public int GetSellingPriceById(Id id)
        {
            return collections.GetItem(id).baseCost / 2;
        }
        /// <summary>
        /// Get the item type of a gardening shop item, pass in id
        /// </summary>
        public string GetTypeById(Id id)
        {
            return GardenList[id].type;
        }

        //test
        public string GetPathById(Id id)
        {
            return GardenList[id].spritePath;
        }

    }
    /// <summary>
    /// Creates Dictionary<Id, ShopItem> of each item that will show up this cycle for the Decor Shop UI to pull from
    /// </summary>
    public class DecorShopUIController
    {
        Dictionary<Id, ShopItem> DecorList;
        CollectionsSO collections;
        /// <summary>
        /// Pass in the collections SO. List will need to retrieve base costs of each item from the collections
        /// </summary>
        public DecorShopUIController(CollectionsSO collections)
        {
            DecorList = new Dictionary<Id, ShopItem>();

            //use CollectionsSO GetItem(Id) to retrieve ItemProperties of each item, set each attribute

            //Decor (non garden expansion):
            for (int i = 4001; i <= 4100; i++)
            {
                DecorationId id = (DecorationId)i;
                ShopItem itemProps = new ShopItem();
                itemProps.baseCost = collections.GetItem(id).baseCost;
                itemProps.type = "Decor";
                itemProps.quantity = 1;
                DecorList.Add(id, itemProps);
            }
            //Garden Exp:
            //uhh idk yet

        }
        /// <summary>
        /// Get Dictionary<Id, ShopItem> containing all items in the decor shop this cycle
        /// </summary>
        public Dictionary<Id, ShopItem> GetDecorList()
        {
            return DecorList;
        }
        /// <summary>
        /// Get the base cost of an item in the decor shop, pass in the id
        /// </summary>
        public int GetBaseCostById(Id id)
        {
            return DecorList[id].baseCost;
        }
        /// <summary>
        /// Get the selling price of an item in player's inventory in the decor shop (for now is just half the base cost)
        /// </summary>
        public int GetSellingPriceById(Id id)
        {
            return collections.GetItem(id).baseCost / 2;
        }
        /// <summary>
        /// Get the item type of a decor shop item, pass in id
        /// </summary>
        public string GetTypeById(Id id)
        {
            return DecorList[id].type;
        }

    }
}