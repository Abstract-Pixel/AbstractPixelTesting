using UnityEngine;
using UnityEngine.SceneManagement;
using AbstractPixel.GameManager.Events;
using AbstractPixel.GameManager.Configuration;
using AbstractPixel.Core;


namespace AbstractPixel.GameManager
{
    [System.Serializable]
    public abstract class GameBehaviorBase : IGameBehavior
    {
        [SerializeField][HideInInspector] string behaviorName;
        [SerializeField] protected BaseGameBehaviorConfigSO behaviorConfigSO;

        [HideInInspector][SerializeField] protected bool isInitialEnter = true;
        [field: SerializeField][HideInInspector] public GameStateEvent EventType { get; protected set; }
        [field: SerializeField][HideInInspector] public GameStateEvent InGameUIEventType { get; protected set; }
        public BaseGameBehaviorConfigSO BehaviorConfigSO => behaviorConfigSO;

        public GameBehaviorBase(BaseGameBehaviorConfigSO _behaviorConfigSO)
        {
            behaviorConfigSO = _behaviorConfigSO;
            behaviorName = _behaviorConfigSO.BehaviorName;
            isInitialEnter = true;
        }

        public void Enter(SceneReference sceneReference)
        {
            OnEnter();
            ApplyBaseSettings(sceneReference);
        }

        /// <summary>
        /// This is intended to be called every frame when the behavior is active
        /// This needs to be overridden in the derived class if custom logic needs 
        /// to run every frame
        /// </summary>
        public virtual void OnUpdate() { }
        public virtual void Exit() { }

        public void Reset()
        {
            isInitialEnter = true;
        }
        /// <summary>
        ///  If The behavior needs to have custom logic when
        ///  entering the state,then it needs to be overridden in the derived class
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// if the behavior needs to have custom logic when
        /// resetting the state, then it needs to be overridden in the derived class
        protected virtual void OnReset() { }

        private void ApplyBaseSettings(SceneReference sceneReference)
        {
            if (behaviorConfigSO == null)
            {
                Debug.LogError("Config So is not assigned to this Game Manager Behavior.Please assign it in the Inspector.");
            }
            //Debug.Log("Executing " + GetType().ToString());
            SetTimescale(behaviorConfigSO.IsTimeZeroOnExecution ? 0f : 1f);
            SetCursorLockMode(behaviorConfigSO.IsCursorLockedOnExecution);
            SetCursorVisible(behaviorConfigSO.IsCursorVisibleOnExecution);
            SetInGameUiEventType();
            HandleSceneLoading(behaviorConfigSO.SceneLoadTypeOnExecution,sceneReference);
            isInitialEnter = false;
            GameManagerEventBus.Raise(EventType);
            GameManagerEventBus.Raise(InGameUIEventType);
        }

        private void SetInGameUiEventType()
        {
            if (behaviorConfigSO.ShowGameUIOnExecution)
            {
                InGameUIEventType = GameStateEvent.OnInGameUIActive;
            }
            else
            {
                InGameUIEventType = GameStateEvent.OnInGameUIInactive;
            }
        }

        protected virtual void SetTimescale(float customScale)
        {
            Time.timeScale = customScale;
        }

        protected virtual void SetCursorLockMode(bool enabled)
        {
            if (enabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        protected virtual void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
        }

        protected void HandleSceneLoading(SceneLoadType loadType, SceneReference sceneReference)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            bool isSceneLoadingNotAllowed = loadType == SceneLoadType.NoSceneLoad;
            if (isSceneLoadingNotAllowed)
            {
                return;
            }
            bool isCurrentActiveSceneValid = behaviorConfigSO.IsSceneValid(currentScene);
            if (sceneReference == null || sceneReference.SceneAssetReference == null || sceneReference.SceneFieldReference == null)
            {                
                if(!isCurrentActiveSceneValid)
                {
                    LoadScene(behaviorConfigSO.DefaultScene.SceneName);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (!behaviorConfigSO.IsSceneValid(sceneReference))
            {
                LoadScene(behaviorConfigSO.DefaultScene.SceneName);
                return;
            }
            switch (loadType)
            {
                case SceneLoadType.NoSceneLoad:
                    break;
                case SceneLoadType.LoadSceneOnce:
                    if (!isInitialEnter) break;
                    LoadScene(behaviorConfigSO.DefaultScene.SceneName);
                    break;
                case SceneLoadType.LoadSceneAlways:
                    LoadScene(sceneReference.SceneName);
                    break;
            }
        }

        void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is null or empty. Please check the scene reference for this behavior.");
                return;
            }
            if (sceneName == SceneManager.GetActiveScene().name)
            {
                Debug.LogWarning("Trying to load the same scene again. Please check the scene name.");
                return;
            }
            SceneManager.LoadScene(sceneName);
        }
    }
}