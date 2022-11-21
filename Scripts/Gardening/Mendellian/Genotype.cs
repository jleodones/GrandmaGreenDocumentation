using UnityEngine;
using Exception = System.Exception;
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
                throw new Exception("Invalid genotype: " + genotype); 
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

        public override bool Equals(object obj) =>
            obj is Genotype other
            && other.trait == trait
            && other.size == size;
        public override string ToString()
        {
            return (a1 ? "A" : "a")
		        + (a2 ? "A" : "a")
		        + (b1 ? "B" : "b")
		        + (b2 ? "B" : "b");
        }

        public Genotype Cross(Genotype dad)
        {
            // pre-baked punnett square lookup
            int[][] punnettSquare = new int[][] {
                new int[] { 0, 4, 2, 6 }, new int[] { 0, 4, 3, 6 }, new int[] { 1, 4, 2, 6 }, new int[] { 1, 4, 3, 6 },
                new int[] { 0, 4, 2, 7 }, new int[] { 0, 4, 3, 7 }, new int[] { 1, 4, 2, 7 }, new int[] { 1, 4, 3, 7 },
                new int[] { 0, 5, 2, 6 }, new int[] { 0, 5, 3, 6 }, new int[] { 1, 5, 2, 6 }, new int[] { 1, 5, 3, 6 },
                new int[] { 0, 5, 2, 7 }, new int[] { 0, 5, 3, 7 }, new int[] { 1, 5, 2, 7 }, new int[] { 1, 5, 3, 7 }
            };
            string cross = this.ToString() + dad.ToString();
            string child = "";
            // debug
            string debug = "CrossBreed Debug (Click to Expand)\n";
            debug += "Mom: " + this.ToString() + " Dad: " + dad.ToString() + "\n";
            debug += "Punnett Square: \n";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    foreach (int k in punnettSquare[i*4+j])
                    {
                        debug += cross[k];
		            }
                    debug += " ";
		        }
                debug += "\n";
	        }
	        foreach (int i in punnettSquare[(int)(Random.value * punnettSquare.Length)])
            {
                child += cross[i];
	        }
            debug += "Child: " + child;
            Debug.Log(debug);
            return new Genotype(child);
        }
    }
}
