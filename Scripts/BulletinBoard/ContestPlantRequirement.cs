using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Bulletin/ContestPlantRequirement")]
    public class ContestPlantRequirement : BulletinRequirement
    {
        public Plant TargetPlant = new Plant((ushort)PlantId.Tulip, "Tulip", new Genotype("AaBb"));

        public override void SetupBulletinRequirement()
        {
            EventManager.instance.EVENT_SUBMIT_PLANT += ComparePlants;
            //EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY += RequirementValueChanged;
        }

        public override void ReleaseBulletinRequirement()
        {
            EventManager.instance.EVENT_SUBMIT_PLANT -= ComparePlants;
            //EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY -= RequirementValueChanged;
        }

        public void ComparePlants(ushort submissionID, Genotype submissionGenotype)
        {
            if (submissionID == TargetPlant.itemID)
            {
                RequirementValueChanged(1);
            }
            else
            {
                RequirementValueChanged(-1);
            }
        }
    }
}
