using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Jois
{
    public class ImageBlinker : MonoBehaviour
    {
        [SerializeField] Image targetImage;
        //[SerializeField] float m_duration; 
        [SerializeField] int m_count;  
        [SerializeField] float m_alpha;  

        Tween m_tween;
        bool m_isBlink;

        [ContextMenu("Blink")]
        public void Blink(float duration)
        {
            gameObject.SetActive(true);

            duration = 0.5f;
            m_count = 100;

            m_tween?.Kill();

            m_tween = DOTween
                .To(value => { }, 0, 1, duration)
                .OnStepComplete(() => DoBlink(!m_isBlink))
                .SetLoops(m_count * 2, LoopType.Restart)
                .OnComplete(() => DoBlink(false));

            DoBlink(false);
        }

        public void CancelBlink()
        {
            if (m_tween != null)
            {
                m_tween.Kill();
                m_tween = null;
            }
            gameObject.SetActive(false);
        }

        void DoBlink(bool isBlink)
        {
            m_isBlink = isBlink;

            var color = targetImage.color;
            color.a = isBlink ? m_alpha : 1;
            targetImage.color = color;
        }
    }
}
