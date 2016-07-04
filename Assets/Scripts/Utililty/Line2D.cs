// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using UnityEngine;

using System;
using System.Runtime.Serialization;

/// <summary>
/// The class encapsulates 2D line and provides some tool methods related to lines.
/// </summary>
/// 
/// <remarks><para>The class provides some methods which are related to lines:
/// angle between lines, distance to point, finding intersection point, etc.
/// </para>
/// 
/// <para>Generally, the equation of the line is y = <see cref="Slope"/> * x + 
/// <see cref="Intercept"/>. However, when <see cref="Slope"/> is an Infinity,
/// <see name="Intercept"/> would normally be meaningless, and it would be
/// impossible to distinguish the line x = 5 from the line x = -5. Therefore,
/// if <see cref="Slope"/> is <see cref="float.PositiveInfinity"/> or
/// <see cref="float.NegativeInfinity"/>, the line's equation is instead 
/// x = <see cref="Intercept"/>.</para>
/// 
/// <para>Sample usage:</para>
/// <code>
/// // create a line
/// Line line = Line.FromPoints( new Vector2( 0, 0 ), new Vector2( 3, 4 ) );
/// // check if it is vertical or horizontal
/// if ( line.IsVertical || line.IsHorizontal )
/// {
///     // ...
/// }
/// 
/// // get intersection point with another line
/// Vector2 intersection = line.GetIntersectionWith(
///     Line.FromPoints( new Vector2( 3, 0 ), new Vector2( 0, 4 ) ) );
/// </code>
/// </remarks>
/// 
public class Line2D {
    #region Fields
    // line's parameters from its equation: y = k * x + b;
    // If k is an Infinity, the equation is x = b.
    private readonly float k; // line's slope
    private readonly float b; // Y-coordinate where line intersects Y-axis
    #endregion

    #region Properties
    /// <summary>
    /// Checks if the line vertical or not.
    /// </summary>
    ///
    public bool IsVertical {
        get { return float.IsInfinity(k); }
    }

    /// <summary>
    /// Checks if the line horizontal or not.
    /// </summary>
    public bool IsHorizontal {
        get { return k == 0; }
    }

    /// <summary>
    /// Gets the slope of the line.
    /// </summary>
    public float Slope { get { return k; } }

    /// <summary>
    /// If not <see cref="IsVertical"/>, gets the Line's Y-intercept.
    /// If <see cref="IsVertical"/> gets the line's X-intercept.
    /// </summary>
    public float Intercept { get { return b; } }
    #endregion

    #region Factories
    /// <summary>
    /// Creates a <see cref="Line2D"/>  that goes through the two specified points.
    /// </summary>
    /// 
    /// <param name="point1">One point on the line.</param>
    /// <param name="point2">Another point on the line.</param>
    /// 
    /// <returns>Returns a <see cref="Line2D"/> representing the line between <paramref name="point1"/>
    /// and <paramref name="point2"/>.</returns>
    /// 
    /// <exception cref="SameStartAndEndpointException">Thrown if the two points are the same.</exception>
    /// 
    public static Line2D FromPoints(Vector2 point1, Vector2 point2) {
        return new Line2D(point1, point2);
    }

    /// <summary>
    /// Creates a <see cref="Line2D"/> with the specified slope and intercept.
    /// </summary>
    /// 
    /// <param name="slope">The slope of the line</param>
    /// <param name="intercept">The Y-intercept of the line, unless the slope is an
    /// infinity, in which case the line's equation is "x = intercept" instead.</param>
    /// 
    /// <returns>Returns a <see cref="Line2D"/> representing the specified line.</returns>
    /// 
    /// <remarks><para>The construction here follows the same rules as for the rest of this class.
    /// Most lines are expressed as y = slope * x + intercept. Vertical lines, however, are 
    /// x = intercept. This is indicated by <see cref="IsVertical"/> being true or by 
    /// <see cref="Slope"/> returning <see cref="float.PositiveInfinity"/> or 
    /// <see cref="float.NegativeInfinity"/>.</para></remarks>
    /// 
    public static Line2D FromSlopeIntercept(float slope, float intercept) {
        return new Line2D(slope, intercept);
    }

    /// <summary>
    /// Constructs a <see cref="Line2D"/> from a radius and an angle (in degrees).
    /// </summary>
    /// 
    /// <param name="radius">The minimum distance from the line to the origin.</param>
    /// <param name="theta">The angle of the vector from the origin to the line.</param>
    /// 
    /// <returns>Returns a <see cref="Line2D"/> representing the specified line.</returns>
    /// 
    /// <remarks><para><paramref name="radius"/> is the minimum distance from the origin
    /// to the line, and <paramref name="theta"/> is the counterclockwise rotation from
    /// the positive X axis to the vector through the origin and normal to the line.</para>
    /// <para>This means that if <paramref name="theta"/> is in [0,180), the point on the line
    /// closest to the origin is on the positive X or Y axes, or in quadrants I or II. Likewise,
    /// if <paramref name="theta"/> is in [180,360), the point on the line closest to the
    /// origin is on the negative X or Y axes, or in quadrants III or IV.</para></remarks>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">Thrown if radius is negative.</exception>
    /// 
    public static Line2D FromRTheta(float radius, float theta) {
        return new Line2D(radius, theta, false);
    }

    /// <summary>
    /// Constructs a <see cref="Line2D"/> from a point and an angle (in degrees).
    /// </summary>
    /// 
    /// <param name="point">The minimum distance from the line to the origin.</param>
    /// <param name="theta">The angle of the normal vector from the origin to the line.</param>
    /// 
    /// <remarks><para><paramref name="theta"/> is the counterclockwise rotation from
    /// the positive X axis to the vector through the origin and normal to the line.</para>
    /// <para>This means that if <paramref name="theta"/> is in [0,180), the point on the line
    /// closest to the origin is on the positive X or Y axes, or in quadrants I or II. Likewise,
    /// if <paramref name="theta"/> is in [180,360), the point on the line closest to the
    /// origin is on the negative X or Y axes, or in quadrants III or IV.</para></remarks>
    /// 
    /// <returns>Returns a <see cref="Line2D"/> representing the specified line.</returns>
    /// 
    public static Line2D FromPointTheta(Vector2 point, float theta) {
        return new Line2D(point, theta);
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs the line based on 2 <see cref="Vector2"/>s.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public Line2D(Vector2 start, Vector2 end) {
        if (start == end) {
            throw new SameStartAndEndpointException(start, end, "Start point of the line cannot be the same as its end point.");
        }

        k = (end.y - start.y) / (end.x - start.x);
        b = float.IsInfinity(k) ? start.x : start.y - k * start.x;
    }
    #endregion

    #region Private Constructors
    private Line2D(float slope, float intercept) {
        k = slope;
        b = intercept;
    }

    private Line2D(float radius, float theta, bool unused) {
        if (radius < 0) {
            throw new ArgumentOutOfRangeException("radius", radius, "Must be non-negative");
        }

        theta *= (float)(Math.PI / 180);

        float sine = (float)Math.Sin(theta), cosine = (float)Math.Cos(theta);
        Vector2 pt1 = new Vector2(radius * cosine, radius * sine);

        // -1/tan, to get the slope of the line, and not the slope of the normal
        k = -cosine / sine;

        if (!float.IsInfinity(k)) {
            b = pt1.y - k * pt1.x;
        } else {
            b = Math.Abs(radius);
        }
    }

    private Line2D(Vector2 point, float theta) {
        theta *= (float)(Math.PI / 180);

        k = (float)(-1.0f / Math.Tan(theta));

        if (!float.IsInfinity(k)) {
            b = point.y - k * point.x;
        } else {
            b = point.x;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Calculate minimum angle between this line and the specified line measured in [0, 90] degrees range.
    /// </summary>
    /// 
    /// <param name="secondLine">The line to find angle between.</param>
    /// 
    /// <returns>Returns minimum angle between lines.</returns>
    /// 
    public float GetAngleBetweenLines(Line2D secondLine) {
        float k2 = secondLine.k;

        bool isVertical1 = IsVertical;
        bool isVertical2 = secondLine.IsVertical;

        // check if lines are parallel
        if ((k == k2) || (isVertical1 && isVertical2))
            return 0;

        float angle = 0;

        if ((!isVertical1) && (!isVertical2)) {
            float tanPhi = ((k2 > k) ? (k2 - k) : (k - k2)) / (1 + k * k2);
            angle = (float)Math.Atan(tanPhi);
        } else {
            // one of the lines is parallel to Y axis

            if (isVertical1) {
                angle = (float)(Math.PI / 2 - Math.Atan(k2) * Math.Sign(k2));
            } else {
                angle = (float)(Math.PI / 2 - Math.Atan(k) * Math.Sign(k));
            }
        }

        // convert radians to degrees
        angle *= (float)(180.0 / Math.PI);

        if (angle < 0) {
            angle = -angle;
        }

        return angle;
    }

    /// <summary>
    /// Finds intersection point with the specified line.
    /// </summary>
    /// 
    /// <param name="secondLine">Line to find intersection with.</param>
    /// 
    /// <returns>Returns intersection point with the specified line, or 
    /// <see langword="null"/> if the lines are parallel and distinct.</returns>
    /// 
    /// <exception cref="InvalidOperationException">Thrown if the specified line is the same line as this line.</exception>
    /// 
    public Vector2? GetIntersectionWith(Line2D secondLine) {
        float k2 = secondLine.k;
        float b2 = secondLine.b;

        bool isVertical1 = IsVertical;
        bool isVertical2 = secondLine.IsVertical;

        Vector2? intersection = null;

        if ((k == k2) || (isVertical1 && isVertical2)) {
            if (b == b2) {
                throw new OverlappingLinesException(this, secondLine, "Identical lines do not have an intersection point.");
            }
        } else {
            if (isVertical1) {
                intersection = new Vector2(b, k2 * b + b2);
            } else if (isVertical2) {
                intersection = new Vector2(b2, k * b2 + b);
            } else {
                // the intersection is at x=(b2-b1)/(k1-k2), and y=k1*x+b1
                float x = (b2 - b) / (k - k2);
                intersection = new Vector2(x, k * x + b);
            }
        }

        return intersection;
    }

    /// <summary>
    /// Finds, provided it exists, the intersection point with the specified <see cref="LineSegment2D"/>.
    /// </summary>
    /// 
    /// <param name="other"><see cref="LineSegment2D"/> to find intersection with.</param>
    /// 
    /// <returns>Returns intersection point with the specified <see cref="LineSegment2D"/>, or <see langword="null"/>,
    /// if this line does not intersect with the segment.</returns>
    /// 
    /// <remarks><para>If the line and segment do not intersect, the method returns <see langword="null"/>.
    /// If the line and segment share multiple points, the method throws an <see cref="InvalidOperationException"/>.
    /// </para></remarks>
    /// 
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="other"/> is a portion
    /// of this line.</exception>
    /// 
    public Vector2? GetIntersectionWith(LineSegment2D other) {
        return other.GetIntersectionWith(this);
    }

    /// <summary>
    /// Calculate Euclidean distance between a point and a line.
    /// </summary>
    /// 
    /// <param name="point">The point to calculate distance to.</param>
    /// 
    /// <returns>Returns the Euclidean distance between this line and the specified point. Unlike
    /// <see cref="LineSegment2D.DistanceToPoint"/>, this returns the distance from the infinite line. (0,0) is 0 units
    /// from the line defined by (0,5) and (0,8), but is 5 units from the segment with those endpoints.</returns>
    /// 
    public float DistanceToPoint(Vector2 point) {
        float distance;

        if (!IsVertical) {
            float div = (float)Math.Sqrt(k * k + 1);
            distance = Math.Abs((k * point.x + b - point.y) / div);
        } else {
            distance = Math.Abs(b - point.x);
        }

        return distance;
    }
    #endregion

    #region Operators
    /// <summary>
    /// Equality operator - checks if two lines have equal parameters.
    /// </summary>
    /// 
    /// <param name="line1">First line to check.</param>
    /// <param name="line2">Second line to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if parameters of specified
    /// lines are equal.</returns>
    ///
    public static bool operator ==(Line2D line1, Line2D line2) {
        if (System.Object.ReferenceEquals(line1, line2)) {
            return true;
        }

        if (((object)line1 == null) || ((object)line2 == null)) {
            return false;
        }

        return ((line1.k == line2.k || (line1.IsVertical && line2.IsVertical)) && (line1.b == line2.b));
    }

    /// <summary>
    /// Inequality operator - checks if two lines have different parameters.
    /// </summary>
    /// 
    /// <param name="line1">First line to check.</param>
    /// <param name="line2">Second line to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if parameters of specified
    /// lines are not equal.</returns>
    ///
    public static bool operator !=(Line2D line1, Line2D line2) {
        return !(line1 == line2);
    }
    #endregion

    #region Object overrides
    /// <summary>
    /// Check if this instance of <see cref="Line2D"/> equals to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another line to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj) {
        return obj is Line2D && (this == (Line2D)obj);
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode() {
        return k.GetHashCode() + b.GetHashCode();
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains values of the like in readable form.</returns>
    ///
    public override string ToString() {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "k = {0}, b = {1}", k, b);
    }
    #endregion
}


[Serializable]
public class OverlappingLinesException : Exception {
    public Line2D Line1, Line2;

    public OverlappingLinesException(Line2D line1, Line2D line2) {
        Line1 = line1;
        Line2 = line2;
    }

    public OverlappingLinesException(Line2D line1, Line2D line2, string message) : base(message) {
        Line1 = line1;
        Line2 = line2;
    }

    public OverlappingLinesException(Line2D line1, Line2D line2, string message, Exception inner) : base(message, inner) {
        Line1 = line1;
        Line2 = line2;
    }

    protected OverlappingLinesException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) {}
}