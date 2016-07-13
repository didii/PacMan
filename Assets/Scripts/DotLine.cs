using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DotLine : MonoBehaviour {

    public LineSegment2D _line;
    public GameObject[] Nodes;

    public LineSegment2D Line {
        get { return _line; }
        set {
            _line = value;
            
        }
    }

    void Start() {
        FindCollidingNodes();
    }

    private void FindCollidingNodes() {
        Nodes = Physics2D.RaycastAll(_line.Start, _line.Direction, _line.Length, LayerMask.NameToLayer("Node")).Select(o => o.transform.gameObject).ToArray();
    }
}
