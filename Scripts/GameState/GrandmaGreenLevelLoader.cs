using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.SceneManagement;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/LevelLoader  ")]
    public class GrandmaGreenLevelLoader : ScriptableObject
    {
        public event System.Action<SCENES> asyncLoadReq;
        public void RequestLevelLoad(SCENES scene) =>asyncLoadReq?.Invoke(scene);
    }
}
