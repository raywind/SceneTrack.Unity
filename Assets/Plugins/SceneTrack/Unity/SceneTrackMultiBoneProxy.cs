using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SceneTrack.Unity
{
  
#if UNITY_EDITOR
    // Proxy Object, for transform.hasChanged, alerts multiple bones once it has changed
    // Created automatically through SceneTrackObject should not be added through the editor!
    [DisallowMultipleComponent]
    public class SceneTrackMultiBoneProxy : MonoBehaviour
    {
        public List<SceneTrackObject.Bone> Bones = new List<SceneTrackObject.Bone>(1); 
    
        Transform _transform;

        private void Awake()
        {
          _transform = transform;
        }

        private void Update()
        {
            if (_transform.hasChanged)
            {
              Vector3 position = _transform.localPosition;
              Vector3 rotation = _transform.localEulerAngles;
              Vector3 scale    = _transform.localScale;

              foreach(var bone in Bones)
              {
                bone.Update(position, rotation, scale);
              }
              _transform.hasChanged = false;
            }
        }
  }
#endif
}
