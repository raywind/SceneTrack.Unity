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
            if ( Application.isPlaying ) {
                EditorGUILayout.HelpBox("Changes Are Not Permitted During PlayMode", MessageType.Info);
                return;
            }




            
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

            GUILayout.Space(10);

            // Calculate Children

            EditorGUILayout.LabelField("Children Trackers", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("Add", EditorStyles.miniButtonLeft))
            {
                // Look through all children (even inactive ones)
                foreach(var t in _targetObject.GetComponentsInChildren<Transform>(true))
                {
                    if (t.GetComponent<SceneTrackObject>() == null)
                    {
                        t.gameObject.AddComponent<SceneTrackObject>();
                    }
                }

                // Update Cache List
                SceneTrack.Unity.System.CacheKnownObjects();
            }

            if (GUILayout.Button("Remove", EditorStyles.miniButtonMid))
            {
                // Look through all children (even inactive ones)
                foreach(var t in _targetObject.GetComponentsInChildren<Transform>(true))
                {
                    var reference = t.GetComponent<SceneTrackObject>();
                    if (reference != null && reference != _targetObject )
                    {
                        UnityEngine.Object.DestroyImmediate(reference);
                    }
                }

                // Update Cache List
                SceneTrack.Unity.System.CacheKnownObjects();
            }

            if (GUILayout.Button("Enable", EditorStyles.miniButtonMid))
            {
                // Look through all children (even inactive ones)
                foreach(var t in _targetObject.GetComponentsInChildren<Transform>(true))
                {
                    var reference = t.GetComponent<SceneTrackObject>();
                    if (reference != null && reference != _targetObject )
                    {
                        reference.TrackObject = true;
                    }
                }

                // Update Cache List
                SceneTrack.Unity.System.CacheKnownObjects();
            }

            if (GUILayout.Button("Disable", EditorStyles.miniButtonRight))
            {
                // Look through all children (even inactive ones)
                foreach(var t in _targetObject.GetComponentsInChildren<Transform>(true))
                {
                    var reference = t.GetComponent<SceneTrackObject>();
                    if (reference != null && reference != _targetObject )
                    {
                        reference.TrackObject = false;
                    }
                }

                // Update Cache List
                SceneTrack.Unity.System.CacheKnownObjects();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_targetObject);
            }
        }
    }
}
