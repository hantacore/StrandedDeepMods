using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrandedDeepMapper
{
    class FastRandomNew
    {
        // Token: 0x06002B6C RID: 11116 RVA: 0x00081AEF File Offset: 0x0007FCEF
        public FastRandomNew()
        {
            this.Reinitialise(Environment.TickCount);
        }

        // Token: 0x06002B6D RID: 11117 RVA: 0x00081B09 File Offset: 0x0007FD09
        public FastRandomNew(int seed)
        {
            this.Reinitialise(seed);
        }

        // Token: 0x06002B6E RID: 11118 RVA: 0x00081B1F File Offset: 0x0007FD1F
        public void Reinitialise(int seed)
        {
            this.x = (uint)seed;
            this.y = 842502087U;
            this.z = 3579807591U;
            this.w = 273326509U;
        }

        // Token: 0x06002B6F RID: 11119 RVA: 0x00081B4C File Offset: 0x0007FD4C
        public int Next()
        {
            uint num = this.x ^ this.x << 11;
            this.x = this.y;
            this.y = this.z;
            this.z = this.w;
            this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8));
            uint num2 = this.w & 2147483647U;
            if (num2 == 2147483647U)
            {
                return this.Next();
            }
            return (int)num2;
        }

        // Token: 0x06002B70 RID: 11120 RVA: 0x00081BC8 File Offset: 0x0007FDC8
        public int Next(int upperBound)
        {
            if (upperBound < 0)
            {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
            }
            uint num = this.x ^ this.x << 11;
            this.x = this.y;
            this.y = this.z;
            this.z = this.w;
            return (int)(4.656612873077393E-10 * (double)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8)))) * (double)upperBound);
        }

        // Token: 0x06002B71 RID: 11121 RVA: 0x00081C58 File Offset: 0x0007FE58
        public int Next(int lowerBound, int upperBound)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
            }
            uint num = this.x ^ this.x << 11;
            this.x = this.y;
            this.y = this.z;
            this.z = this.w;
            int num2 = upperBound - lowerBound;
            if (num2 < 0)
            {
                return lowerBound + (int)(2.3283064365386963E-10 * (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8))) * (double)((long)upperBound - (long)lowerBound));
            }
            return lowerBound + (int)(4.656612873077393E-10 * (double)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8)))) * (double)num2);
        }

        // Token: 0x06002B72 RID: 11122 RVA: 0x00081D28 File Offset: 0x0007FF28
        public double NextDouble()
        {
            uint num = this.x ^ this.x << 11;
            this.x = this.y;
            this.y = this.z;
            this.z = this.w;
            return 4.656612873077393E-10 * (double)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8))));
        }

        // Token: 0x06002B73 RID: 11123 RVA: 0x00081D9C File Offset: 0x0007FF9C
        public void NextBytes(byte[] buffer)
        {
            uint num = this.x;
            uint num2 = this.y;
            uint num3 = this.z;
            uint num4 = this.w;
            int i = 0;
            int num5 = buffer.Length - 3;
            while (i < num5)
            {
                uint num6 = num ^ num << 11;
                num = num2;
                num2 = num3;
                num3 = num4;
                num4 = (num4 ^ num4 >> 19 ^ (num6 ^ num6 >> 8));
                buffer[i++] = (byte)num4;
                buffer[i++] = (byte)(num4 >> 8);
                buffer[i++] = (byte)(num4 >> 16);
                buffer[i++] = (byte)(num4 >> 24);
            }
            if (i < buffer.Length)
            {
                uint num6 = num ^ num << 11;
                num = num2;
                num2 = num3;
                num3 = num4;
                num4 = (num4 ^ num4 >> 19 ^ (num6 ^ num6 >> 8));
                buffer[i++] = (byte)num4;
                if (i < buffer.Length)
                {
                    buffer[i++] = (byte)(num4 >> 8);
                    if (i < buffer.Length)
                    {
                        buffer[i++] = (byte)(num4 >> 16);
                        if (i < buffer.Length)
                        {
                            buffer[i] = (byte)(num4 >> 24);
                        }
                    }
                }
            }
            this.x = num;
            this.y = num2;
            this.z = num3;
            this.w = num4;
        }

        // Token: 0x06002B74 RID: 11124 RVA: 0x00081EAC File Offset: 0x000800AC
        public uint NextUInt()
        {
            uint num = this.x ^ this.x << 11;
            this.x = this.y;
            this.y = this.z;
            this.z = this.w;
            return this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8));
        }

        // Token: 0x06002B75 RID: 11125 RVA: 0x00081F10 File Offset: 0x00080110
        public int NextInt()
        {
            uint num = this.x ^ this.x << 11;
            this.x = this.y;
            this.y = this.z;
            this.z = this.w;
            return (int)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8))));
        }

        // Token: 0x06002B76 RID: 11126 RVA: 0x00081F78 File Offset: 0x00080178
        public bool NextBool()
        {
            if (this.bitMask == 1U)
            {
                uint num = this.x ^ this.x << 11;
                this.x = this.y;
                this.y = this.z;
                this.z = this.w;
                this.bitBuffer = (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8)));
                this.bitMask = 2147483648U;
                return (this.bitBuffer & this.bitMask) == 0U;
            }
            return (this.bitBuffer & (this.bitMask >>= 1)) == 0U;
        }

        // Token: 0x04001A01 RID: 6657
        private const double REAL_UNIT_INT = 4.656612873077393E-10;

        // Token: 0x04001A02 RID: 6658
        private const double REAL_UNIT_UINT = 2.3283064365386963E-10;

        // Token: 0x04001A03 RID: 6659
        private const uint Y = 842502087U;

        // Token: 0x04001A04 RID: 6660
        private const uint Z = 3579807591U;

        // Token: 0x04001A05 RID: 6661
        private const uint W = 273326509U;

        // Token: 0x04001A06 RID: 6662
        private uint x;

        // Token: 0x04001A07 RID: 6663
        private uint y;

        // Token: 0x04001A08 RID: 6664
        private uint z;

        // Token: 0x04001A09 RID: 6665
        private uint w;

        // Token: 0x04001A0A RID: 6666
        private uint bitBuffer;

        // Token: 0x04001A0B RID: 6667
        private uint bitMask = 1U;
    }
}
