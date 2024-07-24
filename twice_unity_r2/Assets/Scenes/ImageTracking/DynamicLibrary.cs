using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jois;
using PhotoAr;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Adds images to the reference library at runtime.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class DynamicLibrary : MonoBehaviour
    {
        private bool _loggerEnabled;

        [Serializable]
        public class ImageData
        {
            [SerializeField, Tooltip("The source texture for the image. Must be marked as readable.")]
            Texture2D m_Texture;

            public Texture2D texture
            {
                get => m_Texture;
                set => m_Texture = value;
            }

            [SerializeField, Tooltip("The name for this image.")]
            string m_Name;

            public string name
            {
                get => m_Name;
                set => m_Name = value;
            }

            [SerializeField, Tooltip("The width, in meters, of the image in the real world.")]
            float m_Width;

            public float width
            {
                get => m_Width;
                set => m_Width = value;
            }

            public AddReferenceImageJobState jobState { get; set; }
        }

        [SerializeField, Tooltip("The set of images to add to the image library at runtime")]
        List<ImageData> m_Images;

        /// <summary>
        /// The set of images to add to the image library at runtime
        /// </summary>
        public List<ImageData> images
        {
            get => m_Images;
            set => m_Images = value;
        }

        enum State
        {
            NoImagesAdded,
            AddImagesRequested,
            AddingImages,
            Done,
            Error
        }

        State m_State;

        string m_ErrorMessage = "";

        StringBuilder m_StringBuilder = new StringBuilder();

        void OnGUI()
        {
            var fontSize = 50;
            GUI.skin.button.fontSize = fontSize;
            GUI.skin.label.fontSize = fontSize;

            float margin = 100;

            GUILayout.BeginArea(new Rect(margin, margin, Screen.width - margin * 2, Screen.height - margin * 2));

            switch (m_State)
            {
                case State.NoImagesAdded:
                {
                    if (_loggerEnabled)
                    {
                        if (GUILayout.Button("Add images"))
                        {
                            m_State = State.AddImagesRequested;
                        }
                    }

                    break;
                }
                case State.AddingImages:
                {
                    if (_loggerEnabled)
                    {
                        m_StringBuilder.Clear();
                        m_StringBuilder.AppendLine("Add image status:");
                        foreach (var image in m_Images)
                        {
                            m_StringBuilder.AppendLine($"\t{image.name}: {(image.jobState.status.ToString())}");
                        }

                        GUILayout.Label(m_StringBuilder.ToString());
                    }

                    break;
                }
                case State.Done:
                {
                    if (_loggerEnabled)
                    {
                        GUILayout.Label("All images added");
                    }

                    break;
                }
                case State.Error:
                {
                    if (_loggerEnabled)
                    {
                        GUILayout.Label(m_ErrorMessage);
                    }

                    break;
                }
            }

            GUILayout.EndArea();
        }

        void SetError(string errorMessage)
        {
            m_State = State.Error;
            m_ErrorMessage = $"Error: {errorMessage}";
            Debug.LogError($"Error: {errorMessage}");
        }

        void Update()
        {
            switch (m_State)
            {
                case State.AddImagesRequested:
                {
                    if (m_Images == null)
                    {
                        SetError("No images to add.");
                        break;
                    }

                    var manager = GetComponent<ARTrackedImageManager>();
                    if (manager == null)
                    {
                        SetError($"No {nameof(ARTrackedImageManager)} available.");
                        break;
                    }

                    // You can either add raw image bytes or use the extension method (used below) which accepts
                    // a texture. To use a texture, however, its import settings must have enabled read/write
                    // access to the texture.
                    foreach (var image in m_Images)
                    {
                        if (!image.texture.isReadable)
                        {
                            SetError($"Image {image.name} must be readable to be added to the image library.");
                            break;
                        }
                    }

                    if (manager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
                    {
                        try
                        {
                            foreach (var image in m_Images)
                            {
                                // Note: You do not need to do anything with the returned JobHandle, but it can be
                                // useful if you want to know when the image has been added to the library since it may
                                // take several frames.
                                image.jobState = mutableLibrary.ScheduleAddImageWithValidationJob(image.texture, image.name, image.width);
                            }

                            m_State = State.AddingImages;
                        }
                        catch (InvalidOperationException e)
                        {
                            SetError($"ScheduleAddImageJob threw exception: {e.Message}");
                        }
                    }
                    else
                    {
                        SetError($"The reference image library is not mutable.");
                    }

                    break;
                }
                case State.AddingImages:
                {
                    // Check for completion
                    var done = true;
                    foreach (var image in m_Images)
                    {
                        if (!image.jobState.jobHandle.IsCompleted)
                        {
                            done = false;
                            break;
                        }
                    }

                    if (done)
                    {
                        m_State = State.Done;
                    }

                    break;
                }
            }
        }

        #region Additional Features

        [SerializeField] private CacheFileManager _cacheFileMgr;

        void Start()
        {
            _cacheFileMgr = APP.CacheFileMgr;

            _loggerEnabled = APP.Config.IsReleaseVersion == false;
            
            // DownloadImages();
            // ReadImages();
        }

        void DownloadImages()
        {
            var files = new List<string>(16);

            files.Add("https://cocoa-dev-test.s3.ap-northeast-2.amazonaws.com/_dev_test/apink_01_01_sq.jpg");

            _cacheFileMgr.DownloadFiles(files, null, progress =>
            {
                // 
                //
            }, (success, path) =>
            {
                // 
                Debug.LogFormat("");
                ReadImages();

                // LoadPackageTextures();

                // UiManager.Current.HideLoading(18001);
            });
        }

        void ReadImages()
        {
            Debug.LogFormat("> ReadImages");
            string fileName = "apink_01_01_sq.jpg";
            if (_cacheFileMgr.ExistsFileCache(null, fileName))
            {
                // if (_packageImages.ContainsKey(pack.name))
                //     continue;

                //var fullPath = fileName;
                var path = _cacheFileMgr.GetFullPath(null, fileName);
                // Debug.LogFormat("trying to ReadAllBytes ({0}) ", path);
                byte[] bytesTexture = File.ReadAllBytes(path);
                if (bytesTexture.Length > 0)
                {
                    var texture = new Texture2D(0, 0);
                    texture.LoadImage(bytesTexture);
                    ImageData imageData = new ImageData
                    {
                        texture = texture,
                        name = fileName,
                        width = 0.1F
                    };
                    this.m_Images.Add(imageData);

                    m_State = State.AddImagesRequested;

                    // _packageImages.Add(pack.name, texture);
                    // _tempImages.Add(texture);
                }
            }
        }

        public void AddArImages(List<string> paths)
        {
            var manager = GetComponent<ARTrackedImageManager>();
            if (manager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                Debug.LogFormat("> DynamicLibrary.AddArImages new({0}) library.count({1}) ", paths.Count, mutableLibrary.count);
            }

            foreach (var path in paths)
            {
                // Debug.LogFormat("trying to ReadAllBytes ({0}) ", path);
                var bytesTexture = File.ReadAllBytes(path);
                if (bytesTexture.Length > 0)
                {
                    var texture = new Texture2D(0, 0);
                    texture.LoadImage(bytesTexture);
                    var cardName = Path.GetFileNameWithoutExtension(path);
                    if (cardName.EndsWith(Consts.SquaredArImageSuffix))
                        cardName = cardName.Remove(cardName.Length - Consts.SquaredArImageSuffix.Length);

                    ImageData imageData = new ImageData
                    {
                        texture = texture,
                        name = cardName,
                        width = 0.1F
                    };

                    this.m_Images.Add(imageData);
                }
                else
                    Debug.LogErrorFormat("{0} create texture error ", path);
            }

            // m_State = State.AddImagesRequested;
        }

        public void AdaptImages()
        {
            var manager = GetComponent<ARTrackedImageManager>();
            if (manager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                Debug.LogFormat("> DynamicLibrary.AdaptImages images({0}) library.count({1}) ",
                    m_Images.Count, mutableLibrary.count);
            }

            m_State = State.AddImagesRequested;
        }

        #endregion
    }
}