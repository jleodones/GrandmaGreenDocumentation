using GrandmaGreen.Garden;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections
{
    public enum PlantId : ushort
    {
        Rose = 1001,
        Tulip = 1002,
        CallaLily = 1003,
        Dahlia = 1004,
        Hyacinth = 1005,
        Pansy = 1006,
        Crocus = 1007,
        Cucumber = 1008,
        Tomato = 1009,
        Pepper = 1010,
        SweetPotato = 1011,
        Corn = 1012,
        Radish = 1013,
        Pumpkin = 1014,
        Apple = 1015,
        Pear = 1016,
        Watermelon = 1017,
        Cherry = 1018,
        Plum = 1019,
        Peach = 1020,
        Blueberry = 1021
    }

    public enum SeedId : ushort
    {
        Rose = 2001,
        Tulip = 2002,
        CallaLily = 2003,
        Dahlia = 2004,
        Hyacinth = 2005,
        Pansy = 2006,
        Crocus = 2007,
        Cucumber = 2008,
        Tomato = 2009,
        Pepper = 2010,
        SweetPotato = 2011,
        Corn = 2012,
        Radish = 2013,
        Pumpkin = 2014,
        Apple = 2015,
        Pear = 2016,
        Watermelon = 2017,
        Cherry = 2018,
        Plum = 2019,
        Peach = 2020,
        Blueberry = 2021
    }

    public enum ToolId : ushort
    {
        Trowel = 3001,
        WateringCan = 3002,
        SeedPacket = 3003,
        UpgradedTrowel = 3004,
        UpgradedWateringCan = 3005,
        Fertilizer = 3006
    }

    public enum DecorationId : ushort
    {
        Torch = 4001,
        TulipGolemMemento = 4002,
        AppleGolemMemento = 4003,
        CrocusGolemMemento = 4004,
        RadishGolemMemento = 4005,
        PumpkinGolemMemento = 4006,
        CottonGolemMemento = 4007,
        CampingLantern = 4008,
        SimpleHangingLampPost = 4009,
        JackOLantern = 4010,
        IronLampPost = 4011,
        JapaneseGardenLantern = 4012,
        MushroomLamp = 4013,
        Scarecrow = 4014,
        Bicycle = 4015,
        FlowerWagon = 4016,
        HarvestBasket = 4017,
        Umbrella = 4018,
        Swingset = 4019,
        Leafpile = 4020,
        PottedPlant01 = 4021,
        PottedPlant02 = 4022,
        PottedPlant03 = 4023,
        FrenchCountryUrn = 4024,
        GardenGnomeBasic = 4025,
        GardenGnomeSexy = 4026,
        GardenGnomeGold = 4027,
        FrogSculpture = 4028,
        Flamingo = 4029,
        ArtNouveauSculture01 = 4030,
        ArtNouveauSculture02 = 4031,
        ArtNouveauSculture03 = 4032,
        SimpleWoodenBench = 4033,
        SimpleWoodenChair = 4034,
        StoneBench = 4035,
        IronBench = 4036,
        IronChair = 4037,
        RockingChair = 4038,
        MushroomStool = 4039,
        PicnicSpread = 4040,
        SimpleWoodenTable = 4041,
        FlowerTable = 4042,
        IronTable = 4043,
        BirdBath = 4044,
        NaturalPond = 4045,
        FancyPond = 4046,
        KoiPond = 4047,
        SimpleFountain = 4048,
        FancyFountain = 4049,
        MagicFountain = 4050,
        Well = 4051,
        Waterpump = 4052,
        GrecianRuin = 4053,
        Sundial = 4054,
        ClaudeMonetPainting = 4055,
        JohnSingerSargentPainting = 4056,
        MailboxBasic = 4057,
        MailboxCottagecore = 4058,
        MailboxGoth = 4059,
        MailboxFairy = 4060,
        MailboxOriental = 4061,
        MailboxCabin = 4062,
        MailboxPastel = 4063,
        WhitePicketFence = 4064,
        SimpleWoodenFence = 4065,
        FarmFence = 4066,
        FancyIronFence = 4067,
        FieldstoneFence = 4068,
        IvyCoveredBrickFence = 4069,
        HistoricalEnglishExterior = 4070,
        FrenchProvincialExterior = 4071,
        PNWCabinExterior = 4072,
        TraditionalJapaneseExterior = 4073,
        ModernExterior = 4074,
        AcornExterior = 4075,
        PastelExterior = 4076,
        WitchyGothExterior = 4077,
        FairyMagicExterior = 4078,
        EuropeanCastleExterior = 4079,
        DirtPath = 4080,
        CobblestonePath = 4081,
        TilePath = 4082,
        WoodPath = 4083,
        OakTree = 4084,
        PineTree = 4085,
        WillowTree = 4086,
        PalmTree = 4087,
        MapleTree = 4088,
        PoplarTree = 4089,
        StumpTree = 4090,
        HydrangeaBush = 4091,
        OleanderBush = 4092,
        GardeniaBush = 4093,
        BoxwoodBush = 4094,
        CommonPoppy = 4095,
        ForgetMeNot = 4096,
        WildDaisies = 4097,
        Dandelion = 4098,
        Clover = 4099,
        Hemlock = 4100
    }

    public enum CharacterId : ushort
    {
        Grandma = 5001,
        GardeningShopkeeper = 5002,
        DecorShopkeeper = 5003,
        TulipGolem = 5004,
        AppleGolem = 5005,
        CottonGolem = 5006,
        CrocusGolem = 5007,
        PumpkinGolem = 5008,
        RadishGolem = 5009
    }

    public enum SeedType : ushort
    {
        Flower = 1,
        Veggie = 2,
        Fruit = 3
    }

    //for anything in inventory
    public struct ItemProperties
    {
        public string name;
        public string description;
        public string spritePath;
        //decor items
        public string tag;
        public int baseCost;
        //set cooldown to specified #cycles until it can appear again. 0 by default.
        public int cycleCooldown;
    }

    public struct CharacterProperties
    {
        public string name;
        public string description;
        public List<string> spritePaths;
    }

    public struct PlantProperties
    {
        public string name;
        public string description;
        public int growthStages;
        public int growthTime;
        public int waterPerStage;
        public string spriteBasePath;
    }
    //for shop
    public struct SeedProperties
    {
        public string name;
        public string description;
        public string spritePath;
        public int baseCost;
        public int cycleCooldown;
        //in this context, seedtype is flower, veggie, or fruit
        public SeedType seedType;
    }
}

namespace GrandmaGreen.Collections
{
    using Id = System.Enum;
    [CreateAssetMenu(fileName = "New Collections SO")]
    ///<summary>
    ///Template to generate the Collections SO, so that it will contain a list of Items
    ///</summary>
    public class CollectionsSO : ScriptableObject
    {
        public Dictionary<Id, ItemProperties> ItemLookup;
        public Dictionary<PlantId, PlantProperties> PlantLookup;
        public Dictionary<CharacterId, CharacterProperties> CharacterLookup;
        public Dictionary<SeedId, SeedProperties> SeedLookup;
        public Dictionary<string, Sprite> SpriteCache;

        ///<summary>
        ///Get any item by its id
        ///</summary>
        public ItemProperties GetItem(Id id)
        {
            return ItemLookup[id];
        }

        ///<summary>
        ///Get any plant by its id
        ///</summary>
        public PlantProperties GetPlant(PlantId id)
        {
            return PlantLookup[id];
        }

        ///<summary>
        ///Retrieve a sprite by its sprite path (which is just its filename)
        ///</summary>
        public Sprite GetSprite(string spritePath)
        {
            return Resources.Load(spritePath, typeof(Sprite)) as Sprite;
        }

        ///<summary>
        ///Retrieve item name by its id
        ///</summary>
        public string GetItemName(Id id)
        {
            return ItemLookup[id].name;
        }

        public PlantId SeedToPlant(SeedId id)
        {
            switch (id)
            {
                case SeedId.Rose:
                    return PlantId.Rose;
                case SeedId.Tulip:
                    return PlantId.Tulip;
                default:
                    return 0;
            }
        }

        public SeedId PlantToSeed(PlantId id)
        {
            switch (id)
            {
                case PlantId.Rose:
                    return SeedId.Rose;
                case PlantId.Tulip:
                    return SeedId.Tulip;
                default:
                    return 0;
            }
        }

        public CharacterId PlantToGolem(PlantId id)
        {
            switch (id)
            {
                case PlantId.Tulip:
                    return CharacterId.TulipGolem;
                default:
                    return 0;
            }
        }

        // TODO: resolve sprites by modifying base path
        //       instead of these hard-coded references
        [SerializeField] Sprite rose_seedling;
        [SerializeField] Sprite rose_growing;
        [SerializeField] Sprite rose_bb;
        [SerializeField] Sprite rose_Bb;
        [SerializeField] Sprite rose_BB;
        [SerializeField] Sprite tulip_seedling;
        [SerializeField] Sprite tulip_growing;
        [SerializeField] Sprite tulip_bb;
        [SerializeField] Sprite tulip_Bb;
        [SerializeField] Sprite tulip_BB;

        public Sprite GetSprite(PlantId type, Genotype genotype, int growthStage)
        {
            if (type == PlantId.Rose)
            {
                if (growthStage == 0) return rose_seedling;
                else if (growthStage == 1) return rose_growing;
                else
                {
                    switch (genotype.trait)
                    {
                        case Genotype.Trait.Recessive:
                            return rose_bb;
                        case Genotype.Trait.Heterozygous:
                            return rose_Bb;
                        case Genotype.Trait.Dominant:
                            return rose_BB;
                    }
                }
            }
            else if (type == PlantId.Tulip)
            {
                if (growthStage == 0) return tulip_seedling;
                else if (growthStage == 1) return tulip_growing;
                else
                {
                    switch (genotype.trait)
                    {
                        case Genotype.Trait.Recessive:
                            return tulip_bb;
                        case Genotype.Trait.Heterozygous:
                            return tulip_Bb;
                        case Genotype.Trait.Dominant:
                            return tulip_BB;
                    }
                }
            }

            return null;
        }

        // temporary hard-coded plant properties
        // to be swapped with properties in CSV
        [ContextMenu("DEBUGLoadPlantProperties")]
        public void DEBUGLoadPlantProperties()
        {
            PlantLookup = new Dictionary<PlantId, PlantProperties>()
            {
                {
                    PlantId.Rose, new PlantProperties
                    {
                        name = "Rose",
                        growthStages = 3, // # of prefabs
                        growthTime = 10, // Probably can delete
                        waterPerStage = 1, // # of times it needs to be watered per stage
                    }
                },
                {
                    PlantId.Tulip, new PlantProperties
                    {
                        name = "Tulip",
                        growthStages = 3,
                        growthTime = 10,
                        waterPerStage = 2,
                    }
                }
            };
        }
    }
}
