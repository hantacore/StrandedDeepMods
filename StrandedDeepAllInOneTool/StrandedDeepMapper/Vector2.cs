using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrandedDeepMapper
{
    /// <summary>
    ///   <para>Representation of 2D vectors and points.</para>
    /// </summary>]
    public struct Vector2
    {
        /// <summary>
        ///   <para>X component of the vector.</para>
        /// </summary>
        public double x;

        /// <summary>
        ///   <para>Y component of the vector.</para>
        /// </summary>
        public double y;

        //    private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

        //    private static readonly Vector2 oneVector = new Vector2(1f, 1f);

        //    private static readonly Vector2 upVector = new Vector2(0f, 1f);

        //    private static readonly Vector2 downVector = new Vector2(0f, -1f);

        //    private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

        //    private static readonly Vector2 rightVector = new Vector2(1f, 0f);

        //    private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        //    private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        //    public const float kEpsilon = 1E-05f;

        //    public double this[int index]
        //    {
        //        get
        //        {
        //            switch (index)
        //            {
        //                case 0:
        //                    return x;
        //                case 1:
        //                    return y;
        //                default:
        //                    throw new IndexOutOfRangeException("Invalid Vector2 index!");
        //            }
        //        }
        //        set
        //        {
        //            switch (index)
        //            {
        //                case 0:
        //                    x = value;
        //                    break;
        //                case 1:
        //                    y = value;
        //                    break;
        //                default:
        //                    throw new IndexOutOfRangeException("Invalid Vector2 index!");
        //            }
        //        }
        //    }

        //    /// <summary>
        //    ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        //    /// </summary>
        //    //public Vector2 normalized
        //    //{
        //    //    get
        //    //    {
        //    //        Vector2 result = new Vector2(x, y);
        //    //        result.Normalize();
        //    //        return result;
        //    //    }
        //    //}

        //    /// <summary>
        //    ///   <para>Returns the length of this vector (Read Only).</para>
        //    /// </summary>
        //    public double magnitude => Math.Sqrt(x * x + y * y);

        /// <summary>
        ///   <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public double sqrMagnitude => x * x + y * y;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(0, 0).</para>
        //    /// </summary>
        //    public static Vector2 zero => zeroVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(1, 1).</para>
        //    /// </summary>
        //    public static Vector2 one => oneVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(0, 1).</para>
        //    /// </summary>
        //    public static Vector2 up => upVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(0, -1).</para>
        //    /// </summary>
        //    public static Vector2 down => downVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(-1, 0).</para>
        //    /// </summary>
        //    public static Vector2 left => leftVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(1, 0).</para>
        //    /// </summary>
        //    public static Vector2 right => rightVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(float.PositiveInfinity, float.PositiveInfinity).</para>
        //    /// </summary>
        //    public static Vector2 positiveInfinity => positiveInfinityVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2(float.NegativeInfinity, float.NegativeInfinity).</para>
        //    /// </summary>
        //    public static Vector2 negativeInfinity => negativeInfinityVector;

        /// <summary>
        ///   <para>Constructs a new vector with given x, y components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        //    /// <summary>
        //    ///   <para>Set x and y components of an existing Vector2.</para>
        //    /// </summary>
        //    /// <param name="newX"></param>
        //    /// <param name="newY"></param>
        //    public void Set(float newX, float newY)
        //    {
        //        x = newX;
        //        y = newY;
        //    }

        //    /// <summary>
        //    ///   <para>Linearly interpolates between vectors a and b by t.</para>
        //    /// </summary>
        //    /// <param name="a"></param>
        //    /// <param name="b"></param>
        //    /// <param name="t"></param>
        //    //public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        //    //{
        //    //    t = Math.Clamp01(t);
        //    //    return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        //    //}

        //    /// <summary>
        //    ///   <para>Linearly interpolates between vectors a and b by t.</para>
        //    /// </summary>
        //    /// <param name="a"></param>
        //    /// <param name="b"></param>
        //    /// <param name="t"></param>
        //    public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        //    {
        //        return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        //    }

        //    /// <summary>
        //    ///   <para>Moves a point current towards target.</para>
        //    /// </summary>
        //    /// <param name="current"></param>
        //    /// <param name="target"></param>
        //    /// <param name="maxDistanceDelta"></param>
        //    //public static Vector2 MoveTowards(Vector2 current, Vector2 target, double maxDistanceDelta)
        //    //{
        //    //    Vector2 a = target - current;
        //    //    double magnitude = a.magnitude;
        //    //    if (magnitude <= maxDistanceDelta || magnitude == 0f)
        //    //    {
        //    //        return target;
        //    //    }
        //    //    return current + a / magnitude * maxDistanceDelta;
        //    //}

        //    /// <summary>
        //    ///   <para>Multiplies two vectors component-wise.</para>
        //    /// </summary>
        //    /// <param name="a"></param>
        //    /// <param name="b"></param>
        //    public static Vector2 Scale(Vector2 a, Vector2 b)
        //    {
        //        return new Vector2(a.x * b.x, a.y * b.y);
        //    }

        //    /// <summary>
        //    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
        //    /// </summary>
        //    /// <param name="scale"></param>
        //    public void Scale(Vector2 scale)
        //    {
        //        x *= scale.x;
        //        y *= scale.y;
        //    }

        //    /// <summary>
        //    ///   <para>Makes this vector have a magnitude of 1.</para>
        //    /// </summary>
        //    //public void Normalize()
        //    //{
        //    //    double magnitude = this.magnitude;
        //    //    if (magnitude > 1E-05f)
        //    //    {
        //    //        this /= magnitude;
        //    //    }
        //    //    else
        //    //    {
        //    //        this = zero;
        //    //    }
        //    //}

        //    /// <summary>
        //    ///   <para>Returns a nicely formatted string for this vector.</para>
        //    /// </summary>
        //    /// <param name="format"></param>
        //    public override string ToString()
        //    {
        //        return String.Format("({0:F1}, {1:F1})", x, y);
        //    }

        //    /// <summary>
        //    ///   <para>Returns a nicely formatted string for this vector.</para>
        //    /// </summary>
        //    /// <param name="format"></param>
        //    public string ToString(string format)
        //    {
        //        return String.Format("({0}, {1})", x.ToString(format), y.ToString(format));
        //    }

        //    public override int GetHashCode()
        //    {
        //        return x.GetHashCode() ^ (y.GetHashCode() << 2);
        //    }

        //    /// <summary>
        //    ///   <para>Returns true if the given vector is exactly equal to this vector.</para>
        //    /// </summary>
        //    /// <param name="other"></param>
        //    public override bool Equals(object other)
        //    {
        //        if (!(other is Vector2))
        //        {
        //            return false;
        //        }
        //        Vector2 vector = (Vector2)other;
        //        return x.Equals(vector.x) && y.Equals(vector.y);
        //    }

        //    /// <summary>
        //    ///   <para>Reflects a vector off the vector defined by a normal.</para>
        //    /// </summary>
        //    /// <param name="inDirection"></param>
        //    /// <param name="inNormal"></param>
        //    //public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
        //    //{
        //    //    return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
        //    //}

        //    /// <summary>
        //    ///   <para>Dot Product of two vectors.</para>
        //    /// </summary>
        //    /// <param name="lhs"></param>
        //    /// <param name="rhs"></param>
        //    public static double Dot(Vector2 lhs, Vector2 rhs)
        //    {
        //        return lhs.x * rhs.x + lhs.y * rhs.y;
        //    }

        //    /// <summary>
        //    ///   <para>Returns the unsigned angle in degrees between from and to.</para>
        //    /// </summary>
        //    /// <param name="from">The vector from which the angular difference is measured.</param>
        //    /// <param name="to">The vector to which the angular difference is measured.</param>
        //    //public static float Angle(Vector2 from, Vector2 to)
        //    //{
        //    //    return Math.Acos(Math.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
        //    //}

        //    /// <summary>
        //    ///   <para>Returns the signed angle in degrees between from and to.</para>
        //    /// </summary>
        //    /// <param name="from">The vector from which the angular difference is measured.</param>
        //    /// <param name="to">The vector to which the angular difference is measured.</param>
        //    //public static float SignedAngle(Vector2 from, Vector2 to)
        //    //{
        //    //    Vector2 normalized = from.normalized;
        //    //    Vector2 normalized2 = to.normalized;
        //    //    float num = Math.Acos(Math.Clamp(Dot(normalized, normalized2), -1f, 1f)) * 57.29578f;
        //    //    float num2 = Math.Sign(normalized.x * normalized2.y - normalized.y * normalized2.x);
        //    //    return num * num2;
        //    //}

        //    /// <summary>
        //    ///   <para>Returns the distance between a and b.</para>
        //    /// </summary>
        //    /// <param name="a"></param>
        //    /// <param name="b"></param>
        //    public static double Distance(Vector2 a, Vector2 b)
        //    {
        //        return (a - b).magnitude;
        //    }

        //    /// <summary>
        //    ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        //    /// </summary>
        //    /// <param name="vector"></param>
        //    /// <param name="maxLength"></param>
        //    //public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        //    //{
        //    //    if (vector.sqrMagnitude > maxLength * maxLength)
        //    //    {
        //    //        return vector.normalized * maxLength;
        //    //    }
        //    //    return vector;
        //    //}

        //    public static double SqrMagnitude(Vector2 a)
        //    {
        //        return a.x * a.x + a.y * a.y;
        //    }

        //    public double SqrMagnitude()
        //    {
        //        return x * x + y * y;
        //    }

        //    /// <summary>
        //    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
        //    /// </summary>
        //    /// <param name="lhs"></param>
        //    /// <param name="rhs"></param>
        //    public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        //    {
        //        return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        //    }

        //    /// <summary>
        //    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
        //    /// </summary>
        //    /// <param name="lhs"></param>
        //    /// <param name="rhs"></param>
        //    public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        //    {
        //        return new Vector2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        //    }

        //    //public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        //    //{
        //    //    smoothTime = Math.Max(0.0001f, smoothTime);
        //    //    float num = 2f / smoothTime;
        //    //    float num2 = num * deltaTime;
        //    //    float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        //    //    Vector2 vector = current - target;
        //    //    Vector2 vector2 = target;
        //    //    float maxLength = maxSpeed * smoothTime;
        //    //    vector = ClampMagnitude(vector, maxLength);
        //    //    target = current - vector;
        //    //    Vector2 vector3 = (currentVelocity + num * vector) * deltaTime;
        //    //    currentVelocity = (currentVelocity - num * vector3) * d;
        //    //    Vector2 vector4 = target + (vector + vector3) * d;
        //    //    if (Dot(vector2 - current, vector4 - vector2) > 0f)
        //    //    {
        //    //        vector4 = vector2;
        //    //        currentVelocity = (vector4 - vector2) / deltaTime;
        //    //    }
        //    //    return vector4;
        //    //}

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        //public static Vector2 operator -(Vector2 a)
        //{
        //    return new Vector2(0f - a.x, 0f - a.y);
        //}

        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        //    public static Vector2 operator *(float d, Vector2 a)
        //    {
        //        return new Vector2(a.x * d, a.y * d);
        //    }

        public static Vector2 operator /(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        //    public static bool operator ==(Vector2 lhs, Vector2 rhs)
        //    {
        //        return (lhs - rhs).sqrMagnitude < 9.99999944E-11f;
        //    }

        //    public static bool operator !=(Vector2 lhs, Vector2 rhs)
        //    {
        //        return !(lhs == rhs);
        //    }

        //    //public static implicit operator Vector2(Vector3 v)
        //    //{
        //    //    return new Vector2(v.x, v.y);
        //    //}

        //    //public static implicit operator Vector3(Vector2 v)
        //    //{
        //    //    return new Vector3(v.x, v.y, 0f);
        //    //}
    }

}
