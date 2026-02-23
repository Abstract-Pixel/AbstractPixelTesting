using AbstractPixel.Utility;
using AbstractPixel.Utility.Save;
using System.Collections;
using UnityEngine;

[Saveable(SaveCategory.Game)]
public class LameoSon : MonoBehaviour, ISaveable<Vector3Data>
{
    [SerializeField] Vector3 position;


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
