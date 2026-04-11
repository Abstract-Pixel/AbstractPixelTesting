using AbstractPixel.GameManager;
using AbstractPixel.GameManager.Conditions;
using UnityEngine;
using AbstractPixel.GameManager.GameBehaviors;

namespace AbstractPixel.GameManager.Configuration
{
    [CreateAssetMenu(fileName = "LoseBehaviourConfigSO", menuName = "Game Manager/Behavior Configs/Lose Behavior Config", order = 6)]
    public class LoseBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Lose Behavior";

        public override GameBehaviorBase CreateBehavior()
        {
            LoseBehavior loseBehavior = new LoseBehavior(this);
            return loseBehavior;
        }

        public override GameCondition CreateGameCondition()
        {
            LoseCondition loseCondition = new LoseCondition(this);
            return loseCondition;
        }
    }
}