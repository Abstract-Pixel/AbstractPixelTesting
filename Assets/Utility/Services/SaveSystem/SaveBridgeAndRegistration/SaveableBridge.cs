using AbstractPixel.Utility.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveableBridge : MonoBehaviour, ISaveableBridge
    {
        [field: SerializeField] public string UniqueId { get; private set; }
        
        [SerializeField] private List<SaveableTarget> saveableTargets = new List<SaveableTarget>();
        [SerializeField] private List<SaveCategory> foundCategoriesList = new List<SaveCategory>();

        private Dictionary<SaveCategory, List<SaveableTarget>> saveableTargetsRegistry;
        
        readonly string isaveableCaptureMethod = "CaptureData";
        readonly string isaveableRestoreMethod = "RestoreData";

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(UniqueId))
            {
                string guid = Guid.NewGuid().ToString().Substring(0, 8);
                UniqueId = $"{gameObject.name} GameObject [{guid}]";
            }

            // ---------------------------------------------------------
            // Automation: Sync ISaveable Components with SaveableTargets
            // ---------------------------------------------------------

            if (saveableTargets == null) saveableTargets = new List<SaveableTarget>();

            // 1. Find all valid ISaveable components on this GameObject
            MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
            List<MonoBehaviour> validScripts = new List<MonoBehaviour>();

            foreach (var script in allScripts)
            {
                if (script == null) continue;
                Type type = script.GetType();

                // Requirement A: Must implement ISaveable<T>
                Type interfaceType = type.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                // Requirement B: Must have [Saveable] attribute (for Category)
                if (type.GetCustomAttribute<SaveableAttribute>() == null) continue;

                validScripts.Add(script);
            }

            // 2. Clean up removed scripts from the list
            for (int i = saveableTargets.Count - 1; i >= 0; i--)
            {
                if (saveableTargets[i] == null || saveableTargets[i].Script == null || !validScripts.Contains(saveableTargets[i].Script))
                {
                    saveableTargets.RemoveAt(i);
                }
            }

            // 3. Register new scripts or update existing ones
            foreach (var script in validScripts)
            {
                // Check if we already have a registration for this specific script reference
                SaveableTarget existingTarget = saveableTargets.FirstOrDefault(t => t.Script == script);

                if (existingTarget != null)
                {
                    // Update Debug Name (In case class was renamed, but we keep the GUID)
                    if (existingTarget.Identification != null)
                    {
                        existingTarget.Identification.ClassName = script.GetType().Name;
                    }
                }
                else
                {
                    // Create New Persistent Registration
                    string newGuid = Guid.NewGuid().ToString();
                    SaveableIdentification id = new SaveableIdentification(script.GetType().Name, newGuid);
                    saveableTargets.Add(new SaveableTarget(script, id));
                }
            }
        }

        private void Awake()
        {
            saveableTargetsRegistry = new Dictionary<SaveCategory, List<SaveableTarget>>();
            foundCategoriesList = new List<SaveCategory>();

            // Hydrate the Runtime Reflection Data into the Serialized Targets
            foreach (SaveableTarget target in saveableTargets)
            {
                if (target == null || target.Script == null) continue;

                Type componentType = target.Script.GetType();
                SaveableAttribute attribute = componentType.GetCustomAttribute<SaveableAttribute>();
                // Double check runtime validity
                if (attribute == null) continue;
                
                Type interfaceType = componentType.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                // 1. Fill Reflection Data
                target.CaptureDataMethod = componentType.GetMethod(isaveableCaptureMethod);
                target.RestoreDataMethod = componentType.GetMethod(isaveableRestoreMethod);
                target.DataToSaveType = interfaceType.GetGenericArguments()[0];

                // 2. Register into Runtime Dictionary (Buckets by Category)
                if (!saveableTargetsRegistry.ContainsKey(attribute.Category))
                {
                    saveableTargetsRegistry.Add(attribute.Category, new List<SaveableTarget>());
                }
                saveableTargetsRegistry[attribute.Category].Add(target);

                // 3. Track Categories for SaveManager
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
                    // CRITICAL: We use a Composite Key (Name#GUID) for Debuggability + Uniqueness.
                    // The Name part allows dev to read the file. The GUID part allows safe renaming.
                    if (target.Identification != null && !string.IsNullOrEmpty(target.Identification.GUID))
                    {
                        string compositeKey = $"{target.Identification.ClassName} #{target.Identification.GUID}";
                        combinedCapturedDataMap.Add(compositeKey, capturedData);
                    }
                }
            }

            return combinedCapturedDataMap;
        }

        public void RestoreState(object data, SaveCategory categoryFilter)
        {
            // Convert raw object back to Dictionary
            Dictionary<string, object> combinedCapturedDataMap = SaveDataConverter.Convert<Dictionary<string, object>>(data);
            if (combinedCapturedDataMap == null) return;

            if (!saveableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> targetsList))
            {
                return;
            }

            // Optimization: Create a GUID map for O(1) lookup
            Dictionary<string, SaveableTarget> guidToTargetMap = new Dictionary<string, SaveableTarget>();
            foreach (var t in targetsList)
            {
                if (t.Identification != null && !string.IsNullOrEmpty(t.Identification.GUID))
                {
                    if (!guidToTargetMap.ContainsKey(t.Identification.GUID))
                    {
                        guidToTargetMap.Add(t.Identification.GUID, t);
                    }
                }
            }

            // Iterate the LOADED KEYS (which contain the Data)
            foreach (KeyValuePair<string, object> kvp in combinedCapturedDataMap)
            {
                string compositeKey = kvp.Key;
                
                // Extract GUID from "ClassName#GUID"
                // We split by the LAST '#' to ensure we get the GUID at the end.
                int separatorIndex = compositeKey.LastIndexOf('#');
                
                // Fallback: If no separator found (legacy data?), assume key is GUID or old ClassKey
                string extractedGuid = (separatorIndex != -1) ? compositeKey.Substring(separatorIndex + 1) : compositeKey;

                if (guidToTargetMap.TryGetValue(extractedGuid, out SaveableTarget target))
                {
                     object typedData = SaveDataConverter.Convert(kvp.Value, target.DataToSaveType);
                     target.RestoreDataMethod.Invoke(target.Script, new object[] { typedData });
                }
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
