using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpookuleleAudio;


namespace GrandmaGreen
{
    public class PlayAudioOnEnable : MonoBehaviour
    {
        public ASoundContainer container;
        SoundPlayer instance;
        // Start is called before the first frame update
        void OnEnable()
        {
            container.Play();
        }

        void OnDisable()
        {

        }
    }
}
