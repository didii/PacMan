using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HereIsAButton))]
class HereIsAButtonEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var obj = (HereIsAButton)target;
        if (GUILayout.Button("Do"))
            obj.Do();
    }
}
