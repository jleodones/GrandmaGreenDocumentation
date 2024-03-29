using GrandmaGreen.Garden;
using Sirenix.OdinInspector;
using System;
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
        Tulip = 5004,
        Apple = 5005,
        Cotton = 5006,
        Crocus = 5007,
        Pumpkin = 5008,
        Turnip = 5009
    }

    public enum ExpressionId : ushort
    { 
        Neutral = 0,
        Happy = 1,
        Sad = 2,
        Surprise = 3,
        Angry = 4,
        Blush = 5,
        Confused = 6
    }


    public enum DecorationId : ushort
    {
        TulipGolemMemento = 4001,
        AppleGolemMemento,
        CrocusGolemMemento,
        RadishGolemMemento,
        PumpkinGolemMemento,
        CottonGolemMemento,
        CampingLantern,
        SimpleHangingLampPost01,
        SimpleHangingLampPost02,
        Torch01,
        Torch02,
        JackOLantern01,
        JackOLantern02,
        IronLampPost01,
        IronLampPost02,
        JapaneseGardenLantern01,
        JapaneseGardenLantern02,
        MushroomLamp01,
        MushroomLamp02,
        Scarecrow,
        BikeBlack,
        BikeBlue,
        FlowerWagon,
        HarvestBasketblue,
        HarvestBasketgreen,
        HarvestBasketpink,
        HarvestBasketwhite,
        Umbrella01,
        Umbrella02,
        Umbrella03,
        Swingset01,
        Swingset02,
        GreenLeafpile,
        OrangeLeafpile,
        FrenchCountryUrn,
        PicnicBasket,
        GardenGnomeBasic,
        GardenGnomeSexy,
        GardenGnomeGold,
        FrogSculpture,
        Flamingo,
        ArtNouveauSculture01,
        ArtNouveauSculture02,
        ArtNouveauSculture03,
        FoxSculpture,
        IronHatch,
        StoneBenchwithFancyLeg,
        StoneNaturalBench,
        StoneBenchwithPlainLeg,
        WoodandIronBench,
        WoodBenchwithoutBack,
        WoodBenchwithBack,
        IronChairCushion,
        IronChairOval,
        MushroomChair,
        WoodThickChair,
        WoodThinChair,
        RockingChair,
        StoneNaturalBenchwithMoss,
        IronBenchCushion,
        PicnicTable,
        MushroomTable,
        DaisyTable,
        IronOneLegBlackTable,
        IronOneLegGreenTable,
        IronOneLegWhiteTable,
        IronThreeLegBlackTable,
        IronThreeLegGreenTable,
        IronThreeLegWhiteTable,
        WoodRoundLegTable,
        WoodSlantLegTable,
        WoodStraightLegTable,
        BirdBath,
        NaturalPond,
        FancyPond,
        KoiPond,
        SimpleFountain,
        FancyFountain,
        MagicFountain,
        Well,
        RusticWell,
        Waterpump,
        RusticBucket,
        GrecianRuin,
        Sundial,
        ClaudeMonetPainting,
        JohnSingerSargentPainting,
        MailboxBasic,
        MailboxCottagecore,
        MailboxGoth,
        MailboxFairy,
        MailboxOriental,
        MailboxCabin,
        MailboxPastel,
        WhitePicket,
        SimpleWooden,
        GateofSimpleWood,
        Farm,
        GateofFarm,
        FancyIron,
        GateofFancyIron,
        WhiteFancyIron,
        GateofWhiteFancyIron,
        Fieldstone,
        GateofFieldstone,
        WoodwithTrees,
        GateofTreeWood,
        IvyCoveredLattice,
        GateofLattice,
        HistoricalEnglish,
        FrenchProvincial,
        PNWCabin,
        TraditionalJapanese,
        Modern,
        Acorn,
        Pastel,
        WitchyGoth,
        FairyMagic,
        EuropearnCastle,
        Dirt,
        Cobblestone,
        Tile,
        Wood,
        Oak,
        Pine01,
        Pine02,
        Willow,
        Palm,
        Maple,
        Poplar,
        Stump,
        Tree01,
        Tree02,
        Hydrangea,
        Oleander,
        Gardenia,
        Boxwood,
        CommonPoppy,
        ForgetMeNot,
        WildDaisies,
        Dandelion,
        Clover,
        Hemlock,
        PottedPlant01,
        PottedPlant02,
        PottedPlant03
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

        public Dictionary<string, Sprite> SingleSpriteCache;
        public Dictionary<string, Sprite[]> SpriteSheetCache;

        public static CollectionsSO LoadedInstance;

        public void LoadCollections()
        {
            GenerateCollections();
            LoadedInstance = this;
        }

        public void UnloadCollections()
        {
            LoadedInstance = null;
        }

        [Button()]
        public void GenerateCollections()
        {
            CSVtoSO.GenerateCollectionsSO(this, dataSheet);
        }

        public static bool IsFlower(PlantId id)
        {
            return Array.Exists<PlantId>((PlantId[])Enum.GetValues(typeof(FlowerId)), element => element == id);
        }

        public static bool IsVegetable(PlantId id)
        {
            return Array.Exists<PlantId>((PlantId[])Enum.GetValues(typeof(VegetableId)), element => element == id);
        }

        public static bool IsFruit(PlantId id)
        {
            return Array.Exists<PlantId>((PlantId[])Enum.GetValues(typeof(FruitId)), element => element == id);
        }

        public string GetResourcePath(PlantId id)
        {
            PlantProperties plant = GetPlant(id);
            string resourceDirPath = string.Format("{0}s/{1}/{2}",
                plant.plantType.ToString(), plant.name, plant.spriteBasePath);
            return resourceDirPath;
        }

        public bool PlantToGolem(PlantId id, out CharacterId golemID)
        {
            golemID = default;

            switch (id)
            {
                case PlantId.Tulip:
                    golemID = CharacterId.Tulip;
                    return true;
                case PlantId.Crocus:
                    golemID = CharacterId.Crocus;
                    return false;
                case PlantId.Pumpkin:
                    golemID = CharacterId.Pumpkin;
                    return true;
                case PlantId.Apple:
                    golemID = CharacterId.Apple;
                    return false;
                case PlantId.Turnip:
                    golemID = CharacterId.Turnip;
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
            Sprite sprite;
            GetCachedSingleSprite(spritePath, out sprite);
            return sprite;
            //return Resources.Load(spritePath, typeof(Sprite)) as Sprite;
        }

        ///<summary>
        ///Retrieve a sprite by its sprite path (which is just its filename)
        ///</summary>
        ///
        public Sprite GetSprite(ushort id)
        {
            ItemProperties item = GetItem(id);
            Sprite sprite;
            GetCachedSingleSprite(item.spritePath, out sprite);
            return sprite;
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

            if (IsFruit(type))
            {
                return GetFruitTree(type, genotype, growthStage);
            }

            switch (growthStage)
            {
                case 0:
                    suffix = "_Seedling";
                    break;
                case 1:
                    suffix = "_Growing";
                    break;
                case 2:
                    suffix = genotype.SpriteSuffix(type);
                    break;
            }
            Sprite sprite;
            GetCachedSingleSprite(GetResourcePath(type) + suffix, out sprite);
            return sprite;
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
            else
            {
                return GetVegetableHead(type, genotype, 2);
            }

            //Sprite[] sheet = Resources.LoadAll<Sprite>(plant.plantType.ToString() + "s/" + plant.name + "/" + plant.spriteBasePath);
            Sprite[] sheet;
            GetCachedSpriteSheet(plant.plantType.ToString() + "s/" + plant.name + "/" + plant.spriteBasePath, out sheet);
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
            string spritePath = GetResourcePath(type) + "_HeadLarge";
            Sprite sprite;
            GetCachedSingleSprite(spritePath, out sprite);
            return sprite;
        }

        public Sprite GetFruitTree(PlantId type, Genotype genotype, int growthStage)
        {
            string resourceDirPath = GetResourcePath(type);
            if (growthStage < 2)
            {
                //Sprite[] sheet = Resources.LoadAll<Sprite>(resourceDirPath);
                Sprite[] sheet;
                GetCachedSpriteSheet(resourceDirPath, out sheet);
                if (growthStage == 0) return sheet[5];
                else if (growthStage == 1) return sheet[1];
            }
            else if (growthStage == 2)
            {
                Sprite sprite;
                GetCachedSingleSprite(resourceDirPath + genotype.SpriteSuffix(type), out sprite);
                return sprite;
            }
            return null;
        }

        public Sprite GetFruitFruit(PlantId type, Genotype genotype, int growthStage)
        {
            if (growthStage < 2) return null;
            string resourceDirPath = GetResourcePath(type);
            //Sprite[] sheet = Resources.LoadAll<Sprite>(resourceDirPath);
            Sprite[] sheet;
            GetCachedSpriteSheet(resourceDirPath, out sheet);
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

            //Sprite[] sheet = Resources.LoadAll<Sprite>("Seed Packets/" + baseSpritePath);
            Sprite[] sheet;
            GetCachedSpriteSheet("Seed Packets/" + baseSpritePath, out sheet);

            Sprite seedSprite = sheet.Single(s => s.name == baseSpritePath + suffix);
            return seedSprite;
        }

        /// <summary>
        /// Returns true if the given path exists in the Sprite Cache dictionary. If true, sets outSprite as the single sprite.
        /// If false, sets outSprite using Resources.Load and saves to the cache dictionary for single sprites
        /// </summary>
        /// <param name="id"></param>
        /// <param name="outSprite"></param>
        /// <returns></returns>
        public bool GetCachedSingleSprite(string path, out Sprite outSprite)
        {
            if (SingleSpriteCache.ContainsKey(path))
            {
                outSprite = SingleSpriteCache[path];
                return true;
            }
            else
            {
                outSprite = Resources.Load<Sprite>(path);
                SingleSpriteCache.Add(path, outSprite);
                return false;
            }
        }

        /// <summary>
        /// Returns true if the given path exists in the Sprite Sheet Cache dictionary. If true, sets outSheet as the sprite sheet.
        /// If false, sets outSheet using Resources.Load and saves to the cache dictionary for sprite sheets
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outSheet"></param>
        /// <returns></returns>
        public bool GetCachedSpriteSheet(string path, out Sprite[] outSheet)
        {
            if (SpriteSheetCache.ContainsKey(path))
            {
                outSheet = SpriteSheetCache[path];
                return true;
            }
            else
            {
                outSheet = Resources.LoadAll<Sprite>(path);
                SpriteSheetCache.Add(path, outSheet);
                return false;
            }
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
