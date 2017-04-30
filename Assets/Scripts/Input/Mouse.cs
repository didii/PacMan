using System;
using UnityEngine;

public static class Mouse {

    #region Properties
    /// <summary>
    /// Returns current mouse coordinates in screen coordinates
    /// </summary>
    static public Vector2 MouseScreenCoordinates {
        get { return Input.mousePosition; }
    }
    
    /// <summary>
    /// Returns the amount of scrolls the mouse has done since the last update
    /// </summary>
    static public Vector2 ScrollDelta {
        get { return Input.mouseScrollDelta; }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns the position of the mouse in pixels
    /// </summary>
    /// <returns></returns>
    static public Vector2 GetMouseScreenCoordinates() {
        return MouseScreenCoordinates;
    }

    /// <summary>
    /// Returns the position of the mouse in world coordinates
    /// </summary>
    /// <param name="cam"></param>
    /// <returns></returns>
    static public Vector3 GetMouseWorldCoordinates(Camera cam) {
        return cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
    }

    /// <summary>
    /// Returns true if button is currently held down
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    static public bool IsButtonDown(KeyCode button) {
        CheckKeyCodeForMouse(button);
        return Input.GetMouseButton(GetMouseButton(button));
    }

    /// <summary>
    /// Return true if button is currently not down
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    static public bool IsButtonUp(KeyCode button) {
        CheckKeyCodeForMouse(button);
        return !IsButtonDown(button);
    }

    /// <summary>
    /// Returns true if the button was pressed this frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    static public bool IsButtonPressed(KeyCode button) {
        CheckKeyCodeForMouse(button);
        return Input.GetMouseButtonDown(GetMouseButton(button));
    }

    /// <summary>
    /// Returns true if the button was released this frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    static public bool IsButtonReleased(KeyCode button) {
        CheckKeyCodeForMouse(button);
        return Input.GetMouseButtonUp(GetMouseButton(button));
    }
    #endregion

    #region Helper methods
    private static void CheckKeyCodeForMouse(KeyCode button) {
        if (GetMouseButton(button) == -1)
            throw new ArgumentException("Only KeyCode.Mouse# is accepted here", "button");
    }

    /// <summary>
    /// Returns the number of the mouse button or -1 if not a mouse button
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
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
