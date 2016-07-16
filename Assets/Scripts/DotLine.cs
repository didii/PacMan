using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DotLine : MonoBehaviour {

    public LineSegment2D Line {
        get {
            var bounds = GetComponent<MeshRenderer>().bounds;
            var dir = bounds.extents.x > bounds.extents.y
                          ? Vector3.right : Vector3.up;
            var start = bounds.center - bounds.extents.Multiply(dir);
            var end = bounds.center + bounds.extents.Multiply(dir);
            return new LineSegment2D(start, end);
        }
    }

    public GameObject[] Nodes {
        get {
            var line = Line;
            return Physics2D.RaycastAll(line.Start, line.Direction, line.Length, 1 << LayerMask.NameToLayer("Node")) // Get all colliding nodes
                     .Select(o => o.transform.gameObject) // Select the GameObject
                     .Where(go => !(go.tag == "Player" || go.tag == "Enemy")) // Remove with tag Player and Enemy
                     .ToArray();
        }
    }

}
