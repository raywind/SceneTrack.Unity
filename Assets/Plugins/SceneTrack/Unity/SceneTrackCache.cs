using System;
using System.IO;
using System.Text;
using SceneTrack;
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

    }
}