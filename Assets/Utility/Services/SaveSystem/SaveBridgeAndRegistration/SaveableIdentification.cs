using System;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    [Serializable]
    public class SaveableIdentification
    {
        public string ClassName;
        public string ClassID;

        public SaveableIdentification(string _className, string _classID)
        {
            ClassName = _className;
            ClassID = _classID;
        }
    
    }
}
