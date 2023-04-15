using Beam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepBetterGroundTexturesMod
{
    static class Main
    {
        private static string configFileName = "StrandedDeepBetterGroundTexturesMod.config";
        private static List<Zone> _handledZones = new List<Zone>();
        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();

        //public static AssetBundle myAssets;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                modEntry.OnUpdate = OnUpdate;
                //modEntry.OnGUI = OnGUI;
                //modEntry.OnHideGUI = OnHideGUI;

                ReadConfig();

                bool result = PreLoadTextures();

                //List<string> textures = new List<string>();
                //textures.Add("Assets/Textures/Terrain_Sand_Underwater4K.png");
                //textures.Add("Assets/Textures/Terrain_Sand_Underwater2K_PACK.png");

                //textures.Add("Assets/Textures/Terrain_Dirt_Sticks_DIFF4K.png");
                ////textures.Add("Assets/Textures/Terrain_Dirt_Sticks_PACK4K.png");

                ////textures.Add("Assets/Textures/Terrain_Dirt_Sticks_DIFF4K.psd");
                //textures.Add("Assets/Textures/Terrain_Dirt_Sticks_PACK4K.psd");

                //textures.Add("Assets/Textures/Terrain_Grass_Dirt_DIFF4K.png");
                //textures.Add("Assets/Textures/Terrain_Grass_Dirt_PACK4K.png");
                ////textures.Add("Assets/Textures/Terrain_Grass_Dirt_PACK4K.jpg");
                ////textures.Add("Assets/Textures/Terrain_Grass_Dirt_DIFF2K.png");
                ////textures.Add("Assets/Textures/Terrain_Grass_Dirt_PACK2K.png");

                //textures.Add("Assets/Textures/Terrain_Sand_Clean_DIFF4K.png");
                //textures.Add("Assets/Textures/Terrain_Sand_Clean_PACK1K.png");

                //textures.Add("Assets/Textures/Terrain_Sand_Dirty_DIFF4K.png");
                //textures.Add("Assets/Textures/Terrain_Sand_Dirty_PACK4K.png");

                ////textures.Add("Assets/Textures/Terrain_Sand_Dirty_PACK4K.psd");

                //textures.Add("Assets/Textures/Terrain_Sand_Wet_DIFF4K.png");
                //textures.Add("Assets/Textures/Terrain_Sand_Wet_NRM4K.png");

                //foreach (string texturepath in textures)
                //{
                //    Texture2D texture = myAssets.LoadAsset<Texture2D>(texturepath);



                //    if (texture != null)
                //    {
                //        Debug.Log("Stranded Deep Better Ground Textures Mod : successfully loaded texture asset " + texture);

                //        //try
                //        //{
                //        //    Debug.Log("Stranded Deep Better Ground Textures Mod : texture name " + texture.name);
                //        //    foreach (var prop in texture.GetType().GetProperties())
                //        //    {
                //        //        Console.WriteLine("Stranded Deep Better Ground Textures Mod : TEXTURE PROPERTY = {0}={1}", prop.Name, prop.GetValue(texture, null));
                //        //    }
                //        //}
                //        //catch { }
                //    }
                //    else
                //    {
                //        Debug.Log("Stranded Deep Better Ground Textures Mod : NOT loaded texture asset " + texture);
                //    }

                //    string texKey = texturepath.Replace("Assets/Textures/", "");
                //    texKey = texKey.Replace("4K", "").Replace("2K", "").Replace("1K", "").Replace(".psd", ".png");

                //    string fullKey = "StrandedDeepBetterGroundTexturesMod.assets.Textures." + texKey;
                //    Debug.Log("Stranded Deep Better Ground Textures Mod : preloaded " + fullKey);
                //    _indexedTextures.Add(fullKey, texture);
                //}

                //Mesh mesh = myAssets.LoadAsset<Mesh>("Assets/Meshes/Low Grass.obj");

                //Texture2D Terrain_Sand_Underwater4K = myAssets.LoadAsset<Texture2D>("Assets/Textures/Terrain_Sand_Underwater4K.png");
                //if (Terrain_Sand_Underwater4K != null)
                //{
                //    Debug.Log("Stranded Deep Better Ground Textures Mod : successfully loaded texture asset Terrain_Sand_Underwater4K.png");
                //}
                //else
                //{
                //    Debug.Log("Stranded Deep Better Ground Textures Mod : NOT loaded texture asset Terrain_Sand_Underwater4K.png");
                //}
                //_indexedTextures.Add("StrandedDeepBetterGroundTexturesMod.assets.Textures.Terrain_Sand_Underwater.png", Terrain_Sand_Underwater4K);
                //Texture2D Terrain_Sand_Underwater2K_PACK = myAssets.LoadAsset<Texture2D>("Assets/Textures/Terrain_Sand_Underwater2K_PACK.png");
                //if (Terrain_Sand_Underwater4K != null)
                //{
                //    Debug.Log("Stranded Deep Better Ground Textures Mod : successfully loaded texture asset Terrain_Sand_Underwater2K_PACK.png");
                //}
                //else
                //{
                //    Debug.Log("Stranded Deep Better Ground Textures Mod : NOT loaded texture asset Terrain_Sand_Underwater2K_PACK");
                //}
                //_indexedTextures.Add("StrandedDeepBetterGroundTexturesMod.assets.Textures.Terrain_Sand_Underwater_PACK.png", Terrain_Sand_Underwater2K_PACK);

                if (result)
                {
                    Debug.Log("Stranded Deep Better Ground Textures Mod properly loaded");
                    return result;
                }
            }
            catch(Exception ex)
            {
                Debug.Log("Stranded Deep Better Ground Textures Mod exception on load : " + ex);
            }
            Debug.Log("Stranded Deep Better Ground Textures Mod failed to load");
            return false;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            //GUILayout.Label("Blending ratio");
            //_blendingRatio = GUILayout.HorizontalSlider(_blendingRatio, 0.1f, 10f);
            //GUILayout.Label("Height ratio");
            //_heightRatio = GUILayout.HorizontalSlider(_heightRatio, 0.1f, 10f);
            //GUILayout.Label("Contrast ratio");
            //_contrastRatio = GUILayout.HorizontalSlider(_contrastRatio, 0.1f, 10f);
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static float _blendingRatio = 1.0f;
        static float _heightRatio = 1.0f;
        static float _contrastRatio = 1.0f;

        private static void UpdateTerrainMaterialShaderValues()
        {
            if (StrandedWorld.Instance != null
                && StrandedWorld.Instance.Zones != null)
            {
                if (StrandedWorld.Instance.Zones.Length == 0)
                {
                    Debug.Log("Stranded Deep Better Ground Textures Mod : Updating terrain layer textures : zones empty");
                }
                else
                {
                    //Debug.Log("Stranded Deep Better Ground Textures Mod : Updating terrain layer textures : looping zones");
                    foreach (Zone z in StrandedWorld.Instance.Zones)
                    {
                        if (!z.Loaded)
                            continue;

                        try
                        {
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = _NoiseTex
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = Header(Noise Texture)
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = _Tex1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = Header(Diffuse Maps)
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = _Pack1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = Header(Packed Maps)
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = _Blend1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = Header(Blending)
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = _Height1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = Header(Parallax)
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = _Contrast1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = Header(Specular)

                            //for (int i = 0; i < z.Terrain.materialTemplate.shader.GetPropertyCount(); i++)
                            //{
                            //    Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = " + z.Terrain.materialTemplate.shader.GetPropertyName(i));
                            //    Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = " + z.Terrain.materialTemplate.shader.GetPropertyFlags(i));
                            //    foreach (string attribute in z.Terrain.materialTemplate.shader.GetPropertyAttributes(i))
                            //    {
                            //        Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY attribute = " + attribute);
                            //    }
                            //    Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY = " + z.Terrain.materialTemplate.shader.GetPropertyDescription(i));
                            //    Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY type = " + z.Terrain.materialTemplate.shader.GetPropertyType(i));

                            //}

                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height1 = " + z.Terrain.materialTemplate.GetFloat("_Height1"));
                            z.Terrain.materialTemplate.SetFloat("_Height1", _heightRatio);// 0.08f * _heightRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height2 = " + z.Terrain.materialTemplate.GetFloat("_Height2"));
                            z.Terrain.materialTemplate.SetFloat("_Height2", _heightRatio);// 0.05f * _heightRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height3 = " + z.Terrain.materialTemplate.GetFloat("_Height3"));
                            z.Terrain.materialTemplate.SetFloat("_Height3", _heightRatio);// 0f + (1.0f - _heightRatio));
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height4 = " + z.Terrain.materialTemplate.GetFloat("_Height4"));
                            z.Terrain.materialTemplate.SetFloat("_Height4", _heightRatio);// 0f + (1.0f - _heightRatio));

                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend1 = " + z.Terrain.materialTemplate.GetFloat("_Blend1"));
                            z.Terrain.materialTemplate.SetFloat("_Blend1", _blendingRatio);// -11 * _blendingRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend2 = " + z.Terrain.materialTemplate.GetFloat("_Blend2"));
                            z.Terrain.materialTemplate.SetFloat("_Blend2", _blendingRatio);// -1.5f * _blendingRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend3 = " + z.Terrain.materialTemplate.GetFloat("_Blend3"));
                            z.Terrain.materialTemplate.SetFloat("_Blend3", _blendingRatio);// 0.65f * _blendingRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend4 = " + z.Terrain.materialTemplate.GetFloat("_Blend4"));
                            z.Terrain.materialTemplate.SetFloat("_Blend4", _blendingRatio);// 0.05f * _blendingRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend5 = " + z.Terrain.materialTemplate.GetFloat("_Blend5"));
                            z.Terrain.materialTemplate.SetFloat("_Blend5", _blendingRatio);// 0.77f * _blendingRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend6 = " + z.Terrain.materialTemplate.GetFloat("_Blend6"));
                            z.Terrain.materialTemplate.SetFloat("_Blend6", _blendingRatio);// 1.67f * _blendingRatio);

                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend1 = -11
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend2 = -1.5
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend3 = 0.65
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend4 = 0.05
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend5 = 0.77
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Blend6 = 1.67
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast1 = 0.1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast2 = 0.1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast3 = 0.1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast4 = 0.1
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height1 = 0.08
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height2 = 0.05
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height3 = 0
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height4 = 0
                            //Material 'Terrain_MAT' with Shader 'Beam Team/Standard/Terrain/Bumped Specular - Procedural' doesn't have a float or range property '_Height5'
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height5 = 0
                            //Material 'Terrain_MAT' with Shader 'Beam Team/Standard/Terrain/Bumped Specular - Procedural' doesn't have a float or range property '_Height6'
                            //Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Height6 = 0

                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast1 = " + z.Terrain.materialTemplate.GetFloat("_Contrast1"));
                            z.Terrain.materialTemplate.SetFloat("_Contrast1", _contrastRatio);// 0f + (1.0f - _contrastRatio));
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast2 = " + z.Terrain.materialTemplate.GetFloat("_Contrast2"));
                            z.Terrain.materialTemplate.SetFloat("_Contrast2", _contrastRatio);//7f * _contrastRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast3 = " + z.Terrain.materialTemplate.GetFloat("_Contrast3"));
                            z.Terrain.materialTemplate.SetFloat("_Contrast3", _contrastRatio);//1f * _contrastRatio);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER PROPERTY _Contrast4 = " + z.Terrain.materialTemplate.GetFloat("_Contrast4"));
                            z.Terrain.materialTemplate.SetFloat("_Contrast4", _contrastRatio);//0.7f * _contrastRatio);

                        }
                        catch { }

                        // Terrain_MAT only
                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " material template name " + z.Terrain.materialTemplate.name);
                        //if (z.Terrain.materialTemplate.name.Contains("Terrain_Sand_Underwater"))
                        //{
                        //    Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " Updating Terrain_Sand_Underwater textures ");
                        //    //ChangeRenderMode(z.Terrain.materialTemplate, BlendMode.Transparent);
                        //    z.Terrain.materialTemplate.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                        //    z.Terrain.materialTemplate.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                        //    z.Terrain.materialTemplate.SetTexture("_OcclusionMap", _indexedTextures["StrandedDeepBetterGroundTexturesMod.assets.Textures.Terrain_Sand_Underwater.png"]);
                        //    z.Terrain.materialTemplate.SetTexture("_BumpMap", _indexedTextures["StrandedDeepBetterGroundTexturesMod.assets.Textures.Terrain_Sand_Underwater_PACK.png"]);
                        //}

                        // in LOD mod
                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone terrain basemapDistance : " + z.Terrain.basemapDistance);
                        //z.Terrain.basemapDistance = 600;
                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone terrain detailObjectDistance : " + z.Terrain.detailObjectDistance);
                        //z.Terrain.detailObjectDistance = 300;
                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone terrain heightmapPixelError : " + z.Terrain.heightmapPixelError);
                        //z.Terrain.heightmapPixelError = 5;
                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone terrain heightmapMaximumLOD : " + z.Terrain.heightmapMaximumLOD);
                        ////Stranded Deep Better Ground Textures Mod : Zone terrain basemapDistance: 180
                        ////Stranded Deep Better Ground Textures Mod : Zone terrain detailObjectDistance: 70
                        ////Stranded Deep Better Ground Textures Mod : Zone terrain heightmapPixelError: 45
                        ////Stranded Deep Better Ground Textures Mod : Zone terrain heightmapMaximumLOD: 0
                    }
                }
            }
        }

        private static void UpdateGroundTextures()
        {
            //Debug.Log("Stranded Deep 2K Mod : Updating terrain layer textures");
            if (StrandedWorld.Instance != null
                && StrandedWorld.Instance.Zones != null)
            {
                if (StrandedWorld.Instance.Zones.Length == 0)
                {
                    Debug.Log("Stranded Deep Better Ground Textures Mod : Updating terrain layer textures : zones empty");
                }
                if (StrandedWorld.Instance.Zones.Length == _handledZones.Count)
                {
                    return;
                }
                else
                {
                    //Debug.Log("Stranded Deep Better Ground Textures Mod : Updating terrain layer textures : looping zones");
                    foreach (Zone z in StrandedWorld.Instance.Zones)
                    {
                        if (!z.Loaded || _handledZones.Contains(z))
                            continue;

                        //try
                        //{
                        //    foreach (UnityEngine.Rendering.LocalKeyword lk in z.Terrain.materialTemplate.enabledKeywords)
                        //    {
                        //        Debug.Log("Stranded Deep Better Ground Textures Mod : MATERIAL KEYWORD = " + lk.name);
                        //    }
                        //}
                        //catch { }

                        //try
                        //{
                        //    foreach (string keyword in z.Terrain.materialTemplate.shaderKeywords)
                        //    {
                        //        Debug.Log("Stranded Deep Better Ground Textures Mod : SHADER KEYWORD = " + keyword);
                        //    }
                        //    z.Terrain.materialTemplate.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                        //}
                        //catch { }


                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " material name " + z.Terrain.materialTemplate.name);
                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Test terrain layer texture name : " + String.Join(" ", z.Terrain.materialTemplate.GetTexturePropertyNames()));
                        string[] textureIds = z.Terrain.materialTemplate.GetTexturePropertyNames();
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _NoiseTex / Noise
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Tex1 / Terrain_Sand_Underwater
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Tex2 / Terrain_Sand_Wet_DIFF
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Tex3 / Terrain_Sand_Clean_DIFF
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Tex4 / Terrain_Sand_Dirty_DIFF
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Tex5 / Terrain_Grass_Dirt_DIFF
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Tex6 / Terrain_Dirt_Sticks_DIFF

                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Pack1 / Terrain_Sand_Clean_PACK
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Pack2 / Terrain_Sand_Wet_NRM
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Pack3 / Terrain_Sand_Clean_PACK
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Pack4 / Terrain_Dirt_Sticks_PACK
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Pack5 / Terrain_Grass_Dirt_PACK
                        //Stranded Deep Better Ground Textures Mod : Zone INTERNAL_PROCEDURAL_30 terrain texture name _Pack6 / Terrain_Dirt_Sticks_PACK
                        foreach (string textureName in textureIds)
                        {
                            Texture tex = z.Terrain.materialTemplate.GetTexture(textureName);

                            //try
                            //{
                            //    Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " terrain texture name " + textureName + " / " + tex.name);
                            //    foreach (var prop in tex.GetType().GetProperties())
                            //    {
                            //        Console.WriteLine("Stranded Deep Better Ground Textures Mod : TEXTURE PROPERTY = {0}={1}", prop.Name, prop.GetValue(tex, null));
                            //    }
                            //}
                            //catch { }


                            //Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " terrain texture name " + textureName + " / " + tex.name);
                            string key = tex.name.ToLower();
                            if (_indexedTextures.ContainsKey(key))
                            {
                                if (tex is Texture2D)
                                {
                                    Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " Updating terrain layer textures " + tex.name);
                                    z.Terrain.materialTemplate.SetTexture(textureName, _indexedTextures[key]);
                                }
                                else
                                {
                                    Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " : TEXTURE IS NOT TEXTURE2D " + tex.name);
                                }
                            }
                            else
                            {
                                Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " : texture " + key + " not found");
                            }
                        }

                        //z.Terrain.materialTemplate.EnableKeyword("_DETAIL_MULX2");
                        //z.Terrain.materialTemplate.SetTexture("_DetailNormalMap", _indexedTextures["StrandedDeepBetterGroundTexturesMod.assets.Textures.Detail_Normal_Noise.png"]);

                        try
                        {
                            z.Terrain.materialTemplate.SetTexture("_NoiseTex", _indexedTextures["detail_normal_noise"]);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " Updated terrain layer textures _NoiseTex detail_normal_noise");

                            // bug patch
                            z.Terrain.materialTemplate.SetTexture("_Pack1", _indexedTextures["terrain_sand_underwater_pack"]);
                            Debug.Log("Stranded Deep Better Ground Textures Mod : Zone " + z.Id + " Updated terrain layer textures _Pack1 terrain_sand_underwater_pack");
                        }
                        catch { }

                        _handledZones.Add(z);
                    }
                }
            }
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                if (Beam.Game.State == GameState.NEW_GAME
                    || Beam.Game.State == GameState.LOAD_GAME)
                {
                    UpdateGroundTextures();

                    //UpdateTerrainMaterialShaderValues();
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Better Ground Textures Mod : error on update : " + e);
            }
        }

        private static void Reset()
        {
            _handledZones.Clear();
        }

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach (string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            //if (tokens[0].Contains("revealWorld"))
                            //{
                            //    revealWorld = bool.Parse(tokens[1]);
                            //}
                            //else if (tokens[0].Contains("revealMissions"))
                            //{
                            //    revealMissions = bool.Parse(tokens[1]);
                            //}
                            //else if (tokens[0].Contains("debugMode"))
                            //{
                            //    debugMode = bool.Parse(tokens[1]);
                            //}
                            //if (tokens[0].Contains("viewDistance"))
                            //{
                            //    viewDistance = float.Parse(tokens[1]);
                            //}
                        }
                    }
                }
            }
        }

        private static void WriteConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine("viewDistance=" + viewDistance + ";");
                //sb.AppendLine("revealWorld=" + revealWorld + ";");
                //sb.AppendLine("revealMissions=" + revealMissions + ";");
                //sb.AppendLine("debugMode=" + debugMode + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        private static bool PreLoadTextures()
        {
            try
            {
                string assetBundleFile = Path.Combine(Directory.GetCurrentDirectory(), @"Mods\StrandedDeepBetterGroundTexturesMod\assets\strandeddeepbettergroundtextures");
                AssetBundle myAssets = AssetBundle.LoadFromFile(assetBundleFile);
                if (myAssets != null)
                {
                    Debug.Log("Stranded Deep Better Ground Textures Mod : successfully loaded AssetBundle");
                }
                else
                {
                    Debug.Log("Stranded Deep Better Ground Textures Mod : NOT loaded AssetBundle");
                }

                foreach (string assetName in myAssets.GetAllAssetNames())
                {
                    if (assetName.EndsWith(".png") || assetName.EndsWith(".psd"))
                    {
                        Debug.Log("Stranded Deep Better Ground Textures Mod : assetName = " + assetName);
                        Texture2D texture = myAssets.LoadAsset<Texture2D>(assetName);
                        if (texture != null)
                        {
                            Debug.Log("Stranded Deep Better Ground Textures Mod : successfully loaded texture asset " + assetName);
                        }
                        else
                        {
                            Debug.Log("Stranded Deep Better Ground Textures Mod : NOT loaded texture asset " + assetName);
                        }
                        string key = assetName.Replace("assets/textures/", "").Replace("4k", "").Replace("2k", "").Replace("1k", "").Replace(".psd", "").Replace(".png", "");
                        Debug.Log("Stranded Deep Better Ground Textures Mod : preloaded " + key);
                        _indexedTextures.Add(key, texture);
                    }
                }

                myAssets.Unload(false);

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Better Ground Textures Mod : texture preload failed : " + e);
                return false;
            }
        }

        //public static byte[] ExtractResource(String filename)
        //{
        //    System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
        //    using (System.IO.Stream resFilestream = a.GetManifestResourceStream(filename))
        //    {
        //        if (resFilestream == null) return null;
        //        byte[] ba = new byte[resFilestream.Length];
        //        resFilestream.Read(ba, 0, ba.Length);
        //        return ba;
        //    }
        //}

        private static void TerrainTestShit()
        {
            try
            {
                if (false
                            && (Beam.Game.State == GameState.NEW_GAME
                            || Beam.Game.State == GameState.LOAD_GAME))
                {
                    Debug.Log("Stranded Deep Better Ground Textures Mod : Test terrain layer Zones");
                    if (StrandedWorld.Instance != null
                        && StrandedWorld.Instance.Zones != null)
                    {
                        if (StrandedWorld.Instance.Zones.Length == 0)
                        {
                            Debug.Log("Stranded Deep Better Ground Textures Mod :Test terrain layer : zones empty");
                        }
                        else
                        {
                            foreach (Zone z in StrandedWorld.Instance.Zones)
                            {
                                //TerrainData td = z.Terrain.terrainData;
                                //if (td != null)
                                //{
                                //    //td.

                                //    foreach (TerrainLayer layer in td.terrainLayers)
                                //    {
                                //        Debug.Log("Stranded Deep 2K Mod :Test terrain layer texture name : " + layer.diffuseTexture.name);
                                //        Debug.Log("Stranded Deep 2K Mod :Test terrain layer mask name : " + layer.maskMapTexture.name);
                                //    }
                                //}
                                //else
                                //{
                                //    Debug.Log("Stranded Deep 2K Mod :Test terrain layer null");
                                //}
                                Debug.Log("Stranded Deep Better Ground Textures Mod :Test terrain layer texture name : " + String.Join(" ", z.Terrain.materialTemplate.GetTexturePropertyNames()));
                                string[] textureIds = z.Terrain.materialTemplate.GetTexturePropertyNames();
                                foreach (string textureName in textureIds)
                                {
                                    Texture tex = z.Terrain.materialTemplate.GetTexture(textureName);
                                    if (tex is Texture2D)
                                    {
#warning they are herrrrrrrrrreeeeeee
                                        //string targetFile = "e:\\" + tex.name + ".png";
                                        //if (!File.Exists(targetFile))
                                        //{
                                        //    Texture2D t = duplicateTexture(tex as Texture2D);
                                        //    byte[] bytes = t.EncodeToPNG();
                                        //    File.WriteAllBytes(targetFile, bytes);
                                        //}
                                    }
                                    else
                                    {
                                        Debug.Log("Stranded Deep Better Ground Textures Mod :Test terrain layer : TEXTURE IS NOT TEXTURE2D");
                                    }
                                }
                            }
                        }
                    }

                    //Debug.Log("Stranded Deep 2K Mod : Test terrain layer StrandedWorld");
                    //TerrainData[] tds = typeof(StrandedWorld).GetField("_terrainDatas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(StrandedWorld.Instance) as TerrainData[];
                    //if (tds != null)
                    //{
                    //    foreach (TerrainData td in tds)
                    //    {
                    //        foreach (TerrainLayer layer in td.terrainLayers)
                    //        {
                    //            Debug.Log("Stranded Deep 2K Mod : Test terrain layer texture name : " + layer.diffuseTexture.name);
                    //            Debug.Log("Stranded Deep 2K Mod : Test terrain layer mask name : " + layer.maskMapTexture.name);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Debug.Log("Stranded Deep 2K Mod : Test terrain layer null");
                    //}
                    ////StrandedWorld.Instance
                    ////_terrainDatas
                    ////TerrainData td;
                    ////td.terrainLayers
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Better Ground Textures Mod : Test terrain layer error : " + e);
            }

            //if (true)
            //{
            //    Debug.Log("Stranded Deep 2K Mod : terrain test");

            //    FieldInfo fi_terrainDatas = typeof(StrandedWorld).GetField("_terrainDatas", BindingFlags.NonPublic | BindingFlags.Instance);

            //    TerrainData[] tds = fi_terrainDatas.GetValue(StrandedWorld.Instance) as TerrainData[];
            //    foreach (TerrainData td in tds)
            //    {
            //        foreach (TerrainLayer tl in td.terrainLayers)
            //        {
            //            Debug.Log("Stranded Deep 2K Mod : terrain layer found " + tl.name);
            //            Debug.Log("Stranded Deep 2K Mod : terrain layer found " + (tl.diffuseTexture != null ? "Texture found" : "Null texture"));
            //        }
            //    }
            //}
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    standardShaderMaterial.SetFloat("_Mode", 1);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    standardShaderMaterial.SetFloat("_Mode", 2);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    standardShaderMaterial.SetFloat("_Mode", 3);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    standardShaderMaterial.SetFloat("_Mode", 3);
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
            }
        }
    }
}
