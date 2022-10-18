using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.TimeLayer
{
    public interface ITimeLayer
    {
        event System.Action<int> onTick;

        float scale { get; }
        bool paused { get; }
        void Tick(double deltaTime);

        void Pause();
        void Resume(bool tickAccumulated);
    }

    /// <summary>
    /// TODO: way to animate scale
    /// </summary>

    [CreateAssetMenu(menuName = "GrandmaGreen/Time/Time Layer")]
    public class TimeLayer : ScriptableObject, ITimeLayer
    {
        [field: SerializeField] public float scale { get; private set; } = 1;

        public bool paused { get; private set; } = false;
        public event Action<int> onTick;

        double m_tickValue = 0;

        public void Tick(double deltaTime)
        {
            m_tickValue += deltaTime;

            if (m_tickValue >= scale && !paused)
            {
                int tickCount = (int)(m_tickValue / scale);

                if (tickCount > 0) onTick?.Invoke(tickCount);

                m_tickValue = 0;
            }
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            paused = true;
        }

        [ContextMenu("Resume")]
        public void Resume(bool tickAccumulated = false)
        {
            paused = false;

            if (!tickAccumulated) m_tickValue = 0;
        }

    }
}
