using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using SpookuleleAudio;

namespace GrandmaGreen.Audio
{
    [System.Serializable]
    public class AudioSystemClip : PlayableAsset, ITimelineClipAsset
    {
        public ASoundContainer SoundContainer;

        public ClipCaps clipCaps => ClipCaps.None;

        //public override double duration;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (SoundContainer == null)
                return Playable.Null;

            ScriptPlayable<AudioSystemBehavior> playable = ScriptPlayable<AudioSystemBehavior>.Create(graph);
            AudioSystemBehavior audioBehavior = playable.GetBehaviour();
            audioBehavior.soundContainer = SoundContainer;

            return playable;
        }
    }
}