using AbstractPixel.Core;
using AbstractPixel.GameManager;
using AbstractPixel.GameManager.GameBehaviors;
using UnityEngine;

public class GameManagerTesting : MonoBehaviour
{
    [SerializeField] private SceneReference sceneReference;
    private void Start()
    {
        GameManager.Instance.BehaviorSceneContext.SetContext<PlayBehavior>(sceneReference);

    }
}
