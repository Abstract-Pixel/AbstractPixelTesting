using UnityEngine;
using Newtonsoft.Json;
using System;

namespace AbstractPixel.Utility.Save
{
    public class JsonSerializer : ISerializer
    {
        public bool TrySerialize<T>(T _data, out string _output)
        {
            if (_data == null)
            {
                Debug.LogError("JsonSerializer: Cannot serialize null data.");
                _output = null;
                return false;
            }
            if(_data is UnityEngine.Object)
            {
                Debug.LogError("JsonSerializer: Cannot serialize UnityEngine.Object types.");
                _output = null;
                return false;
            }

            try
            {
                _output = JsonConvert.SerializeObject(_data);
                return true;

            }
            catch (Exception e)
            {
                Debug.LogError($"JsonSerializer: Serialization failed with exception: {e.Message} {e.StackTrace}");
                _output = null;
                return false;
            }

        }
        public bool TryDeserialize<T>(string _data, out T _output)
        {
            if (string.IsNullOrEmpty(_data))
            {
                Debug.LogError("JsonSerializer: Cannot deserialize null or empty string.");
                _output = default;
                return false;
            }
            try
            {
                _output = JsonConvert.DeserializeObject<T>(_data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"JsonSerializer: Deserialization failed with exception: {e.Message} {e.StackTrace}");
                _output = default;
                return false;
            }
        }
    }
}
