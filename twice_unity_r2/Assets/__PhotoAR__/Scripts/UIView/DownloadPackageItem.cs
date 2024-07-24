using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PhotoAr
{
    public class DownloadPackageItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _artistText;
        [SerializeField] private Button _downloadButton;
        [SerializeField] private Button _clearButton;

        public void SetStates(Texture2D albumTexture, string title, string artist, bool hasDownloaded)
        {
            _rawImage.texture = albumTexture;
            _titleText.text = title;
            _artistText.text = artist;
            _downloadButton.gameObject.SetActive(hasDownloaded == false);
            _clearButton.gameObject.SetActive(hasDownloaded);
        }
    }
}