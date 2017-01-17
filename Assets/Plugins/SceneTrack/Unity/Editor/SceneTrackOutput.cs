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

        public enum FbxAxis
        {
          NX,
          NY,
          NZ,
          PX,
          PY,
          PZ
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

        public static int AxisTSX
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisTSX", (int) FbxAxis.PX);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSX", value);
            }
        }
        
        public static int AxisTSY
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisTSY", (int) FbxAxis.PY);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSY", value);
            }
        }
    
        public static int AxisTSZ
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisTSZ", (int) FbxAxis.PZ);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSZ", value);
            }
        }
    
        public static int AxisRX
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRX", (int) FbxAxis.PX);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRX", value);
            }
        }
        
        public static int AxisRY
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRY", (int) FbxAxis.PY);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRY", value);
            }
        }
    
        public static int AxisRZ
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRZ", (int) FbxAxis.PZ);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRZ", value);
            }
        }

        private static void SetupSwizzle(int node, int trsMask, int srcAxis, FbxAxis axis)
        {
          int dstAxis = 0, sign = 0;
          switch(axis)
          {
            case FbxAxis.NX:
              dstAxis = SceneTrackFbx.Axis.X;
              sign = -1;
              break;
            case FbxAxis.NY:
              dstAxis = SceneTrackFbx.Axis.Y;
              sign = -1;
              break;
            case FbxAxis.NZ:
              dstAxis = SceneTrackFbx.Axis.Z;
              sign = -1;
              break;
            case FbxAxis.PX:
              dstAxis = SceneTrackFbx.Axis.X;
              sign = 1;
              break;
            case FbxAxis.PY:
              dstAxis = SceneTrackFbx.Axis.Y;
              sign = 1;
              break;
            case FbxAxis.PZ:
              dstAxis = SceneTrackFbx.Axis.Z;
              sign = 1;
              break;
          }
          SceneTrackFbx.Conversion.SetAxisSwizzle(node, trsMask, srcAxis, dstAxis, sign);
        }

      private static void SetupSwizzles(int node, int trsMask, int x, int y, int z)
      {
        SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.X, (FbxAxis) x);
        SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Y, (FbxAxis) y);
        SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Z, (FbxAxis) z);
      }

      public static void SetupExport()
      {
        SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Translation, AxisTSX, AxisTSY, AxisTSZ);
        SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Rotation, AxisRX, AxisRY, AxisRZ);
        SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Scale,  AxisTSX, AxisTSY, AxisTSZ);
      
        SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Translation, AxisTSX, AxisTSY, AxisTSZ);
        SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Rotation, AxisRX, AxisRY, AxisRZ);
        SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Scale,  AxisTSX, AxisTSY, AxisTSZ);

        SceneTrackFbx.Conversion.SetFileVersion(Version);
      }

      public static void EditorPreferences()
      {
        var versionTemp = (FileVersion) EditorGUILayout.EnumPopup("File Version", (FileVersion) Version);
        Version = (int) versionTemp;
      
        var tsX = (FbxAxis) EditorGUILayout.EnumPopup("Translational/Scale Axis X", (FbxAxis) AxisTSX);
        AxisTSX = (int) tsX;
      
        var tsY = (FbxAxis) EditorGUILayout.EnumPopup("Translational/Scale Axis Y", (FbxAxis) AxisTSY);
        AxisTSY = (int) tsY;
      
        var tsZ = (FbxAxis) EditorGUILayout.EnumPopup("Translational/Scale Axis Z", (FbxAxis) AxisTSZ);
        AxisTSZ = (int) tsZ;
      
        var rX = (FbxAxis) EditorGUILayout.EnumPopup("Rotational Axis X", (FbxAxis) AxisRX);
        AxisRX = (int) rX;
      
        var rY = (FbxAxis) EditorGUILayout.EnumPopup("Rotational Axis Y", (FbxAxis) AxisRY);
        AxisRY = (int) rY;
      
        var rZ = (FbxAxis) EditorGUILayout.EnumPopup("Rotational Axis Z", (FbxAxis) AxisRZ);
        AxisRZ = (int) rZ;

      }

      public static string GetExportExtension()
      {
        return "fbx";
      }
    }
}