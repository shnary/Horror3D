using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Editors {
    public static class MyEditorUtils {
        
        public static T[] GetAllInstances<T>() where T : ScriptableObject {
            // FindAssets uses tags check documentation for more info
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); 
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }
        
        public static void DrawButton(string title, Action onClick) {
            bool clicked = GUILayout.Button(title);
            if (clicked) {
                onClick?.Invoke();
            }
        }

        public static void DrawBoldTitle(string title) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        public static void DrawInfo(string labelName, string value, float minWidth=80) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelName, EditorStyles.boldLabel, GUILayout.MinWidth(minWidth));
            EditorGUILayout.LabelField(value, GUILayout.MinWidth(minWidth));
            EditorGUILayout.EndHorizontal();
        }
        
        public static void DrawPlusMinusProperty(string label, float value, Action<int> onClick, ref float valuePersistent) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.MinWidth(50));
            EditorGUILayout.LabelField(value.ToString(), GUILayout.MinWidth(50));
            bool increment= GUILayout.Button("+");
            bool decrement= GUILayout.Button("-");
            valuePersistent = EditorGUILayout.FloatField(valuePersistent);

            if (increment) {
                onClick?.Invoke(1);
            }

            if (decrement) {
                onClick?.Invoke(-1);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        public static void DrawInspectorLine(Color color, int thickness = 2, int padding = 10) {
            DrawRect(color, thickness, padding);
        }

        public static void DrawRect(Color color, int height, int padding) {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding+height));
            rect.width += 6;
            rect.height = height;
            rect.y += (float)padding / 2;
            rect.x-=2;
            EditorGUI.DrawRect(rect, color);
        }
    }
}
