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
                return Application.dataPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "SceneTrack" + Path.DirectorySeparatorChar ;
            }

        }

        /// <summary>
        /// Extension used with cache files
        /// </summary>
        public static string FileExtension
        {
            get
            {
                return "st";
            }
        }

        public static string[] CachedFiles
        {
            get { return _cachedFiles; }
            set { _cachedFiles = value; }
        }

        private static string[] _cachedFiles = new string[0];

        public static string[] GetCacheFiles()
        {
            // Make Sure The Directory Exists
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            CachedFiles = Directory.GetFiles(Folder, "*." + FileExtension) ?? new string[0];

            return CachedFiles;
        }

        public static void ClearFiles()
        {
            foreach (var s in GetCacheFiles())
            {
                File.Delete(s);
            }
            _cachedFiles = new string[0];
        }

        public static void ClearFile(string path)
        {
            File.Delete(Folder +  path + "." + FileExtension);
            GetCacheFiles();
        }

        public static int CurrentTakeNumber = 0;

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