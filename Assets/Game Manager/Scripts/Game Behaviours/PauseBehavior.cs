using UnityEngine;
using AbstractPixel.GameManager.Configuration;
using AbstractPixel.GameManager.Events;
using AbstractPixel.GameManager.Conditions;

namespace AbstractPixel.GameManager.GameBehaviors
{
    [System.Serializable]
    public class PauseBehavior : GameBehaviorBase
    {
        [HideInInspector][SerializeField] private PauseBehaviorConfigSO config;
        [HideInInspector][SerializeField] private PauseCondition pauseCondition;

        public PauseBehavior( BaseGameBehaviorConfigSO _behaviourConfigSO) : base( _behaviourConfigSO)
        {
            config = _behaviourConfigSO as PauseBehaviorConfigSO;
            EventType = GameStateEvent.OnPaused;
        }

        public override void Exit()
        {
            GameManagerEventBus.Raise(GameStateEvent.OnUnPaused);
        }
    }
} 