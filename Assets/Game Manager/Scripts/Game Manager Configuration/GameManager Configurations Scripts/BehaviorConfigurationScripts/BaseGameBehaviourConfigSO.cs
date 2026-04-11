using AbstractPixel.GameManager;
using AbstractPixel.GameManager.Conditions;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractPixel.GameManager.Configuration
{
    public abstract class BaseGameBehaviorConfigSO : ScriptableObject
    {
        [Header("Base Behavior Configuration")]
        [Tooltip("If true, the game time will be set to zero upon execution.")]
        public bool IsTimeZeroOnExecution = true;
        [Tooltip("If true, the cursor will be locked upon execution.")]
        public bool IsCursorLockedOnExecution = true;
        [Tooltip("If true, the cursor will be visible upon execution.")]
        public bool IsCursorVisibleOnExecution = true;
        [Tooltip("If true, the game UI will be shown upon execution.")]
        public bool ShowGameUIOnExecution = false;

        [Tooltip("Specifies the type of scene load behavior to perform upon execution.")]
        public SceneLoadType SceneLoadTypeOnExecution = SceneLoadType.NoSceneLoad;


        [Tooltip("The default scene to load when scene provided to load is not valid or failed")]
        public SceneReference DefaultScene;
        [Tooltip("List of valid scenes for this behavior. This is used to determine which scenes can be loaded when this behavior is executed.")]
        public List<SceneReference> ValidScenes;

        public abstract string BehaviorName { get; }
        public abstract GameBehaviorBase CreateBehavior();
        public abstract GameCondition CreateGameCondition();

        public bool IsSceneValid(SceneReference sceneReference)
        {
            if (sceneReference == null)
            {
                return false;
            }
            foreach (SceneReference validScene in ValidScenes)
            {
                if (validScene.SceneName == sceneReference.SceneName)
                {
                    return true;
                }
            }
            Debug.LogWarning($"Scene '{sceneReference.SceneName}' is not valid for behavior '{BehaviorName}'.");
            return false;
        }

        public bool IsSceneValid(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is null or empty.");
                return false;
            }
            foreach (SceneReference validScene in ValidScenes)
            {
                if (validScene.SceneName == sceneName)
                {
                    return true;
                }
            }
            //Debug.LogWarning($"Scene '{sceneName}' is not valid for behavior '{BehaviorName}'.");
            return false;

        }
    }
}
