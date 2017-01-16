using System.CodeDom;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SceneTrack.Unity
{
    public static class Helper
    {
        public static uint GetTypeMemorySize(global::System.Type objectType, uint elements)
        {
            if (objectType == typeof(float))
            {
                return (uint) sizeof(float) * elements;
            }
            else if (objectType == typeof(int))
            {
                return (uint) sizeof(int) * elements;
            }
            else if (objectType == typeof(uint))
            {
                return (uint) sizeof(uint) * elements;
            }
            else if (objectType == typeof(byte))
            {
                return sizeof(byte) * elements;
            }
            return 0;
        }

        public static Vector4 ToVector4(this Color color)
        {
            return new Vector4(color.r,color.g, color.b, color.a);
        }
    }
}