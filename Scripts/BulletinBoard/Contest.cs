using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;

namespace GrandmaGreen
{
    /// <summary>
    /// Basic contest struct. All it does is store a plant and the assumed prize money.
    /// </summary>
    public struct Contest
    {
        public Plant targetPlant;
        public int rewardMoney;
    }
}
