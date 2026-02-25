using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbstractPixel.Utility.Save
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            VisualElement debugContainer = new VisualElement();

            Foldout debugFoldOut = new Foldout { text = "Debug Controls" };
            VisualElement separatorFoldOut = new VisualElement();
            separatorFoldOut.style.height = 1;

            debugContainer.Add(separatorFoldOut);
            debugContainer.Add(debugFoldOut);

            Label editorControlsLabel = new Label("Editor Controls");
            Button deleteAllSavesButton = new Button();
            deleteAllSavesButton.text = "Delete All Saves";
            Button openReleaseSaveDirectory = new Button();
            openReleaseSaveDirectory.text = "Open Save Directory";
            Button openDebugSaveDirectory = new Button();
            openDebugSaveDirectory.text = "Open Debug Save Directory";
            VisualElement editorControlsSeparator = new VisualElement();
            editorControlsSeparator.style.height = 1;

            debugFoldOut.Add(editorControlsLabel);
            debugFoldOut.Add(deleteAllSavesButton);
            debugFoldOut.Add(openReleaseSaveDirectory);
            debugFoldOut.Add(openDebugSaveDirectory);
            debugFoldOut.Add(editorControlsSeparator);

            Label runtimeControlsLabel = new Label("Runtime Controls");
            Button saveRuntimeDataButton = new Button();
            saveRuntimeDataButton.text = "SAVE ALL DATA !";
            Button loadRuntimeDataButton = new Button();
            loadRuntimeDataButton.text = "LOAD ALL DATA !";
            VisualElement runtimeControlsSeparator = new VisualElement();
            runtimeControlsSeparator.style.height = 1;

            debugFoldOut.Add(runtimeControlsLabel);
            debugFoldOut.Add(saveRuntimeDataButton);
            debugFoldOut.Add(loadRuntimeDataButton);
            debugFoldOut.Add(runtimeControlsSeparator);

            root.Add(debugContainer);
            return root;
        }
    
    }
}
