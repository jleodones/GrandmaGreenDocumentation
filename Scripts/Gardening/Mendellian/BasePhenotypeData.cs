using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;


namespace GrandmaGreen.Garden
{
    public abstract class BasePhenotypeData : ScriptableObject
    {
        public abstract void SetTraitData();

        [SerializeReference] protected ITraitSetData[] traitList;
        public ITraitSetData[] TraitList => traitList;
        public virtual int TraitCount => 0;

        public virtual T Trait<T>(int index) where T : class, ITraitSetData
        {
            return traitList[index] as T;
        }
    }
}