using System.IO;
using UnityEngine;

namespace SceneTrack.Unity
{
    public class SceneTrackObject : MonoBehaviour
    {
        public bool TrackTransform;
        public bool TrackMeshRenderer;

        private uint _handle;
        private bool _rootObject = true;

        private void Awake()
        {
            // Initialize SceneTrack
            // Don't worry it only does it once, but we dont control the order of operations on which MB calls first so each one has the call
            System.EnterPlayMode();

            // Register GameObject


            // Register Components
        }

        private void OnDestroy()
        {
            // Leave PlayMode
            System.ExitPlayMode();
        }
    }
}
