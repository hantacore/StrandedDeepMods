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

namespace StrandedDeepAnimatedFoliageMod
{
    static partial class Main
    {
        private static string configFileName = "StrandedDeepAnimatedFoliageMod.config";

        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        internal static bool perfCheck = false;

        internal static bool animateTrees = false;
        internal static bool animateBushes = false;
        internal static bool animateSmallTrees = false;
        internal static bool animatePlants = false;
        internal static bool animateCorals = false;
        internal static bool replaceTreeLeaves = false;
        internal static bool replaceBushTextures = false;
        internal static float distanceRatio = 1.0f;

        internal static bool debugMode = false;

        internal static Vector3 Wind
        {
            get; set;
        }

        private static Harmony harmony;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;
                modEntry.OnUnload = OnUnload;

                ReadConfig();

                Wind = new Vector3(1.0f, 0, 1.0f);

                Texture2D leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                string key = "StrandedDeepAnimatedFoliageMod.assets.Textures.BRANCH_MainTex.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);

                leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                key = "StrandedDeepAnimatedFoliageMod.assets.Textures.BRANCH_BumpMap.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);

                leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                key = "StrandedDeepAnimatedFoliageMod.assets.Textures.Bush_Leafs_MAT_MainTex.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);

                leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                key = "StrandedDeepAnimatedFoliageMod.assets.Textures.Bush_Leafs_MAT_OcclusionMap.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);

                leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                key = "StrandedDeepAnimatedFoliageMod.assets.Textures.Bush_Tile_MAT _BumpMap.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);

                leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                key = "StrandedDeepAnimatedFoliageMod.assets.Textures.Bush_Tile_MAT_MainTex.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);

                leavesTex = new Texture2D(2048, 2048, UnityEngine.TextureFormat.ARGB32, false, false);
                key = "StrandedDeepAnimatedFoliageMod.assets.Textures.Bush_Tile_MAT_DetailAlbedoMap.png";
                leavesTex.LoadImage(ExtractResource(key));
                Debug.Log("Stranded Deep AnimatedFoliage Mod : texture " + key + " pre-loaded");
                _indexedTextures.Add(key, leavesTex);


                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("Stranded Deep AnimatedFoliage Mod properly loaded");

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod loading failed : " + ex);
            }
            finally
            {
                if (chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage Mod load time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }

            return false;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Animated foliage mod by Hantacore");
            GUILayout.Label("Animations options");
            animateTrees = GUILayout.Toggle(animateTrees, "Animate big trees (performance +)");
            if (animateTrees)
            {
                animateSmallTrees = GUILayout.Toggle(animateSmallTrees, "Animate small trees (performance ++)");
                animatePlants = GUILayout.Toggle(animatePlants, "Animate small plants (performance +++)");
            }
            animateBushes = GUILayout.Toggle(animateBushes, "Animate vegetation (performance ++)");
            animateCorals = GUILayout.Toggle(animateCorals, "Animate underwater corals and kelp (performance ++)");
            GUILayout.Label("Animation distance");
            distanceRatio = GUILayout.HorizontalSlider(distanceRatio, 0.1f, 1.0f);

            GUILayout.Label("Textures options");
            replaceTreeLeaves = GUILayout.Toggle(replaceTreeLeaves, "Replace tree leaves");
            replaceBushTextures = GUILayout.Toggle(replaceBushTextures, "Replace bushes textures");

            GUILayout.Label("Other options");
            debugMode = GUILayout.Toggle(debugMode, "Debug mode");
            if (debugMode && WorldUtilities.IsWorldLoaded())
            {
                if (GUILayout.Button("Test frond spawn"))
                {
                    SpawnFrond();
                }

                if (GUILayout.Button("Start Storm") && AtmosphereStorm.Instance != null)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage Mod : Try start storm");
                    MethodInfo mi_CreateWeatherEvent = typeof(AtmosphereStorm).GetMethod("CreateWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    //CreateWeatherEvent(float eventRatio, float startTimeRatio)
                    mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { 100f, 0f });
                    AtmosphereStorm.Instance.StartWeatherEvent();
                }
                if (GUILayout.Button("Start Rain") && AtmosphereStorm.Instance != null)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage Mod : Try start rain");
                    MethodInfo mi_CreateWeatherEvent = typeof(AtmosphereStorm).GetMethod("CreateWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    //CreateWeatherEvent(float eventRatio, float startTimeRatio)
                    mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { 50f, 0f });
                    AtmosphereStorm.Instance.StartWeatherEvent();
                }
                if (GUILayout.Button("Start Light Rain") && AtmosphereStorm.Instance != null)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage Mod : Try start light rain");
                    MethodInfo mi_CreateWeatherEvent = typeof(AtmosphereStorm).GetMethod("CreateWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    //CreateWeatherEvent(float eventRatio, float startTimeRatio)
                    mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { 25f, 0f });
                    AtmosphereStorm.Instance.StartWeatherEvent();
                }
                if (GUILayout.Button("End current event") && AtmosphereStorm.Instance != null)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage Mod : Try end current event");
                    foreach (WeatherEvent we in AtmosphereStorm.Instance.WeatherEvents)
                    {
                        we.ResetEvent();
                        we.StopEvent();
                    }
                }
            }
        }

        private static void SpawnFrond()
        {
            Zone z = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
            Vector3 velocity = Wind * 20; // add little randomness
            Create(12, z.transform.position + new Vector3(0, 10, 0), velocity, null);
            //gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 50f;
        }

        private static void SpawnStick()
        {
            Zone z = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
            Vector3 velocity = Wind * 20; // add little randomness
            Create(12, z.transform.position + new Vector3(0, 10, 0), velocity, null);
            //gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 50f;
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        //static int flag = 0;
        //private static StrandedWorld previousWorldInstance = null;
        //private static bool worldLoaded = false;

        //private static TreeMeshFiltersHandler handler = new TreeMeshFiltersHandler(5, true);

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
#warning TODO : coconuts (attach parent)
            try
            {
                chrono.Reset();

                if (Game.State == GameState.NEW_GAME
                    || Game.State == GameState.LOAD_GAME)
                {
                    // anti memory leak
                    //if (previousWorldInstance != null
                    //    && !System.Object.ReferenceEquals(previousWorldInstance, StrandedWorld.Instance))
                    //{
                    //    Debug.Log("Stranded Deep AnimatedFoliage Mod : world instance changed, clearing events");
                    //    previousWorldInstance.WorldGenerated -= Instance_WorldGenerated;
                    //    previousWorldInstance = null;
                    //    worldLoaded = false;
                    //}
                    //// to reattach at the right moment
                    //if (StrandedWorld.Instance != null
                    //    && !System.Object.ReferenceEquals(StrandedWorld.Instance, previousWorldInstance))
                    //{
                    //    Debug.Log("Stranded Deep AnimatedFoliage Mod : world instance found, registering events");
                    //    previousWorldInstance = StrandedWorld.Instance;
                    //    StrandedWorld.Instance.WorldGenerated -= Instance_WorldGenerated;
                    //    StrandedWorld.Instance.WorldGenerated += Instance_WorldGenerated;
                    //}

                    if (WorldUtilities.IsWorldLoaded())
                    {
                        ComputeStormPercentage();
                    }

                    chrono.Start();
                }
                else
                {
                    Reset();
                }

                //flag++;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod : error on update : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        //private static void AnimateTreesV2()
        //{
        //    if (animateTrees || animateCorals || animateBushes || animatePlants)
        //    {
        //        //AnimateTreesV1();

        //        if (handler.MustFillQueue)
        //        {
        //            handler.FillQueue(Game.FindObjectsOfType<MeshFilter>());
        //        }
        //        handler.Handle();
        //    }
        //}

        //private static void AnimateTreesV1()
        //{
        //    //InteractiveObject_PALM[] palms = Game.FindObjectsOfType<InteractiveObject_PALM>();
        //    //foreach (InteractiveObject_PALM palm in palms)
        //    //{
        //    //    if (palm.gameObject.GetComponent<TreeBender>() == null)
        //    //    {
        //    //        Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Tree Bender component");
        //    //        TreeBender tb = palm.gameObject.AddComponent<TreeBender>();
        //    //    }
        //    //}
        //    SaveablePrefab[] prefabs = Game.FindObjectsOfType<SaveablePrefab>();
        //    foreach (SaveablePrefab prefab in prefabs)
        //    {
        //        if ((prefab.PrefabId == 157 // palm
        //            || prefab.PrefabId == 158 // palm
        //            || prefab.PrefabId == 159 // palm
        //            || prefab.PrefabId == 160 // palm
        //            || prefab.PrefabId == 66 // ficus tree
        //            || prefab.PrefabId == 67 // ficus tree
        //            || prefab.PrefabId == 202 // pine
        //            || prefab.PrefabId == 203 // pine
        //            || prefab.PrefabId == 204 // pine
        //            || animateSmallTrees && (prefab.PrefabId == 47 // ficus
        //                || prefab.PrefabId == 48 // ficus
        //                || prefab.PrefabId == 49 // ficus
        //                || prefab.PrefabId == 206 // pine small
        //                || prefab.PrefabId == 207 // pine small
        //                )
        //            || animatePlants && (prefab.PrefabId == 50 // alocasia
        //                || prefab.PrefabId == 51 // alocasia
        //                || prefab.PrefabId == 58 // alocasia
        //                || prefab.PrefabId == 59 // ceriman
        //                || prefab.PrefabId == 60// ceriman
        //                )
        //            )
        //            && prefab.gameObject.GetComponent<TreeBender>() == null)
        //        {
        //            if (debugMode)
        //                Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Tree Bender component to " + prefab.gameObject.name);
        //            TreeBender tb = prefab.gameObject.AddComponent<TreeBender>();
        //            if (prefab.PrefabId == 202
        //                || prefab.PrefabId == 203
        //                || prefab.PrefabId == 204)
        //            {
        //                tb.BendAngle = 2;
        //            }
        //            if (prefab.PrefabId == 157
        //                || prefab.PrefabId == 158
        //                || prefab.PrefabId == 159
        //                || prefab.PrefabId == 160)
        //            {
        //                tb.IsPalm = true;
        //            }
        //            //if (prefab.PrefabId == 66
        //            //    || prefab.PrefabId == 67
        //            //    || prefab.PrefabId == 47
        //            //    || prefab.PrefabId == 48
        //            //    || prefab.PrefabId == 49)
        //            //{
        //            //    tb.ReplaceLeaves(prefab.PrefabId, prefab.gameObject.GetComponent<Renderer>());
        //            //}
        //        }
        //        if (animateBushes
        //            && prefab.PrefabId == 205
        //            && prefab.gameObject.GetComponent<BushBender>() == null)
        //        {
        //            if (debugMode)
        //                Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Bush Bender component");
        //            BushBender tb = prefab.gameObject.AddComponent<BushBender>();
        //        }
        //if (animateCorals)
        //{
        //    MeshFilter[] mfilters = Game.FindObjectsOfType<MeshFilter>();
        //    foreach (MeshFilter mfilter in mfilters)
        //    {
        //        //Debug.Log("Stranded Deep AnimatedFoliage mod Main meshfilter go name : " + mfilter.gameObject.name);
        //        if ((mfilter.gameObject.name.Contains("Kelp")
        //                || mfilter.gameObject.name.Contains("Coral_Group")
        //            //|| mfilter.gameObject.name.Contains("Seaweed")
        //            ) && mfilter.gameObject.GetComponent<CoralBender>() == null)
        //        {
        //            Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Coral Bender component to " + mfilter.gameObject.name);
        //            CoralBender cb = mfilter.gameObject.AddComponent<CoralBender>();
        //        }
        //    }
        //}
        //    }
        //}

        //private static void Instance_WorldGenerated()
        //{
        //    Debug.Log("Stranded Deep AnimatedFoliage Mod : World Loaded event");
        //    worldLoaded = true;
        //}

        static float t = 0.0f;
        static float previousStormPercentage = 0;
        internal static float stormPercentage = 0;

        private static void ComputeStormPercentage()
        {
            if (AtmosphereStorm.Instance.CurrentWeatherEvent != null)
            {
                if (previousStormPercentage != AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage)
                {
                    t += 0.025f * Time.deltaTime;
                    stormPercentage = Mathf.Lerp(previousStormPercentage, AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage, t);
                    if (Mathf.Approximately(stormPercentage, AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage))
                    {
                        previousStormPercentage = AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage;
                        t = 0.0f;
                    }
                    //Debug.Log("Stranded Deep AnimatedFoliage Mod storm percentage : " + stormPercentage);
                }
            }
            else if (previousStormPercentage != 0)
            {
                t += 0.025f * Time.deltaTime;
                stormPercentage = Mathf.Lerp(previousStormPercentage, 0, t);
                if (Mathf.Approximately(stormPercentage, 0))
                {
                    previousStormPercentage = 0;
                    t = 0.0f;
                }
                //Debug.Log("Stranded Deep AnimatedFoliage Mod storm percentage : " + stormPercentage);
            }
        }

        private static void Reset()
        {
            //handler.Reset();

            FicusTextureUpdater.Reset();
            BushTextureUpdater.Reset();

            //DeformingPalmTopReset();
            //flag = 0;
            //worldLoaded = false;
        }

        public static void Create(uint prefabId, Vector3 position, Vector3 velocity, Transform parent = null)
        {
            //Vector3 forward = Camera.main.transform.forward;
            //forward.y = 0f;
            //forward.Normalize();
            string text;
            bool flag = Prefabs.TryGetMultiplayerPrefabName(prefabId, out text);
            Vector3 vector = position;
            SaveablePrefab sp = null;
            if (!flag)
            {
                sp = MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, vector, Quaternion.identity, MiniGuid.New(), null);
                if (parent != null)
                {
                    sp.gameObject.transform.parent = parent;
                }
                if (velocity.magnitude > 0)
                {
                    sp.gameObject.GetComponent<Rigidbody>().velocity = velocity;
                }
                new ReplicateCreate
                {
                    PrefabId = (short)prefabId,
                    Pos = vector
                }.Post();
                return;
            }
            if (Game.Mode.IsClient())
            {
                new ReplicateCreate
                {
                    PrefabId = (short)prefabId,
                    Pos = vector
                }.Post();
                return;
            }
            sp = MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, vector, Quaternion.identity, MiniGuid.New(), null);
            if (parent != null)
            {
                sp.gameObject.transform.parent = parent;
            }
            if (velocity.magnitude > 0)
            {
                sp.gameObject.GetComponent<Rigidbody>().velocity = velocity;
            }
        }

        internal class ReplicateCreate : MultiplayerMessage
        {
            public override void OnPeer()
            {
                MultiplayerMng.Instantiate<SaveablePrefab>((uint)this.PrefabId, this.Pos, Quaternion.identity, MiniGuid.New(), null);
            }

            public ReplicateCreate()
            {
            }

            [Replicate]
            public short PrefabId;

            [Replicate]
            public Vector3 Pos;
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
                            if (tokens[0].Contains("animateTrees"))
                            {
                                animateTrees = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("animateBushes"))
                            {
                                animateBushes = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("animateSmallTrees"))
                            {
                                animateSmallTrees = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("animatePlants"))
                            {
                                animatePlants = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("animateCorals"))
                            {
                                animateCorals = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("replaceTreeLeaves"))
                            {
                                replaceTreeLeaves = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("replaceBushTextures"))
                            {
                                replaceBushTextures = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("distanceRatio"))
                            {
                                distanceRatio = float.Parse(tokens[1]);
                                if (distanceRatio < 0.1f)
                                    distanceRatio = 0.1f;
                                if (distanceRatio > 1.0f)
                                    distanceRatio = 1.0f;
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
                sb.AppendLine("animateTrees=" + animateTrees + ";");
                sb.AppendLine("animateBushes=" + animateBushes + ";");
                sb.AppendLine("animateSmallTrees=" + animateSmallTrees + ";");
                sb.AppendLine("animatePlants=" + animatePlants + ";");
                sb.AppendLine("animateCorals=" + animateCorals + ";");
                sb.AppendLine("replaceTreeLeaves=" + replaceTreeLeaves + ";");
                sb.AppendLine("replaceBushTextures=" + replaceBushTextures + ";");
                sb.AppendLine("distanceRatio=" + distanceRatio + ";");

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
