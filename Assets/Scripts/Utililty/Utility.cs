using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public static class Utility {

    #region Enums
    /// <summary>
    /// An enum defining the 4 orthogonal directions in 2D space including None
    /// </summary>
    public enum EDirection4 {
        None,
        Up,
        Right,
        Down,
        Left
    }

    /// <summary>
    /// An enum defining the classical 8 direction in 2D space including None
    /// </summary>
    public enum EDirection8 {
        None,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }
    #endregion

    #region Enum extensions
    /// <summary>
    /// Returns the number of values in an enum
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static int Count(this Enum e) {
        return Enum.GetValues(e.GetType()).Length;
    }

    public static int FlagCount(this Fruit.EFruit e) {
        int count = 0;
        var values = Enum.GetValues(e.GetType());
        foreach (Fruit.EFruit value in values) {
            if ((e & value) != 0)
                count++;
        }
        return count;
    }
    #endregion

    #region EDirection extensions
    /// <summary>
    /// Converts a <see cref="EDirection4"/> to <see cref="EDirection8"/>
    /// </summary>
    /// <param name="dir4"></param>
    /// <returns></returns>
    public static EDirection8 To8(this EDirection4 dir4) {
        if (dir4 == EDirection4.None)
            return EDirection8.None;
        return (EDirection8)((int)dir4 * 2 - 1);
    }

    /// <summary>
    /// Converts an <see cref="EDirection8"/> to <see cref="EDirection4"/>. Throws an exception if given value is diagonal.
    /// </summary>
    /// <param name="dir8"></param>
    /// <returns></returns>
    public static EDirection4 To4(this EDirection8 dir8) {
        if (dir8 == EDirection8.None)
            return EDirection4.None;
        if (((int)dir8).IsEven())
            throw new InvalidEnumArgumentException("dir8", (int)dir8, dir8.GetType());
        return (EDirection4)(((int)dir8 + 1) / 2);
    }

    /// <summary>
    /// Check to see if both <see cref="EDirection4"/> are opposite to each other
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>True if opposite</returns>
    public static bool IsOpposite(this EDirection4 lhs, EDirection4 rhs) {
        return lhs.To8().IsOpposite(rhs.To8());
    }

    /// <summary>
    /// Check to see if both <see cref="EDirection8s"/> are opposite to each other
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>True if opposite</returns>
    public static bool IsOpposite(this EDirection8 lhs, EDirection8 rhs) {
        return lhs != EDirection8.None && rhs != EDirection8.None && Math.Abs((int)lhs - (int)rhs) == 4;
    }

    /// <summary>
    /// Get the opposite <see cref="EDirection4"/> of the given value
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static EDirection4 Opposite(this EDirection4 dir) {
        if (dir == EDirection4.None)
            return EDirection4.None;
        return (EDirection4)(((int)dir + 1) % 4 + 1);
    }

    /// <summary>
    /// Get the opposite <see cref="EDirection8"/> of the given value
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static EDirection8 Opposite(this EDirection8 dir) {
        if (dir == EDirection8.None)
            return EDirection8.None;
        return (EDirection8)(((int)dir + 3) % (dir.Count() - 1) + 1);
    }

    /// <summary>
    /// Get the direction as a <see cref="Vector2"/>
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this EDirection4 dir) {
        return dir.To8().ToVector2();
    }

    /// <summary>
    /// Get the direction as a <see cref="Vector2"/>
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this EDirection8 dir) {
        switch (dir) {
        case EDirection8.None:
            return Vector2.zero;
        case EDirection8.Up:
            return Vector2.up;
        case EDirection8.UpRight:
            return Vector2.up + Vector2.right;
        case EDirection8.Right:
            return Vector2.right;
        case EDirection8.DownRight:
            return Vector2.down + Vector2.right;
        case EDirection8.Down:
            return Vector2.down;
        case EDirection8.DownLeft:
            return Vector2.down + Vector2.left;
        case EDirection8.Left:
            return Vector2.left;
        case EDirection8.UpLeft:
            return Vector2.up + Vector2.left;
        default:
            throw new ArgumentOutOfRangeException("dir", dir, null);
        }
    }

    #endregion

    #region Bool extensions

    /// <summary>
    /// Bool conversion to int (false=0, true=1)
    /// </summary>
    /// <param name="val">The bool value</param>
    /// <returns>0 if false, 1 if true</returns>
    public static int ToInt(this bool val) {
        return val ? 1 : 0;
    }

    /// <summary>
    /// Bool conversion to int (false=-1, true=1)
    /// </summary>
    /// <param name="val">The bool value</param>
    /// <returns>-1 if false, 1 if true</returns>
    public static int ToIntSign(this bool val) {
        return val ? 1 : -1;
    }

    #endregion

    #region Int extensions

    /// <summary>
    /// Returns true if value is even
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEven(this int value) {
        return (value & 1) == 0;
    }

    /// <summary>
    /// Returns true if value is uneven
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsUneven(this int value) {
        return (value & 1) == 1;
    }

    #endregion

    #region Float utilities
    /// <summary>
    /// Default tolerance value used for floating point number for comparing
    /// </summary>
    /// <returns>1e-6f</returns>
    public const float FloatTolerance = 1e-6f;

    /// <summary>
    /// Returns true if val is strictly between bound1 and bound2
    /// </summary>
    /// <param name="val">The value to compare</param>
    /// <param name="bound1">First bound</param>
    /// <param name="bound2">Second bound</param>
    /// <returns></returns>
    public static bool IsStrictlyBetween(float val, float bound1, float bound2) {
        Sort(ref bound1, ref bound2);
        return bound1 < val && val < bound2;
    }

    /// <summary>
    /// Returns true if val is between bound1 and bound2 including the lower bound
    /// </summary>
    /// <param name="val">The value to compare</param>
    /// <param name="bound1">First bound</param>
    /// <param name="bound2">Second bound</param>
    /// <returns></returns>
    public static bool IsInclusiveLowerBetween(float val, float bound1, float bound2) {
        Sort(ref bound1, ref bound2);
        return bound1 <= val && val < bound2;
    }

    /// <summary>
    /// Returns true if val is between bound1 and bound2 including the upper bound
    /// </summary>
    /// <param name="val"></param>
    /// <param name="bound1"></param>
    /// <param name="bound2"></param>
    /// <returns></returns>
    public static bool IsInclusiveHigherBetween(float val, float bound1, float bound2) {
        Sort(ref bound1, ref bound2);
        return bound1 < val && val <= bound2;
    }

    /// <summary>
    /// Returns true if val is the same or between bound1 and bound2
    /// </summary>
    /// <param name="val"></param>
    /// <param name="bound1"></param>
    /// <param name="bound2"></param>
    /// <returns></returns>
    public static bool IsInclusiveBetween(float val, float bound1, float bound2) {
        Sort(ref bound1, ref bound2);
        return bound1 <= val && val <= bound2;
    }

    /// <summary>
    /// Changes the first value to the lowest and the second to the highest value
    /// </summary>
    /// <param name="lower">The value to be changed to the lowest of the 2</param>
    /// <param name="higher">The value to be changed to the highest of the 2</param>
    private static void Sort(ref float lower, ref float higher) {
        // Values are sorted
        if (lower <= higher)
            return;
        // switch the values
        var temp = lower;
        lower = higher;
        higher = temp;
    }

    #endregion

    #region Float extensions
    /// <summary>
    /// Converts a radian value to degrees
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static float ToDegree(this float val) {
        return (float)(val*180/Math.PI);
    }

    public static float ToRadian(this float val) {
        return (float)(val*Math.PI/180);
    }
    #endregion

    #region Double extensions

    public static double ToDegree(this double val) {
        return val*180/Math.PI;
    }

    public static double ToRadian(this double val) {
        return val*Math.PI/180;
    }
    #endregion

    #region object extensions

    #endregion

    #region Vector2 extensions

    /// <summary>
    /// Calculates the Euclidian distance between 2 points
    /// </summary>
    /// <param name="source"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static float DistanceTo(this Vector2 source, Vector2 other) {
        float dx = source.x - other.x;
        float dy = source.y - other.y;

        return (float)System.Math.Sqrt(dx*dx + dy*dy);
    }

    /// <summary>
    /// Divides all corresponding elements. 
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>(lhs.x/rhs.x, lhs.y/rhs.y)</returns>
    public static Vector2 Divide(this Vector2 lhs, Vector2 rhs) {
        return new Vector2(lhs.x/rhs.x, lhs.y/rhs.y);
    }

    public static Vector2 Multiply(this Vector2 lhs, Vector2 rhs) {
        return new Vector2(lhs.x * rhs.x, lhs.y * rhs.y);
    }


    #endregion

    #region Vector2i extensions

    public static Vector2i Index1DTo2D(int index, int width) {
        return new Vector2i(index%width, index/width);
    }

    public static int Index2DTo1D(Vector2i index, int width) {
        return index.X + index.Y*width;
    }

    #endregion

    #region Vector3 extensions
    /// <summary>
    /// Divides all corresponding elements. 
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>(lhs.x/rhs.x, lhs.y/rhs.y, lhs.z/rhs.z)</returns>
    public static Vector3 Divide(this Vector3 lhs, Vector3 rhs) {
        return new Vector3(lhs.x/rhs.x, lhs.y/rhs.y, lhs.z/rhs.z);
    }

    /// <summary>
    /// Returns theta, the 
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static float Theta(this Vector3 vec) {
        return (float)Math.Atan2(vec.y,vec.x);
    }

    public static float Phi(this Vector3 vec) {
        return (float)Math.Atan2(Math.Sqrt(Math.Pow(vec.x, 2) + Math.Pow(vec.y, 2)), vec.z);
    }

    public static Vector3 Abs(this Vector3 vec) {
        if (vec.x < 0)
            return -vec;
        return vec;
    }

    public static Vector3 Multiply(this Vector3 lhs, Vector3 rhs) {
        return new Vector3(lhs.x*rhs.x, lhs.y*rhs.y, lhs.z*rhs.z);
    }
    #endregion

    #region TimeSpan extensions

    /// <summary>
    /// Multiplies the TimeSpan with the given integer
    /// </summary>
    /// <param name="ts"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static TimeSpan Multiply(this TimeSpan ts, int num) {
        return TimeSpan.FromTicks(ts.Ticks*num);
    }

    /// <summary>
    /// Multiplies the TimeSpan with the given float
    /// </summary>
    /// <param name="ts"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static TimeSpan Multiply(this TimeSpan ts, float num) {
        return TimeSpan.FromTicks((long)(ts.Ticks*num));
    }

    #endregion

    #region Unity extensions

    /// <summary>
    /// Gets the collision mask of the indicated layer
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int GetCollisionMask2D(int layer) {
        int result = 0;
        for (int i = 0; i < 32; i++) {
            if (!Physics2D.GetIgnoreLayerCollision(layer, i))
                result = result | 1 << i;
        }
        return result;
    }

    /// <summary>
    /// Gets the collision mask of the indicated layer
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int GetCollisionMask2D(LayerMask layer) {
        return GetCollisionMask2D(layer.value);
    }

    /// <summary>
    /// Gets all children directly under the given one
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform[] GetChildren(this Transform transform) {
        var result = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            result[i] = transform.GetChild(i);
        return result;
    }

    /// <summary>
    /// Get all children from the transform, including their children and so forth
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform[] GetAllChildren(this Transform transform) {
        return transform.GetAllChildren(null);
    }

    /// <summary>
    /// Get all children from the transform, including their children and so forth
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Transform[] GetAllChildren(this Transform transform, [NotNull] Func<Transform, bool> predicate) {
        if (predicate == null) throw new ArgumentNullException("predicate");
        return GetAllChildrenRecursive(transform, predicate, false);
    }

    /// <summary>
    /// Helper function to get all children and their children recursively
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="predicate"></param>
    /// <param name="addSelf"></param>
    /// <returns></returns>
    private static Transform[] GetAllChildrenRecursive(Transform transform, Func<Transform, bool> predicate = null, bool addSelf = true) {
        var result = new List<Transform>();
        if (addSelf && (predicate == null || predicate(transform)))
            result.Add(transform);
        for (int i = 0; i < transform.childCount; i++)
            result.AddRange(GetAllChildrenRecursive(transform.GetChild(i), predicate));
        return result.ToArray();
    }

    #endregion

    #region ICollection extensions
    /// <summary>
    /// Remove the first element of the collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static IEnumerable<T> RemoveFirst<T>(this IEnumerable<T> list) {
        var e = list.GetEnumerator();
        e.MoveNext();
        while (e.MoveNext())
            yield return e.Current;
    }

    /// <summary>
    /// Removes the first element of the collection matching the predicate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<T> RemoveFirst<T>(this IEnumerable<T> list, Func<T, bool> predicate) {
        var e = list.GetEnumerator();
        while (e.MoveNext()) {
            if (predicate(e.Current))
                break;
            yield return e.Current;
        }
        while (e.MoveNext()) {
            yield return e.Current;
        }
    }

    /// <summary>
    /// Removes the last element of the collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static void RemoveLast<T>(this ICollection<T> list) {
        list.Remove(list.Last());
    }

    /// <summary>
    /// Removes all elements matching the predicate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static void RemoveAll<T>(this ICollection<T> list, Func<T, bool> predicate) {
        for (int i = list.Count - 1; i >= 0; i--) {
            if (predicate(list.ElementAt(i)))
                list.Remove(list.ElementAt(i));
        }
    }

    /// <summary>
    /// Gets a random element from the list (linear distribution).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this ICollection<T> list) {
        return list.ElementAt(Random.Range(0, list.Count));
    }
    #endregion
}