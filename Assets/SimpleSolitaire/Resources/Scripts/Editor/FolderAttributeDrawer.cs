using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;

namespace SimpleSolitaire
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FolderAttribute : PropertyAttribute
    {
    }
    
    [CustomPropertyDrawer(typeof(FolderAttribute))]
    public class FolderAttributeDrawer : PropertyDrawer
    {
        private const int margin = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float currentViewWidth = GUILayoutUtility.GetLastRect().width;

            Rect labelPosition = position;
            labelPosition.width = EditorGUIUtility.labelWidth;

            position = EditorGUI.PrefixLabel(labelPosition, GUIUtility.GetControlID(FocusType.Passive), label);
            Rect objectPosition = position;
            objectPosition.x = labelPosition.width - (EditorGUI.indentLevel - 1) * margin;

            objectPosition.width = currentViewWidth - objectPosition.x - 2 + margin;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(objectPosition, "[Folder] only works with strings.", MessageType.Error);
            }
            else
            {
                Object folder = null;
                string path = null;

                if (!string.IsNullOrEmpty(property.stringValue))
                {
                    path = property.stringValue;
                    folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                    if (folder == null) 
                    {
                        Debug.LogWarning("The folder at path " + path + " could not be found.");
                        path = null;
                    }
                }

                folder = EditorGUI.ObjectField(objectPosition, folder, typeof(DefaultAsset), false);

                string folderPathInAssets = AssetDatabase.GetAssetPath(folder);
                TryUpdateProperty(folderPathInAssets, path, property);
            }

            EditorGUI.EndProperty();
        }

        private void TryUpdateProperty(string folderPathInAssets, string folderInfo, SerializedProperty property)
        {
            string appDataPath = Application.dataPath;
            string folderPathOnPC = appDataPath.Substring(0, appDataPath.LastIndexOf("/") + 1) + folderPathInAssets;

            if (!Directory.Exists(folderPathOnPC))
            {
                Debug.LogError(folderPathInAssets + " is not a folder.");
            }
            else
            {
                if (folderInfo == null || folderPathInAssets != folderInfo)
                {
                    property.stringValue = folderPathInAssets ?? "";
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
