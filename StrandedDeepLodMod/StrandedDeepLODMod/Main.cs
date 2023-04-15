using Beam;
using Beam.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using Beam.Utilities;
using SharpNeatLib.Maths;
using HarmonyLib;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        private static string configFileName = "StrandedDeepLODMod.config";

        internal static bool moreFishes = false;
        internal static bool increaseFishDrawingDistance = false;
        internal static bool increasLODs = false;
        internal static bool ultraDistance = false;
        internal static bool ultraMFBBQDistance = false;
        internal static bool permanentGroundCover = false;
        internal static bool increaseTerrainLOD = false;
        internal static bool increaseShadowsQuality = false;

        internal static bool addSmallFishes = false;
        internal static bool addShrimps = false;
        internal static bool addJellyFishes = false;

        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        internal static bool perfCheck = true;

        private static Harmony harmony;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;

                ReadConfig();

                InitMultiplyFishes();
                //InitIncreaseFishRendererDistance();

                Texture2D tex = new Texture2D(1280, 1280, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.fish.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.fish.png", tex);

                tex = new Texture2D(1280, 1280, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.jellyfish.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.jellyfish.png", tex);

                tex = new Texture2D(1280, 1280, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.shrimp2.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.shrimp2.png", tex);

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("Stranded Deep LOD Mod properly loaded");

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("Stranded Deep LOD Mod loading failed : " + ex);
            }
            finally
            {
                if (chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep LOD Mod load time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }

            return false;
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

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Stranded Deep LOD mod by Hantacore");
            GUILayout.Label("LOD options");
            increasLODs = GUILayout.Toggle(increasLODs, "Increase drawing distances (x5)");
            ultraDistance = GUILayout.Toggle(ultraDistance, "Ultra drawing distance (x10, active only if \"Increase drawing distances\" is active)");
            ultraMFBBQDistance = GUILayout.Toggle(ultraMFBBQDistance, "Ultra MF BBQ drawing distance (x1000, active only if \"Increase drawing distances\" is active)");
            increaseTerrainLOD = GUILayout.Toggle(increaseTerrainLOD, "Improve terrain smoothness");
            increaseShadowsQuality = GUILayout.Toggle(increaseShadowsQuality, "Improve shadows quality (experimental)");
            GUILayout.Label("Fishes options");
            moreFishes = GUILayout.Toggle(moreFishes, "More fishes in flocks");
            increaseFishDrawingDistance = GUILayout.Toggle(increaseFishDrawingDistance, "Increase fishes drawing distance");
            addSmallFishes = GUILayout.Toggle(addSmallFishes, "Add small fishes particle animation");
            addShrimps = GUILayout.Toggle(addShrimps, "Add shrimps particle animation");
            addJellyFishes = GUILayout.Toggle(addJellyFishes, "Add jellyfishes particle animation");
            GUILayout.Label("Grass options");
            permanentGroundCover = GUILayout.Toggle(permanentGroundCover, "Show permanent ground cover");
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static int flag = 0;

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();

                if (Beam.Game.State != GameState.NEW_GAME && Beam.Game.State != Beam.GameState.LOAD_GAME)
                {
                    Reset();
                    return;
                }

                chrono.Start();

                if (!WorldUtilities.IsWorldLoaded())
                    return;

                if (fr == null)
                {
                    fr = new FastRandom(StrandedWorld.WORLD_SEED);
                }

                //Debug.Log("Stranded Deep LOD Mod update step 1 (ms) = " + chrono.ElapsedMilliseconds);

                if (moreFishes && flag == 0)
                {
                    //FillFishSpawnersQueue();
                    //MultiplyFishes();

                    if (!multiplyFishesDone)
                    {
                        MultiplyFishesNew();
                    }
                }

                //Debug.Log("Stranded Deep LOD Mod update step 2 (ms) = " + chrono.ElapsedMilliseconds);

                //if (increaseFishDrawingDistance && flag == 1)
                //{
                //    FillFishRenderersQueue();
                //    IncreaseFishDrawingDistance();
                //}

                //Debug.Log("Stranded Deep LOD Mod update step 3 (ms) = " + chrono.ElapsedMilliseconds);

                if (false && increasLODs && flag == 2)
                {
                    //IncreaseLODs();

                    if (PlayerRegistry.LocalPlayer == null)
                        return;

                    Zone zone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                    if (zone == null)
                    {
                        Debug.Log("Stranded Deep LOD Mod : IncreaseLODs : no zone found for impostor creation");
                    }
                    else
                    {
                        zone.CreateImpostors();
                    }
                }

                //Debug.Log("Stranded Deep LOD Mod update step 4 (ms) = " + chrono.ElapsedMilliseconds);

                //if ((addJellyFishes || addSmallFishes || addShrimps) && flag == 3)
                //{
                //    FillParticleSpawnersQueue();
                //    AddParticleSpawners();
                //}

                //Debug.Log("Stranded Deep LOD Mod update step 5 (ms) = " + chrono.ElapsedMilliseconds);

                if (permanentGroundCover && flag == 4)
                {
                    AddPermanentGroundCover();
                }

                //Debug.Log("Stranded Deep LOD Mod update step 6 (ms) = " + chrono.ElapsedMilliseconds);

                if (increaseTerrainLOD && flag == 5)
                {
                    IncreaseTerrainLOD();
                }

                //Debug.Log("Stranded Deep LOD Mod update step 7 (ms) = " + chrono.ElapsedMilliseconds);

                if (increaseShadowsQuality && flag == 5)
                {
                    IncreaseShadowsQuality();
                }

                //Debug.Log("Stranded Deep LOD Mod update step 8 (ms) = " + chrono.ElapsedMilliseconds);

                //foreach (IGameCamera gameCamera in Cameras.AllCameras)
                //{
                //    //public static readonly int PLAYER;
                //    //public static readonly int INTERACTIVE_TREES;
                //    //public static readonly int CONNECTOR_FOUNDATION;
                //    //public static readonly int CONNECTOR_PANEL;
                //    //public static readonly int CONSTRUCTIONS_RAFTS;
                //    //public static readonly int CONSTRUCTIONS_SMALL;
                //    //public static readonly int CONSTRUCTIONS;
                //    //public static readonly int PARTICLES;
                //    //public static readonly int INTERACTIVE_OBJECTS;
                //    //public static readonly int PROJECTILE_HITBOXES;
                //    //public static readonly int PROJECTILES;
                //    //public static readonly int TERRAIN_DETAILS;
                //    //public static readonly int TERRAIN_OBJECTS;
                //    //public static readonly int TERRAIN;
                //    //public static readonly int WATER;
                //    //public static readonly int IGNORE_RAYCAST;
                //    //public static readonly int UI_MAIN_MENU;

                //    Debug.Log("Stranded Deep LOD Mod : layer INTERACTIVE_TREES " + Layers.INTERACTIVE_TREES + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.INTERACTIVE_TREES]);
                //    Debug.Log("Stranded Deep LOD Mod : layer CONSTRUCTIONS_RAFTS " + Layers.CONSTRUCTIONS_RAFTS + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.CONSTRUCTIONS_RAFTS]);
                //    Debug.Log("Stranded Deep LOD Mod : layer CONSTRUCTIONS_SMALL " + Layers.CONSTRUCTIONS_SMALL + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.CONSTRUCTIONS_SMALL]);
                //    Debug.Log("Stranded Deep LOD Mod : layer CONSTRUCTIONS " + Layers.CONSTRUCTIONS + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.CONSTRUCTIONS]);
                //    Debug.Log("Stranded Deep LOD Mod : layer PARTICLES " + Layers.PARTICLES + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.PARTICLES]);
                //    Debug.Log("Stranded Deep LOD Mod : layer INTERACTIVE_OBJECTS " + Layers.INTERACTIVE_OBJECTS + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.INTERACTIVE_OBJECTS]);
                //    Debug.Log("Stranded Deep LOD Mod : layer TERRAIN_DETAILS " + Layers.TERRAIN_DETAILS + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.TERRAIN_DETAILS]);
                //    Debug.Log("Stranded Deep LOD Mod : layer TERRAIN_OBJECTS " + Layers.TERRAIN_OBJECTS + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.TERRAIN_OBJECTS]);
                //    Debug.Log("Stranded Deep LOD Mod : layer TERRAIN " + Layers.TERRAIN + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.TERRAIN]);
                //    Debug.Log("Stranded Deep LOD Mod : layer WATER " + Layers.WATER + " cull distance for camera " + gameCamera.Id + " : " + gameCamera.Camera.layerCullDistances[Layers.WATER]);
                //}

                foreach (IGameCamera gameCamera in Cameras.AllCameras)
                {
                    float[] array = new float[32];
                    Array.Copy(gameCamera.Camera.layerCullDistances, array, 32);
                    array[Layers.INTERACTIVE_OBJECTS] = 500f;
                    array[Layers.PARTICLES] = 500f;
                    array[Layers.TERRAIN_OBJECTS] = 500f;
                    array[Layers.TERRAIN_DETAILS] = 500f;

                    //for(int layer = 0; layer < array.Length; layer++)
                    //{
                    //    Debug.Log("Stranded Deep LOD Mod : layer " + layer + " cull distance for camera " + gameCamera.Id + " : " + array[layer]);
                    //}

                    gameCamera.Camera.layerCullDistances = array;
                }

                //Debug.Log("Stranded Deep LOD Mod update step 9 (ms) = " + chrono.ElapsedMilliseconds);

                flag++;

                // round robin
                if (flag > 5)
                    flag = 0;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : error on update : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep LOD Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        #region Permanent ground cover

        private static FastRandom _random = new FastRandom();

        public static bool TryGetSample(Vector3 worldPosition, Zone zone, out Sample sample)
        {
            sample = Sample.Empty;
            if (zone == null)
            {
                return false;
            }
            Terrain terrain = zone.Terrain;
            if (terrain == null)
            {
                return false;
            }
            sample = default(Sample);
            sample.Point = worldPosition;
            sample.Point.y = terrain.SampleHeight(worldPosition) + -100f;
            Vector3 vector = worldPosition - terrain.transform.position;
            sample.Normal = terrain.terrainData.GetInterpolatedNormal(Mathf.InverseLerp(0f, terrain.terrainData.size.x, vector.x), Mathf.InverseLerp(0f, terrain.terrainData.size.z, vector.z));
            return true;
        }

        private static List<Zone> _handledZones = new List<Zone>();
        static FieldInfo fi_GridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);

        static FollowSpawn.GridObject grid_object_green_grass_01 = null;
        static FollowSpawn.GridObject grid_object_green_grass_03 = null;
        static FollowSpawn.GridObject grid_object_ground_cover_a = null;
        static FollowSpawn.GridObject grid_object_ground_cover_c = null;

        private static void AddPermanentGroundCover()
        {
            if (grid_object_green_grass_01 == null 
                || grid_object_green_grass_03 == null
                || grid_object_ground_cover_a == null
                || grid_object_ground_cover_c == null)
            {
                FollowSpawn fs = Game.FindObjectOfType<FollowSpawn>();
                if (fs == null || fs.gameObject == null)
                    return;

                fs.gameObject.SetLayerRecursively(Layers.WATER);

                foreach (FollowSpawn.BiomeGrid biomeGrid in fi_GridObjects2.GetValue(fs) as FollowSpawn.BiomeGrid[])
                {
                    //Debug.Log("Stranded Deep LOD Mod : grass / biome : " + biomeGrid._biome);
                    foreach (FollowSpawn.GridObject gridObject in biomeGrid.biomeParameters)
                    {
                        //if (gridObject.grass)
                        //{
                            //Debug.Log("Stranded Deep LOD Mod : grass / grass = " + gridObject.grass);
                            //Debug.Log("Stranded Deep LOD Mod : grass / objectsToSpawn.Length = " + gridObject.objectsToSpawn.Length);
                            foreach (GameObject go in gridObject.objectsToSpawn)
                            {
                                //Debug.Log("Stranded Deep LOD Mod : grass / go name = " + go.name);
                                if (go.name.Contains("green_grass_03"))
                                {
                                    grid_object_green_grass_03 = gridObject;
                                }
                                else if (go.name.Contains("green_grass_01"))
                                {
                                    grid_object_green_grass_01 = gridObject;
                                }
                                else if (go.name.Contains("ground_cover_a"))
                                {
                                    grid_object_ground_cover_a = gridObject;
                                }
                                else if (go.name.Contains("ground_cover_c"))
                                {
                                    grid_object_ground_cover_c = gridObject;
                                }
                                //"Grass_Dry"
                            }
                            //Debug.Log("Stranded Deep LOD Mod : grass / maxObjects = " + gridObject.maxObjects);
                            //Debug.Log("Stranded Deep LOD Mod : grass / cycleGrid = " + gridObject.cycleGrid);
                            //Debug.Log("Stranded Deep LOD Mod : grass / rarity = " + gridObject.rarity);
                            //Debug.Log("Stranded Deep LOD Mod : grass / randomYRotation = " + gridObject.randomYRotation);
                            //Debug.Log("Stranded Deep LOD Mod : grass / minslope = " + gridObject.minslope);
                            //Debug.Log("Stranded Deep LOD Mod : grass / maxslope = " + gridObject.maxslope);
                            //Debug.Log("Stranded Deep LOD Mod : grass / minHeight = " + gridObject.minHeight);
                            //Debug.Log("Stranded Deep LOD Mod : grass / maxHeight = " + gridObject.maxHeight);
                            //Debug.Log("Stranded Deep LOD Mod : grass / align = " + gridObject.align);
                            //Debug.Log("Stranded Deep LOD Mod : "+ gridObject.objectsToSpawn[0].name + " / gridCheckDensity = " + gridObject.gridCheckDensity);
                        //}
                    }
                }
            }

            if (PlayerRegistry.LocalPlayer == null)
                return;

            Zone zone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
            if (zone == null 
                || _handledZones.Contains(zone)
                || grid_object_green_grass_03 == null
                || grid_object_green_grass_01 == null 
                || grid_object_ground_cover_a == null
                || grid_object_ground_cover_c == null)
                return;

            _random.Reinitialise((int)(zone.transform.position.x + zone.transform.position.z));

            AddPermanentObject(zone, grid_object_green_grass_03);
            AddPermanentObject(zone, grid_object_green_grass_01);
            AddPermanentObject(zone, grid_object_ground_cover_a);
            AddPermanentObject(zone, grid_object_ground_cover_c);

            _handledZones.Add(zone);
        }

        private static bool _handledShadows = false;

        private static void IncreaseShadowsQuality()
        {
            if (_handledShadows)
                return;

            try
            {
                Debug.Log("Stranded Deep LOD Mod : shadows distance " + QualitySettings.shadowDistance);
                QualitySettings.shadowDistance = 500.0f;
                Debug.Log("Stranded Deep LOD Mod : shadows resolution " + QualitySettings.shadowResolution);
                QualitySettings.shadowmaskMode = ShadowmaskMode.DistanceShadowmask;
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                QualitySettings.shadowProjection = ShadowProjection.CloseFit;
                //QualitySettings.shadowCascades = 4;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : IncreaseShadowsQuality error : " + e);
            }
            finally
            {
                _handledShadows = true;
            }
        }

        private static void AddPermanentObject(Zone zone, FollowSpawn.GridObject grid_object)
        {
            if (zone == null
                || grid_object == null)
                return;

            int gridDimension = 1000;
            Vector3 origin = zone.transform.position;
            //Debug.Log("Stranded Deep LOD Mod : zone position = " + origin);
            //Debug.Log("Stranded Deep LOD Mod : player position = " + PlayerRegistry.LocalPlayer.transform.position);

            for (int x = 0; x < gridDimension; x += (int)(grid_object.gridCheckDensity))// / (grid_object.grass ? 1.2f : 1.0f)))
            {
                for (int y = 0; y < gridDimension; y += (int)(grid_object.gridCheckDensity))// / (grid_object.grass ? 1.2f : 1.0f)))
                {
                    //Debug.Log("Stranded Deep LOD Mod : adding grass " + x + " / " + y);
                    Vector3 pos = origin + new Vector3((float)(x - gridDimension / 2), 50f, (float)(y - gridDimension / 2));
                    //Debug.Log("Stranded Deep LOD Mod : adding grass " + pos.x + " / " + pos.z);
                    int randRot = _random.Next(360);
                    if (_random.Next(100) <= grid_object.rarity)// * (grid_object.grass ? 4 : 1))
                    {
                        //Debug.Log("Stranded Deep LOD Mod : grass rarity check pass");

                        Sample sample;
                        if (TryGetSample(pos, zone, out sample))
                        {
                            //Debug.Log("Stranded Deep LOD Mod : grass : got sample");
                            float slope = Mathf.Acos(Vector3.Dot(sample.Normal, Vector3.up)) * 57.29578f;
                            //Debug.Log("Stranded Deep LOD Mod : grass : slope = " + slope);
                            Vector3 vector = sample.Point;
                            //Debug.Log("Stranded Deep LOD Mod : grass : height = " + vector.y);
                            GameObject gameObject = null;
                            if (slope >= grid_object.minslope && slope <= grid_object.maxslope && vector.y >= grid_object.minHeight && vector.y <= grid_object.maxHeight)
                            {
                                vector = zone.transform.worldToLocalMatrix.MultiplyPoint3x4(vector);
                                Vector3 b = zone.transform.worldToLocalMatrix.MultiplyVector(sample.Normal);
                                Vector3 normalized = Vector3.Lerp(Vector3.up, b, grid_object.align).normalized;
                                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalized);

                                gameObject = UnityEngine.Object.Instantiate<GameObject>(grid_object.objectsToSpawn[_random.Next(grid_object.objectsToSpawn.Length)], vector + origin, rotation);
                                //Debug.Log("Stranded Deep LOD Mod : spawning "+ gameObject.name + " at " + (vector + origin));
                                if (grid_object.randomYRotation)
                                {
                                    gameObject.transform.Rotate(Vector3.up * (float)randRot);
                                }
                                gameObject.transform.parent = zone.transform;
                                gameObject.SetActive(true);
                                gameObject.SetLayerRecursively(Layers.WATER);
                            }
                            //if (slope >= minslope && slope <= maxslope && vector.y >= minHeight03 && vector.y <= maxHeight03)
                            //{
                            //    vector = zone.transform.worldToLocalMatrix.MultiplyPoint3x4(vector);
                            //    Vector3 b = zone.transform.worldToLocalMatrix.MultiplyVector(sample.Normal);
                            //    Vector3 normalized = Vector3.Lerp(Vector3.up, b, align).normalized;
                            //    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalized);

                            //    //Debug.Log("Stranded Deep LOD Mod : spawning grass 03 at " + (vector + origin));
                            //    gameObject = UnityEngine.Object.Instantiate<GameObject>(gogreen_grass_03, vector + origin, rotation);
                            //    if (randomYRotation)
                            //    {
                            //        gameObject.transform.Rotate(Vector3.up * (float)randRot);
                            //    }
                            //    gameObject.transform.parent = zone.transform;
                            //    gameObject.SetActive(true);
                            //    gameObject.SetLayerRecursively(Layers.TERRAIN_OBJECTS);
                            //}

                            if (gameObject != null)
                            {
                                try
                                {
                                    //Debug.Log("Stranded Deep LOD Mod : grass : " + gameObject.name + " changing cull distance");
                                    Renderer r = gameObject.GetComponent<Renderer>();
                                    if (r != null)
                                    {
                                        r.sharedMaterial.SetFloat("_Cull", 0.0f);
                                        r.sharedMaterial.SetFloat("_DitherDistance", 400.0f);
                                        //r.sharedMaterial.SetFloat("_DitherDistance", 800.0f);
                                        r.sharedMaterial.SetFloat("_Translucent", 0f);
                                        r.sharedMaterial.SetFloat("_Translucency", 0f);

                                        if (r is MeshRenderer)
                                        {
                                            //Debug.Log("Stranded Deep LOD Mod : grass : adding shadows");
                                            MeshRenderer mr = r as MeshRenderer;
                                            mr.receiveShadows = true;
                                            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                                        }
                                    }

                                    //LODGroup lg = gameObject.GetComponent<LODGroup>();
                                    //if (lg != null)
                                    //{
                                    //    Debug.Log("Stranded Deep LOD Mod : LOD group found : " + gameObject.name);
                                    //    lg.ForceLOD(0);
                                    //}

                                    //foreach (Renderer r in rs)
                                    //{
                                    //    //Debug.Log("Stranded Deep LOD Mod : grass shader " + r.sharedMaterial.shader.name);
                                    //    //for (int property = 0; property < r.sharedMaterial.shader.GetPropertyCount(); property++)
                                    //    //{
                                    //    //    Debug.Log("Stranded Deep LOD Mod : grass shader property " + r.sharedMaterial.shader.GetPropertyName(property));
                                    //    //    //Stranded Deep LOD Mod : grass shader property _Cull
                                    //    //    //Stranded Deep LOD Mod : grass shader property _Mode
                                    //    //    //Stranded Deep LOD Mod : grass shader property _DitherMode
                                    //    //    //Stranded Deep LOD Mod : grass shader property _DitherDistance
                                    //    //    //Stranded Deep LOD Mod : grass shader property _DitherWidth
                                    //    //    //Stranded Deep LOD Mod : grass shader property _PackedNormals
                                    //    //    //Stranded Deep LOD Mod : grass shader property _InvertedNormals
                                    //    //    //Stranded Deep LOD Mod : grass shader property _Translucent
                                    //    //    //Stranded Deep LOD Mod : grass shader property _Translucency
                                    //    //}

                                    //    //Stranded Deep LOD Mod : grass shader _DitherDistance 20
                                    //    //Stranded Deep LOD Mod : grass shader _Cull 0
                                    //    //Stranded Deep LOD Mod : grass shader _DitherMode 2

                                    //    Debug.Log("Stranded Deep LOD Mod : grass shader _DitherDistance " + r.sharedMaterial.GetFloat("_DitherDistance"));
                                    //    r.sharedMaterial.SetFloat("_DitherDistance", 200.0f);
                                    //    //Debug.Log("Stranded Deep LOD Mod : grass shader _Cull " + r.sharedMaterial.GetFloat("_Cull"));
                                    //    //Debug.Log("Stranded Deep LOD Mod : grass shader _DitherMode " + r.sharedMaterial.GetFloat("_DitherMode"));
                                    //}
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Stranded Deep LOD Mod : grass Renderers error : " + e);
                                }
                            }
                        }
                        //Debug.Log("Stranded Deep LOD Mod : adding grass " + pos.x + " / " + pos.z);
                        pos = default(Vector3);
                    }
                }
            }
        }

        #endregion

        #region Increased terrain LOD

        private static List<Zone> _handledLODZones = new List<Zone>();

        private static void IncreaseTerrainLOD()
        {
            try
            {
                // Zone defaults
                //this._terrain.detailObjectDistance = 70f;
                //this._terrain.heightmapPixelError = 45f;

                //Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD");
                if (StrandedWorld.Instance != null
                    && StrandedWorld.Instance.Zones != null)
                {
                    if (StrandedWorld.Instance.Zones.Length == 0)
                    {
                        Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD : zones empty");
                    }
                    else
                    {
                        if (StrandedWorld.Instance.Zones.Length <= _handledLODZones.Count)
                            return;

                        //Debug.Log("Stranded Deep Better Ground Textures Mod : Updating terrain layer textures : looping zones");
                        foreach (Zone z in StrandedWorld.Instance.Zones)
                        {
                            if (!z.Loaded)
                                continue;

                            try
                            {
                                //Stranded Deep Better LOD Mod : Zone terrain basemapDistance: 180
                                //Stranded Deep Better LODMod : Zone terrain detailObjectDistance: 70
                                //Stranded Deep Better LOD Mod : Zone terrain heightmapPixelError: 45
                                //Stranded Deep Better LOD Mod : Zone terrain heightmapMaximumLOD: 0
                                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD : Zone " + z.name + " terrain basemapDistance : " + z.Terrain.basemapDistance);
                                // Heightmap patches beyond basemap distance will use a precomputed low res basemap.
                                // This improves performance for far away patches.Close up Unity renders the heightmap using splat maps by blending between any amount of provided terrain textures.
                                //z.Terrain.basemapDistance = 600;
                                z.Terrain.basemapDistance = 2000;
                                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD : Zone" + z.name + "terrain detailObjectDistance : " + z.Terrain.detailObjectDistance);
                                // Detail objects will be displayed up to this distance.
                                //z.Terrain.detailObjectDistance = 300;
                                z.Terrain.detailObjectDistance = 600;
                                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD : Zone" + z.name + "terrain heightmapPixelError : " + z.Terrain.heightmapPixelError);
                                // An approximation of how many pixels the terrain will pop in the worst case when switching lod.
                                // A higher value reduces the number of polygons drawn.
                                z.Terrain.heightmapPixelError = 5;
                                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD : Zone" + z.name + "terrain heightmapMaximumLOD : " + z.Terrain.heightmapMaximumLOD);
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD error for zone " + z.name + " : " + ex);
                            }
                            finally
                            {
                                _handledLODZones.Add(z);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD error : " + e);
            }
        }

        #endregion

        //public static bool isMeshInCameraView(Mesh mesh, Camera camera)
        //{
        //    Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        //    foreach(Plane plane in frustumPlanes)
        //    {
        //        Debug.Log("Stranded Deep LOD Mod : plane distance " + plane);
        //    }
        //    return GeometryUtility.TestPlanesAABB(frustumPlanes, mesh.bounds);
        //}

        private static void Reset()
        {
            try
            {
                fr = null;

                //if (_LODhandler != null)
                //    _LODhandler.Reset();
                //if (_handledLodControllers != null)
                //    _handledLodControllers.Clear();

                //FishSpawnersToHandle = null;
                //lastPassFishSpawnersCount = 0;
                //currentFishSpawnerIndex = -1;
                //if (multipliedFishes != null)
                //    multipliedFishes.Clear();
                //if (_handledFishRenderers != null)
                //    _handledFishRenderers.Clear();

                multiplyFishesDone = false;

                //ResetParticles();

                grid_object_green_grass_01 = null;
                grid_object_green_grass_03 = null;
                grid_object_ground_cover_a = null;
                grid_object_ground_cover_c = null;

                if (_handledZones != null)
                    _handledZones.Clear();
                if (_handledLODZones != null)
                    _handledLODZones.Clear();

                _handledShadows = false;

                flag = 0;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : Reset error : " + e);
            }
        }

        static FieldInfo fi_lodControllers = typeof(LodManager).GetField("_lodControllers", BindingFlags.NonPublic | BindingFlags.Instance);
        //private static List<LodController> _handledLodControllers = new List<LodController>();
        //private static List<Lod> _handledLods = new List<Lod>();

//        internal static LODHandler _LODhandler = new LODHandler();

//        private static void IncreaseLODs()
//        {
//            try
//            {
//                if (fi_lodControllers == null || LodManager.Instance == null)
//                    return;

//                if (PlayerRegistry.LocalPlayer == null)
//                    return;

//                Zone zone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
//                if (zone == null)
//                {
//                    Debug.Log("Stranded Deep LOD Mod : IncreaseLODs : no zone found for impostor creation");
//                }
//                else
//                {
//                    zone.CreateImpostors();
//                }

//                if (_LODhandler.MustFillQueue)
//                {
//                    List<LodController> _lodControllers = fi_lodControllers.GetValue(LodManager.Instance) as List<LodController>;
//                    _LODhandler.FillQueue(_lodControllers.ToArray());
//                }
//                else
//                {
//                    _LODhandler.Handle();
//                }

//                return;
//                //LodManager.LOD_BIAS = (ultraDistance ? 10 : 5);

//                List<LodController> lodControllers = fi_lodControllers.GetValue(LodManager.Instance) as List<LodController>;

//                Debug.Log("Stranded Deep LOD Mod IncreaseLODs step 1 (ms) = " + chrono.ElapsedMilliseconds);

//                if (lodControllers == null)
//                    return;

//                Debug.Log("Stranded Deep LOD Mod : number of LODs = " + lodControllers.Count);

//                foreach (LodController lc in lodControllers)
//                {
//                    try
//                    {
//                        if (_handledLodControllers.Contains(lc))
//                            continue;

//                        Debug.Log("Stranded Deep LOD Mod IncreaseLODs step 2.1 (ms) = " + chrono.ElapsedMilliseconds);

//                        //Debug.Log("Stranded Deep LOD Mod : lod info : " + lc.gameObject.name);

//                        int bestLOD = 3;
//                        for (int i = 0; i < lc.LodGroup.Lods.Count; i++)
//                        {
//                            Lod lod = lc.LodGroup.Lods[i];
//                            if (lod.Renderers[0].name.Contains("LOD0") && bestLOD > 0)
//                            {
//                                bestLOD = 0;
//                            }
//                            else if (lod.Renderers[0].name.Contains("LOD1") && bestLOD > 1)
//                            {
//                                bestLOD = 1;
//                            }
//                            else if (lod.Renderers[0].name.Contains("LOD2") && bestLOD > 2)
//                            {
//                                bestLOD = 2;
//                            }
//                        }
//                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : best LOD found : " + bestLOD);

//                        //for (int i = 0; i < lc.LodGroup.Lods.Count; i++)
//                        for (int i = lc.LodGroup.Lods.Count - 1; i >= 0; i--)
//                        {
//                            Lod lod = lc.LodGroup.Lods[i];
//                            Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name);
//                            Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / cull distance " + lod.CullingDistance);
//                            Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / IsImpostor " + lod.IsImpostor);

//                            if (!lod.Renderers[0].name.Contains("LOD" + bestLOD) && !lod.IsImpostor)
//                            {
//                                // remove
//                                Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / cull distance " + lod.CullingDistance);
//                                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / removing LOD at " + i);
//                                //lc.LodGroup.Lods.RemoveAt(i);
//                            }
//                            else
//                            {
//                                Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " old cull distance " + lod.CullingDistance);
//                                if (!lod.IsImpostor)
//                                {
//                                    lod.CullingDistance = lod.CullingDistance * (ultraMFBBQDistance ? 1000 : (ultraDistance ? 10 : 5));
//                                    Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " new cull distance " + lod.CullingDistance);
//                                }
//                            }
//                        }

//                        FieldInfo fiCull = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);
//#warning perf ?
//                        //fiCull.SetValue(lc, 2000);

//                        Debug.Log("Stranded Deep LOD Mod IncreaseLODs step 2.2 (ms) = " + chrono.ElapsedMilliseconds);

//                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : " + (lc.Impostor != null ? " has impostor " : "has no impostor"));
//                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : _localImpostorCullingDistance " + fiCull.GetValue(lc));
//                        if ((lc.gameObject.name.Contains("PINE_SMALL")
//                            || lc.gameObject.name.Contains("PALM_")
//                            || lc.gameObject.name.Contains("YUCCA")
//                            || lc.gameObject.name.Contains("ROCK"))
//                            && lc.Impostor == null)
//                        {
//                            FieldInfo fiScope = typeof(LodController).GetField("_scope", BindingFlags.NonPublic | BindingFlags.Instance);
//                            fiScope.SetValue(lc, ImposterScope.All);

//                            Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller position : " + lc.transform.position);
//                            Zone z = StrandedWorld.GetZone(lc.transform.position, false);
//                            if (z == null)
//                            {
//                                Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : no zone found for impostor creation");
//                            }
//                            else
//                            {
//#warning perf ?
//                                //MethodInfo mi = typeof(LodController).GetMethod("CreateImpostor", BindingFlags.NonPublic | BindingFlags.Instance);
//                                //mi.Invoke(lc, new object[] { });
//                            }
//                        }

//                        Debug.Log("Stranded Deep LOD Mod IncreaseLODs step 2.3 (ms) = " + chrono.ElapsedMilliseconds);

//                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : Scope " + lc.Scope);
//                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : Mode " + lc.Mode);
//                        if (lc.Impostor != null
//                            && lc.Impostor.Lod != null)
//                        {
//                            Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : lc.Impostor.Lod.CullingDistance " + lc.Impostor.Lod.CullingDistance);
//                        }

//                        //lc.Impostor.Lod.CullingDistance = 2000;
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.Log("Stranded Deep LOD Mod : error while handling LodController for " + lc.gameObject.name + " : " + ex);
//                    }
//                    finally
//                    {
//                        _handledLodControllers.Add(lc);
//                    }
//                }

//                Debug.Log("Stranded Deep LOD Mod IncreaseLODs step 3 (ms) = " + chrono.ElapsedMilliseconds);

//                // 20
//                //FollowSpawn.GRID_DIMENSION = 80;

//                //IList<SetupAdvancedFoliageShader> foliageShaders = Beam.Game.FindObjectsOfType<SetupAdvancedFoliageShader>();
//                //if (foliageShaders != null && foliageShaders.Count > 0)
//                //{
//                //    foreach (SetupAdvancedFoliageShader setupShader in foliageShaders)
//                //    {
//                //        Debug.Log("Stranded Deep LOD Mod : foliageShader DetailDistanceForGrassShader " + setupShader.DetailDistanceForGrassShader);
//                //        setupShader.DetailDistanceForGrassShader = 160;
//                //        Debug.Log("Stranded Deep LOD Mod : foliageShader new DetailDistanceForGrassShader " + setupShader.DetailDistanceForGrassShader);

//                //        Debug.Log("Stranded Deep LOD Mod : foliageShader SmallDetailsDistance " + setupShader.SmallDetailsDistance);
//                //        setupShader.SmallDetailsDistance = 140;
//                //        Debug.Log("Stranded Deep LOD Mod : foliageShader new SmallDetailsDistance " + setupShader.SmallDetailsDistance);

//                //        Debug.Log("Stranded Deep LOD Mod : foliageShader MediumDetailsDistance " + setupShader.MediumDetailsDistance);
//                //        setupShader.MediumDetailsDistance = 180;
//                //        Debug.Log("Stranded Deep LOD Mod : foliageShader new MediumDetailsDistance " + setupShader.MediumDetailsDistance);
//                //    }
//                //}

//                //// 20
//                //FollowSpawn.GRID_DIMENSION = 40;

//                //FollowSpawn[] followSpawns = Beam.Game.FindObjectsOfType<FollowSpawn>();
//                //foreach (FollowSpawn fs in followSpawns)
//                //{
//                //    FieldInfo fi_grass = typeof(FollowSpawn).GetField("_grass", BindingFlags.NonPublic | BindingFlags.Instance);
//                //    if (fi_grass != null && (bool)fi_grass.GetValue(fs))
//                //    {
//                //        Debug.Log("Stranded Deep LOD Mod : grass spawner found");
//                //        FieldInfo fiGridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);
//                //        FollowSpawn.BiomeGrid[] biomeGrid = fiGridObjects2.GetValue(fs) as FollowSpawn.BiomeGrid[];
//                //        foreach (FollowSpawn.BiomeGrid bg in biomeGrid)
//                //        {
//                //            Debug.Log("Stranded Deep LOD Mod : grass biome " + bg.name);
//                //            foreach (FollowSpawn.GridObject gridObject in bg.biomeParameters)
//                //            {
//                //                if (gridObject.grass)
//                //                {
//                //                    Debug.Log("Stranded Deep LOD Mod : grass grid object " + gridObject.objectsToSpawn[0].name + " / rarity : " + gridObject.rarity);
//                //                    Debug.Log("Stranded Deep LOD Mod : grass grid object " + gridObject.objectsToSpawn[0].name + " / maxObjects : " + gridObject.maxObjects);
//                //                    Debug.Log("Stranded Deep LOD Mod : grass grid object " + gridObject.objectsToSpawn[0].name + " / gridCheckDensity : " + gridObject.gridCheckDensity);

//                //                    foreach (GameObject go in gridObject.objectsToSpawn)
//                //                    {
//                //                        LODGroup[] lgs = go.GetComponents<LODGroup>();
//                //                        foreach (LODGroup lg in lgs)
//                //                        {
//                //                            Debug.Log("Stranded Deep LOD Mod : grass grid object LodGroup found");
//                //                            LOD[] lods = lg.GetLODs();
//                //                            for (int i = 0; i < lods.Length; i++)
//                //                            {
//                //                                LOD lod = lods[i];

//                //                                //if (_handledLods.Contains(lod))
//                //                                //    continue;

//                //                                Debug.Log("Stranded Deep LOD Mod : lod info : " + lod.renderers[0].name);
//                //                                //lod.CullingDistance = lod.CullingDistance * (ultraDistance ? 10 : 5);
//                //                                //Debug.Log("Stranded Deep LOD Mod : new lod info : " + lod.Renderers[0].name + " / cull distance " + lod.CullingDistance);

//                //                                //_handledLods.Add(lod);
//                //                            }
//                //                        }

//                //                        //LODGroup[] lgs = go.GetComponents<LODGroup>();
//                //                        //foreach (LODGroup lg in lgs)
//                //                        //{
//                //                        //    LOD[] lods = lg.GetLODs();
//                //                        //    foreach (LOD lod in lods)
//                //                        //    {
//                //                        //    }
//                //                        //}
//                //                    }
//                //                }

//                //                if (gridObject.grass)
//                //                {
//                //                    //gridObject.
//                //                }
//                //            }
//                //        }
//                //    }
//                //    else
//                //    {
//                //        // FollowSpawn FollowSpawn.BiomeGrid[] GridObjects2; <- increase some densities
//                //        FieldInfo fiGridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);
//                //        FollowSpawn.BiomeGrid[] biomeGrid = fiGridObjects2.GetValue(fs) as FollowSpawn.BiomeGrid[];
//                //        foreach (FollowSpawn.BiomeGrid bg in biomeGrid)
//                //        {
//                //            Debug.Log("Stranded Deep LOD Mod : biome " + bg.name);
//                //            foreach (FollowSpawn.GridObject go in bg.biomeParameters)
//                //            {
//                //                Debug.Log("Stranded Deep LOD Mod : grid object " + go.objectsToSpawn[0].name + " / rarity : " + go.rarity);
//                //                Debug.Log("Stranded Deep LOD Mod : grid object " + go.objectsToSpawn[0].name + " / maxObjects : " + go.maxObjects);
//                //                //if (indexedMaxObjects.ContainsKey(go.objectsToSpawn[0].name))
//                //                //    go.maxObjects = indexedMaxObjects[go.objectsToSpawn[0].name];
//                //                //else
//                //                //    go.maxObjects = 50;
//                //            }
//                //        }
//                //    }
//                //}



//                ////CullingSettings
//                //float[] array = new float[32];
//                //array[Layers.INTERACTIVE_OBJECTS] = 200f;
//                //array[Layers.PARTICLES] = 200f;
//                //array[Layers.TERRAIN_OBJECTS] = 200f;
//                //array[Layers.TERRAIN_DETAILS] = 200f;
//                //foreach (IGameCamera gameCamera in Cameras.AllCameras)
//                //{
//                //    gameCamera.Camera.layerCullDistances = array;
//                //    gameCamera.Camera.layerCullSpherical = true;
//                //}

//                //Debug.Log("Stranded Deep LOD Mod : layer  PLAYER " + Layers.PLAYER);
//                //Debug.Log("Stranded Deep LOD Mod : layer  INTERACTIVE_TREES " + Layers.INTERACTIVE_TREES);
//                //Debug.Log("Stranded Deep LOD Mod : layer  CONNECTOR_FOUNDATION " + Layers.CONNECTOR_FOUNDATION);
//                //Debug.Log("Stranded Deep LOD Mod : layer  CONNECTOR_PANEL " + Layers.CONNECTOR_PANEL);
//                //Debug.Log("Stranded Deep LOD Mod : layer  CONSTRUCTIONS_RAFTS " + Layers.CONSTRUCTIONS_RAFTS);
//                //Debug.Log("Stranded Deep LOD Mod : layer  CONSTRUCTIONS_SMALL " + Layers.CONSTRUCTIONS_SMALL);
//                //Debug.Log("Stranded Deep LOD Mod : layer  CONSTRUCTIONS " + Layers.CONSTRUCTIONS);
//                //Debug.Log("Stranded Deep LOD Mod : layer  PARTICLES " + Layers.PARTICLES);
//                //Debug.Log("Stranded Deep LOD Mod : layer  INTERACTIVE_OBJECTS " + Layers.INTERACTIVE_OBJECTS);
//                //Debug.Log("Stranded Deep LOD Mod : layer  PROJECTILE_HITBOXES " + Layers.PROJECTILE_HITBOXES);
//                //Debug.Log("Stranded Deep LOD Mod : layer  PROJECTILES " + Layers.PROJECTILES);
//                //Debug.Log("Stranded Deep LOD Mod : layer  TERRAIN_DETAILS " + Layers.TERRAIN_DETAILS);
//                //Debug.Log("Stranded Deep LOD Mod : layer  TERRAIN_OBJECTS " + Layers.TERRAIN_OBJECTS);
//                //Debug.Log("Stranded Deep LOD Mod : layer  TERRAIN " + Layers.TERRAIN);
//                //Debug.Log("Stranded Deep LOD Mod : layer  WATER " + Layers.WATER);
//                //Debug.Log("Stranded Deep LOD Mod : layer  IGNORE_RAYCAST " + Layers.IGNORE_RAYCAST);
//                //Debug.Log("Stranded Deep LOD Mod : layer  UI_MAIN_MENU " + Layers.UI_MAIN_MENU);

//                ////CullingSettings
//                //float[] array = new float[32];
//                //array[Layers.INTERACTIVE_OBJECTS] = 0;
//                //array[Layers.PARTICLES] = 0;
//                //array[Layers.TERRAIN_OBJECTS] = 0;
//                //array[Layers.TERRAIN_DETAILS] = 0;
//                //foreach (IGameCamera gameCamera in Cameras.AllCameras)
//                //{
//                //    gameCamera.Camera.layerCullDistances = array;
//                //    //gameCamera.Camera.layerCullSpherical = true;

//                //    for (int layer = 0; layer < gameCamera.Camera.layerCullDistances.Length; layer++)
//                //    {
//                //        Debug.Log("Stranded Deep LOD Mod : " + gameCamera.Camera.name + " layer " + layer + " cull distance " + gameCamera.Camera.layerCullDistances[layer]);
//                //        gameCamera.Camera.layerCullDistances[layer] = 0;
//                //        Debug.Log("Stranded Deep LOD Mod : " + gameCamera.Camera.name + " layer " + layer + " new cull distance " + gameCamera.Camera.layerCullDistances[layer]);
//                //    }
//                //}

//                //for (int i = 0; i < Camera.allCameras.Length; i++)
//                //{
//                //    float[] array = new float[32];
//                //    array = Camera.allCameras[i].layerCullDistances;
//                //    array[8] = (float)this.SmallDetailsDistance;
//                //    array[9] = (float)this.MediumDetailsDistance;
//                //    Camera.allCameras[i].layerCullDistances = array;
//                //}
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep LOD Mod : error in IncreaseLODs : " + e);
//            }
//        }

//        private static void IncreaseUnderwaterObjects()
//        {
//            // 20
//            FollowSpawn.GRID_DIMENSION = 20;

//            try
//            {
//                if (false)
//                {
//                    int ratio = FollowSpawn.GRID_DIMENSION / 20;
//                    Dictionary<string, int> indexedMaxObjects = new Dictionary<string, int>();
//                    indexedMaxObjects.Add("SHORELINE_ROCK_2", 5 * ratio * 10);
//                    indexedMaxObjects.Add("CORAL_ROCK_1", 4 * ratio * 5);
//                    indexedMaxObjects.Add("Brain_Coral", 6 * ratio);
//                    indexedMaxObjects.Add("Coral_Group_White", 20 * ratio);
//                    indexedMaxObjects.Add("StagHorn_Coral", 15 * ratio * 5);
//                    indexedMaxObjects.Add("Seaweed", 150 * ratio * 10);
//                    indexedMaxObjects.Add("Table_Coral", 5 * ratio * 10);
//                    indexedMaxObjects.Add("SEA_URCHIN", 5 * ratio);
//                    indexedMaxObjects.Add("PARTICLE_BUBBLE_STREAM", 4 * ratio);
//                    indexedMaxObjects.Add("Kelp_1", 10 * ratio * 5);
//                    indexedMaxObjects.Add("Shoreline_Seaweed", 100 * ratio * 10);
//                    indexedMaxObjects.Add("Tube_Coral_Blue", 20 * ratio);
//                    indexedMaxObjects.Add("Table_Coral_Brown_1", 20 * ratio * 10);
//                    indexedMaxObjects.Add("Starfish_Blue", 5 * ratio);
//                    indexedMaxObjects.Add("Broken_Barrel", 1 * ratio);

//                    indexedMaxObjects.Add("green_grass_01", 600 * ratio);
//                    indexedMaxObjects.Add("green_grass_03", 600 * ratio);
//                    indexedMaxObjects.Add("ground_cover_a", 100 * ratio);
//                    indexedMaxObjects.Add("ground_cover_c", 100 * ratio);
//                    indexedMaxObjects.Add("Grass_Dry", 400 * ratio);


//                    //LodManager.LOD_BIAS = 200;
//                    FollowSpawn[] followSpawns = Beam.Game.FindObjectsOfType<FollowSpawn>();
//                    foreach (FollowSpawn fs in followSpawns)
//                    {
//                        // FollowSpawn FollowSpawn.BiomeGrid[] GridObjects2; <- increase some densities
//                        FieldInfo fiGridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);
//                        FollowSpawn.BiomeGrid[] biomeGrid = fiGridObjects2.GetValue(fs) as FollowSpawn.BiomeGrid[];
//                        foreach (FollowSpawn.BiomeGrid bg in biomeGrid)
//                        {
//                            Debug.Log("Stranded Deep LOD Mod : biome " + bg.name);
//                            foreach (FollowSpawn.GridObject go in bg.biomeParameters)
//                            {
//                                //Debug.Log("Stranded Deep LOD Mod : grid object " + go.objectsToSpawn[0].name + " / rarity : " + go.rarity);
//                                //Debug.Log("Stranded Deep LOD Mod : grid object " + go.objectsToSpawn[0].name + " / maxObjects : " + go.maxObjects);
//                                if (indexedMaxObjects.ContainsKey(go.objectsToSpawn[0].name))
//                                    go.maxObjects = indexedMaxObjects[go.objectsToSpawn[0].name];
//                                else
//                                    go.maxObjects = 50;
//                            }
//                        }
//                    }
//                }

//                SetupAdvancedFoliageShader[] foliageShaders = Beam.Game.FindObjectsOfType<SetupAdvancedFoliageShader>();
//                foreach (SetupAdvancedFoliageShader fs in foliageShaders)
//                {
//                    //Debug.Log("Stranded Deep LOD Mod : increase grass drawing distance");
//                    //fs.DetailDistanceForGrassShader = 500f;
//                }

//                //TreeManager.Instance.Wind = new Vector4(5, 0, 5);
//                //TreeManager.Instance.WindFrequency = 1.6f;
//                //TreeManager.Instance.
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep LOD Mod : initial tests failed : " + e);
//            }
//        }

//        private static void IncreaseDrawingDistanceUpdate()
//        {
//            IncreaseUnderwaterObjects();

//            // 1
//            try
//            {
//                float[] array = new float[32];
//                array[Layers.INTERACTIVE_OBJECTS] = 1000f;
//                array[Layers.PARTICLES] = 1000f;
//                array[Layers.TERRAIN_OBJECTS] = 1000f;
//                array[Layers.TERRAIN_DETAILS] = 1000f;
//                foreach (IGameCamera gameCamera in Cameras.AllCameras)
//                {
//#warning TODO : add anti-infinite loop
//                    Debug.Log("Stranded Deep LOD Mod : increase camera drawing distance");
//                    gameCamera.Camera.layerCullDistances = array;
//                    gameCamera.Camera.layerCullSpherical = true;
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep LOD Mod : increase camera drawing distance failed : " + e);
//            }

//            // 2

//            try
//            {
//                //Debug.Log("Stranded Deep LOD Mod : increasing impostor culling distance");
//                // int
//                FieldInfo fiCull = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);
//                FieldInfo fiLodGroup = typeof(LodController).GetField("_lodGroup", BindingFlags.NonPublic | BindingFlags.Instance);

//                LodController[] lcs = Beam.Game.FindObjectsOfType<LodController>();
//                foreach (LodController lc in lcs)
//                {
//                    //Debug.Log("Stranded Deep LOD Mod : current culling distance for " + lc.GetType().Name + " : " + fiCull.GetValue(lc));
//#warning TODO : add anti-infinite loop
//                    //Debug.Log("Stranded Deep LOD Mod : increasing impostor culling distance to 1000 for " + lc.GetType().Name);
//                    //fiCull.SetValue(lc, 1000);

//                    //int i = 1000;
//                    //foreach(Lod lod in lc.LodGroup.Lods)
//                    //{
//                    //    Debug.Log("Stranded Deep LOD Mod : increasing Lod culling distance for : " + lod.GetType().Name);
//                    //    lod.CullingDistance = i;
//                    //    i += 1000;
//                    //}

//                    LodGroup lg = fiLodGroup.GetValue(lc) as LodGroup;
//                    if (lg != null)
//                    {
//                        foreach (Lod lod in lg.Lods)
//                        {
//                            //Debug.Log("Stranded Deep LOD Mod : current culling distance for " + lod.GetType().Name + " : " + lod.CullingDistance);
//                            // 7 12
//                            //if (lod.CullingDistance == 10)
//                            //{
//                            //    lod.CullingDistance = 1;
//                            //}
//                            //if (lod.CullingDistance == 25)
//                            //{
//                            //    lod.CullingDistance = 997;
//                            //}
//                            //if (lod.CullingDistance == 40)
//                            //{
//                            //    lod.CullingDistance = 998;
//                            //}
//                            //if (lod.CullingDistance == 60)
//                            //{
//                            //    lod.CullingDistance = 999;
//                            //}
//                            //if (lod.CullingDistance == 100)
//                            //{
//                            //    lod.CullingDistance = 1000;
//                            //}
//                            //Debug.Log("Stranded Deep LOD Mod : new culling distance for " + lod.GetType().Name + " : " + lod.CullingDistance);
//                        }
//                    }

//                    //dynamic impostor DISTANCE BiAS (static)
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep LOD Mod : increase impostor culling distance failed : " + e);
//            }


//            try
//            {
//                //Debug.Log("Stranded Deep LOD Mod : increasing impostor culling distance");
//                // int
//                //FieldInfo fi = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);

//                //                LODGroup[] lgs = Beam.Game.FindObjectsOfType<LODGroup>();
//                //                foreach (LODGroup lg in lgs)
//                //                {
//                //#warning TODO : add anti-infinite loop
//                //                    Debug.Log("Stranded Deep LOD Mod : force LOD group 0 for : " + lg.GetType().Name);
//                //                    //fi.SetValue(lc, 1000);
//                //                    lg.ForceLOD(0);
//                //                }

//                //LodGroup lg;
//                //lg.Lods[0].
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep LOD Mod : force LOD group 0 failed : " + e);
//            }

//            //Beam.Rendering.LodController lc;
//            //private int _localImpostorCullingDistance = 200;

//            //FieldInfo fi = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic);
//            //LodController[] lodControllers = Game.FindObjectsOfType<LodController>();
//            //foreach (LodController lc in lodControllers)
//            //{
//            //    // 200 : make higher ?
//            //    if (fi != null)
//            //        fi.SetValue(lc, 2000);
//            //    //lc._localImpostorCullingDistance
//            //}

//            // 3
//            /// 80 : make higher
//            //TreeManager.Instance._detailDistanceForGrassShader

//            // 4 
//            //SetupAdvancedFoliageShader.DetailDistanceForGrassShader

//            // 5
//            // FishRendererBase._cullingDistance

//            //6 object
//            // CullingSettings

//            // 7
//            // Lod.CullingDistance
//        }

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
                            if (tokens[0].Contains("moreFishes"))
                            {
                                moreFishes = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("increaseFishDrawingDistance"))
                            {
                                increaseFishDrawingDistance = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("increasLODs"))
                            {
                                increasLODs = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("ultraDistance"))
                            {
                                ultraDistance = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("ultraMFBBQDistance"))
                            {
                                ultraMFBBQDistance = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("addSmallFishes"))
                            {
                                addSmallFishes = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("addShrimps"))
                            {
                                addShrimps = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("addJellyFishes"))
                            {
                                addJellyFishes = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("permanentGroundCover"))
                            {
                                permanentGroundCover = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("increaseTerrainLOD"))
                            {
                                increaseTerrainLOD = bool.Parse(tokens[1]);
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
                sb.AppendLine("moreFishes=" + moreFishes + ";");
                sb.AppendLine("increaseFishDrawingDistance=" + increaseFishDrawingDistance + ";");
                sb.AppendLine("increasLODs=" + increasLODs + ";");
                sb.AppendLine("ultraDistance=" + ultraDistance + ";");
                sb.AppendLine("ultraMFBBQDistance=" + ultraMFBBQDistance + ";");

                sb.AppendLine("addSmallFishes=" + addSmallFishes + ";");
                sb.AppendLine("addShrimps=" + addShrimps + ";");
                sb.AppendLine("addJellyFishes=" + addJellyFishes + ";");
                sb.AppendLine("permanentGroundCover=" + permanentGroundCover + ";");
                sb.AppendLine("increaseTerrainLOD=" + increaseTerrainLOD + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
