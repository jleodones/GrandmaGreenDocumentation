using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;


namespace GrandmaGreen
{
    public abstract class BaseGenotypeData : ScriptableObject
    {  
        
        public abstract void SetTraitData();
        [SerializeReference] protected ITraitPairData[] traitList;
        public int traitCount;

        public virtual T Trait<T>(int index) where T : class, ITraitPairData
        {
            return traitList[index] as T;
        }

    }
}