using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{
    public static class Output
    {
        /// <summary>
        /// Export file to selected export format
        /// </summary>
        /// <param name="sourcePath">Full path to the source file to use in the conversion</param>
        public static void Export(string sourcePath)
        {




            // Get destination folder
            var outputFile = UnityEditor.EditorUtility.SaveFilePanel(
                "Destination File",
                Environment.SpecialFolder.DesktopDirectory.ToString(),
                Path.GetFileNameWithoutExtension(sourcePath) + "." + FBXOutput.GetExportExtension(),
                FBXOutput.GetExportExtension());

            if (!string.IsNullOrEmpty(outputFile))
            {
                FBXOutput.SetupExport();

                //TODO: EntryPointNotFoundException: fbxConvertSceneTrackFile
                int response = SceneTrackFbx.Conversion.ConvertSceneTrackFile(new StringBuilder(sourcePath),
                    new StringBuilder(outputFile));
                if (response == 0)
                {
                  UnityEngine.Debug.Log("FBX Conversion Successfull");
                }
                else
                {
                  UnityEngine.Debug.Log("FBX Conversion Failed.");
                }
            }
        }
    }

    public enum OutputType
    {
        FBX = 0
    }

    public static class FBXOutput
    {

        public static class FbxFileVersion
        {
            public const int FBX_53_MB55 = (0);
            public const int FBX_60_MB60 = (1);
            public const int FBX_200508_MB70 = (2);
            public const int FBX_200602_MB75 = (3);
            public const int FBX_200608 = (4);
            public const int FBX_200611 = (5);
            public const int FBX_200900 = (6);
            public const int FBX_200900v7 = (7);
            public const int FBX_201000 = (8);
            public const int FBX_201100 = (9);
            public const int FBX_201200 = (10);
            public const int FBX_201300 = (11);
            public const int FBX_201400 = (12);
            public const int FBX_201600 = (13);
        }

        public enum FileVersion
        {
            FBX_53_MB55 = 0,
            FBX_60_MB60 = 1,
            FBX_200508_MB70 = 2,
            FBX_200602_MB75 = 3,
            FBX_200608 = 4,
            FBX_200611 = 5,
            FBX_200900 = 6,
            FBX_200900v7 = 7,
            FBX_201000 = 8,
            FBX_201100 = 9,
            FBX_201200 = 10,
            FBX_201300 = 11,
            FBX_201400 = 12,
            FBX_201600 = 13
        }


        public static int Version
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_FBX_Version", 12);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_FBX_Version", value);
            }
        }

        public static void SetupExport()
        {
            SceneTrackFbx.Conversion.SetFileVersion(Version);
        }

        public static void EditorPreferences()
        {
            var temp = (FileVersion)EditorGUILayout.EnumPopup("File Version", (FileVersion) Version);

            Version = (int)temp;
        }

        public static string GetExportExtension()
        {
            return "fbx";
        }
    }
}