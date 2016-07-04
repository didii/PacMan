using UnityEngine;
using System.Collections;

public class LateFixedUpdateHack : MonoBehaviour {

    public static event System.Action Event;
    private bool _fixedUpdate;

    void Start() {
        // Create a couple of colliders to trigger OnTriggerStay each fixed update frame after the physics run
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
        if (Event != null) Event.Invoke();
    }
}
