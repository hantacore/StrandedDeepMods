using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StrandedDeepMapper
{
    public static class ZonePositionGenerator
    {
        public static Vector2[] GeneratePoints(int WORLD_SEED, double radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
        {
            //string path = @"FastRandomLog.txt";

            double num = radius / Math.Sqrt(2f);
            //int[,] array = new int[Math.CeilToInt(sampleRegionSize.x / num), Math.CeilToInt(sampleRegionSize.y / num)];
            int[,] array = new int[(int)Math.Ceiling(sampleRegionSize.x / num), (int)Math.Ceiling(sampleRegionSize.y / num)];
            List<Vector2> list = new List<Vector2>();
            List<Vector2> list2 = new List<Vector2>();
            list2.Add(sampleRegionSize / 2f);
            //FastRandom fastRandom = new FastRandom(WORLD_SEED);
            FastRandomNew fastRandom = new FastRandomNew(WORLD_SEED);
            while (list2.Count > 0)
            {
                int index = fastRandom.Next(0, list2.Count);
                Vector2 vector = list2[index];
                bool flag = false;
                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    double randomValue = fastRandom.NextDouble();

                    float test = (float)randomValue;
                    float test2 = (float)Math.PI;
                    float test3 = test * test2 * 2f;

                    float f = (float)randomValue * (float)Math.PI * 2f;
                    Vector2 a = new Vector2(Math.Sin(f), Math.Cos(f));
                    int nextRandom = fastRandom.Next((int)(radius * 100f), (int)(1.25f * radius * 100f));
                    Vector2 vectortemp = a * (nextRandom / 100);
                    Vector2 vector2 = vector + vectortemp;

                    //using (StreamWriter sw = File.AppendText(path))
                    //{
                    //    sw.WriteLine("sin/cos$" + Math.Sin(f) + "$" + Math.Cos(f));
                    //    sw.WriteLine(randomValue + "$" + f + "$" + a.x + "$" + a.y + "$" + nextRandom + "$" + vector2.x + "$" + vector2.y);
                    //}

                    if (i == 0)
                    {
                        vector2 = vector;
                    }
                    bool isValid = IsValid(vector2, sampleRegionSize, num, radius, list, array);
                    //using (StreamWriter sw = File.AppendText(path))
                    //{
                    //    sw.WriteLine("IsValid " + vector2.x + "/" + vector2.y + " /" + isValid);
                    //}
                    if (isValid)
                    {
                        list.Add(vector2);
                        list2.Add(vector2);
                        array[(int)(vector2.x / num), (int)(vector2.y / num)] = list.Count;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list2.RemoveAt(index);
                }

                //using (StreamWriter sw = File.AppendText(path))
                //{
                //    sw.WriteLine("list2 length = " + list2.Count);
                //}
            }
            Vector2[] array2 = new Vector2[list.Count];
            for (int j = 0; j < list.Count; j++)
            {
                double num2 = sampleRegionSize.x * 0.5f;
                ref Vector2 reference = ref array2[j];
                Vector2 vector3 = list[j];
                double x = vector3.x - num2;
                Vector2 vector4 = list[j];
                reference = new Vector2(x, vector4.y - num2);
            }
            return array2;
        }

        private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, double cellSize, double radius, List<Vector2> points, int[,] grid)
        {
            //string path = @"FastRandomLog.txt";
            //using (StreamWriter sw = File.AppendText(path))
            //{
                if (candidate.x >= 0f && candidate.x < sampleRegionSize.x && candidate.y >= 0f && candidate.y < sampleRegionSize.y)
                {
                    int num = (int)(candidate.x / cellSize);
                    int num2 = (int)(candidate.y / cellSize);
                    int num3 = Math.Max(0, num - 2);
                    int num4 = Math.Min(num + 2, grid.GetLength(0) - 1);
                    int num5 = Math.Max(0, num2 - 2);
                    int num6 = Math.Min(num2 + 2, grid.GetLength(1) - 1);
                    //sw.WriteLine("IsValid : num " + num + " / num2 " + num2 + " / num3 " + num3 + " / num4 " + num4 + " / num5 " + num5 + " / num6 " + num6);
                    for (int i = num3; i <= num4; i++)
                    {
                        for (int j = num5; j <= num6; j++)
                        {
                            int num7 = grid[i, j] - 1;
                            if (num7 != -1)
                            {
                                double sqrMagnitude = (candidate - points[num7]).sqrMagnitude;
                                //sw.WriteLine("sqmagnitude = " + sqrMagnitude.ToString("G17") + " < radius2 = " + (radius * radius) + "$" + points[num7].x + "$" + points[num7].y + "$" + candidate.x + "$" + candidate.y);
                                if (sqrMagnitude < radius * radius)
                                {
                                    //sw.WriteLine("IsValid : " + candidate.x + " / " + candidate.y + " / false (sqmagnitude=" + sqrMagnitude.ToString("G17") + " < radius2=" + (radius * radius) + ")");
                                    return false;
                                }
                            }
                        }
                    }
                    //sw.WriteLine("IsValid : " + candidate.x + " / " + candidate.y + " true ");
                    return true;
                }
                //sw.WriteLine("IsValid : " + candidate.x + " / " + candidate.y + " / false (sampleRegionSize.x=" + sampleRegionSize.x + " / sampleRegionSize.y=" + sampleRegionSize.y + ")");
                return false;
            }
        //}
    }
}
