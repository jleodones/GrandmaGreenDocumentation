using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GrandmaGreen.Garden
{
    /// <summary>
    /// Simple enum to define an Allele
    /// Either recessive or dominant
    /// </summary>
    //public enum Allele
    //{
    //    recessive = 0,
    //    dominant = 1
    //}

    /// <summary>
    /// Struct that defines a trait
    /// Currently defined with 2 Alleles
    /// </summary>
    //[Serializable]
    //public struct Trait
    //{
    //    public Allele allele1;
    //    public Allele allele2;
    //}

    /// <summary>
    /// Defines a unique Genotype
    /// A genotype can have up to 3 unique traits
    /// </summary>
    //[Serializable]
    //public class Genotype
    //{
    //    public static readonly int MAX_TRAIT_COUNT = 3;

    //    [SerializeField]
    //    Trait[] traits = new Trait[MAX_TRAIT_COUNT];

    //    public Genotype()
    //    {
    //        traits = new Trait[MAX_TRAIT_COUNT];
    //    }

    //    public Trait this[int i]
    //    {
    //        get { return traits[i]; }
    //        set { traits[i] = value; }
    //    }
    //}

    [System.Serializable]
    public struct Genotype
    {
        public readonly bool a1;
        public readonly bool a2;
        public readonly bool b1;
        public readonly bool b2;

        public Genotype(string genotype)
        {
            if (genotype.Length != 4)
            {
                throw new System.Exception("Invalid genotype: " + genotype); 
	        }
            string valid = "AaBb";
            foreach (char c in genotype)
            {
                if (!valid.Contains(c)) throw new Exception("Invalid genotype: " + genotype); 
	        }
            a1 = genotype[0] == 'A';
            a2 = genotype[1] == 'A';
            b1 = genotype[2] == 'B';
            b2 = genotype[3] == 'B';
        }

        public override string ToString()
        {
            return (a1 ? "A" : "a")
		        + (a2 ? "A" : "a")
		        + (b1 ? "B" : "b")
		        + (b2 ? "B" : "b");
        }

        public Genotype Cross(Genotype dad)
        {
            int[][] punnettSquare = {
                new int[]{ 0, 4, 2, 6 }, new int[]{ 0, 4, 3, 6 }, new int[]{ 1, 4, 2, 6 }, new int[]{ 1, 4, 3, 6 },
                new int[]{ 0, 4, 2, 7 }, new int[]{ 0, 4, 3, 7 }, new int[]{ 1, 4, 2, 7 }, new int[]{ 1, 4, 3, 7 },
                new int[]{ 0, 5, 2, 6 }, new int[]{ 0, 5, 3, 6 }, new int[]{ 1, 5, 2, 6 }, new int[]{ 1, 5, 3, 6 },
                new int[]{ 0, 5, 2, 7 }, new int[]{ 0, 5, 3, 7 }, new int[]{ 1, 5, 2, 7 }, new int[]{ 1, 5, 3, 7 }
            };

            string traits = this.ToString() + dad.ToString();
            string childGenotype = "";

            foreach(int i in punnettSquare[Random.Range(0, punnettSquare.Length)])
            {
                childGenotype += traits[i];
	        }

            return new Genotype(childGenotype);
        }
    }
}
