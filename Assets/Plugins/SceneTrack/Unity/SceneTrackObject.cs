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
        /// Should SceneTrack use the SkinnedMeshRenderer
        /// </summary>
        public bool IsSkinned { get; private set; }
    
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
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private uint _meshRendererHandle;
        private uint _skinnedMeshRendererHandle;
        private SceneTrackObject _parentSceneTrackObject;
        private SceneTrackObject _boneRootObject;
        private Transform _transform;
        private Transform _transformParent;
        private uint _transformParentHandle;

        private uint[] _materialHandles;
        private List<uint> _componentHandles;
        private uint _meshHandle;
        Bone[]  _bones;

        #region Unity Specific Events

        /// <summary>
        /// Unity's Awake Event
        /// </summary>
        /// <remarks>This starts up SceneTrack's initialization, only once, and then initializes the component.</remarks>

        private void Awake()
        {
            if (!TrackObject) return;

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
            if (!TrackObject) return;
            System.SubmitRecording(_frameCount, Time.deltaTime);
        }

        public void OnEnable()
        {
            if (!TrackObject) return;
            Object.SetValue_uint8(_handle, Classes.GameObject.Visibility, 1);
        }

        public void OnDisable()
        {
            if (!TrackObject) return;
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
            if (!TrackObject) return;

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
            if (_initialized || !TrackObject) return;
            
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
            if (_componentHandles.Count != 0)
            { 
              var componentArray = _componentHandles.ToArray();
              Helper.SubmitArray(_handle, Classes.GameObject.Components, componentArray, Helper.GetTypeMemorySize(typeof(uint), 1));
              componentArray = null;
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
            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

            // Determine if we need to use the SkinnedMeshRenderer Logic
            IsSkinned = _skinnedMeshRenderer != null;

            // Create Materials
            InitMaterials(IsSkinned ? _skinnedMeshRenderer.sharedMaterials : _meshRenderer.sharedMaterials);

            // Create Mesh
            InitMesh(IsSkinned ? _skinnedMeshRenderer.sharedMesh : _meshFilter.sharedMesh );

            // Create Renderer
            if (IsSkinned)
            {
                _skinnedMeshRendererHandle = SceneTrack.Object.CreateObject(Classes.SkinnedMeshRenderer.Type);

                // Assign Mesh (shared reference if found) to MeshRenderer
                Object.SetValue_uint32(_skinnedMeshRendererHandle, Classes.SkinnedMeshRenderer.Mesh, _meshHandle);

                // Assign Materials (shared references as found)
                Helper.SubmitArray(_skinnedMeshRendererHandle, Classes.SkinnedMeshRenderer.Materials, _materialHandles, Helper.GetTypeMemorySize(typeof(uint), 1));

                // Assign Bone Transform (Root Object)
                // Check if we have a SceneTrackObject on the bone root, if we don't add one
                // TODO: Robin/Matthew
                //  Check to see if this is true. I've seen RootBones that aren't a direct child of the SkinnedMeshRenderer.
                _boneRootObject = _skinnedMeshRenderer.rootBone.GetComponent<SceneTrackObject>() ??
                                  _skinnedMeshRenderer.rootBone.gameObject.AddComponent<SceneTrackObject>();
                Object.SetValue_uint32(_skinnedMeshRendererHandle, Classes.SkinnedMeshRenderer.BoneTransform, _boneRootObject.TransformHandle);

                // Assign Bones
                Transform[] cachedBones = _skinnedMeshRenderer.bones;
                int cachedBonesCount = cachedBones.Length;
        
                _bones = new Bone[cachedBonesCount];
                
                // First Pass
                for(int i=0;i < cachedBonesCount;i++)
                {
                  _bones[i] = new Bone(i, cachedBones[i]);
                }

                // Second Pass
                for(int i=0;i < cachedBonesCount;i++)
                {
                  _bones[i].Initialise(cachedBones, _bones);
                }
                
            }
            else
            {
                _meshRendererHandle = SceneTrack.Object.CreateObject(Classes.StandardMeshRenderer.Type);

                // Assign Mesh (shared reference if found) to MeshRenderer
                Object.SetValue_uint32(_meshRendererHandle, Classes.StandardMeshRenderer.Mesh, _meshHandle);

                // Assign Materials (shared references as found)
                var meshMaterialsHandle = GCHandle.Alloc(_materialHandles, GCHandleType.Pinned);
                var meshMaterialsPointer = meshMaterialsHandle.AddrOfPinnedObject();
                SceneTrack.Object.SetValue_p_uint32(_meshRendererHandle, Classes.StandardMeshRenderer.Materials,
                    meshMaterialsPointer, (uint) _materialHandles.Length, Helper.GetTypeMemorySize(typeof(uint), 1));
                meshMaterialsHandle.Free();
            }


            // Add to be included in component link
            _componentHandles.Add(_meshRendererHandle);
        }

        /// <summary>
        /// Initialize references to materials used on the renderers.
        /// </summary>
        /// <param name="materials">Array of materials used in the mesh.</param>
        private void InitMaterials(Material[] materials)
        {
            var materialHandles = new List<uint>();
            foreach (var m in materials)
            {
                uint materialHandle = 0;


                if (!Unity.System.SharedMaterials.TryGetValue(m, out materialHandle))
                {
                    materialHandle = Object.CreateObject(Classes.Material.Type);

                    if (m.mainTexture != null)
                    {
                        Helper.SubmitString(materialHandle, Classes.Material.MainTexture, m.mainTexture.name);
                        Helper.SubmitString(materialHandle, Classes.Material.Name, m.name);
                        Helper.SubmitString(materialHandle, Classes.Material.Shader, m.shader.name);
                    }
                    else
                    {
                        Helper.SubmitString(materialHandle, Classes.Material.MainTexture, "Default");
                        Helper.SubmitString(materialHandle, Classes.Material.Name, "Default");
                        Helper.SubmitString(materialHandle, Classes.Material.Shader, "Default");
                    }


                    System.SharedMaterials.Add(m, materialHandle);
                }


                // Store material in temp array
                materialHandles.Add(materialHandle);
            }
            _materialHandles = materialHandles.ToArray();
        }

        /// <summary>
        /// Initialize reference to the mesh used on the renderer
        /// </summary>
        /// <param name="mesh">The mesh</param>
        private void InitMesh(Mesh mesh)
        {
            // Create Mesh
            _meshHandle = 0;
            var cachedMesh = mesh;

            if (!System.SharedMeshes.TryGetValue(cachedMesh, out _meshHandle))
            {
                _meshHandle = Object.CreateObject(Classes.Mesh.Type);

                // Handle Name (C# String)
                Helper.SubmitString(_meshHandle, Classes.Mesh.Name, cachedMesh.name);

                // Handle Vertices (Vector3)
                Helper.SubmitArray(_meshHandle, Classes.Mesh.Vertices, cachedMesh.vertices);

                // Handle Normals (Vector3)
                if (cachedMesh.normals != null && cachedMesh.normals.Length != 0)
                { 
                  Helper.SubmitArray(_meshHandle, Classes.Mesh.Normals, cachedMesh.normals);
                }

                // Handle Tangents (Vector4)
                if (cachedMesh.tangents != null && cachedMesh.tangents.Length != 0)
                {
                  Helper.SubmitArray(_meshHandle, Classes.Mesh.Tangents, cachedMesh.tangents);
                }

                // Handle Colors (Color)
                // We have to make an array of values as colors are stored differently
                if (cachedMesh.colors != null && cachedMesh.colors.Length != 0)
                {
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.Colors, cachedMesh.colors);
                }

                // Handle UV (Vector2)
                if (cachedMesh.uv != null && cachedMesh.uv.Length != 0)
                {
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.UV, cachedMesh.uv);
                }

                // Handle UV2 (Vector2)
                if (cachedMesh.uv2 != null && cachedMesh.uv2.Length != 0)
                {
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.UV2, cachedMesh.uv2);
                }

                // Handle UV3 (Vector2)
                if (cachedMesh.uv3 != null && cachedMesh.uv3.Length != 0)
                {
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.UV3, cachedMesh.uv3);
                }

                // Handle UV4 (Vector2)
                if (cachedMesh.uv4 != null && cachedMesh.uv4.Length != 0)
                {
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.UV4, cachedMesh.uv4);
                }

                // Assign Bones
                if (IsSkinned && cachedMesh.boneWeights != null && cachedMesh.boneWeights.Length != 0 && cachedMesh.bindposes != null && cachedMesh.bindposes.Length != 0)
                {
                    // TODO: Robin
                    //  Add a second SubmitArray into SceneTrack, where I can read this directly in.
                    //  Which uses Strides and Pointer offsets, so there isn't a second copy.

                    var cachedBoneWeightLength = cachedMesh.boneWeights.Length;
                    var boneIndexes = new byte[cachedBoneWeightLength * 4];
                    var boneWeights = new Vector4[cachedBoneWeightLength];

                    for (var i = 0; i < cachedBoneWeightLength; i++)
                    {
                        var indexLocation = i * 4;
                        boneIndexes[indexLocation + 0] = (byte) cachedMesh.boneWeights[i].boneIndex0;
                        boneIndexes[indexLocation + 1] = (byte) cachedMesh.boneWeights[i].boneIndex1;
                        boneIndexes[indexLocation + 2] = (byte) cachedMesh.boneWeights[i].boneIndex2;
                        boneIndexes[indexLocation + 3] = (byte) cachedMesh.boneWeights[i].boneIndex3;

                        boneWeights[i] = new Vector4(cachedMesh.boneWeights[i].weight0,
                            cachedMesh.boneWeights[i].weight1,
                            cachedMesh.boneWeights[i].weight2,
                            cachedMesh.boneWeights[i].weight3);
                    }
                    
                    // The indices are stored as a ByteVector4 array inside of ST, we have to alter the data description here accordingly.
                    // Handle BoneWeight Indexes (ByteVector4 like)
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.BoneWeightIndex, boneIndexes, 4);
                    // Handle BoneWeight Vector  (Vector4 like)
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.BoneWeightWeight, boneWeights);

                    boneIndexes = null;
                    boneWeights = null;

                    // Handle Bind Pose (Matrix44)
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.BindPoses, cachedMesh.bindposes);

                }

                // Handle Bounds (Vector3[2])
                var bounds = new Vector3[2];
                bounds[0] = _meshRenderer.bounds.min;
                bounds[1] = _meshRenderer.bounds.max;
                Helper.SubmitArray(_meshHandle, Classes.Mesh.Bounds, bounds);
                bounds = null;

                // Create Sub Meshes (If we have any!)
                if (cachedMesh.subMeshCount > 0)
                {
                    var subMeshHandles = new uint[cachedMesh.subMeshCount];


                    for (var i = 0; i < cachedMesh.subMeshCount; i++)
                    {
                        var subMeshIndices = cachedMesh.GetIndices(i);

                        // Create component
                        var subMeshHandle = SceneTrack.Object.CreateObject(Classes.SubMesh.Type);
            
                        // Handle Indexes (int (written as UInt))
                        Helper.SubmitArrayForceUInt32(subMeshHandle, Classes.SubMesh.Indexes, subMeshIndices, Helper.GetTypeMemorySize(typeof(uint), 1));
            
                        // Assign Submesh Index
                        subMeshHandles[i] = subMeshHandle;
                    }

                    // Assign index to submesh on mesh
                    Helper.SubmitArray(_meshHandle, Classes.Mesh.SubMesh, subMeshHandles, Helper.GetTypeMemorySize(typeof(uint), 1));
                }

                System.SharedMeshes.Add(cachedMesh, _meshHandle);
            }
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
      
            // Reset the flag
            // NOTE: This may effect other scripts
            _transform.hasChanged = false;

        }
        #endregion

        #region Bones

        public class Bone
        {
          public Transform _transform;
          public uint      _handle;
          public SceneTrackMultiBoneProxy _proxy;

          public Bone(int id, Transform boneTransform)
          {
            _transform = boneTransform;
            _handle = SceneTrack.Object.CreateObject(Classes.Bone.Type);
            Object.SetValue_uint8(_handle, Classes.Bone.Id, (byte) id);
            
            _proxy = boneTransform.GetComponent<SceneTrackMultiBoneProxy>();

            if (_proxy == null)
            {
              _proxy = boneTransform.gameObject.AddComponent<SceneTrackMultiBoneProxy>();
            }

            _proxy.Bones.Add(this);
          }

          public void Initialise(Transform[] transform, Bone[] bones)
          {
            Transform parent = _transform.parent;
            if (parent != null)
            {
              int parentIndex = global::System.Array.IndexOf(transform, parent);
              if (parentIndex != -1)
              {
                Bone boneParent = bones[parentIndex];
                Object.SetValue_uint32(_handle, Classes.Bone.Parent, boneParent._handle);
              }
            }
          }

          public void Update(Vector3 position, Vector3 rotation, Vector3 scale)
          {
            Object.SetValue_3_float32(_handle, Classes.Bone.LocalPosition, position.x,
                position.y, position.z);
        
            Object.SetValue_3_float32(_handle, Classes.Bone.LocalRotation, rotation.x,
                rotation.y, rotation.z);
        
            Object.SetValue_3_float32(_handle, Classes.Bone.LocalScale, scale.x,
                scale.y, scale.z);
          }

        }

        #endregion

#endif
  }
}
