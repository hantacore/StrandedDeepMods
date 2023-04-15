using Beam;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedWideMod_Harmony
{
    public static partial class Main
    {
        [HarmonyPatch(typeof(MapTools), "CreateHeightmapTexture")]
        class MapTools_CreateHeightmapTexture_Patch
        {
            static bool Prefix(float[,] heightData, ref Texture2D __result)
            {
                try
                {
                    Texture2D texture2D = new Texture2D(_islandSize + 1, _islandSize + 1, TextureFormat.ARGB32, false, false);

#warning missions and old maps test
                    if (heightData.GetLength(0) < _islandSize + 1)
                    {
                        float minimum = heightData[0, 0];
                        // init all with base value
                        for (int i = 0; i < _islandSize + 1; i++)
                        {
                            for (int j = 0; j < _islandSize + 1; j++)
                            {
                                float num = minimum;
                                float a = 1f;
                                if (num * 150f - (float)Mathf.Abs(-100) < 0f)
                                {
                                    num = 0f;
                                    a = 0f;
                                }
                                Color color = new Color(num, num, num, a);
                                texture2D.SetPixel(j, i, color);
                            }
                        }

                        // get the effective values
                        for (int i = 0; i < heightData.GetLength(0); i++)
                        {
                            for (int j = 0; j < heightData.GetLength(0); j++)
                            {
                                float num = heightData[i, j];
                                float a = 1f;
                                if (num * 150f - (float)Mathf.Abs(-100) < 0f)
                                {
                                    num = 0f;
                                    a = 0f;
                                }
                                Color color = new Color(num, num, num, a);
                                texture2D.SetPixel(j, i, color);
                            }
                        }
#warning end missions and old maps test
                    }
                    else
                    {
                        // original code
                        for (int i = 0; i < _islandSize + 1; i++)
                        {
                            for (int j = 0; j < _islandSize + 1; j++)
                            {
                                float num = heightData[i, j];
                                float a = 1f;
                                if (num * 150f - (float)Mathf.Abs(-100) < 0f)
                                {
                                    num = 0f;
                                    a = 0f;
                                }
                                Color color = new Color(num, num, num, a);
                                texture2D.SetPixel(j, i, color);
                            }
                        }
                    }
                    texture2D.Apply();
                    __result = texture2D;
                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Wide (Harmony edition) : error while patching MapTools.CreateHeightmapTexture : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MapTools), "GetHeightsFromTexture")]
        class MapTools_GetHeightsFromTexture_Patch
        {
            static bool Prefix(Texture2D heightmap, ref float[,] __result)
            {
                try
                {
                    float[,] array = new float[_islandSize + 1, _islandSize + 1];
                    Color[] pixels = heightmap.GetPixels();

#warning missions and old maps test
                    if (heightmap.width < array.GetLength(0))
                    {
                        // init all with base value
                        for (int i = 0; i < _islandSize + 1; i++)
                        {
                            for (int j = 0; j < _islandSize + 1; j++)
                            {
                                Color color = pixels[0];
                                array[j, i] = color.grayscale;
                            }
                        }

                        // get the effective values
                        for (int i = 0; i < heightmap.width; i++)
                        {
                            for (int j = 0; j < heightmap.width; j++)
                            {
                                Color color = pixels[j * _islandSize + 1 + i];
                                array[j, i] = color.grayscale;
                            }
                        }
                    }
                    else
                    {
                        // original code
                        for (int i = 0; i < _islandSize + 1; i++)
                        {
                            for (int j = 0; j < _islandSize + 1; j++)
                            {
                                Color color = pixels[j * _islandSize + 1 + i];
                                array[j, i] = color.grayscale;
                            }
                        }
                    }
#warning end missions and old maps test

                    __result = array;
                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Wide (Harmony edition) : error while patching MapTools.GetHeightsFromTexture : " + e);
                }
                return true;
            }
        }
    }
}
