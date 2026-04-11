using AbstractPixel.GameManager.UI;
using UnityEngine;
using AbstractPixel.GameManager.Configuration;

namespace AbstractPixel.GameManager.GameBehaviors
{
    [System.Serializable]
    public class PlayBehavior : GameBehaviorBase
    {
        public PlayBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO): base(_behaviorConfigSO)
        {
            EventType = GameStateEvent.OnPlay;
        }
    }
}