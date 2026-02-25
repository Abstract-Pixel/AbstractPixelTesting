using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SavableBridge : MonoBehaviour, ISavableBridge
    {
        [field: SerializeField,ReadOnly(true)] public string UniqueId { get; private set; }
        [SerializeField,HideInInspector] string lastKnownInstanceID = "";

        [SerializeField,ReadOnly] private List<SaveableTarget> savableTargets = new List<SaveableTarget>();
        private List<SaveCategory> foundCategoriesList = new List<SaveCategory>();

        private Dictionary<SaveCategory, List<SaveableTarget>> savableTargetsRegistry;

        readonly string isavableCaptureMethod = "CaptureData";
        readonly string isavableRestoreMethod = "RestoreData";
        readonly string stringSeparatorIdentifier = "#";

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (lastKnownInstanceID != GetCurrentGlobalID() || string.IsNullOrEmpty(UniqueId))
            {
                string guid = Guid.NewGuid().ToString();
                UniqueId = $"{gameObject.name} GameObject {stringSeparatorIdentifier}[{guid}]";
                lastKnownInstanceID = GetCurrentGlobalID();
            }

            if (savableTargets == null) savableTargets = new List<SaveableTarget>();

            MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
            List<MonoBehaviour> validScripts = new List<MonoBehaviour>();

            foreach (var script in allScripts)
            {
                if (script == null) continue;
                Type type = script.GetType();

                Type interfaceType = type.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                if (type.GetCustomAttribute<SaveableAttribute>() == null) continue;

                validScripts.Add(script);
            }

            //Clean up removed scripts from the list
            for (int i = savableTargets.Count - 1; i >= 0; i--)
            {
                if (savableTargets[i] == null || savableTargets[i].Script == null || !validScripts.Contains(savableTargets[i].Script))
                {
                    savableTargets.RemoveAt(i);
                }
            }

            foreach (MonoBehaviour script in validScripts)
            {
                // Check if we already have a registration for this specific script reference
                SaveableTarget existingTarget = savableTargets.FirstOrDefault(t => t.Script == script);

                if (existingTarget != null)
                {
                    // Update Debug Name (In case class was renamed, but we keep the GUID)
                    if (existingTarget.Identification != null)
                    {
                        existingTarget.Identification.ClassName = script.GetType().Name;
                        existingTarget.InpsectorName = script.GetType().Name;
                    }
                }
                else
                {
                    string newGuid = Guid.NewGuid().ToString();
                    SaveableIdentification id = new SaveableIdentification(script.GetType().Name, newGuid);
                    savableTargets.Add(new SaveableTarget(script, id));
                }
            }
            string GetCurrentGlobalID()
            {
                UnityEditor.GlobalObjectId globalId = UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(this);
                return globalId.ToString();
            }
        }
#endif



        private void Awake()
        {
            savableTargetsRegistry = new Dictionary<SaveCategory, List<SaveableTarget>>();
            foundCategoriesList = new List<SaveCategory>();

            foreach (SaveableTarget target in savableTargets)
            {
                if (target == null || target.Script == null) continue;

                Type componentType = target.Script.GetType();
                SaveableAttribute attribute = componentType.GetCustomAttribute<SaveableAttribute>();
                if (attribute == null) continue;

                Type interfaceType = componentType.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                target.CaptureDataMethod = componentType.GetMethod(isavableCaptureMethod);
                target.RestoreDataMethod = componentType.GetMethod(isavableRestoreMethod);
                target.DataToSaveType = interfaceType.GetGenericArguments()[0];

                if (!savableTargetsRegistry.ContainsKey(attribute.Category))
                {
                    savableTargetsRegistry.Add(attribute.Category, new List<SaveableTarget>());
                }
                savableTargetsRegistry[attribute.Category].Add(target);

                if (!foundCategoriesList.Contains(attribute.Category))
                {
                    foundCategoriesList.Add(attribute.Category);
                }
            }
        }

        public object CaptureState(SaveCategory categoryFilter)
        {
            Dictionary<string, object> combinedCapturedDataMap = new Dictionary<string, object>();

            if (!savableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> saveableTargetsList))
            {
                return null;
            }

            foreach (SaveableTarget target in saveableTargetsList)
            {
                object capturedData = target.CaptureDataMethod.Invoke(target.Script, null);
                if (capturedData != null)
                {
                    if (target.Identification != null && !string.IsNullOrEmpty(target.Identification.GUID))
                    {
                        string compositeKey = $"{target.Identification.ClassName} Component {stringSeparatorIdentifier}{target.Identification.GUID}";
                        combinedCapturedDataMap.Add(compositeKey, capturedData);
                    }
                }
            }

            return combinedCapturedDataMap;
        }

        public void RestoreState(object data, SaveCategory categoryFilter)
        {
            Dictionary<string, object> combinedCapturedDataMap = SaveDataConverter.Convert<Dictionary<string, object>>(data);
            if (combinedCapturedDataMap == null) return;

            if (!savableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> targetsList))
            {
                return;
            }

            Dictionary<string, SaveableTarget> guidToTargetMap = new Dictionary<string, SaveableTarget>();
            foreach (SaveableTarget target in targetsList)
            {
                if (target.Identification != null && !string.IsNullOrEmpty(target.Identification.GUID))
                {
                    if (!guidToTargetMap.ContainsKey(target.Identification.GUID))
                    {
                        guidToTargetMap.Add(target.Identification.GUID, target);
                    }
                }
            }

            foreach (KeyValuePair<string, object> kvp in combinedCapturedDataMap)
            {
                string compositeKey = kvp.Key;

                // Extract GUID from "ClassName#GUID"
                // We split by the LAST '#' to ensure we get the GUID at the end.
                int separatorIndex = compositeKey.LastIndexOf(stringSeparatorIdentifier) + 1;

                string extractedGuid = (separatorIndex != -1) ? compositeKey.Substring(separatorIndex) : compositeKey;

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
                SaveManager.Instance.RegisterSavableObject(this, foundCategoriesList);
            }
        }

        private void OnDisable()
        {
            if (SaveManager.Instance != null && foundCategoriesList.Count > 0)
            {
                SaveManager.Instance.UnregisterSavableObject(this, foundCategoriesList);
            }
        }
    }
}