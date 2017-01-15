using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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
        /// <summary>
        /// Game Object Handle
        /// </summary>
        private uint _handle;

        private bool _initialized;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private uint _meshRendererHandle;
        private SceneTrackObject _parentSceneTrackObject;
        private Transform _transform;
        private Transform _transformParent;
        private uint _transformParentHandle;

        private uint[] _materialHandles;
        private List<uint> _componentHandles;
        private uint _meshHandle;

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

        public void OnEnable()
        {
            Object.SetValue_uint8(_handle, Classes.GameObject.Visibility, 1);
        }

        public void OnDisable()
        {
            Object.SetValue_uint8(_handle, Classes.GameObject.Visibility, 0);
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


            // Clear Component Cache
            _componentHandles = new List<uint>();

            // Register Components
            if ( TrackTransform )
            {
                InitTransform();
            }
            if ( TrackMeshRenderer )
            {
                InitMeshRenderer();
            }

            // Add components to game object, transform not needed
            var componentArray = _componentHandles.ToArray();
            var componentsHandle = GCHandle.Alloc(componentArray, GCHandleType.Pinned);
            var componentsPointer = componentsHandle.AddrOfPinnedObject();
            SceneTrack.Object.SetValue_p_uint32(_handle, Classes.GameObject.Components, componentsPointer, (uint)_componentHandles.Count, Helper.GetTypeMemorySize(typeof(uint), (uint)_componentHandles.Count, 1));
            componentsHandle.Free();


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

            // Create Materials
            var materialHandles = new List<uint>();
            foreach (var m in _meshRenderer.sharedMaterials)
            {
                uint materialHandle = 0;


                if (!Unity.System.SharedMaterials.TryGetValue(m, out materialHandle))
                {
                    materialHandle = Object.CreateObject(Classes.Material.Type);

                    if (m.mainTexture != null)
                    {
                        Object.SetValue_string(materialHandle, Classes.Material.Image, new StringBuilder(m.mainTexture.name), (uint) m.mainTexture.name.Length);
                        Object.SetValue_string(materialHandle, Classes.Material.Name, new StringBuilder(m.name), (uint) m.name.Length);
                        Object.SetValue_string(materialHandle, Classes.Material.Shader, new StringBuilder(m.shader.name), (uint) m.shader.name.Length);
                    }
                    else
                    {
                        Object.SetValue_string(materialHandle, Classes.Material.Image, new StringBuilder("Default"), 7);
                        Object.SetValue_string(materialHandle, Classes.Material.Name, new StringBuilder("Default"), 7);
                        Object.SetValue_string(materialHandle, Classes.Material.Shader, new StringBuilder("Default"), 7);
                    }


                    System.SharedMaterials.Add(m, materialHandle);
                }


                // Store material in temp array
                materialHandles.Add(materialHandle);
            }
            _materialHandles = materialHandles.ToArray();

            // Create Mesh
            _meshHandle = 0;
            var cachedMesh = _meshFilter.sharedMesh;


            if (!System.SharedMeshes.TryGetValue(cachedMesh, out _meshHandle))
            {
                _meshHandle = Object.CreateObject(Classes.Mesh.Type);

                Object.SetValue_string(_meshHandle, Classes.Mesh.Name, new StringBuilder(cachedMesh.name),
                    (uint) cachedMesh.name.Length);

                // Cache length
                var cachedLength = (uint) cachedMesh.vertices.Length;

                // Handle Vertices
                var verticesHandle = GCHandle.Alloc(cachedMesh.vertices, GCHandleType.Pinned);
                var verticesPointer = verticesHandle.AddrOfPinnedObject();
                Object.SetValue_p_float32(_meshHandle, Classes.Mesh.Vertices, verticesPointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 3));
                verticesHandle.Free();

                // Handle Normals
                var normalsHandle = GCHandle.Alloc(cachedMesh.normals, GCHandleType.Pinned);
                var normalsPointer = normalsHandle.AddrOfPinnedObject();
                Object.SetValue_p_float32(_meshHandle, Classes.Mesh.Normals, normalsPointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 3));
                normalsHandle.Free();

                // Handle Tangents (Vector4)
                var tangentsHandle = GCHandle.Alloc(cachedMesh.tangents, GCHandleType.Pinned);
                var tangentsPointer = tangentsHandle.AddrOfPinnedObject();
                Object.SetValue_p_float32(_meshHandle, Classes.Mesh.Tangents, tangentsPointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 4));
                tangentsHandle.Free();

                // Handle Colors (Vector4)
                // We have to make an array of values as colors are stored differently
                if (cachedMesh.colors != null && cachedMesh.colors.Length != 0)
                {
                    var colorsArray = new Vector4[cachedMesh.colors.Length];
                    for (var i = 0; i < cachedLength; i++)
                    {
                        colorsArray[i] = Classes.Mesh.ToVector4(cachedMesh.colors[i]);
                    }
                    var colorsHandle = GCHandle.Alloc(colorsArray, GCHandleType.Pinned);
                    var colorsPointer = colorsHandle.AddrOfPinnedObject();
                    Object.SetValue_p_float32(_meshHandle, Classes.Mesh.Colors, colorsPointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 4));
                    colorsHandle.Free();
                }

                // Handle UV
                if (cachedMesh.uv != null && cachedMesh.uv.Length != 0)
                {
                    var uvHandle = GCHandle.Alloc(cachedMesh.uv, GCHandleType.Pinned);
                    var uvPointer = uvHandle.AddrOfPinnedObject();
                    Object.SetValue_p_float32(_meshHandle, Classes.Mesh.UV, uvPointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 2));
                    uvHandle.Free();
                }

                // Handle UV2
                if (cachedMesh.uv2 != null && cachedMesh.uv2.Length != 0)
                {
                    var uv2Handle = GCHandle.Alloc(cachedMesh.uv2, GCHandleType.Pinned);
                    var uv2Pointer = uv2Handle.AddrOfPinnedObject();
                    Object.SetValue_p_float32(_meshHandle, Classes.Mesh.UV2, uv2Pointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 2));
                    uv2Handle.Free();
                }

                // Handle UV3
                if (cachedMesh.uv3 != null && cachedMesh.uv3.Length != 0)
                {
                    var uv3Handle = GCHandle.Alloc(cachedMesh.uv3, GCHandleType.Pinned);
                    var uv3Pointer = uv3Handle.AddrOfPinnedObject();
                    Object.SetValue_p_float32(_meshHandle, Classes.Mesh.UV3, uv3Pointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 2));
                    uv3Handle.Free();
                }

                // Handle UV4
                if (cachedMesh.uv4 != null && cachedMesh.uv4.Length != 0)
                {
                    var uv4Handle = GCHandle.Alloc(cachedMesh.uv4, GCHandleType.Pinned);
                    var uv4Pointer = uv4Handle.AddrOfPinnedObject();
                    Object.SetValue_p_float32(_meshHandle, Classes.Mesh.UV4, uv4Pointer, cachedLength, Helper.GetTypeMemorySize(typeof(float), cachedLength, 2));
                    uv4Handle.Free();
                }

                // Assign Bones
                var cachedBoneLength = cachedMesh.boneWeights.Length;
                var boneIndexes = new byte[cachedBoneLength * 4];
                var boneWeights = new Vector4[cachedBoneLength];

                for (var i = 0; i < cachedBoneLength; i++)
                {
                    var indexLocation = i * 4;
                    boneIndexes[indexLocation + 0] = (byte)cachedMesh.boneWeights[i].boneIndex0;
                    boneIndexes[indexLocation + 1] = (byte)cachedMesh.boneWeights[i].boneIndex1;
                    boneIndexes[indexLocation + 2] = (byte)cachedMesh.boneWeights[i].boneIndex2;
                    boneIndexes[indexLocation + 3] = (byte)cachedMesh.boneWeights[i].boneIndex3;

                    boneWeights[i] = new Vector4(cachedMesh.boneWeights[i].weight0, cachedMesh.boneWeights[i].weight1,
                        cachedMesh.boneWeights[i].weight2, cachedMesh.boneWeights[i].weight3);
                }
                var boneIndexHandle = GCHandle.Alloc(boneIndexes, GCHandleType.Pinned);
                var boneIndexPointer = boneIndexHandle.AddrOfPinnedObject();
                Object.SetValue_p_float32(_meshHandle, Classes.Mesh.BoneWeightIndex, boneIndexPointer, (uint)cachedBoneLength * 4, Helper.GetTypeMemorySize(typeof(byte), (uint)cachedBoneLength * 4, 1));
                boneIndexHandle.Free();

                var boneWeightsHandle = GCHandle.Alloc(boneWeights, GCHandleType.Pinned);
                var boneWeightsPointer = boneWeightsHandle.AddrOfPinnedObject();
                Object.SetValue_p_float32(_meshHandle, Classes.Mesh.BoneWeightWeight, boneWeightsPointer, (uint)cachedBoneLength, Helper.GetTypeMemorySize(typeof(float), (uint)cachedBoneLength, 4));
                boneIndexHandle.Free();

                // Assign Pose
                // TODO: Implement


                // Assign Bone Trasnform Stuff
                // TODO: Robin?

                // Assign Bounds
                var bounds = new Vector3[2];
                bounds[0] = _meshRenderer.bounds.min;
                bounds[1] = _meshRenderer.bounds.max;
                var boundsHandle = GCHandle.Alloc(bounds, GCHandleType.Pinned);
                var boundsPointer = boundsHandle.AddrOfPinnedObject();
                Object.SetValue_p_float32(_meshHandle, Classes.Mesh.Bounds, boundsPointer, 2, Helper.GetTypeMemorySize(typeof(float), 2, 3));
                boundsHandle.Free();

                // Create Sub Meshes (If we have any!)
                if (cachedMesh.subMeshCount > 0)
                {
                    var subMeshPointers = new uint[cachedMesh.subMeshCount];


                    for (var i = 0; i < cachedMesh.subMeshCount; i++)
                    {
                        var subMeshIndices = cachedMesh.GetIndices(i);
                        var cachedIndicesLength = (uint)subMeshIndices.Length;

                        // Create component
                        var newSubMesh = SceneTrack.Object.CreateObject(Classes.SubMesh.Type);

                        var newSubMeshHandle = GCHandle.Alloc(subMeshIndices, GCHandleType.Pinned);
                        var newSubMeshPointer = newSubMeshHandle.AddrOfPinnedObject();
                        Object.SetValue_p_int32(newSubMesh, Classes.SubMesh.Indexes, newSubMeshPointer, cachedIndicesLength, Helper.GetTypeMemorySize(typeof(int), cachedIndicesLength, 1));
                        newSubMeshHandle.Free();

                        // Assign Submesh Index
                        subMeshPointers[i] = newSubMesh;
                    }

                    // Assign index to submesh on mesh
                    var subMeshListHandle = GCHandle.Alloc(subMeshPointers, GCHandleType.Pinned);
                    var subMeshListPointer = subMeshListHandle.AddrOfPinnedObject();
                    Object.SetValue_p_uint32(_meshHandle, Classes.Mesh.SubMesh, subMeshListPointer, (uint)cachedMesh.subMeshCount, Helper.GetTypeMemorySize(typeof(uint), (uint)cachedMesh.subMeshCount, 1));
                    subMeshListHandle.Free();
                }

                System.SharedMeshes.Add(cachedMesh, _meshHandle);
            }

            // Create Renderer
            _meshRendererHandle = SceneTrack.Object.CreateObject(Classes.StandardMeshRenderer.Type);

            // Assign Mesh (shared reference if found) to MeshRenderer
            Object.SetValue_uint32(_meshRendererHandle, Classes.StandardMeshRenderer.Mesh, _meshHandle);

            // Assign Materials (shared references as found)
            var meshMaterialsHandle = GCHandle.Alloc(_materialHandles, GCHandleType.Pinned);
            var meshMaterialsPointer = meshMaterialsHandle.AddrOfPinnedObject();
            SceneTrack.Object.SetValue_p_uint32(_meshRendererHandle, Classes.StandardMeshRenderer.Materials, meshMaterialsPointer, (uint)_materialHandles.Length, Helper.GetTypeMemorySize(typeof(uint), (uint) _materialHandles.Length, 1));
            meshMaterialsHandle.Free();


            // Add to be included in component link
            _componentHandles.Add(_meshRendererHandle);
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
