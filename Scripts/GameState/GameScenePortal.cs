using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.SceneManagement;

namespace GrandmaGreen
{
    public class GameScenePortal : MonoBehaviour
    {
        public SCENES toLoad;

        void OnTriggerEnter(Collider other)
        {
            toLoad.LoadAsync();
        }
    }
}
