using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using SceneTrackFbx;
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

        public static int AxisTX
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
        
        public static int AxisTY
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
    
        public static int AxisTZ
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
    
        public static int AxisSX
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisSX", (int) FbxAxis.PX);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisSX", value);
            }
        }
        
        public static int AxisSY
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisSY", (int) FbxAxis.PY);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisSY", value);
            }
        }
    
        public static int AxisSZ
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisSZ", (int) FbxAxis.PZ);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisSZ", value);
            }
        }
    
        public static float RotationAddX
        {
            get
            {
                return UnityEditor.EditorPrefs.GetFloat("SceneTrack_RotationAddX", 0.0f);
            }
            set
            {
                UnityEditor.EditorPrefs.SetFloat("SceneTrack_RotationAddX", value);
            }
        }
        
        public static float RotationAddY
        {
            get
            {
                return UnityEditor.EditorPrefs.GetFloat("SceneTrack_RotationAddY", 0.0f);
            }
            set
            {
                UnityEditor.EditorPrefs.SetFloat("SceneTrack_RotationAddY", value);
            }
        }
    
        public static float RotationAddZ
        {
            get
            {
                return UnityEditor.EditorPrefs.GetFloat("SceneTrack_RotationAddZ", 0.0f);
            }
            set
            {
                UnityEditor.EditorPrefs.SetFloat("SceneTrack_RotationAddZ", value);
            }
        }

        public static float ScaleMultiplyX
        {
            get
            {
                return UnityEditor.EditorPrefs.GetFloat("SceneTrack_ScaleMultiplyX", 1.0f);
            }
            set
            {
                UnityEditor.EditorPrefs.SetFloat("SceneTrack_ScaleMultiplyX", value);
            }
        }
        
        public static float ScaleMultiplyY
        {
            get
            {
                return UnityEditor.EditorPrefs.GetFloat("SceneTrack_ScaleMultiplyY", 1.0f);
            }
            set
            {
                UnityEditor.EditorPrefs.SetFloat("SceneTrack_ScaleMultiplyY", value);
            }
        }
    
        public static float ScaleMultiplyZ
        {
            get
            {
                return UnityEditor.EditorPrefs.GetFloat("SceneTrack_ScaleMultiplyZ", 1.0f);
            }
            set
            {
                UnityEditor.EditorPrefs.SetFloat("SceneTrack_ScaleMultiplyZ", value);
            }
        }
    
        public static int RotationOrder
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_RotationOrder", 0);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_RotationOrder", value);
            }
        }

        public static int ReferenceFrame
        {
            get
            {
                return UnityEditor.EditorPrefs.GetInt("SceneTrack_ReferenceFrame", 0);
            }
            set
            {
                UnityEditor.EditorPrefs.SetInt("SceneTrack_ReferenceFrame", value);
            }
        }

        public static bool ReverseTriangles
        {
            get
            {
                return UnityEditor.EditorPrefs.GetBool("SceneTrack_RewindTriangle", false);
            }
            set
            {
                UnityEditor.EditorPrefs.SetBool("SceneTrack_RewindTriangle", value);
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
          SceneTrackFbx.Settings.SetAxisSwizzle(node, trsMask, srcAxis, dstAxis, sign);
        }

      private static void SetupSwizzles(int node, int trsMask, int x, int y, int z)
      {
        SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.X, (FbxAxis) x);
        SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Y, (FbxAxis) y);
        SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Z, (FbxAxis) z);
      }

      public static void SetupExport()
      {
        SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Translation, AxisTX, AxisTY, AxisTZ);
        SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Rotation, AxisRX, AxisRY, AxisRZ);
        SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Scale,  AxisSX, AxisSY, AxisSZ);
      
        SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Translation, AxisTX, AxisTY, AxisTZ);
        SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Rotation, AxisRX, AxisRY, AxisRZ);
        SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Scale,  AxisSX, AxisSY, AxisSZ);

        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Rotation, SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Add, RotationAddX);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Rotation, SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Add, RotationAddY);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Rotation, SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Add, RotationAddZ);
      
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Scale, SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Multiply, ScaleMultiplyX);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Scale, SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Multiply, ScaleMultiplyY);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.TRS.Scale, SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Multiply, ScaleMultiplyZ);
      
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Rotation, SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Add, RotationAddX);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Rotation, SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Add, RotationAddY);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Rotation, SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Add, RotationAddZ);
      
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Scale, SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Multiply, ScaleMultiplyX);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Scale, SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Multiply, ScaleMultiplyY);
        SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.TRS.Scale, SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Multiply, ScaleMultiplyZ);
        
        SceneTrackFbx.Settings.SetAxisRotationOrder(RotationOrder);
        SceneTrackFbx.Settings.SetReferenceFrame(ReferenceFrame);
        SceneTrackFbx.Settings.SetReverseTriangleWinding(ReverseTriangles ? 1 : 0);

        SceneTrackFbx.Settings.SetFileVersion(Version);
      }
    
      static int[] FbxVersionInt = new int[]
      {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
      };

      static string[] FbxVersionStr = new string[]
      {
        "FBX 5.3 MB55",
        "FBX 6.0 MB60",
        "FBX 2005.08 MB70",
        "FBX 2006.02 MB75",
        "FBX 2006.08",
        "FBX 2006.11",
        "FBX 2009",
        "FBX 2009v7",
        "FBX 2010",
        "FBX 2011",
        "FBX 2012",
        "FBX 2013",
        "FBX 2014",
        "FBX 2016"
      };

      static int[] AxisPresets= new int[]
      {
        0, 1, 2
      };

      static String[] AxisPresetsStr = new string[]
      {
        "Presets...",
        "Default",
        "Unity To Fbx"
      };

      static int[] AxisInts = new int[]
      {
        0, 1, 2, 3, 4, 5
      };

      static String[] AxisStr = new string[]
      {
        "-X", "-Y", "-Z", "+X", "+Y", "+Z"
      };

      static int[] ReferenceFrameInt = new int[]
      {
        SceneTrackFbx.ReferenceFrame.Local,
        SceneTrackFbx.ReferenceFrame.World,
      };
    
      static String[] ReferenceFrameStr = new String[]
      {
        "Hierarchical (Keep)",
        "Flatten",
      };

      static int[] TriangleOrderInt = new int[]
      {
        0, 1
      };
    
      static String[] TriangleOrderStr = new String[]
      {
        "Keep (1, 2, 3)", "Reverse (1, 3, 2)"
      };

      static int[] RotationOrderInt = new int[]
      {
        SceneTrackFbx.AxisOrder.XYZ,
        SceneTrackFbx.AxisOrder.XZY,
        SceneTrackFbx.AxisOrder.YXZ,
        SceneTrackFbx.AxisOrder.YZX,
        SceneTrackFbx.AxisOrder.ZXY,
        SceneTrackFbx.AxisOrder.ZYX
      };

      static string[] RotationOrderStr = new string[]
      {
        "XYZ",
        "XZY",
        "YXZ",
        "YZX",
        "ZXY",
        "ZYX"
      };

      public static void EditorPreferences()
      {
        Version = EditorGUILayout.IntPopup("FBX Version", Version, FbxVersionStr, FbxVersionInt);
      
        EditorGUILayout.PrefixLabel(String.Empty);

        EditorGUILayout.BeginHorizontal();
          EditorGUILayout.PrefixLabel("   ");

        int preset = EditorGUILayout.IntPopup(0, AxisPresetsStr, AxisPresets);
        if (preset != 0)
        {
          switch(preset)
          {
            case 1: // Default
              AxisTX = (int) FbxAxis.PX; AxisRX = (int) FbxAxis.PX;  AxisSX = (int) FbxAxis.PX;
              AxisTY = (int) FbxAxis.PY; AxisRY = (int) FbxAxis.PY;  AxisSY = (int) FbxAxis.PY;
              AxisTZ = (int) FbxAxis.PZ; AxisRZ = (int) FbxAxis.PZ;  AxisSZ = (int) FbxAxis.PZ;
              RotationAddX = 0.0f;
              RotationAddY = 0.0f;
              RotationAddZ = 0.0f;
              ScaleMultiplyX = 1.0f;
              ScaleMultiplyY = 1.0f;
              ScaleMultiplyZ = 1.0f;
              RotationOrder = SceneTrackFbx.AxisOrder.XYZ;
              ReferenceFrame = SceneTrackFbx.ReferenceFrame.Local;
              ReverseTriangles = false;
            break;
            case 2: // Unity To Fbx
              AxisTX = (int) FbxAxis.NX; AxisRX = (int) FbxAxis.PX;  AxisSX = (int) FbxAxis.PX;
              AxisTY = (int) FbxAxis.PY; AxisRY = (int) FbxAxis.NY;  AxisSY = (int) FbxAxis.PY;
              AxisTZ = (int) FbxAxis.PZ; AxisRZ = (int) FbxAxis.NZ;  AxisSZ = (int) FbxAxis.PZ;
              RotationAddX = 0.0f;
              RotationAddY = 0.0f;
              RotationAddZ = 0.0f;
              ScaleMultiplyX = 1.0f;
              ScaleMultiplyY = 1.0f;
              ScaleMultiplyZ = 1.0f;
              RotationOrder = SceneTrackFbx.AxisOrder.ZXY;
              ReferenceFrame = SceneTrackFbx.ReferenceFrame.Local;
              ReverseTriangles = true;
            break;
          }
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
          EditorGUILayout.PrefixLabel("Translation");
      
          var tsX = (FbxAxis) EditorGUILayout.IntPopup((int) AxisTX, AxisStr, AxisInts);
          AxisTX = (int) tsX;
      
          var tsY = (FbxAxis) EditorGUILayout.IntPopup((int) AxisTY, AxisStr, AxisInts);
          AxisTY = (int) tsY;
      
          var tsZ = (FbxAxis) EditorGUILayout.IntPopup((int) AxisTZ, AxisStr, AxisInts);
          AxisTZ = (int) tsZ;

        EditorGUILayout.EndHorizontal();
      
        EditorGUILayout.BeginHorizontal();
          EditorGUILayout.PrefixLabel("Rotation");
      
          var rX = (FbxAxis) EditorGUILayout.IntPopup((int) AxisRX, AxisStr, AxisInts);
          AxisRX = (int) rX;
      
          var rY = (FbxAxis) EditorGUILayout.IntPopup((int) AxisRY, AxisStr, AxisInts);
          AxisRY = (int) rY;
      
          var rZ = (FbxAxis) EditorGUILayout.IntPopup((int) AxisRZ, AxisStr, AxisInts);
          AxisRZ = (int) rZ;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
          EditorGUILayout.PrefixLabel("   ");
        
          RotationAddX = EditorGUILayout.FloatField(RotationAddX);
          RotationAddY = EditorGUILayout.FloatField(RotationAddY);
          RotationAddZ = EditorGUILayout.FloatField(RotationAddZ);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
          EditorGUILayout.PrefixLabel("Scale");
      
          var sX = (FbxAxis) EditorGUILayout.IntPopup((int) AxisSX, AxisStr, AxisInts);
          AxisSX = (int) sX;
      
          var sY = (FbxAxis) EditorGUILayout.IntPopup((int) AxisSY, AxisStr, AxisInts);
          AxisSY = (int) sY;
      
          var sZ = (FbxAxis) EditorGUILayout.IntPopup((int) AxisSZ, AxisStr, AxisInts);
          AxisSZ = (int) sZ;

        EditorGUILayout.EndHorizontal();
      
        EditorGUILayout.BeginHorizontal();
          EditorGUILayout.PrefixLabel("   ");
        
          ScaleMultiplyX = EditorGUILayout.FloatField(ScaleMultiplyX);
          ScaleMultiplyY = EditorGUILayout.FloatField(ScaleMultiplyY);
          ScaleMultiplyZ = EditorGUILayout.FloatField(ScaleMultiplyZ);

        EditorGUILayout.EndHorizontal();
      
        EditorGUILayout.PrefixLabel(String.Empty);

        RotationOrder = EditorGUILayout.IntPopup("Rotation Order", RotationOrder, RotationOrderStr, RotationOrderInt);

        ReferenceFrame = EditorGUILayout.IntPopup("Scene Graph", ReferenceFrame, ReferenceFrameStr, ReferenceFrameInt);

        int reverseTriangle = ReverseTriangles ? 1 : 0;

        reverseTriangle = EditorGUILayout.IntPopup("Triangle Winding", reverseTriangle, TriangleOrderStr, TriangleOrderInt);

        ReverseTriangles = reverseTriangle == 1 ? true : false;

      }

      public static string GetExportExtension()
      {
        return "fbx";
      }
    }
}