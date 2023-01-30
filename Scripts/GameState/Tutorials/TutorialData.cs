using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{

    [CreateAssetMenu(fileName = "SlideshowData", menuName = "GrandmaGreen/Tutorials/Tutorial Data", order = 0)]
    public class TutorialData : ScriptableObject
    {
        public string tutorialName;
        public Storyline storylineData;
        public SlideshowData[] slideshowData;

        public uint progress => storylineData.progress;
        public int length => storylineData.requirements.Length;
        public bool isActive => storylineData.currentState == StoryState.ACTIVE;
        public bool isComplete => storylineData.currentState == StoryState.COMPLETED;

        public event System.Action<SlideshowData> onPlaySlideshow;

        void OnValidate()
        {
            if (storylineData == null)
                return;

            if (slideshowData == null || slideshowData.Length != storylineData.requirements.Length)
                slideshowData = new SlideshowData[storylineData.requirements.Length];
        }

    }
}