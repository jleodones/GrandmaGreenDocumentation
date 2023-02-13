using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
        [CreateAssetMenu(menuName = "GrandmaGreen/Achivements/SpendMoneyRequirement")]
    public class SpendMoneyRequirement : AchivementRequirement
    {
        public override void ReleaseAchivementRequirement()
        {
            EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY -= RequirementValueChanged;
        }

        public override void SetupAchivementRequirement()
        {
            EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY += RequirementValueChanged;
        }
    }
}
