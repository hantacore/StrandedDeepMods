using Beam;
using Beam.Rendering;
using Beam.Terrain;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Funlabs;
using Beam.Crafting;
using Beam.Serialization.Json;
using System.IO;
using Beam.Serialization;
using HarmonyLib;
using System.Runtime.CompilerServices;
using StrandedDeepModsUtils;

namespace StrandedDeepTweaksMod
{
    static partial class Main
    {
        private static string configFileName = "StrandedDeepTweaksMod.config";

        private static Harmony harmony;

        private static bool alwaysSkipIntro = false;

        private static bool infiniteAir = false;
        private static bool biggerAirTank = false;
        //private static List<Funlabs.MiniGuid> handledAirtankReferences;

        private static bool showDistances = false;

        private static bool saveAnywhereAllowed = true;

        private static bool addBuoyancies = false;

        private static bool harderbosses = false;
        private static bool hardcorebosses = false;

        private static bool fixItemWeigths = false;
        private static bool fixRainReset = true;
        private static bool fixRainStart = true;
        private static bool betterRainTextures = true;
        private static bool fixBirdsEverywhere = true;
        private static bool fixFlyingPigs = true;

        private static bool fixAudioReset = true;

        private static bool biggerStackSizes = false;

        private static bool bossesInitialized = false;

        private static bool smallerRaftTurnAngle = false;

        private static bool spawnJerrycansAbaiaWreck = false;

        private static bool compassActive = false;
        private static bool permaCompassEnabled = false;
        private static bool permaCompassAlwaysVisible = false;

        private static bool betterSpyglass = false;

        //private static bool hardersharks = true;
        //private static bool hardcoresharks = true;

        private static bool infiniteGas = false;
        private static bool biggerGasTank = false;
        //private static List<string> handledGastankReferences;

        private static bool stopMissedAchievementsSpam = false;

        private static float textCanvasDefaultScreenWitdh = 1024f;
        private static float textCanvasDefaultScreenHeight = 768f;

        private static FastRandom random = new FastRandom();

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        internal static bool perfCheck = false;

        private static FieldInfo fi_holding;
        //private static FieldInfo fi_owner;
        private static FieldInfo fi_rain = typeof(AtmosphereStorm).GetField("_rain", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_AudioManagerInstance = typeof(AudioManager).GetField("_Instance", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo fi_rigidbody = typeof(Buoyancy).GetField("_rigidbody", BindingFlags.NonPublic | BindingFlags.Instance);
        private static PropertyInfo fi_LootSpawned = typeof(Interactive_STORAGE).GetProperty("LootSpawned", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static Dictionary<InteractiveType, int> STACK_SIZES = null;

        internal static BuoyancyHandler buoyancyHandler = null;

        internal static MethodInfo mi_Use = typeof(InteractiveObject).GetMethod("Use", BindingFlags.Instance | BindingFlags.Public);
        internal static MethodInfo mi_OnUsing = typeof(VehicleMovementBase).GetMethod("OnUsing", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static FieldInfo fi_rudder = typeof(RudderVehicleMovement).GetField("_rudder", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_rudderInput = typeof(RudderVehicleMovement).GetField("_rudderInput", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_rudderTurningAngle = typeof(RudderVehicleMovement).GetField("_rudderTurningAngle", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_rudderDeflection = typeof(RudderVehicleMovement).GetField("_rudderDeflection", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static FieldInfo fi_motorRudder = typeof(MotorVehicleMovement).GetField("_rudder", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_fuel = typeof(MotorVehicleMovement).GetField("_fuel", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_motorRudderInput = typeof(MotorVehicleMovement).GetField("_rudderInput", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_propellerTurningAngle = typeof(MotorVehicleMovement).GetField("_propellerTurningAngle", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_propellerRudderDeflection = typeof(MotorVehicleMovement).GetField("_rudderDeflection", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_ranOutOfFuel = typeof(MotorVehicleMovement).GetField("_ranOutOfFuel", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_engineForce = typeof(MotorVehicleMovement).GetField("_engineForce", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static MethodInfo mi_AnchorsDeployed = typeof(MotorVehicleMovement).GetMethod("AnchorsDeployed", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static MethodInfo mi_OnFuelRanOut = typeof(MotorVehicleMovement).GetMethod("OnFuelRanOut", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo fi_buoyancyDensity = typeof(Buoyancy).GetField("_density", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_stackSizes = typeof(SlotStorage).GetField("STACK_SIZES", BindingFlags.NonPublic | BindingFlags.Static);

        internal static FieldInfo fi_cookingLevel = typeof(PlayerSkills).GetField("_cookingLevel", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_physicalLevel = typeof(PlayerSkills).GetField("_physicalLevel", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_huntingLevel = typeof(PlayerSkills).GetField("_huntingLevel", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static FieldInfo fi_harvestingLevel = typeof(PlayerSkills).GetField("_harvestingLevel", BindingFlags.NonPublic | BindingFlags.Instance);

        static string _infojsonlocation = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedDeepTweaksMod/StrandedDeepTweaksMod/Info.json";

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;
                modEntry.OnUnload = OnUnload;


                //handledAirtankReferences = new List<Funlabs.MiniGuid>();
                //handledGastankReferences = new List<string>();

                STACK_SIZES = fi_stackSizes.GetValue(null) as Dictionary<InteractiveType, int>;

                buoyancyHandler = new BuoyancyHandler();

                ReadConfig();

                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings MasterVolume before : " + Options.AudioSettings.MasterVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings MusicVolume before : " + Options.AudioSettings.MusicVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings VoiceVolume before : " + Options.AudioSettings.VoiceVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings EffectsVolume before : " + Options.AudioSettings.EffectsVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings EnvironmentVolume before : " + Options.AudioSettings.EnvironmentVolume);
                ForceAudioSettingsReload();
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings MasterVolume after : " + Options.AudioSettings.MasterVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings MusicVolume after : " + Options.AudioSettings.MusicVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings VoiceVolume after : " + Options.AudioSettings.VoiceVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings EffectsVolume after : " + Options.AudioSettings.EffectsVolume);
                //Debug.Log("Stranded Deep Tweaks Mod : Audiosettings EnvironmentVolume after : " + Options.AudioSettings.EnvironmentVolume);

                fi_holding = typeof(Beam.Crafting.InteractiveObject_SPYGLASS).GetField("_holding", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi_holding == null)
                {
                    Debug.Log("Stranded Deep Tweaks Mod : _holding field not found");
                    return false;
                }

                //fi_owner = typeof(Beam.Crafting.InteractiveObject_SPYGLASS).BaseType.GetField("_owner", BindingFlags.NonPublic | BindingFlags.Instance);
                //if (fi_owner == null)
                //{
                //    Debug.Log("Stranded Deep Tweaks Mod : _owner field not found");
                //    return false;
                //}

                if (fixItemWeigths)
                {
                    foreach (Beam.Serialization.PrefabReference prefab in Beam.Serialization.Prefabs.PrefabReferences)
                    {
                        //prefab.Prefab.

                        //Renderer r = prefab.Prefab.GetComponent<Renderer>();
                        //if (r != null)
                        //{
                        //    Debug.Log("Stranded Deep Tweaks Mod : prefab renderer type : " + r.GetType().Name);
                        //    Texture2D t2d = new Texture2D(1, 1);
                        //    t2d.SetPixel(0, 0, Color.red);
                        //    r.sharedMaterial.mainTexture = t2d;
                        //}

                        if (prefab.Id == 13)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing rock mass to 1");
                                body.mass = 1;
                            }
                        }
                        if (prefab.Id >= 1 && prefab.Id <= 7 || prefab.Id >= 16 && prefab.Id <= 19 || prefab.Id >= 191 && prefab.Id <= 200 || prefab.Id == 247 || prefab.Id == 281 || prefab.Id == 328)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing " + prefab.Prefab.name + " mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 16)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing water bottle mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 250)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing water skin mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 25)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing compass mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 27)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing flare gun mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 29)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing lantern mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 39)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing cloth mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 41)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing label maker mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 14)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing rope coil mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 26)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing ducttape mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 74)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing bandage mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 150)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing kura mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 151)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing quwawa mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 156)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing coconut mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 176)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing spear mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }
                        if (prefab.Id == 187)
                        {
                            foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                            {
                                Debug.Log("Stranded Deep Tweaks Mod : changing kindling mass to 0.3");
                                body.mass = 0.3f;
                            }
                        }

                        //foreach (Rigidbody body in prefab.Prefab.AllRigidbodies)
                        //{
                        //    Debug.Log("Stranded Deep Tweaks Mod : " + prefab.Id + " / " + prefab.Prefab.name + " mass = " + body.mass);
                        //}
                    }
                }

                //if (biggerStackSizes)
                //{
                //    IncreaseStackSizes(4);
                //}

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                VersionChecker.CheckVersion(modEntry, _infojsonlocation);

                Debug.Log("Stranded Deep Tweaks Mod properly loaded");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Tweaks Mod error on load : " + e);
            }
            finally
            {
                Debug.Log("Stranded Deep Tweaks Mod load time (ms) = " + chrono.ElapsedMilliseconds);
            }

            return false;
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        private static void ForceAudioSettingsReload()
        {
            try
            {
                if (!File.Exists(FilePath.OPTIONS_FILE))
                    return;

                JObject jobject;
                try
                {
                    using (StreamReader streamReader = new StreamReader(FilePath.OPTIONS_FILE))
                    {
                        jobject = new JObject(streamReader.ReadToEnd());
                    }
                    JObject field4 = jobject.GetField("Audio");
                    if (field4 != null)
                    {
                        if (!SavingUtilities.Load(Options.AudioSettings, field4))
                        {
                            Debug.LogError("Options::Load:: Could not load [audio] data. \nAudio settings have been reverted to defaults.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Options::Load:: No [audio] data was found. \nAudio settings have been reverted to defaults.");
                    }
                }
                catch (Exception ex)
                {
                    string str = "Options::Load:: Could not load options data. \nOptions settings have been reverted to defaults. \n\n";
                    Exception ex2 = ex;
                    Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
                    return;
                }
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep Tweaks Mod error in ForceAudioSettingsReload : " + e);
            }
        }

        //private static void IncreaseStackSizes(int ratio)
        //{
        //    try
        //    {
        //        Debug.Log("Stranded Deep Tweaks Mod : changing stack sizes ratio to " + ratio);
        //        Debug.Log("Stranded Deep Tweaks Mod : changing stack sizes ratio step 1");
        //        STACK_SIZES[InteractiveType.CONTAINER] = 1 * ratio;
        //        STACK_SIZES[InteractiveType.CRAFTING_LEAVES] = 24 * ratio;
        //        STACK_SIZES[InteractiveType.CRAFTING_LOG] = 1 * ratio;
        //        STACK_SIZES[InteractiveType.TOOLS_ARROW] = 24 * ratio;
        //        STACK_SIZES[InteractiveType.TOOLS_SPEARGUN_ARROW] = 24 * ratio;
        //        STACK_SIZES[InteractiveType.ANIMALS_FISH] = 8 * ratio;
        //        STACK_SIZES[InteractiveType.FOOD_MEAT] = 8 * ratio;
        //        STACK_SIZES[InteractiveType.FOOD_FRUIT] = 8 * ratio;

        //        Debug.Log("Stranded Deep Tweaks Mod : changing stack sizes ratio step 2");
        //        // default is 4, add any value in the dictionary to increase
        //        //private const int DEFAULT_STACK_SIZE = 4;
        //        foreach (InteractiveType current in Enum.GetValues(typeof(InteractiveType)).Cast<InteractiveType>())
        //        {
        //            if (!STACK_SIZES.ContainsKey(current))
        //            {
        //                STACK_SIZES.Add(current, 4 * ratio);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep Tweaks Mod : error on IncreaseStackSizes : " + e);
        //    }
        //}

        


        //static float _latitude;
        //static float _longitude;

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Tweaks mod by Hantacore");
            GUILayout.Label("<b>Various fixes</b>");
            fixRainReset = GUILayout.Toggle(fixRainReset, "Fix the rain not resetting bug");
            fixRainStart = GUILayout.Toggle(fixRainStart, "Fix the rain not starting bug");
            fixAudioReset = GUILayout.Toggle(fixAudioReset, "Fix audio parameters resetting bug");
            fixBirdsEverywhere = GUILayout.Toggle(fixBirdsEverywhere, "Fix birds and bats only showing on starting island");
            fixFlyingPigs = GUILayout.Toggle(fixFlyingPigs, "Fix pigs and giant crabs flying bug (experimental)");
            GUILayout.Label("<b>QoL options</b>");
            alwaysSkipIntro = GUILayout.Toggle(alwaysSkipIntro, "Always skip the private jet intro");
            saveAnywhereAllowed = GUILayout.Toggle(saveAnywhereAllowed, "Save anywhere allowed (F7)");
            stopMissedAchievementsSpam = GUILayout.Toggle(stopMissedAchievementsSpam, "No more missed achievement notifications");
            GUILayout.Label("<b>Gameplay options</b>");
            biggerStackSizes = GUILayout.Toggle(biggerStackSizes, stackSizeRatio + " x bigger stack sizes");
            stackSizeRatio = (int)GUILayout.HorizontalSlider(stackSizeRatio, 2, 10);
            showDistances = GUILayout.Toggle(showDistances, "Show distances through spyglass");
            betterSpyglass = GUILayout.Toggle(betterSpyglass, "Enhanced spyglass (x" + MaxZoom + " zoom)");
            biggerAirTank = GUILayout.Toggle(biggerAirTank, "Bigger air tank (" + biggerAirTankCapacity + " charges)");
            infiniteAir = GUILayout.Toggle(infiniteAir, "Infinite air in tank");
            biggerGasTank = GUILayout.Toggle(biggerGasTank, "Bigger gas tanks in vehicles (" + biggerGasTankSize + " charges)");
            infiniteGas = GUILayout.Toggle(infiniteGas, "Inifinite gas in vehicles");
            smallerRaftTurnAngle = GUILayout.Toggle(smallerRaftTurnAngle, "Improve raft steering (smaller turning angle)");
            harderbosses = GUILayout.Toggle(harderbosses, "Make the bosses harder (doubled HP, instant bleeding, Abaia poisons)");
            hardcorebosses = GUILayout.Toggle(hardcorebosses, "Make the bosses hardcore (increased damage)");
            permaCompassEnabled = GUILayout.Toggle(permaCompassEnabled, "Show compass when item is in inventory");// (toggle with XX)");
            permaCompassAlwaysVisible = GUILayout.Toggle(permaCompassAlwaysVisible, "Always show compass");// (toggle with XX)");
            GUILayout.Label("<b>Realism options</b>");
            addBuoyancies = GUILayout.Toggle(addBuoyancies, "Add floatability to some objects (performance impact)");
            fixItemWeigths = GUILayout.Toggle(fixItemWeigths, "More realistic item weights on raft (WIP)");
            betterRainTextures = GUILayout.Toggle(betterRainTextures, "Better rain textures");

            //if (Atmosphere.Instance != null)
            //{
            //    _latitude = GUILayout.HorizontalSlider(_latitude, -90f, 90f);
            //    _longitude = GUILayout.HorizontalSlider(_longitude, -180f, 180f);
            //}

            IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
            // show only if ingame
            if (players.Count > 0)
            {
                GUILayout.Label("<b><color=orange>Trainer/cheats</color></b>");

                Beam.Crafting.CraftingCombination craftingCombination3 = PlayerRegistry.LocalPlayer.Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "AIRCRAFT ENGINE PART");
                craftingCombination3.Unlocked = GUILayout.Toggle(craftingCombination3.Unlocked, "Unlock AIRCRAFT ENGINE PART for player 1 (cheat)");

                Beam.Crafting.CraftingCombination craftingCombination6 = PlayerRegistry.LocalPlayer.Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "AIRCRAFT RUDDER PART");
                craftingCombination6.Unlocked = GUILayout.Toggle(craftingCombination6.Unlocked, "Unlock AIRCRAFT RUDDER PART for player 1 (cheat)");

                Beam.Crafting.CraftingCombination craftingCombination9 = PlayerRegistry.LocalPlayer.Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "AIRCRAFT PROPELLER PART");
                craftingCombination9.Unlocked = GUILayout.Toggle(craftingCombination9.Unlocked, "Unlock AIRCRAFT PROPELLER PART for player 1 (cheat)");

                // Seems enabled by default
                //Beam.Crafting.CraftingCombination craftingCombination2 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "GYROCOPTER FRAME");
                //craftingCombination2.Unlocked = GUILayout.Toggle(craftingCombination2.Unlocked, "Unlock GYROCOPTER FRAME for player 1 (cheat)");

                //Beam.Crafting.CraftingCombination craftingCombination5 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "GYROCOPTER MOTOR");
                //craftingCombination5.Unlocked = GUILayout.Toggle(craftingCombination5.Unlocked, "Unlock GYROCOPTER MOTOR for player 1 (cheat)");

                //Beam.Crafting.CraftingCombination craftingCombination8 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "GYROCOPTER ROTORS");
                //craftingCombination8.Unlocked = GUILayout.Toggle(craftingCombination8.Unlocked, "Unlock GYROCOPTER ROTORS for player 1 (cheat)");

                //if (GUI.Button(new Rect(10, 70, 50, 30), "Give a paddle"))
                //{
                //    Debug.Log("Clicked the button with text = Give a paddle");
                //}

                GUILayout.Label("Crafting level");
                int craftmanshipLevel = PlayerRegistry.LocalPlayer.PlayerSkills.CraftsmanshipLevel;
                craftmanshipLevel = (int)GUILayout.HorizontalSlider(craftmanshipLevel, 0, 7);
                PlayerRegistry.LocalPlayer.PlayerSkills.DebugSetCraftsmanshipLevel(craftmanshipLevel);

                GUILayout.Label("Cooking level");
                int cookingLevel = (int)fi_cookingLevel.GetValue(PlayerRegistry.LocalPlayer.PlayerSkills);
                cookingLevel = (int)GUILayout.HorizontalSlider(cookingLevel, 0, 7);
                PlayerRegistry.LocalPlayer.PlayerSkills.DebugSetCookingLevel(cookingLevel);

                GUILayout.Label("Physical level");
                int physicalLevel = (int)fi_physicalLevel.GetValue(PlayerRegistry.LocalPlayer.PlayerSkills);
                physicalLevel = (int)GUILayout.HorizontalSlider(physicalLevel, 0, 7);
                PlayerRegistry.LocalPlayer.PlayerSkills.DebugSetPhysicalLevel(physicalLevel);

                GUILayout.Label("Hunting level");
                int huntingLevel = (int)fi_huntingLevel.GetValue(PlayerRegistry.LocalPlayer.PlayerSkills);
                huntingLevel = (int)GUILayout.HorizontalSlider(huntingLevel, 0, 7);
                PlayerRegistry.LocalPlayer.PlayerSkills.DebugSetHuntingLevel(huntingLevel);

                GUILayout.Label("Harvesting level");
                int harvestingLevel = (int)fi_harvestingLevel.GetValue(PlayerRegistry.LocalPlayer.PlayerSkills);
                harvestingLevel = (int)GUILayout.HorizontalSlider(harvestingLevel, 0, 7);
                PlayerRegistry.LocalPlayer.PlayerSkills.DebugSetHarvestingLevel(harvestingLevel);
            }

            // Give back coconut after consumable drink
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
            //if (biggerStackSizes)
            //{
            //    IncreaseStackSizes(4);
            //}
            //else
            //{
            //    IncreaseStackSizes(1);
            //}

            if ((Beam.Game.State == GameState.NEW_GAME || Beam.Game.State == GameState.LOAD_GAME)
                && (!permaCompassAlwaysVisible || !permaCompassEnabled))
            {
                compassCanvasVisible = false;
                RefreshCompassCanvas();
            }

            //if (Atmosphere.Instance != null)
            //{
            //    GameTime.Instance.Hours = 4;

            //    FieldInfo fi_latitude = typeof(Atmosphere).GetField("_latitude", BindingFlags.Instance | BindingFlags.NonPublic);
            //    FieldInfo fi_longitude = typeof(Atmosphere).GetField("_longitude", BindingFlags.Instance | BindingFlags.NonPublic);

            //    fi_latitude.SetValue(Atmosphere.Instance, _latitude);
            //    fi_longitude.SetValue(Atmosphere.Instance, _longitude);
            //}
        }

        static int flag = 0;
        static AudioManager audioManagerInstance = null;

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                if (audioManagerInstance == null)
                {
                    AudioManager am = fi_AudioManagerInstance.GetValue(null) as AudioManager;
                    if (am != null
                        && !System.Object.ReferenceEquals(am, audioManagerInstance))
                    {
                        MethodInfo mi_LoadOptions = typeof(AudioManager).GetMethod("LoadOptions");
                        if (mi_LoadOptions != null)
                        {
                            //am.LoadOptions();
                            mi_LoadOptions.Invoke(am, null);
                            Debug.Log("Stranded Deep Tweaks Mod : Audiosettings forced");
                            audioManagerInstance = am;
                        }
                    }
                }

                //if (Beam.Game.State == GameState.INTRO)
                //{
                //    GameObject[] gobjects = Game.FindObjectsOfType<GameObject>();
                //    foreach(GameObject go in gobjects)
                //    {
                //        Debug.Log("Stranded Deep Tweaks Mod : intro GO object name " + go.name);
                //    }
                //}

                if (alwaysSkipIntro
                    && Beam.Game.State == GameState.INTRO)
                {
                    bool canSkipIntro = false;
                    Beam.Intro.PrivateJet privateJet = Beam.Game.FindObjectOfType<Beam.Intro.PrivateJet>();
                    if (privateJet != null)
                    {
                        FieldInfo fi = typeof(Beam.Intro.PrivateJet).GetField("_canSkipIntro", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fi != null)
                        {
                            canSkipIntro = (bool)fi.GetValue(privateJet);
                        }

                        if (canSkipIntro)
                        {
                            MethodInfo mi = typeof(Beam.Intro.PrivateJet).GetMethod("LoadNewGame", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (mi != null)
                            {
                                mi.Invoke(privateJet, null);
                                // performance
                                //alwaysSkipIntro = false;
                            }
                        }
                    }
                }

                //Debug.Log("Stranded Deep Tweaks Mod update step 1 (ms) = " + chrono.ElapsedMilliseconds);

                if (Beam.Game.State == GameState.NEW_GAME || Beam.Game.State == GameState.LOAD_GAME)
                {
                    if (saveAnywhereAllowed)
                    {
                        Event currentevent = Event.current;
                        if (currentevent.isKey)
                        {
                            if (currentevent.keyCode == KeyCode.F7 && !Game.Mode.IsMultiplayer())
                            {
                                if (Game.Mode.IsMaster())
                                {
                                    // Try and save the game
                                    //SaveManager.Save(Beam.Game.Mode, true);
                                    SaveManager.SaveGame(PlayerRegistry.LocalPlayer);
                                }
                            }
                        }
                    }

                    //Debug.Log("Stranded Deep Tweaks Mod update step 2 (ms) = " + chrono.ElapsedMilliseconds);

                    //if (fixRainReset)
                    //{
                    //    if (AtmosphereStorm.Instance != null
                    //        && (AtmosphereStorm.Instance.CurrentWeatherEvent == null
                    //            || !Mathf.Approximately(AtmosphereStorm.Instance.CurrentWeatherEvent.Humidity, 100f))
                    //        && AtmosphereStorm.Instance.Rain > 0)
                    //    {
                    //        Debug.Log("Stranded Deep Tweaks Mod : resetting rain");
                    //        fi_rain.SetValue(AtmosphereStorm.Instance, 0);
                    //    }
                    //}

                    //Debug.Log("Stranded Deep Tweaks Mod update step 3 (ms) = " + chrono.ElapsedMilliseconds);

                    //IPlayer player; player.Holder.CurrentObject is InteractiveObject_AIRTANK

                    //if ((biggerAirTank || infiniteAir) && flag == 0)
                    //{
                    //    if (PlayerRegistry.LocalPlayer != null
                    //        && PlayerRegistry.LocalPlayer.Holder != null
                    //        && PlayerRegistry.LocalPlayer.Holder.CurrentObject != null
                    //        && PlayerRegistry.LocalPlayer.Holder.CurrentObject is InteractiveObject_AIRTANK)
                    //    {
                    //        InteractiveObject_AIRTANK airtank = PlayerRegistry.LocalPlayer.Holder.CurrentObject as InteractiveObject_AIRTANK;

                    //        //Beam.Crafting.InteractiveObject_AIRTANK[] airtanks = Beam.Game.FindObjectsOfType<Beam.Crafting.InteractiveObject_AIRTANK>();
                    //        //if (airtanks != null)
                    //        //{
                    //        //    foreach (Beam.Crafting.InteractiveObject_AIRTANK airtank in airtanks)
                    //        //    {
                    //        if (infiniteAir)
                    //        {
                    //            if (airtank.DurabilityPoints != newCapacity)
                    //            {
                    //                airtank.DurabilityPoints = newCapacity;
                    //                Debug.Log("Stranded Deep Tweaks Mod : refilling infinite airtank");
                    //            }
                    //        }
                    //        else if (biggerAirTank && !handledAirtankReferences.Contains(airtank.ReferenceId))
                    //        {
                    //            //Debug.Log("Stranded Deep Tweaks Mod : on airtank load charges : " + airtank.DurabilityPoints);
                    //            if (!airtank.name.Contains("Bigger"))
                    //            {
                    //                float newCharges = newCapacity - (3 - airtank.DurabilityPoints);
                    //                Debug.Log("Stranded Deep Tweaks Mod : setting airtank max capacity to : " + newCharges + " for reference " + airtank.ReferenceId);
                    //                airtank.DurabilityPoints = newCharges;
                    //                var propertyInfo = typeof(InteractiveObject).GetProperty("OriginalDurabilityPoints", BindingFlags.Public | BindingFlags.Instance);
                    //                if (propertyInfo != null)
                    //                {
                    //                    //Debug.Log("Stranded Deep Tweaks Mod : OriginalDurabilityPoints Property found");
                    //                    MethodInfo mi = propertyInfo.GetSetMethod(true);
                    //                    if (mi != null)
                    //                    {
                    //                        //Debug.Log("Stranded Deep Tweaks Mod : OriginalDurabilityPoints Property private setter found");
                    //                        mi.Invoke(((InteractiveObject)airtank), new object[] { newCharges });
                    //                    }
                    //                }
                    //                //Debug.Log("Stranded Deep Tweaks Mod : airtank OriginalDurabilityPoints : " + airtank.OriginalDurabilityPoints);
                    //                airtank.DisplayName = "Bigger air tank";
                    //            }
                    //            handledAirtankReferences.Add(airtank.ReferenceId);
                    //        }
                    //        //    }
                    //        //}
                    //    }
                    //}

                    //Debug.Log("Stranded Deep Tweaks Mod update step 4 (ms) = " + chrono.ElapsedMilliseconds);

                    //Debug.Log("Stranded Deep Tweaks Mod update step 5 (ms) = " + chrono.ElapsedMilliseconds);

                    if (showDistances)
                    {
                        if (PlayerRegistry.LocalPlayer != null
                            && PlayerRegistry.LocalPlayer.Holder != null
                            && PlayerRegistry.LocalPlayer.Holder.CurrentObject != null
                            && PlayerRegistry.LocalPlayer.Holder.CurrentObject is InteractiveObject_SPYGLASS)
                        {
                            InteractiveObject_SPYGLASS spyglass = PlayerRegistry.LocalPlayer.Holder.CurrentObject as InteractiveObject_SPYGLASS;
                            bool holding = (bool)fi_holding.GetValue(spyglass);
                            //Debug.Log("Stranded Deep Tweaks Mod : is holding spyglass = " + holding);
                            if (!holding)
                            {
                                if (distanceCanvasVisible)
                                {
                                    textCanvas.SetActive(false);
                                }
                                distanceCanvasVisible = false;
                            }
                            else
                            {
                                //IPlayer owner = fi_owner.GetValue(spyglass) as IPlayer;
                                IPlayer owner = PlayerRegistry.LocalPlayer;
                                if (owner != null && owner.Input.GetButton(8))
                                {
                                    //Debug.Log("Stranded Deep Tweaks Mod : spyglass in use");
                                    // show distance
                                    RefreshDistance();
                                }
                                else
                                {
                                    //Debug.Log("Stranded Deep Tweaks Mod : spyglass NOT in use");
                                    // hide distance
                                    if (distanceCanvasVisible)
                                    {
                                        textCanvas.SetActive(false);
                                    }
                                    distanceCanvasVisible = false;
                                }
                            }
                        }
                        else
                        {
                            //Debug.Log("Stranded Deep Tweaks Mod : NO spyglasses hold");
                            // hide distance
                            if (distanceCanvasVisible)
                            {
                                textCanvas.SetActive(false);
                            }
                            distanceCanvasVisible = false;
                        }

                        //    Beam.Crafting.InteractiveObject_SPYGLASS[] spyglasses = Beam.Game.FindObjectsOfType<Beam.Crafting.InteractiveObject_SPYGLASS>();
                        //    if (spyglasses != null)
                        //    {
                        //        //Debug.Log("Stranded Deep Tweaks Mod : spyglasses found");
                        //        foreach (Beam.Crafting.InteractiveObject_SPYGLASS spyglass in spyglasses)
                        //        {
                        //            bool holding = (bool)fi_holding.GetValue(spyglass);
                        //            //Debug.Log("Stranded Deep Tweaks Mod : is holding spyglass = " + holding);
                        //            if (!holding)
                        //            {
                        //                if (distanceCanvasVisible)
                        //                {
                        //                    textCanvas.SetActive(false);
                        //                }
                        //                distanceCanvasVisible = false;
                        //                continue;
                        //            }

                        //            IPlayer owner = fi_owner.GetValue(spyglass) as IPlayer;
                        //            if (owner != null && owner.Input.GetButton(8))
                        //            {
                        //                //Debug.Log("Stranded Deep Tweaks Mod : spyglass in use");
                        //                // afficher la distance
                        //                RefreshDistance();
                        //            }
                        //            else
                        //            {
                        //                //Debug.Log("Stranded Deep Tweaks Mod : spyglass NOT in use");
                        //                // cacher la distance
                        //                if (distanceCanvasVisible)
                        //                {
                        //                    textCanvas.SetActive(false);
                        //                }
                        //                distanceCanvasVisible = false;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //Debug.Log("Stranded Deep Tweaks Mod : NO spyglasses found");
                        //        // cacher la distance
                        //        if (distanceCanvasVisible)
                        //        {
                        //            textCanvas.SetActive(false);
                        //        }
                        //        distanceCanvasVisible = false;
                        //    }
                        //}
                        //else
                        //{
                        //    // cacher la distance
                        //    if (distanceCanvasVisible)
                        //    {
                        //        textCanvas.SetActive(false);
                        //    }
                        //    distanceCanvasVisible = false;
                    }

                    //Debug.Log("Stranded Deep Tweaks Mod update step 6 (ms) = " + chrono.ElapsedMilliseconds);

                    if ((harderbosses || spawnJerrycansAbaiaWreck && !fuelCansSpawned) && StrandedWorld.Instance != null && flag == 2)
                    {
                        foreach (IPlayer player in PlayerRegistry.AllPlayers)
                        {
                            Zone z = StrandedWorld.GetZone(player.transform.position, false);
                            if (z != null
                                && z.Id.Contains("MISSION"))
                            {
                                //Debug.Log("Stranded Deep Tweaks Mod zone name " + z.Id + " / " + z.transform.position);
                                if (harderbosses)
                                {
                                    MakeBossesHarder();
                                }
                                if (spawnJerrycansAbaiaWreck
                                    && !fuelCansSpawned
                                    && z.Id.Contains("INTERNAL_MISSION_0")) // 0 EEL - 3 CARRIER
                                {
                                    // spawn jerrycans in the wreck
                                    uint jerrycanPrefabId = 28;
                                    Create(jerrycanPrefabId, new Vector3(z.transform.position.x - 1f, z.transform.position.y - 15f, z.transform.position.z - 0.5f));
                                    Create(jerrycanPrefabId, new Vector3(z.transform.position.x - 1.2f, z.transform.position.y - 15f, z.transform.position.z - 0.5f));
                                    Debug.Log("Stranded Deep Tweaks Mod : spawning the jerrycan");
                                    fuelCansSpawned = true;

                                    //"x": 939.0896,
                                    //"y": -11.48055,
                                    //"z": 1548.712

                                    //"x": 940.0547,
                                    //"y": -15.08254,
                                    //"z": 1529.036

                                    //island (941.43, 0.00, 1529.41)


                                }
                            }
                        }
                    }

                    //Debug.Log("Stranded Deep Tweaks Mod update step 7 (ms) = " + chrono.ElapsedMilliseconds);

                    //BuoyancyTests();
                    //if (addBuoyancies && flag == 3)
                    //{
                    //    //AddBuoyancies();
                    //    if (buoyancyHandler.MustFillQueue)
                    //    {
                    //        buoyancyHandler.FillQueue(Beam.Game.FindObjectsOfType<InteractiveObject>());
                    //    }
                    //    buoyancyHandler.Handle();
                    //}

                    if ((permaCompassEnabled || permaCompassAlwaysVisible))
                    {
                        try
                        {
                            IPlayer player = PlayerRegistry.LocalPlayer;
                            if (player != null)
                            {
                                bool showCompass = compassCanvasVisible;
                                if (flag == 5)
                                {
                                    showCompass = permaCompassAlwaysVisible;
                                    if (!showCompass)
                                    {
                                        if (player.Holder != null 
                                            && player.Holder.CurrentObject != null
                                            && player.Holder.CurrentObject is InteractiveObject_COMPASS)
                                        {
                                            showCompass = true;
                                        }
                                    }
                                    if (!showCompass && player.Inventory != null)
                                    {
                                        ISlotStorage<IPickupable> inventory = player.Inventory.GetSlotStorage();
                                        foreach (StorageSlot<IPickupable> slot in inventory.GetSlots())
                                        {
                                            if (slot.CraftingType != null
                                                && slot.CraftingType.InteractiveType == InteractiveType.TOOLS_COMPASS)
                                            {
                                                showCompass = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // hide compass if in endgame
                                // bug fix : compass always visible
                                showCompass = showCompass && !player.Movement.IsInCutscene;

                                if (showCompass)
                                {
                                    compassCanvasVisible = true;
                                    Vector3 playerForward = player.transform.forward;
                                    Vector3 trueNorth = new Vector3(1f, 0f, 1f);

                                    float angleToNorth = Vector3.SignedAngle(trueNorth, playerForward, player.transform.up);
                                    if (imgCompass != null)
                                    {
                                        imgCompass.transform.rotation = Quaternion.Euler(0, 0, angleToNorth);
                                    }
                                }
                                else
                                {
                                    compassCanvasVisible = false;
                                }
                                RefreshCompassCanvas();
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Stranded Deep Tweaks Mod : perma compass update error : " + e);
                        }
                    }

                    flag++;

                    if (flag > 5)
                        flag = 0;

                    //Event tempcurrentevent = Event.current;
                    //if (tempcurrentevent.isKey)
                    //{
                    //    if (tempcurrentevent.keyCode == KeyCode.KeypadMinus)
                    //    {
                    //        try
                    //        {
                    //            Vector3 playerPosition = PlayerRegistry.LocalPlayer.transform.position;
                    //            Debug.Log("Stranded Deep Tweaks Mod : FLOCK POSITION TEST playerPosition " + playerPosition);
                    //            Vector3 playerLocalPosition = PlayerRegistry.LocalPlayer.transform.localPosition;
                    //            Debug.Log("Stranded Deep Tweaks Mod : FLOCK POSITION TEST localPosition " + playerLocalPosition);
                    //            int index = 0;
                    //            Zone zone = FindClosestZone(playerPosition, out index);
                    //            Debug.Log("Stranded Deep Tweaks Mod : FLOCK POSITION TEST zone.gameObject.transform.position " + zone.gameObject.transform.position);
                    //            Debug.Log("Stranded Deep Tweaks Mod : FLOCK POSITION TEST delta = " + (playerPosition - zone.gameObject.transform.position));
                    //        }
                    //        catch
                    //        {
                    //            Debug.Log("Stranded Deep Tweaks Mod : error on FLOCK POSITION TEST");
                    //        }
                    //    }
                    //}
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Tweaks Mod : error on update : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep Tweaks Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        private static Zone FindClosestZone(Vector3 position, out int index)
        {
            Zone[] zones = StrandedWorld.Instance.Zones;
            index = -1;
            Zone result = null;
            float num = float.PositiveInfinity;
            for (int i = 0; i < zones.Length; i++)
            {
                float num2 = Vector3.Distance(position, zones[i].transform.position);
                if (num2 < num)
                {
                    num = num2;
                    result = zones[i];
                    index = i;
                }
            }
            return result;
        }

        private static void FixFlocks()
        {
            try
            {
                /*
                 Stranded Deep Tweaks Mod FLOCK : parent = Manager - Animals
                Stranded Deep Tweaks Mod FLOCK : position = (1064.90, 0.00, -139.50)
                Stranded Deep Tweaks Mod FLOCK : _center = (0.00, 7.00, 0.00)
                Stranded Deep Tweaks Mod FLOCK : ControllerPosition = (1064.90, 7.00, -139.50)
                Stranded Deep Tweaks Mod FLOCK : fixed position = (1192.90, 0.00, -11.50)
                Stranded Deep Tweaks Mod FLOCK : fixed ControllerPosition = (1192.90, 7.00, -11.50)
                Stranded Deep Tweaks Mod FLOCK : MaxLandingSpotDistance = 30
                Stranded Deep Tweaks Mod FLOCK : Species Seagull
                Stranded Deep Tweaks Mod FLOCK : Population 15
                Stranded Deep Tweaks Mod FLOCK : Percentage 70
                Stranded Deep Tweaks Mod FLOCK : Species Bat
                Stranded Deep Tweaks Mod FLOCK : Population 15
                Stranded Deep Tweaks Mod FLOCK : Percentage 70
                 */

                foreach (FlockController fc in Game.FindObjectsOfType<FlockController>())
                {
                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : parent = " + fc.transform.parent.name);
                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : position = " + fc.transform.position);
                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : _center = " + typeof(FlockController).GetField("_center", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fc));
                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : ControllerPosition = " + fc.ControllerPosition);
                    if (WorldUtilities.IsStrandedWide())
                    {
                        Debug.Log("Stranded Deep Tweaks Mod : Birds everywhere fix : adding Stranded Wide offset");
                        fc.transform.position = fc.transform.position + new Vector3(128, 0, 128);
                    }
                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : fixed position = " + fc.transform.position);
                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : fixed ControllerPosition = " + fc.ControllerPosition);

                    //Debug.Log("Stranded Deep Tweaks Mod FLOCK : MaxLandingSpotDistance = " + fc.MaxLandingSpotDistance);
                    foreach (FlockSpecies fs in fc.AvailableFlocks.Keys)
                    {
                        FlockData fd = fc.AvailableFlocks[fs];
                        //Debug.Log("Stranded Deep Tweaks Mod FLOCK : Species " + fd.Species);
                        //Debug.Log("Stranded Deep Tweaks Mod FLOCK : Population " + fd.Population);
                        //Debug.Log("Stranded Deep Tweaks Mod FLOCK : Percentage " + fd.Percentage);
                    }
                }
            }
            catch { }
        }

        static bool fuelCansSpawned = false;

        public static void Create(uint prefabId, Vector3 position, Transform parent = null)
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

        private static void Reset()
        {
            buoyancyHandler.Reset();

            CurrentDamageCause = DamageCause.None;

            imgCompass = null;
            imgNeedle = null;
            compassCanvas = null;
            compassCanvasVisible = false;
        }

        private static void ReduceTurningAngles()
        {
            try
            {
                return;

                VehicleMovementBase[] vms = Game.FindObjectsOfType<VehicleMovementBase>();

                foreach (VehicleMovementBase vm in vms)
                {
                    if (vm is RudderVehicleMovement)
                    {
                        //FieldInfo fi_rudderTurningAngle = typeof(RudderVehicleMovement).GetField("_rudderTurningAngle", BindingFlags.NonPublic | BindingFlags.Instance);
                        fi_rudderTurningAngle.SetValue(vm, 10.0f);
                        //FieldInfo fi_rudderDeflection = typeof(RudderVehicleMovement).GetField("_rudderDeflection", BindingFlags.NonPublic | BindingFlags.Instance);
                        fi_rudderDeflection.SetValue(vm, 10.0f);
                        //Debug.Log("Stranded Deep Tweaks Mod : RudderVehicleMovement updated");
                    }
                    else if (vm is MotorVehicleMovement)
                    {
                        //FieldInfo fi_propellerTurningAngle = typeof(MotorVehicleMovement).GetField("_propellerTurningAngle", BindingFlags.NonPublic | BindingFlags.Instance);
                        fi_propellerTurningAngle.SetValue(vm, 10.0f);
                        //FieldInfo fi_propellerRudderDeflection = typeof(MotorVehicleMovement).GetField("_rudderDeflection", BindingFlags.NonPublic | BindingFlags.Instance);
                        fi_propellerRudderDeflection.SetValue(vm, 10.0f);
                        //Debug.Log("Stranded Deep Tweaks Mod : MotorVehicleMovement updated");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Tweaks Mod : error on ReduceTurningAngles : " + e);
            }
        }

        private static void AddBuoyancies()
        {
            try
            {
                InteractiveObject[] ios = Beam.Game.FindObjectsOfType<InteractiveObject>();
                foreach (InteractiveObject io in ios)
                {
                    //Debug.Log("Stranded Deep Tweaks Mod : " + io.name);
                    Buoyancy buoy = io.GetComponent<Buoyancy>();
                    if (buoy == null
                        && (io.name.Contains("STICK")
                        || io.name.Contains("COCONUT")
                        || io.name.Contains("SCRAP_PLANK")
                        || io.name.Contains("CONTAINER_CRATE")
                        || io.name.Contains("FROND")
                        || io.name.Contains("PALM_TOP")
                        || io.name.Contains("PALM_LOG")
                        || io.name.Contains("PADDLE")
                        || io.name.Contains("FUELCAN")
                        || io.name.Contains("LEAVES_FIBROUS")
                        || io.name.Contains("WOLLIE")
                        || io.name.Contains("COCONUT_FLASK")
                        || io.name.Contains("MEDICAL")))
                    {
                        if (io.name.Contains("CONTAINER_CRATE")
                            && io is Interactive_STORAGE)
                        {
                            Interactive_STORAGE istor = io as Interactive_STORAGE;
                            //Debug.Log("Stranded Deep Tweaks Mod : CRATE spawnItemLoot = " + fi_LootSpawned.GetValue(istor));
                            if (!(bool)fi_LootSpawned.GetValue(istor))
                            {
                                continue;
                            }
                        }

                        io.gameObject.SetActive(false);
                        Debug.Log("Stranded Deep Tweaks Mod : adding buoyancy to " + io.name);
                        Buoyancy newbuoy = io.gameObject.AddComponent<Buoyancy>();
                        fi_buoyancyDensity.SetValue(newbuoy, 600.0f);
                        Rigidbody rb = fi_rigidbody.GetValue(newbuoy) as Rigidbody;
                        if (rb == null)
                        {
                            //Debug.Log("Stranded Deep Tweaks Mod : buoyancy rigidbody is null");
                            fi_rigidbody.SetValue(newbuoy, io.rigidbody);
                        }
                        io.gameObject.SetActive(true);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Tweaks Mod : error on AddBuoyancies : " + e);
            }
        }

        private static void MakeSharksHarder()
        {
            
        }

        //_normalAttackDamage
        private static FieldInfo fi_normalAttackDamage = typeof(Piscus_Creature).GetField("_normalAttackDamage", BindingFlags.NonPublic | BindingFlags.Instance);
        //_normalAttackCritDamage
        private static FieldInfo fi_normalAttackCritDamage = typeof(Piscus_Creature).GetField("_normalAttackCritDamage", BindingFlags.NonPublic | BindingFlags.Instance);

        //_attackDamage
        private static FieldInfo fi_attackDamage = typeof(Piscus_Creature).GetField("_attackDamage", BindingFlags.NonPublic | BindingFlags.Instance);
        //_attackCritDamage
        private static FieldInfo fi_attackCritDamage = typeof(Piscus_Creature).GetField("_attackCritDamage", BindingFlags.NonPublic | BindingFlags.Instance);

        //_minExploreDepth
        private static FieldInfo fi_minExploreDepth = typeof(Piscus_Creature).GetField("_minExploreDepth", BindingFlags.NonPublic | BindingFlags.Instance);
        //_maxExploreDepth
        private static FieldInfo fi_maxExploreDepth = typeof(Piscus_Creature).GetField("_maxExploreDepth", BindingFlags.NonPublic | BindingFlags.Instance);
        
        //_fleeTimer
        private static FieldInfo fi_fleetimer = typeof(Piscus_Creature).GetField("_fleeTimer", BindingFlags.NonPublic | BindingFlags.Instance);
        //_avoidanceRayDistance
        private static FieldInfo fi_avoidanceRayDistance = typeof(Piscus_Creature).GetField("_avoidanceRayDistance", BindingFlags.NonPublic | BindingFlags.Instance);
        //_avoidanceRayWidth
        private static FieldInfo fi_avoidanceRayWidth = typeof(Piscus_Creature).GetField("_avoidanceRayWidth", BindingFlags.NonPublic | BindingFlags.Instance);

        //_interactiveObject
        private static FieldInfo fi_interactiveObject = typeof(Piscus_Creature).GetField("_interactiveObject", BindingFlags.NonPublic | BindingFlags.Instance);

        //OriginalHealthPoints
        private static PropertyInfo pi_OriginalHealthPoints = typeof(BaseObject).GetProperty("OriginalHealthPoints", BindingFlags.NonPublic | BindingFlags.Instance);

        private static Piscus_Creature currentBoss = null;
        private static Boss_Squid_Tentacle currentTentacle = null;

        private static bool initEel = false;
        private static bool initMeg = false;
        private static bool initSquid = false;
        private static bool initTentacle = false;
        private static float newMinExploreDepth = -50;//-10
        private static int newavoidanceRayDistance = 0;//6
        private static int newavoidanceRayWidth = 0;//6

        private static void MakeBossesHarder()
        {
            foreach (IPlayer player in PlayerRegistry.AllPlayers)
            {
                player.Statistics.Damaged -= Statistics_Damaged;
                player.Statistics.Damaged += Statistics_Damaged;
            }

            Boss_Eel[] eels = Beam.Game.FindObjectsOfType<Boss_Eel>();
            if (eels != null)
            {
                foreach (Boss_Eel eel in eels)
                {
                    currentBoss = eel;
                    UnFlee(eel);
                    if (currentBoss.CurrentState is Piscus_Creature.PiscusBehaviour
                        && ((Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackJump
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackPlayer
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackingPlayer
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackingPlayer))
                    {
                        CurrentDamageCause = DamageCause.None;
                    }
                    if (initEel)
                        return;

                    Debug.Log("Stranded Deep Tweaks Mod : eel BiomeSize : " + eel.BiomeSize);
                    eel.BiomeSize = 300; //200
                    Debug.Log("Stranded Deep Tweaks Mod : eel new BiomeSize : " + eel.BiomeSize);

                    Debug.Log("Stranded Deep Tweaks Mod : eel _minExploreDepth : " + fi_minExploreDepth.GetValue(eel));
                    fi_minExploreDepth.SetValue(eel, newMinExploreDepth); // -10
                    Debug.Log("Stranded Deep Tweaks Mod : eel new _minExploreDepth : " + fi_minExploreDepth.GetValue(eel));

                    Debug.Log("Stranded Deep Tweaks Mod : eel _avoidanceRayDistance : " + fi_avoidanceRayDistance.GetValue(eel));
                    fi_avoidanceRayDistance.SetValue(eel, newavoidanceRayDistance); // 6
                    Debug.Log("Stranded Deep Tweaks Mod : eel new_avoidanceRayDistance : " + fi_avoidanceRayDistance.GetValue(eel));

                    Debug.Log("Stranded Deep Tweaks Mod : eel _avoidanceRayWidth : " + fi_avoidanceRayWidth.GetValue(eel));
                    fi_avoidanceRayWidth.SetValue(eel, newavoidanceRayWidth); // 6
                    Debug.Log("Stranded Deep Tweaks Mod : eel new_avoidanceRayWidth : " + fi_avoidanceRayWidth.GetValue(eel));

                    if (hardcorebosses)
                    {
                        //200
                        Debug.Log("Stranded Deep Tweaks Mod : eel _attackDamage : " + fi_attackDamage.GetValue(eel));
                        fi_attackDamage.SetValue(eel, 350);
                        Debug.Log("Stranded Deep Tweaks Mod : eel new _attackDamage : " + fi_attackDamage.GetValue(eel));

                        //300
                        Debug.Log("Stranded Deep Tweaks Mod : eel _attackCritDamage : " + fi_attackCritDamage.GetValue(eel));
                        fi_attackCritDamage.SetValue(eel, 450);
                        Debug.Log("Stranded Deep Tweaks Mod : eel new _attackCritDamage : " + fi_attackCritDamage.GetValue(eel));
                    }
                    InteractiveObject io = fi_interactiveObject.GetValue(eel) as InteractiveObject;
                    if (io != null)
                    {
                        // 750
                        Debug.Log("Stranded Deep Tweaks Mod : eel health : " + io.HealthPoints);
                        float vanillaHP = 750;
                        if ((float)pi_OriginalHealthPoints.GetValue(io) == vanillaHP)
                        {
                            pi_OriginalHealthPoints.SetValue(io, 2 * vanillaHP);
                            io.HealthPoints = 2 * vanillaHP;
                        }
                        Debug.Log("Stranded Deep Tweaks Mod : eel new health : " + io.HealthPoints);
                    }
                    eel.Attacked -= Boss_Attacked;
                    eel.Attacked += Boss_Attacked;
                    eel.Jumped -= Boss_Jumped;
                    eel.Jumped += Boss_Jumped;
                    initEel = true;
                }
            }

            Boss_Shark[] megalodons = Beam.Game.FindObjectsOfType<Boss_Shark>();
            if (megalodons != null)
            {
                foreach (Boss_Shark megalodon in megalodons)
                {
                    currentBoss = megalodon;
                    UnFlee(megalodon);
                    if (currentBoss.CurrentState is Piscus_Creature.PiscusBehaviour
                        && ((Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackJump
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackPlayer
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackingPlayer
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackingPlayer))
                    {
                        CurrentDamageCause = DamageCause.None;
                    }
                    if (initMeg)
                        return;

                    Debug.Log("Stranded Deep Tweaks Mod : megalodon BiomeSize : " + megalodon.BiomeSize);
                    megalodon.BiomeSize = 300; //200
                    Debug.Log("Stranded Deep Tweaks Mod : megalodon new BiomeSize : " + megalodon.BiomeSize);

                    Debug.Log("Stranded Deep Tweaks Mod : megalodon _minExploreDepth : " + fi_minExploreDepth.GetValue(megalodon));
                    fi_minExploreDepth.SetValue(megalodon, newMinExploreDepth); // -10
                    Debug.Log("Stranded Deep Tweaks Mod : megalodon new _minExploreDepth : " + fi_minExploreDepth.GetValue(megalodon));

                    Debug.Log("Stranded Deep Tweaks Mod : megalodon _avoidanceRayDistance : " + fi_avoidanceRayDistance.GetValue(megalodon));
                    fi_avoidanceRayDistance.SetValue(megalodon, newavoidanceRayDistance); // 6
                    Debug.Log("Stranded Deep Tweaks Mod : megalodon new_avoidanceRayDistance : " + fi_avoidanceRayDistance.GetValue(megalodon));

                    Debug.Log("Stranded Deep Tweaks Mod : megalodon _avoidanceRayWidth : " + fi_avoidanceRayWidth.GetValue(megalodon));
                    fi_avoidanceRayWidth.SetValue(megalodon, newavoidanceRayWidth); // 6
                    Debug.Log("Stranded Deep Tweaks Mod : megalodon new_avoidanceRayWidth : " + fi_avoidanceRayWidth.GetValue(megalodon));

                    if (hardcorebosses)
                    {
                        //100
                        Debug.Log("Stranded Deep Tweaks Mod : megalodon _attackDamage : " + fi_attackDamage.GetValue(megalodon));
                        fi_attackDamage.SetValue(megalodon, 250);
                        Debug.Log("Stranded Deep Tweaks Mod : megalodon new _attackDamage : " + fi_attackDamage.GetValue(megalodon));
                        //100
                        Debug.Log("Stranded Deep Tweaks Mod : megalodon _attackCritDamage : " + fi_attackCritDamage.GetValue(megalodon));
                        fi_attackCritDamage.SetValue(megalodon, 250);
                        Debug.Log("Stranded Deep Tweaks Mod : megalodon new _attackCritDamage : " + fi_attackCritDamage.GetValue(megalodon));
                    }
                    InteractiveObject io = fi_interactiveObject.GetValue(megalodon) as InteractiveObject;
                    if (io != null)
                    {
                        //500
                        Debug.Log("Stranded Deep Tweaks Mod : megalodon health : " + io.HealthPoints);
                        float vanillaHP = 500;
                        if ((float)pi_OriginalHealthPoints.GetValue(io) == vanillaHP)
                        {
                            pi_OriginalHealthPoints.SetValue(io, 2 * vanillaHP);
                            io.HealthPoints = 2 * vanillaHP;
                        }
                        Debug.Log("Stranded Deep Tweaks Mod : megalodon new health : " + io.HealthPoints);
                    }
                    megalodon.Attacked -= Boss_Attacked;
                    megalodon.Attacked += Boss_Attacked;
                    megalodon.Jumped -= Boss_Jumped;
                    megalodon.Jumped += Boss_Jumped;
                    initMeg = true;
                }
            }
            Boss_Squid[] squids = Beam.Game.FindObjectsOfType<Boss_Squid>();
            if (squids != null)
            {
                foreach (Boss_Squid squid in squids)
                {
                    currentBoss = squid;
                    UnFlee(squid);
                    if (currentBoss.CurrentState is Piscus_Creature.PiscusBehaviour
                        && ((Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackJump
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackPlayer
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackingPlayer
                            && (Piscus_Creature.PiscusBehaviour)currentBoss.CurrentState != Piscus_Creature.PiscusBehaviour.AttackingPlayer))
                    {
                        CurrentDamageCause = DamageCause.None;
                    }
                    if (initSquid)
                        return;

                    Debug.Log("Stranded Deep Tweaks Mod : squid BiomeSize : " + squid.BiomeSize);
                    squid.BiomeSize = 300; //200
                    Debug.Log("Stranded Deep Tweaks Mod : squid new BiomeSize : " + squid.BiomeSize);

                    Debug.Log("Stranded Deep Tweaks Mod : squid _minExploreDepth : " + fi_minExploreDepth.GetValue(squid));
                    fi_minExploreDepth.SetValue(squid, newMinExploreDepth); // -10
                    Debug.Log("Stranded Deep Tweaks Mod : squid new _minExploreDepth : " + fi_minExploreDepth.GetValue(squid));

                    Debug.Log("Stranded Deep Tweaks Mod : squid _avoidanceRayDistance : " + fi_avoidanceRayDistance.GetValue(squid));
                    fi_avoidanceRayDistance.SetValue(squid, newavoidanceRayDistance); // 6
                    Debug.Log("Stranded Deep Tweaks Mod : squid new_avoidanceRayDistance : " + fi_avoidanceRayDistance.GetValue(squid));

                    Debug.Log("Stranded Deep Tweaks Mod : squid _avoidanceRayWidth : " + fi_avoidanceRayWidth.GetValue(squid));
                    fi_avoidanceRayWidth.SetValue(squid, newavoidanceRayWidth); // 6
                    Debug.Log("Stranded Deep Tweaks Mod : squid new_avoidanceRayWidth : " + fi_avoidanceRayWidth.GetValue(squid));

                    if (hardcorebosses)
                    {
                        //100
                        Debug.Log("Stranded Deep Tweaks Mod : squid _attackDamage : " + fi_attackDamage.GetValue(squid));
                        fi_attackDamage.SetValue(squid, 250);
                        Debug.Log("Stranded Deep Tweaks Mod : squid new _attackDamage : " + fi_attackDamage.GetValue(squid));
                        //200
                        Debug.Log("Stranded Deep Tweaks Mod : squid _attackCritDamage : " + fi_attackCritDamage.GetValue(squid));
                        fi_attackCritDamage.SetValue(squid, 350);
                        Debug.Log("Stranded Deep Tweaks Mod : squid new _attackCritDamage : " + fi_attackCritDamage.GetValue(squid));
                    }
                    InteractiveObject io = fi_interactiveObject.GetValue(squid) as InteractiveObject;
                    if (io != null)
                    {
                        // 1000
                        Debug.Log("Stranded Deep Tweaks Mod : squid health : " + io.HealthPoints);
                        float vanillaHP = 1000;
                        if ((float)pi_OriginalHealthPoints.GetValue(io) == vanillaHP)
                        {
                            pi_OriginalHealthPoints.SetValue(io, 2 * vanillaHP);
                            io.HealthPoints = 2 * vanillaHP;
                        }
                        Debug.Log("Stranded Deep Tweaks Mod : squid new health : " + io.HealthPoints);
                    }
                    squid.Attacked -= Boss_Attacked;
                    squid.Attacked += Boss_Attacked;
                    squid.EnteredTargetProximity.RemoveListener(squid_EnteredTargetProximity);
                    squid.EnteredTargetProximity.AddListener(squid_EnteredTargetProximity);
                    initSquid = true;
                }
            }

            Boss_Squid_Tentacle[] squidtentacles = Beam.Game.FindObjectsOfType<Boss_Squid_Tentacle>();
            if (squidtentacles != null)
            {
                foreach (Boss_Squid_Tentacle squidtentacle in squidtentacles)
                {
                    currentTentacle = squidtentacle;
                    if (initTentacle)
                        break;

                    squidtentacle.Attacked.RemoveListener(squidtentacle_Attacked);
                    squidtentacle.Attacked.AddListener(squidtentacle_Attacked);

                    if (hardcorebosses)
                    {
                        squidtentacle.AttackedGrabbed.RemoveListener(squidtentacle_AttackedGrabbed);
                        squidtentacle.AttackedGrabbed.AddListener(squidtentacle_AttackedGrabbed);
                    }

                    initTentacle = true;
                }
            }
        }

        enum DamageCause
        {
            None,
            MegalodonJump,
            MegalodonBite,
            MegalodonAttack,
            EelJump,
            EelBite,
            EelAttack,
            SquidTentacleSlap,
            SquidTentacleGrab,
            SquidBite,
            SquidAttack
        }

        static DamageCause CurrentDamageCause = DamageCause.None;
        private static FieldInfo fi_currentTarget = typeof(Piscus_Creature).GetField("_currentTarget", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void Statistics_Damaged()
        {
            Debug.Log("Stranded Deep Tweaks Mod : boss damaged player, cause = " + CurrentDamageCause);
            switch (CurrentDamageCause)
            {
                case DamageCause.MegalodonAttack :
                case DamageCause.SquidAttack:
                    {
                        bool bleed = (random.Next(0, 100) > (hardcorebosses ? 50 : 70));

                        if (!currentBoss.CurrentTarget.Statistics.HasStatusEffect(StatusEffect.BLEEDING))
                            currentBoss.CurrentTarget.Statistics.ApplyStatusEffect(StatusEffect.BLEEDING, false);

                        break;
                    }
                case DamageCause.EelAttack:
                case DamageCause.EelJump:
                case DamageCause.EelBite:
                    {
                        bool poison = (random.Next(0, 100) > (hardcorebosses ? 50 : 75));

                        if (poison && !currentBoss.CurrentTarget.Statistics.HasStatusEffect(StatusEffect.POISON))
                        {
                            currentBoss.CurrentTarget.Statistics.ApplyStatusEffect(StatusEffect.POISON, false);
                        }

                        break;
                    }
                case DamageCause.SquidTentacleGrab:
                    {
                        bool bleed = (random.Next(0, 100) > (hardcorebosses ? 50 : 70));

                        if (bleed && !currentBoss.CurrentTarget.Statistics.HasStatusEffect(StatusEffect.BROKEN_BONES))
                            currentBoss.CurrentTarget.Statistics.ApplyStatusEffect(StatusEffect.BROKEN_BONES, false);

                        break;
                    }
                case DamageCause.SquidTentacleSlap:
                    {
                        bool breakBones = (random.Next(0, 100) > (hardcorebosses ? 70 : 85));

                        if (breakBones && !currentBoss.CurrentTarget.Statistics.HasStatusEffect(StatusEffect.BROKEN_BONES))
                            currentBoss.CurrentTarget.Statistics.ApplyStatusEffect(StatusEffect.BROKEN_BONES, false);

                        break;
                    }
                case DamageCause.MegalodonJump:
                    {
                        bool breakBones = (random.Next(0, 100) > (hardcorebosses ? 60 : 80));

                        if (breakBones && !currentBoss.CurrentTarget.Statistics.HasStatusEffect(StatusEffect.BROKEN_BONES))
                            currentBoss.CurrentTarget.Statistics.ApplyStatusEffect(StatusEffect.BROKEN_BONES, false);

                        break;
                    }
                case DamageCause.None:
                default: break;
            }
            CurrentDamageCause = DamageCause.None;
        }

        private static void UnFlee(Piscus_Creature currentCreature)
        {
            //Debug.Log("Stranded Deep Tweaks Mod : current boss behavior : " + currentCreature.CurrentState);

            if (!hardcorebosses || currentCreature == null)
                return;

            float fleeTimer = (float)fi_fleetimer.GetValue(currentCreature);
            if (fleeTimer != 0)
            {
                Debug.Log("Stranded Deep Tweaks Mod : boss un-flee current timer = " + fleeTimer);
                // bosses don't flee
                IPlayer currentTarget = fi_currentTarget.GetValue(currentCreature) as IPlayer;
                if (currentTarget == null && random.Next(0, 100) > (hardcorebosses ? 70 : 90))
                {
                    Debug.Log("Stranded Deep Tweaks Mod : boss continue attack");
                    fi_fleetimer.SetValue(currentCreature, 100f);
                    Debug.Log("Stranded Deep Tweaks Mod : boss un-flee new timer value : " + fi_fleetimer.GetValue(currentCreature));
                    currentCreature.CurrentState = Piscus_Creature.PiscusBehaviour.CircleTarget;
                }
                else
                {
                    Debug.Log("Stranded Deep Tweaks Mod : boss un-flee return explore");
                    currentCreature.CurrentState = Piscus_Creature.PiscusBehaviour.Explore;
                }
            }
        }

        private static MethodInfo mi_Attempt_Tentacle_GrabAttack = typeof(Boss_Squid).GetMethod("Attempt_Tentacle_GrabAttack", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void squid_EnteredTargetProximity()
        {
            if (currentBoss == null || !(currentBoss is Boss_Squid))
                return;

            mi_Attempt_Tentacle_GrabAttack.Invoke(currentBoss, null);
        }

        private static FieldInfo fi_bossSquid = typeof(Boss_Squid_Tentacle).GetField("_bossSquid", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void squidtentacle_Attacked()
        {
            CurrentDamageCause = DamageCause.None;

            if (currentTentacle == null)
                return;

            Boss_Squid squid = fi_bossSquid.GetValue(currentTentacle) as Boss_Squid;
            if (squid == null)
                return;

            IPlayer currentTarget = fi_currentTarget.GetValue(squid) as IPlayer;
            if (currentTarget == null)
                return;

            CurrentDamageCause = DamageCause.SquidTentacleSlap;
        }

        private static void squidtentacle_AttackedGrabbed()
        {
            CurrentDamageCause = DamageCause.None;

            if (currentTentacle == null)
                return;

            Boss_Squid squid = fi_bossSquid.GetValue(currentTentacle) as Boss_Squid;
            if (squid == null)
                return;

            IPlayer currentTarget = fi_currentTarget.GetValue(squid) as IPlayer;
            if (currentTarget == null)
                return;

            CurrentDamageCause = DamageCause.SquidTentacleGrab;
        }

        private static void Boss_Attacked()
        {
            CurrentDamageCause = DamageCause.None;

            if (currentBoss == null)
                return;

            IPlayer currentTarget = fi_currentTarget.GetValue(currentBoss) as IPlayer;
            if (currentTarget == null)
                return;

            if (currentBoss is Boss_Eel)
            {
                CurrentDamageCause = DamageCause.EelAttack;
            }
            else if (currentBoss is Boss_Shark)
            {
                CurrentDamageCause = DamageCause.MegalodonAttack;
            }
            else if (currentBoss is Boss_Squid)
            {
                CurrentDamageCause = DamageCause.SquidAttack;
            }
        }

        private static void Boss_Jumped()
        {
            CurrentDamageCause = DamageCause.None;

            if (currentBoss == null)
                return;
            IPlayer currentTarget = fi_currentTarget.GetValue(currentBoss) as IPlayer;
            if (currentTarget == null)
                return;

            if (currentBoss is Boss_Eel)
            {
                CurrentDamageCause = DamageCause.EelJump;
            }
            else if (currentBoss is Boss_Shark)
            {
                CurrentDamageCause = DamageCause.MegalodonJump;
            }
        }

        private static void BuoyancyTests()
        {
            try
            {
                Buoyancy[] bos = Beam.Game.FindObjectsOfType<Buoyancy>();
                foreach (Buoyancy bo in bos)
                {
                    Debug.Log("Stranded Deep Tweaks Mod : Buoyancy tests : " + bo.gameObject.name);
                    Debug.Log("Stranded Deep Tweaks Mod : Buoyancy tests parent : " + bo.gameObject.GetType().Name);
                    Debug.Log("Stranded Deep Tweaks Mod : Buoyancy tests density : " + fi_buoyancyDensity.GetValue(bo));
                }
                InteractiveObject[] ios = Beam.Game.FindObjectsOfType<InteractiveObject>();
                foreach (InteractiveObject io in ios)
                {
                    Buoyancy buoy = io.GetComponent<Buoyancy>();
                    if (buoy != null)
                    {
                        Debug.Log("Stranded Deep Tweaks Mod : Buoyancy tests : " + buoy.gameObject.name);
                        Debug.Log("Stranded Deep Tweaks Mod : Buoyancy tests density : " + fi_buoyancyDensity.GetValue(buoy));
                    }
                    Debug.Log("Stranded Deep Tweaks Mod : Density tests : " + io.gameObject.name);
                    Debug.Log("Stranded Deep Tweaks Mod : Density tests mass : " + io.rigidbody.useGravity);
                    Debug.Log("Stranded Deep Tweaks Mod : Density tests mass : " + io.rigidbody.mass);
                }

                //Stranded Deep Tweaks Mod : Buoyancy tests : BARREL(Clone)
                //Stranded Deep Tweaks Mod : Buoyancy tests density: 500
                //Stranded Deep Tweaks Mod : Density tests : BARREL(Clone)
                //Stranded Deep Tweaks Mod : Density tests mass: True
                //Stranded Deep Tweaks Mod: Density tests mass: 4
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep Tweaks Mod : BuoyancyTests error : " + e);
            }
        }

        private static void ReadConfig()
        {
            Debug.Log("Stranded Deep Tweaks Mod : config file directory : " + FilePath.SAVE_FOLDER);
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
                            if (tokens[0].Contains("biggerAirTank"))
                            {
                                biggerAirTank = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("infiniteAir"))
                            {
                                infiniteAir = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("biggerGasTank"))
                            {
                                biggerGasTank = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("infiniteGas"))
                            {
                                infiniteGas = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("showDistances"))
                            {
                                showDistances = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("alwaysSkipIntro"))
                            {
                                alwaysSkipIntro = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("harderbosses"))
                            {
                                harderbosses = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("hardcorebosses"))
                            {
                                hardcorebosses = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("fixRainReset"))
                            {
                                fixRainReset = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("biggerStackSizes"))
                            {
                                biggerStackSizes = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("stackSizeRatio"))
                            {
                                stackSizeRatio = int.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("fixAudioReset"))
                            {
                                fixAudioReset = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("addBuoyancies"))
                            {
                                addBuoyancies = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("smallerRaftTurnAngle"))
                            {
                                smallerRaftTurnAngle = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("fixItemWeigths"))
                            {
                                fixItemWeigths = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("permaCompassEnabled"))
                            {
                                permaCompassEnabled = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("permaCompassAlwaysVisible"))
                            {
                                permaCompassAlwaysVisible = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("saveAnywhereAllowed"))
                            {
                                saveAnywhereAllowed = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("betterSpyglass"))
                            {
                                betterSpyglass = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("fixRainStart"))
                            {
                                fixRainStart = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("betterRainTextures"))
                            {
                                betterRainTextures = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("fixBirdsEverywhere"))
                            {
                                fixBirdsEverywhere = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("stopMissedAchievementsSpam"))
                            {
                                stopMissedAchievementsSpam = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("fixFlyingPigs"))
                            {
                                fixFlyingPigs = bool.Parse(tokens[1]);
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
                sb.AppendLine("biggerAirTank=" + biggerAirTank + ";");
                sb.AppendLine("infiniteAir=" + infiniteAir + ";");
                sb.AppendLine("biggerGasTank=" + biggerGasTank + ";");
                sb.AppendLine("infiniteGas=" + infiniteGas + ";");
                sb.AppendLine("showDistances=" + showDistances + ";");
                sb.AppendLine("alwaysSkipIntro=" + alwaysSkipIntro + ";");
                sb.AppendLine("harderbosses=" + harderbosses + ";");
                sb.AppendLine("hardcorebosses=" + hardcorebosses + ";");
                sb.AppendLine("fixRainReset=" + fixRainReset + ";");
                sb.AppendLine("biggerStackSizes=" + biggerStackSizes + ";");
                sb.AppendLine("stackSizeRatio=" + stackSizeRatio + ";");
                sb.AppendLine("fixAudioReset=" + fixAudioReset + ";");
                sb.AppendLine("addBuoyancies=" + addBuoyancies + ";");
                sb.AppendLine("smallerRaftTurnAngle=" + smallerRaftTurnAngle + ";");
                sb.AppendLine("fixItemWeigths=" + fixItemWeigths + ";");
                sb.AppendLine("permaCompassEnabled=" + permaCompassEnabled + ";");
                sb.AppendLine("permaCompassAlwaysVisible=" + permaCompassAlwaysVisible + ";");
                sb.AppendLine("saveAnywhereAllowed=" + saveAnywhereAllowed + ";");
                sb.AppendLine("betterSpyglass=" + betterSpyglass + ";");
                sb.AppendLine("fixRainStart=" + fixRainStart + ";");
                sb.AppendLine("betterRainTextures=" + betterRainTextures + ";");
                sb.AppendLine("fixBirdsEverywhere=" + fixBirdsEverywhere + ";");
                sb.AppendLine("stopMissedAchievementsSpam=" + stopMissedAchievementsSpam + ";");
                sb.AppendLine("fixFlyingPigs=" + fixFlyingPigs + ";");


                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        #region Compass

        private static bool compassCanvasVisible = false;
        private static GameObject compassCanvas;
        private static Image imgCompass;
        private static Image imgNeedle;

        private static void RefreshCompassCanvas()
        {
            if (compassCanvas == null)
            {
                compassCanvas = createCanvas(false, "CompassCanvas");

                GameObject compassGO = new GameObject("Compass_Sprite");
                imgCompass = compassGO.AddComponent<Image>();
                compassGO.transform.SetParent(compassCanvas.transform);
                Texture2D texCompass = new Texture2D(450, 450, TextureFormat.ARGB32, false);
                texCompass.LoadImage(ExtractResource("StrandedDeepTweaksMod.assets.Textures.compass2.png"));
                Sprite compassSprite = Sprite.Create(texCompass, new Rect(0, 0, 450, 450), new Vector2(225, 225));
                imgCompass.sprite = compassSprite;
                imgCompass.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
                imgCompass.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                imgCompass.rectTransform.localPosition = new Vector3(-450, 250, 0);

                GameObject needleGO = new GameObject("Needle_Sprite");
                imgNeedle = needleGO.AddComponent<Image>();
                needleGO.transform.SetParent(compassCanvas.transform);
                Texture2D texNeedle = new Texture2D(450, 450, TextureFormat.ARGB32, false);
                texNeedle.LoadImage(ExtractResource("StrandedDeepTweaksMod.assets.Textures.needle.png"));
                Sprite needleSprite = Sprite.Create(texNeedle, new Rect(0, 0, 450, 450), new Vector2(225, 225));
                imgNeedle.sprite = needleSprite;
                imgNeedle.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
                imgNeedle.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                imgNeedle.rectTransform.localPosition = new Vector3(-450, 250, 0);

                compassCanvas.SetActive(compassCanvasVisible);
            }
            else
            {
                compassCanvas.SetActive(compassCanvasVisible);
            }
        }

        #endregion

        #region Distance

        private static void RefreshDistance()
        {
            RefreshDistanceCanvas();

            // MOST IMPORTANT SECTION : which object are we looking at
            if (Beam.Game.State == GameState.NEW_GAME
                || Beam.Game.State == GameState.LOAD_GAME)
            {
                if (Camera.main != null)
                {
                    //Debug.Log("Stranded Deep 2K Mod : debug objects loop");

                    // Get the ray going through the GUI position
                    Ray r = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                    // Do a raycast
                    RaycastHit hit;
                    string text = null;
                    if (Physics.Raycast(r, out hit))
                    //if (Physics.RaycastAll(r, out hit))
                    {
                        if (hit.transform != null
                            && hit.transform.gameObject != null
                            && !String.IsNullOrEmpty(hit.transform.gameObject.name)
                            && String.Compare(hit.transform.gameObject.name, "Terrain") == 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(String.Format("Distance = {0:F1}m", hit.distance));
                            //sb.AppendLine("I'm looking at " + hit.transform.name + " / distance = " + hit.distance);
                            //sb.AppendLine(hit.transform.gameObject.name);
                            text = sb.ToString();
                        }
                    }
                    else
                    {
                        text = "";
                    }

                    //Debug.Log("Stranded Deep 2K Mod : debug text = " + text);

                    if (!string.IsNullOrEmpty(text))
                    {
                        distanceText.text = text;
                        distanceCanvasVisible = true;
                    }
                    else
                    {
                        distanceCanvasVisible = false;
                    }
                }
            }
            else
            {
                distanceCanvasVisible = false;
            }
        }

        private static bool distanceCanvasVisible = false;
        private static GameObject textCanvas;
        private static Text distanceText;

        private static void RefreshDistanceCanvas()
        {
            if (textCanvas == null)
            {
                //Debug.Log("Stranded Deep 2K Mod : debug canvas creation");

                textCanvas = createCanvas(false, "DistanceCanvas");

                Font defaultFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                GameObject textDebugGO = new GameObject("TweaksMod_DistanceText_Sprite");
                textDebugGO.transform.SetParent(textCanvas.transform);
                distanceText = textDebugGO.AddComponent<Text>();
                distanceText.horizontalOverflow = HorizontalWrapMode.Wrap;
                distanceText.verticalOverflow = VerticalWrapMode.Overflow;
                distanceText.alignment = TextAnchor.MiddleCenter;
                distanceText.font = defaultFont;
                distanceText.color = Color.white;
                distanceText.fontSize = 20;
                //distanceText.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
                distanceText.text = "";
                distanceText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 600);
                distanceText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600);
                distanceText.rectTransform.localPosition = new Vector3(0,250,0);

                textCanvas.SetActive(true);
            }
            else
            {
                textCanvas.SetActive(distanceCanvasVisible);
            }
        }

        #endregion

        #region Canvas instanciation

        //Creates Hidden GameObject and attaches Canvas component to it
        private static GameObject createCanvas(bool hide, string name = "TweaksModCanvas")
        {
            //Create Canvas GameObject
            GameObject tempCanvas = new GameObject(name);
            if (hide)
            {
                tempCanvas.hideFlags = HideFlags.HideAndDontSave;
            }

            //Create and Add Canvas Component
            Canvas cnvs = tempCanvas.AddComponent<Canvas>();
            cnvs.renderMode = RenderMode.ScreenSpaceOverlay;
            cnvs.pixelPerfect = false;

            //Set Cavas sorting order to be above other Canvas sorting order
            cnvs.sortingOrder = 12;

            cnvs.targetDisplay = 0;

            addCanvasScaler(tempCanvas);
            addGraphicsRaycaster(tempCanvas);

            CanvasGroup cg = tempCanvas.AddComponent<CanvasGroup>();
            cg.alpha = 0.35f;

            return tempCanvas;
        }

        //Adds CanvasScaler component to the Canvas GameObject 
        private static void addCanvasScaler(GameObject parentaCanvas)
        {
            CanvasScaler cvsl = parentaCanvas.AddComponent<CanvasScaler>();
            cvsl.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cvsl.referenceResolution = new Vector2(textCanvasDefaultScreenWitdh, textCanvasDefaultScreenHeight);
            cvsl.matchWidthOrHeight = 0.5f;
            cvsl.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cvsl.referencePixelsPerUnit = 100f;
        }

        //Adds GraphicRaycaster component to the Canvas GameObject 
        private static void addGraphicsRaycaster(GameObject parentaCanvas)
        {
            GraphicRaycaster grcter = parentaCanvas.AddComponent<GraphicRaycaster>();
            grcter.ignoreReversedGraphics = true;
            grcter.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        }

        #endregion

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
    }
}
