using AbstractPixel.Utility.Save;
using UnityEngine;

namespace AbstractPixel.Utility
{
    public  interface ISaveable<T>
    {
        public T CaptureData();
        public void RestoreData(T _loadedData); 
    }
}
