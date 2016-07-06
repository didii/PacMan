using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IntersectionNode))]
public class ArrangeNodes : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var node = (IntersectionNode)target;
        if (GUILayout.Button("Arrange")) {
            node.ArrangeNodes(); // how do i call this?
        }
    }

}
