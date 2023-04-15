using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrandedDeepMapper
{
    public struct Mathf
    {
        // Token: 0x06001060 RID: 4192 RVA: 0x00019430 File Offset: 0x00017630
        public static float Sin(float f)
        {
            return (float)Math.Sin((double)f);
        }

        // Token: 0x06001061 RID: 4193 RVA: 0x0001944C File Offset: 0x0001764C
        public static float Cos(float f)
        {
            return (float)Math.Cos((double)f);
        }

        // Token: 0x06001062 RID: 4194 RVA: 0x00019468 File Offset: 0x00017668
        public static float Tan(float f)
        {
            return (float)Math.Tan((double)f);
        }

        // Token: 0x06001063 RID: 4195 RVA: 0x00019484 File Offset: 0x00017684
        public static float Asin(float f)
        {
            return (float)Math.Asin((double)f);
        }

        // Token: 0x06001064 RID: 4196 RVA: 0x000194A0 File Offset: 0x000176A0
        public static float Acos(float f)
        {
            return (float)Math.Acos((double)f);
        }

        // Token: 0x06001065 RID: 4197 RVA: 0x000194BC File Offset: 0x000176BC
        public static float Atan(float f)
        {
            return (float)Math.Atan((double)f);
        }

        // Token: 0x06001066 RID: 4198 RVA: 0x000194D8 File Offset: 0x000176D8
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2((double)y, (double)x);
        }

        // Token: 0x06001067 RID: 4199 RVA: 0x000194F4 File Offset: 0x000176F4
        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt((double)f);
        }

        // Token: 0x06001068 RID: 4200 RVA: 0x00019510 File Offset: 0x00017710
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        // Token: 0x06001069 RID: 4201 RVA: 0x0001952C File Offset: 0x0001772C
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        // Token: 0x0600106A RID: 4202 RVA: 0x00019544 File Offset: 0x00017744
        public static float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

        // Token: 0x0600106B RID: 4203 RVA: 0x00019560 File Offset: 0x00017760
        public static float Min(params float[] values)
        {
            int num = values.Length;
            bool flag = num == 0;
            float result;
            if (flag)
            {
                result = 0f;
            }
            else
            {
                float num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    bool flag2 = values[i] < num2;
                    if (flag2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x0600106C RID: 4204 RVA: 0x000195B8 File Offset: 0x000177B8
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        // Token: 0x0600106D RID: 4205 RVA: 0x000195D4 File Offset: 0x000177D4
        public static int Min(params int[] values)
        {
            int num = values.Length;
            bool flag = num == 0;
            int result;
            if (flag)
            {
                result = 0;
            }
            else
            {
                int num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    bool flag2 = values[i] < num2;
                    if (flag2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x0600106E RID: 4206 RVA: 0x00019628 File Offset: 0x00017828
        public static float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }

        // Token: 0x0600106F RID: 4207 RVA: 0x00019644 File Offset: 0x00017844
        public static float Max(params float[] values)
        {
            int num = values.Length;
            bool flag = num == 0;
            float result;
            if (flag)
            {
                result = 0f;
            }
            else
            {
                float num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    bool flag2 = values[i] > num2;
                    if (flag2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x06001070 RID: 4208 RVA: 0x0001969C File Offset: 0x0001789C
        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        // Token: 0x06001071 RID: 4209 RVA: 0x000196B8 File Offset: 0x000178B8
        public static int Max(params int[] values)
        {
            int num = values.Length;
            bool flag = num == 0;
            int result;
            if (flag)
            {
                result = 0;
            }
            else
            {
                int num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    bool flag2 = values[i] > num2;
                    if (flag2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x06001072 RID: 4210 RVA: 0x0001970C File Offset: 0x0001790C
        public static float Pow(float f, float p)
        {
            return (float)Math.Pow((double)f, (double)p);
        }

        // Token: 0x06001073 RID: 4211 RVA: 0x00019728 File Offset: 0x00017928
        public static float Exp(float power)
        {
            return (float)Math.Exp((double)power);
        }

        // Token: 0x06001074 RID: 4212 RVA: 0x00019744 File Offset: 0x00017944
        public static float Log(float f, float p)
        {
            return (float)Math.Log((double)f, (double)p);
        }

        // Token: 0x06001075 RID: 4213 RVA: 0x00019760 File Offset: 0x00017960
        public static float Log(float f)
        {
            return (float)Math.Log((double)f);
        }

        // Token: 0x06001076 RID: 4214 RVA: 0x0001977C File Offset: 0x0001797C
        public static float Log10(float f)
        {
            return (float)Math.Log10((double)f);
        }

        // Token: 0x06001077 RID: 4215 RVA: 0x00019798 File Offset: 0x00017998
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling((double)f);
        }

        // Token: 0x06001078 RID: 4216 RVA: 0x000197B4 File Offset: 0x000179B4
        public static float Floor(float f)
        {
            return (float)Math.Floor((double)f);
        }

        // Token: 0x06001079 RID: 4217 RVA: 0x000197D0 File Offset: 0x000179D0
        public static float Round(float f)
        {
            return (float)Math.Round((double)f);
        }

        // Token: 0x0600107A RID: 4218 RVA: 0x000197EC File Offset: 0x000179EC
        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling((double)f);
        }

        // Token: 0x0600107B RID: 4219 RVA: 0x00019808 File Offset: 0x00017A08
        public static int FloorToInt(float f)
        {
            return (int)Math.Floor((double)f);
        }

        // Token: 0x0600107C RID: 4220 RVA: 0x00019824 File Offset: 0x00017A24
        public static int RoundToInt(float f)
        {
            return (int)Math.Round((double)f);
        }

        // Token: 0x0600107D RID: 4221 RVA: 0x00019840 File Offset: 0x00017A40
        public static float Sign(float f)
        {
            return (f >= 0f) ? 1f : -1f;
        }

        // Token: 0x0600107E RID: 4222 RVA: 0x00019868 File Offset: 0x00017A68
        public static float Clamp(float value, float min, float max)
        {
            bool flag = value < min;
            if (flag)
            {
                value = min;
            }
            else
            {
                bool flag2 = value > max;
                if (flag2)
                {
                    value = max;
                }
            }
            return value;
        }

        // Token: 0x0600107F RID: 4223 RVA: 0x00019894 File Offset: 0x00017A94
        public static int Clamp(int value, int min, int max)
        {
            bool flag = value < min;
            if (flag)
            {
                value = min;
            }
            else
            {
                bool flag2 = value > max;
                if (flag2)
                {
                    value = max;
                }
            }
            return value;
        }

        // Token: 0x06001080 RID: 4224 RVA: 0x000198C0 File Offset: 0x00017AC0
        public static float Clamp01(float value)
        {
            bool flag = value < 0f;
            float result;
            if (flag)
            {
                result = 0f;
            }
            else
            {
                bool flag2 = value > 1f;
                if (flag2)
                {
                    result = 1f;
                }
                else
                {
                    result = value;
                }
            }
            return result;
        }

        // Token: 0x06001081 RID: 4225 RVA: 0x000198FC File Offset: 0x00017AFC
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }

        // Token: 0x06001082 RID: 4226 RVA: 0x0001991C File Offset: 0x00017B1C
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        // Token: 0x06001083 RID: 4227 RVA: 0x00019938 File Offset: 0x00017B38
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Mathf.Repeat(b - a, 360f);
            bool flag = num > 180f;
            if (flag)
            {
                num -= 360f;
            }
            return a + num * Mathf.Clamp01(t);
        }

        // Token: 0x06001084 RID: 4228 RVA: 0x00019978 File Offset: 0x00017B78
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            bool flag = Mathf.Abs(target - current) <= maxDelta;
            float result;
            if (flag)
            {
                result = target;
            }
            else
            {
                result = current + Mathf.Sign(target - current) * maxDelta;
            }
            return result;
        }

        // Token: 0x06001085 RID: 4229 RVA: 0x000199AC File Offset: 0x00017BAC
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = Mathf.DeltaAngle(current, target);
            bool flag = -maxDelta < num && num < maxDelta;
            float result;
            if (flag)
            {
                result = target;
            }
            else
            {
                target = current + num;
                result = Mathf.MoveTowards(current, target, maxDelta);
            }
            return result;
        }

        // Token: 0x06001086 RID: 4230 RVA: 0x000199E8 File Offset: 0x00017BE8
        public static float SmoothStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        // Token: 0x06001087 RID: 4231 RVA: 0x00019A28 File Offset: 0x00017C28
        public static float Gamma(float value, float absmax, float gamma)
        {
            bool flag = false;
            bool flag2 = value < 0f;
            if (flag2)
            {
                flag = true;
            }
            float num = Mathf.Abs(value);
            bool flag3 = num > absmax;
            float result;
            if (flag3)
            {
                result = (flag ? (-num) : num);
            }
            else
            {
                float num2 = Mathf.Pow(num / absmax, gamma) * absmax;
                result = (flag ? (-num2) : num2);
            }
            return result;
        }

        // Token: 0x06001088 RID: 4232 RVA: 0x00019A80 File Offset: 0x00017C80
        //public static bool Approximately(float a, float b)
        //{
        //    return Mathf.Abs(b - a) < Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
        //}

        // Token: 0x0600108F RID: 4239 RVA: 0x00019C4C File Offset: 0x00017E4C
        public static float Repeat(float t, float length)
        {
            return Mathf.Clamp(t - Mathf.Floor(t / length) * length, 0f, length);
        }

        // Token: 0x06001090 RID: 4240 RVA: 0x00019C78 File Offset: 0x00017E78
        public static float PingPong(float t, float length)
        {
            t = Mathf.Repeat(t, length * 2f);
            return length - Mathf.Abs(t - length);
        }

        // Token: 0x06001091 RID: 4241 RVA: 0x00019CA4 File Offset: 0x00017EA4
        public static float InverseLerp(float a, float b, float value)
        {
            bool flag = a != b;
            float result;
            if (flag)
            {
                result = Mathf.Clamp01((value - a) / (b - a));
            }
            else
            {
                result = 0f;
            }
            return result;
        }

        // Token: 0x06001092 RID: 4242 RVA: 0x00019CD8 File Offset: 0x00017ED8
        public static float DeltaAngle(float current, float target)
        {
            float num = Mathf.Repeat(target - current, 360f);
            bool flag = num > 180f;
            if (flag)
            {
                num -= 360f;
            }
            return num;
        }

        // Token: 0x06001095 RID: 4245 RVA: 0x00019EDC File Offset: 0x000180DC
        internal static long RandomToLong(Random r)
        {
            byte[] array = new byte[8];
            r.NextBytes(array);
            return (long)(BitConverter.ToUInt64(array, 0) & 9223372036854775807UL);
        }

        // Token: 0x040005B3 RID: 1459
        public const float PI = 3.1415927f;

        // Token: 0x040005B4 RID: 1460
        public const float Infinity = float.PositiveInfinity;

        // Token: 0x040005B5 RID: 1461
        public const float NegativeInfinity = float.NegativeInfinity;

        // Token: 0x040005B6 RID: 1462
        public const float Deg2Rad = 0.017453292f;

        // Token: 0x040005B7 RID: 1463
        public const float Rad2Deg = 57.29578f;
    }
}
