using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
class TestEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var obj = (Test)target;
        if (GUILayout.Button("Do"))
            obj.Do();
    }
}
