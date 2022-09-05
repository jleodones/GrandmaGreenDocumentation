using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GrandmaGreen.Garden
{
    public interface ITraitData
    {
        Type type { get; }
    }
    
    public interface ITraitSetData
    {
        int weight { get; }
        Type type { get; }

        ITraitData recessive { get; }
        ITraitData dominant { get; }
        ITraitData mixed { get; }
    }

    [Serializable]
    public class TraitData<T> : ITraitData where T : struct
    {
        public T value;
        public Type type => typeof(T);
    }

    [Serializable]
    public class TraitSetData<T> : ITraitSetData where T : struct
    {
        [SerializeField]
        [HideInInspector]
        string name = typeof(T).Name.ToString();
        public int weight;

        [field: SerializeField] public TraitData<T> recessive { get; private set; }
        [field: SerializeField] public TraitData<T> dominant { get; private set; }
        [field: SerializeField] public TraitData<T> mixed { get; private set; }

        int ITraitSetData.weight => weight;
        public Type type => typeof(T);
        
        ITraitData ITraitSetData.recessive => recessive;
        ITraitData ITraitSetData.dominant => dominant;
        ITraitData ITraitSetData.mixed => mixed;
    }

    [Serializable]
    public class ColorSet : TraitSetData<Color>
    {

    }

    [Serializable]
    public class SizeSet : TraitSetData<float>
    {

    }
}




