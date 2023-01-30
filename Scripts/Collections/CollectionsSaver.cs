using GrandmaGreen.SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Garden;
using System;
using System.ComponentModel;
using static UnityEditor.Progress;

namespace GrandmaGreen.Collections
{
    public struct PlantCollectionProperties
    {
        public ushort id;
        public string name;
        public string description;
        public List<Sprite> sprites; //sprites for this genotype
        public bool matureUnlocked; //set to true once mature plant is harvested and added to inventory

        // Is this the same trait? Ie. same color, variation, etc. This is how we measure similarity.
        public Genotype.Trait trait;

        // The sizes that we have unlocked.
        public List<Genotype.Size> unlockedSizes;

        public override bool Equals(object obj) =>
            obj is PlantCollectionProperties other
            && id == other.id
            && trait == other.trait;
    }

    public struct FriendCollectionProperties
    {
        public string name;
        public Sprite illustration;
        public string description; // of the NPC’s personality and backstory
        public string location; //Golem: home or away. Shopkeepers: shop name
    }

    public struct AwardCollectionProperties
    {
        public Sprite picture;
        public string name; // (ex: 1st place)
        public string category; // (ex: Biggest Pumpkin)
        public int numTimesWon;
    }


    [CreateAssetMenu(menuName = "GrandmaGreen/Collections/CollectionsSaver")]
    public class CollectionsSaver : ObjectSaver
    {
        // [JsonIgnoreAttribute]
        [SerializeField]
        CollectionsSO collections;


        private readonly int plantCollectionsKey = 0;
        private readonly int friendCollectionsKey = 1;
        private readonly int awardCollectionsKey = 2;

        /// <summary>
        /// Pre-create component stores of appropriate types so we don't have to manually do it in the editor
        /// </summary>
        public void Initialize()
        {
            if (componentStores.Count == 0)
            {
                CreateNewStore(typeof(PlantCollectionProperties));
                CreateNewStore(typeof(FriendCollectionProperties));
                CreateNewStore(typeof(AwardCollectionProperties));
                return;
            }
        }

        //public T PlantSearch<T>(int itemID, Genotype.Trait trait) where T : struct
        //{
        //    // Iterates through component stores to find component store of appropriate type.
        //    foreach (ComponentStore<T> componentStore in componentStores)
        //    {
        //        if (componentStore.GetType() == typeof(T))
        //        {
        //            List<T> components = componentStore.components;
        //            foreach (T component in components)
        //            {
        //                if (component.Equals(itemID))
        //                {
        //                    return component;
        //                }
        //            }
        //        }
        //    }
        //    return default(T);
        //}

        /// <summary>
        /// Find a plant's info from the saved components based on its id and trait.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="trait"></param>
        /// <returns></returns>
        public PlantCollectionProperties PlantSearch(int itemID, Genotype.Trait trait)
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach (ComponentStore<PlantCollectionProperties> componentStore in componentStores)
            {
                if (componentStore.GetType() == typeof(PlantCollectionProperties))
                {
                    List<PlantCollectionProperties> components = componentStore.components;
                    foreach (PlantCollectionProperties component in components)
                    {
                        if (component.id == itemID && component.trait.Equals(trait))
                        {
                            return component;
                        }
                    }
                }
            }
            return default(PlantCollectionProperties);
        }

        /// <summary>
        /// Clear all saved data for plants, friends, and award collections
        /// </summary>
        public void Clear()
        {
            ((ComponentStore<PlantCollectionProperties>)componentStores[plantCollectionsKey]).components.Clear();
            ((ComponentStore<FriendCollectionProperties>)componentStores[friendCollectionsKey]).components.Clear();
            ((ComponentStore<AwardCollectionProperties>)componentStores[awardCollectionsKey]).components.Clear();
        }

        /// <summary>
        /// Check if there exists a plant with the given properties in the saved components
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool PlantContainsKey(PlantCollectionProperties k)
        {
            return ((ComponentStore<PlantCollectionProperties>)componentStores[plantCollectionsKey]).components.Contains(k);
        }

        /// <summary>
        /// Check if there exists a friend with the given properties in the saved components
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool FriendContainsKey(FriendCollectionProperties k)
        {
            return ((ComponentStore<FriendCollectionProperties>)componentStores[friendCollectionsKey]).components.Contains(k);
        }

        /// <summary>
        /// Check if there exists an award with the given properties in the saved components
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool AwardContainsKey(AwardCollectionProperties k)
        {
            return ((ComponentStore<AwardCollectionProperties>)componentStores[awardCollectionsKey]).components.Contains(k);
        }

        /// <summary>
        /// Return the list of plant collection entries
        /// </summary>
        /// <returns></returns>
        public List<PlantCollectionProperties> PlantValues()
        {
            return ((ComponentStore<PlantCollectionProperties>)componentStores[plantCollectionsKey]).components;
        }

        /// <summary>
        /// Return the list of friend collection entries
        /// </summary>
        /// <returns></returns>
        public List<FriendCollectionProperties> FriendValues()
        {
            return ((ComponentStore<FriendCollectionProperties>)componentStores[friendCollectionsKey]).components;
        }

        /// <summary>
        /// Return the list of award collection entries
        /// </summary>
        /// <returns></returns>
        public List<AwardCollectionProperties> AwardValues()
        {
            return ((ComponentStore<AwardCollectionProperties>)componentStores[awardCollectionsKey]).components;
        }

        /// <summary>
        /// Call whenever an item is added to the inventory. Will add to or update collections checking if it's
        /// a seed or plant, whether its a new genotype, adds to harvest count and adds the sprite if it's mature
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePlantCollections(IInventoryItem item)
        {
            PlantCollectionProperties props = new PlantCollectionProperties();
            props.id = item.itemID;
            if(item.itemType == ItemType.Seed)
            {
                Seed seed = (Seed)item;
                props.trait = seed.seedGenotype.trait; //trait is what differs plants of the same id
            }
            else if(item.itemType == ItemType.Plant)
            {
                Plant plant = (Plant)item;
                props.trait = plant.plantGenotype.trait;
            }

            // If it finds the data within the component store.
            if (RequestData<PlantCollectionProperties>(-1, ref props))
            {
                // Make any changes to props here.
                if (item.itemType == ItemType.Seed) //check if new genotype
                {
                    Seed seed = (Seed)item;
                    if(!props.unlockedSizes.Contains(seed.seedGenotype.size))
                    {
                        props.unlockedSizes.Add(seed.seedGenotype.size);
                    }
                }
                if (item.itemType == ItemType.Plant)
                {
                    Plant plant = (Plant)item;
                    props.matureUnlocked = true;
                }

                // Update props after adjusting it.
                UpdateValue<PlantCollectionProperties>(-1, props);

            }
            else // If it doesn't find the data within the component store, ie. it doesn't exist yet.
            {
                // Add it as a seed.
                Seed seed = (Seed)item;
                props.name = seed.itemName;
                props.description = collections.GetItem(props.id).description;
                props.sprites = new List<Sprite>();
                props.sprites.Add(collections.GetSprite((PlantId)props.id, seed.seedGenotype)); //seed packet
                for (int i = 0; i < 3; i++) //growth stages sprites
                {
                    props.sprites.Add(collections.GetSprite((PlantId)props.id, seed.seedGenotype, i)); //growth stage seedling, growing, mature
                }
                props.matureUnlocked = false; //if harvested is 0, only seed packet has been unlocked. otherwise mature has been unlocked.
                //props.genotypes = new List<Genotype>();
                //props.genotypes.Add(seed.seedGenotype);
                props.unlockedSizes = new List<Genotype.Size>();
                props.unlockedSizes.Add(seed.seedGenotype.size);
                props.trait = seed.seedGenotype.trait;

                // Finally, add it to the component store.
                AddComponent<PlantCollectionProperties>(-1, props);
            }
        }

    }
}