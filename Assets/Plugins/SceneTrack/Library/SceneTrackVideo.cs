using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SceneTrackVideo
{
	public static class Node
	{
		public const int Transform = (0);
		public const int Bone = (1);
		public const int Mesh = (2);
	}

	public static class NodeProperty
	{
		public const int Translation = (1);
		public const int Vertex = (1);
		public const int Rotation = (2);
		public const int Normal = (2);
		public const int Scale = (4);
	}

	public static class Axis
	{
		public const int X = (0);
		public const int Y = (1);
		public const int Z = (2);
		public const int W = (3);
	}

	public static class AxisOrder
	{
		public const int XYZ = (0);
		public const int XZY = (1);
		public const int YXZ = (2);
		public const int YZX = (3);
		public const int ZXY = (4);
		public const int ZYX = (5);
	}

	public static class Operator
	{
		public const int SetZero = (0);
		public const int SetOne = (1);
		public const int Value = (2);
		public const int Constant = (3);
		public const int Add = (4);
		public const int Subtract = (5);
		public const int Multiply = (6);
		public const int Divide = (7);
		public const int Negate = (8);
	}

	public static class Conversion
	{

		#if UNITY_EDITOR
		[DllImport("SceneTrackVideo", CallingConvention = CallingConvention.Cdecl, EntryPoint = "videoConvertSceneTrackFile"), SuppressUnmanagedCodeSecurity]
		public static extern int ConvertSceneTrackFile(StringBuilder src, StringBuilder dst);
		#else
		public static int ConvertSceneTrackFile(StringBuilder src, StringBuilder dst) { return default(int); }
		#endif

		#if UNITY_EDITOR
		[DllImport("SceneTrackVideo", CallingConvention = CallingConvention.Cdecl, EntryPoint = "videoStepConvertSceneTrackFileBegin"), SuppressUnmanagedCodeSecurity]
		public static extern int StepConvertSceneTrackFileBegin(StringBuilder src, StringBuilder dst);
		#else
		public static int StepConvertSceneTrackFileBegin(StringBuilder src, StringBuilder dst) { return default(int); }
		#endif

		#if UNITY_EDITOR
		[DllImport("SceneTrackVideo", CallingConvention = CallingConvention.Cdecl, EntryPoint = "videoStepConvertSceneTrackFileUpdate"), SuppressUnmanagedCodeSecurity]
		public static extern int StepConvertSceneTrackFileUpdate();
		#else
		public static int StepConvertSceneTrackFileUpdate() { return default(int); }
		#endif

		#if UNITY_EDITOR
		[DllImport("SceneTrackVideo", CallingConvention = CallingConvention.Cdecl, EntryPoint = "videoStepConvertProgress"), SuppressUnmanagedCodeSecurity]
		public static extern float StepConvertProgress();
		#else
		public static float StepConvertProgress() { return default(float); }
		#endif

	}
	public static class Settings
	{

		#if UNITY_EDITOR
		[DllImport("SceneTrackVideo", CallingConvention = CallingConvention.Cdecl, EntryPoint = "videoSetFileType"), SuppressUnmanagedCodeSecurity]
		public static extern int SetFileType(int fileType);
		#else
		public static int SetFileType(int fileType) { return default(int); }
		#endif

	}
}

