using UnityEngine;
using System;
using AbstractPixel.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneField
{
    [SerializeField] private string sceneName = string.Empty;
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset = null;
#endif

    public string SceneName => sceneName;
}
