﻿using System.IO;
using UnityEngine;

namespace SceneTrack.Unity
{
    public static class Log
    {
        public static void Message(string logMessage)
        {
            UnityEngine.Debug.Log(logMessage);
        }
		public static void Warning(string warningMessage)
		{
            UnityEngine.Debug.LogWarning(warningMessage);
        }
		public static void Error(string errorMessage)
		{
			UnityEngine.Debug.LogError(errorMessage);
		}
    }
}