using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public abstract class BulletinRequirement : ScriptableObject
    {
        public System.Action<int> onBulletinRequirementChanged;

        public abstract void SetupBulletinRequirement();
        public abstract void ReleaseBulletinRequirement();

        public void RegisterBulletin(GameBulletin bulletinOption)
        {
            onBulletinRequirementChanged += bulletinOption.AddProgression;
        }

        public void DeregisterBulletin(GameBulletin bulletinOption)
        {
            onBulletinRequirementChanged -= bulletinOption.AddProgression;
        }

        protected virtual void RequirementValueChanged(int change)
        {
            onBulletinRequirementChanged?.Invoke(change);
        }
    }
}
