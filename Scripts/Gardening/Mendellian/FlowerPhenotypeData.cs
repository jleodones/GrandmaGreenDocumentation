using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Phenotypes/Flower")]
    public class FlowerPhenotypeData : BasePhenotypeData
    {
        public override int TraitCount => 2;

        [NaughtyAttributes.Button]
        public override void SetTraitData()
        {
            traitList = new ITraitSetData[TraitCount];
            traitList[0] = new ColorSet();
            traitList[1] = new SizeSet();
        }
    }
}