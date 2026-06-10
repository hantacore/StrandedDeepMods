using Beam;
using Beam.Utilities;
using HarmonyLib;
using SharpNeatLib.Maths;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeep2KModBundles
{
    static partial class Main
    {
        private static Harmony harmony;

        internal static Dictionary<string, Texture2D> _indexedTextures = new Dictionary<string, Texture2D>();

        internal static GameObject newBushPrefab = null;
        internal static GameObject newBush2Prefab = null;
        internal static GameObject newBush3Prefab = null;
        internal static GameObject mangrove1Prefab = null;
        internal static GameObject cucumberGrassPrefab = null;
        internal static GameObject oldTreePrefab = null;
        internal static GameObject allPlantsPrefab = null;
        internal static GameObject longGrassPrefab = null;
        internal static GameObject taroPrefab = null;
        internal static GameObject smallTreePrefab = null;

        internal static GameObject exoticTreePrefab = null;
        internal static GameObject coconutSproutPrefab = null;
        internal static GameObject branch1Prefab = null;
        internal static GameObject branch2Prefab = null;
        internal static GameObject lemongrassPrefab = null;
        internal static GameObject sugarcanePrefab = null;

        internal static GameObject colaTrashPrefab = null;
        internal static GameObject drifttrunkPrefab = null;
        internal static GameObject plasticBottleTrashPrefab = null;
        internal static GameObject propaneTankTrashPrefab = null;
        internal static GameObject turtleShellTrashPrefab = null;

        internal static GameObject pebblesPrefab = null;
        internal static GameObject pebbles2Prefab = null;
        internal static GameObject pebbles3Prefab = null;
        internal static GameObject pebbles4Prefab = null;

        internal static GameObject chineseBanyaPrefab = null;
        internal static GameObject bushHeroPrefab = null;

        internal static GameObject fanCoralsMultiPrefab = null;
        internal static GameObject fanCoralsOrangePrefab = null;
        internal static GameObject fanCoralsYellowPrefab = null;
        internal static GameObject giantClamPrefab = null;

        internal enum PrefabType : int
        {
            None = 0,
            AllPlants = 1,
            Mangrove = 2,
            LongGrass = 3,
            CucumberGrass = 4,
            Taro = 5,
            ExoticTree = 6,
            CoconutSprout = 7,
            Branch1 = 8,
            Branch2 = 9,
            MedBush = 10,
            SugarCane = 11,
            LemonGrass = 12,
            DriftTrunk = 13,
            ColaTrash = 14,
            PlasticBottleTrash = 15,
            PropaneTrash = 16,
            TurtleShellTrash = 17,
            Pebbles = 18,
            Pebbles2 = 19,
            Pebbles3 = 20,
            Pebbles4 = 21,
            StarBush = 22,
            ChineseBanyan = 23,
            FanCoralMulti = 24,
            FanCoralOrange = 25,
            FanCoralYellow = 26,
            SmallTree = 27,
            GiantClam = 28
        }

        private static Dictionary<PrefabType, DetailObjectProperties> prefabs = new Dictionary<PrefabType, DetailObjectProperties>();

        private class ModelInfo
        {
            public string Name;
            public string Url;
            public string Author;
            public string ConfigKey;
            public bool ConfigEnabled;
            public PrefabType PrefabType;
        }

        private static List<ModelInfo> _modelInfos = new List<ModelInfo>();

        public static bool debugLog = false;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;
                modEntry.OnUnload = OnUnload;

                ReadConfig();

                if (!ExistsConfig())
                {
                    WriteConfig();
                }

                CustomLogger.InitCustomLogger(FilePath.SAVE_FOLDER);

                CustomLogger.Log("#######################################################################");
                CustomLogger.Log(" ");
                CustomLogger.Log(" START PRE-LOADING ");
                CustomLogger.Log(" ");
                CustomLogger.Log("#######################################################################");

                bool loadOK = PreLoadAssetsTextures(modEntry.Path) && PreLoadAssetsModels(modEntry.Path);

                CustomLogger.Log("#######################################################################");
                CustomLogger.Log(" ");
                CustomLogger.Log(" END PRE-LOADING ");
                CustomLogger.Log(" ");
                CustomLogger.Log("#######################################################################");

                // add 3d models config
                ReadConfig();
                WriteConfig();

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                ResetLists();

                if (loadOK)
                    Debug.Log("Stranded Deep 2K Mod properly loaded");
                else
                    Debug.Log("Stranded Deep 2K Mod textures preload failed");

                return loadOK;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep 2K Mod loading failed : " + e);
            }
            return false;
        }

        private static bool PreLoadAssetsTextures(string path)
        {
            try
            {
                string assetBundleFile = Path.Combine(path, @"assets\strandeddeep2kmod");
                if (!File.Exists(assetBundleFile))
                    return true;
                AssetBundle myAssets = AssetBundle.LoadFromFile(assetBundleFile);
                if (myAssets != null)
                {
                    CustomLogger.Log("Stranded Deep Better 2K Mod : successfully loaded AssetBundle" + assetBundleFile);
                }
                else
                {
                    CustomLogger.Log("Stranded Deep Better 2K Mod : NOT loaded AssetBundle" + assetBundleFile);
                }

                foreach (string assetName in myAssets.GetAllAssetNames())
                {
                    if (assetName.EndsWith(".jpg") || assetName.EndsWith(".png") || assetName.EndsWith(".psd"))
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : assetName = " + assetName);
                        Texture2D texture = myAssets.LoadAsset<Texture2D>(assetName);
                        if (texture != null)
                        {
                            CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded texture asset " + assetName);
                        }
                        else
                        {
                            CustomLogger.Log("Stranded Deep 2K Mod : NOT loaded texture asset " + assetName);
                        }
                        string key = assetName
                            .Replace("assets/animals/", "")
                            .Replace("assets/containers/", "")
                            .Replace("assets/items/", "")
                            .Replace("assets/particles/", "")
                            .Replace("assets/plants/", "")
                            .Replace("assets/rocks/", "")
                            .Replace("assets/sea/", "")
                            .Replace("assets/structures/", "")
                            .Replace("assets/wrecks/", "")
                            .Replace("assets/", "")
                            .Replace(" (instance)", "")
                            .Replace("8k", "")
                            .Replace("4k", "")
                            .Replace("2k", "")
                            .Replace("1k", "")
                            .Replace(".psd", "")
                            .Replace(".png", "")
                            .Replace(".jpg", "");
                        CustomLogger.Log("Stranded Deep 2K Mod : preloaded " + key);
                        _indexedTextures.Add(key, texture);
                    }
                }

                myAssets.Unload(false);

                return true;
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : texture preload failed : " + e);
                return false;
            }
        }

        private static bool PreLoadAssetsModels(string path)
        {
            try
            {
                string assetBundleFile = Path.Combine(path, @"assets\strandeddeep2kmodmodels");
                if (!File.Exists(assetBundleFile))
                    return true;
                AssetBundle myAssets = AssetBundle.LoadFromFile(assetBundleFile);
                if (myAssets != null)
                {
                    CustomLogger.Log("Stranded Deep Better 2K Mod : successfully loaded AssetBundle" + assetBundleFile);
                }
                else
                {
                    CustomLogger.Log("Stranded Deep Better 2K Mod : NOT loaded AssetBundle" + assetBundleFile);
                    replaceBushes = false;
                    replaceFicus = false;
                    newModels = false;
                }

                //foreach (string assetName in myAssets.GetAllAssetNames())
                //{
                //    if (assetName.EndsWith(".jpg") || assetName.EndsWith(".png") || assetName.EndsWith(".psd"))
                //    {
                //        CustomLogger.Log("Stranded Deep 2K Mod : assetName = " + assetName);
                //        Texture2D texture = myAssets.LoadAsset<Texture2D>(assetName);
                //        if (texture != null)
                //        {
                //            CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded texture asset " + assetName);
                //        }
                //        else
                //        {
                //            CustomLogger.Log("Stranded Deep 2K Mod : NOT loaded texture asset " + assetName);
                //        }
                //        string key = assetName
                //            .Replace("assets/animals/", "")
                //            .Replace("assets/containers/", "")
                //            .Replace("assets/items/", "")
                //            .Replace("assets/particles/", "")
                //            .Replace("assets/plants/", "")
                //            .Replace("assets/rocks/", "")
                //            .Replace("assets/sea/", "")
                //            .Replace("assets/structures/", "")
                //            .Replace("assets/wrecks/", "")
                //            .Replace("assets/", "")
                //            .Replace(" (instance)", "")
                //            .Replace("8k", "")
                //            .Replace("4k", "")
                //            .Replace("2k", "")
                //            .Replace("1k", "")
                //            .Replace(".psd", "")
                //            .Replace(".png", "")
                //            .Replace(".jpg", "");
                //        CustomLogger.Log("Stranded Deep 2K Mod : preloaded " + key);
                //        _indexedTextures.Add(key, texture);
                //    }
                //}

                int instancesRatio = WorldUtilities.IslandSizeRatio;

                if (replaceBushes || newModels)
                {
                    newBush3Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/bush_small.prefab");
                    if (newBush3Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + newBush3Prefab.name);
                        _modelInfos.Add(new ModelInfo() { Name = "Bush", Author = "?", Url = "https://bendtrade.com/3dmodels/kust-17624", ConfigKey = "bush", ConfigEnabled = false });
                    }
                }

                if (replaceFicus || newModels)
                {
                    oldTreePrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/old_tree.prefab");
                    if (oldTreePrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + oldTreePrefab.name);
                        _modelInfos.Add(new ModelInfo() { Name = "Old Tree", Author = "ASMA3D", Url = "https://skfb.ly/6RZyA", ConfigKey = "oldtree", ConfigEnabled = false });
                    }
                }

                if (newModels)
                {
                    bushHeroPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/Bush_Hero.prefab");
                    if (bushHeroPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + bushHeroPrefab.name);
                        _modelInfos.Add(new ModelInfo() { Name = "Simple Bush", Author = "marco.cossetti", Url = "https://skfb.ly/6S79v", ConfigKey = "simplebush", ConfigEnabled = false });
                    }

                    newBushPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/bush.prefab");
                    if (newBushPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + newBushPrefab.name);
                        prefabs.Add(PrefabType.StarBush, new DetailObjectProperties()
                        {
                            Prefab = newBushPrefab,
                            Scale = 2,
                            MaxInstances = 2 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.99f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "3D Bush - 01", Author = "Grand Dog Studio", Url = "https://skfb.ly/o7qWZ", ConfigKey = "3dbush", ConfigEnabled = false, PrefabType = PrefabType.StarBush });
                    }

                    newBush2Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/mid_bush.prefab");
                    if (newBush2Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + newBush2Prefab.name);
                        prefabs.Add(PrefabType.MedBush, new DetailObjectProperties()
                        {
                            Prefab = newBush2Prefab,
                            Scale = 100,
                            MaxInstances = 2 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.8f,
                            UseTerrainNormal = false,
                            ForceRotation = Quaternion.Euler(-180, 0, 0)
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "3D Bush - 01", Author = "Grand Dog Studio", Url = "https://skfb.ly/o7qWZ", ConfigKey = "3dbush2", ConfigEnabled = false, PrefabType = PrefabType.MedBush });
                    }


                    lemongrassPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/lemongrass.prefab");
                    if (lemongrassPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + lemongrassPrefab.name);
                        prefabs.Add(PrefabType.LemonGrass, new DetailObjectProperties()
                        {
                            Prefab = lemongrassPrefab,
                            MaxInstances = 5 * instancesRatio,
                            Scale = 0.8f,
                            RandomizeScale = true,
                            Rarity = 9.99f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Lemon Grass", Author = "charles.cla", Url = "https://skfb.ly/oAynS", ConfigKey = "lemongrass", ConfigEnabled = false, PrefabType = PrefabType.LemonGrass });
                    }

                    mangrove1Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/RedMangrove_Tree_small.prefab");
                    if (mangrove1Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + mangrove1Prefab.name);
                        prefabs.Add(PrefabType.Mangrove, new DetailObjectProperties()
                        {
                            Prefab = mangrove1Prefab,
                            Scale = 1.0f,
                            MaxInstances = 15 * instancesRatio,
                            RandomizeScale = true,
                            MinHeight = -0.1f,
                            MaxHeight = 0.0f,
                            UseTerrainNormal = false,
                            Rarity = 9.8f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Red Mangrove Tree", Author = "SAVIA_Colombia", Url = "https://skfb.ly/ozCCG", ConfigKey = "mangrovetree", ConfigEnabled = false, PrefabType = PrefabType.Mangrove });
                    }

                    cucumberGrassPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/cucumber_grass.prefab");
                    if (cucumberGrassPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + cucumberGrassPrefab.name);
                        prefabs.Add(PrefabType.CucumberGrass, new DetailObjectProperties()
                        {
                            Prefab = cucumberGrassPrefab,
                            Scale = 1,
                            MaxInstances = 2 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.8f,
                            UseTerrainNormal = false
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Cucumber Grass", Author = "kelvladmail", Url = "https://skfb.ly/6SrsZ", ConfigKey = "cucumbergrass", ConfigEnabled = false, PrefabType = PrefabType.CucumberGrass });
                    }

                    allPlantsPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/All Plants.prefab");
                    if (allPlantsPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + allPlantsPrefab.name);
                        prefabs.Add(PrefabType.AllPlants, new DetailObjectProperties()
                        {
                            Prefab = allPlantsPrefab,
                            Scale = 1,
                            MaxInstances = 15 * instancesRatio,
                            RandomizeScale = true
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "All Plants", Author = "Svea_99", Url = "https://skfb.ly/6WNDG", ConfigKey = "allplants", ConfigEnabled = false, PrefabType = PrefabType.AllPlants });
                    }

                    longGrassPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/long_grass.prefab");
                    if (longGrassPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + longGrassPrefab.name);
                        prefabs.Add(PrefabType.LongGrass, new DetailObjectProperties()
                        {
                            Prefab = longGrassPrefab,
                            Scale = 1,
                            MaxInstances = 15 * instancesRatio,
                            RandomizeScale = true
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Animated Grass - Vegetation", Author = "raguramkgr", Url = "https://skfb.ly/ouLpu", ConfigKey = "longgrass", ConfigEnabled = false, PrefabType = PrefabType.LongGrass });
                    }

                    taroPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/taro.prefab");
                    if (taroPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + taroPrefab.name);
                        prefabs.Add(PrefabType.Taro, new DetailObjectProperties()
                        {
                            Prefab = taroPrefab,
                            Scale = 0.4f,
                            MaxInstances = 2 * instancesRatio,
                            RandomizeScale = true,
                            UseTerrainNormal = false,
                            Rarity = 9.8f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Taro", Author = "The_Structure_World", Url = "https://skfb.ly/oqn9R", ConfigKey = "taro", ConfigEnabled = false, PrefabType = PrefabType.Taro });
                    }

                    smallTreePrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/small_tree.prefab");
                    if (smallTreePrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + smallTreePrefab.name);
                        prefabs.Add(PrefabType.SmallTree, new DetailObjectProperties()
                        {
                            Prefab = smallTreePrefab,
                            Scale = 0.5f,
                            MaxInstances = 1 * instancesRatio,
                            RandomizeScale = true,
                            UseTerrainNormal = false,
                            Rarity = 9.8f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Small Tree", Author = "", Url = "", ConfigKey = "smalltree", ConfigEnabled = false, PrefabType = PrefabType.SmallTree });
                    }

                    exoticTreePrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/exotic_tree.prefab");
                    if (exoticTreePrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + exoticTreePrefab.name);
                        prefabs.Add(PrefabType.ExoticTree, new DetailObjectProperties()
                        {
                            Prefab = exoticTreePrefab,
                            MaxInstances = 1 * instancesRatio,
                            RandomizeScale = true,
                            UseTerrainNormal = false,
                            ForceRotation = Quaternion.Euler(-180, 0, 0),
                            Rarity = 9.8f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Exotic Tree", Author = "", Url = "", ConfigKey = "exotictree", ConfigEnabled = false, PrefabType = PrefabType.ExoticTree });
                    }

                    coconutSproutPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/Coconut_Plant_small.prefab");
                    if (coconutSproutPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + coconutSproutPrefab.name);
                        prefabs.Add(PrefabType.CoconutSprout, new DetailObjectProperties()
                        {
                            Prefab = coconutSproutPrefab,
                            MaxInstances = 5 * instancesRatio,
                            RandomizeScale = true,
                            UseTerrainNormal = false,
                            Rarity = 9.99f,
                            MinHeight = 1.0f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Coconut Plant", Author = "The_Structure_World", Url = "https://skfb.ly/orTZO", ConfigKey = "coconutsprout", ConfigEnabled = false, PrefabType = PrefabType.CoconutSprout });
                    }

                    branch1Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/Branch.prefab");
                    if (branch1Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + branch1Prefab.name);
                        prefabs.Add(PrefabType.Branch1, new DetailObjectProperties()
                        {
                            Prefab = branch1Prefab,
                            MaxInstances = 10 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.9f,
                            MinHeight = 0.1f,
                            ForceRotation = Quaternion.Euler(0, 0, 90)
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Branch", Author = "", Url = "", ConfigKey = "branch", ConfigEnabled = false, PrefabType = PrefabType.Branch1 });
                    }

                    branch2Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/dune_branch.prefab");
                    if (branch2Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + branch2Prefab.name);
                        prefabs.Add(PrefabType.Branch2, new DetailObjectProperties()
                        {
                            Prefab = branch2Prefab,
                            MaxInstances = 5 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Dune branch", Author = "marijnwillemse", Url = "https://skfb.ly/67NGt", ConfigKey = "dunebranch", ConfigEnabled = false, PrefabType = PrefabType.Branch2 });
                    }

                    sugarcanePrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/sugar_cane.prefab");
                    if (sugarcanePrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + sugarcanePrefab.name);
                        prefabs.Add(PrefabType.SugarCane, new DetailObjectProperties()
                        {
                            Prefab = sugarcanePrefab,
                            MaxInstances = 5 * instancesRatio,
                            Scale = 0.8f,
                            RandomizeScale = true,
                            UseTerrainNormal = false,
                            Rarity = 9.99f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Sugar Cane", Author = "printable_models", Url = "https://free3d.com/3d-model/-sugarcane-field-v1--778762.html", ConfigKey = "sugarcane", ConfigEnabled = false, PrefabType = PrefabType.SugarCane });
                    }

                    chineseBanyaPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/ChineseBanyan_Med.prefab");
                    if (chineseBanyaPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + chineseBanyaPrefab.name);
                        prefabs.Add(PrefabType.ChineseBanyan, new DetailObjectProperties()
                        {
                            Prefab = chineseBanyaPrefab,
                            MaxInstances = 1 * instancesRatio,
                            Scale = 0.3f,
                            RandomizeScale = true,
                            UseTerrainNormal = false,
                            ForceRotation = Quaternion.Euler(-90, 0, 0),
                            Rarity = 9.99f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Chinese Banyan (Ficus Microcarpa)", Author = "Valery.Li", Url = "https://skfb.ly/o7XxS", ConfigKey = "banyan", ConfigEnabled = false, PrefabType = PrefabType.ChineseBanyan });
                    }

                    #region shore trash

                    drifttrunkPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/driftwood.prefab");
                    if (drifttrunkPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + drifttrunkPrefab.name);
                        prefabs.Add(PrefabType.DriftTrunk, new DetailObjectProperties()
                        {
                            Prefab = drifttrunkPrefab,
                            MaxInstances = 2 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Drift Wood", Author = "nick_clicks", Url = "https://skfb.ly/6zzrB", ConfigKey = "driftwood", ConfigEnabled = false, PrefabType = PrefabType.DriftTrunk });
                    }

                    colaTrashPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/cola_trash.prefab");
                    if (colaTrashPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + colaTrashPrefab.name);
                        prefabs.Add(PrefabType.ColaTrash, new DetailObjectProperties()
                        {
                            Prefab = colaTrashPrefab,
                            MaxInstances = 1 * instancesRatio,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Cola Trash", Author = "No More Mondays", Url = "https://skfb.ly/oqVPQ", ConfigKey = "colatrash", ConfigEnabled = false, PrefabType = PrefabType.ColaTrash });
                    }

                    plasticBottleTrashPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/plastic_bottle.prefab");
                    if (plasticBottleTrashPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + plasticBottleTrashPrefab.name);
                        prefabs.Add(PrefabType.PlasticBottleTrash, new DetailObjectProperties()
                        {
                            Prefab = plasticBottleTrashPrefab,
                            MaxInstances = 1 * instancesRatio,
                            Scale = 0.5f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Plastic Water Bottle", Author = "RoutineStudio", Url = "https://skfb.ly/6Uzt9", ConfigKey = "plastictrash", ConfigEnabled = false, PrefabType = PrefabType.PlasticBottleTrash });
                    }


                    turtleShellTrashPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/TurtleShell.prefab");
                    if (turtleShellTrashPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + turtleShellTrashPrefab.name);
                        prefabs.Add(PrefabType.TurtleShellTrash, new DetailObjectProperties()
                        {
                            Prefab = turtleShellTrashPrefab,
                            MaxInstances = 1 * instancesRatio,
                            Scale = 0.7f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f
                        });

                        _modelInfos.Add(new ModelInfo() { Name = "Turtle Shell", Author = "Tyler Jackson", Url = "https://skfb.ly/6RGNA", ConfigKey = "turtleshell", ConfigEnabled = false, PrefabType = PrefabType.TurtleShellTrash });
                    }

                    propaneTankTrashPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/propane_tank.prefab");
                    if (propaneTankTrashPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + propaneTankTrashPrefab.name);
                        prefabs.Add(PrefabType.PropaneTrash, new DetailObjectProperties()
                        {
                            Prefab = propaneTankTrashPrefab,
                            MaxInstances = 1 * instancesRatio,
                            Scale = 0.7f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Propane Tank", Author = "Oliver Triplett", Url = "https://skfb.ly/6AWtA", ConfigKey = "propanetank", ConfigEnabled = false, PrefabType = PrefabType.PropaneTrash });
                    }

                    #endregion

                    pebblesPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/pebbles.prefab");
                    if (pebblesPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + pebblesPrefab.name);
                        prefabs.Add(PrefabType.Pebbles, new DetailObjectProperties()
                        {
                            Prefab = pebblesPrefab,
                            MaxInstances = 3 * instancesRatio,
                            Scale = 0.7f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f,
                            UseTerrainNormal = false
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Ground Pebbles", Author = "Kallvin", Url = "https://skfb.ly/6zSsL", ConfigKey = "pebbles", ConfigEnabled = false, PrefabType = PrefabType.Pebbles });
                    }

                    pebbles2Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/pebbles2.prefab");
                    if (pebblesPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + pebbles2Prefab.name);
                        prefabs.Add(PrefabType.Pebbles2, new DetailObjectProperties()
                        {
                            Prefab = pebbles2Prefab,
                            MaxInstances = 3 * instancesRatio,
                            Scale = 0.7f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f,
                            UseTerrainNormal = false
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Ground Pebbles 2", Author = "Kallvin", Url = "https://skfb.ly/6zSsL", ConfigKey = "pebbles2", ConfigEnabled = false, PrefabType = PrefabType.Pebbles2 });
                    }

                    pebbles3Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/pebbles3.prefab");
                    if (pebbles3Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + pebbles3Prefab.name);
                        prefabs.Add(PrefabType.Pebbles3, new DetailObjectProperties()
                        {
                            Prefab = pebbles3Prefab,
                            MaxInstances = 3 * instancesRatio,
                            Scale = 0.7f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f,
                            UseTerrainNormal = false
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Ground Pebbles 2", Author = "Kallvin", Url = "https://skfb.ly/6zSsL", ConfigKey = "pebbles3", ConfigEnabled = false, PrefabType = PrefabType.Pebbles3 });
                    }

                    pebbles4Prefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/pebbles4.prefab");
                    if (pebbles4Prefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + pebbles4Prefab.name);
                        prefabs.Add(PrefabType.Pebbles4, new DetailObjectProperties()
                        {
                            Prefab = pebbles4Prefab,
                            MaxInstances = 3 * instancesRatio,
                            Scale = 0.7f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = 0.1f,
                            MaxHeight = 0.5f,
                            UseTerrainNormal = false
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Ground Pebbles 2", Author = "Kallvin", Url = "https://skfb.ly/6zSsL", ConfigKey = "pebbles4", ConfigEnabled = false, PrefabType = PrefabType.Pebbles4 });
                    }

                    fanCoralsMultiPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/FanCoral_Cluster_Low.prefab");
                    if (fanCoralsMultiPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + fanCoralsMultiPrefab.name);
                        prefabs.Add(PrefabType.FanCoralMulti, new DetailObjectProperties()
                        {
                            Prefab = fanCoralsMultiPrefab,
                            MaxInstances = 30 * instancesRatio,
                            ForceRotation = Quaternion.Euler(-90f, 0, 0),
                            Scale = 2f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = -5f,
                            MaxHeight = -1.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Fan Coral Cluster Low", Author = "Valery.Li", Url = "https://skfb.ly/oyyPI", ConfigKey = "fancoral1", ConfigEnabled = false, PrefabType = PrefabType.FanCoralMulti });
                    }

                    fanCoralsOrangePrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/FanCoral_Cluster_Med_Orange.prefab");
                    if (fanCoralsOrangePrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + fanCoralsOrangePrefab.name);
                        prefabs.Add(PrefabType.FanCoralOrange, new DetailObjectProperties()
                        {
                            Prefab = fanCoralsOrangePrefab,
                            MaxInstances = 30 * instancesRatio,
                            Scale = 0.5f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = -5f,
                            MaxHeight = -1.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Fan Coral Med", Author = "Valery.Li", Url = "https://skfb.ly/oyyOH", ConfigKey = "fancoral2", ConfigEnabled = false, PrefabType = PrefabType.FanCoralOrange });
                    }

                    fanCoralsYellowPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/FanCoral_Cluster_Med_Yellow.prefab");
                    if (fanCoralsYellowPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + fanCoralsYellowPrefab.name);
                        prefabs.Add(PrefabType.FanCoralYellow, new DetailObjectProperties()
                        {
                            Prefab = fanCoralsYellowPrefab,
                            MaxInstances = 30 * instancesRatio,
                            Scale = 0.5f,
                            RandomizeScale = true,
                            Rarity = 9.99f,
                            MinHeight = -5f,
                            MaxHeight = -1.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Fan Coral Med", Author = "Valery.Li", Url = "https://skfb.ly/oyyOH", ConfigKey = "fancoral3", ConfigEnabled = false, PrefabType = PrefabType.FanCoralYellow });
                    }

                    giantClamPrefab = myAssets.LoadAsset<GameObject>("Assets/Prefabs/giant_clam.prefab");
                    if (giantClamPrefab != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : successfully loaded " + giantClamPrefab.name);
                        prefabs.Add(PrefabType.GiantClam, new DetailObjectProperties()
                        {
                            Prefab = giantClamPrefab,
                            MaxInstances = 30 * instancesRatio,
                            Scale = 1.0f,
                            RandomizeScale = true,
                            Rarity = 9f,
                            MinHeight = -5f,
                            MaxHeight = -1.5f
                        });
                        _modelInfos.Add(new ModelInfo() { Name = "Giant Clam", Author = "Natural History Museum Vienna", Url = "https://skfb.ly/o7A6H", ConfigKey = "giantclam", ConfigEnabled = false, PrefabType = PrefabType.GiantClam });
                    }
                }

                myAssets.Unload(false);

                return true;
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : models preload failed : " + e);
                return false;
            }
        }

        internal static bool newModels = false;
        internal static bool replaceBushes = false;
        internal static bool replaceFicus = false;

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Stranded Deep 2K mod by Hantacore");
            GUILayout.Label("You must restart the game for the change to apply");
            replaceBushes = GUILayout.Toggle(replaceBushes, "More realistic bushes");
            replaceFicus = GUILayout.Toggle(replaceFicus, "Replace big ficus model");
            newModels = GUILayout.Toggle(newModels, "Add new models");
            GUILayout.Label("---------------------------------");
            foreach (ModelInfo mi in _modelInfos)
            {
                GUILayout.Label(String.Format("{0} (<a=\"{1}\">{1}</a>) by {2} is licensed under Creative Commons Attribution (<a href=\"http://creativecommons.org/licenses/by/4.0/)\">http://creativecommons.org/licenses/by/4.0/</a>).", mi.Name, mi.Url, mi.Author));
                mi.ConfigEnabled = GUILayout.Toggle(mi.ConfigEnabled, "Enable " + mi.Name);
            }
            /*GUILayout.Label("Branch (https://skfb.ly/oAWGH) by Nogordo is licensed under CC Attribution-NonCommercial-ShareAlike (http://creativecommons.org/licenses/by-nc-sa/4.0/).");
            GUILayout.Label("Bush (https://skfb.ly/LR9q) by light_heists is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Bush Hero (https://skfb.ly/opuNv) by Siamak Tavakoli is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Chinese Banyan (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Coconut Plant (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Cola can trash (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Cucumber grass (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Driftwood (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Dune branch (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Exotic tree (retextured) (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Fan coral low poly (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Fan coral med poly (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Lemongrass (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Long grass (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Mid bush (retextured) (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Old tree (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Pebbles (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Platic bottle (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Propane tank (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Red Mangrove tree (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Sugar cane (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Taro (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");
            GUILayout.Label("Turtle shell (https://skfb.ly/oqGB9) by Albert Gregl is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).");*/
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static bool objectsGenerated = false;

        public static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                if (Game.State == GameState.NEW_GAME || Game.State == GameState.LOAD_GAME)
                {
                    if (!newModels)
                        return;

                    if (!WorldUtilities.IsWorldLoaded())
                        return;

                    if (objectsGenerated)
                        return;

                    Beam.Terrain.Map map = null;
                    Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;

                    if (StrandedWorld.Instance != null
                        && StrandedWorld.Instance.Zones != null
                        && StrandedWorld.Instance.Zones.Length >= WorldUtilities.IslandsCount
                        && maps != null
                        && maps.Length >= WorldUtilities.IslandsCount)
                    {
                        int islandSize = StrandedWorld.ZONE_HEIGHTMAP_SIZE - 1;
                        try
                        {
                            if (WorldUtilities.IsStrandedWide())
                            {
                                CustomLogger.Log("Stranded Deep 2K Mod : Stranded Wide detected");
                                //islandSize = 512;
                                islandSize = WorldUtilities.IslandSize;
                            }
                        }
                        catch { }

                        CustomLogger.Log("#######################################################################");
                        CustomLogger.Log(" ");
                        CustomLogger.Log(" START ADDING NEW MODELS ");
                        CustomLogger.Log(" ");
                        CustomLogger.Log("#######################################################################");

                        for (int islandIndex = 0; islandIndex < WorldUtilities.IslandsCount; islandIndex++)
                        {
                            Zone zone = StrandedWorld.Instance.Zones[islandIndex];
                            try
                            {
                                if (zone.IsUserMap)
                                    continue;
                                CustomLogger.Log("Stranded Deep 2K Mod : Generating flora new objects for Zone " + zone.name + " / World seed : " + StrandedWorld.WORLD_SEED);
                                map = maps[islandIndex];

                                foreach(PrefabType prefab in prefabs.Keys)
                                {
                                    CustomLogger.Log("Stranded Deep 2K Mod : Generating flora new objects : " + prefab.ToString(), debugLog);
                                    bool skip = false;
                                    foreach(ModelInfo mi in _modelInfos)
                                    {
                                        if (mi.PrefabType == prefab && !mi.ConfigEnabled)
                                        {
                                            skip = true;
                                            break;
                                        }
                                    }
                                    if (skip)
                                        continue;

                                    // to keep consistent if model removed
                                    FastRandom fr = new FastRandom(StrandedWorld.WORLD_SEED + zone.Seed + (int)prefab);
                                    RandomlyPlacePrefabsOnIsland(fr, zone, islandSize, map, prefabs[prefab]);
                                }
                            }
                            catch (Exception e)
                            {
                                CustomLogger.Log("Stranded Deep 2K Mod : flora new objects generation failed for island " + zone.name + " / " + e);
                            }
                        }

                        CustomLogger.Log("#######################################################################");
                        CustomLogger.Log(" ");
                        CustomLogger.Log(" END ADDING NEW MODELS ");
                        CustomLogger.Log(" ");
                        CustomLogger.Log("#######################################################################");

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
                CustomLogger.Log("Stranded Deep 2K Mod : error on update : " + e);
                objectsGenerated = true;
            }
        }

        private static void Reset()
        {
            objectsGenerated = false;
            ResetLists();
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        internal static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, float scale)
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

                return go;
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : error on instantiate prefab : " + e);
            }
            return null;
        }

        private static void RandomlyPlacePrefabsOnIsland(FastRandom fr, Zone zone, int islandSize, Beam.Terrain.Map map, DetailObjectProperties properties)
        {
            int currentIslandObjects = 0;

            float islandHalfSize = islandSize / 2;
            // magic number found in the vanilla code
            float num = 0.9765625f;
            Terrain terrain = zone.Terrain;
            TerrainData terrainData = zone.Terrain.terrainData;
            float[,] heights = terrainData.GetHeights(0, 0, islandSize + 1, islandSize + 1);
            float[,] array2 = map.HeightmapData;

            Vector3 islandCenter = new Vector3(terrain.transform.position.x + num * (float)islandHalfSize, 0, terrain.transform.position.z + num * (float)islandHalfSize);
            CustomLogger.Log("Stranded Deep 2K Mod : island center " + islandCenter, debugLog);
            int stepI = fr.Next(1, 3);
            int stepJ = fr.Next(1, 3);
            for (int i = 0; i < islandSize; i+=stepI)
            {
                for (int j = 0; j < islandSize; j+=stepJ)
                {
                    float y = 150f * heights[i, j] + -100f;
                    if (y <= properties.MinHeight || y >= properties.MaxHeight)
                        continue;

                    Vector3 vector = new Vector3(terrain.transform.position.x + num * (float)j, y, terrain.transform.position.z + num * (float)i);
                    float x = (vector.x - terrain.transform.position.x) / islandHalfSize;
                    float y2 = (vector.z - terrain.transform.position.z) / islandHalfSize;
                    Vector3 forward = new Vector3(x - islandCenter.x, 0, y2 - islandCenter.z);
                    Quaternion rot = new Quaternion();
                    if (properties.UseTerrainNormal)
                    {
                        rot = Quaternion.LookRotation(forward, terrainData.GetInterpolatedNormal(x, y2));
                    }
                    if (properties.ForceRotation != new Quaternion())
                    {
                        rot = Quaternion.Euler(rot.eulerAngles.x + properties.ForceRotation.eulerAngles.x, rot.eulerAngles.y + properties.ForceRotation.eulerAngles.y, rot.eulerAngles.z + properties.ForceRotation.eulerAngles.z);
                    }
                    if (properties.RandomizeRotation)
                    {
                        rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + fr.Next(0, 359), rot.eulerAngles.z);
                    }
                    //float steepness = terrainData.GetSteepness(x, y2);

                    if (fr.Next(0, 1000) >= properties.Rarity * 100)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : instantiating " + properties.Prefab.name + " at " + vector, debugLog);
                        InstantiatePrefab(properties.Prefab, vector, rot, properties.Scale);
                        currentIslandObjects++;
                        //i = Math.Min(i + 50, islandSize - 1);
                        //j = Math.Min(j + 50, islandSize - 1);
                    }

                    if (currentIslandObjects >= properties.MaxInstances)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : flora MaxInstances reached : " + currentIslandObjects, debugLog);
                        break;
                    }
                }
                if (currentIslandObjects >= properties.MaxInstances)
                {
                    CustomLogger.Log("Stranded Deep 2K Mod : flora MaxInstances reached 2 : " + currentIslandObjects, debugLog);
                    break;
                }
            }
        }
    }
}
