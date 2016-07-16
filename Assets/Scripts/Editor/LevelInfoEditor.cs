using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelInfo))]
public class LevelInfoEditor : Editor {
    #region Fields

    private LevelInfo _self;
    #endregion

    #region Events

    public override void OnInspectorGUI() {
        this.DrawDefaultInspector();
        _self = (LevelInfo)target;
        if (GUILayout.Button("Insert DotLines"))
            AddDotLines();
        if (GUILayout.Button("Place Dots"))
            AddDots();
        if (GUILayout.Button("Remove Dots"))
            RemoveDots();
    }

    /// <summary>
    /// Adds DotLines onto the scene based on the location of the nodes
    /// </summary>
    private void AddDotLines() {
        _self.Start();
        RemoveDotLines();
        // Create empty parent
        var parent = new GameObject("DotLines");
        // Iterate over all node-connections
        foreach (var line in _self.NodeConnections) {
            // Instantiate it
            var obj = PrefabUtility.InstantiatePrefab(_self.DotLinePrefab) as GameObject;
            if (obj == null) {
                Debug.LogError("Could not assign properties to instantiated " + _self.DotLinePrefab.name);
                continue;
            }
            // Set its parent
            obj.transform.SetParent(parent.transform, true);
            // Set its location
            obj.transform.position = line.Center;

            // Set scale to match width or height
            var currBounds = obj.GetComponent<MeshRenderer>().bounds;
            var length = line.Length + 0.05f;
            var boundsSize = new Vector3(line.ToDirection2() == Utility.EDirection2.Horizontal ? length : 0.25f,
                                         line.ToDirection2() == Utility.EDirection2.Vertical ? length : 0.25f,
                                         0.25f);
            var newBounds = new Bounds(line.Center, boundsSize);
            obj.transform.localScale = Utility.GetScaleFromBounds(currBounds, newBounds);

            // Do some more stuff
            //obj.GetComponent<DotLine>().Line = new LineSegment2D(line);
            //obj.GetComponent<DotLine>().FindCollidingNodes();
        }
    }

    /// <summary>
    /// Adds dots onto the dotlines, ensuring there is always a dot on a node.
    /// </summary>
    private void AddDots() {
        RemoveDots();

        // Create parent
        var parent = new GameObject("Dots");
        // Iterate over all existing DotLines
        var dotLines = GameObject.FindGameObjectsWithTag("DotLine");
        foreach (var dotLine in dotLines) {
            // Convert to line segment
            var nodes = dotLine.GetComponent<DotLine>().Nodes.Select(o => o.transform.position).ToArray();
            // Iterate over the nodes and place dots in-between
            Vector3? node1 = null;
            foreach (var node in nodes) {
                var node2 = node1;
                node1 = node;
                if (node2 == null)
                    continue; // skip first iteration
                
                // Calculate number of dots
                var dist = Mathf.Abs((node2.Value - node1.Value).magnitude);
                var num = Mathf.RoundToInt(dist / _self.DotSpacing);
                var spacing = dist / num;

                // Place all dots between the nodes
                for (int i = 0; i <= num; i++) {
                    // Position of a dot
                    var pos = node1.Value + (Vector3)new LineSegment2D(node1.Value, node2.Value).Direction * spacing * i;
                    // If a dot already exists here
                    if (GameObject.FindGameObjectsWithTag("Dot").Count(go => go.transform.position == pos) > 0)
                        continue; // skip
                    // Create the dot
                    var dot = PrefabUtility.InstantiatePrefab(_self.DotPrefab) as GameObject;
                    if (dot == null) {
                        Debug.LogError("Could not instantiate a dot");
                        continue;
                    }
                    // Set its position and parent
                    dot.transform.position = pos;
                    dot.transform.SetParent(parent.transform, false);
                }
            }
        }
        // Disbale the existing dot lines
        GameObject.Find("DotLines").SetActive(false);
    }
    #endregion

    public void RemoveDotLines() {
        DestroyImmediate(GameObject.Find("DotLines"));
    }

    public void RemoveDots() {
        DestroyImmediate(GameObject.Find("Dots"));
    }
}
