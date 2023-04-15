using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrandedDeepMapper
{
    public static class ZonePositionGeneratorOld
    {
        // Token: 0x060006B7 RID: 1719 RVA: 0x00020D8C File Offset: 0x0001EF8C
        public static Vector2New[] GeneratePoints(int WORLD_SEED, float radius, Vector2New sampleRegionSize, int numSamplesBeforeRejection = 30)
        {
            float num = radius / Mathf.Sqrt(2f);
            int[,] array = new int[Mathf.CeilToInt(sampleRegionSize.x / num), Mathf.CeilToInt(sampleRegionSize.y / num)];
            List<Vector2New> list = new List<Vector2New>();
            List<Vector2New> list2 = new List<Vector2New>();
            list2.Add(sampleRegionSize / 2f);
            FastRandom fastRandom = new FastRandom(WORLD_SEED);
            while (list2.Count > 0)
            {
                int index = fastRandom.Next(0, list2.Count);
                Vector2New vector = list2[index];
                bool flag = false;
                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float f = (float)fastRandom.NextDouble() * 3.1415927f * 2f;
                    Vector2New a = new Vector2New(Mathf.Sin(f), Mathf.Cos(f));
                    Vector2New Vector2New = vector + a * (float)(fastRandom.Next((int)(radius * 100f), (int)(1.25f * radius * 100f)) / 100);
                    if (i == 0)
                    {
                        Vector2New = vector;
                    }
                    if (ZonePositionGeneratorOld.IsValid(Vector2New, sampleRegionSize, num, radius, list, array))
                    {
                        list.Add(Vector2New);
                        list2.Add(Vector2New);
                        array[(int)(Vector2New.x / num), (int)(Vector2New.y / num)] = list.Count;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list2.RemoveAt(index);
                }
            }
            Vector2New[] array2 = new Vector2New[list.Count];
            for (int j = 0; j < list.Count; j++)
            {
                float num2 = sampleRegionSize.x * 0.5f;
                array2[j] = new Vector2New(list[j].x - num2, list[j].y - num2);
            }
            return array2;
        }

        // Token: 0x060006B8 RID: 1720 RVA: 0x00020F44 File Offset: 0x0001F144
        private static bool IsValid(Vector2New candidate, Vector2New sampleRegionSize, float cellSize, float radius, List<Vector2New> points, int[,] grid)
        {
            if (candidate.x >= 0f && candidate.x < sampleRegionSize.x && candidate.y >= 0f && candidate.y < sampleRegionSize.y)
            {
                int num = (int)(candidate.x / cellSize);
                int num2 = (int)(candidate.y / cellSize);
                int num3 = Mathf.Max(0, num - 2);
                int num4 = Mathf.Min(num + 2, grid.GetLength(0) - 1);
                int num5 = Mathf.Max(0, num2 - 2);
                int num6 = Mathf.Min(num2 + 2, grid.GetLength(1) - 1);
                for (int i = num3; i <= num4; i++)
                {
                    for (int j = num5; j <= num6; j++)
                    {
                        int num7 = grid[i, j] - 1;
                        if (num7 != -1 && (candidate - points[num7]).sqrMagnitude < radius * radius)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
