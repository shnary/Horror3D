using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Core.Systems {
    public static class FileManager {
        public static void CreateDirectoryIfNotExists(string directoryPath, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                directoryPath = GetPath(directoryPath);
            }
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static void WriteFile(string path, string data, bool binaryFormat=true, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                path = GetPath(path);
            }
            if (binaryFormat) {
                using FileStream stream = new FileStream(path, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
            else {
                using StreamWriter stream = new StreamWriter(path);
                stream.Write(data);
            }
        }

        public static bool FileExists(string path, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                path = GetPath(path);
            }

            return File.Exists(path);
        }

        public static bool DirectoryExists(string path, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                path = GetPath(path);
            }

            return Directory.Exists(path);
        }

        public static string ReadFile(string filePath, bool binaryFormat=true, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                filePath = GetPath(filePath);
            }
            if (File.Exists(filePath)) {

                string data = "";

                if (binaryFormat) {
                    using FileStream stream = new FileStream(filePath, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    data = formatter.Deserialize(stream) as string;
                }
                else {
                    using StreamReader stream = new StreamReader(filePath);
                    data = stream.ReadToEnd();
                }
                
                return data;
            }

            return "";
        }

        public static Dictionary<string, string> ReadFiles(string directoryPath, string searchPattern, bool binaryFormat=true, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                directoryPath = GetPath(directoryPath);
            }
            var files = Directory.GetFiles(directoryPath, searchPattern);
            var contents = new Dictionary<string, string>();

            foreach (string filePath in files) {
                string data = "";

                if (binaryFormat) {
                    using FileStream stream = new FileStream(filePath, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    data = formatter.Deserialize(stream) as string;
                }
                else {
                    using StreamReader stream = new StreamReader(filePath);
                    data = stream.ReadToEnd();
                }
                
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                contents[fileName] = data;
            }

            return contents;
        }

        public static string[] GetFiles(string directoryPath, string searchPattern, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                directoryPath = GetPath(directoryPath);
            }
            var files = Directory.GetFiles(directoryPath, searchPattern);
            var fileNames = new string[files.Length];

            for (int i = 0; i < fileNames.Length; i++) {
                fileNames[i] = Path.GetFileNameWithoutExtension(files[i]);
            }

            return fileNames;
        }

        public static bool TryRenameFile(string directoryPath, string oldFileName, string newFileName, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                directoryPath = GetPath(directoryPath);
            }
            if (Directory.Exists(directoryPath) == false) {
                return false;
            }
            
            if (File.Exists(directoryPath + "/" + oldFileName) == false) {
                return false;
            }
            
            if (File.Exists(directoryPath + "/" + newFileName)) {
                return false;
            }
            
            File.Move(oldFileName, newFileName);
            return true;
        }
        
        public static void DeleteFile(string filePath, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                filePath = GetPath(filePath);
            }
            if (!File.Exists(filePath)) {
                return;
            }

            File.Delete(filePath);
        }

        public static DateTime GetLastModifiedDate(string filePath, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                filePath = GetPath(filePath);
            }
            if (File.Exists(filePath)) {
                
                return File.GetLastWriteTime(filePath);
            }

            return DateTime.MinValue;
        }

        public static DateTime GetLastAccessDate(string filePath, bool usePersistentDataPath=true) {
            if (usePersistentDataPath) {
                filePath = GetPath(filePath);
            }
            if (File.Exists(filePath)) {
                
                return File.GetLastAccessTime(filePath);
            }

            return DateTime.MinValue;
        }

        private static string GetPath(string filePath) {
            string path = Application.persistentDataPath + filePath;
            return path;
        }
    }
}