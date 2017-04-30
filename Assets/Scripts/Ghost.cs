using System;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class Ghost : MonoBehaviour {

    /// <summary>
    /// Different types of behaviour for the different ghosts
    /// </summary>
    public enum EBehavior {
        Random, StayInFront, StayBehind, KeepDistance
    }

    /// <summary>
    /// Scatter of chase state (scatter = random, chasing is from <see cref="EBehavior"/>
    /// </summary>
    public enum EState {
        Chasing, Scatter
    }

    #region Fields
    /// <summary>
    /// Adapts the chance to get out a bit
    /// </summary>
    public static int ExitCount = 1;

    public EBehavior Behavior = EBehavior.Random;
    public EState State;
    private MoveQueue _moveQueue;
    #endregion

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        _moveQueue = GetComponent<MoveQueue>();
    }
	
    /// <summary>
    /// If on an exit node, there is a smaller chance to go up
    /// </summary>
    /// <returns></returns>
    public bool OnExitNodeTrigger() {
        if (Random.value > .25/ExitCount)
            return false;

        _moveQueue.CurrentDirection = Utility.EDirection4.Up;
        ExitCount++;
        return true;
    }

    /// <summary>
    /// Behaviour when on <see cref="IntersectionNode"/>
    /// </summary>
    /// <param name="node"></param>
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

    /// <summary>
    /// When on a node, go to a randomly allowed direction except backwards
    /// </summary>
    /// <param name="node"></param>
    private void OnNodeTriggerRandom(IntersectionNode node) {
        var dirs = node.AllowedDirections;
        if (dirs.Contains(_moveQueue.CurrentDirection.Opposite()) && dirs.Count > 1)
            dirs.Remove(_moveQueue.CurrentDirection.Opposite());
        transform.position = node.Position;

        _moveQueue.CurrentDirection = dirs.ElementAt((int)(Random.value * dirs.Count));
    }

    /// <summary>
    /// When on a node, try to get in front of the player
    /// </summary>
    /// <param name="node"></param>
    private void OnNodeTriggerInFront(IntersectionNode node) {
        
    }

    /// <summary>
    /// When on a node, try to get behind the player
    /// </summary>
    /// <param name="node"></param>
    private void OnNodeTriggerBehind(IntersectionNode node) {
        
    }

    /// <summary>
    /// When on a node, keep at a certain distance of the player
    /// </summary>
    /// <param name="node"></param>
    private void OnNodeTriggerDistance(IntersectionNode node) {
        
    }
}
    