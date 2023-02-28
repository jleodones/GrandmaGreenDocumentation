using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class ResponsiveBulletinUIControl: UIDisplayBase
    {
        void Start()
        {
            float screenRatio = Camera.main.aspect;
            float pad = 10.0f;
            VisualElement bulletin = m_rootVisualElement.hierarchy.ElementAt(1);
            VisualElement exit = m_rootVisualElement.hierarchy.ElementAt(3);



            bulletin.style.width = screenRatio switch
            {
                float f when f >= 2.1f => 1500.0f,
                float f when f >= 1.7f => 1700.0f,
                float f when f >= 1.43f => 1603.0f,
                float f when f >= 1.33f => 1603.0f,
                _ => 1500.0f
            };

            bulletin.style.height = screenRatio switch
            {
                float f when f >= 2.1f => 1100.0f,
                float f when f >= 1.7f => 1100.0f,
                float f when f >= 1.43f => 1250.0f,
                float f when f >= 1.33f => 1250.0f,
                _ => 1100.0f
            };

            foreach (VisualElement child in m_rootVisualElement.hierarchy.ElementAt(1).ElementAt(0).Children())
            {
                child.style.width = screenRatio switch
                {
                    float f when f >= 2.1f => 500.0f,
                    float f when f >= 1.7f => 500.0f,
                    float f when f >= 1.43f => 500.0f,
                    float f when f >= 1.33f => 500.0f,
                    _ => 500.0f
                };

                child.style.height = screenRatio switch
                {
                    float f when f >= 2.1f => 500.0f,
                    float f when f >= 1.7f => 500.0f,
                    float f when f >= 1.43f => 500.0f,
                    float f when f >= 1.33f => 500.0f,
                    _ => 500.0f
                };
            }

            if (bulletin.style.width == 1700.0f)
            {
                bulletin.ElementAt(0).ElementAt(1).style.left = 150.0f;
                bulletin.ElementAt(0).ElementAt(2).style.right = 150.0f;
            }

            exit.style.left = screenRatio switch
            {
                float f when f >= 2.16f => 1790.0f,
                float f when f >= 2.1f => 1750.0f,
                float f when f >= 2.08f => 1730.0f,
                float f when f == 2.0f => 1690.0f,
                float f when f > 2.0f => 1720.0f,
                float f when f >= 1.7f => 1580.0f,
                float f when f >= 1.6f => 1670.0f,
                float f when f >= 1.43f => 1670.0f,
                float f when f >= 1.33f => 1650.0f,
                _ => 770.0f
            };

            exit.style.top = screenRatio switch
            {
                float f when f >= 2.16f => 60.0f,
                float f when f >= 2.1f => 60.0f,
                float f when f >= 2.08f => 60.0f,
                float f when f == 2.0f => 60.0f,
                float f when f > 2.0f => 60.0f,
                float f when f >= 1.7f => 60.0f,
                float f when f >= 1.6f => 60.0f,
                float f when f >= 1.43f => 200.0f,
                float f when f >= 1.33f => 280.0f,
                _ => 370.0f
            };

            exit.style.width = screenRatio switch
            {
                float f when f >= 2.1f => 100.0f,
                float f when f >= 1.7f => 100.0f,
                float f when f >= 1.43f => 100.0f,
                float f when f >= 1.33f => 120.0f,
                _ => 80.0f
            };

            exit.style.height = screenRatio switch
            {
                float f when f >= 2.1f => 100.0f,
                float f when f >= 1.7f => 100.0f,
                float f when f >= 1.43f => 100.0f,
                float f when f >= 1.33f => 120.0f,
                _ => 80.0f
            };

            Debug.Log(screenRatio);
        }
    }
}
