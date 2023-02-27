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

        public GardeningShopUIController m_controller;

        [SerializeField]
        public Timer shopTimer;

        public void StartShop()
        {
            if (m_controller == null)
            {
                m_controller = new GardeningShopUIController();
            }
            currGardenList = m_controller.GardenList;
            shopTimer.Resume(true);
            shopTimer.onTick += m_controller.UpdateCycle;
            
        }

        public double GetTimeLeft()
        {
            return shopTimer.tickSeconds - shopTimer.GetTickValue();
        }
    }
}
