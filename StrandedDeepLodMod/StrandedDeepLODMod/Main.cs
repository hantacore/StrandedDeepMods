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
using Ceto;
using StrandedDeepModsUtils;
using System.IO;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        private static string configFileName = "StrandedDeepLODMod.config";

        internal static bool moreFishes = false;
        internal static bool increaseFishDrawingDistance = false;
        internal static bool increaseLODs = false;
        internal static bool ultraDistance = false;
        internal static bool permanentGroundCover = false;
        internal static bool increaseTerrainLOD = false;
        internal static bool increaseOceanLOD = false;
        internal static bool increaseShadowsQuality = false;

        internal static bool addSmallFishes = false;
        internal static bool addShrimps = false;
        internal static bool addJellyFishes = false;

        internal static bool ultraUnderwaterDetail = false;

        internal static bool betterFoam = false;
        internal static bool underwaterParticles = false;

        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        internal static bool perfCheck = true;

        private static Harmony harmony;

        static FieldInfo fi_lodControllers = typeof(LodManager).GetField("_lodControllers", BindingFlags.NonPublic | BindingFlags.Instance);

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

                tex = new Texture2D(4096, 4096, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.foam.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.foam.png", tex);

                tex = new Texture2D(4096, 4096, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.foam2.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.foam2.png", tex);

                tex = new Texture2D(4096, 4096, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.random_mask.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.random_mask.png", tex);

                tex = new Texture2D(1024, 1024, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepLODMod.assets.Textures.particles.png"));
                _indexedTextures.Add("StrandedDeepLODMod.assets.Textures.particles.png", tex);

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
            increaseLODs = GUILayout.Toggle(increaseLODs, "Increase drawing distances");
            ultraDistance = GUILayout.Toggle(ultraDistance, "Ultra drawing distance (remove impostors, active only if \"Increase drawing distances\" is active, performance heavy)");
            increaseTerrainLOD = GUILayout.Toggle(increaseTerrainLOD, "Improve terrain smoothness");
            increaseOceanLOD = GUILayout.Toggle(increaseTerrainLOD, "Improve ocean smoothness");
            increaseShadowsQuality = GUILayout.Toggle(increaseShadowsQuality, "Improve shadows quality (experimental)");
            ultraUnderwaterDetail = GUILayout.Toggle(ultraUnderwaterDetail, "Ultra underwater details");
            betterFoam = GUILayout.Toggle(betterFoam, "Better foam textures");
            underwaterParticles = GUILayout.Toggle(underwaterParticles, "Underwater particles");
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

                if (permanentGroundCover && flag == 4)
                {
                    AddPermanentGroundCover();
                }

                if (increaseTerrainLOD && flag == 5)
                {
                    IncreaseTerrainLOD();
                }

                if (increaseShadowsQuality && flag == 5)
                {
                    IncreaseShadowsQuality();
                }

                flag++;

                // round robin
                if (flag > 5)
                    flag = 0;

                if (underwaterParticles)
                {
                    AddUnderwaterParticles();
                }
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

        private static void SetUltraQuality()
        {
            if (!ultraDistance)
                return;

            Debug.Log("Stranded Deep LOD Mod : better quality setting " + (ultraDistance ? "Ultra" : "Increased"));
            float cullDistance = 999f;
            //LodManager.LOD_BIAS = 25f;

            float[] layerCullDistances = new float[32];
            foreach (IGameCamera gameCamera in Cameras.AllCameras)
            {
                Array.Copy(gameCamera.Camera.layerCullDistances, layerCullDistances, 32);
                layerCullDistances[Layers.INTERACTIVE_OBJECTS] = 300;
                layerCullDistances[Layers.PARTICLES] = cullDistance;
                layerCullDistances[Layers.TERRAIN_OBJECTS] = cullDistance;
                layerCullDistances[Layers.TERRAIN_DETAILS] = cullDistance; // IMPOSTORS ?

                gameCamera.Camera.layerCullDistances = layerCullDistances;
            }
        }

        private static void SetUltraOceanQuality()
        {
            ProjectedGrid projectedGrid = UnityEngine.Object.FindObjectOfType<ProjectedGrid>();
            if (projectedGrid != null)
            {
                projectedGrid.resolution = MESH_RESOLUTION.ULTRA;
            }
            WaveSpectrum waveSpectrum = UnityEngine.Object.FindObjectOfType<WaveSpectrum>();
            if (waveSpectrum != null)
            {
                waveSpectrum.fourierSize = FOURIER_SIZE.HIGH_128_CPU;
            }
        }

        private static bool particlesAdded = false;
        private static void AddUnderwaterParticles()
        {
            try
            {
                if (PlayerRegistry.LocalPlayer == null || ((Player)PlayerRegistry.LocalPlayer).gameObject == null)
                    return;

                GameObject playerGO = ((Player)PlayerRegistry.LocalPlayer).gameObject;
                if (!particlesAdded && playerGO.GetComponent<FollowPlayerParticleSystem>() == null)
                {
                    ParticleSystem psorig = playerGO.GetComponent<ParticleSystem>();
                    if (psorig != null)
                    {
                        Debug.Log("Stranded Deep LOD Mod : player already has particle system");
                        return;
                    }

                    Debug.Log("Stranded Deep LOD Mod : adding player underwater particle system");
                    ParticleSystem ps = playerGO.AddComponent<ParticleSystem>();
                    ps.name = FollowPlayerParticleSystem.ParticleSystemName;
                    playerGO.AddComponent<FollowPlayerParticleSystem>();
                    ps.Play();

                    particlesAdded = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : error on ParticlesTest : " + e);
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
                || !zone.isActiveAndEnabled
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
                            if (_handledZones.Contains(z))
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

                                // detailResolution Specifies the number of pixels in the detail resolution map. A larger detailResolution, leads to more accurate detail object painting.
                                // resolutionPerPatch Specifies the size in pixels of each individually rendered detail patch. A larger number reduces draw calls, but might increase triangle count since detail patches are culled on a per batch basis. A recommended value is 16. If you use a very large detail object distance and your grass is very sparse, it makes sense to increase the value.
                                //z.Terrain.terrainData.SetDetailResolution(1024, 32);
                                z.Terrain.terrainData.SetDetailResolution(2048, 64);
                                Debug.Log("Stranded Deep LOD Mod : IncreaseTerrainLOD : SetDetailResolution : " + z.Terrain.terrainData.detailResolution);
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

        private static void Reset()
        {
            try
            {
                fr = null;

                multiplyFishesDone = false;
                particlesAdded = false;

                grid_object_green_grass_01 = null;
                grid_object_green_grass_03 = null;
                grid_object_ground_cover_a = null;
                grid_object_ground_cover_c = null;

                if (_handledZones != null)
                    _handledZones.Clear();
                if (_handledLODZones != null)
                    _handledLODZones.Clear();
                if (_preloadedZones != null)
                    _preloadedZones.Clear();

                _handledShadows = false;

                foamTexturesUpdated = false;
                particlesAdded = false;

                flag = 0;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : Reset error : " + e);
            }
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
                                increaseLODs = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("ultraDistance"))
                            {
                                ultraDistance = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("increaseOceanLOD"))
                            {
                                increaseOceanLOD = bool.Parse(tokens[1]);
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
                            else if (tokens[0].Contains("ultraUnderwaterDetail"))
                            {
                                ultraUnderwaterDetail = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("betterFoam"))
                            {
                                betterFoam = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("underwaterParticles"))
                            {
                                underwaterParticles = bool.Parse(tokens[1]);
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
                sb.AppendLine("increasLODs=" + increaseLODs + ";");
                sb.AppendLine("ultraDistance=" + ultraDistance + ";");
                sb.AppendLine("increaseOceanLOD=" + increaseOceanLOD + ";");

                sb.AppendLine("addSmallFishes=" + addSmallFishes + ";");
                sb.AppendLine("addShrimps=" + addShrimps + ";");
                sb.AppendLine("addJellyFishes=" + addJellyFishes + ";");
                sb.AppendLine("permanentGroundCover=" + permanentGroundCover + ";");
                sb.AppendLine("increaseTerrainLOD=" + increaseTerrainLOD + ";");
                sb.AppendLine("ultraUnderwaterDetail=" + ultraUnderwaterDetail + ";");
                sb.AppendLine("betterFoam=" + betterFoam + ";");
                sb.AppendLine("underwaterParticles=" + underwaterParticles + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        public static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        public static void ExportTexture(Texture2D tex, string name)
        {
            try
            {
                Texture2D t = duplicateTexture(tex) as Texture2D;
                byte[] bytes = t.EncodeToPNG();
                File.WriteAllBytes("e:\\" + name + ".png", bytes);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD mod : error ExportTexture " + e);
            }
        }

        public static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            Texture2D nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return nTex;
        }

        public static Texture2D MergeShorelines(Texture2D shore, Texture2D shoreHeightmap,  Texture2D mask)
        {
            Texture2D result = new Texture2D(shore.width, shore.height);

            for (int i = 0; i < result.width; i++)
            {
                for (int j = 0; j < result.height; j++)
                {
                    result.SetPixel(i, j, shore.GetPixel(i, j));
                }
            }
            result.Apply();

            //Main.ExportTexture(result, @"MergeShorelines result");

            Texture2D resizedshoreHeightmap = null;
            if (mask != null)
            {
                resizedshoreHeightmap = Main.ResizeTexture(shoreHeightmap, result.width, result.height);
                //Main.ExportTexture(resizedshoreHeightmap, @"MergeShorelines resizedshoreHeightmap");
            }

            Texture2D resizedMask = null;
            if (mask != null)
            {
                resizedMask = Main.ResizeTexture(mask, result.width, result.height);
                //Main.ExportTexture(resizedMask, @"MergeShorelines resizedMask");
            }

            Color[] pixels = result.GetPixels();
            for (int i = 0; i < result.width; i++)
            {
                for (int j = 0; j < result.height; j++)
                {
                    Color c = result.GetPixel(i, j);
                    Color resultColor = c;
                    if (resizedshoreHeightmap != null)
                    {
                        Color shoreC = resizedshoreHeightmap.GetPixel(i, j);
                        resultColor = new Color(1, 1, 1, Mathf.Max(resultColor.a, shoreC.a));
                    }
                    result.SetPixel(i, j, resultColor);
                }
            }
            result.Apply();
            //Main.ExportTexture(result, @"MergeShorelines result height-shore");

            pixels = result.GetPixels();
            for (int i = 0; i < result.width; i++)
            {
                for (int j = 0; j < result.height; j++)
                {
                    Color c = result.GetPixel(i, j);
                    Color resultColor = c;
                    if (resizedMask != null)
                    {
                        Color maskC = resizedMask.GetPixel(i, j);
                        resultColor = new Color(1, 1, 1, Mathf.Min(c.a, maskC.a));
                    }
                    result.SetPixel(i, j, resultColor);
                }
            }
            result.Apply();

            //Main.ExportTexture(result, @"MergeShorelines result");

            return result;
        }

        public static Texture2D EnlargeTexture(Texture2D source, int newWidth, int newHeight, float ratio = 0.95f)
        {
            Texture2D result = new Texture2D(newWidth, newHeight);

            Texture2D largertex = Main.ResizeTexture(source, (int)Math.Round(ratio * newWidth), (int)Math.Round(ratio * newHeight));

            // fill the background with the first pixel in the original texture
            for (int i = 0; i < result.width; i++)
            {
                for (int j = 0; j < result.height; j++)
                {
                    result.SetPixel(i, j, source.GetPixel(0, 0));
                }
            }
            result.Apply();

            // put the resized texture in the center of the target
            Color[] pixels = largertex.GetPixels();
            for (int i = 0; i < largertex.width; i++)
            {
                for (int j = 0; j < largertex.height; j++)
                {
                    Color c = largertex.GetPixel(i, j);
                    Color resultColor = c;
                    result.SetPixel(i + ((newWidth - largertex.width) / 2), j + ((newHeight - largertex.height) / 2), resultColor);
                }
            }

            result.Apply();

            return result;
        }
    }
}
