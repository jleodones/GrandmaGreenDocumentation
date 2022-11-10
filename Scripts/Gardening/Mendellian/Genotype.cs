using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GrandmaGreen.Garden
{
    [System.Serializable]
    public struct Genotype
    {
        public enum Size { Big, Medium, Small };
        public enum Trait { Dominant, Heterozygous, Recessive };

        public bool a1;
        public bool a2;
        public bool b1;
        public bool b2;

        public Size size;
        public Trait trait;

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
            size = a1 && a2 ? Size.Big : (a1 || a2 ? Size.Medium : Size.Small);
            trait = b1 && b2 ? Trait.Dominant : (b1 || b2 ? Trait.Heterozygous : Trait.Recessive);
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
