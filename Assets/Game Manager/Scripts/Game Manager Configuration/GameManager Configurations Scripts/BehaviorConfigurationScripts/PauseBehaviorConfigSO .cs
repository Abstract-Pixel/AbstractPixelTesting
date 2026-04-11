using Game_Manager.Conditions;
using UnityEngine;
using Game_Manager.GameBehaviors;
using UnityEngine.InputSystem;

namespace Game_Manager.Configuration
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