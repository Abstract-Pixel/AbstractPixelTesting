using System.Reflection;
using System;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveableTarget
    {
        public MonoBehaviour Script;
        public MethodInfo CaptureDataMethod;
        public MethodInfo RestoreDataMethod;
        public Type DataToSaveType;
        public string ClassKey = default;

    }
}
