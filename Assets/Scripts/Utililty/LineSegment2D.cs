// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using System;
using UnityEngine;

/// <summary>
/// The class encapsulates 2D line segment and provides some tool methods related to lines.
/// </summary>
/// 
/// <remarks><para>The class provides some methods which are related to line segments:
/// distance to point, finding intersection point, etc.
/// </para>
/// 
/// <para>A line segment may be converted to a <see cref="Line2D"/>.</para>
/// 
/// <para>Sample usage:</para>
/// <code>
/// // create a segment
/// LineSegment segment = new LineSegment( new Vector2( 0, 0 ), new Vector2( 3, 4 ) );
/// // get segment's length
/// float length = segment.Length;
/// 
/// // get intersection point with a line
/// Vector2? intersection = segment.GetIntersectionWith(
///     new Line( new Vector2( -3, 8 ), new Vector2( 0, 4 ) ) );
/// </code>
/// </remarks>
/// 
public class LineSegment2D {
    #region Fields
    // segment's start/end point
    private readonly Vector2 _start;
    private readonly Vector2 _end;

    private readonly Line2D _line;
    #endregion

    #region Properties
    /// <summary>
    /// Start point of the line segment.
    /// </summary>
    public Vector2 Start {
        get { return _start; }
    }

    /// <summary>
    /// End point of the line segment.
    /// </summary>
    public Vector2 End {
        get { return _end; }
    }

    /// <summary>
    /// Get segment's length - Euclidean distance between its <see cref="Start"/> and <see cref="End"/> points.
    /// </summary>
    public float Length {
        get { return _start.DistanceTo(_end); }
    }

    /// <summary>
    /// Get the normalized direction the line is facing
    /// </summary>
    public Vector2 Direction {
        get { return (_end - _start).normalized; }
    }

    /// <summary>
    /// Returns the center point of the line segment.
    /// </summary>
    public Vector2 Center {
        get { return (Start + End)/2; }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment2D"/> class.
    /// </summary>
    /// 
    /// <param name="start">Segment's start point.</param>
    /// <param name="end">Segment's end point.</param>
    /// 
    /// <exception cref="ArgumentException">Thrown if the two points are the same.</exception>
    /// 
    public LineSegment2D(Vector2 start, Vector2 end) {
        _line = Line2D.FromPoints(start, end);
        this._start = start;
        this._end = end;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment2D"/> class.
    /// </summary>
    /// <param name="start">The starting point</param>
    /// <param name="direction">The direction of the line</param>
    /// <param name="length">The length of the line</param>
    public LineSegment2D(Vector2 start, Vector2 direction, float length) {
        this._start = start;
        this._end = start + direction.normalized*length;
        _line = Line2D.FromPoints(_start, _end);
    }

    /// <summary>
    /// Converts this <see cref="LineSegment2D"/> to a <see cref="Line2D"/> by discarding
    /// its endpoints and extending it infinitely in both directions.
    /// </summary>
    /// 
    /// <param name="segment">The segment to convert to a <see cref="Line2D"/>.</param>
    /// 
    /// <returns>Returns a <see cref="Line2D"/> that contains this <paramref name="segment"/>.</returns>
    /// 
    public static explicit operator Line2D(LineSegment2D segment) {
        return segment._line;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Calculate Euclidean distance between a point and a finite line segment.
    /// </summary>
    /// 
    /// <param name="point">The point to calculate the distance to.</param>
    /// 
    /// <returns>Returns the Euclidean distance between this line segment and the specified point. Unlike
    /// <see cref="Line2D.DistanceToPoint"/>, this returns the distance from the finite segment. (0,0) is 5 units
    /// from the segment (0,5)-(0,8), but is 0 units from the line through those points.</returns>
    /// 
    public float DistanceToPoint(Vector2 point) {
        float segmentDistance;

        switch (LocateProjection(point)) {
        case ProjectionLocation.RayA:
            segmentDistance = point.DistanceTo(_start);
            break;
        case ProjectionLocation.RayB:
            segmentDistance = point.DistanceTo(_end);
            break;
        default:
            segmentDistance = _line.DistanceToPoint(point);
            break;
        };

        return segmentDistance;
    }

    /// <summary>
    /// Finds, provided it exists, the intersection point with the specified <see cref="LineSegment2D"/>.
    /// </summary>
    /// 
    /// <param name="other"><see cref="LineSegment2D"/> to find intersection with.</param>
    /// 
    /// <returns>Returns intersection point with the specified <see cref="LineSegment2D"/>, or <see langword="null"/>, if
    /// the two segments do not intersect.</returns>
    /// 
    /// <remarks><para>If the two segments do not intersect, the method returns <see langword="null"/>. If the two
    /// segments share multiple points, this throws an <see cref="OverlappingLineSegmentsException"/>.
    /// </para></remarks>
    /// 
    /// <exception cref="OverlappingLineSegmentsException">Thrown if the segments overlap - if they have
    /// multiple points in common.</exception>
    /// 
    public Vector2? GetIntersectionWith(LineSegment2D other) {
        Vector2? result = null;

        if ((_line.Slope == other._line.Slope) || (_line.IsVertical && other._line.IsVertical)) {
            if (_line.Intercept == other._line.Intercept) {
                // Collinear segments. Inspect and handle.
                // Consider this segment AB and other as CD. (start/end in both cases)
                // There are three cases:
                // 0 shared points: C and D both project onto the same ray of AB
                // 1 shared point: One of A or B equals one of C or D, and the other of C/D 
                //      projects on the correct ray.
                // Many shared points.

                ProjectionLocation projC = LocateProjection(other._start), projD = LocateProjection(other._end);

                if ((projC != ProjectionLocation.SegmentAB) && (projC == projD)) {
                    // no shared points
                    result = null;
                } else if (((_start == other._start) && (projD == ProjectionLocation.RayA)) ||
                            ((_start == other._end) && (projC == ProjectionLocation.RayA))) {
                    // shared start point
                    result = _start;
                } else if (((_end == other._start) && (projD == ProjectionLocation.RayB)) ||
                            ((_end == other._end) && (projC == ProjectionLocation.RayB))) {
                    // shared end point
                    result = _end;
                } else {
                    // overlapping
                    throw new OverlappingLineSegmentsException(this, other, "Overlapping segments do not have a single intersection point.");
                }
            }
        } else {
            result = GetIntersectionWith(other._line);

            if (result.HasValue && (other.LocateProjection(result.Value) != ProjectionLocation.SegmentAB)) {
                // the intersection is on the extended line of this segment
                result = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Finds, provided it exists, the intersection point with the specified <see cref="Line2D"/>.
    /// </summary>
    /// 
    /// <param name="other"><see cref="Line2D"/> to find intersection with.</param>
    /// 
    /// <returns>Returns intersection point with the specified <see cref="Line2D"/>, or <see langword="null"/>, if
    /// the line does not intersect with this segment.</returns>
    /// 
    /// <remarks><para>If the line and the segment do not intersect, the method returns <see langword="null"/>. If the line
    /// and the segment share multiple points, the method throws an <see cref="OverlappingLinesException"/>.
    /// </para></remarks>
    /// 
    /// <exception cref="OverlappingLinesException">Thrown if this segment is a portion of
    /// <paramref name="other"/> line.</exception>
    /// 
    public Vector2? GetIntersectionWith(Line2D other) {
        Vector2? result;

        if ((_line.Slope == other.Slope) || (_line.IsVertical && other.IsVertical)) {
            if (_line.Intercept == other.Intercept)
                throw new OverlappingLineLineSegmentException(this, other, "Segment is a portion of the specified line.");

            // unlike Line.GetIntersectionWith(Line), this does not throw on parallel distinct lines
            result = null;
        } else {
            result = _line.GetIntersectionWith(other);
        }

        if ((result.HasValue) && (LocateProjection(result.Value) != ProjectionLocation.SegmentAB)) {
            // the intersection is on this segment's extended line, but not on the segment itself
            result = null;
        }

        return result;
    }

    /// <summary>
    /// Returns whether a <see cref="point"/> is directly on the line segment or not.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns></returns>
    public bool IsIntersectingWith(Vector2 point) {
        return DistanceToPoint(point) == 0;
    }

    /// <summary>
    /// Returns whether another <see cref="LineSegment2D"/> is intersecting or not.
    /// </summary>
    /// <param name="other">The other <see cref="LineSegment2D"/></param>
    /// <returns></returns>
    public bool IsIntersectingWith(LineSegment2D other) {
        return GetIntersectionWith(other) != null;
    }

    /// <summary>
    /// Returns whether a <see cref="Line2D"/> is intersecting with this segment.
    /// </summary>
    /// <param name="other">The other <see cref="Line"/></param>
    /// <returns></returns>
    public bool IsIntersectingWith(Line2D other) {
        return GetIntersectionWith(other) != null;
    }

    /// <summary>
    /// Draws the line on the screen in blue for a single frame.
    /// </summary>
    public void Draw() {
        Draw(Color.blue);
    }

    /// <summary>
    /// Draws the line on the screen for a single frame.
    /// </summary>
    /// <param name="color"></param>
    public void Draw(Color color) {
        Debug.DrawLine(_start, _end, color);
    }

    /// <summary>
    /// Draws the line on the screen in a given color, duration and optional depth test.
    /// </summary>
    /// <param name="color">The color to draw the line in.</param>
    /// <param name="duration">How long the line is visible in seconds.</param>
    /// <param name="depthTest">Whether the line should be obstructed by object in front of it.</param>
    public void Draw(Color color, float duration, bool depthTest = false) {
        Debug.DrawLine(_start, _end, color, duration, depthTest);
    }
    #endregion

    #region Helper methods
    // Represents the location of a projection of a point on the line that contains this segment.
    // If the point projects to A,B, or anything between them, it is SegmentAB.
    // If it projects beyond A, it's RayA; if it projects beyond B, it's RayB.
    private enum ProjectionLocation { RayA, SegmentAB, RayB }

    // Get type of point's projections to this line segment
    private ProjectionLocation LocateProjection(Vector2 point) {
        // Modified from http://www.codeguru.com/forum/showthread.php?t=194400

        /*  How do I find the distance from a point to a line segment?

            Let the point be C (Cx,Cy) and the line be AB (Ax,Ay) to (Bx,By).
            Let P be the point of perpendicular projection of C on AB.  The parameter
            r, which indicates P's position along AB, is computed by the dot product 
            of AC and AB divided by the square of the length of AB:

            (1)     AC dot AB
                r = ---------  
                    ||AB||^2

            r has the following meaning:

                r=0      P = A
                r=1      P = B
                r<0      P is on the backward extension of AB (and distance C-AB is distance C-A)
                r>1      P is on the forward extension of AB (and distance C-AB is distance C-B)
                0<r<1    P is interior to AB (and distance C-AB(segment) is distance C-AB(line))

            The length of the line segment AB is computed by:

                L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 )

            and the dot product of two 2D vectors, U dot V, is computed:

                D = (Ux * Vx) + (Uy * Vy) 

            So (1) expands to:

                    (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay)
                r = -------------------------------
                         (Bx-Ax)^2 + (By-Ay)^2
        */

        // the above is modified here to compare the numerator and denominator, rather than doing the division
        Vector2 abDelta = _end - _start;
        Vector2 acDelta = point - _start;

        float numerator = acDelta.x * abDelta.x + acDelta.y * abDelta.y;
        float denomenator = abDelta.x * abDelta.x + abDelta.y * abDelta.y;

        ProjectionLocation result = (numerator < 0) ? ProjectionLocation.RayA : (numerator > denomenator) ? ProjectionLocation.RayB : ProjectionLocation.SegmentAB;

        return result;
    }
    #endregion

    #region Operators
    /// <summary>
    /// Equality operator - checks if two line segments have equal parameters.
    /// </summary>
    /// 
    /// <param name="line1">First line segment to check.</param>
    /// <param name="line2">Second line segment to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if parameters of specified
    /// line segments are equal.</returns>
    ///
    public static bool operator ==(LineSegment2D line1, LineSegment2D line2) {
        if (ReferenceEquals(line1, line2))
            return true;

        if ((object)line1 == null || (object)line2 == null)
            return false;

        return (line1._start == line2._start && line1._end == line2._end) || (line1._start == line2._end && line1._end == line2._start);
    }

    /// <summary>
    /// Inequality operator - checks if two lines have different parameters.
    /// </summary>
    /// 
    /// <param name="line1">First line segment to check.</param>
    /// <param name="line2">Second line segment to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if parameters of specified
    /// line segments are not equal.</returns>
    ///
    public static bool operator !=(LineSegment2D line1, LineSegment2D line2) {
        return !(line1 == line2);
    }
    #endregion

    #region Object overrides
    /// <summary>
    /// Check if this instance of <see cref="LineSegment2D"/> equals to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another line segment to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj) {
        return (obj is LineSegment2D) && (this == (LineSegment2D)obj);
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode() {
        return _start.GetHashCode() + _end.GetHashCode();
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains values of the like in readable form.</returns>
    ///
    public override string ToString() {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0}) -> ({1})", _start, _end);
    }
    #endregion
}

[Serializable]
public class OverlappingLineSegmentsException : Exception {
    public LineSegment2D Line1, Line2;

    public OverlappingLineSegmentsException(LineSegment2D line1, LineSegment2D line2) {
        Line1 = line1;
        Line2 = line2;
    }

    public OverlappingLineSegmentsException(LineSegment2D line1, LineSegment2D line2, string msg)
        : base(msg) {
        Line1 = line1;
        Line2 = line2;
    }

    public OverlappingLineSegmentsException(LineSegment2D line1, LineSegment2D line2, string msg, Exception inner)
        : base(msg, inner) {
        Line1 = line1;
        Line2 = line2;
    }
}

public class OverlappingLineLineSegmentException : Exception {
    public LineSegment2D LineSegment;
    public Line2D Line;

    public OverlappingLineLineSegmentException(LineSegment2D lineSegment, Line2D line, string msg = "")
        : base(msg) {
        LineSegment = lineSegment;
        Line = line;
    }
}

public class SameStartAndEndpointException : Exception {
    public Vector2 Start, End;

    public SameStartAndEndpointException(Vector2 start, Vector2 end, string msg = "")
        : base(msg) {
        Start = start;
        End = end;
    }
}