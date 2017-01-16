using System;
using SceneTrack;
using UnityEngine;

namespace SceneTrack.Unity
{
    public static class Classes
    {
        /// <summary>
        /// Register all of our class definitions with the SceneTrack system
        /// </summary>
        public static void CreateSchema()
        {
            GameObject.Register();
            Transform.Register();
            StandardMeshRenderer.Register();
            SkinnedMeshRenderer.Register();
            Bone.Register();
            Mesh.Register();
            SubMesh.Register();
            Material.Register();
        }


        public static class GameObject
        {
            public static uint Type = 0;
            public static uint Name = 0;
            public static uint Layer = 0;
            public static uint Transform = 0;
            public static uint Components = 0;
            public static uint Visibility = 0;


            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 100);
                Name = Object.AddObjectTypeComponentEx2(Type, Kind.Named, SceneTrack.Type.CString, 1, 1, Units.Unspecified, Reference.Unspecified);
                Layer = Object.AddObjectTypeComponentEx2(Type, Kind.Layer, SceneTrack.Type.Int32, 1, 1, Units.Unspecified, Reference.Unspecified);
                Transform = Object.AddObjectTypeComponentEx2(Type, Kind.Spatial, SceneTrack.Type.Uint32, 1, 1, Units.Unspecified, Reference.Unspecified);
                Components = Object.AddObjectTypeComponentEx2(Type, Kind.Child, SceneTrack.Type.Uint32, 1, int.MaxValue, Units.Unspecified, Reference.Unspecified);
                Visibility = Object.AddObjectTypeComponentEx2(Type, Kind.Intensity, SceneTrack.Type.Uint8, 1, 1, Units.Unspecified, Reference.Unspecified);
            }
        }


        public static class Transform
        {
            public static uint Type = 0;
            public static uint LocalPosition = 0;
            public static uint LocalRotation = 0;
            public static uint LocalScale = 0;
            public static uint Parent = 0;

            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 101);
                LocalPosition = Object.AddObjectTypeComponentEx2(Type, Kind.Position, SceneTrack.Type.Float32, 3, 1, Units.Unspecified, Reference.Local);
                LocalRotation = Object.AddObjectTypeComponentEx2(Type, Kind.Rotation, SceneTrack.Type.Float32, 3, 1, Units.Degree, Reference.Local);
                LocalScale = Object.AddObjectTypeComponentEx2(Type, Kind.Scale, SceneTrack.Type.Float32, 3, 1, Units.Unspecified, Reference.Local);
                Parent = Object.AddObjectTypeComponentEx2(Type, Kind.Parent, SceneTrack.Type.Uint32, 1, 1, Units.Unspecified, Reference.Unspecified);
            }
        }

        public static class StandardMeshRenderer
        {
            public static uint Type = 0;
            public static uint Materials = 0;
            public static uint Mesh = 0;
            public static uint Parent = 0;

            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 102);
                Materials = Object.AddObjectTypeComponentEx2(Type, Kind.Surface, SceneTrack.Type.Uint32, 1,
                    int.MaxValue, Units.Unspecified, Reference.Unspecified);
                Mesh = Object.AddObjectTypeComponentEx2(Type, Kind.Geometry, SceneTrack.Type.Uint32, 1, 1,
                    Units.Unspecified, Reference.Unspecified);
                Parent = Object.AddObjectTypeComponentEx2(Type, Kind.Parent, SceneTrack.Type.Uint32, 1, 1,
                    Units.Unspecified, Reference.Unspecified);
            }
        }
        public static class SkinnedMeshRenderer
        {
            public static uint Type = 0;
            public static uint Materials = 0;
            public static uint Mesh = 0;
            public static uint Bones = 0;
            public static uint Parent = 0;
            public static uint BoneTransform = 0;
          

            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 103);
                Materials = Object.AddObjectTypeComponentEx2(Type, Kind.Surface, SceneTrack.Type.Uint32, 1, int.MaxValue, Units.Unspecified, Reference.Unspecified);
                Mesh = Object.AddObjectTypeComponentEx2(Type, Kind.Geometry, SceneTrack.Type.Uint32, 1, 1, Units.Unspecified, Reference.Unspecified);
                Bones = Object.AddObjectTypeComponentEx2(Type, Kind.Bone, SceneTrack.Type.Uint32, 1, int.MaxValue, Units.Unspecified, Reference.Unspecified);
                Parent = Object.AddObjectTypeComponentEx2(Type, Kind.Parent, SceneTrack.Type.Uint32, 1, 1, Units.Unspecified, Reference.Unspecified);
                BoneTransform = Object.AddObjectTypeComponentEx2(Type, Kind.BoneBegin, SceneTrack.Type.Uint32, 1, 1, Units.Unspecified, Reference.Unspecified);
            }
        }

        public static class Bone
        {
            public static uint Type = 0;
            public static uint LocalPosition = 0;
            public static uint LocalRotation = 0;
            public static uint LocalScale = 0;
            public static uint Parent = 0;
            public static uint Id = 0;

            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 104);
                Parent = Object.AddObjectTypeComponentEx2(Type, Kind.Parent, SceneTrack.Type.Uint32, 1, 1, Units.Unspecified, Reference.Unspecified);
                LocalPosition = Object.AddObjectTypeComponentEx2(Type, Kind.Position, SceneTrack.Type.Float32, 3, 1, Units.Unspecified, Reference.Local);
                LocalRotation = Object.AddObjectTypeComponentEx2(Type, Kind.Rotation, SceneTrack.Type.Float32, 3, 1, Units.Degree, Reference.Local);
                LocalScale = Object.AddObjectTypeComponentEx2(Type, Kind.Scale, SceneTrack.Type.Float32, 3, 1, Units.Unspecified, Reference.Local);
                Id = Object.AddObjectTypeComponentEx2(Type, Kind.Named, SceneTrack.Type.Uint8, 1, 1, Units.Unspecified, Reference.Unspecified);
            }
        }

        public static class Mesh
        {
            public static uint Type = 0;
            public static uint Name = 0;
            public static uint Vertices = 0;
            public static uint Normals = 0;
            public static uint Tangents = 0;
            public static uint Colors = 0;
            public static uint UV = 0;
            public static uint UV2 = 0;
            public static uint UV3 = 0;
            public static uint UV4 = 0;
            public static uint BoneWeightWeight = 0;
            public static uint BoneWeightIndex = 0;
            public static uint BindPoses = 0;
            public static uint Bounds = 0;
            public static uint SubMesh = 0;


            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 201);
                Name = Object.AddObjectTypeComponentEx2(Type, Kind.Named, SceneTrack.Type.CString, 1, 1, Units.Unspecified, Reference.Unspecified);
                Vertices = Object.AddObjectTypeComponentEx2(Type, Kind.Vertex, SceneTrack.Type.Float32, 3, 1 << 16, Units.Unspecified, Reference.Local);
                Normals = Object.AddObjectTypeComponentEx2(Type, Kind.Normal, SceneTrack.Type.Float32, 3, 1 << 16, Units.Unspecified, Reference.Unspecified);
                Tangents = Object.AddObjectTypeComponentEx2(Type, Kind.Tangent, SceneTrack.Type.Float32, 4, 1 << 16, Units.Unspecified, Reference.Unspecified);
                Colors = Object.AddObjectTypeComponentEx2(Type, Kind.Color, SceneTrack.Type.Float32, 4, 1 << 16, Units.Unspecified, Reference.Unspecified);
                UV = Object.AddObjectTypeComponentEx2(Type, Kind.UV0, SceneTrack.Type.Float32, 2, 1 << 16, Units.Unspecified, Reference.Unspecified);
                UV2 = Object.AddObjectTypeComponentEx2(Type, Kind.UV1, SceneTrack.Type.Float32, 2, 1 << 16, Units.Unspecified, Reference.Unspecified);
                UV3 = Object.AddObjectTypeComponentEx2(Type, Kind.UV2, SceneTrack.Type.Float32, 2, 1 << 16, Units.Unspecified, Reference.Unspecified);
                UV4 = Object.AddObjectTypeComponentEx2(Type, Kind.UV3, SceneTrack.Type.Float32, 2, 1 << 16, Units.Unspecified, Reference.Unspecified);
                BoneWeightIndex = Object.AddObjectTypeComponentEx2(Type, Kind.BoneIndex, SceneTrack.Type.Uint8, 4, 1 << 16, Units.Unspecified, Reference.Unspecified);
                BoneWeightWeight = Object.AddObjectTypeComponentEx2(Type, Kind.BoneWeight, SceneTrack.Type.Float32, 4, 1 << 16, Units.Unspecified, Reference.Unspecified);
                Bounds = Object.AddObjectTypeComponentEx2(Type, Kind.Size, SceneTrack.Type.Float32, 3, 2, Units.Unspecified, Reference.Unspecified);
                SubMesh = Object.AddObjectTypeComponentEx2(Type, Kind.Geometry, SceneTrack.Type.Uint32, 1, 8, Units.Unspecified, Reference.Unspecified);
                BindPoses = Object.AddObjectTypeComponentEx2(Type, Kind.Pose, SceneTrack.Type.Float32, 16, 1 << 16, Units.Unspecified, Reference.Unspecified);
            }
        }

        public static class SubMesh
        {
            public static uint Type = 0;
            public static uint Indexes = 0;

            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Dynamic, 202);
                Indexes = Object.AddObjectTypeComponentEx2(Type, Kind.Index, SceneTrack.Type.Uint32, 1, 1 << 16, Units.Unspecified, Reference.Unspecified);
            }
        }

        public static class Material
        {
            public static uint Type = 0;
            public static uint Name = 0;
            public static uint Shader = 0;
            public static uint MainTexture = 0;

            public static void Register()
            {
                Type = Object.CreateObjectTypeEx(Frequency.Static, 203);
                Name = Object.AddObjectTypeComponentEx2(Type, Kind.Named, SceneTrack.Type.CString, 1, 1, Units.Unspecified, Reference.Unspecified);
                Shader = Object.AddObjectTypeComponentEx2(Type, Kind.Surface, SceneTrack.Type.CString, 1, 1, Units.Unspecified, Reference.Unspecified);
                MainTexture = Object.AddObjectTypeComponentEx2(Type, Kind.Image, SceneTrack.Type.CString, 1, 1, Units.Unspecified, Reference.ExternalAsset);
            }
        }
    }
}