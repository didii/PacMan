using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class Mouse {

    #region Properties
    // Returns current mouse coordinates in screen coordinates
    static public Vector2 MouseScreenCoordinates {
        get { return Input.mousePosition; }
    }
    
    // 
    static public Vector2 ScrollDelta {
        get { return Input.mouseScrollDelta; }
    }
    #endregion

    #region Methods
    // Returns the position of the mouse in pixels
    static public Vector2 GetMouseScreenCoordinates() {
        return MouseScreenCoordinates;
    }

    // Returns the position of the mouse in world coordinates
    static public Vector3 GetMouseWorldCoordinates(Camera cam) {
        return cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
    }

    // Returns true if button is currently held down
    static public bool IsButtonDown(KeyCode button) {
        if (GetMouseButton(button) == -1)
            throw new ArgumentException("Only KeyCode.Mouse# is accepted here", "button");
        return Input.GetMouseButton(GetMouseButton(button));
    }

    // Return true if button is currently not down
    static public bool IsButtonUp(KeyCode button) {
        if (GetMouseButton(button) == -1)
            throw new ArgumentException("Only KeyCode.Mouse# is accepted here", "button");
        return !IsButtonDown(button);
    }

    // Returns true if the button was pressed this frame
    static public bool IsButtonPressed(KeyCode button) {
        if (GetMouseButton(button) == -1)
            throw new ArgumentException("Only KeyCode.Mouse# is accepted here", "button");
        return Input.GetMouseButtonDown(GetMouseButton(button));
    }

    // Returns true if the button was released this frame
    static public bool IsButtonReleased(KeyCode button) {
        if (GetMouseButton(button) == -1)
            throw new ArgumentException("Only KeyCode.Mouse# is accepted here", "button");
        return Input.GetMouseButtonUp(GetMouseButton(button));
    }
    #endregion

    #region Helper methods
    // Returns the number of the mouse button or -1 if not a mouse button
    static private int GetMouseButton(KeyCode button) {
        switch (button) {
            case KeyCode.Mouse0:
                return 0;
            case KeyCode.Mouse1:
                return 1;
            case KeyCode.Mouse2:
                return 2;
            case KeyCode.Mouse3:
                return 3;
            case KeyCode.Mouse4:
                return 4;
            case KeyCode.Mouse5:
                return 5;
            case KeyCode.Mouse6:
                return 6;
            default:
                return -1;
        }
    }
    #endregion
}
