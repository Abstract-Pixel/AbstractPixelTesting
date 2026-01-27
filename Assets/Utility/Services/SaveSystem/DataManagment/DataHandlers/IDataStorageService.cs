using System.Threading.Tasks;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public interface IDataStorageService
    {
        bool Save(string _data, string fullpath);

        string Load(string fullpath);

        bool DeleteFile(string _filePath);

        bool DeleteDirectory(string _directoryPath);
    }
}
