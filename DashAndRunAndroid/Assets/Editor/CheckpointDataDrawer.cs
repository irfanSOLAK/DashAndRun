using UnityEditor;
using UnityEngine;

#region olmadý mk

[CustomPropertyDrawer(typeof(CheckpointData))]
public class CheckpointDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Properties
        SerializedProperty checkPointProp = property.FindPropertyRelative("CheckPoint");
        SerializedProperty isActiveProp = property.FindPropertyRelative("IsActive");

        // Create a horizontal layout
        EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        // Calculate the widths for the fields
        float checkPointWidth = contentPosition.width * 0.7f; // GameObject alaný için geniþlik
        float isActiveWidth = contentPosition.width * 0.25f; // Bool alaný için geniþlik

        // Draw headers
        //    Rect headerRect = new Rect(contentPosition.x + checkPointWidth / 2 - EditorGUIUtility.fieldWidth, contentPosition.y - EditorGUIUtility.singleLineHeight, checkPointWidth, EditorGUIUtility.singleLineHeight);
        //      EditorGUI.LabelField(headerRect, "Check Points", EditorStyles.boldLabel);

        Rect checkPointRect = new Rect(contentPosition.x, contentPosition.y, checkPointWidth, contentPosition.height);
        EditorGUI.PropertyField(checkPointRect, checkPointProp, GUIContent.none);

        // Draw Bool field at the far right
        Rect isActiveRect = new Rect(contentPosition.x + contentPosition.width - isActiveWidth, contentPosition.y, isActiveWidth, contentPosition.height);
        EditorGUI.PropertyField(isActiveRect, isActiveProp, GUIContent.none);

        // Draw Active header
        //    Rect activeHeaderRect = new Rect(contentPosition.x + contentPosition.width - isActiveWidth, contentPosition.y - EditorGUIUtility.singleLineHeight, isActiveWidth, EditorGUIUtility.singleLineHeight);
        //    EditorGUI.LabelField(activeHeaderRect, "Active", EditorStyles.boldLabel);

        EditorGUI.EndProperty();
    }
}

#endregion