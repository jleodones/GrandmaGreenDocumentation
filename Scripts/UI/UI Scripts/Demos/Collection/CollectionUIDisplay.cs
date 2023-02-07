using System.Collections.Generic;
using System.Diagnostics;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using GrandmaGreen.SaveSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public struct CurrentPage
    {
        public string scrollPage;
        public string displayPage;
    }
    public enum CollectionsMode
    {
        Plants,
        Friends,
        Awards
    }
    public class CollectionUIDisplay : UIDisplayBase
    {
        public CollectionsSaver collectionSaver;
        public CollectionsMode collectionsMode = CollectionsMode.Plants;
        private List<Button> m_scrollIcons;

        private CurrentPage m_currentPage = new CurrentPage();
        public override void Load()
        {
            // Event manager subscribe.
            EventManager.instance.EVENT_UPDATE_PLANT_COLLECTIONS += UpdatePlantInfo;
            
            collectionSaver.onPlantUpdate = UpdatePlant;

            // Set the scroll buttons.
            // TODO: For now, these respond based on touch/click events. Need to replace this with swiping mechanic later.
            // Get all scroll buttons.
            m_scrollIcons = m_rootVisualElement.Query<Button>(className: "scroll-icon").ToList();
            for (int i = 0; i < m_scrollIcons.Count; i++)
            {
                m_scrollIcons[i].userData = (i + 1).ToString();
                m_scrollIcons[i].RegisterCallback<ClickEvent>(SwitchPage);
            }

            // Back button registration.
            m_rootVisualElement.Q<Button>("back-button").RegisterCallback<ClickEvent>(evt =>
                {
                    Clear(new ClickEvent());
                    m_rootVisualElement.Q<VisualElement>(m_currentPage.scrollPage).AddToClassList("active-page");
                }
            );
            
            // Load collections based off of available data.
            LoadPlants();
            
        }

        public override void Unload()
        {
            // Event manager unsubscribe.
            EventManager.instance.EVENT_UPDATE_PLANT_COLLECTIONS -= UpdatePlantInfo;
        }

        public override void OpenUI()
        {
            Clear(new ClickEvent());
            collectionsMode = CollectionsMode.Plants;
            SwitchPageInternal(m_scrollIcons[0]);
            
            base.OpenUI();
        }

        private void Clear(ClickEvent ce)
        {
            m_rootVisualElement.Q<Button>("back-button").style.display = DisplayStyle.None;
            if (m_currentPage.displayPage != "")
            {
                m_rootVisualElement.Q<VisualElement>(m_currentPage.displayPage).RemoveFromClassList("active-page");
                m_currentPage.displayPage = "";
            }
            m_rootVisualElement.Q<VisualElement>("scroll-area").style.display = DisplayStyle.Flex;
        }

        private void SwitchPage(ClickEvent ce)
        {
            // Retrieve button.
            Button scrollButton = ce.currentTarget as Button;
            SwitchPageInternal(scrollButton);
        }

        private void SwitchPageInternal(Button scrollButton)
        {
            // All pages and scroll icons will be turned off except this one.
            string pageType = "";
            switch (collectionsMode)
            {
                case CollectionsMode.Plants:
                    pageType += "plant-page-";
                    break;
                case CollectionsMode.Friends:
                    break;
                case CollectionsMode.Awards:
                    break;
            }
            
            foreach (Button button in m_scrollIcons)
            {
                VisualElement page = m_rootVisualElement.Q<VisualElement>(pageType + button.userData);
                if (button != scrollButton)
                {
                    // Scroll icon resolution.
                    button.RemoveFromClassList("scroll-icon-active");
                    button.AddToClassList("scroll-icon-inactive");
                    
                    // Page resolution.
                    page.RemoveFromClassList("active-page");
                }
                else
                {
                    // Scroll icon resolution.
                    button.RemoveFromClassList("scroll-icon-inactive");
                    button.AddToClassList("scroll-icon-active");
                    
                    // Page resolution.
                    page.AddToClassList("active-page");

                    m_currentPage.scrollPage = pageType + button.userData;
                }
            }
        }
        
        // Loads plant sprites.
        private void LoadPlants()
        {
            var plantsInCollection = collectionSaver.PlantValues();

            foreach (PlantCollectionProperties plant in plantsInCollection)
            {
                UpdatePlant(plant);
            }
        }

        public void UpdatePlant(PlantCollectionProperties plant, bool isNewPlant = true)
        {
            string plantToUnlock = "";
            plantToUnlock += ((PlantId)plant.id).ToString();

            switch (plant.trait)
            {
                case (Genotype.Trait.Dominant):
                    plantToUnlock += "-Dom";
                    break;
                case Genotype.Trait.Heterozygous:
                    plantToUnlock += "-Het";
                    break;
                case Genotype.Trait.Recessive:
                    plantToUnlock += "-Rec";
                    break;
            }

            if (plant.isMega)
            {
                plantToUnlock += "-M";
            }

            var entryButton = m_rootVisualElement.Q<VisualElement>(plantToUnlock).Q<Button>();
            entryButton.userData = plant;
            if (isNewPlant)
            {
                entryButton.RemoveFromClassList("entry-locked");
                entryButton.RegisterCallback<ClickEvent>(DisplayPlantInfo);
            }
        }

        public void UpdatePlantInfo(IInventoryItem item)
        {
            collectionSaver.UpdatePlantCollections(item);
        }

        private void DisplayPlantInfo(ClickEvent ce)
        {
            Button b = ce.currentTarget as Button;
            PlantCollectionProperties plantProperties = (PlantCollectionProperties) b.userData;
            
            // Retrieving the plant info page.
            VisualElement plantInfoPage = m_rootVisualElement.Q<VisualElement>("plant-entry");
            m_currentPage.displayPage = "plant-entry";
            
            // Hooking up the various parts of it.
            // Name.
            plantInfoPage.Q<Label>("plant-entry-name").text = plantProperties.name;
            
            // Description.
            plantInfoPage.Q<Label>("plant-description").text = plantProperties.description;
            
            // Trait.
            string genotypeString = "genotype: ";
            if (plantProperties.isMega)
            {
                genotypeString += "Mega";
            }

            genotypeString += plantProperties.trait.ToString();

            plantInfoPage.Q<Label>("plant-genotype").text = genotypeString;

            // Retrieve seed packet.
            Genotype g = new Genotype();
            g.trait = plantProperties.trait;
            
            Sprite seed = CollectionsSO.LoadedInstance.GetSprite((PlantId)plantProperties.id, g);
            plantInfoPage.Q<VisualElement>("plant-entry-seed").style.backgroundImage = new StyleBackground(seed);
            
            // Check if mature plant.
            // Retrieve size indicators.
            var plantGrowthImages = plantInfoPage.Q<VisualElement>("plant-entry-images");
            var sizeBadges = plantInfoPage.Query<VisualElement>(className: "size-badge").ToList();
            if (plantProperties.matureUnlocked)
            {
                plantGrowthImages.style.display = DisplayStyle.Flex;
                // Set sprites.
                Sprite seedling = CollectionsSO.LoadedInstance.GetSprite((PlantId)plantProperties.id, g, 0);
                Sprite growing = CollectionsSO.LoadedInstance.GetSprite((PlantId)plantProperties.id, g, 1);
                Sprite mature = CollectionsSO.LoadedInstance.GetSprite((PlantId)plantProperties.id, g, 2);

                plantGrowthImages.Q<VisualElement>("plant-seedling").style.backgroundImage = new StyleBackground(seedling);
                plantGrowthImages.Q<VisualElement>("plant-growing").style.backgroundImage = new StyleBackground(growing);
                plantGrowthImages.Q<VisualElement>("plant-mature").style.backgroundImage = new StyleBackground(mature);

                // Size badges.
                if (plantProperties.unlockedSizes.Contains(Genotype.Size.Small))
                {
                    plantInfoPage.Q<VisualElement>("small").RemoveFromClassList("size-badge-locked");
                }
                else if (plantProperties.unlockedSizes.Contains(Genotype.Size.Medium))
                {
                    plantInfoPage.Q<VisualElement>("mid").RemoveFromClassList("size-badge-locked");
                }
                else if (plantProperties.unlockedSizes.Contains(Genotype.Size.Big))
                {
                    plantInfoPage.Q<VisualElement>("large").RemoveFromClassList("size-badge-locked");
                }
            }
            else
            {
                plantGrowthImages.style.display = DisplayStyle.None;
                foreach (VisualElement badge in sizeBadges)
                {
                    badge.AddToClassList("size-badge-locked");
                }
            }
            
            // TODO: Do award badges here once they're synced.
            
            // VISUAL DISPLAY LAST.
            // Turn off scroll bar.
            m_rootVisualElement.Q<VisualElement>("scroll-area").style.display = DisplayStyle.None;
            
            // Turn off navigation page.
            m_rootVisualElement.Q<VisualElement>(m_currentPage.scrollPage).RemoveFromClassList("active-page");
            // Turn on detailed plant page.
            plantInfoPage.AddToClassList("active-page");
            
            // Turn on button.
            m_rootVisualElement.Q<Button>("back-button").style.display = DisplayStyle.Flex;
        }
    }
}
