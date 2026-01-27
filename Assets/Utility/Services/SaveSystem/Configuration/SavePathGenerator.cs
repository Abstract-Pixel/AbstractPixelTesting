using System.IO;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public static class SavePathGenerator
    {
        private static SaveSystemConfigSO saveConfig { get; set; }
        private static string debugRoot => Path.Combine(Application.dataPath, "DebugSaveFiles");
        private static string shipRoot => Path.Combine(Application.persistentDataPath, "SaveFiles");
        private static string currentRoot { get; set; }

        public static void Initialize(SaveSystemConfigSO config)
        {
            saveConfig = config;
            currentRoot = saveConfig.useDebugPath ? debugRoot : shipRoot;
        }

        // 1. GLOBAL PATHS
        public static string GetGlobalPath(string filename)
        {
            // Returns: .../SaveFiles/Global/settings.json
            return Path.Combine(shipRoot, "Global", filename);
        }

        // 2. PROFILE PATHS
        public static string GetProfileRoot(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/Warrior_Lv50/
            return Path.Combine(shipRoot, "GameSaves", "Profiles", profileId);
        }

        public static string GetProfileBackupPath(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/Warrior_Lv50/Backups/
            return Path.Combine(GetProfileRoot(profileId), "Backups");
        }

        // 3. AUTOSAVE PATHS
        public static string GetAutoSavePath(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/AutoSave/Warrior_Lv50/
            return Path.Combine(shipRoot, "GameSaves", "AutoSave", profileId);
        }
    }
}
