using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using Random = UnityEngine.Random;

namespace PhotoAr
{
    public class ArPlaceContainer : MonoBehaviour
    {
        // [SerializeField] private Text _devText;
        [SerializeField] private GameObject[] _rectangleShapes;
        [SerializeField] private GameObject _circleShape;
        private VideoPlayer _videoPlayer;

        private Transform _transform;
        private Transform _videoTransform;

        private int _spawnId;
        private string _cardName = "";
        private string _videoPath;
        private Vector2 _size;

        private PackageData _package;
        private CardData _card;

        private bool _comboShowing;

        private Renderer _renderer;
        
        // private bool _isPlaying;
        public int activeCounter;

        private static int _spawnCounter = 0;

        public static int currentPlayingVideos = 0;
        private static int requestedVideos;

        enum VideoState
        {
            Stopped,
            Preparing,
            Playing
        }

        private VideoState _videoState;
        // private bool _stopReserved;

        public string CardName => _cardName;
        public Vector2 CardSize => _size;

        private static readonly Vector3 HiddenScale = Vector3.one * 0.000001F;

        public CardData GetCard()
        {
            return _card;
        }

        private void Awake()
        {
            _spawnId = ++_spawnCounter;
            Debug.LogFormat("ArPlaceContainer.Awake spawn counter({0}) ", _spawnCounter);
        }

        private void OnEnable()
        {
            Debug.LogFormat(">> APC.OnEnable ({0}) id({1}) ", _cardName, _spawnId);

            // _devText.gameObject.SetActive(APP.Config.IsReleaseVersion == false);
            _transform = this.transform;
            PlayVideo(_cardName);

            // if (_card.comboEnabled)
            {
                ComboArObjManager.Current.AddArObj(this);
            }
        }

        private void OnDisable()
        {
            Debug.LogFormat(">> APC.OnDisable ({0}) id({1}) ", _cardName, _spawnId);
            StopVideo();

            // if (_card.comboEnabled)
            {
                ComboArObjManager.Current.RemoveArObj(this);
            }
        }

        public void InitOnAdded(CardData card, ARTrackedImage item) // string packageName, string cardName)
        {
            Debug.LogFormat(">> ({0}) prefab has been instantiated ! ", card.name);
            _cardName = card.name;
            _size = new Vector2(card.videoShape.width, card.videoShape.height);
            _package = APP.Data.GetPackage(card.packageName);
            _card = card;
            // _devText.text = string.Format("spawnId({0}), name({1}) ", _spawnId, _cardName);
            _transform = this.transform;
            _renderer = GetComponent<Renderer>();

            var shape = card.videoShape;

            SetVideoShapeType(shape);

            foreach (var package in APP.Data.Packages)
            {
                foreach (var combo in package.Value.combos)
                {
                    var leftSide = combo.cards[0] == _cardName;
                    if (leftSide)
                    {
                        GameObject detectorObj = Instantiate(APP.Config._leftTarget, _videoTransform);
                        detectorObj.transform.localScale = new Vector3(5F, 1F, 1F); // * 500F;
                        detectorObj.transform.localRotation = Quaternion.identity;
                        detectorObj.transform.localPosition = new Vector3(-10F, 0, 0);
                        detectorObj.GetComponent<Renderer>().enabled = false;
                        break;
                    }
            
                    var directionDetector = combo.cards[1] == _cardName;
                    if (directionDetector)
                    {
                        GameObject targetObj = Instantiate(APP.Config._directionDetector, _videoTransform);
                        targetObj.transform.localScale = new Vector3(2F, 1F, 4F); // * 500F;
                        targetObj.transform.localRotation = Quaternion.identity;
                        targetObj.transform.localPosition = Vector3.zero;
                        targetObj.GetComponent<Renderer>().enabled = false;
                        break;
                    }
                }
            }
            //
            // foreach (var combo in _package.combos)
            // {
            //     var leftSide = combo.cards[0] == _cardName;
            //     if (leftSide)
            //     {
            //         GameObject detectorObj = Instantiate(APP.Config._leftTarget, _videoTransform);
            //         detectorObj.transform.localScale = new Vector3(5F, 1F, 1F);
            //         detectorObj.transform.localRotation = Quaternion.identity;
            //         detectorObj.transform.localPosition = new Vector3(-10F, 0, 0);
            //         // detectorObj.GetComponent<Renderer>().enabled = false;
            //         break;
            //     }
            //
            //     var directionDetector = combo.cards[1] == _cardName;
            //     if (directionDetector)
            //     {
            //         GameObject targetObj = Instantiate(APP.Config._directionDetector, _videoTransform);
            //         targetObj.transform.localScale = new Vector3(2F, 1F, 4F);
            //         targetObj.transform.localRotation = Quaternion.identity;
            //         targetObj.transform.localPosition = Vector3.zero;
            //         // targetObj.GetComponent<Renderer>().enabled = false;
            //         break;
            //     }
            // }


            Debug.LogFormat("videoShape {0} ", shape);

            _videoPath = APP.Data.PackagesInApp.GetCachedVideoPathForPlay(card.packageName, card.name, false);

            gameObject.SetActive(true);
        }

        void SetVideoShapeType(VideoShape shape)
        {
            Debug.LogFormat("SetShapeType shape({0}) options({1}) ", shape.shape, shape.shapeOptions);

            GameObject obj;
            switch (shape.shape)
            {
                // Circle
                case 1:
                {
                    obj = _circleShape;
                    shape.shapeOptions = 0;
                    break;
                }
                // Rectangle
                default:
                {
                    if (shape.shapeOptions > _rectangleShapes.Length)
                        shape.shapeOptions = _rectangleShapes.Length - 1;
                    obj = _rectangleShapes[shape.shapeOptions];
                    break;
                }
            }

            obj.SetActive(true);
            // if (_videoPlayer == null)
            _videoPlayer = obj.GetComponent<VideoPlayer>();
            _videoTransform = _videoPlayer.transform;

            Debug.LogFormat("shape object name({0}) ", obj.name);
            Debug.AssertFormat(_videoPlayer != null, "_videoPlayer is NULL ! ");
        }

        public void PlayVideo(string videoFileName, int comboCount = 1)
        {
            Debug.AssertFormat(_videoState == VideoState.Stopped, "({0}) is already playing ", videoFileName);

            ++requestedVideos;
            if (++currentPlayingVideos > 2 && comboCount == 1)
            {
                // _videoPlayer.gameObject.SetActive(false);
                // return;
            }

            _videoState = VideoState.Preparing;

            var videoPath = APP.Data.PackagesInApp.GetCachedVideoPathForPlay(_card.packageName, videoFileName, false);

            Debug.LogFormat("> PlayVideo ({0}) ( {1} ) - {2} ({3}) ", videoFileName, comboCount, videoPath, videoPath);

            APP.Api.RequestLog(LogActionNames.PlayAR, $"{_card.name}");
            
            StartCoroutine(PlayVideoImpl(videoPath, comboCount));
        }

        private static float _xx;

        void SetSize(VideoShape shape, int comboCount)
        {
            var offsetX = shape.offsetX;
            var offsetY = shape.offsetY;
            //var modelScale = shape.shapeOptions > 0 ? 500F
            var scale = 0.1F * shape.scale;
            Debug.LogFormat("rot: {0} ", _videoTransform.localRotation);

            Debug.LogFormat("shape({0}) opt({1}) w,h({2}, {3}) ", shape.shape, shape.shapeOptions, shape.width,
                shape.height);

            var blenderModeled = shape.shapeOptions > 0 || shape.shape == 1;
            
            switch (comboCount)
            {
                case 1: // normal
                {
                    _videoTransform.localScale = blenderModeled
                        ? new Vector3(shape.width, shape.height, 1F) * scale
                        : new Vector3(shape.width, 1F, shape.height) * scale;
                    _videoTransform.localPosition = new Vector3(offsetX, 0, offsetY);
                    break;
                }
                case 2:
                {
                    // var localScale = new Vector3(shape.width * 2F, 1F, shape.height) * scale;
                    var localScale = !blenderModeled 
                        ? new Vector3(shape.width * 2F, 1F, shape.height) * scale
                        : new Vector3(shape.width * 2F, shape.height, 1F) * scale;
                    _videoTransform.localScale = localScale;
                    _videoTransform.localPosition = new Vector3(offsetX + shape.width * 0.5F, 0, offsetY);
                    break;
                }
                case 3:
                {
                    // normal
                    // _videoTransform.localScale = new Vector3(shape.width * 3F, 1F, shape.height) * scale;
                    // modeled
                    _videoTransform.localScale = !blenderModeled
                        ?new Vector3(shape.width * 3F, 1F, shape.height) * scale
                        : new Vector3(shape.width * 3F,  shape.height, 1F) * scale;
                    _videoTransform.localPosition = new Vector3(offsetX , 0, offsetY);
                    break;
                }
                case 4:
                case 5:
                case 6:
                {
                    _videoTransform.localScale = new Vector3(shape.width * 3F, 1F, shape.height * 2F) * scale;
                    _videoTransform.localPosition =
                        new Vector3(offsetX + shape.width, 0, offsetY - shape.height * 0.5F);
                    break;
                }
            }

            Debug.LogFormat("video scale: {0} ", _videoTransform.localScale);
        }

        IEnumerator PlayVideoImpl(string url, int comboCount)
        {
            // _transform.localScale = Vector3.one * 0.00001F;
            // _videoPlayer.gameObject.SetActive(true);
            _renderer.enabled = true;
            _videoPlayer.url = url;
            _videoPlayer.isLooping = true;
            _videoPlayer.SetDirectAudioVolume(0, _package.mute ? 0 : 1F);
            _videoPlayer.transform.localScale = HiddenScale;
            _videoPlayer.Prepare();

            for (var i = 0; i < 30; ++i)
            {
                if (_videoState == VideoState.Stopped)
                    yield break;

                if (_videoPlayer.isPrepared)
                {
                    Debug.LogFormat("video prepared ({0}) ", i);
                    break;
                }

                yield return new WaitForSeconds(0.1F);
            }

            if (_videoState == VideoState.Preparing)
            {
                _videoState = VideoState.Playing;
                Debug.LogFormat("- Play ({0}) ", Path.GetFileNameWithoutExtension(url)); // _cardName);
                // _videoPlayer.transform.localScale = Vector3.one * 0.5F;
                SetSize(_card.videoShape, comboCount);
                // _videoPlayer.GetComponent<Renderer>().enabled = true;
                _videoPlayer.Play();
            }
        }


        public void StopVideo()
        {
            if (VideoState.Stopped != _videoState)
            {
                Debug.LogFormat("- Stop ({0}) ", _cardName);
                // --requestedVideos;

                --currentPlayingVideos;
                _videoState = VideoState.Stopped;
                _videoTransform.localScale = HiddenScale;
                _videoPlayer.Stop();
                _renderer.enabled = false;

                // _videoPlayer.gameObject.SetActive(false);
            }
        }

        // private bool _pausedByCombo;

        public void OnComboFound(string anchorCardName, List<string> targets, string filename, int comboCount)
        {
            // 1. Stop 
            if (targets.Contains(_cardName))
            {
                Debug.LogFormat("trying to stop video for combo enabled ({0}) ", _cardName);
                StopVideo();
            }

            // 2. Play
            if (_cardName == anchorCardName)
            {
                PlayVideo(filename, comboCount);
            }
        }

        public void OnComboLost()
        {
            // Combo 가 깨져서 원래 비디오를 플레이 
            StopVideo();
            PlayVideo(_cardName, 1);
        }
    }
}