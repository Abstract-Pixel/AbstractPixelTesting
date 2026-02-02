using AbstractPixel.Utility.Save;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveableBridge : MonoBehaviour, ISaveableBridge
    {
        [field: SerializeField] public string UniqueId { get; private set; }
        private Dictionary<SaveCategory, List<SaveableTarget>> saveableTargetsRegistry;
        [SerializeField] private List<SaveCategory> foundCategoriesList;
        readonly string isaveableCaptureMethod = "CaptureData";
        readonly string isaveableRestoreMethod = "RestoreData";

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(UniqueId))
            {
                // LATER : Want to see if you can make a complete distinct id wihout ever making duplicates
                // GOTCHA : If GameObject is renamed and save file exists then it can cause data corruption.
                string guid = System.Guid.NewGuid().ToString();
                guid = guid.Substring(0, 8);
                UniqueId = $"{gameObject.name} GameObject [{guid}]";
            }
        }

        private void Awake()
        {
            saveableTargetsRegistry = new Dictionary<SaveCategory, List<SaveableTarget>>();
            

            Component[] componentsOnObject = GetComponents<Component>();
            foreach (Component component in componentsOnObject)
            {
                Type componentType = component.GetType();
                SaveableAttribute attribute = componentType.GetCustomAttribute<SaveableAttribute>();
                if (attribute == null) continue;

                string classKey = !string.IsNullOrEmpty(attribute.ClassId) ? attribute.ClassId : componentType.Name;
                Type interfaceType = componentType.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                SaveableTarget target = new SaveableTarget()
                {
                    Script = component as MonoBehaviour,
                    ClassKey = classKey,
                    CaptureDataMethod = componentType.GetMethod(isaveableCaptureMethod),
                    RestoreDataMethod = componentType.GetMethod(isaveableRestoreMethod),
                    DataToSaveType = interfaceType.GetGenericArguments()[0]
                };

                if (!saveableTargetsRegistry.ContainsKey(attribute.Category))
                {
                    List<SaveableTarget> saveableTargetsList = new List<SaveableTarget>();
                    saveableTargetsList.Add(target);
                    saveableTargetsRegistry.Add(attribute.Category, saveableTargetsList);
                }
                else
                {
                    if (!saveableTargetsRegistry.TryGetValue(attribute.Category, out List<SaveableTarget> saveableTargetsList))
                    {
                        continue;
                    }
                    if (!saveableTargetsList.Contains(target))
                    {
                        saveableTargetsList.Add(target);
                    }

                }
                if (!foundCategoriesList.Contains(attribute.Category))
                {
                    foundCategoriesList.Add(attribute.Category);
                }
            }
        }

        public object CaptureState(SaveCategory categoryFilter)
        {
            Dictionary<string, object> combinedCapturedDataMap = new Dictionary<string, object>();

            if (!saveableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> saveableTargetsList))
            {
                return null;
            }
            foreach (SaveableTarget target in saveableTargetsList)
            {
                object capturedData = target.CaptureDataMethod.Invoke(target.Script, null);
                if (capturedData != null)
                {
                    combinedCapturedDataMap.Add(target.ClassKey, capturedData);
                }
            }

            return combinedCapturedDataMap;
        }

        public void RestoreState(object data, SaveCategory categoryFilter)
        {
            Type dictionaryType = typeof(Dictionary<string, object>);

            Dictionary<string, object> combinedCapturedDataMap = SaveDataConverter.Convert<Dictionary<string, object>>(data);
            if (combinedCapturedDataMap == null) return;

            if (!saveableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> targetsList))
            {
                return;
            }
            foreach (SaveableTarget target in targetsList)
            {
                if (!combinedCapturedDataMap.TryGetValue(target.ClassKey, out object rawLoadedData))
                {
                    continue;
                }
                object typedData = SaveDataConverter.Convert(rawLoadedData, target.DataToSaveType);
                target.RestoreDataMethod.Invoke(target.Script, new object[] { typedData });
            }
        }


        private void OnEnable()
        {
            if (foundCategoriesList.Count > 0)
            {
                SaveManager.Instance.RegisterSaveableObject(this, foundCategoriesList);
            }
        }

        private void OnDisable()
        {
            if (SaveManager.Instance != null && foundCategoriesList.Count > 0)
            {
                SaveManager.Instance.UnregisterSaveableObject(this, foundCategoriesList);
            }
        }
    }
}
