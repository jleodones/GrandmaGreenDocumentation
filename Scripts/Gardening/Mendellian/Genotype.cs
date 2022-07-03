using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public enum Allele
    {
        recessive = 0,
        dominant = 1
    }

    [System.Serializable]
    public struct Trait
    {
        public Allele allele1;
        public Allele allele2;
    }

    [System.Serializable]
    public class Genotype
    {
        public static readonly int MAX_TRAIT_COUNT = 3;
        [SerializeField] Trait[] traits = new Trait[MAX_TRAIT_COUNT];

        public Genotype()
        {
            traits = new Trait[MAX_TRAIT_COUNT];
        }

        public Trait this[int i]
        {
            get { return traits[i]; }
            set { traits[i] = value; }
        }
    }
}
