using UnityEngine;
using System.Collections;

public static class Keyboard {

    #region Properties
    // Returns true is any key is currently held
    public static bool IsAnyKeyDown {
        get { return Input.anyKey; }
    }

    // Returns true if any key was pressed this frame
    public static bool IsAnyKeyPressed {
        get { return Input.anyKeyDown; }
    }
    #endregion

    #region Methods
    // Returns true if the key is currently held down
    static public bool IsKeyDown(KeyCode key) {
        return Input.GetKey(key);
    }

    // Returns true if the key is currently not held down
    static public bool IsKeyUp(KeyCode key) {
        return !IsKeyDown(key);
    }

    // Returns true if key was pressed this frame
    static public bool IsKeyPressed(KeyCode key) {
        return Input.GetKeyDown(key);
    }

    // Returns true if key was released this frame
    static public bool IsKeyReleased(KeyCode key) {
        return Input.GetKeyUp(key);
    }
    #endregion
}
