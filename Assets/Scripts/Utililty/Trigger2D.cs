using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Makes sure colliding always happens for very small objects by doing a raycast over the travelled path.
/// </summary>
public class Trigger2D : MonoBehaviour {

    #region Fields
    //public LayerMask LayerMask;
    public bool AlwaysCheck = true;
    public float SkinWidth = 0.1f; //probably doesn't need to be changed 

    private float _minimumExtent;
    private float _partialExtent;
    private Vector2 _previousPosition;
    private Collider2D _collider;

    private int _lastCollideFrame = 0;
    private List<GameObject> _lastCollidedWith = new List<GameObject>();

    #endregion

    #region Initialization
    /// <summary>
    /// Initialize some values
    /// </summary> 
    void Start() {
        _collider = GetComponent<Collider2D>();
        _previousPosition = transform.position;
        _minimumExtent = Mathf.Min(_collider.bounds.extents.x, _collider.bounds.extents.y);
        _partialExtent = _minimumExtent * (1.0f - SkinWidth);
    }
    #endregion

    #region Runtime Methods

    void FixedUpdate() {
        // Update last collided with variables
        bool collidedLastFrame = _lastCollideFrame + 1 == GameController.FixedFrameCount;
        var lastCollidedWith = _lastCollidedWith;

        //have we moved more than our minimum extent? 
        Vector2 movementThisStep = (Vector2)transform.position - _previousPosition;
        float movementMagnitude = movementThisStep.magnitude;
        if (AlwaysCheck || Math.Abs(movementMagnitude) > Math.Abs(_minimumExtent)) {

            // Get the layer collision mask
            int layerMask = Utility.GetCollisionMask2D(gameObject.layer);

            //check for all obstructions we might have missed 
            var hitInfo = Physics2D.RaycastAll(_previousPosition, movementThisStep, movementMagnitude, layerMask);
            bool needReset = true;
            foreach (var hit in hitInfo) {
                // Check if object has collider and is not itself
                if (hit.collider == null || hit.collider == _collider)
                    continue;

                // If valid, check if _last* need a reset
                if (needReset) {
                    _lastCollideFrame = GameController.FixedFrameCount;
                    _lastCollidedWith = new List<GameObject>();
                    needReset = false;
                }
                // Add the collider to last collided with list
                _lastCollidedWith.Add(hit.collider.gameObject);

                // If collided with it last frame
                if (collidedLastFrame && lastCollidedWith.Contains(hit.collider.gameObject))
                    continue; // Skip

                // If collider is set as trigger, send trigger
                if (hit.collider.isTrigger) {
                    hit.collider.SendMessage("OnTrigger2D", _collider, SendMessageOptions.DontRequireReceiver);
                } // Not yet checked 
                if (!hit.collider.isTrigger)
                    transform.position = hit.point - movementThisStep/movementMagnitude*_partialExtent;
            }
        }
        _previousPosition = transform.position;
    }
    #endregion
}