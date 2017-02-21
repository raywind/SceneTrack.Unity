using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace SceneTrack.Unity
{
    public class SceneTrackCamera : MonoBehaviour
    {
        private static int textureWidth = 640;
        private static int textureHeight = 360;

        public bool TrackCamera { get; set; }
        public bool DisableCamera = true;

        private Camera _cameraReference;
        private RenderTexture _renderTexture;
        private Texture2D _proxyTexture;
        private uint _frameID;

        public void Awake()
        {
            // Get local reference to camera
            _cameraReference = GetComponent<Camera>();

            if (_cameraReference != Camera.main)
            {
                if (DisableCamera)
                {
                    _cameraReference.enabled = false;
                }
            }

            // Initialize SceneTrack
            System.EnterPlayMode();

            // Create Camera
            SceneTrack.Unity.Classes.CreateSchema();

            // Create new proxy texture
            _proxyTexture = new Texture2D(textureWidth, textureHeight);

            // Create Camera Frame ID
            _frameID = SceneTrack.Object.CreateObject(SceneTrack.Unity.Classes.VideoFrame.Type);
        }

        public void LateUpdate()
        {
            // Render the camera (we really could add framerate delays here)
            RenderCamera();

            // Save Frame
            SceneTrack.Object.SetValue_2_uint32(_frameID, SceneTrack.Unity.Classes.VideoFrame.Size, (uint)textureWidth, (uint)textureHeight);
            SceneTrack.Unity.Helper.SubmitArray(_frameID,
                SceneTrack.Unity.Classes.VideoFrame.Image,
                _proxyTexture.GetPixels32(),
                SceneTrack.Object.CalculateStride1(SceneTrack.Type.Uint8, 4));
        }


        private void RenderCamera()
        {

            // Create render texture
            _renderTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 32, RenderTextureFormat.ARGB32);

            // Assign camera output
            _cameraReference.targetTexture = _renderTexture;

            // Render camera
            _cameraReference.Render();

            // Make render texture the current active in engine (so the ReadPixels gets it)
            RenderTexture.active = _renderTexture;

            // Read RenderTexture data
            _proxyTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0, false);

            // Reset things
            _cameraReference.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
}