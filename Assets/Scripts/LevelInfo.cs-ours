using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;


public class LevelInfo : MonoBehaviour {

    #region Fields

    private Bounds _levelSize;
    private HashSet<GameObject> _nodes;
    private HashSet<LineSegment2D> _nodeConnections;

    [Header("Editor")] public GameObject DotLinePrefab;
    #endregion

    #region Properties

    public Bounds LevelSize {
        get { return _levelSize; }
    }

    public GameObject[] Nodes {
        get { return _nodes.ToArray(); }
    }

    public LineSegment2D[] NodeConnections {
        get { return _nodeConnections.ToArray(); }
    }
    #endregion

    // Use this for initialization
    public void Start() {
        _levelSize = GetComponent<SpriteRenderer>().bounds;

        // Get all intersection nodes
        _nodes = new HashSet<GameObject>(transform.GetAllChildren(child => child.tag == "Node")
                                                  .Select(child => child.gameObject));

        // Get all connections between nodes
        _nodeConnections = new HashSet<LineSegment2D>();
        GetNodeConnections();
    }

    // Update is called once per frame
    void Update() {
        
    }


    #region Helper methods

    private void GetNodeConnections() {
        // Clear the existing list
        _nodeConnections.Clear();
        // Get all nodes as IntersectionNodes
        var nodes = _nodes.Select(node => node.GetComponent<IntersectionNode>()).ToArray();
        // Setup some vars to reduce multiple checks
        var checkedHorizontalNodes = new List<IntersectionNode>();
        var checkedVerticalNodes = new List<IntersectionNode>();

        // Start looping over all nodes
        foreach (var sNode in nodes) {
            // Setup some variables
            IntersectionNode[] matchNodes;
            Vector3? pos = null;

            // Check if node was already vertically processed
            if (!checkedVerticalNodes.Contains(sNode)) {
                // Do a raycast up over the size of the level, get the nodes and sort them according to their y-value
                matchNodes = Physics2D.RaycastAll(new Vector2(sNode.transform.position.x, _levelSize.min.y),
                                                  Vector2.up,
                                                  _levelSize.size.y,
                                                  1 << LayerMask.NameToLayer("Node"))
                                      .Where(hit => hit.transform.tag == "Node")
                                      .OrderBy(hit => hit.transform.position.y)
                                      .Select(node => node.transform.GetComponent<IntersectionNode>())
                                      .ToArray();
                // Add valid lines between the nodes
                foreach (var node in matchNodes) {
                    if (!pos.HasValue && node.AllowUp)
                        pos = node.transform.position;
                    else if (pos.HasValue && !node.AllowUp) {
                        _nodeConnections.Add(new LineSegment2D(pos.Value, node.Position));
                        pos = null;
                    }
                    // Add every node to checked list
                    checkedVerticalNodes.Add(node);
                }
            }

            // Check if node was already horizontally processed
            if (!checkedHorizontalNodes.Contains(sNode)) {
                // Do the same with a raycast to the right over the level and sort according to their x-value
                matchNodes = Physics2D.RaycastAll(new Vector2(_levelSize.min.x, sNode.transform.position.y),
                                                  Vector2.right,
                                                  _levelSize.size.x,
                                                  1 << LayerMask.NameToLayer("Node"))
                                      .Where(hit => hit.transform.tag == "Node")
                                      .OrderBy(hit => hit.transform.position.x)
                                      .Select(node => node.transform.GetComponent<IntersectionNode>())
                                      .ToArray();
                // Add valid lines between the nodes
                foreach (var node in matchNodes) {
                    if (!pos.HasValue && node.AllowRight)
                        pos = node.transform.position;
                    else if (pos.HasValue && !node.AllowRight) {
                        _nodeConnections.Add(new LineSegment2D(pos.Value, node.Position));
                        pos = null;
                    }
                    checkedHorizontalNodes.Add(node);
                }
            }
        }
    }

    private void DrawNodeConnections() {
        foreach (var line in _nodeConnections)
            line.Draw();
    }

    #endregion

    #region Editor methods

    #endregion
}
