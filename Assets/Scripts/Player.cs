using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Fields

    public float JumpDistance;
    public LevelInfo LevelInfo;
    public GameInfo GameInfo;

    private Vector2? _jumpPos;
    private Animator _animator;
    private NodeMovement _nodeMovement;
    private IntersectionNode _collidingNode;
    private bool _fire1Pressed;
    #endregion

    #region Properties

    private bool _jumping {
        get { return _jumpPos.HasValue; }
    }
    #endregion

    #region Initialisation Methods
    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        _nodeMovement = GetComponent<NodeMovement>();
        _animator = GetComponent<Animator>();
        _animator.Play("PlayerRight");
        _animator.enabled = false;

        _nodeMovement.DirectionChanged += OnDirectionChange;
    }

    #endregion

    #region Runtime Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (Input.GetButtonDown("Up") && (_collidingNode == null || _collidingNode.AllowUp))
            _nodeMovement.NextDirection = Utility.EDirection4.Up;
        else if (Input.GetButtonDown("Right") && (_collidingNode == null || _collidingNode.AllowRight))
            _nodeMovement.NextDirection = Utility.EDirection4.Right;
        else if (Input.GetButtonDown("Down") && (_collidingNode == null || _collidingNode.AllowDown))
            _nodeMovement.NextDirection = Utility.EDirection4.Down;
        else if (Input.GetButtonDown("Left") && (_collidingNode == null || _collidingNode.AllowLeft))
            _nodeMovement.NextDirection = Utility.EDirection4.Left;

        if (_nodeMovement.CurrentDirection != Utility.EDirection4.None)
            _collidingNode = null;
    }

    #endregion

    #region Events

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy")
            Destroy(this.gameObject);
        if (other.tag == "Dot") {
            Destroy(other.gameObject);
            GameInfo.Score += 10;
        }
    }

    void OnDirectionChange(Utility.EDirection4 dir) {
        var animator = GetComponent<Animator>();
        switch (dir) {
        case Utility.EDirection4.None:
            animator.enabled = false;
            break;
        case Utility.EDirection4.Up:
            animator.enabled = true;
            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
            break;
        case Utility.EDirection4.Right:
            animator.enabled = true;
            transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            break;
        case Utility.EDirection4.Down:
            animator.enabled = true;
            transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
            break;
        case Utility.EDirection4.Left:
            animator.enabled = true;
            transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
            break;
        default:
            throw new ArgumentOutOfRangeException("dir", dir, null);
        }
    }

    #endregion

    #region Methods

    public void OnNodeTrigger(IntersectionNode node) {
        // Check if next direction is allowed
        if (_nodeMovement.NextDirection != Utility.EDirection4.None && node.IsAllowed(_nodeMovement.NextDirection)) {
            // Set correct position and update movement
            transform.position = node.Position;
            _nodeMovement.CurrentDirection = _nodeMovement.NextDirection;
            //if (_nodeMovement.CurrentDirection != Utility.EDirection.None)
            //    _collidingNode = null;
        }
        // Check if current direction is allowed
        else if (!node.IsAllowed(_nodeMovement.CurrentDirection)) {
            //GetComponent<Rigidbody2D>().position = node.Position;
            transform.position = node.Position;
            _nodeMovement.CurrentDirection = Utility.EDirection4.None;
            _collidingNode = node;
        }
    }

    public void OnTeleportDissapear() {
        if (_jumpPos.HasValue)
            transform.position = _jumpPos.Value;
    }

    public void OnTeleportAnimationEnd() {
        _nodeMovement.Resume();
        _jumpPos = null;
    }

    #endregion

    #region Helper Methods
    
    private void Jump(Utility.EDirection4 dir) {
        // Check if jumping the distance does the job
        var endPos = (Vector2)transform.position + dir.ToVector2()*JumpDistance;
        foreach (var connection in LevelInfo.NodeConnections) {

            if (connection.IsIntersectingWith(new LineSegment2D(endPos-Vector2.one*.01f, endPos+Vector2.one*.01f))) {
                _jumpPos = endPos;
                _animator.enabled = true;
                _animator.Play("PlayerTeleport");
                return;
            }
        }

        // Jump over a wall
        LineSegment2D jumpLine;
        List<LineSegment2D> validLines;
        Vector3 newPos;
        switch (dir) {
        case Utility.EDirection4.Up:
            // The line to move over for the jump
            jumpLine = new LineSegment2D(transform.position, new Vector2(transform.position.x, LevelInfo.LevelSize.max.y));
            // Get all lines intersecting with it, sorted according to y-value
            validLines =
                LevelInfo.NodeConnections.Where(
                    line => (Line2D)line != (Line2D)jumpLine && line.IsIntersectingWith(jumpLine) &&
                            // ReSharper disable PossibleInvalidOperationException
                            line.GetIntersectionWith(jumpLine).Value.y > transform.position.y)
                         .OrderBy(line => line.GetIntersectionWith(jumpLine).Value.y)
                         // ReSharper restore PossibleInvalidOperationException
                         .ToList();

            // Jump to new position
            newPos = new Vector3(transform.position.x, validLines.First().Start.y, transform.position.z);
            break;

        case Utility.EDirection4.Right:
            // The line to move over for the jump
            jumpLine = new LineSegment2D(transform.position, new Vector2(LevelInfo.LevelSize.max.x, transform.position.y));
            // Get all lines intersecting with it, sorted according to y-value
            validLines =
                LevelInfo.NodeConnections.Where(
                    line => (Line2D)line != (Line2D)jumpLine && line.IsIntersectingWith(jumpLine) &&
                            // ReSharper disable PossibleInvalidOperationException
                            line.GetIntersectionWith(jumpLine).Value.x > transform.position.x)
                         .OrderBy(line => line.GetIntersectionWith(jumpLine).Value.x)
                         // ReSharper restore PossibleInvalidOperationException
                         .ToList();

            // Jump to new position
            newPos = new Vector3(validLines.First().Start.x, transform.position.y, transform.position.z);
            break;

        case Utility.EDirection4.Down:
            // The line to move over for the jump
            jumpLine = new LineSegment2D(transform.position, new Vector2(transform.position.x, LevelInfo.LevelSize.min.y));
            // Get all lines intersecting with it, sorted according to y-value
            validLines =
                LevelInfo.NodeConnections.Where(
                    line => (Line2D)line != (Line2D)jumpLine && line.IsIntersectingWith(jumpLine) &&
                            // ReSharper disable PossibleInvalidOperationException
                            line.GetIntersectionWith(jumpLine).Value.y < transform.position.y)
                         .OrderByDescending(line => line.GetIntersectionWith(jumpLine).Value.y)
                         // ReSharper restore PossibleInvalidOperationException
                         .ToList();

            // Jump to new position
            newPos = new Vector3(transform.position.x, validLines.First().Start.y, transform.position.z);
            break;

        case Utility.EDirection4.Left:
            // The line to move over for the jump
            jumpLine = new LineSegment2D(transform.position, new Vector2(LevelInfo.LevelSize.min.x, transform.position.y));
            // Get all lines intersecting with it, sorted according to y-value
            validLines =
                LevelInfo.NodeConnections.Where(
                    line => (Line2D)line != (Line2D)jumpLine && line.IsIntersectingWith(jumpLine) &&
                            // ReSharper disable PossibleInvalidOperationException
                            line.GetIntersectionWith(jumpLine).Value.x < transform.position.x)
                         .OrderByDescending(line => line.GetIntersectionWith(jumpLine).Value.x)
                         // ReSharper restore PossibleInvalidOperationException
                         .ToList();

            // Jump to new position
            newPos = new Vector3(validLines.First().Start.x, transform.position.y, transform.position.z);
            break;
        default:
            throw new ArgumentOutOfRangeException("dir", dir, null);
        }
        _jumpPos = newPos;
        _animator.enabled = true;
        _animator.Play("PlayerTeleport");
    }

    #endregion
}