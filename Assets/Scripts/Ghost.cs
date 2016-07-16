using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

public class Ghost : MonoBehaviour {

    public enum EBehavior {
        Random, StayInFront, StayBehind, KeepDistance
    }

    public enum EState {
        Chasing, Scatter
    }

    #region Fields

    public static int ExitCount = 1;

    public EBehavior Behavior = EBehavior.Random;
    public EState State;
    private NodeMovement _nodeMovement;
    #endregion

    // Use this for initialization
    void Start () {
        _nodeMovement = GetComponent<NodeMovement>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public bool OnExitNodeTrigger() {
        if (Random.value > .25/ExitCount)
            return false;

        _nodeMovement.CurrentDirection = Utility.EDirection4.Up;
        ExitCount++;
        return true;
    }

    public void OnNodeTrigger(IntersectionNode node) {
        switch (Behavior) {
        case EBehavior.Random:
            OnNodeTriggerRandom(node);
            break;
        case EBehavior.StayInFront:
            OnNodeTriggerInFront(node);
            break;
        case EBehavior.StayBehind:
            OnNodeTriggerBehind(node);
            break;
        case EBehavior.KeepDistance:
            OnNodeTriggerDistance(node);
            break;
        default:
            throw new ArgumentOutOfRangeException();
        }
    }

    private void OnNodeTriggerRandom(IntersectionNode node) {
        var dirs = node.AllowedDirections;
        if (dirs.Contains(_nodeMovement.CurrentDirection.Opposite()) && dirs.Count > 1)
            dirs.Remove(_nodeMovement.CurrentDirection.Opposite());
        transform.position = node.Position;

        _nodeMovement.CurrentDirection = dirs.ElementAt((int)(Random.value * dirs.Count));
    }

    private void OnNodeTriggerInFront(IntersectionNode node) {
        
    }

    private void OnNodeTriggerBehind(IntersectionNode node) {
        
    }

    private void OnNodeTriggerDistance(IntersectionNode node) {
        
    }
}
    