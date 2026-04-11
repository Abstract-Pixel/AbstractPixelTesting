using AbstractPixel.GameManager.Conditions;
using UnityEngine;
using AbstractPixel.GameManager.GameBehaviors;
using UnityEngine.InputSystem;

namespace AbstractPixel.GameManager.Configuration
{
    [CreateAssetMenu(fileName = "PauseBehaviorConfig", menuName = "Game Manager/Behavior Configs/Pause Behavior Config", order = 3)]
    public class PauseBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        [Header("Pause Behavior Settings")]
        [SerializeField] public InputActionAsset InputMapActionAsset;
        [SerializeField] public string InputActionName;
        public override string BehaviorName => "Pause Behavior";

        public override GameBehaviorBase CreateBehavior()
        {
            PauseBehavior pauseBehavior = new PauseBehavior(this);
            return pauseBehavior;
        }

        public override GameCondition CreateGameCondition()
        {
            PauseCondition pauseCondition = new PauseCondition(this);
            return pauseCondition;
        }
    }
}