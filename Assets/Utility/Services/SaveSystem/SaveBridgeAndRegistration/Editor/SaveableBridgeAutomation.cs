#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    [InitializeOnLoad]
    public static class SaveableBridgeAutomation
    {
        static SaveableBridgeAutomation()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                AddNessecerySaveableBridges();
            }

        }

        static void AddNessecerySaveableBridges()
        {
            MonoBehaviour[] activeScriptsInScene = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (MonoBehaviour script in activeScriptsInScene)
            {
                if (script.GetType().GetCustomAttribute<SaveableAttribute>() == null)
                {
                    continue;
                }
                if (script.TryGetComponent(out ISaveableBridge bridge))
                {
                    // It already contains a SaveableBridge
                    continue;
                }
                script.gameObject.AddComponent<SaveableBridge>();
                Debug.Log($"Added Bridge to{script.gameObject.name}");

            }
        }
    }
}
#endif
