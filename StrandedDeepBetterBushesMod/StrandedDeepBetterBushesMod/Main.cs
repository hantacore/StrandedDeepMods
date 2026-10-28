using Beam;
using Beam.Rendering;
using Beam.Serialization;
using Funlabs;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepBetterBushesMod
{
    static partial class Main
    {
        private static string configFileName = "StrandedDeepBetterBushesMod.config";

        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();

        internal static bool replaceBushTextures = false;

        private static Harmony harmony;

        //public static string _infojsonlocation = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedWide/StrandedWide/StrandedWideMod_Harmony/Info.json";


        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;
                modEntry.OnUnload = OnUnload;

                //VersionChecker.CheckVersion(modEntry, _infojsonlocation);

                ReadConfig();

                //Texture2D leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                //leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                //string key = "StrandedDeepBetterBushesMod.assets.Textures.Bush_Leafs_MAT_MainTex.png";
                //leavesTex.LoadImage(ExtractResource(key));
                //Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                //_indexedTextures.Add(key, leavesTex);

                //leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                //key = "StrandedDeepBetterBushesMod.assets.Textures.Bush_Leafs_MAT_OcclusionMap.png";
                //leavesTex.LoadImage(ExtractResource(key));
                //Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                //_indexedTextures.Add(key, leavesTex);

                //leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                //key = "StrandedDeepBetterBushesMod.assets.Textures.Bush_Tile_MAT _BumpMap.png";
                //leavesTex.LoadImage(ExtractResource(key));
                //Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                //_indexedTextures.Add(key, leavesTex);

                //leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                //key = "StrandedDeepBetterBushesMod.assets.Textures.Bush_Tile_MAT_MainTex.png";
                //leavesTex.LoadImage(ExtractResource(key));
                //Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                //_indexedTextures.Add(key, leavesTex);

                //leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                //key = "StrandedDeepBetterBushesMod.assets.Textures.Bush_Tile_MAT_DetailAlbedoMap.png";
                //leavesTex.LoadImage(ExtractResource(key));
                //Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                //_indexedTextures.Add(key, leavesTex);

                // dans Load :
                LoadTexture("Bush_Leafs_MAT_MainTex");
                LoadTexture("Bush_Tile_MAT_MainTex");

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("Stranded Deep Better Bushes Mod properly loaded");

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod loading failed : " + ex);
            }

            return false;
        }

        private static void LoadTexture(string name)
        {
            string key = "StrandedDeepBetterBushesMod.assets.Textures." + name + ".png";
            byte[] data = ExtractResource(key);
            if (data == null) { Debug.Log("Better Bushes : ressource absente, ignorée : " + key); return; }
            Texture2D tex = new Texture2D(2048, 2048, TextureFormat.ARGB32, true, false);
            tex.LoadImage(data);

            tex.filterMode = FilterMode.Trilinear;
            tex.mipMapBias = -1f;   // essaie entre -0.5 et -1.5
            tex.anisoLevel = 4;

            _indexedTextures[key] = tex;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Better bushes mod by Hantacore");
            GUILayout.Label("Textures options");
            replaceBushTextures = GUILayout.Toggle(replaceBushTextures, "Replace bushes textures");
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        private static void Reset()
        {
            BushTextureUpdater.Reset();
        }

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
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
                            if (tokens[0].Contains("replaceBushTextures"))
                            {
                                replaceBushTextures = bool.Parse(tokens[1]);
                            }
                        }
                    }
                }
            }
        }

        private static void WriteConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("replaceBushTextures=" + replaceBushTextures + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        public static byte[] ExtractResource(String filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }
    }
}
