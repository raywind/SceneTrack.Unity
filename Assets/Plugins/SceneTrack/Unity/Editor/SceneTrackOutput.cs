﻿#define EXPORTER_IS_THREADED

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading;
using SceneTrackFbx;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{
    public static class FbxOutputRunner
    {
        static bool _exporting = false;

        private static float _exportProgress = 0.0f;
        private static int _exportSuccessfull = -1;
        private static Thread _exportThread;
        private static string _dstPath;

        public static bool IsExporting
        {
            get { return _exporting; }
        }

        public static int IsExportSucessfull
        {
            get { return _exportSuccessfull; }
        }

        public static float ExportProgress
        {
            get { return _exportProgress; }
        }

        public static bool StartExport(string srcPath_, string dstPath_)
        {
            if (IsExporting)
                return false;

            var exportInfo = new ExportInfo()
            {
                srcPath = srcPath_,
                dstPath = dstPath_
            };

            _dstPath = dstPath_;
            FBXOutput.SetupExport();

            _exportThread = new Thread(new ParameterizedThreadStart(ExportThreadFn));
            _exportThread.Start(exportInfo);

            return true;
        }

        public static String ReceiveExport()
        {
            _exportSuccessfull = -1;
            _exportThread = null;
            return _dstPath;
        }

        class ExportInfo
        {
            public String srcPath, dstPath;
        }

        static void ExportThreadFn(object exportInfoObj)
        {
            if (_exporting)
                return;

            _exportSuccessfull = -1;
            _exportProgress = 0.0f;

            ExportInfo exportInfo = (ExportInfo) exportInfoObj;

            int mode = 0, response = 0;
            _exporting = true;

            while (true)
            {
                if (mode == 0) // Configure
                {
                    _exportProgress = 0.0f;
                    mode = 1;
                }
                else if (mode == 1) // Begin
                {
                    response = SceneTrackFbx.Conversion.StepConvertSceneTrackFileBegin(
                        new StringBuilder(exportInfo.srcPath), new StringBuilder(exportInfo.dstPath));
                    if (response == 0)
                    {
                        mode = 2;
                    }
                    else
                    {
                        _exportSuccessfull = 0;
                        break;
                    }
                }
                else if (mode == 2) // Update
                {
                    response = SceneTrackFbx.Conversion.StepConvertSceneTrackFileUpdate();

                    if (response == 1)
                    {
                        _exportSuccessfull = 1;
                        break;
                    }
                    else if (response == -1)
                    {
                        _exportSuccessfull = 0;
                        break;
                    }
                    else
                    {
                        _exportProgress = SceneTrackFbx.Conversion.StepConvertProgress();
                        Thread.Sleep(1);
                    }
                }
            }

            _exporting = false;
        }


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

#if EXPORTER_IS_THREADED
                StartExport(sourcePath, outputFile);
#else
                FBXOutput.SetupExport();
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
#endif
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
            NW,
            PX,
            PY,
            PZ,
            PW
        }

        public static int Version
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_FBX_Version", 12); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_FBX_Version", value); }
        }

        public static int AxisTX
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisTSX", (int) FbxAxis.PX); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSX", value); }
        }

        public static int AxisTY
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisTSY", (int) FbxAxis.PY); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSY", value); }
        }

        public static int AxisTZ
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisTSZ", (int) FbxAxis.PZ); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSZ", value); }
        }

        public static int AxisRX
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRX", (int) FbxAxis.PX); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRX", value); }
        }

        public static int AxisRY
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRY", (int) FbxAxis.PY); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRY", value); }
        }

        public static int AxisRZ
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRZ", (int) FbxAxis.PZ); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRZ", value); }
        }

        public static int AxisRW
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisRW", (int) FbxAxis.PW); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisRW", value); }
        }

        public static int AxisSX
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisSX", (int) FbxAxis.PX); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisSX", value); }
        }

        public static int AxisSY
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisSY", (int) FbxAxis.PY); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisSY", value); }
        }

        public static int AxisSZ
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_AxisSZ", (int) FbxAxis.PZ); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisSZ", value); }
        }

        public static int VertexX
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_VertexX", (int) FbxAxis.PX); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_VertexX", value); }
        }

        public static int VertexY
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_VertexY", (int) FbxAxis.PY); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_VertexY", value); }
        }

        public static int VertexZ
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_VertexZ", (int) FbxAxis.PZ); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_VertexZ", value); }
        }

        public static int NormalX
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_NormalX", (int) FbxAxis.PX); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_NormalX", value); }
        }

        public static int NormalY
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_NormalY", (int) FbxAxis.PY); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_NormalY", value); }
        }

        public static int NormalZ
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_NormalZ", (int) FbxAxis.PZ); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_NormalZ", value); }
        }

        public static float ScaleMultiplyX
        {
            get { return UnityEditor.EditorPrefs.GetFloat("SceneTrack_ScaleMultiplyX", 1.0f); }
            set { UnityEditor.EditorPrefs.SetFloat("SceneTrack_ScaleMultiplyX", value); }
        }

        public static float ScaleMultiplyY
        {
            get { return UnityEditor.EditorPrefs.GetFloat("SceneTrack_ScaleMultiplyY", 1.0f); }
            set { UnityEditor.EditorPrefs.SetFloat("SceneTrack_ScaleMultiplyY", value); }
        }

        public static float ScaleMultiplyZ
        {
            get { return UnityEditor.EditorPrefs.GetFloat("SceneTrack_ScaleMultiplyZ", 1.0f); }
            set { UnityEditor.EditorPrefs.SetFloat("SceneTrack_ScaleMultiplyZ", value); }
        }

        public static int ReferenceFrame
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_ReferenceFrame", 0); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_ReferenceFrame", value); }
        }

        public static bool ReverseTriangles
        {
            get { return UnityEditor.EditorPrefs.GetBool("SceneTrack_RewindTriangle", false); }
            set { UnityEditor.EditorPrefs.SetBool("SceneTrack_RewindTriangle", value); }
        }

        private static void SetupSwizzle(int node, int trsMask, int srcAxis, FbxAxis axis)
        {
            int dstAxis = 0, sign = 0;
            switch (axis)
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
                case FbxAxis.NW:
                    dstAxis = SceneTrackFbx.Axis.W;
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
                case FbxAxis.PW:
                    dstAxis = SceneTrackFbx.Axis.W;
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

        private static void SetupSwizzles(int node, int trsMask, int x, int y, int z, int w)
        {
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.X, (FbxAxis) x);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Y, (FbxAxis) y);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Z, (FbxAxis) z);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.W, (FbxAxis) w);
        }

        public static void SetupExport()
        {
            SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Translation, AxisTX, AxisTY, AxisTZ);
            SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Rotation, AxisRX, AxisRY, AxisRZ,
                AxisRW);
            SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Scale, AxisSX, AxisSY, AxisSZ);

            SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.NodeProperty.Translation, AxisTX, AxisTY, AxisTZ);
            SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.NodeProperty.Rotation, AxisRX, AxisRY, AxisRZ, AxisRW);
            SetupSwizzles(SceneTrackFbx.Node.Bone, SceneTrackFbx.NodeProperty.Scale, AxisSX, AxisSY, AxisSZ);

            SetupSwizzles(SceneTrackFbx.Node.Mesh, SceneTrackFbx.NodeProperty.Vertex, VertexX, VertexY, VertexZ);
            SetupSwizzles(SceneTrackFbx.Node.Mesh, SceneTrackFbx.NodeProperty.Normal, NormalX, NormalY, NormalZ);

            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Translation,
                SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Multiply, ScaleMultiplyX);
            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Translation,
                SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Multiply, ScaleMultiplyY);
            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Translation,
                SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Multiply, ScaleMultiplyZ);

            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.NodeProperty.Translation,
                SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Multiply, ScaleMultiplyX);
            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.NodeProperty.Translation,
                SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Multiply, ScaleMultiplyY);
            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Bone, SceneTrackFbx.NodeProperty.Translation,
                SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Multiply, ScaleMultiplyZ);

            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Mesh, SceneTrackFbx.NodeProperty.Vertex,
                SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Multiply, ScaleMultiplyX);
            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Mesh, SceneTrackFbx.NodeProperty.Vertex,
                SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Multiply, ScaleMultiplyY);
            SceneTrackFbx.Settings.SetAxisOperation(SceneTrackFbx.Node.Mesh, SceneTrackFbx.NodeProperty.Vertex,
                SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Multiply, ScaleMultiplyZ);

            SceneTrackFbx.Settings.SetReferenceFrame(ReferenceFrame);
            SceneTrackFbx.Settings.SetReverseTriangleWinding(ReverseTriangles ? 1 : 0);

            SceneTrackFbx.Settings.SetFileVersion(Version);

            SceneTrackFbx.Settings.ClearAssetSearchPaths();
            SceneTrackFbx.Settings.AddAssetSearchPath(new StringBuilder(UnityEngine.Application.dataPath + "/"));

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

        static int[] AxisPresets = new int[]
        {
            0, 1, 2
        };

        static String[] AxisPresetsStr = new string[]
        {
            "Default",
            "Unity",
            "Custom"
        };

        static int[] AxisInts = new int[]
        {
            0, 1, 2, 3, 4, 5
        };

        static String[] AxisStr = new string[]
        {
            "-X", "-Y", "-Z", "-W", "+X", "+Y", "+Z", "+W"
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

        public static void EditorPreferences()
        {
            Version = EditorGUILayout.IntPopup("FBX Version", Version, FbxVersionStr, FbxVersionInt);

            EditorGUILayout.PrefixLabel(String.Empty);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("   ");

            SceneTrackPreferences.OutputAxisSettings = CheckSwizzle();

            int preset = EditorGUILayout.IntPopup(SceneTrackPreferences.OutputAxisSettings, AxisPresetsStr,
                AxisPresets);

            if (preset != SceneTrackPreferences.OutputAxisSettings)
            {
                SetSwizzle(preset);
            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Translation");
            AxisTX = (int) EditorGUILayout.IntPopup((int) AxisTX, AxisStr, AxisInts);
            AxisTY = (int) EditorGUILayout.IntPopup((int) AxisTY, AxisStr, AxisInts);
            AxisTZ = (int) EditorGUILayout.IntPopup((int) AxisTZ, AxisStr, AxisInts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Rotation (Quaternion)");
            AxisRX = (int) EditorGUILayout.IntPopup((int) AxisRX, AxisStr, AxisInts);
            AxisRY = (int) EditorGUILayout.IntPopup((int) AxisRY, AxisStr, AxisInts);
            AxisRZ = (int) EditorGUILayout.IntPopup((int) AxisRZ, AxisStr, AxisInts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Scale");
            AxisSX = (int) EditorGUILayout.IntPopup((int) AxisSX, AxisStr, AxisInts);
            AxisSY = (int) EditorGUILayout.IntPopup((int) AxisSY, AxisStr, AxisInts);
            AxisSZ = (int) EditorGUILayout.IntPopup((int) AxisSZ, AxisStr, AxisInts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Vertex");
            VertexX = (int) EditorGUILayout.IntPopup((int) VertexX, AxisStr, AxisInts);
            VertexY = (int) EditorGUILayout.IntPopup((int) VertexY, AxisStr, AxisInts);
            VertexZ = (int) EditorGUILayout.IntPopup((int) VertexZ, AxisStr, AxisInts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Normal");
            NormalX = (int) EditorGUILayout.IntPopup((int) NormalX, AxisStr, AxisInts);
            NormalY = (int) EditorGUILayout.IntPopup((int) NormalY, AxisStr, AxisInts);
            NormalZ = (int) EditorGUILayout.IntPopup((int) NormalZ, AxisStr, AxisInts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PrefixLabel(String.Empty);

            ReferenceFrame = EditorGUILayout.IntPopup("Scene Graph", ReferenceFrame, ReferenceFrameStr,
                ReferenceFrameInt);

            int reverseTriangle = ReverseTriangles ? 1 : 0;

            reverseTriangle = EditorGUILayout.IntPopup("Triangle Winding", reverseTriangle, TriangleOrderStr,
                TriangleOrderInt);

            ReverseTriangles = reverseTriangle == 1 ? true : false;
      
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Scene Scale");
            ScaleMultiplyX = ScaleMultiplyY = ScaleMultiplyZ = EditorGUILayout.FloatField(ScaleMultiplyX);
            EditorGUILayout.EndHorizontal();

        }

        public static string GetExportExtension()
        {
            return "fbx";
        }

        public static void SetSwizzle(int type)
        {
            switch (type)
            {
                case 0: // DEFAULT
                    AxisTX = (int) FbxAxis.PX;
                    AxisRX = (int) FbxAxis.PX;
                    AxisSX = (int) FbxAxis.PX;
                    VertexX = (int) FbxAxis.PX;
                    NormalX = (int) FbxAxis.PX;
                    AxisTY = (int) FbxAxis.PY;
                    AxisRY = (int) FbxAxis.PY;
                    AxisSY = (int) FbxAxis.PY;
                    VertexY = (int) FbxAxis.PY;
                    NormalY = (int) FbxAxis.PY;
                    AxisTZ = (int) FbxAxis.PZ;
                    AxisRZ = (int) FbxAxis.PZ;
                    AxisSZ = (int) FbxAxis.PZ;
                    VertexZ = (int) FbxAxis.PZ;
                    NormalZ = (int) FbxAxis.PZ;
                    AxisRW = (int) FbxAxis.PW;
                    ScaleMultiplyX = 1.0f;
                    ScaleMultiplyY = 1.0f;
                    ScaleMultiplyZ = 1.0f;
                    ReferenceFrame = SceneTrackFbx.ReferenceFrame.Local;
                    ReverseTriangles = false;
                    break;
                case 1: // UNITY
                    AxisTX = (int) FbxAxis.NX;
                    AxisRX = (int) FbxAxis.PX;
                    AxisSX = (int) FbxAxis.PX;
                    VertexX = (int) FbxAxis.NX;
                    NormalX = (int) FbxAxis.NX;
                    AxisTY = (int) FbxAxis.PY;
                    AxisRY = (int) FbxAxis.NY;
                    AxisSY = (int) FbxAxis.PY;
                    VertexY = (int) FbxAxis.PY;
                    NormalY = (int) FbxAxis.PY;
                    AxisTZ = (int) FbxAxis.PZ;
                    AxisRZ = (int) FbxAxis.NZ;
                    AxisSZ = (int) FbxAxis.PZ;
                    VertexZ = (int) FbxAxis.PZ;
                    NormalZ = (int) FbxAxis.PZ;
                    AxisRW = (int) FbxAxis.PW;
                    ScaleMultiplyX = 1.0f;
                    ScaleMultiplyY = 1.0f;
                    ScaleMultiplyZ = 1.0f;
                    ReferenceFrame = SceneTrackFbx.ReferenceFrame.Local;
                    ReverseTriangles = true;
                    break;
            }

            // Save change
            SceneTrackPreferences.OutputAxisSettings = type;
        }

        public static int CheckSwizzle()
        {
            bool isDefault = true;
            bool isUnity = true;

            // Check if we're using the default settings
            if (AxisTX != (int) FbxAxis.PX) isDefault = false;
            if (AxisRX != (int) FbxAxis.PX) isDefault = false;
            if (AxisSX != (int) FbxAxis.PX) isDefault = false;
            if (VertexX != (int) FbxAxis.PX) isDefault = false;
            if (NormalX != (int) FbxAxis.PX) isDefault = false;
            if (AxisTY != (int) FbxAxis.PY) isDefault = false;
            if (AxisRY != (int) FbxAxis.PY) isDefault = false;
            if (AxisSY != (int) FbxAxis.PY) isDefault = false;
            if (VertexY != (int) FbxAxis.PY) isDefault = false;
            if (NormalY != (int) FbxAxis.PY) isDefault = false;
            if (AxisTZ != (int) FbxAxis.PZ) isDefault = false;
            if (AxisRZ != (int) FbxAxis.PZ) isDefault = false;
            if (AxisSZ != (int) FbxAxis.PZ) isDefault = false;
            if (VertexZ != (int) FbxAxis.PZ) isDefault = false;
            if (NormalZ != (int) FbxAxis.PZ) isDefault = false;
            if (AxisRW != (int) FbxAxis.PW) isDefault = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyX, 1.0f)) isDefault = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyY, 1.0f)) isDefault = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyZ, 1.0f)) isDefault = false;
            if (ReferenceFrame != SceneTrackFbx.ReferenceFrame.Local) isDefault = false;
            if (ReverseTriangles != false) isDefault = false;

            // Check if we're using the unity settings
            if (AxisTX != (int) FbxAxis.NX) isUnity = false;
            if (AxisRX != (int) FbxAxis.PX) isUnity = false;
            if (AxisSX != (int) FbxAxis.PX) isUnity = false;
            if (VertexX != (int) FbxAxis.NX) isUnity = false;
            if (NormalX != (int) FbxAxis.NX) isUnity = false;
            if (AxisTY != (int) FbxAxis.PY) isUnity = false;
            if (AxisRY != (int) FbxAxis.NY) isUnity = false;
            if (AxisSY != (int) FbxAxis.PY) isUnity = false;
            if (VertexY != (int) FbxAxis.PY) isUnity = false;
            if (NormalY != (int) FbxAxis.PY) isUnity = false;
            if (AxisTZ != (int) FbxAxis.PZ) isUnity = false;
            if (AxisRZ != (int) FbxAxis.NZ) isUnity = false;
            if (AxisSZ != (int) FbxAxis.PZ) isUnity = false;
            if (VertexZ != (int) FbxAxis.PZ) isUnity = false;
            if (NormalZ != (int) FbxAxis.PZ) isUnity = false;
            if (AxisRW != (int) FbxAxis.PW) isUnity = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyX, 1.0f)) isUnity = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyY, 1.0f)) isUnity = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyZ, 1.0f)) isUnity = false;
            if (ReferenceFrame != SceneTrackFbx.ReferenceFrame.Local) isUnity = false;
            if (ReverseTriangles != true) isUnity = false;

            if (isUnity) return 1;
            if (isDefault) return 0;
            return 2;
        }
    }

  
    public static class MidiOutputRunner
    {
        static bool _exporting = false;

        private static float _exportProgress = 0.0f;
        private static int _exportSuccessfull = -1;
        private static Thread _exportThread;
        private static string _dstPath;

        public static bool IsExporting
        {
            get { return _exporting; }
        }

        public static int IsExportSucessfull
        {
            get { return _exportSuccessfull; }
        }

        public static float ExportProgress
        {
            get { return _exportProgress; }
        }

        public static bool StartExport(string srcPath_, string dstPath_)
        {
            if (IsExporting)
                return false;

            var exportInfo = new ExportInfo()
            {
                srcPath = srcPath_,
                dstPath = dstPath_
            };

            _dstPath = dstPath_;
            MidiOutput.SetupExport();

            _exportThread = new Thread(new ParameterizedThreadStart(ExportThreadFn));
            _exportThread.Start(exportInfo);

            return true;
        }

        public static String ReceiveExport()
        {
            _exportSuccessfull = -1;
            _exportThread = null;
            return _dstPath;
        }

        class ExportInfo
        {
            public String srcPath, dstPath;
        }

        static void ExportThreadFn(object exportInfoObj)
        {
            if (_exporting)
                return;

            _exportSuccessfull = -1;
            _exportProgress = 0.0f;

            ExportInfo exportInfo = (ExportInfo) exportInfoObj;

            int mode = 0, response = 0;
            _exporting = true;

            while (true)
            {
                if (mode == 0) // Configure
                {
                    _exportProgress = 0.0f;
                    mode = 1;
                }
                else if (mode == 1) // Begin
                {
                    response = SceneTrackMidi.Conversion.StepConvertSceneTrackFileBegin(
                        new StringBuilder(exportInfo.srcPath), new StringBuilder(exportInfo.dstPath));
                    if (response == 0)
                    {
                        mode = 2;
                    }
                    else
                    {
                        _exportSuccessfull = 0;
                        break;
                    }
                }
                else if (mode == 2) // Update
                {
                    response = SceneTrackMidi.Conversion.StepConvertSceneTrackFileUpdate();

                    if (response == 1)
                    {
                        _exportSuccessfull = 1;
                        break;
                    }
                    else if (response == -1)
                    {
                        _exportSuccessfull = 0;
                        break;
                    }
                    else
                    {
                        _exportProgress = SceneTrackMidi.Conversion.StepConvertProgress();
                        Thread.Sleep(1);
                    }
                }
            }

            _exporting = false;
        }


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
                Path.GetFileNameWithoutExtension(sourcePath) + "." + MidiOutput.GetExportExtension(),
                MidiOutput.GetExportExtension());


            UnityEngine.Debug.Log(sourcePath);
            UnityEngine.Debug.Log(outputFile);

            if (!string.IsNullOrEmpty(outputFile))
            {
#if EXPORTER_IS_THREADED
                  StartExport(sourcePath, outputFile);
#else 
                  FBXOutput.SetupExport();
                  int response = SceneTrackMidi.Conversion.ConvertSceneTrackFile(new StringBuilder(sourcePath), new StringBuilder(outputFile));
                  if (response == 0)
                  {
                    UnityEngine.Debug.Log("MIDI Conversion Successfull");
                  }
                  else
                  {
                    UnityEngine.Debug.Log("MIDI Conversion Failed.");
                  }
                
#endif

            }
        }
    }
  
    public static class MidiOutput
    {
    
        public static int FileType
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_Midi_Type", 0); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_Midi_Type", value); }
        }
    
        public static bool IsMidiOutput
        {
          get { return FileType == 0; }
        }

        public static bool IsXmlOutput
        {
          get { return FileType == 1; }
        }

        public static int AxisTX
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_Midi_AxisTSX", (int) FBXOutput.FbxAxis.PX); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_Midi_AxisTSX", value); }
        }

        public static int AxisTY
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_Midi_AxisTSY", (int) FBXOutput.FbxAxis.PY); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_Midi_AxisTSY", value); }
        }

        public static int AxisTZ
        {
            get { return UnityEditor.EditorPrefs.GetInt("SceneTrack_Midi_Midi_AxisTSZ", (int) FBXOutput.FbxAxis.PZ); }
            set { UnityEditor.EditorPrefs.SetInt("SceneTrack_AxisTSZ", value); }
        }
    
        public static float ScaleMultiplyX
        {
            get { return UnityEditor.EditorPrefs.GetFloat("SceneTrack_Midi_ScaleMultiplyX", 1.0f); }
            set { UnityEditor.EditorPrefs.SetFloat("SceneTrack_Midi_ScaleMultiplyX", value); }
        }

        public static float ScaleMultiplyY
        {
            get { return UnityEditor.EditorPrefs.GetFloat("SceneTrack_Midi_ScaleMultiplyY", 1.0f); }
            set { UnityEditor.EditorPrefs.SetFloat("SceneTrack_Midi_ScaleMultiplyY", value); }
        }

        public static float ScaleMultiplyZ
        {
            get { return UnityEditor.EditorPrefs.GetFloat("SceneTrack_Midi_ScaleMultiplyZ", 1.0f); }
            set { UnityEditor.EditorPrefs.SetFloat("SceneTrack_Midi_ScaleMultiplyZ", value); }
        }

        private static void SetupSwizzle(int node, int trsMask, int srcAxis, FBXOutput.FbxAxis axis)
        {
            int dstAxis = 0, sign = 0;
            switch (axis)
            {
                case FBXOutput.FbxAxis.NX:
                    dstAxis = SceneTrackFbx.Axis.X;
                    sign = -1;
                    break;
                case FBXOutput.FbxAxis.NY:
                    dstAxis = SceneTrackFbx.Axis.Y;
                    sign = -1;
                    break;
                case FBXOutput.FbxAxis.NZ:
                    dstAxis = SceneTrackFbx.Axis.Z;
                    sign = -1;
                    break;
                case FBXOutput.FbxAxis.NW:
                    dstAxis = SceneTrackFbx.Axis.W;
                    sign = -1;
                    break;
                case FBXOutput.FbxAxis.PX:
                    dstAxis = SceneTrackFbx.Axis.X;
                    sign = 1;
                    break;
                case FBXOutput.FbxAxis.PY:
                    dstAxis = SceneTrackFbx.Axis.Y;
                    sign = 1;
                    break;
                case FBXOutput.FbxAxis.PZ:
                    dstAxis = SceneTrackFbx.Axis.Z;
                    sign = 1;
                    break;
                case FBXOutput.FbxAxis.PW:
                    dstAxis = SceneTrackFbx.Axis.W;
                    sign = 1;
                    break;
            }

           // SceneTrackMidi.Settings.SetAxisSwizzle(node, trsMask, srcAxis, dstAxis, sign);
        }

        private static void SetupSwizzles(int node, int trsMask, int x, int y, int z)
        {
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.X, (FBXOutput.FbxAxis) x);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Y, (FBXOutput.FbxAxis) y);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Z, (FBXOutput.FbxAxis) z);
        }

        private static void SetupSwizzles(int node, int trsMask, int x, int y, int z, int w)
        {
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.X, (FBXOutput.FbxAxis) x);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Y, (FBXOutput.FbxAxis) y);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.Z, (FBXOutput.FbxAxis) z);
            SetupSwizzle(node, trsMask, SceneTrackFbx.Axis.W, (FBXOutput.FbxAxis) w);
        }

        public static void SetupExport()
        {
            SetupSwizzles(SceneTrackFbx.Node.Transform, SceneTrackFbx.NodeProperty.Translation, AxisTX, AxisTY, AxisTZ);

            SceneTrackMidi.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackMidi.NodeProperty.Translation,
                SceneTrackFbx.Axis.X, SceneTrackFbx.Operator.Multiply, ScaleMultiplyX);
            SceneTrackMidi.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackMidi.NodeProperty.Translation,
                SceneTrackFbx.Axis.Y, SceneTrackFbx.Operator.Multiply, ScaleMultiplyY);
            SceneTrackMidi.Settings.SetAxisOperation(SceneTrackFbx.Node.Transform, SceneTrackMidi.NodeProperty.Translation,
                SceneTrackFbx.Axis.Z, SceneTrackFbx.Operator.Multiply, ScaleMultiplyZ);

            SceneTrackMidi.Settings.SetFileType(FileType);
        }
    
        static int[] MidiFileTypeInt = new int[]
        {
            0, 1
        };

        static string[] MidiFileTypeStr = new string[]
        {
            "MIDI",
            "XML"
        };


        static int[] AxisPresets = new int[]
        {
            0, 1, 2
        };

        static String[] AxisPresetsStr = new string[]
        {
            "Default",
            "Unity",
            "Custom"
        };

        static int[] AxisInts = new int[]
        {
            0, 1, 2, 3, 4, 5
        };

        static String[] AxisStr = new string[]
        {
            "-X", "-Y", "-Z", "-W", "+X", "+Y", "+Z", "+W"
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

        public static void EditorPreferences()
        {
           
            EditorGUILayout.PrefixLabel(String.Empty);
      
            FileType = EditorGUILayout.IntPopup("FBX Version", FileType, MidiFileTypeStr, MidiFileTypeInt);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("   ");

            SceneTrackPreferences.OutputAxisSettings = CheckSwizzle();

            int preset = EditorGUILayout.IntPopup(SceneTrackPreferences.OutputAxisSettings, AxisPresetsStr,
                AxisPresets);

            if (preset != SceneTrackPreferences.OutputAxisSettings)
            {
                SetSwizzle(preset);
            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Translation");
            AxisTX = (int) EditorGUILayout.IntPopup((int) AxisTX, AxisStr, AxisInts);
            AxisTY = (int) EditorGUILayout.IntPopup((int) AxisTY, AxisStr, AxisInts);
            AxisTZ = (int) EditorGUILayout.IntPopup((int) AxisTZ, AxisStr, AxisInts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PrefixLabel(String.Empty);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Scene Scale");
            ScaleMultiplyX = ScaleMultiplyY = ScaleMultiplyZ = EditorGUILayout.FloatField(ScaleMultiplyX);
            EditorGUILayout.EndHorizontal();

        }

        public static string GetExportExtension()
        {
          if (IsMidiOutput)
            return "mid";
          else if (IsXmlOutput)
            return "xml";
          return "txt";
        }

        public static void SetSwizzle(int type)
        {
            switch (type)
            {
                case 0: // DEFAULT
                    AxisTX = (int) FBXOutput.FbxAxis.PX;
                    AxisTY = (int) FBXOutput.FbxAxis.PY;
                    AxisTZ = (int) FBXOutput.FbxAxis.PZ;
                    ScaleMultiplyX = 1.0f;
                    ScaleMultiplyY = 1.0f;
                    ScaleMultiplyZ = 1.0f;
                    break;
                case 1: // UNITY
                    AxisTX = (int) FBXOutput.FbxAxis.NX;
                    AxisTY = (int) FBXOutput.FbxAxis.PY;
                    AxisTZ = (int) FBXOutput.FbxAxis.PZ;
                    ScaleMultiplyX = 1.0f;
                    ScaleMultiplyY = 1.0f;
                    ScaleMultiplyZ = 1.0f;
                    break;
            }

            // Save change
            SceneTrackPreferences.OutputAxisSettings = type;
        }

        public static int CheckSwizzle()
        {
            bool isDefault = true;
            bool isUnity = true;

            // Check if we're using the default settings
            if (AxisTX != (int) FBXOutput.FbxAxis.PX) isDefault = false;
            if (AxisTY != (int) FBXOutput.FbxAxis.PY) isDefault = false;
            if (AxisTZ != (int) FBXOutput.FbxAxis.PZ) isDefault = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyX, 1.0f)) isDefault = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyY, 1.0f)) isDefault = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyZ, 1.0f)) isDefault = false;

            // Check if we're using the unity settings
            if (AxisTX != (int) FBXOutput.FbxAxis.NX) isUnity = false;
            if (AxisTY != (int) FBXOutput.FbxAxis.PY) isUnity = false;
            if (AxisTZ != (int) FBXOutput.FbxAxis.PZ) isUnity = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyX, 1.0f)) isUnity = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyY, 1.0f)) isUnity = false;
            if (!UnityEngine.Mathf.Approximately(ScaleMultiplyZ, 1.0f)) isUnity = false;

            if (isUnity) return 1;
            if (isDefault) return 0;
            return 2;
        }
    }


}