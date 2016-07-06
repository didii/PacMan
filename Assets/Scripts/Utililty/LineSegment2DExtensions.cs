using System;
using UnityEngine;

public static class LineSegment2DExtensions {

    /// <summary>
    /// Says is the line is more horizontal or more vertical.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Utility.EDirection2 ToDirection2(this LineSegment2D self) {
        switch ((int)(self.AngleTop*4/Mathf.PI)) {
        case 0:
        case 3:
            return Utility.EDirection2.Horizontal;
        case 1:
        case 2:
            return Utility.EDirection2.Vertical;
        default:
            throw new InvalidOperationException("Aqcuired a result that is mathematically impossible. Congratulations!");
        }
    }
}
