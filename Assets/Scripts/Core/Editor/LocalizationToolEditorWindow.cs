using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Core.Editors {
    
    public class LocalizationToolEditorWindow : EditorWindow {

        public class ContentTable : ScriptableObject {
            public string h;
            public LocalizedString l;
            public StringTableCollection[] tables;
        }
        
        public class ContentCollection : ScriptableObject {
            public string h;
            public LocalizedString l;
            public StringTableCollection[] tables;
        }
        
        public class ContentEverything : ScriptableObject {
            public string h;
            public StringTable l;
            public StringTableCollection[] tables;
        }

        private enum LocalizeState {
            StringTable, Collection, Everything
        }

        private LocalizeState _currentState;
        private Editor _myScriptableObjectEditor;
        
        private ContentTable _contentTable;
        private ContentCollection _contentCollection;
        private ContentEverything _contentEverything;
        
        [MenuItem("Tools/Localization Tool")]
        static void CreateWindow() {
            EditorWindow.GetWindow(typeof(LocalizationToolEditorWindow), false, "Localization Tool", true);
        }
        
        [MenuItem("Tools/Localization Tool (Standalone)")]
        static void CreateWindowStandalone() {
            EditorWindow.GetWindow(typeof(LocalizationToolEditorWindow), true, "Localization Tool", true);
        }
        
        private void OnEnable() {
            var collection = LocalizationEditorSettings.GetStringTableCollection("Testing");
            foreach (var lazyLoadReference in collection.Tables) {
                Debug.Log(lazyLoadReference.asset.LocaleIdentifier.Code);
            }

            _contentTable = ScriptableObject.CreateInstance<ContentTable>();
            _contentCollection = ScriptableObject.CreateInstance<ContentCollection>();
            _contentEverything = ScriptableObject.CreateInstance<ContentEverything>();
            _contentTable.tables = MyEditorUtils.GetAllInstances<StringTableCollection>();
            _myScriptableObjectEditor = Editor.CreateEditor(_contentTable);
        }

        private void OnGUI() {
            
            EditorGUILayout.BeginHorizontal();
            MyEditorUtils.DrawButton(LocalizeState.StringTable.ToString(), () => {
                _myScriptableObjectEditor = Editor.CreateEditor(_contentTable);
                _currentState = LocalizeState.StringTable;
            });
            MyEditorUtils.DrawButton(LocalizeState.Collection.ToString(), () => {
                _myScriptableObjectEditor = Editor.CreateEditor(_contentCollection);
                _currentState = LocalizeState.Collection;
            });
            MyEditorUtils.DrawButton(LocalizeState.Everything.ToString(), () => {
                _myScriptableObjectEditor = Editor.CreateEditor(_contentEverything);
                _currentState = LocalizeState.Everything;
            });
            EditorGUILayout.EndHorizontal();
            
            MyEditorUtils.DrawBoldTitle("Localize " + _currentState.ToString());

            _myScriptableObjectEditor.OnInspectorGUI();
        }

        private void LocalizeStringTable(StringTableEntry localizedString) {
            Debug.Log(localizedString.ToString());
        }
    }
}
