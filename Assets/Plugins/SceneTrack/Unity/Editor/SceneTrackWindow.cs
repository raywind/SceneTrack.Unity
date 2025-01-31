﻿using System.ComponentModel;
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

        void OnInspectorUpdate()
        {
          Repaint();
        }

        public void OnGUI()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Auto Add To Scene"))
            {
                SceneTrack.Unity.Editor.Helper.AddToMeshRenderer();
                SceneTrack.Unity.Editor.Helper.AddToColliders();
            }
            GUILayout.Space(10);


            EditorGUILayout.LabelField("Tracked Scene Objects (" + System.CachedKnownObjects.Count + ")" , EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh", EditorStyles.miniButtonLeft))
            {
                System.CacheKnownObjects();
            }


            if (GUILayout.Button("Enable", EditorStyles.miniButtonMid))
            {
                // Update Cache List
                SceneTrack.Unity.System.CacheKnownObjects();

                foreach(var t in SceneTrack.Unity.System.CachedKnownObjects)
                {
                    t.TrackObject = true;
                }
            }

            if (GUILayout.Button("Disable", EditorStyles.miniButtonRight))
            {
                // Update Cache List
                SceneTrack.Unity.System.CacheKnownObjects();

                foreach(var t in SceneTrack.Unity.System.CachedKnownObjects)
                {
                    t.TrackObject = false;
                }
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);

            EditorGUILayout.LabelField("Takes (" + Cache.CachedFiles.Length + ")" , EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            // Update Take Information
            if (GUILayout.Button("Refresh", EditorStyles.miniButtonLeft))
            {
                Cache.GetCacheFiles();
                _outputIndex = Cache.CachedFiles.Length-1;
            }

            // Delete All requires confirmation
            if (GUILayout.Button("Clear", EditorStyles.miniButtonRight))
            {
                if (EditorUtility.DisplayDialog("Clear SceneTrack Data", "Are you sure you wish to delete all SceneTrack data?", "Yes", "No"))
                {
                    Cache.ClearFiles();
                    _outputIndex = 0;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(Cache.CachedFiles.Length == 0);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Selected Take" , EditorStyles.boldLabel);

            string[] options = Cache.CachedFiles;
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = options[i].Replace(Cache.Folder, string.Empty).TrimEnd(Cache.FileExtension.ToCharArray()).TrimEnd('.');
            }


            _outputIndex = EditorGUILayout.Popup(_outputIndex, options);

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            bool disableExport = FbxOutputRunner.IsExporting || MidiOutputRunner.IsExporting;

            if (FbxOutputRunner.IsExporting && FbxOutputRunner.IsExportSucessfull == -1)
            {
              // Exporting right now.
              EditorUtility.DisplayProgressBar("Exporting SceneTrack", "Saving Take to FBX File...", FbxOutputRunner.ExportProgress);
            }

            if (!FbxOutputRunner.IsExporting && FbxOutputRunner.IsExportSucessfull != -1)
            {
              bool didExport = (FbxOutputRunner.IsExportSucessfull == 1);
              string dstPath = FbxOutputRunner.ReceiveExport();
              EditorUtility.ClearProgressBar();

              // Exported or failed export
              if (didExport == true)
              {
                  if (EditorPrefs.GetBool("SceneTrack_OpenAfterExporting", false))
                  {
                    Application.OpenURL("file://" + dstPath);
                  }
                  SceneTrack.Unity.Log.Message("Export Successful to " + dstPath);
              }
              else if (didExport == false)
              {
                EditorUtility.DisplayDialog("Exporting SceneTrack Failed!", "The SceneTrack file could not be exported.", "Okay");
              }
            }
            

            if (GUILayout.Button("Export Animation", EditorStyles.miniButtonLeft))
            {
                string path = Path.GetFullPath(Path.Combine(Cache.Folder, Cache.CachedFiles[_outputIndex] + ".st"));
                FbxOutputRunner.Export(path);
            }





            
            if (MidiOutputRunner.IsExporting && MidiOutputRunner.IsExportSucessfull == -1)
            {
              // Exporting right now.
              EditorUtility.DisplayProgressBar("Exporting SceneTrack", "Saving Take to MIDI File...", MidiOutputRunner.ExportProgress);
            }

            if (!MidiOutputRunner.IsExporting && MidiOutputRunner.IsExportSucessfull != -1)
            {
              bool didExport = (MidiOutputRunner.IsExportSucessfull == 1);
              string dstPath = MidiOutputRunner.ReceiveExport();
              EditorUtility.ClearProgressBar();

              // Exported or failed export
              if (didExport == true)
              {
                  if (EditorPrefs.GetBool("SceneTrack_OpenAfterExporting", false))
                  {
                    Application.OpenURL("file://" + dstPath);
                  }
                  SceneTrack.Unity.Log.Message("Export Successful to " + dstPath);
              }
              else if (didExport == false)
              {
                EditorUtility.DisplayDialog("Exporting SceneTrack Failed!", "The SceneTrack file could not be exported.", "Okay");
              }
            }
            
            if (disableExport)
              GUI.enabled = false;

            if (GUILayout.Button("Export Events", EditorStyles.miniButtonMid))
            {
                string path = Path.GetFullPath(Path.Combine(Cache.Folder, Cache.CachedFiles[_outputIndex] + ".st"));
                MidiOutputRunner.Export(path);
            }

            
            
            if (VideoOutputRunner.IsExporting && VideoOutputRunner.IsExportSucessfull == -1)
            {
              // Exporting right now.
              EditorUtility.DisplayProgressBar("Exporting SceneTrack", "Saving Take to Video Files...", VideoOutputRunner.ExportProgress);
            }

            if (!VideoOutputRunner.IsExporting && VideoOutputRunner.IsExportSucessfull != -1)
            {
              bool didExport = (VideoOutputRunner.IsExportSucessfull == 1);
              string dstPath = VideoOutputRunner.ReceiveExport();
              EditorUtility.ClearProgressBar();

              // Exported or failed export
              if (didExport == true)
              {
                  if (EditorPrefs.GetBool("SceneTrack_OpenAfterExporting", false))
                  {
                    Application.OpenURL("file://" + dstPath);
                  }
                  SceneTrack.Unity.Log.Message("Export Successful to " + dstPath);
              }
              else if (didExport == false)
              {
                EditorUtility.DisplayDialog("Exporting SceneTrack Failed!", "The SceneTrack file could not be exported.", "Okay");
              }
            }
            
            if (disableExport)
              GUI.enabled = false;

            if (GUILayout.Button("Export Video", EditorStyles.miniButtonMid))
            {
                string path = Path.GetFullPath(Path.Combine(Cache.Folder, Cache.CachedFiles[_outputIndex] + ".st"));
                VideoOutputRunner.Export(path);
            }
            
            if (GUILayout.Button("Clear", EditorStyles.miniButtonRight))
            {
                if (EditorUtility.DisplayDialog("Clear SceneTrack Data",
                    "Are you sure you wish to clear " + Cache.CachedFiles[_outputIndex] + "?", "Yes", "No"))
                {
                    Cache.ClearFile(Cache.CachedFiles[_outputIndex]);
                    _outputIndex--;
                    if (_outputIndex < 0) _outputIndex = 0;
                }
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);



            EditorGUI.EndDisabledGroup();
        }
    }
}
