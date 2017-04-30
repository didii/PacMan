using UnityEngine;

public static class Keyboard {
    #region Properties
    /// <summary>
    ///     Returns true is any key is currently held
    /// </summary>
    public static bool IsAnyKeyDown {
        get { return Input.anyKey; }
    }

    /// <summary>
    ///     Returns true if any key was pressed this frame
    /// </summary>
    public static bool IsAnyKeyPressed {
        get { return Input.anyKeyDown; }
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Returns true if the key is currently held down
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyDown(KeyCode key) {
        return Input.GetKey(key);
    }

    /// <summary>
    ///     Returns true if the key is currently not held down
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyUp(KeyCode key) {
        return !IsKeyDown(key);
    }

    /// <summary>
    ///     Returns true if key was pressed this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyPressed(KeyCode key) {
        return Input.GetKeyDown(key);
    }

    /// <summary>
    ///     Returns true if key was released this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyReleased(KeyCode key) {
        return Input.GetKeyUp(key);
    }
    #endregion
}
