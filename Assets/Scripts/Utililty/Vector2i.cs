using System.Collections.Generic;

/// <summary>
///     Simple vector class that holds integers instead of floats
/// </summary>
public class Vector2i {
    /// <summary>
    ///     The X-value of this vector
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The Y-value of this vector
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    ///     Default constructor. Sets <see cref="X"/> and <see cref="Y"/> to 0.
    /// </summary>
    public Vector2i() {
        X = 0;
        Y = 0;
    }

    /// <summary>
    ///     Constructor to immediatly set the values of <see cref="X"/> and <see cref="Y"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Vector2i(int x, int y) {
        X = x;
        Y = y;
    }
}

/// <summary>
///     Rule to compare <see cref="Vector2i"/>.
/// </summary>
public class Vector2iComparer : IEqualityComparer<Vector2i> {
    /// <summary>
    ///     Equals both vectors where the x and y components must be exactly the same.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public bool Equals(Vector2i v1, Vector2i v2) {
        if (v1 == null && v2 == null)
            return true;
        if (v1 == null || v2 == null)
            return false;
        return v1.X == v2.X && v1.Y == v2.Y;
    }

    /// <summary>
    ///     Not used
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public int GetHashCode(Vector2i vec) {
        return 0;
    }
}
