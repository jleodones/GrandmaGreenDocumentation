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

        public override string ToString()
        {
            return (a1 ? "A" : "a")
		        + (a2 ? "A" : "a")
		        + (b1 ? "B" : "b")
		        + (b2 ? "B" : "b");
        }

        public Genotype Cross(Genotype dad)
        {
            string childGenotype = "";
            for (int i = 0; i < 4; i++)
            {
                childGenotype += (Random.value > 0.5f) ? dad.ToString()[i] : this.ToString()[i];
	        }
            return new Genotype(childGenotype);
        }
    }
}
