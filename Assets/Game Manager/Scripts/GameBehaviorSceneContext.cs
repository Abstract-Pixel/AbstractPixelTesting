using UnityEngine;
using System;
using Game_Manager;

public class GameBehaviorSceneContext
{
    public Type TargetBehaviorType { get; private set; }
    public SceneReference TargetSceneReference { get; private set; }

    // This forces the "Validation" mindset
    public void SetContext<BehaviorType>(SceneReference scene) where BehaviorType : GameBehaviorBase
    {
        TargetBehaviorType = typeof(BehaviorType);
        TargetSceneReference = scene;
    }

    public void ClearContext()
    {
        TargetBehaviorType = null;
        TargetSceneReference = null;
    }

    public bool IsContextNull()
    {
        return TargetBehaviorType == null && TargetSceneReference == null;
    }
}
