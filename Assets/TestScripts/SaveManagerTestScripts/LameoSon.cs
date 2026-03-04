using AbstractPixel.Core;
using AbstractPixel.SaveSystem;
using UnityEngine;

[Savable(SaveCategory.Game)]
public class LameoSon : MonoBehaviour, ISavable<Vector3Data>
{
    [ReadOnly(true)] public Vector3 position;


    public Vector3Data CaptureData()
    {
        Vector3Data data = position;
        return data;
    }

    public void RestoreData(Vector3Data _loadedData)
    {
        position = _loadedData;
    }
}
