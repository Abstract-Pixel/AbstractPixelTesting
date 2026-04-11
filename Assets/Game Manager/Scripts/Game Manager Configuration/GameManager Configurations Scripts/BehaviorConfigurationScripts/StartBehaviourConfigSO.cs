using AbstractPixel.GameManager;
using AbstractPixel.GameManager.Conditions;
using UnityEngine;
using AbstractPixel.GameManager.GameBehaviors;

namespace AbstractPixel.GameManager.Configuration
{

    [CreateAssetMenu(fileName = "StartBehaviourConfigSO", menuName = "Game Manager/Behavior Configs/Start Behavior Config", order = 2)]
    public class StartBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Start Behavior";
        public override GameBehaviorBase CreateBehavior()
        {
            StartBehavior startBehavior = new StartBehavior(this);
            return startBehavior;
        }

        public override GameCondition CreateGameCondition()
        {
            // Start Behaviour doe not have and doe snot need a game condtion
            // So this is why it not created here and is null
            return null;
        }
    }
}