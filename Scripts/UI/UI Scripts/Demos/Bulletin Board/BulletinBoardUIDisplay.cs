using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class BulletinBoardUIDisplay : UIDisplayBase
    {

        public override void OpenUI()
        {
            EventManager.instance.HandleEVENT_CLOSE_HUD();
            m_rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public override void CloseUI()
        {
            EventManager.instance.HandleEVENT_OPEN_HUD();
            m_rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}
