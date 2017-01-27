using UnityEditor;
using UnityEngine;

namespace SceneTrack.Unity.Editor
{
    public static class Helper
    {
        public static void AutoMeshRenderer()
        {
            foreach (var skMr in UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>())
            {
                if (skMr.GetComponent<SceneTrackObject>() == null)
                {
                    Unity.Helper.RecursiveBackwardsAddObject(skMr.transform);
                }
            }

            foreach (var mr in UnityEngine.Object.FindObjectsOfType<MeshRenderer>())
            {
                if (mr.GetComponent<SceneTrackObject>() == null)
                {
                    Unity.Helper.RecursiveBackwardsAddObject(mr.transform);
                }
            }
        }
    }
}