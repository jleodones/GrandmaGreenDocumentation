using System.Collections;
using GrandmaGreen.Collections;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    public interface IEmotionAgent
    {
        public void ChangeEmotion(ushort CharID, ushort EmoID);
        public void ChangeEmotionInTime(ushort CharID, ushort EmoID, float time);

    }
}