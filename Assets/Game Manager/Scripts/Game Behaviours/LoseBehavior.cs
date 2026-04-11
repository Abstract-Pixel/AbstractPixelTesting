using UnityEngine;
using AbstractPixel.GameManager.Configuration;

namespace AbstractPixel.GameManager.GameBehaviors
{
    [System.Serializable]
    public class LoseBehavior : GameBehaviorBase
    {
        public LoseBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO) : base(_behaviorConfigSO)
        {
            EventType = GameStateEvent.OnLose;
        }
    }
}