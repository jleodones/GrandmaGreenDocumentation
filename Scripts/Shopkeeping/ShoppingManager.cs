using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Shopkeeping
{
    using Timer = TimeLayer.TimeLayer;
    [CreateAssetMenu(menuName = "GrandmaGreen/Shopkeeping/ShoppingManager")]
    public class ShoppingManager : ScriptableObject
    {
        public List<ShopItem> currGardenList;
        public List<ShopItem> currDecorList;
        
        public GardeningShopUIController gardenController;
        public DecorShopUIController decorController;
        
        [SerializeField]
        public Timer shopTimer;

        public void StartShop()
        {
            if (gardenController == null)
            {
                gardenController = new GardeningShopUIController();
                decorController = new DecorShopUIController();
            }
            currGardenList = gardenController.GardenList;
            currDecorList = decorController.DecorList;

            shopTimer.Resume(true);
            shopTimer.onTick += gardenController.UpdateCycle;
            shopTimer.onTick += decorController.UpdateCycle;
        }

        public double GetTimeLeft()
        {
            return shopTimer.tickSeconds - shopTimer.GetTickValue();
        }
    }
}
