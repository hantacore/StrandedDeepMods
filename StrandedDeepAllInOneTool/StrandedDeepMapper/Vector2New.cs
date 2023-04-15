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
    public struct Vector2New
    {
        /// <summary>
        ///   <para>X component of the vector.</para>
        /// </summary>
        public float x;

        /// <summary>
        ///   <para>Y component of the vector.</para>
        /// </summary>
        public float y;

        //    private static readonly Vector2New zeroVector = new Vector2New(0f, 0f);

        //    private static readonly Vector2New oneVector = new Vector2New(1f, 1f);

        //    private static readonly Vector2New upVector = new Vector2New(0f, 1f);

        //    private static readonly Vector2New downVector = new Vector2New(0f, -1f);

        //    private static readonly Vector2New leftVector = new Vector2New(-1f, 0f);

        //    private static readonly Vector2New rightVector = new Vector2New(1f, 0f);

        //    private static readonly Vector2New positiveInfinityVector = new Vector2New(float.PositiveInfinity, float.PositiveInfinity);

        //    private static readonly Vector2New negativeInfinityVector = new Vector2New(float.NegativeInfinity, float.NegativeInfinity);

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
        //                    throw new IndexOutOfRangeException("Invalid Vector2New index!");
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
        //                    throw new IndexOutOfRangeException("Invalid Vector2New index!");
        //            }
        //        }
        //    }

        //    /// <summary>
        //    ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        //    /// </summary>
        //    //public Vector2New normalized
        //    //{
        //    //    get
        //    //    {
        //    //        Vector2New result = new Vector2New(x, y);
        //    //        result.Normalize();
        //    //        return result;
        //    //    }
        //    //}

        //    /// <summary>
        //    ///   <para>Returns the length of this vector (Read Only).</para>
        //    /// </summary>
        //    public double magnitude => Math.Sqrt(x * x + y * y);

        // Token: 0x17000366 RID: 870
        // (get) Token: 0x060010AB RID: 4267 RVA: 0x0001A358 File Offset: 0x00018558
        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y));
            }
        }

        // Token: 0x17000367 RID: 871
        // (get) Token: 0x060010AC RID: 4268 RVA: 0x0001A38C File Offset: 0x0001858C
        public float sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(0, 0).</para>
        //    /// </summary>
        //    public static Vector2New zero => zeroVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(1, 1).</para>
        //    /// </summary>
        //    public static Vector2New one => oneVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(0, 1).</para>
        //    /// </summary>
        //    public static Vector2New up => upVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(0, -1).</para>
        //    /// </summary>
        //    public static Vector2New down => downVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(-1, 0).</para>
        //    /// </summary>
        //    public static Vector2New left => leftVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(1, 0).</para>
        //    /// </summary>
        //    public static Vector2New right => rightVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(float.PositiveInfinity, float.PositiveInfinity).</para>
        //    /// </summary>
        //    public static Vector2New positiveInfinity => positiveInfinityVector;

        //    /// <summary>
        //    ///   <para>Shorthand for writing Vector2New(float.NegativeInfinity, float.NegativeInfinity).</para>
        //    /// </summary>
        //    public static Vector2New negativeInfinity => negativeInfinityVector;

        // Token: 0x0600109A RID: 4250 RVA: 0x00019FA0 File Offset: 0x000181A0
        public Vector2New(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        //    /// <summary>
        //    ///   <para>Set x and y components of an existing Vector2New.</para>
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
        //    //public static Vector2New Lerp(Vector2New a, Vector2New b, float t)
        //    //{
        //    //    t = Math.Clamp01(t);
        //    //    return new Vector2New(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        //    //}

        //    /// <summary>
        //    ///   <para>Linearly interpolates between vectors a and b by t.</para>
        //    /// </summary>
        //    /// <param name="a"></param>
        //    /// <param name="b"></param>
        //    /// <param name="t"></param>
        //    public static Vector2New LerpUnclamped(Vector2New a, Vector2New b, float t)
        //    {
        //        return new Vector2New(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        //    }

        //    /// <summary>
        //    ///   <para>Moves a point current towards target.</para>
        //    /// </summary>
        //    /// <param name="current"></param>
        //    /// <param name="target"></param>
        //    /// <param name="maxDistanceDelta"></param>
        //    //public static Vector2New MoveTowards(Vector2New current, Vector2New target, double maxDistanceDelta)
        //    //{
        //    //    Vector2New a = target - current;
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
        //    public static Vector2New Scale(Vector2New a, Vector2New b)
        //    {
        //        return new Vector2New(a.x * b.x, a.y * b.y);
        //    }

        //    /// <summary>
        //    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
        //    /// </summary>
        //    /// <param name="scale"></param>
        //    public void Scale(Vector2New scale)
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
        //        if (!(other is Vector2New))
        //        {
        //            return false;
        //        }
        //        Vector2New vector = (Vector2New)other;
        //        return x.Equals(vector.x) && y.Equals(vector.y);
        //    }

        //    /// <summary>
        //    ///   <para>Reflects a vector off the vector defined by a normal.</para>
        //    /// </summary>
        //    /// <param name="inDirection"></param>
        //    /// <param name="inNormal"></param>
        //    //public static Vector2New Reflect(Vector2New inDirection, Vector2New inNormal)
        //    //{
        //    //    return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
        //    //}

        //    /// <summary>
        //    ///   <para>Dot Product of two vectors.</para>
        //    /// </summary>
        //    /// <param name="lhs"></param>
        //    /// <param name="rhs"></param>
        //    public static double Dot(Vector2New lhs, Vector2New rhs)
        //    {
        //        return lhs.x * rhs.x + lhs.y * rhs.y;
        //    }

        //    /// <summary>
        //    ///   <para>Returns the unsigned angle in degrees between from and to.</para>
        //    /// </summary>
        //    /// <param name="from">The vector from which the angular difference is measured.</param>
        //    /// <param name="to">The vector to which the angular difference is measured.</param>
        //    //public static float Angle(Vector2New from, Vector2New to)
        //    //{
        //    //    return Math.Acos(Math.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
        //    //}

        //    /// <summary>
        //    ///   <para>Returns the signed angle in degrees between from and to.</para>
        //    /// </summary>
        //    /// <param name="from">The vector from which the angular difference is measured.</param>
        //    /// <param name="to">The vector to which the angular difference is measured.</param>
        //    //public static float SignedAngle(Vector2New from, Vector2New to)
        //    //{
        //    //    Vector2New normalized = from.normalized;
        //    //    Vector2New normalized2 = to.normalized;
        //    //    float num = Math.Acos(Math.Clamp(Dot(normalized, normalized2), -1f, 1f)) * 57.29578f;
        //    //    float num2 = Math.Sign(normalized.x * normalized2.y - normalized.y * normalized2.x);
        //    //    return num * num2;
        //    //}

        //    /// <summary>
        //    ///   <para>Returns the distance between a and b.</para>
        //    /// </summary>
        //    /// <param name="a"></param>
        //    /// <param name="b"></param>
        //    public static double Distance(Vector2New a, Vector2New b)
        //    {
        //        return (a - b).magnitude;
        //    }

        //    /// <summary>
        //    ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        //    /// </summary>
        //    /// <param name="vector"></param>
        //    /// <param name="maxLength"></param>
        //    //public static Vector2New ClampMagnitude(Vector2New vector, float maxLength)
        //    //{
        //    //    if (vector.sqrMagnitude > maxLength * maxLength)
        //    //    {
        //    //        return vector.normalized * maxLength;
        //    //    }
        //    //    return vector;
        //    //}

        //    public static double SqrMagnitude(Vector2New a)
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
        //    public static Vector2New Min(Vector2New lhs, Vector2New rhs)
        //    {
        //        return new Vector2New(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        //    }

        //    /// <summary>
        //    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
        //    /// </summary>
        //    /// <param name="lhs"></param>
        //    /// <param name="rhs"></param>
        //    public static Vector2New Max(Vector2New lhs, Vector2New rhs)
        //    {
        //        return new Vector2New(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        //    }

        //    //public static Vector2New SmoothDamp(Vector2New current, Vector2New target, ref Vector2New currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        //    //{
        //    //    smoothTime = Math.Max(0.0001f, smoothTime);
        //    //    float num = 2f / smoothTime;
        //    //    float num2 = num * deltaTime;
        //    //    float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        //    //    Vector2New vector = current - target;
        //    //    Vector2New Vector2New = target;
        //    //    float maxLength = maxSpeed * smoothTime;
        //    //    vector = ClampMagnitude(vector, maxLength);
        //    //    target = current - vector;
        //    //    Vector2New vector3 = (currentVelocity + num * vector) * deltaTime;
        //    //    currentVelocity = (currentVelocity - num * vector3) * d;
        //    //    Vector2New vector4 = target + (vector + vector3) * d;
        //    //    if (Dot(Vector2New - current, vector4 - Vector2New) > 0f)
        //    //    {
        //    //        vector4 = Vector2New;
        //    //        currentVelocity = (vector4 - Vector2New) / deltaTime;
        //    //    }
        //    //    return vector4;
        //    //}

        // Token: 0x060010B8 RID: 4280 RVA: 0x0001A7E4 File Offset: 0x000189E4
        public static Vector2New operator +(Vector2New a, Vector2New b)
        {
            return new Vector2New(a.x + b.x, a.y + b.y);
        }

        // Token: 0x060010B9 RID: 4281 RVA: 0x0001A818 File Offset: 0x00018A18
        public static Vector2New operator -(Vector2New a, Vector2New b)
        {
            return new Vector2New(a.x - b.x, a.y - b.y);
        }

        // Token: 0x060010BA RID: 4282 RVA: 0x0001A84C File Offset: 0x00018A4C
        public static Vector2New operator *(Vector2New a, Vector2New b)
        {
            return new Vector2New(a.x * b.x, a.y * b.y);
        }

        // Token: 0x060010BB RID: 4283 RVA: 0x0001A880 File Offset: 0x00018A80
        public static Vector2New operator /(Vector2New a, Vector2New b)
        {
            return new Vector2New(a.x / b.x, a.y / b.y);
        }

        // Token: 0x060010BC RID: 4284 RVA: 0x0001A8B4 File Offset: 0x00018AB4
        public static Vector2New operator -(Vector2New a)
        {
            return new Vector2New(-a.x, -a.y);
        }

        // Token: 0x060010BD RID: 4285 RVA: 0x0001A8DC File Offset: 0x00018ADC
        public static Vector2New operator *(Vector2New a, float d)
        {
            return new Vector2New(a.x * d, a.y * d);
        }

        // Token: 0x060010BE RID: 4286 RVA: 0x0001A904 File Offset: 0x00018B04
        public static Vector2New operator *(float d, Vector2New a)
        {
            return new Vector2New(a.x * d, a.y * d);
        }

        // Token: 0x060010BF RID: 4287 RVA: 0x0001A92C File Offset: 0x00018B2C
        public static Vector2New operator /(Vector2New a, float d)
        {
            return new Vector2New(a.x / d, a.y / d);
        }

        // Token: 0x060010C0 RID: 4288 RVA: 0x0001A954 File Offset: 0x00018B54
        public static bool operator ==(Vector2New lhs, Vector2New rhs)
        {
            float num = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            return num * num + num2 * num2 < 9.9999994E-11f;
        }

        // Token: 0x060010C1 RID: 4289 RVA: 0x0001A990 File Offset: 0x00018B90
        public static bool operator !=(Vector2New lhs, Vector2New rhs)
        {
            return !(lhs == rhs);
        }

        //// Token: 0x060010C2 RID: 4290 RVA: 0x0001A9AC File Offset: 0x00018BAC
        //public static implicit operator Vector2New(Vector3 v)
        //{
        //    return new Vector2New(v.x, v.y);
        //}

        //// Token: 0x060010C3 RID: 4291 RVA: 0x0001A9D0 File Offset: 0x00018BD0
        //public static implicit operator Vector3(Vector2New v)
        //{
        //    return new Vector3(v.x, v.y, 0f);
        //}
    }

}
