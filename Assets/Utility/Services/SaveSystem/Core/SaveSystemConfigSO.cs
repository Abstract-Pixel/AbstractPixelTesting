using System.Collections.Generic;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    [CreateAssetMenu(fileName = "SaveSystemConfigSO", menuName = "Utility/SaveSystemConfigSO")]
    public class SaveSystemConfigSO : ScriptableObject
    {
        [field: SerializeField] public bool useDebugPath { get; private set; } = false;
        [SerializeField] List<SaveCatgeoryDefinition> categoryDefinitionList;

        Dictionary<SaveCategory, SaveCatgeoryDefinition> categoryDefinitionMap = new();

        private void OnValidate()
        {
            for (int i = 0; i < categoryDefinitionList.Count; i++)
            {
                string nameToDisplay = categoryDefinitionList[i].Category.ToString();
                categoryDefinitionList[i].DisplayName = nameToDisplay;
            }
        }

        public void Initialize()
        {
            categoryDefinitionMap = new Dictionary<SaveCategory, SaveCatgeoryDefinition>();
            foreach (SaveCatgeoryDefinition definition in categoryDefinitionList)
            {
                if (!categoryDefinitionMap.ContainsKey(definition.Category))
                {
                    categoryDefinitionMap.Add(definition.Category, definition);
                }
            }
        }

        public SaveCatgeoryDefinition GetCategoryDefinition(SaveCategory category)
        {
            if (categoryDefinitionMap.TryGetValue(category, out SaveCatgeoryDefinition definition))
            {
                return definition;
            }
            Debug.LogWarning($"[SaveSystemConfigSO] - No definition found for category: {category}");
            return null;
        }

    }
}
