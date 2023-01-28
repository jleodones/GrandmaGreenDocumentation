using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    public class SpriteOcclusionHandler : MonoBehaviour
    {
        static float FADE_TIME = 0.25f;
        static float FADE_ALPHA = 0.25f;

        SpriteRenderer SpriteRenderer;

        public EntityController GrandmaController;
        public Rect BoundingRect;

        public bool Occlude;

        Vector3 BoundPosition => transform.position + new Vector3(BoundingRect.x, BoundingRect.y);
        Vector3 BoundSize => new Vector3(BoundingRect.width, BoundingRect.height, 1.0f);

        float BoundLeftEdge => BoundPosition.x - BoundSize.x/2;
        float BoundRightEdge => BoundPosition.x + BoundSize.x/2;
        float BoundTopEdge => BoundPosition.y + BoundSize.y/2;
        float BoundBottomEdge => BoundPosition.y - BoundSize.y/2;

        bool GrandmaIsInsideMe = true;

        void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            Vector3 grandmaPosition = GrandmaController.entity.transform.position;

            if(GrandmaIsInsideMe && !IsPositionWithinBounds(grandmaPosition))
            {
                Fade(false);
                GrandmaIsInsideMe = false;
            } else if(!GrandmaIsInsideMe && IsPositionWithinBounds(grandmaPosition))
            {
                Fade(true);
                GrandmaIsInsideMe = true;
            }
        }

        bool IsPositionWithinBounds(Vector3 position)
        {
            return position.x > BoundLeftEdge && position.x < BoundRightEdge && position.y > BoundBottomEdge && position.y < BoundTopEdge;
        }


        IEnumerator FadeRoutine;

        void Fade(bool fadeIn)
        {
            if(FadeRoutine != null)
            {
                StopCoroutine(FadeRoutine);
            }
            FadeRoutine = CR_Fade(fadeIn);
            StartCoroutine(FadeRoutine);
        }

        IEnumerator CR_Fade(bool fadeIn)
        {

            Color color = SpriteRenderer.color;

            for(float t = 0; t < 1.0f; t += Time.deltaTime / FADE_TIME)
            {
                color.a = Mathf.Lerp(FADE_ALPHA, 1.0f, fadeIn ? (1-t) : (t));
                SpriteRenderer.color = color;
                yield return null;
            }

            color.a = fadeIn ? FADE_ALPHA : 1.0f;
            SpriteRenderer.color = color;
        }


#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if(Occlude)
            {
                Gizmos.DrawWireCube(BoundPosition, BoundSize);
            }
        }

#endif
    }
}