using System.IO;
using UnityEngine;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{


    public static class SceneTrackPreferences
    {
        private static int _currentTab = 0;
        
        public static int AppendRecordFrames
        {
          get { return EditorPrefs.GetInt("SceneTrack_AppendRecordFrames", 2); }
          set { EditorPrefs.SetInt("SceneTrack_AppendRecordFrames", value); }
        }
        
        public static int MemoryReserveFramePool
        {
          get { return EditorPrefs.GetInt("SceneTrack_MemoryReserveFramePool", 0); }
          set { EditorPrefs.SetInt("SceneTrack_MemoryReserveFramePool", value); }
        }
        
        public static int MemoryReserveFrameDataPool
        {
          get { return EditorPrefs.GetInt("SceneTrack_MemoryReserveFrameDataPool", 0); }
          set { EditorPrefs.SetInt("SceneTrack_MemoryReserveFrameDataPool", value); }
        }

        public static bool OpenAfterExporting
        {
          get { return EditorPrefs.GetBool("SceneTrack_OpenAfterExporting", false); }
          set { EditorPrefs.SetBool("SceneTrack_OpenAfterExporting", value); }
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
        
        static int[] AppendRecordFramesInt = new int[]
        {
          1, 2, 4, 8, 10, 30, 60
        };
        
        static string[] AppendRecordFramesStr = new string[]
        {
          "Continous", "Flip-Flop (Default)", "4 Frames", "8 Frames", "10 Frames", "30 Frames", "60 Frames"
        };
        

        [PreferenceItem("SceneTrack")]
        private static void SceneTrackPreferencesItem()
        {
            EditorGUILayout.BeginHorizontal();
      
            if (GUILayout.Toggle(_currentTab == 0, "SceneTrack", EditorStyles.miniButtonLeft))
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
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Default", EditorStyles.miniButton))
                    {
                        AppendRecordFrames = 2;
                        MemoryReserveFramePool = 0;
                        MemoryReserveFrameDataPool = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    OpenAfterExporting = EditorGUILayout.Toggle("Open Output after Exporting", OpenAfterExporting);

                    GUILayout.Label("Advanced Settings", EditorStyles.boldLabel);

                    AppendRecordFrames = Mathf.Clamp(EditorGUILayout.IntPopup("Save Frame Time", AppendRecordFrames, AppendRecordFramesStr, AppendRecordFramesInt), 1, 60);
                    
                    if (AppendRecordFrames > 2)
                    {
                      GUILayout.BeginHorizontal();
                      GUILayout.Label("Warning:\nLonger save times will result in increased memory usage.\nThis can cause reduced recording performance or errors.", EditorStyles.miniLabel);
                      GUILayout.EndHorizontal();
                    }
                    
                    MemoryReserveFramePool = Mathf.Clamp(EditorGUILayout.IntField("Frame Pool (Reserve)", MemoryReserveFramePool), 0, 16);
                    MemoryReserveFrameDataPool = Mathf.Clamp(EditorGUILayout.IntField("Frame Data Pool (Reserve)", MemoryReserveFrameDataPool), 0, 16);


                    break;

                }
            }
      
            EditorGUILayout.EndVertical();
        }

        public static void SetupUnity()
        {





        }
    }
}