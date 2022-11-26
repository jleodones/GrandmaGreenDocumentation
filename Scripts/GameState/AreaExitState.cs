using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST,
    }
    [CreateAssetMenu(menuName = "GrandmaGreen/Area/AreaExitState", fileName = "AreaExitState")]
    public class AreaExitState : ScriptableObject
    {
        public Direction exitSide;
    }
}
