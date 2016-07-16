using System;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class IntersectionNode : MonoBehaviour {

    #region Fields
    
    public bool AllowUp, AllowRight, AllowDown, AllowLeft;
    public bool DotsUp, DotsRight, DotsDown, DotsLeft;
    public bool IsExitNode = false;

    [Header("Editor")]
    public Rect MaxSearchRect;
    [Range(0,.5f)]
    public float SearchWidth;

    public Sprite UpSprite, RightSprite, DownSprite, LeftSprite;
    public GameObject SpritePrefab;
    #endregion

    #region Properties

    public Vector2 Position {
        get { return transform.position; }
    }

    public HashSet<Utility.EDirection4> AllowedDirections {
        get {
            var result = new HashSet<Utility.EDirection4>();
            if (AllowUp)
                result.Add(Utility.EDirection4.Up);
            if (AllowRight)
                result.Add(Utility.EDirection4.Right);
            if (AllowDown)
                result.Add(Utility.EDirection4.Down);
            if (AllowLeft)
                result.Add(Utility.EDirection4.Left);
            return result;
        }
    }
    #endregion

    #region Unity methods
    // Use this for initialization
    void Start() {}

    // Update is called once per frame
    void Update() {}
    #endregion

    #region Methods

    public bool IsAllowed(Utility.EDirection4 dir) {
        if (dir == Utility.EDirection4.Up)
            return AllowUp;
        if (dir == Utility.EDirection4.Right)
            return AllowRight;
        if (dir == Utility.EDirection4.Down)
            return AllowDown;
        if (dir == Utility.EDirection4.Left)
            return AllowLeft;
        if (dir == Utility.EDirection4.None)
            return false;
        throw new InvalidEnumArgumentException();
    }

    /// <summary>
    /// Makes sure all nodes are aligned horizontal and vertical
    /// </summary>
    public void ArrangeNodes() {
        var nodes = GameObject.FindGameObjectsWithTag("Node");
        var processedNodesHorizontal = new List<GameObject>(nodes.Length);
        var processedNodesVertical = new List<GameObject>(nodes.Length);
        foreach (var node in nodes) {
            // Search horizontal
            if (!processedNodesHorizontal.Contains(node)) {
                var hHits = Physics2D.CircleCastAll(new Vector2(MaxSearchRect.xMin, node.transform.position.y),
                                                    SearchWidth,
                                                    Vector2.right,
                                                    MaxSearchRect.width,
                                                    1 << LayerMask.NameToLayer("Node"));
                var yPos = (float)Math.Round(hHits.Average(hit => hit.collider.transform.position.y), 2);
                foreach (var hNode in hHits) {
                    hNode.transform.position = new Vector2(hNode.transform.position.x, yPos);
                    processedNodesHorizontal.Add(hNode.transform.gameObject);
                }
            }

            // Search vertical
            if (!processedNodesVertical.Contains(node)) {
                var vHits = Physics2D.CircleCastAll(new Vector2(node.transform.position.x, MaxSearchRect.yMin),
                                                    SearchWidth,
                                                    Vector2.up,
                                                    MaxSearchRect.height,
                                                    1 << LayerMask.NameToLayer("Node"));
                var xPos = (float)Math.Round(vHits.Average(hit => hit.collider.transform.position.x), 2);
                foreach (var vNode in vHits) {
                    vNode.transform.position = new Vector2(xPos, vNode.transform.position.y);
                    processedNodesVertical.Add(vNode.transform.gameObject);
                }
            }
        }
        // Remove existing directional sprites
        foreach (var node in nodes) {
            foreach (var child in node.transform.GetChildren()) {
                DestroyImmediate(child.gameObject);
            }
        }

        // Add the directional sprites
        foreach (var node in nodes.Select(node => node.GetComponent<IntersectionNode>())) {
            var allowedDirs = node.AllowedDirections;
            foreach (var dir in allowedDirs) {
                var obj = (GameObject)Instantiate(SpritePrefab, Vector2.zero, Quaternion.identity);
                obj.transform.SetParent(node.transform, false);
                obj.layer = LayerMask.NameToLayer("Node");
                switch (dir) {
                    case Utility.EDirection4.None:
                        break;
                    case Utility.EDirection4.Up:
                        obj.GetComponent<SpriteRenderer>().sprite = UpSprite;
                        break;
                    case Utility.EDirection4.Right:
                        obj.GetComponent<SpriteRenderer>().sprite = RightSprite;
                        break;
                    case Utility.EDirection4.Down:
                        obj.GetComponent<SpriteRenderer>().sprite = DownSprite;
                        break;
                    case Utility.EDirection4.Left:
                        obj.GetComponent<SpriteRenderer>().sprite = LeftSprite;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public void AddAllowedMoveDecals() {
        
    }
    
    #endregion

    #region Events

    public void OnTrigger2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
            other.GetComponentInParent<Player>().OnNodeTrigger(this);
        else if (other.gameObject.tag == "Enemy") {
            if (IsExitNode) {
                if (!other.GetComponentInParent<Ghost>().OnExitNodeTrigger())
                    other.GetComponentInParent<Ghost>().OnNodeTrigger(this);
            } else
                other.GetComponentInParent<Ghost>().OnNodeTrigger(this);
        }
    }

    #endregion
}