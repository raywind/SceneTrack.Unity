using System.IO;
using UnityEngine;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{


    public static class SceneTrackPreferences
    {
        private static int _currentTab = 0;


        public static bool OpenAfterExporting
        {
          get { return EditorPrefs.GetBool("SceneTrack_OpenAfterExporting", false); }
          set { EditorPrefs.SetBool("SceneTrack_OpenAfterExporting", value); }
        }
    
        public static float OutputFramerate
        {
            get
            {
                return EditorPrefs.GetFloat("SceneTrack_OutputFramerate", 24.25f);
            }
            set
            {
                EditorPrefs.SetFloat("SceneTrack_OutputFramerate", value);
            }
        }

        public static int OutputAxisSettings
        {
            get
            {
                return EditorPrefs.GetInt("SceneTrack_OutputAxisSettings", 1);
            }
            set
            {
                EditorPrefs.SetInt("SceneTrack_OutputAxisSettings", value);
            }
        }
    
        [PreferenceItem("SceneTrack")]
        private static void SceneTrackPreferencesItem()
        {
            EditorGUILayout.BeginHorizontal();
      
            if (GUILayout.Toggle(_currentTab == 0, "General", EditorStyles.miniButtonLeft))
            {
              _currentTab = 0;
            }
            
            if (GUILayout.Toggle(_currentTab == 1, "Animation", EditorStyles.miniButtonMid))
            {
              _currentTab = 1;
            }
            
            if (GUILayout.Toggle(_currentTab == 2, "Events", EditorStyles.miniButtonMid))
            {
              _currentTab = 2;
            }
            
            if (GUILayout.Toggle(_currentTab == 3, "Video", EditorStyles.miniButtonRight))
            {
              _currentTab = 3;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            switch (_currentTab)
            {
                case 1:
                    FBXOutput.EditorPreferences();
                    break;
                case 2:
                    MidiOutput.EditorPreferences();
                    break;
                case 3:
                    VideoOutput.EditorPreferences();
                    break;
                default:
                    OutputFramerate = (float) EditorGUILayout.FloatField("Output Framerate", OutputFramerate);
                    OpenAfterExporting = EditorGUILayout.Toggle("Open Output after Exporting", OpenAfterExporting);
                    break;
            }
      
            EditorGUILayout.EndVertical();
        }

        public static void SetupUnity()
        {





        }
    }
}