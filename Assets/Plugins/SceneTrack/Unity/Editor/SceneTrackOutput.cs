using System;
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
            var outputFolder = UnityEditor.EditorUtility.SaveFilePanel("Destination File", UnityEngine.Application.dataPath, "Output" + FBXOutput.GetExportExtension(), FBXOutput.GetExportExtension());

            SceneTrackFbx.Conversion.ConvertSceneTrackFile(new StringBuilder(sourcePath), new StringBuilder(outputFolder));

            Console.WriteLine(outputFolder);
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