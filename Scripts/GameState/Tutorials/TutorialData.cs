using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace GrandmaGreen
{
    [System.Serializable]
    public class TutorialSlideshow
    {
        public SlideshowData slideshow;
        public float openDelay;
        public GameEventFlag flag;
    }


    [CreateAssetMenu(fileName = "SlideshowData", menuName = "GrandmaGreen/Tutorials/Tutorial Data", order = 0)]
    public class TutorialData : ScriptableObject
    {
        public string tutorialName;
        public Storyline storylineData;
        public TutorialSlideshow[] slideshowData;

        public uint progress => storylineData.progress;
        public int length => storylineData.requirements.Length;
        public bool isActive => storylineData.currentState == StoryState.ACTIVE;
        public bool isComplete => storylineData.currentState == StoryState.COMPLETED;

        public event System.Action<TutorialSlideshow> onPlaySlideshow;

        void OnValidate()
        {
            if (storylineData == null)
                return;

            if (slideshowData == null || slideshowData.Length != storylineData.requirements.Length)
                slideshowData = new TutorialSlideshow[storylineData.requirements.Length];
        }

    }
}