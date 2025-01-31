﻿using UnityEngine;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{
    [CustomEditor(typeof(SceneTrackMultiBoneProxy))]
    public class SceneTrackMultiBroneProxyEditor : UnityEditor.Editor
    {
        private SceneTrackMultiBoneProxy _targetObject;

        public override void OnInspectorGUI()
        {

            // Get Current Reference
            _targetObject = (SceneTrackMultiBoneProxy)target;

            if ( !Application.isPlaying ) {

                SceneTrack.Unity.Log.Error("Removing SceneTrackMultiBoneProxy off of " + _targetObject.gameObject.name + ". These are meant to be autogenerated.");

                // TODO: This spawns an error because the drag isn't completed
                UnityEngine.Object.DestroyImmediate(_targetObject);
            }

        }
    }
}
