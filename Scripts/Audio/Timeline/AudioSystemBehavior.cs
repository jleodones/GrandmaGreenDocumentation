using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using SpookuleleAudio;

namespace GrandmaGreen.Audio
{
    public class AudioSystemBehavior : PlayableBehaviour
    {
        public ASoundContainer soundContainer;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            SoundPlayer player = ASoundContainer.CurrentSoundPreview;
        }
    }
}