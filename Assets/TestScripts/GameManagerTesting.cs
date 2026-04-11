using Game_Manager;
using Game_Manager.GameBehaviors;
using UnityEngine;

public class GameManagerTesting : MonoBehaviour
{
    [SerializeField] private SceneReference sceneReference;
    private void Start()
    {
        GameManager.Instance.BehaviorSceneContext.SetContext<PlayBehavior>(sceneReference);

    }
}
