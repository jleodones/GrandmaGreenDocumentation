using System.Collections;
using System.Collections.Generic;
using Core.Input;
using GrandmaGreen.UI.Golems;
using GrandmaGreen.Collections;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using Sirenix.OdinInspector;
using GrandmaGreen.Garden;

namespace GrandmaGreen.Entities
{
    public class GolemInputHandler : MonoBehaviour
    {
        private GolemController golemController;
        [SerializeField] float validCheckTime = 0.05f;

        // Start is called before the first frame update
        void Awake()
        {
            golemController = GetComponent<GolemController>();
        }

    }
}
