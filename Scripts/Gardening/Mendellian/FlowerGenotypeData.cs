using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GrandmaGreen
{
    [CreateAssetMenu()]
    public class FlowerGenotypeData : BaseGenotypeData
    {
        [NaughtyAttributes.Button]
        public override void SetTraitData()
        {
            traitList = new ITraitPairData[traitCount];
            traitList[0] = new ColorPair();
            traitList[1]  = new FloatPair();
        }
    }
}