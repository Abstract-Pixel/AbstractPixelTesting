using AbstractPixel.GameManager.Configuration;

namespace AbstractPixel.GameManager.Conditions
{
    /// <summary>
    /// References cannot be dragged in inspector 
    /// if the Game Manager is marked as persistent
    /// If The Game Manager persists across scenes, then the references
    /// Will be lost and be null in the process
    /// Instead try getting the references 
    /// </summary>
    [System.Serializable]
    public class WinCondition : GameCondition
    {
        public WinCondition(BaseGameBehaviorConfigSO _configSO) : base(_configSO)
        {
            conditionName = "Win Condition";
            configSO = _configSO;
            requestEventType = GameRequestEvent.RequestWinGame;
        }

        public override void Initialize()
        {

        }
        protected override void HandleOnGameConditionMet()
        {
        }

        public override void CleanUp()
        {

        }
    }
}

