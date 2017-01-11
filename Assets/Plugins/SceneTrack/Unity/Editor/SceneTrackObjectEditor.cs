using UnityEngine;
using UnityEditor;

namespace SceneTrack.Unity.Editor
{


    [CustomEditor(typeof(SceneTrackObject))]
    public class SceneTrackObjectEditor : UnityEditor.Editor
    {
        private SceneTrackObject _targetObject;

        public override void OnInspectorGUI()
        {
            // Get Current Reference
            _targetObject = (SceneTrackObject)target;

            GUILayout.Space(10);

            _targetObject.TrackObject = EditorGUILayout.Toggle("Track Object", _targetObject.TrackObject);

            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(!_targetObject.TrackObject);

            EditorGUILayout.LabelField("Track Components", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            _targetObject.TrackTransform = EditorGUILayout.Toggle("Transform", _targetObject.TrackTransform);
            _targetObject.TrackMeshRenderer = EditorGUILayout.Toggle("Mesh", _targetObject.TrackMeshRenderer);
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_targetObject);
            }
        }
    }
}
