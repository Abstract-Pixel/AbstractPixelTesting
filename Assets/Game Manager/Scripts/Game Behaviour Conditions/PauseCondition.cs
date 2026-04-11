using AbstractPixel.GameManager.Configuration;
using AbstractPixel.GameManager.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AbstractPixel.GameManager.Conditions
{
    /// <summary>
    /// References cannot be dragged in inspector 
    /// if the Game Manager is marked as persistant
    /// If The Game Manager persists across scenes, then the references
    /// Will be lost and be null in the process
    /// Instead try getting the references 
    /// </summary>
    [System.Serializable]
    public class PauseCondition : GameCondition
    {
        [HideInInspector][SerializeField] PauseBehaviorConfigSO pauseConfig;
        [SerializeField][HideInInspector] bool isPaused = false;
        InputAction pauseAction;

        public PauseCondition(BaseGameBehaviorConfigSO _pauseConfig) : base(_pauseConfig)
        {
            conditionName = "Pause Condition";
            pauseConfig = (PauseBehaviorConfigSO)_pauseConfig;
            requestEventType = GameRequestEvent.RequestPauseGame;
        }

        public override void Initialize()
        {
            GameManagerEventBus.Subscribe(GameStateEvent.OnPaused, EnablePause);
            GameManagerEventBus.Subscribe(GameStateEvent.OnUnPaused, DisablePause);
            pauseAction = pauseConfig.InputMapActionAsset.FindAction(pauseConfig.InputActionName);
            if (pauseAction == null)
            {
                Debug.Log("[PauseCondition] pauseInputAction is null");
                return;
            }

            pauseAction.Enable();
            pauseAction.performed += OnPausePerformed;

        }

        void OnPausePerformed(InputAction.CallbackContext context)
        {
            HandleOnGameConditionMet();
        }
        protected override void HandleOnGameConditionMet()
        {
            isPaused = !isPaused;
            requestEventType = isPaused ? GameRequestEvent.RequestPauseGame : GameRequestEvent.RequestUnPauseGame;
            GameManagerEventBus.Raise(requestEventType);
        }

        void DisablePause() => isPaused = false;
        void EnablePause() => isPaused = true;

        public override void CleanUp()
        {
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPaused, EnablePause);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnUnPaused, DisablePause);
            if (pauseAction != null)
            {
                pauseAction.performed -= OnPausePerformed;
                pauseAction.Disable();
            }
        }
    }
}
