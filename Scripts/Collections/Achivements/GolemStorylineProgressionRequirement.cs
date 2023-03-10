using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Achivements/GolemStorylineProgression")]
    public class GolemStorylineProgressionRequirement : AchivementRequirement
    {
        public int happinessRequirement;
        public bool isMatureRequirement;
        public int progressRequirement;
        public override void SetupAchivementRequirement()
        {
            
        }

        public override void ReleaseAchivementRequirement()
        {
            
        }

        public bool VerifyAchievement(int happiness, bool isMature, int progress)
        {
            return happiness >= happinessRequirement && isMature == isMatureRequirement &&
                   progress == progressRequirement;
        }
    }
}
