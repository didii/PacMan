using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelInfo))]
public class LevelInfoEditor : Editor {
    #region Fields

    public GameObject Tst;
    #endregion

    #region Events

    public override void OnInspectorGUI() {
        this.DrawDefaultInspector();

        var info = (LevelInfo)target;
        if (GUILayout.Button("Insert DotLines")) {
            info.AddDotLines();
        }
    }
    #endregion
}
