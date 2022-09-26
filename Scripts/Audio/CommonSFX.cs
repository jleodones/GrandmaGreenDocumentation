using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpookuleleAudio;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Audio/CommonSFX")]
    public class CommonSFX : ScriptableObject
    {
        public void PlaySFX(ASoundContainer sound) => sound.Play();
    }
}
