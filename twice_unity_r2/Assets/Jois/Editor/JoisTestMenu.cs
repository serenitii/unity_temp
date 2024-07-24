
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Jois
{
    public class JoisTools : EditorWindow
    {
        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
        
        
        SerializedProperty _events;

        private List<GameObject> _regionGuideObjs;

        private const string kRegionGuide = "RegionGuide";
        
        
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Jois/Test My Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            JoisTools window = (JoisTools) EditorWindow.GetWindow(typeof(JoisTools));
            window.Show();
        }
        
        [MenuItem("Window/Jois/JDK Path")]
        static void DoSomething()
        {
#if UNITY_2019_3_OR_NEWER && UNITY_ANDROID
            Debug.LogFormat("jdkRootPath ({0})\n", UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath);
#else
            var jdkPath = EditorPrefs.GetString("JdkPath");
            
            if (string.IsNullOrEmpty(jdkPath))
            {
                jdkPath = System.Environment.GetEnvironmentVariable("JAVA_HOME");
            }
            Debug.LogFormat("jdkRootPath ({0})\n", jdkPath);
#endif
        }
        
        
        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            {
                myBool = EditorGUILayout.Toggle("Toggle", myBool);
                myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            }
            EditorGUILayout.EndToggleGroup();
            
            if (GUILayout.Button("- Scene -"))
            {
                Debug.Log("Clicked Scene Button \n");
                EditorSceneManager.OpenScene("Assets/0-iCSN/Scenes/lgpk_outgame.unity");
                
#if UNITY_2019_3_OR_NEWER && UNITY_ANDROID
                Debug.LogFormat("jdkRootPath {0}\n", UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath);
#endif
            }
            
            if (GUILayout.Button("- Lobby -"))
            {
                var obj = GameObject.Find("/MasterCanvas/View - Lobby");
                if (obj != null)
                {
                    Debug.LogFormat("({0}) found! \n", obj.name);
                    Selection.activeObject = obj;
                }
            }

            if (GUILayout.Button("- Region Guide -"))
            {
                ToggleRegionGuideObjs(kRegionGuide);
            }
        }
        
        void OnSelectionChange()
        {
            // if (Selection.activeGameObject != null)
            //     Debug.LogFormat("({0}) selected\n", Selection.activeGameObject.name);
        }


        void GetRegionGuideObjs()
        {
            _regionGuideObjs = new  List<GameObject>(16);
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var obj in allGameObjects)
            {
                if (obj.name == kRegionGuide)
                {
                    _regionGuideObjs.Add(obj);
                }
            }
        }
        void TraverseAll()
        {
            foreach (var obj in _regionGuideObjs)
            {
                obj.SetActive(obj.activeInHierarchy == false);
            }
        }

        void ToggleRegionGuideObjs(string name)
        {
            Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].hideFlags == HideFlags.None)
                {
                    if (objs[i].name == name)
                    {
                        objs[i].gameObject.SetActive(objs[i].gameObject.activeInHierarchy == false);
                    }
                }
            }
        }
    }
}