using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IntersectionNode))]
public class IntersectionNodeEditor : Editor {
    //IntersectionNode _instance;
    //private PropertyField[] _fields;

    public void OnEnable() {
        //_instance = target as IntersectionNode;
        //_fields = ExposeProperties.GetProperties(_instance);
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var node = (IntersectionNode)target;
        if (GUILayout.Button("Arrange")) {
            node.ArrangeNodes();
            node.AddAllowedMoveDecals();
        }
    }
}