using System.IO;
using UnityEngine;

namespace SceneTrack.Unity
{
    public class SceneTrackObject : MonoBehaviour
    {

#if UNITY_EDITOR

        public bool TrackObject = true;
        public bool TrackTransform = true;
        public bool TrackMeshRenderer = false;

        private uint _handle;
        private uint _frameCount;

        private bool _rootObject = true;

        
        

        private Transform _transform;
        private uint _transformHandle;

        private MeshRenderer _meshRenderer;
        private uint _meshRendererHandle;
        private MeshFilter _meshFilter;


        private void Awake()
        {
            // Initialize SceneTrack
            System.EnterPlayMode();


            // Register GameObject
            _handle = SceneTrack.Object.CreateObject(SceneTrack.Unity.Classes.GameObject.Type);

            // Register Components
            if ( TrackTransform ) 
            {
                InitTransform();
            }

            if ( TrackMeshRenderer ) 
            {
                InitMeshRenderer();
            }
        }
        private void Update()
        {
            // Get last used frame number, we use this to determine if this is the first late update being called!
            _frameCount = SceneTrack.Unity.System.FrameCount;


            if (TrackTransform) { RecordTransform(); }
            if (TrackMeshRenderer) { RecordMeshRenderer(); }
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

        private void Reset()
        {
            // This should never really fail, but we'll check it for the sake of checking
            if ( GetComponent<Transform>() != null ) {
                TrackTransform = true;
            }

            // Check if there is a mesh render and enable
            if ( GetComponent<MeshRenderer>() != null ) {
                TrackMeshRenderer = true;
            }
        }

        private void InitTransform()
        {
            // Cache transform reference
            _transform = transform;

            RecordTransform();
        }
        private void RecordTransform()
        {
            Vector3 localCache = _transform.localPosition;
            SceneTrack.Object.SetValue_3_float32(_handle, Classes.Transform.LocalPosition, localCache.x, localCache.y, localCache.z);
            
            localCache = _transform.eulerAngles;
            SceneTrack.Object.SetValue_3_float32(_handle, Classes.Transform.LocalRotation, localCache.x, localCache.y, localCache.z);

            localCache = _transform.localScale;
            SceneTrack.Object.SetValue_3_float32(_handle, Classes.Transform.LocalScale, localCache.x, localCache.y, localCache.z);
        }

        private void InitMeshRenderer()
        {
            // Cache references
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        private void RecordMeshRenderer()
        {
            // TODO RECORD
        }
    }

#endif

}
