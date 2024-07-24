using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Jois
{
    public static class UFileIoTools
    {
        public static string MakePersistentDataPathForRead(string fileName)
        {
            return "file://" + Application.persistentDataPath + "/" + fileName;
        }

        public static string MakePersistentDataPath(string subDirectory, string fileName) //, string extension)
        {
            return string.IsNullOrEmpty(subDirectory)
                ? Application.persistentDataPath + "/" + fileName
                : Application.persistentDataPath + "/" + subDirectory + "/" + fileName;
        }

        public static void CreateDirectoryInPersistent(string directory)
        {
            var path = Application.persistentDataPath + "/" + directory;
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        public static void DeleteDirectoryInPersistent(string directory)
        {
            var path = Application.persistentDataPath + "/" + directory;
            if (Directory.Exists(path) == false)
                Directory.Delete(path);
        }

        // public static UniTask<byte[]> DownloadFile(string url)
        // {
        //     
        // }

        // public static void WriteFile(string fullPath)
        // {
        // }
        //
        // public static bool IsExists(string fullPath)
        // {
        //     return false;
        // }

        public static bool IsExistsInPersistent(string subDirectory, string fileName)
        {
            var path = string.IsNullOrEmpty(subDirectory)
                ? Application.persistentDataPath + "/" + fileName
                : Application.persistentDataPath + "/" + subDirectory + "/" + fileName;
            return File.Exists(path);
        }
    }
}