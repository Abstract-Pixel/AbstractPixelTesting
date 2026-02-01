using UnityEngine;
using System;

namespace AbstractPixel.Utility.Save
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =true)]
    public class SaveableAttribute : Attribute
    {
        public SaveCategory Category {  get; set; }
        public string ClassId {  get; set; }
        public SaveableAttribute(SaveCategory _dataCategory,string _classId = default)
        {
            Category = _dataCategory;
            ClassId = _classId;

        }
    
    }
}
