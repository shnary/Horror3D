using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Systems;
using UnityEngine;

namespace Core.Systems {
    public class SaveSystem {
        
        private const string ContentSplitTag = "---";
        private const string JsonSplitTag = "###";

        private readonly Dictionary<string, ISavable> _savableDataDictionary = new Dictionary<string, ISavable>();

        private readonly bool _useBinaryFormat;
        private readonly string _directoryName;
        private readonly string _fileExtension;

        public SaveSystem(string directoryName, string fileExtension, bool useBinaryFormat=true) {
            FileManager.CreateDirectoryIfNotExists($"/{directoryName}");
            _directoryName = directoryName;
            _fileExtension = fileExtension;
            _useBinaryFormat = useBinaryFormat;
        }

        public void Register(string key, ISavable savable) {
            _savableDataDictionary[key] = savable;
        }

        public void DeRegister(string key) {
            if (_savableDataDictionary.ContainsKey(key)) {
                _savableDataDictionary.Remove(key);
            }
        }
        
        public void Save(string fileName) {
            var fileContentStringBuilder = new StringBuilder();
            
            foreach (var key in _savableDataDictionary.Keys) {

                var serializedData = _savableDataDictionary[key].OnSerialize();

                var serializedDataStringBuilder = new StringBuilder();

                serializedDataStringBuilder.Append($"[{key}]{JsonSplitTag}");
                serializedDataStringBuilder.Append(serializedData);
                serializedDataStringBuilder.Append($"\n{ContentSplitTag}\n");

                fileContentStringBuilder.Append(serializedDataStringBuilder.ToString());
            }
            
            string saveContent = fileContentStringBuilder.ToString();
            FileManager.WriteFile(GetFilePath(fileName), saveContent, _useBinaryFormat, true);
        }

        public async Task SaveAsync(string fileName, Action onSave=null) {
            var fileContentStringBuilder = new StringBuilder();
            
            await Task.Run(() => {
                foreach (var key in _savableDataDictionary.Keys) {

                    var serializedData = _savableDataDictionary[key].OnSerialize();

                    var serializedDataStringBuilder = new StringBuilder();

                    serializedDataStringBuilder.Append($"[{key}]{JsonSplitTag}");
                    serializedDataStringBuilder.Append(serializedData);
                    serializedDataStringBuilder.Append($"\n{ContentSplitTag}\n");

                    fileContentStringBuilder.Append(serializedDataStringBuilder.ToString());
                }

            });
            
            string saveContent = fileContentStringBuilder.ToString();
            FileManager.WriteFile(GetFilePath(fileName), saveContent, _useBinaryFormat, true);
            onSave?.Invoke();
        }
        
        public void Load(string fileName) {
            var fileContent = FileManager.ReadFile(GetFilePath(fileName), _useBinaryFormat, true);
            
            var jsonDataArray = fileContent.Split(ContentSplitTag);

            var rx = new Regex(@"(?<=\[).+?(?=\])",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (var jsonData in jsonDataArray) {
                if (string.IsNullOrEmpty(jsonData)) {
                    continue;
                }

                var keyContentArray = jsonData.Split(JsonSplitTag);

                // Find matches.
                MatchCollection matches = rx.Matches(keyContentArray[0]);
                if (matches.Count == 0) {
                    continue;
                }

                var key = matches[0].Value;

                var serializedData = keyContentArray[1];

                _savableDataDictionary[key].OnDeserialize(serializedData);
            }
        }

        public async Task LoadAsync(string fileName, Action onLoad=null) {
            var fileContent = FileManager.ReadFile(GetFilePath(fileName), _useBinaryFormat, true);
            
            await Task.Run(() => {
                var jsonDataArray = fileContent.Split(ContentSplitTag);

                var rx = new Regex(@"(?<=\[).+?(?=\])",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

                foreach (var jsonData in jsonDataArray) {

                    if (string.IsNullOrEmpty(jsonData)) {
                        continue;
                    }

                    var keyContentArray = jsonData.Split(JsonSplitTag);

                    // Find matches.
                    MatchCollection matches = rx.Matches(keyContentArray[0]);
                    if (matches.Count == 0) {
                        continue;
                    }

                    var key = matches[0].Value;

                    var serializedData = keyContentArray[1];

                    _savableDataDictionary[key].OnDeserialize(serializedData);
                }
                
                onLoad?.Invoke();
            });
        }

        private string GetFilePath(string fileName) {
            return $"/{_directoryName}/{fileName}.{_fileExtension}";
        }
    }
}
