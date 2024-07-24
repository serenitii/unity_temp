using UnityEngine;
using UnityEngine.UI;

namespace Jois
{
    // parent 에 AspectRatioFitter 가 있으면 사용하지 못한다 
    public class AspectRatioFitter2 : MonoBehaviour
    {
        public enum AspectMode
        {
            /// <summary>
            /// The aspect ratio is not enforced
            /// </summary>
            None,

            /// <summary>
            /// Changes the height of the rectangle to match the aspect ratio.
            /// </summary>
            WidthControlsHeight,

            /// <summary>
            /// Changes the width of the rectangle to match the aspect ratio.
            /// </summary>
            HeightControlsWidth,

            /// <summary>
            /// Sizes the rectangle such that it's fully contained within the parent rectangle.
            /// </summary>
            FitInParent,

            /// <summary>
            /// Sizes the rectangle such that the parent rectangle is fully contained within.
            /// </summary>
            EnvelopeParent
        }


        public void UpdateRatio(Image target)
        {
            var rtx = target.rectTransform;
            Debug.LogFormat("UpdateRatio  rt.sizeDelta({0}, {1}) rect.width({2}) rect.height({3}) ",
                rtx.sizeDelta.x, rtx.sizeDelta.y, rtx.rect.width, rtx.rect.height);

            if (target.sprite == null)
            {
                Debug.LogWarningFormat("({0}) sprite has been destroyed! ", target.name);
                return;
            }

            Debug.LogFormat("target.sprite.rect({0}, {1}) ", target.sprite.rect.width, target.sprite.rect.height);

            //Rect rt2 = rtx.rect;
            var height = target.sprite.rect.height * rtx.rect.width / target.sprite.rect.width;
            ///rt.rect = rt2;
            rtx.sizeDelta = new Vector2(rtx.rect.width, height);
        }
    }
}