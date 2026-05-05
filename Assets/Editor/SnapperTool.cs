using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class SnapperTool : EditorWindow
{
    const string UNDO_SNAP_COMMAND = "SnapGameObjects";
    const string SNAP_INDEX_KEY ="Selected_Snap_Index";
    private float snapUnit = 1f;
    private float[] snapOptions = {0.25f,0.5f,1f,2f};

    int snapUnitSelectedIndex;
    private void OnEnable()
    {
        Selection.selectionChanged += Repaint;
        snapUnitSelectedIndex=EditorPrefs.GetInt(SNAP_INDEX_KEY, 0);
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        EditorPrefs.SetInt(SNAP_INDEX_KEY, snapUnitSelectedIndex);
    }

    [MenuItem("Tools/Snapper")]
    public static void OpenSnapperWindow() => GetWindow<SnapperTool>("Snapper");


    private void OnGUI()
    {
        string[] options = { snapOptions[0] + " Units", snapOptions[1] + " Units", snapOptions[2] + " Units", snapOptions[3] + " Units" };
        snapUnitSelectedIndex = EditorGUILayout.Popup("Snapping Unit",snapUnitSelectedIndex, options);
        snapUnit = snapOptions[snapUnitSelectedIndex];


        if (Selection.gameObjects.Length<=0)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled=true;
        }
        if(GUILayout.Button("Snap Selection"))
        {
            SnapMultipleGameObjects(Selection.gameObjects);
        }
    }

    private void SnapMultipleGameObjects(IEnumerable<GameObject> selectedGameObjects)
    {
        foreach (GameObject gameObject in selectedGameObjects)
        {
            SnapGameObject(gameObject);
        }
    }

    private void SnapGameObject(GameObject gameObject)
    {
        Undo.RecordObject(gameObject.transform, UNDO_SNAP_COMMAND);
        float snappedX = gameObject.transform.position.x;
        snappedX = Mathf.Round((snappedX / snapUnit)) * snapUnit;
        float snappedY = gameObject.transform.position.y;
        snappedY = Mathf.Round((snappedY / snapUnit)) * snapUnit;
        float snappedZ = gameObject.transform.position.z;
        snappedZ = Mathf.Round((snappedZ / snapUnit)) * snapUnit;
        gameObject.transform.localPosition = new Vector3(snappedX, snappedY, snappedZ);
    }


}
