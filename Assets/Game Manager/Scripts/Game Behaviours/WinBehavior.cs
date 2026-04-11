using AbstractPixel.GameManager.Configuration;

namespace AbstractPixel.GameManager.Conditions
{
    [System.Serializable]
    public class WinBehavior : GameBehaviorBase
    {
        public WinBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO) : base( _behaviorConfigSO)
        {
            EventType = GameStateEvent.OnWin;
        }
    }
}