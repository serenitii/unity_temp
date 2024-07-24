using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class MyCollectionCardItem : MonoBehaviour
    {
        [SerializeField] private RawImage _cardRawImage;

        private AspectRatioFitter _ratioFitter;

        void Start()
        {
        }

        public void SetData(VideoShape videoShape, string cardName)
        {
            gameObject.SetActive(true);
            _cardRawImage.color = Color.white;
            _cardRawImage.texture = APP.Data.CardTextures[cardName];
            _ratioFitter = _cardRawImage.GetComponent<AspectRatioFitter>();
            var isPortrait = videoShape.width < (videoShape.height + 0.01f);
            _ratioFitter.aspectMode =
                isPortrait ? AspectRatioFitter.AspectMode.WidthControlsHeight : AspectRatioFitter.AspectMode.HeightControlsWidth;

            _cardRawImage.rectTransform.localScale = new Vector3(0.75F, 0.75F, 1F);
            var portraitRatio = videoShape.width / videoShape.height; // + 0.1F;
            Debug.LogFormat("aspectRation: {0}", videoShape.width / videoShape.height);
            _ratioFitter.aspectRatio = isPortrait ? portraitRatio : videoShape.height / videoShape.width;
        }
    }
}