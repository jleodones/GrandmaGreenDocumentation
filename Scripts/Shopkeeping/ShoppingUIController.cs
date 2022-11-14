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

    [Serializable]
    public struct ShopItem
    {
        public int baseCost;

        public ItemType type;
        //quantity 3 for seeds, 1 for tools, etc
        //test
        public string spritePath;
        public Sprite sprite;
        public string itemName;
        public ushort id;
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
                Enum id = (PlantId)i;
                ShopItem itemProps = new ShopItem();
                itemProps.baseCost = collections.GetItem(id).baseCost;
                itemProps.type = ItemType.Seed;
                itemProps.spritePath = collections.GetItem(id).spritePath;
                itemProps.sprite = collections.GetSprite(id);
                itemProps.itemName = collections.GetItem(id).name;
                itemProps.id = (ushort)i;
                GardenList.Add(id, itemProps);
            }

            //Tools:
            for (int i = 3001; i <= 3006; i++)
            {
                if (i != 3003)
                {
                    Enum id = (ToolId)i;
                    ShopItem itemProps = new ShopItem();
                    itemProps.baseCost = collections.GetItem(id).baseCost;
                    itemProps.type = ItemType.Tool;
                    itemProps.spritePath = collections.GetItem(id).spritePath;
                    itemProps.sprite = collections.GetSprite(id);
                    itemProps.itemName = collections.GetItem(id).name;
                    itemProps.id = (ushort)i;
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
        // /// <summary>
        // /// Get the item type of a gardening shop item, pass in id
        // /// </summary>
        // public string GetTypeById(Id id)
        // {
        //     return GardenList[id].type;
        // }

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
                itemProps.type = ItemType.Decor;
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
        // /// <summary>
        // /// Get the item type of a decor shop item, pass in id
        // /// </summary>
        // public string GetTypeById(Id id)
        // {
        //     return DecorList[id].type;
        // }

    }
}