using AbstractPixel.GameManager.Configuration;

namespace AbstractPixel.GameManager.GameBehaviors
{
    [System.Serializable]
    public class StartBehavior : GameBehaviorBase
    {
        public StartBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO) : base(_behaviorConfigSO)
        {
            EventType = GameStateEvent.OnStart;
        }
    }
}