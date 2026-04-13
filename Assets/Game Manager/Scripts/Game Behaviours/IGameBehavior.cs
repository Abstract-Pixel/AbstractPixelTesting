using AbstractPixel.Core;
using UnityEngine;

public interface IGameBehavior
{
    public void Enter(SceneReference _sceneReference);

    public virtual void OnUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}