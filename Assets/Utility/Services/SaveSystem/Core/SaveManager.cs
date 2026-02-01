using System.Collections.Generic;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveManager : MonoSingleton<SaveManager>
    {
        [SerializeField] private SaveSystemConfigSO saveConfig;
        private SaveProfileManager profileManager;
        private Dictionary<SaveCategory, Dictionary<string, ISaveableBridge>> SaveableObjectsRegistry;
        private IDataStorageService fileStorageService;
        private ISerializer serializer;

        // Remove Later
        string tempProfileID = "TEST_101";

        protected override void Awake()
        {
            base.Awake();
            saveConfig.Initialize();
            SavePathGenerator.Initialize(saveConfig);

            fileStorageService = new FileDataStorageService();
            serializer = new JsonSerializer();

            SaveableObjectsRegistry = new Dictionary<SaveCategory, Dictionary<string, ISaveableBridge>>();
            profileManager = new SaveProfileManager(fileStorageService, saveConfig, serializer);

            // For Testing purposes, replace and remove with actual profile loading and managment
            string profilePath = profileManager.CreateCustomProfileDirectory(tempProfileID);
            profileManager.SetCurrentActiveProfile(tempProfileID, profilePath);
        }

        public void SaveALL()
        {
            foreach (SaveCategory category in SaveableObjectsRegistry.Keys)
            {
                SaveDataOf(category);
            }
        }

        public void SaveDataOf(SaveCategory _category)
        {
            if (!SaveableObjectsRegistry.TryGetValue(_category, out var bridgesDataMap))
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
            foreach (ISaveableBridge bridge in bridgesDataMap.Values)
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
            // This ensures we try to load every file type defined in your game.
            foreach (SaveCatgeoryDefinition definition in saveConfig.GetAllCategoryDefintions())
            {
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
            if (!SaveableObjectsRegistry.TryGetValue(_category, out Dictionary<string, ISaveableBridge> bridgesDataMap))
            {
                // LOGIC DECISION:
                // This is where "Spawning Logic" would go later.
                return;
            }

            foreach (KeyValuePair<string, object> kvp in loadedData.DataMap)
            {
                string guid = kvp.Key;
                object objectData = kvp.Value;

                if (bridgesDataMap.TryGetValue(guid, out ISaveableBridge bridge))
                {
                    bridge.RestoreState(objectData, _category);
                }
            }
        }

        public void RegisterSaveableObject(ISaveableBridge bridge, List<SaveCategory> categories)
        {
            if (IsInstanceNull()) return;
            if (bridge == null || categories == null) return;

            foreach (SaveCategory category in categories)
            {
                // 1. Ensure the Bucket (Inner Dictionary) exists for this Category
                if (!SaveableObjectsRegistry.ContainsKey(category))
                {
                    SaveableObjectsRegistry.Add(category, new Dictionary<string, ISaveableBridge>());
                }

                Dictionary<string, ISaveableBridge> categoryBucket = SaveableObjectsRegistry[category];

                // 3. APPEND the Bridge (Safe Check)
                if (!categoryBucket.ContainsKey(bridge.UniqueId))
                {
                    categoryBucket.Add(bridge.UniqueId, bridge);
                }
                else
                {
                    // Optional: Update the reference if it somehow got recreated
                    categoryBucket[bridge.UniqueId] = bridge;
                }
            }
        }

        public void UnregisterSaveableObject(ISaveableBridge bridge, List<SaveCategory> categories)
        {
            if (IsInstanceNull()) return;
            if (bridge == null || categories == null) return;

            foreach (SaveCategory category in categories)
            {
                if (SaveableObjectsRegistry.TryGetValue(category, out var categoryBucket))
                {
                    if (categoryBucket.ContainsKey(bridge.UniqueId))
                    {
                        categoryBucket.Remove(bridge.UniqueId);
                    }
                }
            }
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

    }
}
