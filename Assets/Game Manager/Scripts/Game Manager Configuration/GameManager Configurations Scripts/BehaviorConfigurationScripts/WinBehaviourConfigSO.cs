using AbstractPixel.GameManager;
using AbstractPixel.GameManager.Conditions;
using UnityEngine;

namespace AbstractPixel.GameManager.Configuration
{
    [CreateAssetMenu(fileName = "WinBehaviorConfigSO", menuName = "Game Manager/Behavior Configs/Win Behavior Config", order = 5)]
    public class WinBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Win Behavior";

        public override GameBehaviorBase CreateBehavior()
        {
            WinBehavior winBehavior = new WinBehavior(this);
            return winBehavior;

        }

        public override GameCondition CreateGameCondition()
        {
            WinCondition winCondition = new WinCondition(this);
            return winCondition;
        }
    }
}