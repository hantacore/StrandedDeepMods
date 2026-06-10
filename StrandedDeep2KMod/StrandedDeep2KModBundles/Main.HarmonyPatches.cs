using Beam.Rendering;
using Beam.Serialization;
using Beam.Serialization.Json;
using HarmonyLib;
using StrandedDeep2KModBundles.TextureUpdater;
using System;
using System.Collections.Generic;
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
        static List<uint> _replaceModelsPrefabIds = null;
        static List<uint> _cliffPrefabIds = null;//new List<uint>() { 68, 69, 70, 71, 72, 73 };
        static List<uint> _genericPrefabIds = null;//new List<uint>() { 135, 142, 143, 198, 156, 99, 157, 158, 159, 160, 66, 67, 202, 203, 204, 47, 48, 49, 206, 207, 180, 181, 50, 51, 58, 59, 60, 266, 52, 148, 149 };
        static List<uint> _clothPrefabIds = null;
        static List<string> _handledPooled = new List<string>();

        static void ResetLists()
        {
            _replaceModelsPrefabIds = new List<uint>() { 205, 66 }; // bush + ficus
            _cliffPrefabIds = new List<uint>() { 68, 69, 70, 71, 72, 73 };
            _genericPrefabIds = new List<uint>() { 135, 142, 143, 198, 99, 157, 158, 159, 160, 66, 67, 202,
                203, 204, 47, 48, 49, 206, 207, 180, 181, 50, 51, 58, 59, 60, 266, 52, 148, 149,
                35, 38, 308, 30, 310, 311, 20, 21, 22, 23, 24,
                248, 249,
                13, 53, 54, 55, 56, 57, 61, 62, 63, 64, 65,
                9, 10, //tiger shark
                18, 19, 156,
                224, 98 // items
                };
            _clothPrefabIds = new List<uint>() { 39 };
            _handledPooled.Clear();
        }

        //static List<SaveablePrefab> _prefabsToHandle = new List<SaveablePrefab>();
        //static List<uint> _treePrefabIds = new List<uint>() { 157, 158, 159, 160, 66, 67, 202, 203, 204 };
        //static List<uint> _smallTreePrefabIds = new List<uint>() { 47, 48, 49, 206, 207, 180, 181 };
        //static List<uint> _plantPrefabIds = new List<uint>() { 50, 51, 58, 59, 60, 266, 52, 148, 149 };
        //static List<string> _keywords = new List<string>() { "FICUS_1", "FICUS_2", "FICUS_3", "FICUS_TREE", "FICUS_TREE_2" };

        [HarmonyPatch(typeof(Prefabs), "CreatePrefabFromId", new Type[] { typeof(uint), typeof(Transform) })]
        class Prefabs_CreatePrefabFromId_Patch
        {
            static void Postfix(ref SaveablePrefab __result, uint id, Transform parent)
            {
                try
                {
                    if (__result == null || __result.gameObject == null)
                        return;
                    uint prefabId = __result.PrefabId;
                    CustomLogger.Log("Stranded Deep 2K mod : CreatePrefabFromId " + prefabId);
                    if (!__result.gameObject.activeSelf)
                        return;
                    if (Main.replaceBushes && prefabId == 205)
                    {
                        CustomLogger.Log("Stranded Deep 2K mod : CreatePrefabFromId add model replacer");
                        AddModelReplacerIfNeeded<ModelReplacerBase>(__result.gameObject);
                        return;
                    }
                    if (Main.replaceFicus && prefabId == 66)
                    {
                        CustomLogger.Log("Stranded Deep 2K mod : CreatePrefabFromId add model replacer");
                        AddModelReplacerIfNeeded<ModelReplacerBase>(__result.gameObject);
                        return;
                    }
                    if (_clothPrefabIds.Contains(prefabId))
                    {
                        CustomLogger.Log("Stranded Deep 2K mod : CreatePrefabFromId add ClothTarpTextureUpdater");
                        AddTextureUpdaterIfNeeded<ClothTarpTextureUpdater>(__result.gameObject);
                        _clothPrefabIds.RemoveAll(item => item == prefabId);
                        return;
                    }
                    //if (_genericPrefabIds.Contains(prefabId))
                    //{
                        CustomLogger.Log("Stranded Deep 2K mod : CreatePrefabFromId add GenericTextureUpdater");
                        AddTextureUpdaterIfNeeded<GenericTextureUpdater>(__result.gameObject);
                        _genericPrefabIds.RemoveAll(item => item == prefabId);
                        return;
                    //}


                    //if (_cliffPrefabIds.Contains(__result.PrefabId))
                    //{
                    //    AddTextureUpdaterIfNeeded<CliffTextureUpdater>(__result.gameObject);
                    //    _cliffPrefabIds.Remove(__result.PrefabId);
                    //}
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching Prefabs.CreatePrefabFromId : " + e);
                }
            }
        }


        [HarmonyPatch(typeof(Prefabs), "CreatePrefabFromId", new Type[] { typeof(JObject), typeof(Transform) })]
        class Prefabs_CreatePrefabFromId2_Patch
        {
            static void Postfix(ref SaveablePrefab __result, JObject prefabIdData, Transform parent)
            {
                try
                {
                    if (__result == null || __result.gameObject == null)
                        return;

                    uint prefabId = __result.PrefabId;
                    CustomLogger.Log("Stranded Deep 2K mod : CreatePrefabFromId 2 " + prefabId);
                    if (!__result.gameObject.activeSelf)
                        return;
                    if (_clothPrefabIds.Contains(prefabId))
                    {
                        AddTextureUpdaterIfNeeded<ClothTarpTextureUpdater>(__result.gameObject);
                        _clothPrefabIds.RemoveAll(item => item == prefabId);
                        return;
                    }

                    //if (_genericPrefabIds.Contains(prefabId))
                    //{
                        AddTextureUpdaterIfNeeded<GenericTextureUpdater>(__result.gameObject);
                        _genericPrefabIds.RemoveAll(item => item == prefabId);
                    //}

                    //if (_cliffPrefabIds.Contains(__result.PrefabId))
                    //{
                    //    AddTextureUpdaterIfNeeded<CliffTextureUpdater>(__result.gameObject);
                    //    _cliffPrefabIds.Remove(__result.PrefabId);
                    //}
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep StrandedDeep 2K mod : error while patching Prefabs.CreatePrefabFromId 2 : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Prefabs), "GetPrefab", new Type[] { typeof(uint) })]
        class Prefabs_GetPrefab_Patch
        {
            static void Postfix(ref SaveablePrefab __result, uint id)
            {
                try
                {
                    if (__result == null || __result.gameObject == null)
                        return;

                    uint prefabId = __result.PrefabId;
                    CustomLogger.Log("Stranded Deep 2K mod : GetPrefab " + prefabId);
                    if (!__result.gameObject.activeSelf)
                        return;
                    if (_clothPrefabIds.Contains(prefabId))
                    {
                        AddTextureUpdaterIfNeeded<ClothTarpTextureUpdater>(__result.gameObject);
                        _clothPrefabIds.RemoveAll(item => item == prefabId);
                        return;
                    }
                    //if (_genericPrefabIds.Contains(prefabId))
                    //{
                        AddTextureUpdaterIfNeeded<GenericTextureUpdater>(__result.gameObject);
                        _genericPrefabIds.RemoveAll(item => item == prefabId);
                    //}
                    //if (_cliffPrefabIds.Contains(__result.PrefabId))
                    //{
                    //    AddTextureUpdaterIfNeeded<CliffTextureUpdater>(__result.gameObject);
                    //    _cliffPrefabIds.Remove(__result.PrefabId);
                    //}
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching Prefabs.GetPrefab : " + e);
                }
            }
        }

        static FieldInfo fi_GridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(FollowSpawn), "CreateGrid")]
        class FollowSpawn_CreateGrid_Patch
        {
            static void Postfix(FollowSpawn __instance)
            {
                try
                {
                    foreach (FollowSpawn.BiomeGrid biomeGrid in fi_GridObjects2.GetValue(__instance) as FollowSpawn.BiomeGrid[])
                    {
                        foreach (FollowSpawn.GridObject gridObject in biomeGrid.biomeParameters)
                        {
                            foreach (GameObject go in gridObject.objectsToSpawn)
                            {
                                if (!String.IsNullOrEmpty(go.name)
                                    && _handledPooled.Contains(go.name))
                                    return;

                                //CustomLogger.Log("Stranded Deep 2K Mod : FollowSpawn object = " + go.name);
                                if (go.name.Contains("green_grass_03"))
                                {
                                    AddTextureUpdaterIfNeeded<GenericTextureUpdater>(go);
                                    _handledPooled.Add(go.name);
                                }
                                else if (go.name.Contains("green_grass_01"))
                                {
                                    AddTextureUpdaterIfNeeded<GenericTextureUpdater>(go);
                                    _handledPooled.Add(go.name);
                                }
                                //"Grass_Dry"
                                else if (go.name.Contains("Grass_Dry"))
                                {
                                    AddTextureUpdaterIfNeeded<GenericTextureUpdater>(go);
                                    _handledPooled.Add(go.name);
                                }
                                else if (go.name.Contains("ground_cover_a"))
                                {
                                    AddTextureUpdaterIfNeeded<GenericTextureUpdater>(go);
                                    _handledPooled.Add(go.name);
//#warning ground cover
//                                    go.transform.localScale = new Vector3(2.0f, 1.0f, 2.0f);
                                }
                                else if (go.name.Contains("ground_cover_c"))
                                {
                                    AddTextureUpdaterIfNeeded<GenericTextureUpdater>(go);
                                    _handledPooled.Add(go.name);
//#warning ground cover
//                                    go.transform.localScale = new Vector3(2.0f, 1.0f, 2.0f);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching FollowSpawn_CreateGrid_Patch : " + e);
                }
            }
        }


        [HarmonyPatch(typeof(ObjectPoolManager), "CreatePooled")]
        class ObjectPoolManager_CreatePooled_Patch
        {
            static void Postfix(ref GameObject __result, GameObject prefab, Vector3 position, Quaternion rotation)
            {
                try
                {
                    //CustomLogger.Log("Stranded Deep 2K mod : follow spawn prefab : " + prefab.name);
                    //CustomLogger.Log("Stranded Deep 2K mod : follow spawn __result : " + __result.name);

                    if (!String.IsNullOrEmpty(__result.name)
                        && _handledPooled.Contains(__result.name))
                        return;

                    if (__result.name.Contains("Kelp_1")
                            || __result.name.Contains("Kelp_2")
                            || __result.name.Contains("Coral_Group_Red")
                            || __result.name.Contains("Coral_Group_Pink")
                            || __result.name.Contains("Coral_Group_White")
                            || __result.name.Contains("Brain_Coral")
                            || __result.name.Contains("Sea_Urchins")
                            || __result.name.Contains("Seaweed")
                            || __result.name.Contains("StagHorn_Coral")
                            || __result.name.Contains("Staghorn2_coral")
                            || __result.name.Contains("Table_Coral_Brown")
                            || __result.name.Contains("Table_Coral")
                            || __result.name.Contains("Tube_Coral_Blue")
                            || __result.name.Contains("Tube_Coral_Orange")
                            || __result.name.Contains("Tube_Coral_Purple")
                            || __result.name.Contains("Tube_Coral_Brown")
                            || __result.name.Contains("PARTICLE_BUBBLE_STREAM"))
                    {
                        AddTextureUpdaterIfNeeded<GenericTextureUpdater>(__result);
                        _handledPooled.Add(__result.name);
                        return;
                    }



                    //CustomLogger.Log("Stranded Deep 2K Mod : ObjectPoolManager.CreatePooled " + __result.name);
                    if (__result.name.ToLower().Contains("coral_rock") && !__result.name.Contains("LOD"))
                    {
                        AddTextureUpdaterIfNeeded<CoralRockTextureUpdater>(__result);
                        _handledPooled.Add(__result.name);
                    }
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching ObjectPoolManager.CreatePooled : " + e);
                }
            }
        }

        internal static FieldInfo fi_activeCreature = typeof(PiscusManager).GetField("_activeCreature", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPatch(typeof(PiscusManager), "PollSpawn")]
        class PiscusManager_PollSpawn_Patch
        {
            static void Postfix(PiscusManager __instance)
            {
                try
                {
                    Piscus_Creature activeCreature = fi_activeCreature.GetValue(__instance) as Piscus_Creature;
                    if (activeCreature != null)
                    {
                        AddTextureUpdaterIfNeeded<GenericTextureUpdater>(activeCreature.gameObject);
                    }
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching PiscusManager_PollSpawn_Patch : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(LodController), "Start")]
        class LodController_Start_Patch
        {
            static void Postfix(LodController __instance)
            {
                try
                {
                    CustomLogger.Log("Stranded Deep 2K mod : LodController.Start");
                    for (int i = 0; i < __instance.LodGroup.Lods.Count; i++)
                    {
                        Lod lod = __instance.LodGroup.Lods[i];
                        foreach (Renderer renderer in lod.Renderers)
                        {
                            TextureReplacerHelper.ReplaceTextures(renderer, __instance.name);
                        }
                    }
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching LodController.Start : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Beam.FishBase), "Start")]
        class FishBase_Start_Patch
        {
            static void Postfix(Beam.FishBase __instance)
            {
                try
                {
                    AddTextureUpdaterIfNeeded<GenericTextureUpdater>(__instance.gameObject);
                }
                catch (Exception e)
                {
                    CustomLogger.Log("Stranded Deep 2K mod : error while patching FishBase.Start : " + e);
                }
            }
        }

        private static void AddTextureUpdaterIfNeeded<T>(GameObject gameObject) where T : TextureUpdaterBase
        {
            CustomLogger.Log("Stranded Deep 2K Mod : adding " + typeof(T).Name + " component to " + gameObject.name);
            if (gameObject.GetComponent<T>() == null)
            {
                T tub = gameObject.AddComponent<T>();
            }
        }

        private static void AddModelReplacerIfNeeded<T>(GameObject gameObject) where T : ModelReplacerBase
        {
            CustomLogger.Log("Stranded Deep 2K Mod : adding " + typeof(T).Name + " component to " + gameObject.name);
            if (gameObject.GetComponent<T>() == null)
            {
                T tub = gameObject.AddComponent<T>();
            }
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }
    }
}
