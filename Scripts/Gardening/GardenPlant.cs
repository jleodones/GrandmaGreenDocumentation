using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;

namespace GrandmaGreen
{
    
    public class GardenPlant : MonoBehaviour, IGameInteractable
    {
        [Header("References")]
        public SpriteRenderer plantSprite;
        public BasePhenotypeData phenotypeData;
        public List<GardenPlant> neighbours;

        [Header("Settings")]
        public Genotype genotype;
        [Header("Breeding")]
        public List<Genotype> punnetSquare;
        public Genotype childGenotype;

        [SerializeReference]
        public ITraitSetData[] traitList;

        public void DoInteraction(Vector3 interactionPoint, PointerState interactionState)
        {
            Harvest();
        }

        public void Harvest()
        {
            //NOTE: This would be calculated when the plant is placed down// when a tile near it is updated

            System.Type phenotype = phenotypeData.GetType();
            List<Genotype> daddies = new List<Genotype>();

            foreach (GardenPlant plant in neighbours)
            {
                if (plant.phenotypeData.GetType() != phenotype) continue;

                Debug.Log("Same species detected");

                daddies.Add(plant.genotype);
            }
            //--------------------------------

            //Punnet square Row + Column

            Genotype daddyGenotype = daddies[Random.Range(0, daddies.Count)];    //NOTE: This should check weighting in the future

            int squareSide = (int)Mathf.Pow(2, phenotypeData.TraitCount);

            List<List<Allele>> topRow = new List<List<Allele>>();
            List<List<Allele>> leftCol = new List<List<Allele>>();

            //Pseudo bit array 
            int[] bitArray = new int[phenotypeData.TraitCount];
            int index;
            //Filling up the punnet square top/left headers
            for (int i = 0; i < squareSide; i++)
            {
                topRow.Add(new List<Allele>());
                leftCol.Add(new List<Allele>());

                for (int j = 0; j < phenotypeData.TraitCount; j++)
                {
                    index = bitArray[j];

                    if (index == 0)
                    {
                        topRow[i].Add(daddyGenotype[j].allele1);
                        leftCol[i].Add(this.genotype[j].allele1);
                    }
                    else
                    {
                        topRow[i].Add(daddyGenotype[j].allele2);
                        leftCol[i].Add(this.genotype[j].allele2);
                    }
                }

                //Incrementing our pseudo binary number (int array)
                bitArray[0] += 1;
                for (int k = 0; k < phenotypeData.TraitCount - 1; k++)
                {
                    if (bitArray[k] > 1)
                    {
                        bitArray[k + 1] += 1;
                        bitArray[k] = 0;
                    }
                }
            }

            //Storing the actual combinations
            punnetSquare = new List<Genotype>();
            Genotype squareValue = null;
            for (int y = 0; y < squareSide; y++)
            {
                for (int x = 0; x < squareSide; x++)
                {
                    squareValue = new Genotype();

                    for (int i = 0; i < phenotypeData.TraitCount; i++)
                    {
                        Trait trait;
                        trait.allele1 = topRow[x][i];
                        trait.allele2 = leftCol[y][i];

                        squareValue[i] = trait;
                    }

                    punnetSquare.Add(squareValue);
                }
            }

            childGenotype = punnetSquare[Random.Range(0, punnetSquare.Count)];

            traitList = new ITraitSetData[phenotypeData.TraitCount];

            for (int i = 0; i < phenotypeData.TraitCount; i++)
            {

                traitList[i] = phenotypeData.TraitList[i];
                if (childGenotype[i].allele1 == Allele.dominant && childGenotype[i].allele2 == Allele.dominant)
                {
                    //traits[i] = phenotypeData.TraitList[i];
                }
                else if (childGenotype[i].allele1 == Allele.recessive && childGenotype[i].allele2 == Allele.recessive)
                {
                    //traits[i] = phenotypeData.TraitList[i];
                }
                else
                {
                   // traits[i] = phenotypeData.TraitList[i];
                }
            }
        }
    }
}
