using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public abstract class AchivementRequirement : ScriptableObject
    {
        public System.Action<int> onRequirementValueChanged;

        public abstract void SetupAchivementRequirement();
        public abstract void ReleaseAchivementRequirement();
    
        public void RegisterAchivement(GameAchivement achivement)
        {
            onRequirementValueChanged += achivement.AddProgression;
        }

        public void DeregisterAchivement(GameAchivement achivement)
        {
            onRequirementValueChanged -= achivement.AddProgression;
        }

        protected virtual void RequirementValueChanged(int change)
        {
            onRequirementValueChanged?.Invoke(change);
        }
    }
}
