using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Garden;
using NaughtyAttributes;
using GrandmaGreen.Collections;

namespace GrandmaGreen
{
    /// <summary>
    /// 
    /// </summary>
    public class GrandmaGreenGameState : MonoBehaviour
    {
        protected static GrandmaGreenGameState s_Instance;

        [SerializeField] CollectionsSO collectionsData;
        [SerializeField] GardenAreaServicer gardenServicer;
        
        [ReadOnly] int activeAreaIndex;

        public GardenAreaController ActiveArea => gardenServicer.ActiveArea;

        void Awake()
        {
            if (s_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;


            InitalizeState();
        }

        void OnDestroy()
        {
            gardenServicer.StopServices();

            if (s_Instance != this)
                return;

            s_Instance = null;
        }

        void InitalizeState()
        {
            gardenServicer.StartServices();
        }

        void Start()
        {
            LoadGardenScreen(0);
        }


        public void LoadGardenScreen(int gardenIndex)
        {
            gardenServicer.ActivateAreaController(gardenIndex);
        }


    }
}