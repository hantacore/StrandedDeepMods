using Beam;
using Beam.UI.Crafting;
using HarmonyLib;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepWetAndColdMod
{
    /// <summary>
    /// Check for shelter DONE
    /// 
    /// Add player effects : wet, sick, fever DONE
    /// 
    /// if sick cough every sometimes DONE
    /// 
    /// find if visual effect possible DONE
    /// 
    /// find if energy possible
    /// 
    /// knock out if fever and not rest
    /// 
    /// reset on game load
    /// 
    /// check if savegame
    /// 
    /// add tracking of effects in savegame DONE
    /// 
    /// fever starts too early
    /// </summary>
    public static partial class Main
    {
        public const string MODNAME = "Wet and Cold";

        private static Harmony harmony;

        private static bool showDebugText = false;

        private static string configFileName = "StrandedDeepWetAndColdMod.config";

        private static float wetAndColdCanvasDefaultScreenWidth = 1024f;
        private static float wetAndColdCanvasDefaultScreenHeight = 768f;
        private static float screenRatioConversion = 1f;

        private static bool coldCanvasVisible = false;
        private static GameObject modCanvas;
        private static Text debugText;

        public static Image wetMeterImage;
        public static Image coldMeterImage;
        public static Image hotMeterImage;
        public static Dictionary<int, Wet> wetEffect;
        public static Dictionary<int, BodyTemperature> bodytemperatureEffect;
        public static Dictionary<int, Sick> sickEffect;
        public static Dictionary<int, Fever> feverEffect;
        public static Dictionary<int, Cold> coldEffect;
        public static Dictionary<int, Hot> hotEffect;

        private static FieldInfo playerStatisticsSleepField;
        private static FieldInfo playerStatisticsWaterField;
        private static FieldInfo playerStatisticsCaloriesField;

        private static Image energyMeterImage;
        private static Image simpleShelterImage;
        private static Image housingShelterImage;
        private static Image heatingImage;
        private static Image activityImage;
        private static Image sweatingImage;
        private static Image caloriesImage;
        private static Image feverOverlayImage;

        internal static DateTime lastCheck = DateTime.MinValue;

        private static bool simpleSheltered = false;
        private static bool housingSheltered = false;
        private static bool sweating = false;
        private static bool burningCalories = false;
        private static bool isHeating = false;

        private static System.Random r = new System.Random();

        private static List<uint> firePrefabs;
        private static List<uint> floorPrefabs;
        private static List<uint> wallPrefabs;
        private static List<uint> roofPrefabs;

        internal static string COLD_MESSAGE = "I'm freezing, I need to warm up.";
        internal static string HOT_MESSAGE = "I'm way too hot, I need to cool down.";
        internal static string SICK_MESSAGE = "A running nose and a sneeze... Crap, I'm sick.";
        internal static string FEVER_MESSAGE = "This fever is killing me, I need to rest. I'm too tired to do anything.";
        internal static string END_FEVER_MESSAGE = "I'm starting to feel better.";

        internal static bool hardFeverEffect = true;
        internal static bool showWnCGUI = true;

        // PlayerEffect
        // AtmosphereTemperature

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        private static bool perfCheck = false;

        static string _infojsonlocation = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedDeepWetAndColdMod/StrandedDeepColdMod/Info.json";

        internal static bool fixRainReset = true;

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

                VersionChecker.CheckVersion(modEntry, _infojsonlocation);

                // check if Tweaks loaded (maybe test fix enabled later)
                //UnityModManager.ModEntry tweaksMod = UnityModManager.FindMod("StrandedDeepTweaksMod");
                //if (tweaksMod != null && tweaksMod.Active && tweaksMod.Loaded)
                //{
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : rain fix hardcoded and enabled");
                    //ParameterValues.RAIN_VALUE_CORRECTION = 0;
                //}

                playerStatisticsSleepField = typeof(Statistics).GetField("_sleep", BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerStatisticsSleepField == null)
                    return false;
                playerStatisticsCaloriesField = typeof(Statistics).GetField("_calories", BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerStatisticsCaloriesField == null)
                    return false;
                playerStatisticsWaterField = typeof(Statistics).GetField("_fluids", BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerStatisticsWaterField == null)
                    return false;

                InitEffects();

                LoadAudioEffects();

                float defaultScreenRatio = wetAndColdCanvasDefaultScreenWidth / wetAndColdCanvasDefaultScreenHeight;
                float currentScreenRatio = (float)Screen.width / (float)Screen.height;
                screenRatioConversion = currentScreenRatio / defaultScreenRatio;

                firePrefabs = new List<uint>();
                firePrefabs.Add(168); //NEW_CAMPFIRE
                firePrefabs.Add(169); //NEW_CAMPFIRE_PIT
                firePrefabs.Add(170); //NEW_CAMPFIRE_SPIT
                firePrefabs.Add(171); //HOBO_STOVE
                firePrefabs.Add(172); //SMOKER
                firePrefabs.Add(208); //FIRE_TORCH
                firePrefabs.Add(229); //FURNACE
                firePrefabs.Add(290); //FUEL_STILL

                floorPrefabs = new List<uint>();
                floorPrefabs.Add(76);//CORRUGATED_FOUNDATION;
                floorPrefabs.Add(78);//DRIFTWOOD_FOUNDATION;
                floorPrefabs.Add(81);//WOOD_FOUNDATION;
                floorPrefabs.Add(87);//PLANK_FOUNDATION;
                floorPrefabs.Add(102);//CORRUGATED_WEDGE_FOUNDATION
                floorPrefabs.Add(105);//DRIFTWOOD_WEDGE_FOUNDATION
                floorPrefabs.Add(108);//PLANK_WEDGE_FOUNDATION
                floorPrefabs.Add(110);//WOOD_WEDGE_FOUNDATION
                floorPrefabs.Add(233);//BRICK_FOUNDATION
                floorPrefabs.Add(238);//BRICK_WEDGE_FOUNDATION
                floorPrefabs.Add(94);//SHIPPING_CONTAINER_1
                floorPrefabs.Add(95);//SHIPPING_CONTAINER_2
                floorPrefabs.Add(96);//SHIPPING_CONTAINER_3

                wallPrefabs = new List<uint>();
                wallPrefabs.Add(77);//CORRUGATED_PANEL_STATIC
                wallPrefabs.Add(80);//DRIFTWOOD_PANEL_STATIC
                wallPrefabs.Add(83);//WOOD_PANEL_STATIC
                wallPrefabs.Add(86);//TARP_PANEL_STATIC
                wallPrefabs.Add(89);//PLANK_PANEL_STATIC
                wallPrefabs.Add(93);//SHIPPING_CONTAINER_PANEL_STATIC
                wallPrefabs.Add(112);//DRIFTWOOD_ARCH
                wallPrefabs.Add(113);//DRIFTWOOD_DOOR
                wallPrefabs.Add(114);//WOOD_ARCH
                wallPrefabs.Add(115);//WOOD_DOOR
                wallPrefabs.Add(116);//PLANK_ARCH
                wallPrefabs.Add(117);//PLANK_DOOR
                wallPrefabs.Add(118);//CORRUGATED_ARCH
                wallPrefabs.Add(119);//CORRUGATED_DOOR
                wallPrefabs.Add(120);//DRIFTWOOD_WALL_HALF
                wallPrefabs.Add(121);//WOOD_WALL_HALF
                wallPrefabs.Add(122);//PLANK_WALL_HALF
                wallPrefabs.Add(123);//CORRUGATED_WALL_HALF
                wallPrefabs.Add(124);//DRIFTWOOD_WALL_WINDOW
                wallPrefabs.Add(125);//WOOD_WALL_WINDOW
                wallPrefabs.Add(126);//PLANK_WALL_WINDOW
                wallPrefabs.Add(127);//CORRUGATED_WALL_WINDOW
                wallPrefabs.Add(231);//BRICK_ARCH
                wallPrefabs.Add(234);//BRICK_PANEL_STATIC
                wallPrefabs.Add(235);//BRICK_WALL_HALF
                wallPrefabs.Add(236);//BRICK_WALL_WINDOW

                roofPrefabs = new List<uint>();
                roofPrefabs.Add(75);//CORRUGATED_FLOOR
                roofPrefabs.Add(79);//DRIFTWOOD_FLOOR
                roofPrefabs.Add(82);//WOOD_FLOOR
                roofPrefabs.Add(88);//PLANK_FLOOR
                roofPrefabs.Add(101);//CORRUGATED_WEDGE_FLOOR
                roofPrefabs.Add(104);//DRIFTWOOD_WEDGE_FLOOR
                roofPrefabs.Add(107);//PLANK_WEDGE_FLOOR
                roofPrefabs.Add(109);//WOOD_WEDGE_FLOOR
                roofPrefabs.Add(232);//BRICK_FLOOR
                roofPrefabs.Add(237);//BRICK_WEDGE_FLOOR
                roofPrefabs.Add(239);//WOOD_ROOF_CAP
                roofPrefabs.Add(240);//WOOD_ROOF_CORNER
                roofPrefabs.Add(241);//WOOD_ROOF_MIDDLE
                roofPrefabs.Add(242);//WOOD_ROOF_WEDGE
                roofPrefabs.Add(243);//BRICK_ROOF_CAP
                roofPrefabs.Add(244);//BRICK_ROOF_CORNER
                roofPrefabs.Add(245);//BRICK_ROOF_MIDDLE
                roofPrefabs.Add(246);//BRICK_ROOF_WEDGE

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("Stranded Deep " + MODNAME + " Mod properly loaded");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep "+MODNAME+" Mod error on load : " + e);
            }
            finally
            {
                Debug.Log("Stranded Deep "+MODNAME+" Mod load time (ms) = " + chrono.ElapsedMilliseconds);
            }

            return false;
        }

        private static void InitEffects()
        {
            Debug.Log("Stranded Deep " + MODNAME + " initialize effects");
            wetEffect = new Dictionary<int, Wet>();
            bodytemperatureEffect = new Dictionary<int, BodyTemperature>();
            sickEffect = new Dictionary<int, Sick>();
            feverEffect = new Dictionary<int, Fever>();
            coldEffect = new Dictionary<int, Cold>();
            hotEffect = new Dictionary<int, Hot>();
            for (int i = 0; i < WorldUtilities.MaxPlayers; i++)
            {
                wetEffect.Add(i, new Wet());
                bodytemperatureEffect.Add(i, new BodyTemperature());
                sickEffect.Add(i, new Sick());
                feverEffect.Add(i, new Fever());
                coldEffect.Add(i, new Cold());
                hotEffect.Add(i, new Hot());
            }
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("<b>Wet and Cold mod by Hantacore</b>");
            GUILayout.Label("This mod adds a hot / wet / cold / sick mechanism to the game");
            GUILayout.Label("It restores the atmosphere temperature mechanism, based on monthly data");
            GUILayout.Label("Storms are colder, and bring rain");
            GUILayout.Label("When you touch water, or get soaked by rain you get wet");
            GUILayout.Label("Shelters protect from the rain, so you won't get wet");
            GUILayout.Label("You can dry under the sun, or near a heat source");
            GUILayout.Label("Wetness increases the cold feeling");
            GUILayout.Label("Staying awake for too long drains energy");
            GUILayout.Label("Sleeping, or resting inside a house restores energy");
            GUILayout.Label("If you stay cold and tired for too long, you get sick");
            GUILayout.Label("If you stay sick and cold for too long, you get fever");
            GUILayout.Label("You can heal fever and cold by keeping warm, and you'll heal faster inside a house");
            GUILayout.Label("");
            GUILayout.Label("Cold and fever effects are in Work in Progress for balancing");
            GUILayout.Label("");
            showWnCGUI = GUILayout.Toggle(showWnCGUI, "Show Wet and Cold GUI icons");
            hardFeverEffect = GUILayout.Toggle(hardFeverEffect, "Fever hard mode (blocks crafting)");
            GUILayout.Label("");
            GUILayout.Label("Balancing factor (Experimental, the higher = the slower) = " + ParameterValues.BALANCING_FACTOR);
            ParameterValues.BALANCING_FACTOR = GUILayout.HorizontalSlider(ParameterValues.BALANCING_FACTOR, 1, 4);
            if (GUILayout.Button("Reset Balancing factor"))
            {
                ParameterValues.BALANCING_FACTOR = 2.0f;
            }
            GUILayout.Label("");
            GUILayout.Label("<b><color=orange>Info values (all readonly)</color></b>");

            GUILayout.Label("Maximum wetness value");
            GUILayout.TextField(ParameterValues.MAX_WETNESS.ToString());
            GUILayout.Label("Maximum temperature value");
            GUILayout.TextField(ParameterValues.MAX_TEMP.ToString());
            GUILayout.Label("Hot threshold");
            GUILayout.TextField(ParameterValues.HOT_THRESHOLD.ToString());
            GUILayout.Label("Sweat threshold");
            GUILayout.TextField(ParameterValues.SWEAT_THRESHOLD.ToString());
            GUILayout.Label("Cold threshold");
            GUILayout.TextField(ParameterValues.COLD_THRESHOLD.ToString());
            GUILayout.Label("Sweat threshold");
            GUILayout.TextField(ParameterValues.SWEAT_THRESHOLD.ToString());

            GUILayout.Label("Get cold ingame delay");
            GUILayout.TextField(ParameterValues.SICK_INGAME_DELAY.ToString());
            GUILayout.Label("Heal from cold ingame delay");
            GUILayout.TextField(ParameterValues.HEAL_INGAME_DELAY_MINUTES.ToString());
            GUILayout.Label("Get fever ingame delay");
            GUILayout.TextField(ParameterValues.FEVER_INGAME_DELAY.ToString());
            GUILayout.Label("Heal fever ingame delay");
            GUILayout.TextField(ParameterValues.FEVER_HEAL_INGAME_DELAY_MINUTES.ToString());

            GUILayout.Label("Underwater wetness modificator");
            GUILayout.TextField(ParameterValues.UNDERWATER_WETNESS_MODIFICATOR_PER_MINUTE.ToString());
            GUILayout.Label("Touch water wetness modificator");
            GUILayout.TextField(ParameterValues.WATER_TOUCH_WETNESS_MODIFICATOR_PER_MINUTE.ToString());
            GUILayout.Label("Unsheletered rain wetness modificator");
            GUILayout.TextField(ParameterValues.UNSHELTERED_RAIN_WETNESS_MODIFICATOR_PER_MINUTE.ToString());
            GUILayout.Label("Daylight sun wetness modificator");
            GUILayout.TextField(ParameterValues.DAYLIGHT_SUN_WETNESS_MODIFICATOR_PER_MINUTE.ToString());
            GUILayout.Label("Night wetness modificator");
            GUILayout.TextField(ParameterValues.NIGHT_WETNESS_MODIFICATOR_PER_MINUTE.ToString());
            GUILayout.Label("Default day wetness modificator");
            GUILayout.TextField(ParameterValues.DAY_WETNESS_MODIFICATOR_PER_MINUTE.ToString());
            GUILayout.Label("Heatsource wetness modificator");
            GUILayout.TextField(ParameterValues.HEATSOURCE_WETNESS_MODIFICATOR_PER_MINUTE.ToString());

            showDebugText = GUILayout.Toggle(showDebugText, "Show debug values");

            //Camera.current.focalLength = GUILayout.HorizontalSlider(Camera.current.focalLength, 0f, 150f);

            if ((Game.State == GameState.NEW_GAME || Game.State == GameState.LOAD_GAME) && showDebugText)
            {
                if (GUILayout.Button("Give sick"))
                {
                    if (!PlayerRegistry.LocalPlayer.Statistics.HasStatusEffect(sickEffect[0]))
                    {
                        Debug.Log("Stranded Deep "+MODNAME+" Mod : debug add sick");
                        sickEffect[0].Reset();
                        PlayerRegistry.LocalPlayer.Statistics.ApplyStatusEffect(sickEffect[0]);
                        Main.ShowSubtitles(PlayerRegistry.LocalPlayer, Main.SICK_MESSAGE);
                        nextCough = DateTime.Now;
                    }
                }

                if (GUILayout.Button("Clear sick"))
                {
                    if (PlayerRegistry.LocalPlayer.Statistics.HasStatusEffect(sickEffect[0]))
                    {
                        Debug.Log("Stranded Deep "+MODNAME+" Mod : debug remove sick");
                        nextGripe = DateTime.MaxValue;
                        nextCough = DateTime.MaxValue;
                        PlayerRegistry.LocalPlayer.Statistics.RemoveStatusEffect(sickEffect[0]);
                        PlayRelief(PlayerRegistry.LocalPlayer);
                    }
                }

                if (GUILayout.Button("Give fever"))
                {
                    if (!PlayerRegistry.LocalPlayer.Statistics.HasStatusEffect(feverEffect[0]))
                    {
                        Debug.Log("Stranded Deep "+MODNAME+" Mod : debug add fever");
                        feverEffect[0].Reset();
                        PlayerRegistry.LocalPlayer.Statistics.ApplyStatusEffect(feverEffect[0]);
                        Main.ShowSubtitles(PlayerRegistry.LocalPlayer, Main.FEVER_MESSAGE);
                        bodytemperatureEffect[0].HasHadFever = true;
                    }
                }

                if (GUILayout.Button("Clear fever"))
                {
                    if (PlayerRegistry.LocalPlayer.Statistics.HasStatusEffect(feverEffect[0]))
                    {
                        Debug.Log("Stranded Deep "+MODNAME+" Mod : debug remove fever");
                        nextGripe = DateTime.MaxValue;
                        PlayerRegistry.LocalPlayer.Statistics.RemoveStatusEffect(feverEffect[0]);
                        PlayRelief(PlayerRegistry.LocalPlayer);
                    }
                }

                if (GUILayout.Button("Start Storm") && AtmosphereStorm.Instance != null)
                {
                    Debug.Log("Stranded Deep "+MODNAME+" Mod : Try start storm");
                    MethodInfo mi_CreateWeatherEvent = typeof(AtmosphereStorm).GetMethod("CreateWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    //CreateWeatherEvent(float eventRatio, float startTimeRatio)
                    // 1.0.38 compatibility
                    //mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { 100f, 20f });
                    mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { });
                    AtmosphereStorm.Instance.StartWeatherEvent();
                }
                //if (GUILayout.Button("Start Rain") && AtmosphereStorm.Instance != null)
                //{
                //    Debug.Log("Stranded Deep "+MODNAME+" Mod : Try start rain");
                //    MethodInfo mi_CreateWeatherEvent = typeof(AtmosphereStorm).GetMethod("CreateWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                //    //CreateWeatherEvent(float eventRatio, float startTimeRatio)
                //    mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { 50f, 20f });
                //    AtmosphereStorm.Instance.StartWeatherEvent();
                //}
                //if (GUILayout.Button("Start Light Rain") && AtmosphereStorm.Instance != null)
                //{
                //    Debug.Log("Stranded Deep "+MODNAME+" Mod : Try start light rain");
                //    MethodInfo mi_CreateWeatherEvent = typeof(AtmosphereStorm).GetMethod("CreateWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                //    //CreateWeatherEvent(float eventRatio, float startTimeRatio)
                //    mi_CreateWeatherEvent.Invoke(AtmosphereStorm.Instance, new object[] { 25f, 20f });
                //    AtmosphereStorm.Instance.StartWeatherEvent();
                //}
                if (GUILayout.Button("End current event") && AtmosphereStorm.Instance != null)
                {
                    Debug.Log("Stranded Deep "+MODNAME+" Mod : Try end current event");
                    foreach(WeatherEvent we in AtmosphereStorm.Instance.WeatherEvents)
                    {
                        we.ResetEvent();
                        we.StopEvent();
                    }
                }
            }
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        //private static bool mustReloadEffectsFromSave = true;
        private static bool mustReinitImages = true;
        private static StringBuilder temperatureStringBuilder = new StringBuilder();
        private static StringBuilder wetnessStringBuilder = new StringBuilder();

        private static FieldInfo fi_rain = typeof(AtmosphereStorm).GetField("_rain", BindingFlags.Instance | BindingFlags.NonPublic);

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                if (Beam.Game.State != GameState.NEW_GAME
                    && Beam.Game.State != GameState.LOAD_GAME)
                {
                    //Debug.Log("Stranded Deep "+MODNAME+" Mod : context changed, resetting");
                    Reset();
                    return;
                }

                coldCanvasVisible = showWnCGUI && (Beam.Game.State == GameState.NEW_GAME
                    || Beam.Game.State == GameState.LOAD_GAME) && WorldUtilities.IsWorldLoaded();

                RefreshColdTextCanvas();

                if (Beam.Game.State == GameState.NEW_GAME
                    || Beam.Game.State == GameState.LOAD_GAME)
                {
                    if (!WorldUtilities.IsWorldLoaded())
                    {
                        return;
                    }

                    if (GameCalendar.Instance == null || GameTime.Instance == null || PlayerRegistry.AllPlayers == null || PlayerRegistry.AllPlayers.Count == 0)
                    {
                        //Debug.Log("Stranded Deep "+MODNAME+" Mod : null calendar or gametime");
                        return;
                    }

                    //if (fixRainReset)
                    //{
                    //    if (AtmosphereStorm.Instance != null
                    //        && (AtmosphereStorm.Instance.CurrentWeatherEvent == null
                    //            || !Mathf.Approximately(AtmosphereStorm.Instance.CurrentWeatherEvent.Humidity, 100f))
                    //        && AtmosphereStorm.Instance.Rain > 0)
                    //    {
                    //        Debug.Log("Stranded Deep " + MODNAME + " Mod : resetting rain");
                    //        fi_rain.SetValue(AtmosphereStorm.Instance, 0);
                    //    }
                    //}

                    for (int currentPlayerIndex = 0; currentPlayerIndex < PlayerRegistry.AllPlayers.Count; currentPlayerIndex++)
                    {
                        Player p = PlayerRegistry.AllPlayers[currentPlayerIndex] as Player;
                        try
                        {
                            //if (p.Movement.GodMode || p.Statistics.Invincible)
                            //{
                            //    debugText.text = "GodMode true";
                            //    lastCheck = GameTime.Now;
                            //    continue;
                            //}

                            if (mustReinitImages)
                            {
                                #region weird exception fix test

                                //InitEffects();

                                #endregion

                                mustReinitImages = false;
                            }

                            if (DateTime.Now.Second % ParameterValues.SHELTER_CHECK_INTERVAL_SECONDS == 0)
                            {
                                //Debug.Log("Stranded Deep "+MODNAME+" Mod : checking sheltered");
                                IsSheltered(p, out simpleSheltered, out housingSheltered);
                                isHeating = IsNearLitFire(p);
                                if (simpleShelterImage != null)
                                    simpleShelterImage.enabled = simpleSheltered;
                                if (housingShelterImage != null)
                                    housingShelterImage.enabled = housingSheltered;
                                if (heatingImage != null)
                                    heatingImage.enabled = isHeating;
                            }

                            UpdateAtmosphereTemperature(p);
                            if (lastCheck == DateTime.MinValue)
                                lastCheck = GameTime.Now;

                            if (false)
                            {
                                DateTime nextCheck = lastCheck.AddMinutes(ParameterValues.STATUS_CHECK_INTERVAL_GAMETIME_MINUTES);
                                bool shouldUpdate = (DateTime.Compare(GameTime.Now, nextCheck) >= 0);
                                //Debug.Log("Stranded Deep "+MODNAME+" Mod : shouldUpdate : " + shouldUpdate + " / GameTime.now " + GameTime.Now + " / nextcheck " + nextCheck + " / interval game minutes " + ParameterValues.STATUS_CHECK_INTERVAL_GAMETIME_MINUTES);
                                if (shouldUpdate && p.Movement != null)
                                {

                                    TimeSpan elapsed = GameTime.Now.Subtract(lastCheck);
                                    int elapsedMinutes = (int)elapsed.TotalMinutes;
                                    //Debug.Log("Stranded Deep "+MODNAME+" Mod : elapsed minutes = " + elapsedMinutes);

                                    //Debug.Log("Stranded Deep "+MODNAME+" Mod : updating stats : " + GameTime.Now);
                                    if (elapsedMinutes <= 2)
                                    {
                                        WeatherEvent we = null;
                                        //if (we != null)
                                        //{
                                        //    Debug.Log("Stranded Deep "+MODNAME+" Mod : atmosphere storm percentage : " + AtmosphereStorm.Instance.Percentage);
                                        //}
                                        //Debug.Log("Stranded Deep "+MODNAME+" Mod : atmosphere rain : " + AtmosphereStorm.Instance.Rain);
                                        if (AtmosphereStorm.Instance.Rain > 0)
                                        {
                                            //Debug.Log("Stranded Deep "+MODNAME+" Mod : atmosphere rain : " + AtmosphereStorm.Instance.Rain);
                                            //FieldInfo fi_currentWeatherEvent = typeof(AtmosphereStorm).GetField("_currentWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                                            //we = fi_currentWeatherEvent.GetValue(AtmosphereStorm.Instance) as WeatherEvent;
                                            we = AtmosphereStorm.Instance.CurrentWeatherEvent;
                                            //if (we != null)
                                            //{
                                            //    Debug.Log("Stranded Deep "+MODNAME+" Mod : weather event percentage : " + we.Percentage);
                                            //    Debug.Log("Stranded Deep "+MODNAME+" Mod : weather event humidity : " + we.Humidity);
                                            //}
                                        }
                                        UpdateWetness(p, currentPlayerIndex, elapsedMinutes, AtmosphereStorm.Instance.Rain, we != null ? we.Percentage : 0.0f);//(AtmosphereStorm.Instance.Rain == 1));
                                        UpdateBodyTemperature(p, currentPlayerIndex, elapsedMinutes, false);
                                        UpdateEnergy(p, currentPlayerIndex, elapsedMinutes, false);
                                        UpdateSick(p, currentPlayerIndex, elapsedMinutes, false);
                                        UpdateFever(p, currentPlayerIndex, elapsedMinutes);
                                    }
                                    else
                                    {
                                        // have to simulate night or sleep
                                        Debug.Log("Stranded Deep " + MODNAME + " Mod : simulating temperatures");
                                        DateTime currentSimulatedTime = lastCheck;
                                        int simulationIntervalInGameMinutes = 15;
                                        while (DateTime.Compare(currentSimulatedTime, GameTime.Now) <= 0)
                                        {
                                            currentSimulatedTime = currentSimulatedTime.AddMinutes(simulationIntervalInGameMinutes);
                                            UpdateAtmosphereTemperature(p, currentSimulatedTime);

                                            WeatherEvent we = null;
                                            if (AtmosphereStorm.Instance.Rain > 0)
                                            {
                                                //FieldInfo fi_currentWeatherEvent = typeof(AtmosphereStorm).GetField("_currentWeatherEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                                                //we = fi_currentWeatherEvent.GetValue(AtmosphereStorm.Instance) as WeatherEvent;
                                                if (AtmosphereStorm.Instance.CurrentWeatherEvent != null)
                                                    we = AtmosphereStorm.Instance.CurrentWeatherEvent;
                                            }
                                            UpdateWetness(p, currentPlayerIndex, simulationIntervalInGameMinutes, AtmosphereStorm.Instance.Rain, we != null ? we.Percentage : 0.0f);//(AtmosphereStorm.Instance.Rain == 1));
                                            UpdateBodyTemperature(p, currentPlayerIndex, simulationIntervalInGameMinutes, true);
                                            UpdateEnergy(p, currentPlayerIndex, simulationIntervalInGameMinutes, true);
                                            UpdateSick(p, currentPlayerIndex, simulationIntervalInGameMinutes, true);
                                            UpdateFever(p, currentPlayerIndex, simulationIntervalInGameMinutes);

                                            //Debug.Log("Stranded Deep "+MODNAME+" Mod : simulated time = " + currentSimulatedTime.ToShortTimeString() + " / atmosphere temperature = " + FahrenheitToCelsius(p.Statistics.environmentTemperature) + " / body temperature modifier : " + bodytemperatureEffect.CurrentTemperature);
                                        }
                                    }
                                    lastCheck = GameTime.Now;
                                }
                            }

                            UpdateAudioAndVisualEffects(p, currentPlayerIndex);
                            
                            if (hardFeverEffect)
                            {
                                CheckFeverBlocksCrafting(p, currentPlayerIndex);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Stranded Deep "+MODNAME+" Mod : error on update stats : " + e);
                        }

                        if (showDebugText)
                        {
                            try
                            {
                                debugText.text = "Body : " + FahrenheitToCelsius(p.Statistics.bodyTemperature) + " | Air : " + FahrenheitToCelsius(p.Statistics.environmentTemperature) + " | Water : " + FahrenheitToCelsius(AtmosphereTemperature.Instance.GetOceanTemperatureForDepth(p.transform.position.y));
                                debugText.text += "\n Wetness : " + wetEffect[currentPlayerIndex].Wetness + " | Body temperature : " + bodytemperatureEffect[currentPlayerIndex].CurrentTemperature + " | Is in the sun " + IsInTheSun(p) + " | Is rain " + AtmosphereStorm.Instance.Rain + " | Is storm " + IsStorm();
                                debugText.text += "\n Simple shelter : " + simpleSheltered + " | Housing Sheltered : " + housingSheltered + " | Sweating : " + sweating + " | Burning calories : " + burningCalories;
                                debugText.text += temperatureStringBuilder.ToString();
                                debugText.text += wetnessStringBuilder.ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Stranded Deep "+MODNAME+" Mod : error on update text : " + e);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep "+MODNAME+" Mod : error on update : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep "+MODNAME+" Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        public static float CurrentRain
        {
            get
            {
                WeatherEvent we = null;
                if (AtmosphereStorm.Instance.Rain > 0)
                {
                    we = AtmosphereStorm.Instance.CurrentWeatherEvent;
                }
                return we != null ? we.Percentage : 0.0f;
            }
        }


        //private static void Instance_WorldGenerated()
        //{
        //    Debug.Log("Stranded Deep AlternativeEndgame Mod : World Loaded event");
        //    worldLoaded = true;
        //}

        private static Beam.UI.QuickCrafterMenuPresenter quickCrafterMenu = null;
        private static Beam.UI.Crafting.CrafterMenuViewAdapterBase craftingMenu = null;
        private static FieldInfo fi_view = typeof(Beam.UI.Crafting.CombinationButtonPresenter).GetField("_view", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void CheckFeverBlocksCrafting(IPlayer player, int currentPlayerIndex)
        {
            if (player.Statistics.HasStatusEffect(feverEffect[currentPlayerIndex]))
            {
                if (craftingMenu == null)
                {
                    craftingMenu = Game.FindObjectOfType<Beam.UI.Crafting.CrafterMenuViewAdapterBase>();
                }
                //craftingMenu.CombinationsCanvasGroup.Interactable = false;
                foreach (Beam.UI.Crafting.CombinationButtonPresenter button in craftingMenu.CombinationButtons)
                {
                    if (button.Combination.CraftsmanshipLevelRequired > 0)
                    {
                        ((CombinationButtonViewAdapterBase)fi_view.GetValue(button)).CanvasGroup.Interactable = false;
                    }
                }
                if (quickCrafterMenu == null)
                {
                    quickCrafterMenu = Game.FindObjectOfType<Beam.UI.QuickCrafterMenuPresenter>();
                }
                quickCrafterMenu.CanOpen = false;
            }
            else if (!player.Statistics.HasStatusEffect(feverEffect[currentPlayerIndex]))
            {
                if (craftingMenu == null)
                {
                    craftingMenu = Game.FindObjectOfType<Beam.UI.Crafting.CrafterMenuViewAdapterBase>();
                }
                foreach (Beam.UI.Crafting.CombinationButtonPresenter button in craftingMenu.CombinationButtons)
                {
                    CombinationButtonViewAdapterBase view = (CombinationButtonViewAdapterBase)fi_view.GetValue(button);
                    view.CanvasGroup.Interactable = (!button.Combination.IsLocked && !button.Combination.IsLimited);
                }
                if (quickCrafterMenu == null)
                {
                    quickCrafterMenu = Game.FindObjectOfType<Beam.UI.QuickCrafterMenuPresenter>();
                }
                quickCrafterMenu.CanOpen = true;
            }
        }

        private static void Reset()
        {
            mustReinitImages = true;
            //mustReloadEffectsFromSave = true;
            //bodytemperatureEffect.Reset();
            //sickEffect.Reset();
            //feverEffect.Reset();
            lastCheck = DateTime.MinValue;
            nextCough = DateTime.MaxValue;
            nextGripe = DateTime.MaxValue;
            quickCrafterMenu = null;
            craftingMenu = null;
            //worldLoaded = false;
        }

        private static void UpdateAtmosphereTemperature(IPlayer p, DateTime? simulatedTime = null)
        {
            try
            {
                DateTime currentTime = GameTime.Now;
                if (simulatedTime != null)
                    currentTime = simulatedTime.GetValueOrDefault();

                // use GameCalendar.Instance.Month !
                //Debug.Log("Stranded Deep "+MODNAME+" Mod : current month in GameTime.Now = " + now.Month + " / current month in GameTime.Instance.Month : " + GameCalendar.Instance.Month);
                //Debug.Log("Stranded Deep "+MODNAME+" Mod : current month data Min : " + GameCalendar.Instance.CurrentMonthData.MinTemp + " / MAx " + GameCalendar.Instance.CurrentMonthData.MaxTemp);

                    // periode (1440 minutes par jour) = 2 * Pi / B => B = (2 * Pi) / 1440
                float period = ((float)Math.PI * 2f) / 1440f;
                float timeShiftLeft = 16f * 60f;//(2am is coldest, 2pm is hottest)
                //GameCalendar.Instance.CurrentMonthData; // <- USE THIS DATA

                float minTemperature = GameCalendar.Instance.CurrentMonthData.MinTemp;
                float maxTemperature = GameCalendar.Instance.CurrentMonthData.MaxTemp;
                if (AtmosphereStorm.Instance != null 
                    && AtmosphereStorm.Instance.CurrentWeatherEvent != null 
                    && AtmosphereStorm.Instance.CurrentWeatherEvent.HasStarted 
                    && AtmosphereStorm.Instance.CurrentWeatherEvent.isActiveAndEnabled)
                {
                    //Debug.Log("Stranded Deep "+MODNAME+" Mod : event affects temperature : " + AtmosphereStorm.Instance.CurrentWeatherEvent.Temperature);
                    minTemperature = AtmosphereStorm.Instance.CurrentWeatherEvent.Temperature - 1;
                    maxTemperature = AtmosphereStorm.Instance.CurrentWeatherEvent.Temperature;
                }
                float amplitude = (maxTemperature - minTemperature);
                float verticalShift = minTemperature;

                float currentTemperature = amplitude * ((float)Math.Sin(period * (currentTime.Hour * 60 + currentTime.Minute + timeShiftLeft)) + 1) / 2 + verticalShift;
                p.Statistics.environmentTemperature = currentTemperature;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep "+MODNAME+" Mod : error on update temperature : " + e);
            }
        }

        private static void RefreshColdTextCanvas()
        {
            if (modCanvas == null)
            {
                modCanvas = createCanvas(false, "WetAndColdModCanvas");

                Font defaultFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                GameObject textColdGO = new GameObject("WetAndColdMod_Text_Sprite");
                textColdGO.transform.SetParent(modCanvas.transform);
                debugText = textColdGO.AddComponent<Text>();
                debugText.horizontalOverflow = HorizontalWrapMode.Wrap;
                debugText.verticalOverflow = VerticalWrapMode.Overflow;
                debugText.alignment = TextAnchor.MiddleCenter;
                debugText.font = defaultFont;
                debugText.color = Color.red;// new Color(0.3f, 0.1f, 0f, 0.95f);
                debugText.fontSize = 10;
                debugText.text = "";
                debugText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250);
                debugText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 750);
                debugText.rectTransform.localPosition = new Vector3(0, 200);//new Vector3(bgRpgGO.GetComponent<RectTransform>().anchoredPosition.x, bgRpgGO.GetComponent<RectTransform>().anchoredPosition.y);

                //GameObject textGO = CreateText(canvas.transform, 0, 0, "Hello world", 32, Color.red);
                //textGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                InitWetnessMeter();
                InitTemperatureMeter();
                InitEnergyMeter();
                InitStaticImages();

                modCanvas.SetActive(coldCanvasVisible);
            }
            else
            {
                if (debugText != null)
                    debugText.enabled = showDebugText;
                modCanvas.SetActive(coldCanvasVisible);
            }
        }

        private static void UpdateWetness(IPlayer p, int currentPlayerIndex, int elapsedMinutes, int rain, float percentage)//bool isRaining)
        {
            try
            {
                wetnessStringBuilder.Append("\n");

                int wetnessChange = 0;
                if (p.Movement.IsUnderwater)
                {
                    wetnessStringBuilder.Append("underwater : " + ParameterValues.UNDERWATER_WETNESS_MODIFICATOR_PER_MINUTE + " | ");
                    wetnessChange += ParameterValues.UNDERWATER_WETNESS_MODIFICATOR_PER_MINUTE;
                }
                else if (p.Movement.touchingWater)
                {
                    wetnessStringBuilder.Append("touch water : " + ParameterValues.WATER_TOUCH_WETNESS_MODIFICATOR_PER_MINUTE + " | ");
                    wetnessChange += ParameterValues.WATER_TOUCH_WETNESS_MODIFICATOR_PER_MINUTE;
                }
                else if (rain > 0.0) //isRaining
                {
                    if (simpleSheltered || housingSheltered)
                    {
                        wetnessStringBuilder.Append("sheltered from rain");
                    }
                    else
                    {
                        wetnessStringBuilder.Append("rain : " + (rain * ParameterValues.UNSHELTERED_RAIN_WETNESS_MODIFICATOR_PER_MINUTE) + " | ");
                        wetnessChange += (rain * ParameterValues.UNSHELTERED_RAIN_WETNESS_MODIFICATOR_PER_MINUTE);
                    }
                }
                else if (IsInTheSun(p))
                {
                    wetnessStringBuilder.Append("sun : " + ParameterValues.DAYLIGHT_SUN_WETNESS_MODIFICATOR_PER_MINUTE + " | ");
                    wetnessChange += ParameterValues.DAYLIGHT_SUN_WETNESS_MODIFICATOR_PER_MINUTE;
                }
                else if (IsNight())
                {
                    wetnessStringBuilder.Append("night : " + ParameterValues.NIGHT_WETNESS_MODIFICATOR_PER_MINUTE + " | ");
                    wetnessChange += ParameterValues.NIGHT_WETNESS_MODIFICATOR_PER_MINUTE;
                }
                else
                {
                    wetnessStringBuilder.Append("day : " + ParameterValues.DAY_WETNESS_MODIFICATOR_PER_MINUTE + " | ");
                    wetnessChange += ParameterValues.DAY_WETNESS_MODIFICATOR_PER_MINUTE;
                }
                if (isHeating)
                {
                    wetnessStringBuilder.Append("heating : " + ParameterValues.HEATSOURCE_WETNESS_MODIFICATOR_PER_MINUTE + " | ");
                    wetnessChange += ParameterValues.HEATSOURCE_WETNESS_MODIFICATOR_PER_MINUTE;
                }

                temperatureStringBuilder.Append("\n total wetness change : " + wetnessChange + " * " + elapsedMinutes + "/min | ");
                wetEffect[currentPlayerIndex].Wetness += wetnessChange * elapsedMinutes;

                if (wetEffect[currentPlayerIndex].Wetness <= 0)
                    wetEffect[currentPlayerIndex].Wetness = 0;

                if (wetEffect[currentPlayerIndex].Wetness >= ParameterValues.MAX_WETNESS)
                    wetEffect[currentPlayerIndex].Wetness = ParameterValues.MAX_WETNESS;

                if (wetMeterImage != null)
                {
                    wetMeterImage.fillAmount = wetEffect[currentPlayerIndex].Wetness / (float)ParameterValues.MAX_WETNESS;
                }

                // update effect
                if (wetEffect[currentPlayerIndex].Wetness > 0)
                {
                    if (!p.Statistics.HasStatusEffect(wetEffect[currentPlayerIndex]))
                    {
                        p.Statistics.ApplyStatusEffect(wetEffect[currentPlayerIndex]);
                    }
                }
                else
                {
                    p.Statistics.RemoveStatusEffect(wetEffect[currentPlayerIndex]);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep "+MODNAME+" Mod : error UpdateWetness : " + e);
            }
        }

        private static FieldInfo fi_physicalLevel = typeof(PlayerSkills).GetField("_physicalLevel", BindingFlags.NonPublic | BindingFlags.Instance);

        public static float GetPhysical(int level)
        {
            if (level <= 0)
                return 1f;

            // SkillsEffectMultipliersAsset
            float[] physicalModifier = new float[]
            {
                                1f,
                                1.05f,
                                1.1f,
                                1.15f,
                                1.2f,
                                1.25f,
                                1.35f,
                                1.5f
            };

            if (level < 7)
            {
                return physicalModifier[level - 1];
            }
            return physicalModifier[7];
        }

        private static void UpdateBodyTemperature(IPlayer p, int currentPlayerIndex, int elapsedMinutes, bool isSleeping)
        {
            //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 1");

            if (p.Statistics == null)
                return;
            if (!p.Statistics.HasStatusEffect(bodytemperatureEffect[currentPlayerIndex]))
                p.Statistics.ApplyStatusEffect(bodytemperatureEffect[currentPlayerIndex]);

            try
            {
                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 2");

                temperatureStringBuilder.Append("\n");
                float totalBodyTemperatureChange = 0;

                float atmosphereTemperatureFeeling = 0;
                //float atmosphereTemperatureDelta = 0;

                if (p.Statistics.HasStatusEffect(sickEffect[currentPlayerIndex]))
                {
                    totalBodyTemperatureChange += ParameterValues.SICK_TEMP_MODIFICATOR_PER_MINUTE;
                    temperatureStringBuilder.Append("SICK_TEMP_MODIFICATOR : " + ParameterValues.SICK_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                }

                if (p.Statistics.HasStatusEffect(feverEffect[currentPlayerIndex]))
                {
                    totalBodyTemperatureChange += ParameterValues.FEVER_TEMP_MODIFICATOR_PER_MINUTE;
                    temperatureStringBuilder.Append("FEVER_TEMP_MODIFICATOR : " + ParameterValues.FEVER_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 3");

                // atmosphere has no influence underwater
                if (!p.Movement.IsUnderwater)
                {
                    if (p.Statistics.environmentTemperature > CelsiusToFahrenheit(ParameterValues.COMFORT_MAX_TEMPERATURE_DEGREES_AIR))
                    {
                        atmosphereTemperatureFeeling = Math.Sign(p.Statistics.environmentTemperature - CelsiusToFahrenheit(ParameterValues.COMFORT_MAX_TEMPERATURE_DEGREES_AIR));
                        //atmosphereTemperatureDelta = p.Statistics.environmentTemperature - CelsiusToFahrenheit(ParameterValues.COMFORT_MAX_TEMPERATURE_DEGREES_AIR);
                    }
                    else if (p.Statistics.environmentTemperature < CelsiusToFahrenheit(ParameterValues.COMFORT_MIN_TEMPERATURE_DEGREES_AIR))
                    {
                        atmosphereTemperatureFeeling = Math.Sign(p.Statistics.environmentTemperature - CelsiusToFahrenheit(ParameterValues.COMFORT_MIN_TEMPERATURE_DEGREES_AIR));
                        //atmosphereTemperatureDelta = p.Statistics.environmentTemperature - CelsiusToFahrenheit(ParameterValues.COMFORT_MIN_TEMPERATURE_DEGREES_AIR);
                    }

                    if (IsNight())
                    {
                        atmosphereTemperatureFeeling += ParameterValues.NIGHT_TEMP_MODIFICATOR_PER_MINUTE;
                    }

                    atmosphereTemperatureFeeling = atmosphereTemperatureFeeling / ParameterValues.BALANCING_FACTOR;

                    totalBodyTemperatureChange += atmosphereTemperatureFeeling;
                    temperatureStringBuilder.Append("atmosphere feeling : " + atmosphereTemperatureFeeling + " | ");
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 4");

                if (IsInTheSun(p) && !p.Movement.InWater)
                {
                    if (GameTime.Now.Hour <= 7 || GameTime.Now.Hour >= 18)
                    {
                        totalBodyTemperatureChange += ParameterValues.DAYLIGHT_DAWN_TEMP_MODIFICATOR_PER_MINUTE;
                        temperatureStringBuilder.Append("DAYLIGHT_SUN_TEMP_MODIFICATOR : " + ParameterValues.DAYLIGHT_DAWN_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                    }
                    else if (GameTime.Now.Hour <= 10 || GameTime.Now.Hour >= 16)
                    {
                        totalBodyTemperatureChange += ParameterValues.DAYLIGHT_MID_SUN_TEMP_MODIFICATOR_PER_MINUTE;
                        temperatureStringBuilder.Append("DAYLIGHT_SUN_TEMP_MODIFICATOR : " + ParameterValues.DAYLIGHT_MID_SUN_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                    }
                    else
                    {
                        totalBodyTemperatureChange += ParameterValues.DAYLIGHT_FULL_SUN_TEMP_MODIFICATOR_PER_MINUTE;
                        temperatureStringBuilder.Append("DAYLIGHT_SUN_TEMP_MODIFICATOR : " + ParameterValues.DAYLIGHT_FULL_SUN_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                    }
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 5");

                if (p.Movement.IsUnderwater)
                {
                    float waterTemperatureEffect = AtmosphereTemperature.Instance.GetOceanTemperatureForDepth(p.transform.position.y) - CelsiusToFahrenheit(ParameterValues.NEUTRAL_TEMPERATURE_DEGREES_WATER);
                    if (!(bodytemperatureEffect[currentPlayerIndex].IsHot && waterTemperatureEffect > 0))
                    {
                        // water chills
                        totalBodyTemperatureChange += waterTemperatureEffect;
                        temperatureStringBuilder.Append("water Effect : " + waterTemperatureEffect + " | ");
                    }
                }
                else if (wetEffect[currentPlayerIndex].Wetness > 0)
                {
                    if (totalBodyTemperatureChange > 0)
                    {
                        // if hot, the wetness will cool down
                        if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature > 0)
                        {
                            temperatureStringBuilder.Append("WETNESS_TEMP_MODIFICATOR * EVAPORATION : " + ParameterValues.EVAPORATION_TEMP_MODIFICATOR_PER_MINUTE * ParameterValues.WETNESS_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                            totalBodyTemperatureChange += ParameterValues.EVAPORATION_TEMP_MODIFICATOR_PER_MINUTE * ParameterValues.WETNESS_TEMP_MODIFICATOR_PER_MINUTE;
                        }
                        else
                        {
                            temperatureStringBuilder.Append("WETNESS_TEMP_MODIFICATOR : " + ParameterValues.WETNESS_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                            totalBodyTemperatureChange += ParameterValues.WETNESS_TEMP_MODIFICATOR_PER_MINUTE;
                        }
                    }
                    else
                    {
                        temperatureStringBuilder.Append("WETNESS_TEMP_MODIFICATOR : " + ParameterValues.WETNESS_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                        totalBodyTemperatureChange += ParameterValues.WETNESS_TEMP_MODIFICATOR_PER_MINUTE;
                    }
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 6");

                if (HasPhysicalActivity(p))
                {
                    totalBodyTemperatureChange += ParameterValues.ACTIVITY_TEMP_MODIFICATOR_PER_MINUTE;
                    temperatureStringBuilder.Append("ACTIVITY_TEMP_MODIFICATOR : " + ParameterValues.ACTIVITY_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                    if (coldCanvasVisible)
                    {
                        activityImage.enabled = true;
                    }
                }
                else
                {
                    if (coldCanvasVisible)
                    {
                        activityImage.enabled = false;
                    }
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 7");

                if (housingSheltered && totalBodyTemperatureChange != 0)
                {
                    if (totalBodyTemperatureChange < 0)
                    {
                        if (totalBodyTemperatureChange + ParameterValues.HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE >= 0)
                        {
                            temperatureStringBuilder.Append("housingSheltered : reset | ");
                            totalBodyTemperatureChange = 0;
                        }
                        else
                        {
                            temperatureStringBuilder.Append("housingSheltered : " + ParameterValues.HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                            totalBodyTemperatureChange += ParameterValues.HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE;
                        }
                    }
                    else
                    {
                        if (totalBodyTemperatureChange + ParameterValues.HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE <= 0)
                        {
                            temperatureStringBuilder.Append("housingSheltered : reset | ");
                            totalBodyTemperatureChange = 0;
                        }
                        else
                        {
                            temperatureStringBuilder.Append("housingSheltered : -" + ParameterValues.HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                            totalBodyTemperatureChange -= ParameterValues.HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE;
                        }
                    }
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 8");

                if (isHeating)
                {
                    if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature < ParameterValues.HOT_THRESHOLD || atmosphereTemperatureFeeling <= 0)
                    {
                        if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature + ParameterValues.HEATSOURCE_TEMP_MODIFICATOR_PER_MINUTE < ParameterValues.HOT_THRESHOLD)
                        {
                            temperatureStringBuilder.Append("isHeating : " + ParameterValues.HEATSOURCE_TEMP_MODIFICATOR_PER_MINUTE + " | ");
                            totalBodyTemperatureChange += ParameterValues.HEATSOURCE_TEMP_MODIFICATOR_PER_MINUTE;
                        }
                        else
                        {
                            temperatureStringBuilder.Append("isHeating : reset | ");
                            totalBodyTemperatureChange = 0;
                        }
                    }
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 9");

                float physicalModificator = GetPhysical((int)fi_physicalLevel.GetValue(p.PlayerSkills));
                // natural resistance with physical
                if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature > 0)
                {
                    if (totalBodyTemperatureChange > 0)
                    {
                        totalBodyTemperatureChange = totalBodyTemperatureChange / physicalModificator;
                        temperatureStringBuilder.Append("physicalModificator : 1/" + physicalModificator);
                    }
                    else
                    {
                        totalBodyTemperatureChange = totalBodyTemperatureChange * physicalModificator;
                        temperatureStringBuilder.Append("physicalModificator : *" + physicalModificator);
                    }
                }
                else if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature < 0)
                {
                    if (totalBodyTemperatureChange < 0)
                    {
                        temperatureStringBuilder.Append("physicalModificator : 1/" + physicalModificator);
                        totalBodyTemperatureChange = totalBodyTemperatureChange / physicalModificator;
                    }
                    else
                    {
                        totalBodyTemperatureChange = totalBodyTemperatureChange * physicalModificator;
                        temperatureStringBuilder.Append("physicalModificator : *" + physicalModificator);
                    }
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 10");

                // test sweating, if wet, can't sweat
                if (wetEffect[currentPlayerIndex].Wetness <= 0 && (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature + totalBodyTemperatureChange) >= ParameterValues.SWEAT_THRESHOLD)
                {
                    sweating = true;
                    burningCalories = false;

                    float regulatedTemperatureChange = Math.Min(0, totalBodyTemperatureChange - ParameterValues.NATURAL_TEMPERATURE_REGULATION_PER_MINUTE);
                    if (isSleeping)
                    {
                        regulatedTemperatureChange = Math.Min(0, totalBodyTemperatureChange - ParameterValues.NATURAL_TEMPERATURE_REGULATION_PER_MINUTE * ParameterValues.NATURAL_TEMPERATURE_REGULATION_SLEEP_EFFICIENCY);
                    }
                    ConsumeWater(p, Math.Abs(totalBodyTemperatureChange - regulatedTemperatureChange) / ParameterValues.REGULATION_CONSUMPTION_DIVIDE_FACTOR);

                    temperatureStringBuilder.Append("natural regulation (sweat) : " + regulatedTemperatureChange + " | ");
                    totalBodyTemperatureChange = regulatedTemperatureChange;
                }
                // test burning calories
                else if ((bodytemperatureEffect[currentPlayerIndex].CurrentTemperature + totalBodyTemperatureChange) <= ParameterValues.CALORIES_THRESHOLD)
                {
                    sweating = false;
                    burningCalories = true;

                    float regulatedTemperatureChange = Math.Max(0, totalBodyTemperatureChange + ParameterValues.NATURAL_TEMPERATURE_REGULATION_PER_MINUTE);
                    if (isSleeping)
                    {
                        regulatedTemperatureChange = Math.Max(0, totalBodyTemperatureChange + ParameterValues.NATURAL_TEMPERATURE_REGULATION_PER_MINUTE * ParameterValues.NATURAL_TEMPERATURE_REGULATION_SLEEP_EFFICIENCY);
                    }
                    ConsumeCalories(p, Math.Abs(totalBodyTemperatureChange - regulatedTemperatureChange) / ParameterValues.REGULATION_CONSUMPTION_DIVIDE_FACTOR);

                    temperatureStringBuilder.Append("natural regulation (calories) : " + regulatedTemperatureChange + " | ");
                    totalBodyTemperatureChange = regulatedTemperatureChange;
                }
                else
                {
                    sweating = false;
                    burningCalories = false;
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 11");

                sweatingImage.enabled = sweating;
                caloriesImage.enabled = burningCalories;

                temperatureStringBuilder.Append("\n total bodytemp change : " + totalBodyTemperatureChange + " * " + elapsedMinutes + "/min | ");
                bodytemperatureEffect[currentPlayerIndex].CurrentTemperature += totalBodyTemperatureChange * elapsedMinutes;

                if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature <= -ParameterValues.MAX_TEMP)
                {
                    bodytemperatureEffect[currentPlayerIndex].CurrentTemperature = -ParameterValues.MAX_TEMP;
                }

                if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature >= ParameterValues.MAX_TEMP)
                {
                    bodytemperatureEffect[currentPlayerIndex].CurrentTemperature = ParameterValues.MAX_TEMP;
                }

                if (coldMeterImage != null)
                {
                    if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature < 0)
                        coldMeterImage.fillAmount = Math.Abs(bodytemperatureEffect[currentPlayerIndex].CurrentTemperature) / (float)ParameterValues.MAX_TEMP;
                    else
                        coldMeterImage.fillAmount = 0;
                }

                if (hotMeterImage != null)
                {
                    if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature > 0)
                        hotMeterImage.fillAmount = bodytemperatureEffect[currentPlayerIndex].CurrentTemperature / (float)ParameterValues.MAX_TEMP;
                    else
                        hotMeterImage.fillAmount = 0;
                }

                if (bodytemperatureEffect[currentPlayerIndex].IsCold)
                {
                    if (bodytemperatureEffect[currentPlayerIndex].StartedTime == DateTime.MinValue)
                        bodytemperatureEffect[currentPlayerIndex].StartedTime = GameTime.Now;
                    bodytemperatureEffect[currentPlayerIndex].PlayShiver = true;

                    if (bodytemperatureEffect[currentPlayerIndex].PlayShiver && !bodytemperatureEffect[currentPlayerIndex].PlayedShiver && !p.Movement.IsUnderwater && !p.Movement.touchingWater)
                    {
                        PlayShiver(p);
                        bodytemperatureEffect[currentPlayerIndex].PlayedShiver = true;
                    }

                    bodytemperatureEffect[currentPlayerIndex].PlayRelief = true;
                }
                else
                {
                    if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature >= 0 && p.Statistics.HasStatusEffect(bodytemperatureEffect[currentPlayerIndex]) && bodytemperatureEffect[currentPlayerIndex].PlayRelief)
                    {
                        PlayRelief(p);
                    }
                    bodytemperatureEffect[currentPlayerIndex].Reset();
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 12");

                if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature >= (ParameterValues.HOT_THRESHOLD)
                    && (p.Statistics.GetPlayerEffect<Sunburn>() != null && (1f - p.Statistics.GetPlayerEffect<Sunburn>().SunExposureTime) <= 0.1f))
                {
                    Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature SunExposureTime : " + (1f - p.Statistics.GetPlayerEffect<Sunburn>().SunExposureTime));
                    if (!p.Statistics.HasStatusEffect(hotEffect[currentPlayerIndex]))
                    {
                        p.Statistics.RemoveStatusEffect(coldEffect[currentPlayerIndex]);
                        p.Statistics.ApplyStatusEffect(hotEffect[currentPlayerIndex], true);
                        if (WorldUtilities.IsWorldLoaded())
                        {
                            ShowSubtitles(p, Main.HOT_MESSAGE);
                        }
                    }
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        hotEffect[currentPlayerIndex].ShowEffect(p);
                    }
                }
                else if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature <= (ParameterValues.COLD_THRESHOLD)
                    && !p.Statistics.HasStatusEffect(coldEffect[currentPlayerIndex]))
                {
                    p.Statistics.RemoveStatusEffect(hotEffect[currentPlayerIndex]);
                    p.Statistics.ApplyStatusEffect(coldEffect[currentPlayerIndex], true);
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        ShowSubtitles(p, Main.COLD_MESSAGE);
                    }
                }
                else if (bodytemperatureEffect[currentPlayerIndex].CurrentTemperature < (ParameterValues.HOT_THRESHOLD) 
                    && bodytemperatureEffect[currentPlayerIndex].CurrentTemperature > (ParameterValues.COLD_THRESHOLD))
                {
                    p.Statistics.RemoveStatusEffect(coldEffect[currentPlayerIndex]);
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        hotEffect[currentPlayerIndex].HideEffect(p);
                    }
                    p.Statistics.RemoveStatusEffect(hotEffect[currentPlayerIndex]);
                }

                //Debug.Log("Stranded Deep "+MODNAME+" Mod : UpdateBodyTemperature 13");
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep "+MODNAME+" Mod : error UpdateBodyTemperature : " + e);
            }
        }

        private static void ConsumeWater(IPlayer p, float delta)
        {
            temperatureStringBuilder.Append(" | sweating : " + delta);

            if (p.Movement.GodMode || p.Statistics.Invincible)
            {
                return;
            }

            SetWater(p, GetWater(p) - delta);
        }

        private static void ConsumeCalories(IPlayer p, float delta)
        {
            temperatureStringBuilder.Append(" | burning calories : " + delta);

            if (p.Movement.GodMode || p.Statistics.Invincible)
            {
                return;
            }

            SetCalories(p, GetCalories(p) - delta);
        }

        private static void UpdateSick(IPlayer p, int currentPlayerIndex, int elapsedMinutes, bool isSleeping)
        {
            if (!p.Statistics.HasStatusEffect(bodytemperatureEffect[currentPlayerIndex]))
            {
                // error
                return;
            }

            if (bodytemperatureEffect[currentPlayerIndex].CheckSick(isSleeping, GetSleep(p)))
            {
                if (!p.Statistics.HasStatusEffect(sickEffect[currentPlayerIndex]))
                {
                    Debug.Log("Stranded Deep "+MODNAME+" Mod : caught a cold");
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        ShowSubtitles(p, Main.SICK_MESSAGE);
                    }
                    sickEffect[currentPlayerIndex].Reset();
                    p.Statistics.ApplyStatusEffect(sickEffect[currentPlayerIndex]);
                    nextCough = DateTime.Now;
                }
            }

            if (p.Statistics.HasStatusEffect(sickEffect[currentPlayerIndex]))
            {
                if (sickEffect[currentPlayerIndex].CheckHealed(p, bodytemperatureEffect[currentPlayerIndex], feverEffect[currentPlayerIndex], housingSheltered, elapsedMinutes))
                {
                    nextGripe = DateTime.MaxValue;
                    nextCough = DateTime.MaxValue;
                    p.Statistics.RemoveStatusEffect(sickEffect[currentPlayerIndex]);
                    //p.Movement.CanSprint = true;
                    PlayRelief(p);
                }
                else
                {
                    float currrentSleep = GetSleep(p);
                    SetSleep(p, currrentSleep + ParameterValues.SICK_ENERGY_MODIFICATOR_PER_MINUTE);
                    //p.Movement.CanSprint = false;
                }
            }
        }

        private static void UpdateFever(IPlayer p, int currentPlayerIndex, int elapsedMinutes)
        {
            if (p.Statistics.HasStatusEffect(feverEffect[currentPlayerIndex]))
            {
                bodytemperatureEffect[currentPlayerIndex].HasHadFever = true;
                if (feverEffect[currentPlayerIndex].CheckHealed(p, bodytemperatureEffect[currentPlayerIndex], housingSheltered, elapsedMinutes))
                {
                    nextGripe = DateTime.MaxValue;
                    p.Statistics.RemoveStatusEffect(feverEffect[currentPlayerIndex]);
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        ShowSubtitles(p, Main.END_FEVER_MESSAGE);
                    }
                    PlayRelief(p);
                }
                else
                {
                    float currrentSleep = GetSleep(p);
                    SetSleep(p, currrentSleep + ParameterValues.FEVER_ENERGY_MODIFICATOR_PER_MINUTE);
                }
            }
        }

        private static void UpdateEnergy(IPlayer p, int currentPlayerIndex, int elapsedMinutes, bool isSleeping)
        {
            // resting mechanic
            if (housingSheltered)
                SetSleep(p, GetSleep(p) + elapsedMinutes * ParameterValues.TIME_ENERGY_MODIFICATOR_PER_MINUTE);

            if (!isSleeping)
                SetSleep(p, GetSleep(p) - elapsedMinutes * ParameterValues.TIME_ENERGY_MODIFICATOR_PER_MINUTE);

            if (energyMeterImage != null)
            {
                energyMeterImage.fillAmount = p.Statistics.SleepPercent;
            }
        }
    }
}
