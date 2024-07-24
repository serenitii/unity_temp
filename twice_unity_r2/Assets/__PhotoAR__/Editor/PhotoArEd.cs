using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace PhotoAr
{
    public class PhotoArEd : EditorWindow
    {
        const string iLLUU_SCENE_PATH = "Assets/__PhotoAR__/__iLLUU__/Scenes/illuu_main.unity";
        const string iLLUU_AR_SCENE_PATH = "Assets/__PhotoAR__/__iLLUU__/Scenes/illuu_ar.unity";
        const string TWICE_SCENE_PATH = "Assets/__PhotoAR__/__TwiceAR__/Scenes/twice_main.unity";
        const string TWICE_AR_SCENE_PATH = "Assets/__PhotoAR__/__TwiceAR__/Scenes/twice_ar.unity";
        const string TEST_AR_SCENE_PATH = "Assets/__PhotoAR__/Scenes/test2/ar_test2.unity";
        
        private static string _currentScenePath;
        private static string _previousScenePath;


        [MenuItem("PhotoAr/Basic Tool")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (PhotoArEd) EditorWindow.GetWindow(typeof(PhotoArEd));
            window.Show();

            //EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void Awake()
        {
            EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private static void OnActiveSceneChanged(Scene current, Scene next)
        {
            Debug.LogFormat("OnActiveSceneChanged current({0}),   next({1}) ", current.name, next.name);
        
            _previousScenePath = current.path;
        
            Debug.LogFormat("Scene path: {0} ", current);
        }

        void OnHierarchyChange()
        {
//            Debug.LogFormat("OnHierarchyChange activeScene({0}) ", EditorSceneManager.GetActiveScene().path);
            _previousScenePath = _currentScenePath;
            _currentScenePath = EditorSceneManager.GetActiveScene().path;
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            if (GUILayout.Button("iLLU Scene"))
            {
                EditorSceneManager.OpenScene(iLLUU_SCENE_PATH);
            }
            if (GUILayout.Button("iLLU AR Scene"))
            {
                EditorSceneManager.OpenScene(iLLUU_AR_SCENE_PATH);
            }
            if (GUILayout.Button("Twice Scene"))
            {
                EditorSceneManager.OpenScene(TWICE_SCENE_PATH);
            }
            if (GUILayout.Button("Twice AR Scene"))
            {
                EditorSceneManager.OpenScene(TWICE_AR_SCENE_PATH);
            } 
            if (GUILayout.Button("TEST AR Scene"))
            {
                EditorSceneManager.OpenScene(TEST_AR_SCENE_PATH);
            }  
            
            // if (GUILayout.Button("- Twice Scene -"))
            // {
            //     EditorSceneManager.OpenScene(TWICE_SCENE_PATH);
            // }

            // if (GUILayout.Button("- Previous Scene -"))
            // {
            //     Debug.LogFormat("Previous Scene( {0} ) ", _previousScenePath);
            //
            //     if (string.IsNullOrEmpty(_previousScenePath) == false)
            //         EditorSceneManager.OpenScene(_previousScenePath);
            // }

            if (GUILayout.Button("- Persistent Data Path -"))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
            // if (GUILayout.Button("- Data Path -"))
            // {
            //     EditorUtility.RevealInFinder(Application.dataPath);
            // }
            // if (GUILayout.Button("- Streaming Assets Path -"))
            // {
            //     EditorUtility.RevealInFinder(Application.streamingAssetsPath);
            // } 
            // if (GUILayout.Button("- Temporary Cache Path -"))
            // {
            //     EditorUtility.RevealInFinder(Application.temporaryCachePath);
            // }  
            
            if (GUILayout.Button("- Clear Debug Console -"))
            {
                Debug.ClearDeveloperConsole();
                
                //EditorGUIUtility.o
                //EditorUtility.op
            }
        }
    }
}