using GrandmaGreen.Garden;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace GrandmaGreen.Collections
{
    public enum ToolId : ushort
    {
        Trowel = 1,
        WateringCan = 2,
        SeedPacket = 3,
        UpgradedTrowel = 4,
        UpgradedWateringCan = 5,
        Fertilizer = 6
    }

    public enum FlowerId : ushort
    {
        Rose = 1001,
        Tulip = 1002,
        CallaLily = 1003,
        Dahlia = 1004,
        Hyacinth = 1005,
        Pansy = 1006,
        Crocus = 1007
    }

    public enum VegetableId : ushort
    {
        Cucumber = 1008,
        Tomato = 1009,
        Pepper = 1010,
        SweetPotato = 1011,
        Corn = 1012,
        Turnip = 1013,
        Pumpkin = 1014
    }

    public enum FruitId : ushort
    {
        Apple = 1015,
        Pear = 1016,
        Watermelon = 1017,
        Cherry = 1018,
        Plum = 1019,
        Peach = 1020,
        Blueberry = 1021
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
        Turnip = 1013,
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
        TurnipGolem = 5009
    }

    public enum DecorationId : ushort
    {
        Torch = 4001,
        TulipGolemMemento = 4002,
        AppleGolemMemento = 4003,
        CrocusGolemMemento = 4004,
        TurnipGolemMemento = 4005,
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
        Vegetable = 2,
        Fruit = 3
    }

    public struct ItemProperties
    {
        public string name;
        public string description;
        public string spritePath;
        public string itemType;
        public int baseCost;
        //decor items
        public string tag;
        public string decorType;
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
        public string spriteBasePath;
        public int growthStages; //always going to be 3
        public int growthTime;
        public int waterPerStage; //always going to be 200
        public float baseGoldPerTimeUnit;
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
    public class CollectionsSO : SerializedScriptableObject
    {
        [SerializeField] TextAsset dataSheet;
        [ShowInInspector]
        public Dictionary<ushort, ItemProperties> ItemLookup;
        public List<Seed> PlantGenotypeMasterList;
        public List<Decor> DecorList;
        public List<Decor> FixtureList; //for cycle tracking purposes
        public Dictionary<ushort, PlantProperties> PlantLookup;

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

        public bool IsFlower(PlantId id)
        {
            return Array.Exists<PlantId>((PlantId[])Enum.GetValues(typeof(FlowerId)), element => element == id);    
        }

        public bool IsVegetable(PlantId id)
        {
            return Array.Exists<PlantId>((PlantId[])Enum.GetValues(typeof(VegetableId)), element => element == id);
        }

        public bool IsFruit(PlantId id)
        {
            return Array.Exists<PlantId>((PlantId[])Enum.GetValues(typeof(FruitId)), element => element == id);
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
                    return false;
                case PlantId.Pumpkin:
                    golemID = CharacterId.PumpkinGolem;
                    return true;
                case PlantId.Apple:
                    golemID = CharacterId.AppleGolem;
                    return false;
                case PlantId.Turnip:
                    golemID = CharacterId.TurnipGolem;
                    return false;
                default:
                    return false;
            }

        }

        ///<summary>
        ///Get any item by its id
        ///</summary>
        public ItemProperties GetItem(ushort id)
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
        public Sprite GetSprite(ushort id)
        {
            ItemProperties item = GetItem(id);
            return Resources.Load(GetItem(id).spritePath, typeof(Sprite)) as Sprite;
        }

        /// <summary>
        /// TODO: checks for single sprite vs spritesheet
        /// Only for plant sprites
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genotype"></param>
        /// <param name="growthStage"></param>
        /// <returns></returns>
        public Sprite GetSprite(PlantId type, Genotype genotype, int growthStage)
        {
            PlantProperties plant = GetPlant(type);
            bool isMega = genotype.generation == Genotype.Generation.F2;
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
                    if (IsFlower(type))
                    {
                        switch (genotype.trait)
                        {
                            case Genotype.Trait.Recessive:
                                suffix = isMega ? "_Rec_M" : "_Rec";
                                break;
                            case Genotype.Trait.Heterozygous:
                                suffix = "_Het";
                                break;
                            case Genotype.Trait.Dominant:
                                suffix = isMega ? "_Dom_M" : "_Dom";
                                break;
                        }
                    }
                    break;
            }

            StringBuilder finalSpritePath = new StringBuilder(plant.plantType.ToString(), 100);
            finalSpritePath.Append("s/" + plant.name + "/" + plant.spriteBasePath + suffix);
            return Resources.Load<Sprite>(finalSpritePath.ToString());
        }

        public Sprite GetInventorySprite(PlantId type, Genotype genotype)
        {
            PlantProperties plant = GetPlant(type);
            bool isMega = genotype.generation == Genotype.Generation.F2;
            string suffix = "";

            if (IsFlower(type))
            {
                switch (genotype.trait)
                {
                    case Genotype.Trait.Recessive:
                        suffix = isMega ? "_Rec_M" : "_Rec";
                        break;
                    case Genotype.Trait.Heterozygous:
                        suffix = "_Het";
                        break;
                    case Genotype.Trait.Dominant:
                        suffix = isMega ? "_Dom_M" : "_Dom";
                        break;
                }

                suffix += "_Inventory";
            }
            else if (IsFruit(type))
            {
                switch (genotype.trait)
                {
                    case Genotype.Trait.Recessive:
                        suffix = "_Rec";
                        break;
                    case Genotype.Trait.Heterozygous:
                        suffix = "_Het";
                        break;
                    case Genotype.Trait.Dominant:
                        suffix = "_Dom";
                        break;
                }
            }
            Sprite[] sheet = Resources.LoadAll<Sprite>(plant.plantType.ToString() + "s/" + plant.name + "/" + plant.spriteBasePath);
            Sprite seedSprite = sheet.Single(s => s.name == plant.spriteBasePath + suffix);
            return seedSprite;
        }

        /// <summary>
        /// Get child sprite for fruits and vegetables.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genotype"></param>
        /// <returns>Fruit or vegetable.</returns>
        public Sprite GetVegetableHead(PlantId type, Genotype genotype, int growthStage)
        {
            if (growthStage < 2) return null;
            PlantProperties plant = GetPlant(type);
            StringBuilder finalSpritePath = new StringBuilder(plant.plantType.ToString(), 100);
            finalSpritePath.Append("s/" + plant.name + "/" + plant.spriteBasePath + "_HeadLarge");
            return Resources.Load<Sprite>(finalSpritePath.ToString());
        }

        public Sprite GetFruitTree(PlantId type, Genotype genotype, int growthStage)
        {
            PlantProperties plant = GetPlant(type);
            Sprite[] sheet = Resources.LoadAll<Sprite>(plant.plantType.ToString() + "s/" + plant.name + "/" + plant.spriteBasePath);
            if (growthStage == 0) return sheet[5];
            else if (growthStage == 1) return sheet[1];
            else if (growthStage == 2) return sheet[3];
            return null;
        }

        public Sprite GetFruitFruit(PlantId type, Genotype genotype, int growthStage)
        {
            if (growthStage < 2) return null;
            PlantProperties plant = GetPlant(type);
            Sprite[] sheet = Resources.LoadAll<Sprite>(plant.plantType.ToString() + "s/" + plant.name + "/" + plant.spriteBasePath);
            if (genotype.trait == Genotype.Trait.Dominant) return sheet[0];
            else if (genotype.trait == Genotype.Trait.Heterozygous) return sheet[2];
            else if (genotype.trait == Genotype.Trait.Recessive) return sheet[4];
            return null;
        }

        /// <summary>
        /// Only for seed packets
        /// </summary>
        public Sprite GetSprite(PlantId type, Genotype genotype)
        {
            ItemProperties seed = GetItem((ushort)type);
            string baseSpritePath = "SEED_" + seed.name.Replace(" ", "");
            bool isMega = genotype.generation == Genotype.Generation.F2;
            string suffix = "";

            switch (genotype.trait)
            {
                case Genotype.Trait.Recessive:
                    suffix = isMega ? "_Rec_M" : "_Rec";
                    break;
                case Genotype.Trait.Heterozygous:
                    suffix = "_Het";
                    break;
                case Genotype.Trait.Dominant:
                    suffix = isMega ? "_Dom_M" : "_Dom";
                    break;
            }
            
            Sprite[] sheet = Resources.LoadAll<Sprite>("Seed Packets/" + baseSpritePath);

            Sprite seedSprite = sheet.Single(s => s.name == baseSpritePath + suffix);
            return seedSprite;
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
            
            PlantProperties callalily = PlantLookup[(ushort)PlantId.CallaLily];
            callalily.growthStages = 3;
            callalily.growthTime = 10;
            callalily.waterPerStage = 1;

            PlantLookup[(ushort)PlantId.CallaLily] = callalily;
            
            PlantProperties dahlia = PlantLookup[(ushort)PlantId.Dahlia];
            dahlia.growthStages = 3;
            dahlia.growthTime = 10;
            dahlia.waterPerStage = 1;

            PlantLookup[(ushort)PlantId.Dahlia] = dahlia;
            
            PlantProperties hyacinth = PlantLookup[(ushort)PlantId.Hyacinth];
            hyacinth.growthStages = 3;
            hyacinth.growthTime = 10;
            hyacinth.waterPerStage = 1;

            PlantLookup[(ushort)PlantId.Hyacinth] = hyacinth;
            
            PlantProperties pansy = PlantLookup[(ushort)PlantId.Pansy];
            pansy.growthStages = 3;
            pansy.growthTime = 10;
            pansy.waterPerStage = 1;

            PlantLookup[(ushort)PlantId.Pansy] = pansy;

            PlantProperties crocus = PlantLookup[(ushort)PlantId.Crocus];
            crocus.growthStages = 3;
            crocus.growthTime = 10;
            crocus.waterPerStage = 1;

            PlantLookup[(ushort)PlantId.Crocus] = crocus;
        }
    }
}
