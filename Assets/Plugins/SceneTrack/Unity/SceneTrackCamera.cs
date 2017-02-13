﻿using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class SceneTrackCamera : MonoBehaviour
{
    private static int textureWidth = 640;
    private static int textureHeight = 360;

    public bool TrackCamera { get; set; }
    public bool DisableCamera = true;

    private Camera _cameraReference;
    private RenderTexture _renderTexture;
    private Texture2D _proxyTexture;

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
    }

    public void LateUpdate()
    {
        // Framerate Delay


        //RenderCamera();

        // Submit Data
        //_proxyTexture.GetPixels();
    }

    private void RenderCamera()
    {

        // Create render texture
        _renderTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.RGB565);

        // Create new proxy texture
        _proxyTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, false);

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
        DestroyImmediate(_renderTexture);
    }
}