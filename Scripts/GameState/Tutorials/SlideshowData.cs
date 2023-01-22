using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GrandmaGreen
{

    [System.Serializable]
    public struct SlideData
    {
        public Sprite image;

        [TextArea]
        public string caption;
    }

    [CreateAssetMenu(fileName = "SlideshowData", menuName = "GrandmaGreen/Tutorials/Slideshow Data", order = 0)]
    public class SlideshowData : ScriptableObject
    {
        public string title;
        public List<SlideData> slides;
    }
}
