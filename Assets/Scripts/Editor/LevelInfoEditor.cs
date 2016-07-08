using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelInfo))]
public class LevelInfoEditor : Editor {
    #region Fields

    private LevelInfo _self;
    private float _dotSpacing = 5;
    #endregion

    #region Events

    public override void OnInspectorGUI() {
        this.DrawDefaultInspector();

        if (GUILayout.Button("Insert DotLines")) {
            _self = (LevelInfo)target;
            AddDotLines();
        }
        if (GUILayout.Button("Place Dots")) {
            _self = (LevelInfo)target;
            AddDotLines();
        }
    }

    /// <summary>
    /// Adds DotLines onto the scene based on the location of the nodes
    /// </summary>
    public void AddDotLines() {
        _self.Start();
        // Removing previously placed dotlines
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("DotLine")) {
            DestroyImmediate(gameObject);
        }

        // Place all possible dotlines
        Debug.Log("Placing DotLines");
        var parent = new GameObject("DotLine");
        parent.tag = "DotLine";
        foreach (var line in _self.NodeConnections) {
            var obj = PrefabUtility.InstantiatePrefab(_self.DotLinePrefab) as GameObject;
            if (obj == null) {
                Debug.LogError("Could not assign properties to instantiated " + _self.DotLinePrefab.name);
                continue;
            }

            var bounds = obj.GetComponent<MeshRenderer>().bounds;
            var trans = obj.transform;
            trans.position = line.Center;
            trans.SetParent(parent.transform, true);
            var dir = line.ToDirection2();
            Vector3 scale = Vector3.one;
            switch (dir) {
            case Utility.EDirection2.Horizontal:
                scale.x = (line.End.x - line.Center.x) / bounds.extents.x;
                scale.y = 0.25f;
                break;
            case Utility.EDirection2.Vertical:
                scale.x = 0.25f;
                scale.y = (line.End.y - line.Center.y) / bounds.extents.y;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
            trans.localScale = scale;
        }
    }

    /// <summary>
    /// Adds dots onto the dotlines, ensuring there is always a dot on a node.
    /// </summary>
    public void AddDots() {
        // Iterate over all existing DotLines
        var dotLines = GameObject.FindGameObjectsWithTag("DotLine");
        foreach (var dotLine in dotLines) {
            // Convert to line segment
            var line = dotLine.GetComponent<MeshRenderer>().bounds;
            // First find every node on it
            //Physics2D.RaycastAll()
        }
    }

    #endregion
}
