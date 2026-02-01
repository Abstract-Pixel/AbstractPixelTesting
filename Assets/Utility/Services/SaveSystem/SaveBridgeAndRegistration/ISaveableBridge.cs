using System.Xml;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public interface  ISaveableBridge
    {
        public string UniqueId{  get; }

        public object CaptureState(SaveCategory caregoryFilter);

        public void RestoreState(object data, SaveCategory caregoryFilter);
      
    }
}
