using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections
{
    public enum PlantId : ushort
    {
        Rose = 1001,
        Tulip = 1002
    }

    public enum SeedId : ushort
    {
        Rose = 2001,
        Tulip = 2002
    }

    public struct PlantProperties
    {
        public string name;
        public int growthStages;
        public double growthTimeSecs;
        public List<string> spritePaths;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Types/TEMP_PlantTypeCollection")]
    public class TEMP_PlantTypeCollection : ScriptableObject
    {
        private readonly Dictionary<PlantId, PlantProperties>
	        plantPropertyLookup = new Dictionary<PlantId, PlantProperties>()
        {
            {
		        PlantId.Rose, new PlantProperties
		        {
                    name = "Rose",
                    growthStages = 3,
                    growthTimeSecs = 10
		        }
		    },
            {
		        PlantId.Tulip, new PlantProperties
                {
                    name = "Tulip",
                    growthStages = 3,
                    growthTimeSecs = 10
		        }
		    }
        };

        public PlantProperties GetPlant(PlantId id)
        {
            return plantPropertyLookup[id]; 
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
    }
}
