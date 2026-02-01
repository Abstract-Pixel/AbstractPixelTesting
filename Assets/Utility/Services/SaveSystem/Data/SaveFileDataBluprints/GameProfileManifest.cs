using UnityEngine;
using System;

namespace AbstractPixel.Utility.Save
{
    [Serializable]
    public sealed class GameProfileManifest
    {
        public static readonly string ManifestFileName = "GameProfileManifest";
        public string Version;
        public string ProfileID;
        public string ProfileName;
        public string CreationDateAndTime;

        public GameProfileManifest() { }

        public GameProfileManifest(string _profileId)
        {
            ProfileID = _profileId;
            ProfileName = SavePathGenerator.GameProfileSavesFolder + _profileId;
            CreationDateAndTime = DateTime.UtcNow.ToString("o");
            Version = Application.version;
        }
    }
}
