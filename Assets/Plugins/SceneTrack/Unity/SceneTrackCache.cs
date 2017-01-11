using System.IO;
using UnityEngine;

namespace SceneTrack.Unity
{
    public static class Cache
    {
        /// <summary>
        /// Location to store cached take information
        /// </summary>
        public static string Folder
        {
            get
            {
                return Application.dataPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Temp" + Path.DirectorySeparatorChar + "SceneTrackData" + Path.DirectorySeparatorChar ;
            }

        }

        /// <summary>
        /// Extension used with cache files
        /// </summary>
        public static string FileExtension
        {
            get
            {
                return ".st";
            }
        }

        public static string[] GetCacheFiles()
        {
            // Make Sure The Directory Exists
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            return Directory.GetFiles(Folder);
        }

        public static int GetNextTakeNumber()
        {
            return GetCacheFiles().Length + 1;
        }

        public static string GetNextTakeFilename()
        {
            return "Take_" + GetNextTakeNumber().ToString();
        }
    }
}