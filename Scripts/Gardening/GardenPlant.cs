using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;

namespace GrandmaGreen
{
    public enum PlantState
    {
        SEEDLING,
        SAPLING,
        MATURE
    }

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
    public class Phenotype
    {
        const int MAX_TRAIT_COUNT = 3;
        [SerializeField] Trait[] traits = new Trait[MAX_TRAIT_COUNT];

        public Phenotype()
        {
            traits = new Trait[MAX_TRAIT_COUNT];
        }

        public Trait this[int i]
        {
            get { return traits[i]; }
            set { traits[i] = value; }
        }
    }

    public class GardenPlant : MonoBehaviour, IGameInteractable
    {
        public SpriteRenderer plantSprite;
        public BaseGenotypeData genotypeData;
        public Phenotype phenotype;
        public List<GardenPlant> neighbours;
        public List<Phenotype> punnetSquare;
        public Phenotype daddyPhenotype;

        public void DoInteraction(Vector3 interactionPoint, PointerState interactionState)
        {
            Harvest();
        }

        public void Harvest()
        {
            //NOTE: This would be calculated when the plant is placed down// when a tile near it is updated

            System.Type genotype = genotypeData.GetType();
            List<Phenotype> daddies = new List<Phenotype>();

            foreach (GardenPlant plant in neighbours)
            {
                if (plant.genotypeData.GetType() != genotype) continue;

                Debug.Log("Same genotype detected");

                daddies.Add(plant.phenotype);
            }
            //--------------------------------

            punnetSquare = new List<Phenotype>();

            //Punnet square


            daddyPhenotype = daddies[Random.Range(0, daddies.Count)];

            int squareLength = (int)Mathf.Pow(2, genotypeData.traitCount) * 2;

            List<List<Allele>> allele1Pool = new List<List<Allele>>();
            List<List<Allele>> allele2Pool = new List<List<Allele>>();

            for (int i = 0; i < genotypeData.traitCount; i++)
            {
                allele1Pool.Add(new List<Allele>(2 * genotypeData.traitCount));
                allele2Pool.Add(new List<Allele>(2 * genotypeData.traitCount));

                allele1Pool[i].Add(daddyPhenotype[i].allele1);
                allele1Pool[i].Add(daddyPhenotype[i].allele2);

                allele2Pool[i].Add(phenotype[i].allele1);
                allele2Pool[i].Add(phenotype[i].allele2);

            }

            Phenotype result = null;
            Trait trait = default(Trait);
            int[] allleIndices = new int[genotypeData.traitCount * 2];
            for (int i = 0; i < squareLength; i++)
            {
                result = new Phenotype();
                int allleIndex = 0;
                for (int j = 0; j < genotypeData.traitCount; j++)
                {
                    trait.allele1 = allele1Pool[j][allleIndex];
                    trait.allele2 = allele2Pool[j][allleIndex];

                    result[j] = trait;
                }

                punnetSquare.Add(result);
            }


        }


    }
}