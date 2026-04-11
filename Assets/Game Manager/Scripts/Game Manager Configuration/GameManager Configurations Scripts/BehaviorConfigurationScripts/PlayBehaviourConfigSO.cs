using AbstractPixel.GameManager.Conditions;
using UnityEngine;
using AbstractPixel.GameManager.GameBehaviors;

namespace AbstractPixel.GameManager.Configuration
{
    [CreateAssetMenu(fileName = "PlayBehaviorConfig", menuName = "Game Manager/Behavior Configs/Play Behavior Config", order = 4)]
    public class PlayBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Play Behavior";
        public override GameBehaviorBase CreateBehavior()
        {
            PlayBehavior playBehavior = new PlayBehavior(this);
            return playBehavior;

        }

        public override GameCondition CreateGameCondition()
        {
            // Play behavior does not have and does not need a specific game condition, so we return null.
            return null;
        }
    }
}
