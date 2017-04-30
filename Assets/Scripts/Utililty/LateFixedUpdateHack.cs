using UnityEngine;

/// <summary>
/// Allows for a LateFixedUpdate call on every Monobehavior which technically doesn't exist.
/// </summary>
/// <remarks>
/// <para>It triggers a collision every physics frame which for Unity is handled after the physics
/// update. Source: http://docs.unity3d.com/Manual/ExecutionOrder.html </para>
/// 
/// <para>Usage:</para>
/// 
/// <para>Add an object with this script attached to the scene. I usually add it to the
/// GameController. This will now broadcast a message to all GameObjects in the scene, triggering
/// the function LateFixedUpdate(). Simply implement this function on any MonoBehaviour to trigger
/// it.</para>
/// </remarks>
public class LateFixedUpdateHack : MonoBehaviour {

    private static bool _fixedUpdate;

    void Start() {
        // Create a couple of colliders to trigger OnTriggerStay each fixed update frame after the
        // physics run
        gameObject.AddComponent<BoxCollider>().isTrigger = true;

        var other = new GameObject();
        other.AddComponent<BoxCollider>();
        other.AddComponent<Rigidbody>().isKinematic = true;
        other.transform.parent = transform;
        other.transform.position = transform.position;
        other.name = "LateFixedUpdateHack";
    }

    void FixedUpdate() {
        _fixedUpdate = true;
    }

    void OnTriggerStay() {
        // Multiple OnTriggerStay could be called if there is another collider
        if (!_fixedUpdate)
            return;
        _fixedUpdate = false;

        // Get all GameObjects
        var allObj = FindObjectsOfType<GameObject>();
        foreach (var obj in allObj)
            // Call LateFixedUpdate on all of them
            obj.SendMessage("LateFixedUpdate", SendMessageOptions.DontRequireReceiver);
    }
}
