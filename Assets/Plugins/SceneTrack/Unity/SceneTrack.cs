using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SceneTrack;
using UnityEditor;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

namespace SceneTrack.Unity
{
    public static class System
    {
        public static string FilenameOverride = string.Empty;
        public static List<SceneTrackObject> CachedKnownObjects = new List<SceneTrackObject>();


        public static Dictionary<Mesh, uint>  SharedMeshes = new Dictionary<Mesh, uint>();
        public static Dictionary<Material, uint>  SharedMaterials = new Dictionary<Material, uint>();

        /// <summary>
        /// Has SceneTrack been initialized for this recording.
        /// </summary>
        public static bool InPlayMode { get; private set; }

        /// <summary>
        /// The current instance handle for SceneTrack
        /// </summary>
        public static uint InstanceHandle { get; private set; }

        public static uint LastUpdate { get; set; }

        public static uint FrameCount { get; private set; }

        public static void EnterPlayMode()
        {
            // Bail Out Check
            if (InPlayMode || InstanceHandle != 0) return;

            var fileName = Cache.GetNextTakeFilename();
            if ( !string.IsNullOrEmpty(FilenameOverride))
            {
                // Find next number
                fileName = FilenameOverride;
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
            if (!InPlayMode || InstanceHandle == 0) return;

            // Clean up anything left in writing the files
            Recording.CloseRecording(InstanceHandle);

            InstanceHandle = 0;

            // Set Flag
            InPlayMode = false;
        }

        public static void SubmitRecording(uint frameCount, float deltaTime)
        {
            if ( frameCount != FrameCount ) return;

            Recording.Submit(deltaTime);

            // Increment
            FrameCount++;
        }

        public static void CacheKnownObjects()
        {
            CachedKnownObjects.Clear();
            CachedKnownObjects.AddRange((SceneTrackObject[])Resources.FindObjectsOfTypeAll(typeof(SceneTrackObject)));
        }
    }
}