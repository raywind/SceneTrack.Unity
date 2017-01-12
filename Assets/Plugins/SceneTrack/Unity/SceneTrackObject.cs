using UnityEngine;

namespace SceneTrack.Unity
{
    // TODO: VALIDATE 100% that its top down initialization
    public class SceneTrackObject : MonoBehaviour
    {

#if UNITY_EDITOR

        /// <summary>
        /// Is this object at the root of the heiarchy?
        /// </summary>
        public bool RootObject
        {
            get { return _transformParentHandle == 0 ; }
        }

        /// <summary>
        /// Reference to the parent SceneTrackObject if available
        /// </summary>
        public SceneTrackObject ParentSceneTrackObject { get; private set; }

        /// <summary>
        /// Handle of the Transform Component
        /// </summary>
        /// <remarks>All GameObjects in Unity have transforms, so we always will have one.</remarks>
        public uint TransformHandle { get; private set; }

        public bool TrackObject = true;
        public bool TrackTransform = true;
        public bool TrackMeshRenderer;

        private uint _frameCount;
        private uint _handle;
        private bool _initialized;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private uint _meshRendererHandle;
        private SceneTrackObject _parentSceneTrackObject;
        private Transform _transform;
        private Transform _transformParent;
        private uint _transformParentHandle;

        #region Unity Specific Events

        /// <summary>
        /// Unity's Awake Event
        /// </summary>
        /// <remarks>This starts up SceneTrack's initialization, only once, and then initializes the component.</remarks>

        private void Awake()
        {
            // Initialize SceneTrack
            System.EnterPlayMode();


            // Initialize
            Init();
        }

        /// <summary>
        /// Unity's LateUpdate Event
        /// </summary>
        /// <remarks>Submits our data to SceneTrack to be saved, it only happens once across all components.</remarks>
        private void LateUpdate()
        {
            System.SubmitRecording(_frameCount, Time.deltaTime);
        }

        /// <summary>
        /// Unity's OnDestroy Event
        /// </summary>
        /// <remarks>Shutdown SceneTrack, cleaning out buffers, etc.</remarks>
        private void OnDestroy()
        {
            // Leave PlayMode
            System.ExitPlayMode();
        }

        /// <summary>
        /// Unity's Component Reset
        /// </summary>
        /// <remarks>Setup default tracking values based on what is on the Game Object</remarks>
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

        /// <summary>
        /// Unity's Update
        /// </summary>
        /// <remarks>Record any changes to the tracked objects</remarks>
        private void Update()
        {
            // Get last used frame number, we use this to determine if this is the first late update being called!
            _frameCount = System.FrameCount;

            // Record whats being tracked
            if (TrackTransform) { RecordTransform(); }
            if (TrackMeshRenderer) { RecordMeshRenderer(); }
        }

        #endregion

        #region SceneTrack Logic

        /// <summary>
        /// One-Time Initialization Of Object Description
        /// </summary>
        /// <remarks>Defines the object, and its properties inside of SceneTrack for recording purposes.</remarks>
        private void Init()
        {
            // Return if we've already initialized
            if (_initialized) return;

            // Register GameObject
            _handle = Object.CreateObject(Classes.GameObject.Type);

            // Register Components
            if ( TrackTransform )
            {
                InitTransform();
            }
            if ( TrackMeshRenderer )
            {
                InitMeshRenderer();
            }

            // Set flag as initialized
            _initialized = true;
        }

        /// <summary>
        /// Initialize SceenTrack MeshFilter/MeshRenderer Component
        /// </summary>
        private void InitMeshRenderer()
        {
            // Cache references
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// Initialize SceneTrack Transform Component
        /// </summary>
        private void InitTransform()
        {
            // Cache transform reference
            _transform = transform;

            // Create transform object in SceneTrack
            TransformHandle = Object.CreateObject(Classes.Transform.Type);

            // Assign Transform to the GameObject in SceneTrack
            Object.SetValue_uint32(_handle, Classes.GameObject.Transform, TransformHandle);

            // Do a force record of the transform, starting positions, etc.
            RecordTransform(true);
        }

        private void RecordMeshRenderer()
        {
            // TODO RECORD


        }

        /// <summary>
        /// Record Transform Changes
        /// </summary>
        /// <param name="forceUpdate">Force the data to be sent to SceneTrack</param>
        private void RecordTransform(bool forceUpdate = false)
        {
            // No need to update
            if (!forceUpdate && !_transform.hasChanged && !(_transform.parent != _transformParent)) return;

            // Check our parent, and reassign and setup it if necessary in SceneTrack
            if (_transform.parent != _transformParent)
            {
                // If we have no parent, make sure to set the handle to 0
                if (_transform.parent == null)
                {
                    _transformParentHandle = 0;
                }
                else
                {
                    // Check if we have a SceneTrackObject on the new parent, if we don't add one
                    _parentSceneTrackObject = _transform.parent.GetComponent<SceneTrackObject>() ??
                                              _transform.parent.gameObject.AddComponent<SceneTrackObject>();

                    // TODO: Possibly add forced init of parent if the order is wrong

                    // Setup some cached references
                    _transformParentHandle = _parentSceneTrackObject.TransformHandle;
                    _transformParent = _transform.parent;
                }

                // Assign our transform handle inside of SceneTrack as it will have changed
                Object.SetValue_uint32(TransformHandle, Classes.Transform.Parent, _transformParentHandle);
            }

            // Update SceneTrack
            var localCache = _transform.localPosition;
            Object.SetValue_3_float32(TransformHandle, Classes.Transform.LocalPosition, localCache.x,
                localCache.y, localCache.z);

            localCache = _transform.eulerAngles;
            Object.SetValue_3_float32(TransformHandle, Classes.Transform.LocalRotation, localCache.x,
                localCache.y, localCache.z);

            localCache = _transform.localScale;
            Object.SetValue_3_float32(TransformHandle, Classes.Transform.LocalScale, localCache.x,
                localCache.y, localCache.z);


            // Change changed flag on the Transform
            // NOTE: This may effect other scripts
        }
        #endregion
#endif
    }
}
