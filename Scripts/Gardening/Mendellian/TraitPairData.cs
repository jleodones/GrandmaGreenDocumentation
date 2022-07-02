using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GrandmaGreen
{
    public interface ITraitPairData
    {
        Type type { get; }
    }

    [Serializable]
    public class TraitData<T> where T : struct
    {
        public T value;
        public Type type => typeof(T);
    }

    [Serializable]
    public class TraitPairData<T> : ITraitPairData where T : struct
    {

        public int weight;

        [field: SerializeField] public TraitData<T> gene_a { get; private set; }
        [field: SerializeField] public TraitData<T> gene_A { get; private set; }
        [field: SerializeField] public TraitData<T> gene_Aa { get; private set; }

        public Type type => typeof(T);
    }


    [Serializable]
    public class ColorPair : TraitPairData<Color> { }

    [Serializable]
    public class FloatPair : TraitPairData<float> { }
}




