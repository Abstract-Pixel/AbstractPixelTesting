using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AbstractPixel.Utility.Save
{
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        [SerializeField] private SaveSystemConfigSO saveConfig;
        private SaveProfileManager profileManager;
        private Dictionary<SaveCategory, Dictionary<string, ISavableBridge>> savableObjectsRegistry;
        private IDataStorageService fileStorageService;
        private ISerializer serializer;

        readonly string stringSeparatorIdentifier = "#";


        protected override void Awake()
        {
            base.Awake();
            saveConfig.Initialize();
            SavePathGenerator.Initialize(saveConfig);

            fileStorageService = new FileDataStorageService();
            serializer = new JsonSerializer();

            savableObjectsRegistry = new Dictionary<SaveCategory, Dictionary<string, ISavableBridge>>();
            profileManager = new SaveProfileManager(fileStorageService, saveConfig, serializer);

            LoadSystemMetaData(out SystemMetaData metaData);
            LoadAllDataByScope(SaveScope.Global);
            ExecuteProfileStartUpPolicy(metaData);
        }

        void ExecuteProfileStartUpPolicy(SystemMetaData _metaData)
        {
            if (saveConfig.StartupPolicy == SaveStartupPolicy.AutoSetUp)
            {
                if (_metaData == null || string.IsNullOrEmpty(_metaData.LastSavedProfileID))
                {
                    string newProfilePath = profileManager.CreateNewActiveProfileDirectory();
                    SaveSystemMetaData();
                }
                else if (!profileManager.ProfileDirectoryExists(_metaData.LastSavedProfileID))
                {
                    string newProfilePath = profileManager.CreateNewActiveProfileDirectory();
                    SaveSystemMetaData();
                }
                else
                {
                    profileManager.SetCurrentActiveProfile(_metaData.LastSavedProfileID);
                }
            }
            else
            {
                // Manual or undefined policy, do nothing. Wait for explicit profile selection from external UI or other system.
            }
        }

        public void SaveALL()
        {
            foreach (SaveCategory category in savableObjectsRegistry.Keys)
            {
                SaveDataOf(category);
            }
        }

        public void SaveDataOf(SaveCategory _category)
        {
            if (!savableObjectsRegistry.TryGetValue(_category, out var bridgesDataMap))
            {
                // No objects registered for this category. Nothing to save.
                return;
            }
            if (bridgesDataMap.Count == 0) return;

            string activeProfileId = profileManager.CurrentProfileID;
            SaveCatgeoryDefinition categoryDefinition = saveConfig.GetCategoryDefinition(_category);

            string fileName = SavePathGenerator.GetFileNameBasedOnCategoryDefinition(categoryDefinition);
            string fullSavePath = SavePathGenerator.GetPath(categoryDefinition, activeProfileId);

            SaveFileData categorizedSaveFileData;
            if (categoryDefinition.DirectoryScope == SaveScope.Global)
            {
                categorizedSaveFileData = new SaveFileData(fileName);
            }
            else
            {
                categorizedSaveFileData = new SaveFileData(fileName, activeProfileId);
            }

            bool hasData = false;
            foreach (ISavableBridge bridge in bridgesDataMap.Values)
            {
                if (bridge == null) continue;

                object capturedData = bridge.CaptureState(_category);
                if (capturedData != null)
                {
                    categorizedSaveFileData.DataMap.Add(bridge.UniqueId, capturedData);
                    hasData = true;
                }
            }

            if (hasData)
            {
                if (serializer.TrySerialize(categorizedSaveFileData, out string json))
                {
                    fileStorageService.SaveFile(json, fullSavePath);
                }
            }
        }

        public void LoadALL()
        {
            foreach (SaveCatgeoryDefinition definition in saveConfig.GetAllCategoryDefinitions())
            {
                LoadDataOf(definition.Category);
            }
        }

        public void LoadAllDataByScope(SaveScope _directoryScope)
        {
            foreach (SaveCatgeoryDefinition definition in saveConfig.GetAllCategoryDefinitions())
            {
                if (_directoryScope != definition.DirectoryScope) continue;
                LoadDataOf(definition.Category);
            }
        }

        public void LoadDataOf(SaveCategory _category)
        {
            string profileId = profileManager.CurrentProfileID;
            SaveCatgeoryDefinition definition = saveConfig.GetCategoryDefinition(_category);

            string loadPath = SavePathGenerator.GetPath(definition, profileId);
            if (!fileStorageService.FileExists(loadPath)) return;

            string loadedjson = fileStorageService.LoadFile(loadPath);
            if (string.IsNullOrEmpty(loadedjson)) return;

            if (!serializer.TryDeserialize(loadedjson, out SaveFileData loadedData))
            {
                Debug.LogError($"[SaveManager] Corrupted file found at {loadPath}");
                return;
            }

            // Check if we have receivers (Bridges)
            if (!savableObjectsRegistry.TryGetValue(_category, out Dictionary<string, ISavableBridge> bridgesDataMap))
            {
                // LOGIC DECISION:
                // This is where "Spawning Logic" would go later.
                return;
            }

            foreach (KeyValuePair<string, object> kvp in loadedData.DataMap)
            {
                string compositeKey = kvp.Key;
                object objectData = kvp.Value;

                int separatorIndex = compositeKey.LastIndexOf(stringSeparatorIdentifier) + 1;
                string extractedGUID = compositeKey.Substring(separatorIndex);

                if (bridgesDataMap.TryGetValue(extractedGUID, out ISavableBridge bridge))
                {
                    bridge.RestoreState(objectData, _category);
                }
            }
        }

        public void RegisterSavableObject(ISavableBridge bridge, List<SaveCategory> categories)
        {
            if (IsInstanceNull()) return;
            if (bridge == null || categories == null) return;

            // Extract GUID from the Bridge's Composite UniqueID "Name#GUID"
            // If no separator, assume ID is the GUID (Legacy support)
            int separatorIndex = bridge.UniqueId.LastIndexOf(stringSeparatorIdentifier) + 1;
            string extractedGuid = separatorIndex > 0 ? bridge.UniqueId.Substring(separatorIndex) : bridge.UniqueId;

            foreach (SaveCategory category in categories)
            {
                if (!savableObjectsRegistry.ContainsKey(category))
                {
                    savableObjectsRegistry.Add(category, new Dictionary<string, ISavableBridge>());
                }

                Dictionary<string, ISavableBridge> categoryBucket = savableObjectsRegistry[category];

                // 3. APPEND the Bridge using EXTRACTED GUID as key
                if (!categoryBucket.ContainsKey(extractedGuid))
                {
                    categoryBucket.Add(extractedGuid, bridge);
                }
                else
                {
                    categoryBucket[extractedGuid] = bridge;
                }
            }
        }

        public void UnregisterSavableObject(ISavableBridge bridge, List<SaveCategory> categories)
        {
            if (IsInstanceNull()) return;
            if (bridge == null || categories == null) return;

            int separatorIndex = bridge.UniqueId.LastIndexOf(stringSeparatorIdentifier) + 1;
            string extractedGuid = separatorIndex > 0 ? bridge.UniqueId.Substring(separatorIndex) : bridge.UniqueId;

            foreach (SaveCategory category in categories)
            {
                if (savableObjectsRegistry.TryGetValue(category, out var categoryBucket))
                {
                    if (categoryBucket.ContainsKey(extractedGuid))
                    {
                        categoryBucket.Remove(extractedGuid);
                    }
                }
            }
        }

        private void SaveSystemMetaData()
        {
            string metaDataFilePath = SavePathGenerator.GetSaveSystemMetaDataPath();
            SystemMetaData metaData = new SystemMetaData(profileManager.CurrentProfileID);
            if (serializer.TrySerialize(metaData, out string json))
            {
                fileStorageService.SaveFile(json, metaDataFilePath);
            }
            fileStorageService.SaveFile(json, metaDataFilePath);
        }

        private bool LoadSystemMetaData(out SystemMetaData metaData)
        {
            string metaDataFilePath = SavePathGenerator.GetSaveSystemMetaDataPath();
            metaData = null;
            if (!fileStorageService.FileExists(metaDataFilePath)) return false;
            string loadedJson = fileStorageService.LoadFile(metaDataFilePath);
            if (string.IsNullOrEmpty(loadedJson)) return false;
            if (!serializer.TryDeserialize(loadedJson, out metaData))
            {
                Debug.LogError($"[SaveManager] Corrupted System MetaData file found at {metaDataFilePath}");
                return false;
            }
            return true;
        }

        private bool IsInstanceNull()
        {
            if (instance == null)
            {
                Debug.LogError("SaveManager instance is null. Ensure SaveManager is initialized before use and present in the scene ");
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (saveConfig.IsSceneIgnored(scene.name)) { return; }
            StartCoroutine(RestoreDataRoutine());
        }

        IEnumerator RestoreDataRoutine()
        {
            // Make sure everything in the scene is initialized before we try to restore data to objects.
            // May add an extra wait if needed, but this should be sufficient for most cases.
            yield return new WaitForEndOfFrame();
            LoadAllDataByScope(SaveScope.GameProfile);
            //SaveDataOf(SaveCategory.Game);
        }
    }
}