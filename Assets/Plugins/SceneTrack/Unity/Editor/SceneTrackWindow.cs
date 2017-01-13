using System.ComponentModel;
using System.IO;
using System.Reflection;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{
    public class SceneTrackWindow : EditorWindow
    {
        public static GUIContent Title
        {
            get { return _title ?? (_title = new GUIContent("Scene Track")); }

        }
        private static GUIContent _title;
        private static int _outputIndex = 0;


        [MenuItem ("Window/Scene Track")]
        private static void Init ()
        {
            var window = (SceneTrackWindow)GetWindow(typeof(SceneTrackWindow));

            window.titleContent = Title;

            Cache.GetCacheFiles();
            System.CacheKnownObjects();

            window.Show();
        }

        public void OnFocus()
        {
            Cache.GetCacheFiles();
            _outputIndex = Cache.CachedFiles.Length-1;
            System.CacheKnownObjects();
        }

        public void OnGUI()
        {
            GUILayout.Space(5);


            EditorGUILayout.LabelField("Tracked Scene Objects (" + System.CachedKnownObjects.Count + ")" , EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                System.CacheKnownObjects();
            }

            EditorGUILayout.EndHorizontal();



            GUILayout.Space(10);



            EditorGUILayout.LabelField("Takes (" + Cache.CachedFiles.Length + ")" , EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("Refresh", EditorStyles.miniButtonLeft))
            {
                Cache.GetCacheFiles();
                _outputIndex = Cache.CachedFiles.Length-1;
            }

            if (GUILayout.Button("Clear", EditorStyles.miniButtonRight))
            {
                Cache.ClearFiles();
                _outputIndex = 0;
            }

            EditorGUILayout.EndHorizontal();



            GUILayout.Space(10);


            EditorGUILayout.LabelField("Output" , EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(Cache.CachedFiles.Length == 0);

            string[] options = Cache.CachedFiles;
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = options[i].Replace(Cache.Folder, string.Empty).TrimEnd(Cache.FileExtension.ToCharArray());
            }

            _outputIndex = EditorGUILayout.Popup("Take", _outputIndex, options);

            if (GUILayout.Button("Export"))
            {
                Output.Export(Cache.CachedFiles[_outputIndex]);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
