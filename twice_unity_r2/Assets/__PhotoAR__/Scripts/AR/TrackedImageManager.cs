using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PhotoAr
{
    public class TrackedImageManager : MonoBehaviour
    {
        [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
        [SerializeField] private GameObject _arPlaneContainerPrefab;
        private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();
        private Dictionary<string, int> _instantiatedCount = new Dictionary<string, int>();
        
        public static TrackedImageManager Current;
        private void OnEnable()
        {
            _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChange;
            Current = this;
        }

        private void OnDisable()
        {
            _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChange;
        }
        public IList<string> GetActiveObjects()
        {
            var list = new List<string>();
            foreach (var item in _instantiatedPrefabs)
            {
                if (item.Value.activeInHierarchy)
                    list.Add(item.Key);
            }

            return list;
        }
        private void OnTrackedImagesChange(ARTrackedImagesChangedEventArgs args)
        {
            // Good reference: https://forum.unity.com/threads/arfoundation-2-image-tracking-with-many-ref-images-and-many-objects.680518/#post-4668326
            // https://github.com/Unity-Technologies/arfoundation-samples/issues/261#issuecomment-555618182

            // Go through all tracked images that have been added
            // (-> new markers detected)
            foreach (var item in args.added)
            {
                Debug.LogFormat(">> added: ({0}) item({1}) ", item.referenceImage.name, item.name);

                var cardName = item.referenceImage.name;
                if (!_instantiatedPrefabs.ContainsKey(cardName))
                {
                    // Found a corresponding prefab for the reference image, and it has not been instantiated yet
                    // -> new instance, with the ARTrackedImage as parent (so it will automatically get updated
                    // when the marker changes in real-life)
                    var newPrefab = Instantiate(_arPlaneContainerPrefab, item.transform);
                    _instantiatedPrefabs[cardName] = newPrefab;
                    _instantiatedCount[cardName] = 0;

                    Debug.LogFormat("OnTrackedImagesChange.Added size({0}) extents({1}) ", item.size, item.extents);
                    
                    CardData card = APP.Data.GetCardByName(cardName);
                    newPrefab.GetComponent<ArPlaceContainer>().InitOnAdded(card, item);
                }
            }

            // Disable instantiated prefabs that are no longer being actively tracked
            foreach (var item in args.updated)
            {
                // Debug.LogFormat(">> updated: ({0}) pos({1}) ", item.referenceImage.name, item.transform.position);
                
                // [Tracking] -> [Limited]
                // if (item.trackingState != TrackingState.Tracking)
                // {
                //     Debug.LogFormat("({0}) tracking state({1}) ", item.referenceImage.name, item.trackingState);
                // }

                // _instantiatedPrefabs[item.referenceImage.name].SetActive(item.trackingState == TrackingState.Tracking);
                
#if false
                var isActive = item.trackingState == TrackingState.Tracking;
                if (!isActive)
                {
                    _instantiatedPrefabs[item.referenceImage.name].GetComponent<ArPlaceContainer>().StopVideo();
                }
                if (_instantiatedPrefabs[item.referenceImage.name].activeInHierarchy != isActive)
                    _instantiatedPrefabs[item.referenceImage.name].SetActive(isActive);
#else
                var isTracking = item.trackingState == TrackingState.Tracking;
                if (isTracking)
                {
                    _instantiatedCount[item.referenceImage.name] = _instantiatedCount[item.referenceImage.name] + 1;
                    
                    if (_instantiatedCount[item.referenceImage.name] > 2)
                        _instantiatedPrefabs[item.referenceImage.name].SetActive(true);
                }
                else
                {
                    _instantiatedCount[item.referenceImage.name] = 0;
                    
                    _instantiatedPrefabs[item.referenceImage.name].GetComponent<ArPlaceContainer>().StopVideo();
                    _instantiatedPrefabs[item.referenceImage.name].SetActive(false);
                }
                
                // if (!isTracking)
                // {
                //     _instantiatedPrefabs[item.referenceImage.name].GetComponent<ArPlaceContainer>().StopVideo();
                // }
                // if (_instantiatedPrefabs[item.referenceImage.name].activeInHierarchy != isTracking)
                //     _instantiatedPrefabs[item.referenceImage.name].SetActive(isTracking);
#endif
            }

            // Remove is called if the subsystem has given up looking for the trackable again.
            // (If it's invisible, its tracking state would just go to limited initially).
            // Note: ARCore doesn't seem to remove these at all; if it does, it would delete our child GameObject
            // as well.
            foreach (var item in args.removed)
            {
                Debug.LogFormat(">> removed: ({0}) ", item.referenceImage.name);
            }
        }
    }
}