using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    /// <summary>
    /// Simple enum to define an Allele
    /// Either recessive or dominant
    /// </summary>
    public enum Allele
    {
        recessive = 0,
        dominant = 1
    }

    /// <summary>
    /// Struct that defines a trait
    /// Currently defined with 2 Alleles
    /// </summary>
    [Serializable]
    public struct Trait
    {
        public Allele allele1;
        public Allele allele2;
    }
    
    /// <summary>
    /// Defines a unique Genotype
    /// A genotype can have up to 3 unique traits
    /// </summary>
    [Serializable]
    public class Genotype
    {
        public static readonly int MAX_TRAIT_COUNT = 3;
        
        [SerializeField]
        Trait[] traits = new Trait[MAX_TRAIT_COUNT];

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
