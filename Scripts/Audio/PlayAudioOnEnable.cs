using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpookuleleAudio;


namespace GrandmaGreen
{
    public class PlayAudioOnEnable : MonoBehaviour
    {
        public ASoundContainer container;
        public bool in3D = false;
        SoundPlayer instance;
        // Start is called before the first frame update
        void OnEnable()
        {
            if (!in3D)
                instance = container.Play();
            else
                instance = container.Play3D(transform);
        }

        void OnDisable()
        {
            instance.Stop();
        }
    }
}
