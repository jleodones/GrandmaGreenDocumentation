using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace GrandmaGreen
{
    struct PlantData
    {
        int plantIndex;
        PlantState plantState;
    }

    [CreateAssetMenu()]
    public class GardenData : ScriptableObject
    {
        public int2 GardenSize;
    }
}