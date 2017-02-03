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

        public static OutputType OutputFormat
        {
            get
            {
                return (OutputType)EditorPrefs.GetInt("SceneTrack_OutputType",0);
            }
            set
            {
                EditorPrefs.SetInt("SceneTrack_OutputType", (int)value);
            }
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
            GUIStyle miniLeftDown = new GUIStyle(EditorStyles.miniButtonLeft);
            miniLeftDown.normal = miniLeftDown.active;
            miniLeftDown.focused = miniLeftDown.active;

            GUIStyle miniMidDown = new GUIStyle(EditorStyles.miniButtonMid);
            miniMidDown.normal = miniMidDown.active;
            miniMidDown.focused = miniMidDown.active;

            GUIStyle miniRightDown = new GUIStyle(EditorStyles.miniButtonRight);
            miniRightDown.normal = miniRightDown.active;
            miniRightDown.focused = miniRightDown.active;


            EditorGUILayout.BeginHorizontal();

            if (_currentTab == 0)
            {
                if (GUILayout.Button("General", miniLeftDown))
                {
                    _currentTab = 0;
                }
            }
            else
            {
                if(GUILayout.Button("General", EditorStyles.miniButtonLeft))
                {
                    _currentTab = 0;
                }
            }

            if (_currentTab == 1)
            {
                if(GUILayout.Button("FBX", miniMidDown))
                {
                    _currentTab = 1;
                }
            }
            else
            {
                if(GUILayout.Button("FBX", EditorStyles.miniButtonMid))
                {
                    _currentTab = 1;
                }
            }

            
            if (_currentTab == 2)
            {
                if(GUILayout.Button("MIDI", miniRightDown))
                {
                    _currentTab = 2;
                }
            }
            else
            {
                if(GUILayout.Button("MIDI", EditorStyles.miniButtonRight))
                {
                    _currentTab = 2;
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();

            switch (_currentTab)
            {
                case 1:
                    FBXOutput.EditorPreferences();
                    break;
                case 2:
                    MidiOutput.EditorPreferences();
                    break;
                default:
                    OutputFramerate = (float) EditorGUILayout.FloatField("Output Framerate", OutputFramerate);
                    OutputFormat = (OutputType)EditorGUILayout.EnumPopup("Output Format", OutputFormat);
                    OpenAfterExporting = EditorGUILayout.Toggle("Open Output after Exporting", OpenAfterExporting);
                    break;
            }




            if (EditorGUI.EndChangeCheck())
            {

//                switch (OutputType)
//                {
//                    case OutputOptions.Maya:
//                        SceneTrack.Editor.Export.Maya.Preferences.ChangeCheck();
//                        break;
//                }
//
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //GUILayout.Label(Version);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        public static void SetupUnity()
        {





        }
    }
}