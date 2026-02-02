using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AbstractPixel.Utility.Save
{
    public class SaveSystemDebugger : EditorWindow
    {
        // The Data
        [SerializeField] private SaveSystemConfigSO config;

        // The Offline Tools (Your actual game logic, reused!)
        private SaveProfileManager offlineProfileManager;
        private IDataStorageService storageService;
        private ISerializer serializer;
        bool isInitialized;

        [MenuItem("Tools/Save Debug Dashboard")]
        public static void ShowWindow()
        {
            SaveSystemDebugger wnd = GetWindow<SaveSystemDebugger>();
            wnd.titleContent = new GUIContent("Save Dashboard");
            wnd.minSize = new Vector2(300, 400);
        }

        private void OnValidate()
        {
            if(config != null)
            {
                storageService = new FileDataStorageService();
                serializer = new JsonSerializer();
                SavePathGenerator.Initialize(config);
                offlineProfileManager = new SaveProfileManager(storageService, config, serializer);
            }
        }

        private void OnDisable()
        {
            
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            Label title = new Label("Save Debug Dashboard")
            {
                style = {fontSize =15,unityFontStyleAndWeight = FontStyle.Bold, marginTop=10,marginLeft =10},
            };
            root.Add(title);

            ObjectField configField = new ObjectField("Save Config")
            {
                objectType = typeof(SaveSystemConfigSO),
                value = config,
                style = {marginTop =10,marginLeft =10},
                

            };
            root.Add(configField);
                 
        }

        // This mimics SaveManager.Awake(), but for the Editor Window
        private void InitializeOfflineBackend()
        {

           
        }

        private bool ValidateConfig()
        {
            return true;
        }
    }
}
