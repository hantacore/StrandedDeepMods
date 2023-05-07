using Beam;
using Beam.Terrain;
using Beam.UI;
using Beam.Utilities;
using SharpNeatLib.Maths;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepExplorationMod
{
    static class Main
    {
        internal static string _modName = "Exploration";
        private static string assetName_moai = "Assets/Moai3_light.prefab";
        private static string assetName_aztec = "Assets/xochipilli.prefab";
        private static string assetName_turtle = "Assets/rapa_nui_turtle_statue.prefab";
        private static string assetName_whalebones = "Assets/whale_bones.prefab";
        private static string assetName_stone = "Assets/Budhha Stone.prefab";
        private static string assetName_gate = "Assets/SunGate.prefab";

        private static string configFileName = "StrandedDeepExplorationMod.config";
        private static GameObject moaiPrefab = null;
        private static GameObject turtlePrefab = null;
        private static GameObject aztecPrefab = null;
        private static GameObject whalebonesPrefab = null;
        private static GameObject stonePrefab = null;
        private static GameObject gatePrefab = null;
        //private static float _targetValue = 100f;

        static string _infojsonlocation = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedDeepExplorationMod/StrandedDeepExplorationMod/Info.json";

        private static Dictionary<PrefabType, ExplorationObjectProperties> prefabs = new Dictionary<PrefabType, ExplorationObjectProperties>();

        internal static bool debugMode = false;

        internal enum PrefabType : int
        {
            None = 0,
            Moai = 1,
            Aztec = 2,
            Stone = 3,
            Gate = 4,
            Turtle = 5,
            WhaleBones = 6
        }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;

            VersionChecker.CheckVersion(modEntry, _infojsonlocation);

            ReadConfig();

            string assetBundleFile = Path.Combine(Directory.GetCurrentDirectory(), @"Mods\StrandedDeepExplorationMod\assets\strandeddeepexplorationmod");
            AssetBundle myAssets = AssetBundle.LoadFromFile(assetBundleFile);
            if (myAssets != null)
            {
                Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded AssetBundle");
                moaiPrefab = myAssets.LoadAsset<GameObject>(assetName_moai); // scale = 5
                if (moaiPrefab != null)
                {
                    Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded " + moaiPrefab.name);
                    prefabs.Add(PrefabType.Moai, new ExplorationObjectProperties()
                    {
                        Prefab = moaiPrefab,
                        Scale = 5
                    });
                }

                turtlePrefab = myAssets.LoadAsset<GameObject>(assetName_turtle);
                if (turtlePrefab != null)
                {
                    Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded " + turtlePrefab.name);
                    prefabs.Add(PrefabType.Turtle, new ExplorationObjectProperties()
                    {
                        Prefab = turtlePrefab,
                        Scale = 5
                    });
                }

                aztecPrefab = myAssets.LoadAsset<GameObject>(assetName_aztec); // scale = 2
                if (aztecPrefab != null)
                {
                    Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded " + aztecPrefab.name);
                    prefabs.Add(PrefabType.Aztec, new ExplorationObjectProperties()
                    {
                        Prefab = aztecPrefab,
                        Scale = 5
                    });
                }

                    whalebonesPrefab = myAssets.LoadAsset<GameObject>(assetName_whalebones);
                if (whalebonesPrefab != null)
                {
                    Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded " + whalebonesPrefab.name);
                    prefabs.Add(PrefabType.WhaleBones, new ExplorationObjectProperties()
                    {
                        Prefab = whalebonesPrefab,
                        Scale = 5
                    });
                }

                stonePrefab = myAssets.LoadAsset<GameObject>(assetName_stone);
                if (stonePrefab != null)
                {
                    Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded " + stonePrefab.name);
                    prefabs.Add(PrefabType.Stone, new ExplorationObjectProperties()
                    {
                        Prefab = stonePrefab,
                        Scale = 5,
                        MaxInstances = 3
                    });
                }

                gatePrefab = myAssets.LoadAsset<GameObject>(assetName_gate);
                if (gatePrefab != null)
                {
                    Debug.Log("Stranded Deep " + _modName + " Mod : successfully loaded " + gatePrefab.name);
                    prefabs.Add(PrefabType.Gate, new ExplorationObjectProperties()
                    {
                        Prefab = gatePrefab,
                        Scale = 4,
                        MinHeight = 2.85f,
                        MaxHeight = 2.95f
                    });
                }

                myAssets.Unload(false);
            }
            else
            {
                Debug.Log("Stranded Deep " + _modName + " : NOT loaded AssetBundle");
                return false;
            }

            Debug.Log("Stranded Deep "+ _modName + " Mod properly loaded");

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label(_modName + " mod by Hantacore");
            GUILayout.Label("Moai Light (https://skfb.ly/6XBCN) by pinsel is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Rapa Nui turtle statue (https://skfb.ly/oAWGH) by Nogordo is licensed under CC Attribution-NonCommercial-ShareAlike (http://creativecommons.org/licenses/by-nc-sa/4.0/).");
            GUILayout.Label("Xochipilli (https://skfb.ly/LR9q) by light_heists is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Ancient Stone (https://skfb.ly/opuNv) by Siamak Tavakoli is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("The Gate of the Sun (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            debugMode = GUILayout.Toggle(debugMode, "Debug mode (spawns everywhere), not persisted");
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static bool objectsGenerated = false;
        static int maxWorldObjects = int.MaxValue;
        static int currentWorldObjects = 0;
        static int currentIslandObjects = 0;

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                if (Game.State == GameState.NEW_GAME || Game.State == GameState.LOAD_GAME)
                {
                    if (!WorldUtilities.IsWorldLoaded())
                        return;

                    DebugPlaceObject();

                    if (objectsGenerated)
                        return;

                    Beam.Terrain.Map map = null;
                    Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;

                    if (StrandedWorld.Instance != null
                        && StrandedWorld.Instance.Zones != null
                        && StrandedWorld.Instance.Zones.Length >= 48
                        && maps != null
                        && maps.Length >= 48)
                    {
                        int islandSize = StrandedWorld.ZONE_HEIGHTMAP_SIZE - 1;
                        try
                        {
                            UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
                            if (Game.FindObjectOfType<UMainMenuViewAdapter>().VersionNumberLabel.Text.Contains("Wide")
                                || mewide != null && mewide.Active && mewide.Loaded)
                            {
                                Debug.Log("Stranded Deep " + _modName + " Mod : Stranded Wide detected");
                                islandSize = 512;
                            }
                        }
                        catch { }
                        FastRandom fr = new FastRandom(StrandedWorld.WORLD_SEED);
                        for (int islandIndex = 0; islandIndex < maps.Length; islandIndex++)
                        {
                            Zone zone = StrandedWorld.Instance.Zones[islandIndex];
                            try
                            {
                                currentIslandObjects = 0;
                                int prefabValue = fr.Next(0, 1000) / 100;

                                if (zone.IsUserMap)
                                    continue;
                                Debug.Log("Stranded Deep " + _modName + " Mod : Generating exploration new objects for " + zone.name + "/" + StrandedWorld.WORLD_SEED);
                                map = maps[islandIndex];

                                //Debug.Log("Stranded Deep " + _modName + " Mod : Generating exploration new objects : " + prefabValue);
                                PrefabType prefab = PrefabType.None;
                                if (prefabValue >= 0 && prefabValue <= 6)
                                {
                                    prefab = (PrefabType)prefabValue;
                                }
                                Debug.Log("Stranded Deep " + _modName + " Mod : Generating exploration new objects : " + prefab.ToString());
                                if (prefabs.ContainsKey(prefab))
                                {
                                    RandomlyPlacePrefabsOnIsland(zone, islandSize, map, prefabs[prefab]);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Stranded Deep " + _modName + " Mod : exploration new objects generation failed for island " + zone.name + " / " + e);
                            }
                        }
                        objectsGenerated = true;
                    }
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep " + _modName + " Mod : error on update : " + e);
                objectsGenerated = true;
            }
        }

        private static void DebugPlaceObject()
        {
            if (debugMode)
            {
                Beam.Terrain.Map map = null;
                Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;

                if (StrandedWorld.Instance == null
                    || StrandedWorld.Instance.Zones == null
                    || StrandedWorld.Instance.Zones.Length < 48
                    || maps == null
                    || maps.Length < 48)
                {
                    return;
                }

                Event currentevent = Event.current;
                if (currentevent.isKey)
                {
                    int islandSize = 256;
                    try
                    {
                        UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
                        if (Game.FindObjectOfType<UMainMenuViewAdapter>().VersionNumberLabel.Text.Contains("Wide")
                            || mewide != null && mewide.Active && mewide.Loaded)
                        {
                            //Debug.Log("Stranded Deep " + _modName + " Mod : Stranded Wide detected");
                            islandSize = 512;
                        }
                    }
                    catch { }

                    Zone debugZone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                    if (debugZone != null)
                    {
                        for (int islandIndex = 0; islandIndex < StrandedWorld.Instance.Zones.Length; islandIndex++)
                        {
                            if (String.Compare(StrandedWorld.Instance.Zones[islandIndex].name, debugZone.name) == 0)
                            {
                                map = maps[islandIndex];
                            }
                        }

                        if (currentevent.keyCode == KeyCode.Keypad1)
                        {
                            DebugPlacePrefabOnIsland(debugZone, islandSize, map, prefabs[PrefabType.Moai]);
                        }
                        if (currentevent.keyCode == KeyCode.Keypad2)
                        {
                            DebugPlacePrefabOnIsland(debugZone, islandSize, map, prefabs[PrefabType.Aztec]);
                        }
                        if (currentevent.keyCode == KeyCode.Keypad3)
                        {
                            DebugPlacePrefabOnIsland(debugZone, islandSize, map, prefabs[PrefabType.Stone]);
                        }
                        if (currentevent.keyCode == KeyCode.Keypad4)
                        {
                            DebugPlacePrefabOnIsland(debugZone, islandSize, map, prefabs[PrefabType.Gate]);
                        }
                        if (currentevent.keyCode == KeyCode.Keypad5)
                        {
                            DebugPlacePrefabOnIsland(debugZone, islandSize, map, prefabs[PrefabType.Turtle]);
                        }
                        if (currentevent.keyCode == KeyCode.Keypad6)
                        {
                            DebugPlacePrefabOnIsland(debugZone, islandSize, map, prefabs[PrefabType.WhaleBones]);
                        }
                    }
                    else
                    {
                        Debug.Log("Stranded Deep " + _modName + " Mod : debug zone is NULL");
                    }
                }
            }
        }

        private static void DebugPlacePrefabOnIsland(Zone zone, int islandSize, Beam.Terrain.Map map, ExplorationObjectProperties properties)
        {
            InstantiatePrefab(properties.Prefab, PlayerRegistry.LocalPlayer.transform.position, PlayerRegistry.LocalPlayer.transform.rotation, properties.Scale);
        }

        private static void RandomlyPlacePrefabsOnIsland(Zone zone, int islandSize, Beam.Terrain.Map map, ExplorationObjectProperties properties)
        {
            float islandHalfSize = islandSize / 2;
            FastRandom fr = new FastRandom(StrandedWorld.WORLD_SEED + zone.Seed);
            // magic number found in the vanilla code
            float num = 0.9765625f;
            Terrain terrain = zone.Terrain;
            TerrainData terrainData = zone.Terrain.terrainData;
            float[,] heights = terrainData.GetHeights(0, 0, islandSize + 1, islandSize + 1);
            float[,] array2 = map.HeightmapData;

            Vector3 islandCenter = new Vector3(terrain.transform.position.x + num * (float)islandHalfSize, 0, terrain.transform.position.z + num * (float)islandHalfSize);
            Debug.Log("Stranded Deep " + _modName + " Mod : island center " + islandCenter);
            for (int i = 0; i < islandSize; i++)
            {
                for (int j = 0; j < islandSize; j++)
                {
                    float y = 150f * heights[i, j] + -100f;
                    if (y <= properties.MinHeight || y >= properties.MaxHeight)
                        continue;

                    Vector3 vector = new Vector3(terrain.transform.position.x + num * (float)j, y, terrain.transform.position.z + num * (float)i);
                    float x = (vector.x - terrain.transform.position.x) / islandHalfSize;
                    float y2 = (vector.z - terrain.transform.position.z) / islandHalfSize;
                    Vector3 forward = new Vector3(x - islandCenter.x, 0, y2 - islandCenter.z);
                    Quaternion rot = Quaternion.LookRotation(forward, terrainData.GetInterpolatedNormal(x, y2));
                    //float steepness = terrainData.GetSteepness(x, y2);

                    if (!debugMode && fr.Next(0, 1000) >= 990
                        || debugMode && fr.Next(0, 1000) >= 500)
                    {
                        Debug.Log("Stranded Deep " + _modName + " Mod : instantiating " + properties.Prefab.name + " at " + vector);
                        InstantiatePrefab(properties.Prefab, vector, rot, properties.Scale);
                        currentWorldObjects++;
                        currentIslandObjects++;
                        i = Math.Min(i + 50, islandSize - 1);
                        j = Math.Min(j + 50, islandSize - 1);
                        break;
                    }

                    if (currentIslandObjects >= properties.MaxInstances)
                        break;
                    if (currentWorldObjects >= maxWorldObjects)
                        break;
                }
                if (currentIslandObjects >= properties.MaxInstances)
                    break;
                if (currentWorldObjects >= maxWorldObjects)
                    break;
            }
        }

        private static void Reset()
        {
            currentWorldObjects = 0;
            objectsGenerated = false;
        }

        private static void InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, float scale)
        {
            try
            {
                GameObject go = Beam.Game.Instantiate(prefab);
                go.transform.position = position;
                go.transform.localPosition = position;
                go.transform.rotation = rotation;
                go.transform.localRotation = rotation;
                go.SetLayerRecursively(Layers.TERRAIN_OBJECTS);
                go.SetActive(true);

                go.transform.localScale = new Vector3(scale, scale, scale);

                go.AddComponent<ShaderScript>();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep " + _modName + " Mod : error on instantiate prefab : " + e);
            }
        }

        private static void ReadConfig()
        {
            string dataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
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
            string dataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
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
    }
}
