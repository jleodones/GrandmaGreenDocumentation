using GrandmaGreen.Garden;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GrandmaGreen.Collections
{
    //TODO: start from 0
    public enum ToolId : ushort
    {
        Trowel = 3001,
        WateringCan = 3002,
        SeedPacket = 3003,
        UpgradedTrowel = 3004,
        UpgradedWateringCan = 3005,
        Fertilizer = 3006
    }

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

    public enum PlantType : ushort
    {
        Flower = 1,
        Veggie = 2,
        Fruit = 3
    }

    //TODO: Needs item type
    //for anything in inventory
    public struct ItemProperties
    {
        public string name;
        public string description;
        public string spritePath;
        //decor items
        public string tag;
        public int baseCost;
    }

    public struct CharacterProperties
    {
        public string name;
        public string description;
        public List<string> spritePaths;
    }

    //TODO: expand with all stats for plants (based on plant data sheet)
    public struct PlantProperties
    {
        public string name;
        public string description;
        public int growthStages;
        public int growthTime;
        public int waterPerStage;
        public string spriteBasePath;
        public PlantType plantType;
    }

    //for shop
    public struct SeedProperties
    {
        public string name;
        public string description;
        public string spritePath;
        public int baseCost;

        //in this context, plantType is flower, veggie, or fruit
        public PlantType plantType;
    }
}

namespace GrandmaGreen.Collections
{
    using ID = System.Enum;
    [CreateAssetMenu(fileName = "New Collections SO")]
    ///<summary>
    ///Template to generate the Collections SO, so that it will contain a list of Items
    ///</summary>
    public class CollectionsSO : ScriptableObject
    {
        [SerializeField] TextAsset dataSheet;
        [ShowInInspector]
        public Dictionary<ushort, ItemProperties> ItemLookup;
        public Dictionary<ushort, PlantProperties> PlantLookup;
        public Dictionary<ushort, CharacterProperties> CharacterLookup;
        public Dictionary<ushort, SeedProperties> SeedLookup;

        static CollectionsSO s_Instance;
        public CollectionsSO LoadedInstance => s_Instance;
        public void LoadCollections()
        {
            GenerateCollections();
            s_Instance = this;
        }

        public void UnloadCollections()
        {
            s_Instance = null;
        }

        [Button()]
        public void GenerateCollections()
        {
            CSVtoSO.GenerateCollectionsSO(this, dataSheet);
        }

        public bool PlantToGolem(PlantId id, out CharacterId golemID)
        {
            golemID = default;

            switch (id)
            {
                case PlantId.Tulip:
                    golemID = CharacterId.TulipGolem;
                    return true;
                case PlantId.Crocus:
                    golemID = CharacterId.CrocusGolem;
                    return true;
                case PlantId.Pumpkin:
                    golemID = CharacterId.PumpkinGolem;
                    return true;
                case PlantId.Apple:
                    golemID = CharacterId.AppleGolem;
                    return true;
                case PlantId.Radish:
                    golemID = CharacterId.RadishGolem;
                    return true;
                default:
                    return false;
            }

        }

        ///<summary>
        ///Get any item by its id
        ///</summary>
        public ItemProperties GetItem(ID id)
        {
            return ItemLookup[(ushort)Convert.ToInt32(id)];
        }

        ///<summary>
        ///Get any plant by its id
        ///</summary>
        public PlantProperties GetPlant(PlantId id)
        {
            return PlantLookup[(ushort)id];
        }

        ///<summary>
        ///Get any plant by its id
        ///</summary>
        public SeedProperties GetSeed(PlantId id)
        {
            return SeedLookup[(ushort)((ushort)id + CSVtoSO.SEED_ID_OFFSET)];
        }

        ///<summary>
        ///Retrieve a sprite by its sprite path (which is just its filename)
        ///</summary>
        public Sprite GetSprite(string spritePath)
        {
            return Resources.Load(spritePath, typeof(Sprite)) as Sprite;
        }

        ///<summary>
        ///Retrieve a sprite by its sprite path (which is just its filename)
        ///</summary>
        ///
        public Sprite GetSprite(ID id)
        {
            return Resources.Load(GetItem(id).spritePath, typeof(Sprite)) as Sprite;
        }

        /// <summary>
        ///// TODO: use string builder for this
        /// TODO: checks for single sprite vs spritesheet
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genotype"></param>
        /// <param name="growthStage"></param>
        /// <returns></returns>
        public Sprite GetSprite(PlantId type, Genotype genotype, int growthStage)
        {
            PlantProperties plant = GetPlant(type);

            Sprite[] plantSpriteSheet = Resources.LoadAll<Sprite>(plant.plantType.ToString() + "s/" + plant.spriteBasePath);

            if (plantSpriteSheet.Length == 0)
            {
                Debug.Log("Spritesheet not found");
                return null;
            }

            string suffix = "";

            switch (growthStage)
            {
                case 0:
                    suffix = "_Seedling";
                    break;
                case 1:
                    suffix = "_Growing";
                    break;
                case 2:
                    suffix = "_Mature";
                    switch (genotype.trait)
                    {
                        case Genotype.Trait.Recessive:
                            suffix += "_bb";
                            break;
                        case Genotype.Trait.Heterozygous:
                            suffix += "_Bb";
                            break;
                        case Genotype.Trait.Dominant:
                            suffix += "_BB";
                            break;
                    }
                    break;
            }

            string finalSpritePath = plant.spriteBasePath + suffix;
            return plantSpriteSheet.Single(s => s.name == finalSpritePath);
        }

        // temporary hard-coded plant properties
        // to be swapped with properties in CSV
        [ContextMenu("DEBUGLoadPlantProperties")]
        public void DEBUGLoadPlantProperties()
        {
            PlantProperties roseProp = PlantLookup[(ushort)PlantId.Rose];
            roseProp.growthStages = 3;
            roseProp.growthTime = 10;
            roseProp.waterPerStage = 1;

            PlantLookup[(ushort)PlantId.Rose] = roseProp;


            PlantProperties tulipProp = PlantLookup[(ushort)PlantId.Tulip];
            tulipProp.growthStages = 3;
            tulipProp.growthTime = 10;
            tulipProp.waterPerStage = 2;

            PlantLookup[(ushort)PlantId.Tulip] = tulipProp;
        }
    }
}
