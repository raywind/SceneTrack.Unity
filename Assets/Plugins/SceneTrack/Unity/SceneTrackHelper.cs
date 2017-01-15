﻿using System.CodeDom;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SceneTrack.Unity
{
    public static class Helper
    {
        public static uint GetTypeMemorySize(global::System.Type objectType, uint elements, uint arraySize)
        {
            if (objectType == typeof(float))
            {
                return (uint) sizeof(float) * elements * arraySize;
            }
            else if (objectType == typeof(int))
            {
                return (uint) sizeof(int) * elements * arraySize;
            }
            else if (objectType == typeof(uint))
            {
                return (uint) sizeof(uint) * elements * arraySize;
            }
            else if (objectType == typeof(byte))
            {
                return sizeof(byte) * elements * arraySize;
            }
            return 0;
        }
    }
}