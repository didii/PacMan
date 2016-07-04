using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var controller = (GameController)target;
        if (GUILayout.Button("SetLives")) {
            controller.SetLivesIndicator();
        }
        if (GUILayout.Button("DoRandomStuff"))
            controller.DoRandomStuff();
    }
}
