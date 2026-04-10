
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        SceneAsset sceneAsset = GetSceneAsset(sceneAssetProperty);
        SerializedProperty sceneNameProperty = property.FindPropertyRelative("sceneName");
        string sceneNameRef = sceneNameProperty.stringValue;
        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sceneAssetProperty, label);
            if (EditorGUI.EndChangeCheck())
            {
                // 1. First, apply the change to the serialization system to make it official.
                sceneAssetProperty.serializedObject.ApplyModifiedProperties();

                // 2. NOW, re-fetch the asset to get the NEW value.
                SceneAsset newSceneAsset = GetSceneAsset(sceneAssetProperty);

                // 3. Run your logic using the NEW asset.
                if (newSceneAsset == null)
                {
                    sceneNameProperty.stringValue = string.Empty;
                }
                else
                {
                    sceneNameProperty.stringValue = newSceneAsset.name;
                }

                // 4. It's good practice to apply this final modification as well.
                sceneNameProperty.serializedObject.ApplyModifiedProperties();
            }

           
            EditorGUILayout.PropertyField(sceneNameProperty);


            using (new EditorGUILayout.VerticalScope("HelpBox"))
            {
                // add spacing
                GUILayout.Space(5);
                using (new EditorGUILayout.HorizontalScope())
                {
                    Rect boxRect = GUILayoutUtility.GetRect(16, 16, GUILayout.ExpandWidth(false));

                    if (IsSceneValidForBuild(sceneAsset))
                    {
                        EditorGUI.DrawRect(boxRect, new Color(0, 1, 1, 0.5f));
                        EditorGUILayout.LabelField("Scene is included in build settings", EditorStyles.whiteLabel);
                    }
                    else
                    {
                        EditorGUI.DrawRect(boxRect, new Color(1, 0, 0, 0.5f));
                        EditorGUILayout.LabelField("Scene is not included in build settings", EditorStyles.whiteLabel);
                    }
                }
                GUILayout.Space(6);

            }
        }
    }


    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return base.CreatePropertyGUI(property);
    }


    SceneAsset GetSceneAsset(SerializedProperty property)
    {
        SceneAsset sceneReference = property.objectReferenceValue as SceneAsset;
        return sceneReference;
    }

    bool IsSceneValidForBuild(SceneAsset sceneAsset)
    {
        if (sceneAsset == null) return false;

        string sceneTargetPath = AssetDatabase.GetAssetPath(sceneAsset);
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (EditorBuildSettings.scenes[i].path == sceneTargetPath)
            {
                return true;
            }
        }
        return false;
    }
}
