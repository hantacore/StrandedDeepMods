using Beam;
using Beam.Crafting;
using Beam.Events;
using Beam.Rendering;
using Beam.Serialization;
using Beam.Serialization.Json;
using Beam.Terrain;
using Beam.UI;
using Beam.Utilities;
using Funlabs;
using MEC;
using SharpNeatLib.Maths;
using StrandedDeepModsUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityModManagerNet;

namespace StrandedDeepAlternativeEndgameMod
{
    static partial class Main
    {
        private static FieldInfo fi_containersGenerated = typeof(LootContainerSpawner).GetField("_containersGenerated", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_dither = typeof(LodController).GetField("_dither", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_localImpostorCullingDistance = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_scope = typeof(LodController).GetField("_scope", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_impostor = typeof(LodController).GetField("_impostor", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_fxMixerGroup = typeof(AudioManager).GetField("_fxMixerGroup", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_flareProjectile = typeof(InteractiveObject_FLAREGUN).GetField("_flareProjectile", BindingFlags.NonPublic | BindingFlags.Instance);


        private static AudioSource audioSource = null;
        private static AudioClip cargoHornSound = null;
        private static AudioClip cargoEngineSound = null;
        private static AudioClip endingMusic = null;
        private static AudioClip reporterSound = null;
        private static AudioClip crowdSound = null;

        private static float alternativeEndingCanvasDefaultScreenWidth = 1024f;
        private static float alternativeEndingCanvasDefaultScreenHeight = 768f;
        private static bool alternativeEndingCanvasVisible = false;
        private static GameObject modCanvas;

        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();
        private static string videoName = "AlternativeEndingVideo.webm";
        private static string videoFileName = "";

        private static string configFileName = "StrandedDeepAlternativeEndgameMod.config";
        private static uint cargoPrefabId = 311;
        private static List<Vector3> positions = new List<Vector3>();
        private static int currentOrigin = 0;
        private static SaveablePrefab cargoInstance = null;
        private static InteractiveObject_FLAREGUN previousFlareGun = null;
        private static DateTime _lastMovementTick = DateTime.MinValue;
        private static float absoluteSpeed = 5.0f;

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        private static bool perfCheck = true;

        public static bool isInEndgame = false;
        public static bool isInEndgameCredits = false;
        private static bool showEndgameCreditsAndEndGame = true;
        private static bool debugModeImmediateCargoAndNoVideo = false;

        private static DateTime _playEndgame = DateTime.MaxValue;
        private static int secondsBeforeEndgameVideo = 10;

        static string _infojsonlocation = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedDeepAlternativeEndgameMod/StrandedDeepAlternativeEndgameMod/Info.json";

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;

                VersionChecker.CheckVersion(modEntry, _infojsonlocation);

                ReadConfig();

                Texture2D tex = new Texture2D(1280, 1280, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.textures.smoke.png"));
                _indexedTextures.Add("StrandedDeepAlternativeEndgameMod.assets.textures.smoke.png", tex);

                tex = new Texture2D(2048, 2048, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.textures.tile_wreck_metal1 (Instance).png"));
                _indexedTextures.Add("StrandedDeepAlternativeEndgameMod.assets.textures.tile_wreck_metal1 (Instance).png", tex);

                tex = new Texture2D(2048, 2048, TextureFormat.ARGB32, false, false);
                tex.LoadImage(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.textures.tile_wreck_ledge13 (Instance).png"));
                _indexedTextures.Add("StrandedDeepAlternativeEndgameMod.assets.textures.tile_wreck_ledge13 (Instance).png", tex);

                cargoHornSound = WavUtility.ToAudioClip(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.sounds.horn.wav"));
                cargoEngineSound = WavUtility.ToAudioClip(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.sounds.cargo_passing_low.wav"));
                endingMusic = WavUtility.ToAudioClip(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.sounds.outro.wav"));
                reporterSound = WavUtility.ToAudioClip(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.sounds.reporter2.wav"));
                crowdSound = WavUtility.ToAudioClip(ExtractResource("StrandedDeepAlternativeEndgameMod.assets.sounds.confroom.wav"));

                // write video file
                string currentPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string modPath = Path.GetDirectoryName(currentPath);
                //videoFileName = Path.Combine(modPath, "alternative_endgame.webm");
                videoFileName = Path.Combine(modPath, videoName);
                Debug.Log("Stranded Deep AlternativeEndgame Mod writing down video in : " + videoFileName);
                //File.WriteAllBytes(videoFileName, ExtractResource("StrandedDeepAlternativeEndgameMod.assets.videos.alternative_endgame.webm"));
                File.WriteAllBytes(videoFileName, ExtractResource("StrandedDeepAlternativeEndgameMod.assets.videos." + videoName));

                try
                {
                    // delete old video
                    string oldVideoPath = Path.Combine(modPath, "alternative_endgame.webm");
                    if (File.Exists(oldVideoPath))
                    {
                        File.Delete(oldVideoPath);
                    }
                }
                catch { }

                Debug.Log("Stranded Deep AlternativeEndgame Mod properly loaded");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod error on load : " + e);
            }
            finally
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod load time (ms) = " + chrono.ElapsedMilliseconds);
            }

            return false;
        }

        private static void ComputeWaypoints(bool resetOrigin)
        {
            positions.Clear();
            FastRandom r = new FastRandom(StrandedWorld.WORLD_SEED);

            float zoneSize = 500f;
            float zoneSpacing = 1.25f;
            float numberOfIslandsOnDiameter = 8f;
            absoluteSpeed = 5.0f;
            try
            {
                if (WorldUtilities.IsStrandedWide())
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod ComputeWaypoints : Stranded Wide detected");
                    zoneSize = 1000f;
                    absoluteSpeed = 10.0f;
                }
            }
            catch { }
            Debug.Log("Stranded Deep AlternativeEndgame Mod ComputeWaypoints zoneSize = " + zoneSize);

            // farthest island position = _zoneSize * _zoneSpacing * 7f

            float radius = (zoneSize * zoneSpacing * numberOfIslandsOnDiameter) / 2.0f; // radius = region size
            //float radius = zoneSize * zoneSpacing * 7f;

            float angle = 360.0f / Parameters.NUMBER_OF_WAYPOINTS;
            bool directionforward = (r.Next(0, 100) >= 50);
            if (directionforward)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod cargo counter clockwise");
                for (int i = 0; i < Parameters.NUMBER_OF_WAYPOINTS; i++)
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : waypoint " + i + " angle = " + i * angle);
                    Vector3 position = CalculatePointOnSquare(radius, i * angle, Parameters.SHIP_WATERLINE);
                    positions.Add(position);
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : position = " + positions.Last());
                }
            }
            else
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod clockwise");
                for (int i = (int)Parameters.NUMBER_OF_WAYPOINTS - 1; i >= 0; i--)
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : waypoint " + i + " angle = " + i * angle);
                    Vector3 position = CalculatePointOnSquare(radius, i * angle, Parameters.SHIP_WATERLINE);
                    positions.Add(position);
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : position = " + positions.Last());
                }
            }
            if (resetOrigin)
            {
                currentOrigin = r.Next(0, positions.Count - 1);
            }
            else
            {
                // cargo exists, compute distance to center of the map, if it drifted away, reset to position 0
                if (cargoInstance != null
                    && cargoInstance.gameObject != null)
                {
                    Vector3 currentPositionLocal = cargoInstance.gameObject.transform.position;
                    float distance = Vector3.Distance(currentPositionLocal, new Vector3());
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo distance to center = " + distance);
                    if (distance > 1.5f * radius)
                    {
                        currentOrigin = r.Next(0, positions.Count - 1);
                        Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo drifted away, resetting to position at index " + currentOrigin);
                        cargoInstance.gameObject.transform.position = positions[currentOrigin];
                    }
                }
            }
            Debug.Log("Stranded Deep AlternativeEndgame Mod : starting position = [" + currentOrigin + "] /" + positions[currentOrigin]);
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Game.State == GameState.NEW_GAME || Game.State == GameState.LOAD_GAME)
            {
                debugModeImmediateCargoAndNoVideo = GUILayout.Toggle(debugModeImmediateCargoAndNoVideo, "Debug mode : immediately spawns cargo and skips ending video");
                showEndgameCreditsAndEndGame = GUILayout.Toggle(showEndgameCreditsAndEndGame, "Debug mode : uncheck to skip ending credits");

                if (GUILayout.Button("Force spawn cargo"))
                {
                    if (cargoInstance == null)
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod Force spawn cargo");
                        FieldInfo fi_daysElapsed = typeof(GameCalendar).GetField("_daysElapsed", BindingFlags.NonPublic | BindingFlags.Instance);
                        fi_daysElapsed.SetValue(GameCalendar.Instance, 20);
                    }
                }
                if (GUILayout.Button("Test 3D horn sound"))
                {
                    if (cargoInstance != null)
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod Test 3D horn sound");
                        InitCargoHorn();
                    }
                }
                if (GUILayout.Button("Find cargo position"))
                {
                    if (cargoInstance != null)
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod Find cargo position");
                        PlayerRegistry.AllPlayers[0].transform.position = new Vector3(cargoInstance.transform.position.x, 20, cargoInstance.transform.position.z);
                    }
                }
                if (GUILayout.Button("Follow cargo position"))
                {
                    if (cargoInstance != null)
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod Follow cargo position");
                        PlayerRegistry.AllPlayers[0].transform.parent = cargoInstance.transform;
                    }
                }
                if (GUILayout.Button("Unfollow cargo position"))
                {
                    if (cargoInstance != null)
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod Unfollow cargo position");
                        PlayerRegistry.AllPlayers[0].transform.parent = null;
                    }
                }
                if (GUILayout.Button("Shoot cargo flare"))
                {
                    if (cargoInstance != null)
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod Shoot cargo flare");
                        ShootFlareBack();
                    }
                }
            }
        }

        private static void InitCargoHorn()
        {
            if (cargoInstance != null)
            {
                if (audioSource == null)
                {
                    audioSource = cargoInstance.gameObject.AddComponent<AudioSource>();
                    audioSource.transform.parent = cargoInstance.transform;
                    audioSource.clip = cargoHornSound;

                    if (World.GenerationZonePositons != null
                        && World.GenerationZonePositons.Length > 0)
                    {
                        float zoneSize = 500f;
                        try
                        {
                            if (Game.FindObjectOfType<UMainMenuViewAdapter>().VersionNumberLabel.Text.Contains("Wide"))
                            {
                                Debug.Log("Stranded Deep AlternativeEndgame Mod ComputeWaypoints : Stranded Wide detected");
                                zoneSize = 1000f;
                            }
                        }
                        catch { }
                        audioSource.maxDistance = 2.5f * zoneSize;
                        Debug.Log("Stranded Deep AlternativeEndgame Mod computed maxdistance = " + audioSource.maxDistance);
                    }
                    else
                    {
                        audioSource.maxDistance = 2500;
                        Debug.Log("Stranded Deep AlternativeEndgame Mod fixed maxdistance = " + audioSource.maxDistance);
                    }
                    audioSource.minDistance = 0.05f;
                    audioSource.loop = false;
                    AudioManager instance = AudioManager.GetAudioPlayer() as AudioManager;
                    audioSource.outputAudioMixerGroup = fi_fxMixerGroup.GetValue(instance) as AudioMixerGroup;
                    cargoInstance.gameObject.AddComponent<SpatialFader>();
                    audioSource.Play();
                }
            }
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                if (Game.State == GameState.NEW_GAME || Game.State == GameState.LOAD_GAME)
                {
                    if (WorldUtilities.IsWorldLoaded() && CheckCargoAppearConditionsMet())
                    {
                        // search
                        if (cargoInstance == null)
                        {
                            bool isFirstCreation = false;
                            foreach (GameObject go in Game.FindObjectsOfType<GameObject>())
                            {
                                try
                                {
                                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : gameobject found name = " + go.name);
                                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : gameobject found isprefab = " + go.IsPrefab());
                                    SaveablePrefab sp = go.GetComponent<SaveablePrefab>();
                                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : gameobject found SaveablePrefab = " + (sp != null ? (sp.name != null ? sp.name : "empty name") : "null"));
                                    if (sp != null)
                                    {
                                        if (new MiniGuid(new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")).ToString().CompareTo(sp.ReferenceId.ToString()) == 0)
                                        {
                                            Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo reference found");
                                            sp.name = Parameters.ENDGAME_CARGO_NAME;
                                            sp.gameObject.name = Parameters.ENDGAME_CARGO_NAME;
                                        }
                                        if (sp.name != null
                                            && sp.name.CompareTo(Parameters.ENDGAME_CARGO_NAME) == 0)
                                        {
                                            Debug.Log("Stranded Deep AlternativeEndgame Mod : retrieved the endgame cargo");
                                            cargoInstance = sp;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Stranded Deep AlternativeEndgame Mod : retrieved the endgame cargo failed : " + e);
                                }
                            }

                            if (cargoInstance == null
                                && !CheckCargoAppearConditionsMet())
                            {
                                return;
                            }

                            // instanciate if not retrieved
                            if (cargoInstance == null)
                            {
                                ComputeWaypoints(true);
                                Debug.Log("Stranded Deep AlternativeEndgame Mod : spawn the endgame cargo");
                                cargoInstance = CreateSaveablePrefabLight(cargoPrefabId, positions[currentOrigin], new Quaternion(), StrandedWorld.Instance.NmlZone);
                                cargoInstance.name = Parameters.ENDGAME_CARGO_NAME;
                                cargoInstance.gameObject.name = Parameters.ENDGAME_CARGO_NAME;
                                isFirstCreation = true;
                            }
                            else
                            {
                                ComputeWaypoints(false);
                            }

                            PrepareIncreasedDistanceAndHideImpostor(isFirstCreation);
                            //AttachChildrenRecursive(cargoInstance.transform, cargoInstance.transform);
                            ComputeLastOriginFromPosition();

                            AddCargoEffects();

                            InitCargoHorn();

                            Texture2D magenta = new Texture2D(1, 1);
                            magenta.SetPixel(0, 0, Color.magenta);
                            Material mat = new Material(Shader.Find("Standard (Specular setup)"));
                            mat.SetTexture("_MainTex", magenta);
                            //mat.SetTexture("_MainTex", Main._indexedTextures["StrandedDeepAlternativeEndgameMod.assets.textures.smoke.png"]);
                            //mat.SetFloat("_Mode", 2);
                            //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            //mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            //mat.SetInt("_ZWrite", 0);
                            //mat.DisableKeyword("_ALPHATEST_ON");
                            //mat.EnableKeyword("_ALPHABLEND_ON");
                            //mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            //mat.renderQueue = 3000;
                            mat.name = "tile_wreck_woodPlanks5";

                            List<Material> materialsList = new List<Material>() { mat };

                            MeshRenderer renderertest = cargoInstance.gameObject.AddComponent<MeshRenderer>();
                            renderertest.materials = materialsList.ToArray();

                            //Renderer cargoRenderer = null;
                            //if (cargoRenderer == null)
                            //{
                            //    foreach (Renderer renderer in Beam.Game.FindObjectsOfType<Renderer>())
                            //    {
                            //        if (renderer.gameObject.name.Contains(ENDGAME_CARGO_NAME))
                            //        {
                            //            Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo renderer found");
                            //            Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo renderer found " + renderer.GetType().Name);
                            //            Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo renderer found " + renderer.material.GetType().Name);
                            //            cargoRenderer = renderer;
                            //        }
                            //    }
                            //}

                            //Renderer r = cargoInstance.gameObject.GetComponentInChildren<Renderer>();
                            //if (r != null)
                            //{
                            //    Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo renderer type : " + r.GetType().Name);
                            //    if (r is MeshRenderer)
                            //    {
                            //        //        Debug.Log("Stranded Deep AlternativeEndgame Mod : found cargo renderer");
                            //        //        r.sharedMaterial = null;
                            //        //        r.sharedMaterials = new Material[] { };

                            //        //        foreach (Material m in r.materials)
                            //        //        {
                            //        //            Texture2D magenta = new Texture2D(1, 1);
                            //        //            magenta.SetPixel(0, 0, Color.magenta);
                            //        //            string[] textureNames = m.GetTexturePropertyNames();
                            //        //            foreach (string texturename in textureNames)
                            //        //            {
                            //        //                m.SetTexture(texturename, magenta);
                            //        //            }
                            //        //        }
                            //    }
                            //}
                            //else
                            //{
                            //    Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo renderer not found");
                            //}

                        }
                        else
                        {

                            //Event currentevent = Event.current;
                            //if (currentevent.isKey)
                            //{
                            //    if (currentevent.keyCode == KeyCode.F3)
                            //    {
                            //        float distance = Vector3.Distance(cargoInstance.transform.position, PlayerRegistry.AllPlayers[0].transform.position);
                            //        Debug.Log("Stranded Deep AlternativeEndgame Mod : endgame cargo distance to player = " + distance);
                            //    }
                            //}

                            cargoInstance.name = Parameters.ENDGAME_CARGO_NAME;
                            cargoInstance.gameObject.name = Parameters.ENDGAME_CARGO_NAME;
                            UpdateTextures();

                            if (_lastMovementTick == DateTime.MinValue)
                            {
                                _lastMovementTick = GameTime.Now;
                            }
                            TimeSpan deltaTime = GameTime.Now.Subtract(_lastMovementTick);
                            if (deltaTime.TotalHours > 0)
                            {
                                float delta = (float)absoluteSpeed * (float)deltaTime.TotalMinutes;
                                Quaternion rotation;
                                Vector3 nextPosition = ComputeNextPosition(currentOrigin, cargoInstance.gameObject.transform.position, delta, out rotation);
                                //Debug.Log("Stranded Deep AlternativeEndgame Mod : cargo new position = " + nextPosition);

                                cargoInstance.AllRigidbodies[0].transform.position = nextPosition;
                                cargoInstance.gameObject.transform.position = nextPosition;
                                cargoInstance.AllRigidbodies[0].transform.rotation = rotation;
                                cargoInstance.gameObject.transform.rotation = rotation;

                                _lastMovementTick = GameTime.Now;
                            }

                            CheckEndgameConditions();
                            if (_playEndgame < DateTime.Now)
                            {
                                _playEndgame = DateTime.MaxValue;
                                PlayEngameVideo();
                            }
                        }
                    }
                }
                else
                {
                    Reset();
                }
                Event currentevent = Event.current;
                if (currentevent.keyCode == KeyCode.Escape)
                {
                    // exit credits
                    if (isInEndgame && isInEndgameCredits)
                    {
                        EndGameCreditsController ecc = Game.FindObjectOfType<EndGameCreditsController>();
                        if (ecc != null)
                        {
                            MethodInfo mi_endCredits = typeof(EndGameCreditsController).GetMethod("View_Finished", BindingFlags.Instance | BindingFlags.NonPublic);
                            if (mi_endCredits != null)
                            {
                                mi_endCredits.Invoke(ecc, new object[] { });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : error on update : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        private static bool CheckCargoAppearConditionsMet()
        {
            if (GameCalendar.Instance == null)
            {
                return false;
            }

            if (debugModeImmediateCargoAndNoVideo)
            {
                return true;
            }

            FastRandom random = new FastRandom(StrandedWorld.WORLD_SEED);
            return GameCalendar.Instance.DaysElapsed >= random.Next(10, 15);
        }

        private static void AddCargoEffects()
        {
            try
            {
                ParticleSystem ps = cargoInstance.gameObject.AddComponent<ParticleSystem>();
                SmokeParticleSystem pst = cargoInstance.gameObject.AddComponent<SmokeParticleSystem>();
                ps.Play();

#warning TODO : cargo lights
                //GameObject lightGameObject1 = new GameObject("Cargo Bridge Light");
                //lightGameObject1.transform.position = new Vector3(0, 15, 0);
                //lightGameObject1.transform.parent = cargoInstance.transform;
                //Light lightComp1 = lightGameObject1.AddComponent<Light>();
                //lightGameObject1.AddComponent<BridgeSpotlight>();
                //lightGameObject1.SetActive(true);

                //GameObject lightGameObject2 = new GameObject("Cargo Spotlight");
                //lightGameObject2.transform.position = new Vector3(0, 20, -12);
                //lightGameObject2.transform.parent = cargoInstance.transform;
                //Light lightComp2 = lightGameObject2.AddComponent<Light>();
                //lightGameObject2.AddComponent<RotatingSpotlight>();
                //lightGameObject2.SetActive(true);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod error in AddCargoEffects : " + e);
            }
        }

        private static void ComputeLastOriginFromPosition()
        {
            try
            {
                if (cargoInstance == null)
                    return;

                Vector3 loadingPosition = cargoInstance.transform.position;
                Debug.Log("Stranded Deep AlternativeEndgame Mod : loaded cargo position = " + loadingPosition);

                SortedList<float, int> sortedDistancesToWayPoints = new SortedList<float, int>();
                for (int waypointIndex = 0; waypointIndex < positions.Count; waypointIndex++)
                {
                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : waypoint [" + waypointIndex + "] position = " + positions[waypointIndex]);

                    float distance = Vector3.Distance(loadingPosition, positions[waypointIndex]);
                    if (!sortedDistancesToWayPoints.ContainsKey(distance))
                    {
                        //Debug.Log("Stranded Deep AlternativeEndgame Mod : current distance to waypoint [" + waypointIndex + "] = " + distance);
                        sortedDistancesToWayPoints.Add(distance, waypointIndex);
                    }
                    else
                    {
                        Debug.Log("Stranded Deep AlternativeEndgame Mod : sortedDistancesToWayPoints contains distance " + distance + " for position index " + sortedDistancesToWayPoints[distance] + " cannot add for " + waypointIndex);
                    }
                }
                // at this point the 2 first elements are the nearest waypoints, the one with the lowest index is the origin
                currentOrigin = sortedDistancesToWayPoints.First().Value;
                if (sortedDistancesToWayPoints.ElementAt(1).Value < currentOrigin
                    || currentOrigin == 0 && sortedDistancesToWayPoints.ElementAt(1).Value == (positions.Count - 1))
                {
                    currentOrigin = sortedDistancesToWayPoints.ElementAt(1).Value;
                }
                Debug.Log("Stranded Deep AlternativeEndgame Mod : retrieved current origin at index : " + currentOrigin);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : error in ComputeLastOriginFromPosition" + e);
            }
        }

        private static void UpdateTextures()
        {
#warning TODO : better textures
            IImpostorParent impostorParent = cargoInstance.GetInterface<IImpostorParent>();
            //Debug.Log("Stranded Deep AlternativeEndgame Mod : impostor parent type = " + impostorParent.GetType());
            LodController lc = impostorParent as LodController;
            foreach (Lod lod in lc.LodGroup.Lods)
            {
                foreach (Renderer renderer in lod.Renderers)
                {
                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : renderer material name = " + renderer.material.name);
                    string key = "StrandedDeepAlternativeEndgameMod.assets.textures." + renderer.material.name + ".png";
                    if (_indexedTextures.ContainsKey(key))
                    {
                        renderer.material.mainTexture = _indexedTextures[key];
                    }
                    //if (renderer is MeshRenderer)
                    //{
                    foreach (string texname in renderer.material.GetTexturePropertyNames())
                    {
                        //Debug.Log("Stranded Deep AlternativeEndgame Mod : renderer texture name = " + texname);
                        Texture tex = renderer.material.GetTexture(texname);
                        if (tex != null)
                        {
                            //Texture2D magenta = new Texture2D(1, 1);
                            //magenta.SetPixel(0, 0, Color.magenta);
                            //renderer.material.mainTexture = magenta;
                            //Debug.Log("Stranded Deep AlternativeEndgame Mod : renderer main tex name = " + tex.name);
                        }
                        //Stranded Deep AlternativeEndgame Mod : renderer main tex name = tile_wreck_metal1_DIFF
                        //Stranded Deep AlternativeEndgame Mod : renderer main tex name = tile_wreck_metal1_NRM
                        //Stranded Deep AlternativeEndgame Mod : renderer main tex name = tile_paint_detail_DIFF
                        //Stranded Deep AlternativeEndgame Mod : renderer main tex name = tile_wreck_metal1_SPEC
                        //renderer.material.SetTexture("_MainTex", Main._indexedTextures["StrandedDeepAlternativeEndgameMod.assets.textures.smoke.png"]);
                        //renderer.material.SetTexture("_BumpMap", Main._indexedTextures["StrandedDeepAlternativeEndgameMod.assets.textures.smoke.png"]);
                    }
                    //}
                }
            }
        }

        private static void PrepareIncreasedDistanceAndHideImpostor(bool isInitialCreation)
        {
            if (cargoInstance == null)
                return;

            try
            {
                IImpostorParent impostorParent = cargoInstance.GetInterface<IImpostorParent>();
                Debug.Log("Stranded Deep AlternativeEndgame Mod : impostor parent type = " + (impostorParent != null ? impostorParent.GetType().ToString() : "null"));
                if (impostorParent == null)
                    return;
                LodController lc = impostorParent as LodController;
                fi_scope.SetValue(lc, ImposterScope.Manual);
                Debug.Log("Stranded Deep AlternativeEndgame Mod : lod controller scope " + lc.Scope);

                fi_dither.SetValue(lc, false);

                if (!isInitialCreation)
                {
                    //int parentInstanceId = cargoInstance.transform.position.Hash();
                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor hash = " + parentInstanceId);
                    //int parentInstanceId2 = cargoInstance.gameObject.transform.position.Hash();
                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor hash = " + parentInstanceId2);
                    //int parentInstanceId3 = new Vector3(165.5361f, -5, 181.4327f).Hash();
                    //Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor hash = " + parentInstanceId3);
                    ImpostorBase ib = fi_impostor.GetValue(lc) as ImpostorBase;
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor in LodController " + (ib != null ? ib.GetType().Name : "null"));
                    if (ib == null)
                    {
                        Zone localZone = StrandedWorld.GetZone(cargoInstance.transform.position, false);
                        if (localZone != null)
                        {
                            ib = localZone.FindImpostor(cargoInstance.gameObject);
                            Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor in local zone " + (ib != null ? ib.GetType().Name : "null"));
                        }
                        else
                        {
                            Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor : no local zone found");
                        }
                    }
                    if (ib == null)
                    {
                        ib = StrandedWorld.Instance.NmlZone.FindImpostor(cargoInstance.gameObject);
                        Debug.Log("Stranded Deep AlternativeEndgame Mod : lod manager impostor in NML zone " + (ib != null ? ib.GetType().Name : "null"));
                    }
                    if (ib != null)
                    {
                        ib.enabled = false;
                        ImpostorManager.Release(ib);
                    }
                }

                // "working" but maybe useless
                //for (int lodIndex = lc.LodGroup.Lods.Count - 1; lodIndex > 0; lodIndex--)
                //{
                //    lc.LodGroup.Lods.RemoveAt(lodIndex);
                //}

                // possibly useless
                int cull = 5000;
                fi_localImpostorCullingDistance.SetValue(lc, cull);
                foreach (Lod lod in lc.LodGroup.Lods)
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : culling distance for cargo lod (1) before " + lod.CullingDistance);
                    lod.CullingDistance = cull;
                    cull += 500;
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : culling distance for cargo lod (1) after " + lod.CullingDistance);
                }

                cargoInstance.gameObject.isStatic = false;
                // working
                cargoInstance.gameObject.SetLayerRecursively(Layers.WATER);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod lod error in PrepareIncreasedDistanceAndHideImpostor : " + e);
            }
        }

        private static Vector3 ComputeNextPosition(int currentOriginParam, Vector3 currentPosition, float delta, out Quaternion rotation)
        {
            Vector3 currentPositionLocal = currentPosition;
            //Debug.Log("Stranded Deep AlternativeEndgame Mod currentOrigin : " + currentOriginParam);
            int destinationIndex = (currentOriginParam == positions.Count - 1 ? 0 : currentOriginParam + 1);
            Vector3 destination = positions[destinationIndex];
            //Debug.Log("Stranded Deep AlternativeEndgame Mod destination : " + destination);
            if (Vector3.Distance(currentPositionLocal, destination) <= 5.0f)
            {
                //Debug.Log("Stranded Deep AlternativeEndgame Mod destination approx same as currentPosition");
                //Debug.Log("Stranded Deep AlternativeEndgame Mod new destination index : "+ GameTime.Now +" / index = " + destinationIndex + "/" + positions.Count);
                currentOrigin = destinationIndex;
                destinationIndex = (currentOrigin == positions.Count - 1 ? 0 : currentOrigin + 1);
                currentPositionLocal = destination;
                destination = positions[destinationIndex];
            }
            //Debug.Log("Stranded Deep AlternativeEndgame Mod destination : " + destination);
            Vector3 origin = positions[currentOrigin];

            Vector3 direction = Vector3.Normalize(new Vector3(destination.x - origin.x, 0, destination.z - origin.z));
            Vector3 result = new Vector3(direction.x * delta, 0, direction.z * delta);

            rotation = Quaternion.LookRotation(direction);

            return currentPositionLocal + result;
        }

        private static void Reset()
        {
            currentOrigin = 0;
            _lastMovementTick = DateTime.MinValue;
            _playEndgame = DateTime.MaxValue;
            cargoInstance = null;
            audioSource = null;
            if (previousFlareGun != null)
            {
                previousFlareGun.Used.RemoveListener(ShootFlareCallback);
            }
            previousFlareGun = null;
            isInEndgame = false;
            isInEndgameCredits = false;
        }

        private static void ShootFlareCallback(IBase arg0, IBaseActionEventData arg1)
        {
            Debug.Log("Stranded Deep AlternativeEndgame Mod : flare gun shot check cargo distance");
            if (cargoInstance == null)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : flare gun shot cargo not found");
                return;
            }

            float distance = Vector3.Distance(previousFlareGun.transform.position, cargoInstance.transform.position);
            Debug.Log("Stranded Deep AlternativeEndgame Mod : flare gun shot distance = " + distance);
            if (distance <= Parameters.CARGOCHECKDISTANCE)
            {
                if (!debugModeImmediateCargoAndNoVideo)
                {
                    isInEndgame = true;
                    _playEndgame = DateTime.Now.AddSeconds(secondsBeforeEndgameVideo);
                }
                ShowSubtitles(PlayerRegistry.AllPlayers[0], "They have seen me ! I'm getting out of this hell !", secondsBeforeEndgameVideo * 1000);

                ShootFlareBack();
                Task.Delay(2000).ContinueWith(t => ShootFlareBack());
                Task.Delay(4000).ContinueWith(t => ShootFlareBack());
                Task.Delay(6000).ContinueWith(t => ShootFlareBack());
                Task.Delay(8000).ContinueWith(t => ShootFlareBack());
            }
        }

        private static void CheckEndgameConditions()
        {
            foreach (IPlayer player in PlayerRegistry.AllPlayers)
            {
                if (player.Holder.CurrentObject is InteractiveObject_FLAREGUN)
                {
                    InteractiveObject_FLAREGUN flaregun = player.Holder.CurrentObject as InteractiveObject_FLAREGUN;
                     if (!System.Object.ReferenceEquals(previousFlareGun, flaregun))
                    {
                        if (previousFlareGun != null)
                        {
                            Debug.Log("Stranded Deep AlternativeEndgame Mod : changed flare gun");
                            previousFlareGun.Used.RemoveListener(ShootFlareCallback);
                        }
                    }
                    else
                    {
                        continue;
                    }
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : registering flare gun shot event");
                    flaregun.Used.AddListener(ShootFlareCallback);
                    previousFlareGun = flaregun;
                }
            }
        }
        
        private static void PlayEngameVideo()
        {
            try
            {
                if (Camera.main == null)
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : PlayEngameVideo no main camera found");
                    return;
                }

                if (showEndgameCreditsAndEndGame)
                {
                    EventManager.RaiseEvent<EndGameSequenceEvent>(new EndGameSequenceEvent());
                    EventManager.RaiseEvent<FreezeNotificationsEvent>(new FreezeNotificationsEvent(true));

                    foreach (IPlayer player in PlayerRegistry.AllPlayers)
                    {
                        try
                        {
                            // underwater bug
                            player.transform.position = new Vector3(player.transform.position.x, 1, player.transform.position.z);
                            // spyglass bug
                            player.Character.HideCurrentItem();
                            if (PlayerRegistry.LocalPlayer != null
                                && PlayerRegistry.LocalPlayer.Holder != null
                                && PlayerRegistry.LocalPlayer.Holder.CurrentObject != null
                                && PlayerRegistry.LocalPlayer.Holder.CurrentObject is InteractiveObject_SPYGLASS)
                            {
                                InteractiveObject_SPYGLASS spyglass = PlayerRegistry.LocalPlayer.Holder.CurrentObject as InteractiveObject_SPYGLASS;
                                spyglass.Hold(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Stranded Deep AlternativeEndgame Mod : could not fix bugs " + ex);
                        }

                        try
                        {
                            if (player.IsOwner)
                            {
                                player.Input.SetAndCacheMapsEnabled(false, 0);
                                player.Input.SetAndCacheMapsEnabled(true, 10);
                            }
                            FreezePlayer(player);
                            player.PlayerCamera.CameraMovement.SetRotation(player.Character.CharacterFirstPerson.CameraRigPosition);
                            player.PlayerCamera.CameraMovement.SetCameraTarget(player.Character.CharacterFirstPerson.CameraRigPosition);
                            player.PlayerCamera.MouseLook.enabled = false;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Stranded Deep AlternativeEndgame Mod : could not freeze player for game ending " + ex);
                        }
                    }

                    Singleton<GameTime>.Instance.MilitaryTime_NoEvents = 16f;
                    Singleton<GameTime>.Instance.Paused = true;
                    Timing.WaitForSeconds(0.5f);


                    // Will attach a VideoPlayer to the main camera.
                    //GameObject camera = GameObject.Find("Main Camera");
                    GameObject camera = Camera.main.gameObject;

                    // VideoPlayer automatically targets the camera backplane when it is added
                    // to a camera object, no need to change videoPlayer.targetCamera.
                    VideoPlayer videoPlayer = camera.GetComponent<VideoPlayer>();
                    if (videoPlayer == null)
                    {
                        videoPlayer = camera.AddComponent<VideoPlayer>();
                    }
                    videoPlayer.loopPointReached -= EndReached;

                    // Play on awake defaults to true. Set it to false to avoid the url set
                    // below to auto-start playback since we're in Start().
                    videoPlayer.playOnAwake = false;

                    // By default, VideoPlayers added to a camera will use the far plane.
                    // Let's target the near plane instead.
                    videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

                    videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;

                    videoPlayer.playbackSpeed = 0.8f;

                    // This will cause our Scene to be visible through the video being played.
                    videoPlayer.targetCameraAlpha = 1.0f;// 0.5F;

                    // Set the video to play. URL supports local absolute or relative paths.
                    // Here, using absolute.
                    videoPlayer.url = @"file://" + videoFileName;
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : PlayEngameVideo video url = " + videoPlayer.url);

                    // Skip the first 100 frames.
                    //videoPlayer.frame = 100;

                    // Restart from beginning when done.
                    videoPlayer.isLooping = false;

                    // Each time we reach the end, we slow down the playback by a factor of 10.
                    videoPlayer.loopPointReached += EndReached;

                    // Start playback. This means the VideoPlayer may have to prepare (reserve
                    // resources, pre-load a few frames, etc.). To better control the delays
                    // associated with this preparation one can use videoPlayer.Prepare() along with
                    // its prepareCompleted event.
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : PlayEngameVideo start play");
                    videoPlayer.Play();
                    AudioManager.GetAudioPlayer().Play2D(cargoEngineSound, AudioMixerChannel.FX, AudioPlayMode.Single);
                    AudioManager.GetAudioPlayer().Play2D(endingMusic, AudioMixerChannel.FX, AudioPlayMode.Single);
                    Task.Delay(6500).ContinueWith(t => AudioManager.GetAudioPlayer().Play2D(reporterSound, AudioMixerChannel.FX, AudioPlayMode.Single));
                    Task.Delay(30000).ContinueWith(t => AudioManager.GetAudioPlayer().Play2D(crowdSound, AudioMixerChannel.FX, AudioPlayMode.Single));
                }
                ShowSubtitles(PlayerRegistry.AllPlayers[0], "I'm here ! I'm here !", 10000);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : error in PlayEngameVideo : " + e);
            }
        }

        private static void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            Debug.Log("Stranded Deep AlternativeEndgame Mod : PlayEngameVideo video ended");

            vp.loopPointReached -= EndReached;
            // show credits
            if (showEndgameCreditsAndEndGame)
            {
                EndGameCreditsController ecc = Game.FindObjectOfType<EndGameCreditsController>();
                if (ecc != null)
                {
                    ecc.Show();
                    // we got our own music
                    ecc.StopTrack(TrackType.MainMenu);
                    isInEndgameCredits = true;
                }
            }
            else
            {
                GameObject camera = Camera.main.gameObject;
                VideoPlayer player = camera.GetComponent<VideoPlayer>();
                player.Stop();
                player.enabled = false;
                isInEndgame = false;
                isInEndgameCredits = false;
            }
        }

        private static void ShowEndgameVideoCanvas()
        {
            if (modCanvas == null)
            {
                modCanvas = createCanvas(false, "AlternativeEndgameCanvas");
            }
            else
            {
                modCanvas.SetActive(alternativeEndingCanvasVisible);
            }
        }
        
        private static void ShootFlareBack()
        {
            Debug.Log("Stranded Deep AlternativeEndgame Mod : try shoot flare back");
            cargoInstance.StartCoroutine(ShootFlareCoroutine());
        }

        private static IEnumerator ShootFlareCoroutine()
        {
            if (previousFlareGun == null)
                yield return new WaitForSeconds(0f);

            Debug.Log("Stranded Deep AlternativeEndgame Mod : ShootFlareCoroutine");

            GameObject flareProjectile = fi_flareProjectile.GetValue(previousFlareGun) as GameObject;
            Debug.Log("Stranded Deep AlternativeEndgame Mod : ShootFlareCoroutine flare projectile found");
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(flareProjectile, cargoInstance.transform.position + new Vector3(0, 0, -15.0f), Quaternion.identity);
            Debug.Log("Stranded Deep AlternativeEndgame Mod : ShootFlareCoroutine flare projectile type = " + gameObject.GetType());

            gameObject.transform.position = cargoInstance.transform.position;
            gameObject.transform.rotation = Quaternion.LookRotation(cargoInstance.transform.up);
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 50f;
            yield return new WaitForSeconds(1f);
            yield break;
        }
    }
}
