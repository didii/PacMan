using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vector2i {
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2i(int x, int y) {
        X = x;
        Y = y;
    }
}

public class Vector2iComparer : IEqualityComparer<Vector2i> {
    public bool Equals(Vector2i v1, Vector2i v2) {
        if (v1 == null && v2 == null)
            return true;
        if (v1 == null || v2 == null)
            return false;
        return (v1.X == v2.X && v1.Y == v2.Y);
    }

    public int GetHashCode(Vector2i vec) {
        return 0;
    }
}
