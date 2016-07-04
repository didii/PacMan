using UnityEngine;
using System.Collections;
using System.Linq;

public class Ghost : MonoBehaviour {

    public enum EBehavior {
        Random, StayInFrom, StayInBack, KeepDistance
    }

    #region Fields

    private NodeMovement _nodeMovement;
    #endregion

    // Use this for initialization
    void Start () {
        _nodeMovement = GetComponent<NodeMovement>();
        //_nodeMovement.CurrentDirection = Utility.EDirection.Down;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnNodeTrigger(IntersectionNode node) {
        var dirs = node.AllowedDirections;
        if (dirs.Contains(_nodeMovement.CurrentDirection.Opposite()) && dirs.Count > 1)
            dirs.Remove(_nodeMovement.CurrentDirection.Opposite());
        transform.position = node.Position;

        _nodeMovement.CurrentDirection = dirs.ElementAt((int)(Random.value*dirs.Count));
    }
}
