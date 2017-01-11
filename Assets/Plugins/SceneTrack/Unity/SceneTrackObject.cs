using System.IO;
using UnityEngine;

namespace SceneTrack.Unity
{
    public class SceneTrackObject : MonoBehaviour
    {
        public bool TrackTransform;
        public bool TrackMeshRenderer;

        private uint _handle;
        private uint _frameCount;

        private bool _rootObject = true;
        

        private void Awake()
        {
            // Initialize SceneTrack
            System.EnterPlayMode();

            // Register GameObject
            _handle = SceneTrack.Object.CreateObject(SceneTrack.Unity.Classes.GameObject.Type);

            // Register Components
        }
        private void Update()
        {
            // Get last used frame number, we use this to determine if this is the first late update being called!
            _frameCount = SceneTrack.Unity.System.FrameCount;
        }

        /// <summary>
        /// The last call during a frame, this allows us to have a consistent call point for all data
        /// </summary>
        private void LateUpdate()
        {
            System.SubmitRecording(_frameCount, Time.deltaTime);
        }

        private void OnDestroy()
        {
            // Leave PlayMode
            System.ExitPlayMode();
        }
    }
}
