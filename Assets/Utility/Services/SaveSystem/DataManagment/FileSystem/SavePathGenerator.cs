using System.IO;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public static class SavePathGenerator
    {
        // Constant Folder Names (Can be changed per project if needed)
        public static readonly string RootSaveFolder = "SaveFiles";
        public static readonly string DebugRootSaveFolder = "DebugSaveFiles";
        public static readonly string GlobalSavesFolder = "Global";
        public static readonly string GlobalBackupSavesFolder = "GlobalBackupsName";
        public static readonly string GameSavesFolder = "GameSaves";
        public static readonly string ProfileSavesRootFolder = "SaveProfiles";
        public static readonly string GameProfileSavesFolder = "GameProfile";
        public static readonly string GameProfileBackupSavesFolder = "ProfileBackups";
        public static readonly string AutoSavesFolder = "AutoSaves";

        // Constant Paths
        public static string DebugRootPath { get; private set; }
        public static string ShipRootPath { get; private set; }

        // Config Properties To Be Initialized
        public static SaveSystemConfigSO saveConfig { get; set; }
        public static string CurrentRootPath { get; set; }
        public static string PrimaryFileExtension { get; set; }
        public static string BackupFileExtension { get; set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeConstantPaths()
        {
            DebugRootPath = Path.Combine(Application.dataPath, DebugRootSaveFolder);
            ShipRootPath = Path.Combine(Application.persistentDataPath, RootSaveFolder);
        }

        public static void Initialize(SaveSystemConfigSO config)
        {
            saveConfig = config;
            CurrentRootPath = saveConfig.UseDebugPath ? DebugRootPath : ShipRootPath;
            PrimaryFileExtension = $".{saveConfig.PrimaryFileExtension.ToString().ToLower()}";
            BackupFileExtension = $".{saveConfig.BackupFileExtension.ToString().ToLower()}";
        }

        public static string GetPath(SaveCatgeoryDefinition _definition, string _profileId = default)
        {
            return GenerateFullPath(_definition, false, _profileId);
        }

        public static string GetBackupPath(SaveCatgeoryDefinition _definition, string _profileId = default)
        {
            return GenerateFullPath(_definition, true, _profileId);
        }
        public static string GetAutoSavePath(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/AutoSave/GameProfile+id/
            return Path.Combine(CurrentRootPath, GameSavesFolder, AutoSavesFolder, profileId);
        }

        public static string GetFileNameBasedOnCategoryDefinition(SaveCatgeoryDefinition _definition, bool _isBackUp=false)
        {
            string fileName = null;
            string fileExtension = _isBackUp ? BackupFileExtension : PrimaryFileExtension;
            if (string.IsNullOrEmpty(_definition.CustomFileName))
            {
                fileName = _definition.Category.ToString() + fileExtension;
            }
            else
            {
                fileName = _definition.CustomFileName + fileExtension;
            }

            return fileName;
        }

        #region Helper Methods
      
        // 1. GLOBAL PATHS
        public static string GetGlobalPath()
        {
            // Returns: .../SaveFiles/Global/
            return Path.Combine(CurrentRootPath, GlobalSavesFolder);
        }

        private static string GetGlobalBackupPath()
        {
            // Returns: .../SaveFiles/Global/Backups/
            return Path.Combine(CurrentRootPath, GlobalSavesFolder, GlobalBackupSavesFolder);
        }

        // 2. PROFILE PATHS
        public static string GetProfilesRootPath()
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/
            return Path.Combine(CurrentRootPath, GameSavesFolder, ProfileSavesRootFolder);
        }
        public static string GetProfilePath(string _profileId)
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/GameProfile+profileId/
            return Path.Combine(CurrentRootPath, GameSavesFolder, ProfileSavesRootFolder, GameProfileSavesFolder + _profileId);
        }
        #endregion

        private static string GenerateFullPath(SaveCatgeoryDefinition _definition, bool _isBackUp, string _profileId = default)
        {
            string fileName = GetFileNameBasedOnCategoryDefinition(_definition, _isBackUp);

            string directoryPath = null;
            if (_definition.DirectoryScope == SaveScope.Global)
            {
                directoryPath = _isBackUp ? GetGlobalBackupPath() : GetGlobalPath();
            }
            else if (_definition.DirectoryScope == SaveScope.GameProfile)
            {
                directoryPath = _isBackUp ? GetProfileBackupPath(_profileId) : GetProfilePath(_profileId);
            }
            directoryPath = Path.Combine(directoryPath, fileName);
            return directoryPath;
        }

        private static string GetProfileBackupPath(string _profileId)
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/GameProfile/Backups/
            return Path.Combine(GetProfilePath(_profileId), GameProfileBackupSavesFolder);
        }
    }
}
