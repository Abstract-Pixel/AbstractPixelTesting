using Game_Manager;
using UnityEngine;

public class GameManagerTesting : MonoBehaviour
{
    [SerializeField] private SceneReference sceneReference;
    private void Start()
    {
        GameManager.Instance.BehaviorSceneContext.SetContext<GameBehaviorBase>(sceneReference);

    }
}
