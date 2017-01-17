using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;

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
        public static void EditorPreferences()
        {

        }

        public static string GetExportExtension()
        {
            return "fbx";
        }
    }
}