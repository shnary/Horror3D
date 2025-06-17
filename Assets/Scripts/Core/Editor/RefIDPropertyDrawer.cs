using System;
using Core.Plains;
using UnityEditor;
using UnityEngine;
 
namespace Core.Editors {
    [CustomPropertyDrawer(typeof(RefID))]
    public class RefIDPropertyDrawer : PropertyDrawer {
    
        private float ySep = 20;
        private float buttonSize;
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Start property draw
            EditorGUI.BeginProperty(position, label, property);
    
            // Get property
            SerializedProperty refId = property.FindPropertyRelative("refId");
            SerializedProperty backup = property.FindPropertyRelative("backupRefId");
    
            // Draw label
            position = EditorGUI.PrefixLabel(new Rect(position.x, position.y + ySep / 2, position.width, position.height), GUIUtility.GetControlID(FocusType.Passive), label);
            position.y -= ySep / 2; // Offsets position so we can draw the label for the field centered
    
            buttonSize = position.width / 4; // Update size of buttons to always fit perfeftly above the string representation field
    
            // Buttons
            if(GUI.Button(new Rect(position.xMin, position.yMin, buttonSize, ySep - 2), "New")) {
                refId.intValue = RefID.GenerateId();
            }
            if(GUI.Button(new Rect(position.xMin + buttonSize, position.yMin, buttonSize, ySep - 2), "Empty")) {
                refId.intValue = 0;
            }
            if(GUI.Button(new Rect(position.xMin + buttonSize * 2, position.yMin, buttonSize, ySep - 2), "Save")) {
                EditorGUIUtility.systemCopyBuffer = refId.intValue.ToString();
                backup.intValue = refId.intValue;
            }
            if(GUI.Button(new Rect(position.xMin + buttonSize * 3, position.yMin, buttonSize, ySep - 2), "Load")) {
                refId.intValue = backup.intValue;
            }
    
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            Rect pos = new Rect(position.xMin, position.yMin + ySep, position.width, ySep - 2);
            EditorGUI.PropertyField(pos, refId, GUIContent.none);
    
            // End property
            EditorGUI.EndProperty();
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // Field height never changes, so ySep * 2 will always return the proper hight of the field
            return ySep * 2;
        }
    }
}