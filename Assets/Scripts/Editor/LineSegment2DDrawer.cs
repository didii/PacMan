using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(LineSegment2D))]
public class LineSegment2DDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        var tmp = EditorGUI.BeginProperty(position, label, property);
        tmp.text = "BLABLABLA";
        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects
        Rect startRect = new Rect(position.x, position.y, position.width/2, position.height);
        Rect endRect = new Rect(position.x + position.width/2, position.y, position.width/2, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        //TODO: properties do not work, private fields do not work
        EditorGUI.PropertyField(startRect, property.FindPropertyRelative("_start"), GUIContent.none);
        EditorGUI.PropertyField(endRect, property.FindPropertyRelative("_end"), GUIContent.none);

        EditorGUI.EndProperty();
    }

}
