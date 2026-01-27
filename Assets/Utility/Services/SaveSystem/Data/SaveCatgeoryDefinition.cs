using System;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    [Serializable]
    public class SaveCatgeoryDefinition
    {
        [HideInInspector]public string DisplayName;
        public SaveCategory Category;
        public SaveScope DirectoryScope;
        public string CustomFileName;
        public bool encryptFile;
    }
}
