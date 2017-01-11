using System;
using System.IO;
using System.Text;
using SceneTrack;
using UnityEngine;

namespace SceneTrack.Unity
{
    public static class System
    {
        /// <summary>
        /// Has SceneTrack been initialized for this recording.
        /// </summary>
        public static bool InPlayMode { get; private set; }

        /// <summary>
        /// The current instance handle for SceneTrack
        /// </summary>
        public static uint InstanceHandle { get; private set; }

        public static void EnterPlayMode(string fileName = "")
        {
            // Bail Out Check
            if (InPlayMode) return;

            if ( string.IsNullOrEmpty(fileName))
            {
                // Find next number
                fileName = Cache.GetNextTakeFilename();
            }

            // Start Recording
            InstanceHandle = Recording.CreateRecording();

            // Setup the new take for recording, writing every 2 frames
            Recording.AppendSaveRecording(new StringBuilder(Cache.Folder + fileName + Cache.FileExtension), Format.Binary, 2);

            // Create Schema
            Classes.CreateSchema();

            // Set Flag
            InPlayMode = true;
        }


        public static void ExitPlayMode()
        {
            // Bail Out Check
            if (!InPlayMode) return;

            // Clean up anything left in writing the files
            Recording.CloseRecording(InstanceHandle);

            // Set Flag
            InPlayMode = false;
        }
    }
}