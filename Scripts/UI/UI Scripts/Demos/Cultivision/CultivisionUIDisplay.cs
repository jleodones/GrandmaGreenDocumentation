using System;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Cultivision
{
    public class CultivisionUIDisplay : UIDisplayBase
    {
        /*
         To-Do:
            - recognize candidate death (remove from canddiate list)
         */

        [SerializeField] private Garden.GardenAreaController gardenController;
        private Garden.PlantState plant;
        private Garden.Genotype genotypePlant;
        private List<Garden.PlantState> breedingCandiates;
        private UQueryBuilder<Button> candidateElements;

        public void Start()
        {
            //Register onClick for all candidate elements
            candidateElements = m_rootVisualElement.Query<Button>(className: "candidate");
            candidateElements.ForEach(candidate =>
            {
                candidate.RegisterCallback<ClickEvent>(OnClickCandidate);
            });
        }

        void OnClickCandidate(ClickEvent evt) {
            ClearPunnetData();
            Button candidate = evt.currentTarget as Button;
            int index = Int32.Parse(candidate.name.Substring(0, 1)) - 1;
            SetParent(breedingCandiates[index].genotype.ToString(), false);
            FillPunnetSquare(breedingCandiates[index]);
        }


        public override void OpenUI()
        {
            base.OpenUI();
            gardenController.onTilemapSelection += DisplayPlantData;
        }

        public override void CloseUI()
        {
            base.CloseUI();
            gardenController.onTilemapSelection -= DisplayPlantData;
            ClearData();
        }

        void DisplayPlantData(Vector3Int cell) {
            ClearData();

            //Get data from cell
            plant = gardenController.GetPlant(cell);

            //Prevent data on empty tile
            if(plant.type == 0)
            {
                return;
            }

            breedingCandiates = gardenController.GetBreedingCandidates(cell);
            var name = plant.type;
            genotypePlant = plant.genotype;
            Sprite sprite = CollectionsSO.LoadedInstance.GetSprite(name, genotypePlant, plant.growthStage);

            //Set data
            SetCandidates(breedingCandiates);
            SetPlantName(name.ToString());
            Display("phenotype", true);
            SetSprite("phenotype", sprite);
            SetParent(genotypePlant.ToString(), true);

        }


        void SetPlantName(string name)
        {
            SetText("plantName", name);
        }

        void SetParent(string genotype, bool isMother)
        {
            if (isMother)
            {
                SetText("genotypeM1", genotype.Substring(0,2));
                SetText("genotypeM2", genotype.Substring(2, 2));
            }
            else
            {
                SetText("genotypeF1", genotype.Substring(0, 2));
                SetText("genotypeF2", genotype.Substring(2, 2));
            }
        }

        void FillPunnetSquare(Garden.PlantState candidateGenotype)
        {
            string mother = genotypePlant.ToString();
            string father = candidateGenotype.genotype.ToString();
            SetText("p1", mother.Substring(0, 1) + father.Substring(0, 1));
            SetText("p2", mother.Substring(0, 1) + father.Substring(1, 1));
            SetText("p3", mother.Substring(1, 1) + father.Substring(0, 1));
            SetText("p4", mother.Substring(1, 1) + father.Substring(1, 1));
            SetText("g1", mother.Substring(2, 1) + father.Substring(2, 1));
            SetText("g2", mother.Substring(2, 1) + father.Substring(3, 1));
            SetText("g3", mother.Substring(3, 1) + father.Substring(2, 1));
            SetText("g4", mother.Substring(3, 1) + father.Substring(3, 1));
        }

        void SetCandidates(List<Garden.PlantState> candidates)
        {
            int index = 1;
            foreach (Garden.PlantState candidate in candidates)
            {
                Garden.Genotype genotype = candidate.genotype;
                string elementName = index.ToString() + "Candidate";
                Display(elementName, true);
                Sprite sprite = CollectionsSO.LoadedInstance.GetSprite(candidate.type, genotype, candidate.growthStage);
                SetSprite(elementName, sprite);
                index++;
            }

        }

        void SetText(string id, string newText)
        {
            m_rootVisualElement.Q<Label>(id).text = newText;
        }

        void SetSprite(string elementName, Sprite sprite) {
            m_rootVisualElement.Q(elementName).style.backgroundImage = new StyleBackground(sprite);

        }

        void Display(string name, bool show) {
            if (show) {
                m_rootVisualElement.Q(name).style.display = DisplayStyle.Flex;
            }
            else {
                m_rootVisualElement.Q(name).style.display = DisplayStyle.None;
            }
        }


        void ClearData() {
            ClearCandidates();
            SetPlantName("");
            Display("phenotype", false);
            SetText("genotypeM1", "");
            SetText("genotypeM2", "");
            ClearPunnetData();
        }

        void ClearCandidates()
        {
            for(int i = 1; i <= 4; i++)
            {
                Display(i.ToString() + "Candidate", false);
            }
        }

        void ClearPunnetData() {
            //Clear Headings
            SetText("genotypeF1", "");
            SetText("genotypeF2", "");

            //Clear punnet squares
            SetText("p1", "");
            SetText("p2", "");
            SetText("p3", "");
            SetText("p4", "");
            SetText("g1", "");
            SetText("g2", "");
            SetText("g3", "");
            SetText("g4", "");
        }


    }
}
