using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SpookuleleAudio;

namespace GrandmaGreen.Audio
{
    [TrackClipType(typeof(AudioSystemClip))]
    [TrackColor(1, 0.5f, 0.7f)]
    public class AudioSystemTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<AudioSystemMixerBehavior>.Create(graph, inputCount);
        }

        
    }
}
