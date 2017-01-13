using System.IO;
using UnityEngine;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{


    public static class SceneTrackPreferences
    {

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




        [PreferenceItem("SceneTrack")]
        private static void SceneTrackPreferencesItem()
        {
            EditorGUILayout.BeginVertical();


            EditorGUI.BeginChangeCheck();



            OutputFramerate = (float) EditorGUILayout.FloatField("Output Framerate", OutputFramerate);
            OutputFormat = (OutputType)EditorGUILayout.EnumPopup("Output Format", OutputFormat);



            GUILayout.Space(10);

            switch (OutputFormat)
            {
                case OutputType.FBX:
                    FBXOutput.EditorPreferences();
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();



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