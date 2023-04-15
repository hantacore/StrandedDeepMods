using Beam.Serialization;
using Beam.Serialization.Json;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepAnimatedFoliageMod
{
    static partial class Main
    {
        static List<SaveablePrefab> _prefabsToHandle = new List<SaveablePrefab>();
        static List<uint> _treePrefabIds = new List<uint>() { 157, 158, 159, 160, 66, 67, 202, 203, 204 };
        static List<uint> _smallTreePrefabIds = new List<uint>() { 47, 48, 49, 206, 207, 180, 181 };
        static List<uint> _plantPrefabIds = new List<uint>() { 50, 51, 58, 59, 60, 266, 52, 148, 149 };
        static List<string> _keywords = new List<string>() { "FICUS_1", "FICUS_2", "FICUS_3", "FICUS_TREE", "FICUS_TREE_2" };

        [HarmonyPatch(typeof(Prefabs), "CreatePrefabFromId", new Type[] { typeof(uint), typeof(Transform) })]
        class Prefabs_CreatePrefabFromId_Patch
        {
            static void Postfix(ref SaveablePrefab __result, uint id, Transform parent)
            {
                try
                {
                    if (Main.animateTrees)
                    {
                        if (_treePrefabIds.Contains(__result.PrefabId)
                            || Main.animateSmallTrees && _smallTreePrefabIds.Contains(__result.PrefabId)
                            || Main.animatePlants && _plantPrefabIds.Contains(__result.PrefabId))
                        {
                            if (Main.debugMode)
                                Debug.Log("Stranded Deep AnimatedFoliage mod : Adding tree bender if needed " + __result.gameObject.name);
                            AddTreeBenderIfNeeded(__result);
                        }
                    }
                    if (Main.replaceTreeLeaves
                        && __result.gameObject.GetComponent<FicusTextureUpdater>() == null
                        && (__result.PrefabId == 66
                        || __result.PrefabId == 67
                        || __result.PrefabId == 47
                        || __result.PrefabId == 48
                        || __result.PrefabId == 49))
                    {
                        if (Main.debugMode)
                            Debug.Log("Stranded Deep AnimatedFoliage mod : Adding FicusTextureUpdater if needed " + __result.gameObject.name);
                        __result.gameObject.AddComponent<FicusTextureUpdater>();
                    }

                    if (Main.animateBushes
                        && __result.PrefabId == 205
                        && __result.gameObject.GetComponent<BushBender>() == null)
                    {
                        if (Main.debugMode)
                            Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Bush Bender component");
                        BushBender tb = __result.gameObject.AddComponent<BushBender>();
                    }
                    if (Main.replaceBushTextures
                        && __result.gameObject.GetComponent<BushTextureUpdater>() == null
                        && __result.PrefabId == 205)
                    {
                        __result.gameObject.AddComponent<BushTextureUpdater>();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching Prefabs.CreatePrefabFromId : " + e);
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
                    if (Main.animateTrees)
                    {
                        if (_treePrefabIds.Contains(__result.PrefabId)
                            || Main.animateSmallTrees && _smallTreePrefabIds.Contains(__result.PrefabId)
                            || Main.animatePlants && _plantPrefabIds.Contains(__result.PrefabId))
                        {
                            if (Main.debugMode)
                                Debug.Log("Stranded Deep AnimatedFoliage mod : adding tree bender if needed " + __result.gameObject.name);
                            AddTreeBenderIfNeeded(__result);
                        }
                    }
                    if (Main.replaceTreeLeaves
                        && __result.gameObject.GetComponent<FicusTextureUpdater>() == null
                        && (__result.PrefabId == 66
                        || __result.PrefabId == 67
                        || __result.PrefabId == 47
                        || __result.PrefabId == 48
                        || __result.PrefabId == 49))
                    {
                        if (Main.debugMode)
                            Debug.Log("Stranded Deep AnimatedFoliage mod : Adding FicusTextureUpdater if needed " + __result.gameObject.name);
                        __result.gameObject.AddComponent<FicusTextureUpdater>();
                    }

                    if (Main.animateBushes
                        && __result.PrefabId == 205
                        && __result.gameObject.GetComponent<BushBender>() == null)
                    {
                        if (Main.debugMode)
                            Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Bush Bender component");
                        BushBender tb = __result.gameObject.AddComponent<BushBender>();
                    }
                    if (Main.replaceBushTextures
                        && __result.gameObject.GetComponent<BushTextureUpdater>() == null
                        && __result.PrefabId == 205)
                    {
                        __result.gameObject.AddComponent<BushTextureUpdater>();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching Prefabs.CreatePrefabFromId : " + e);
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
                    if (Main.animateTrees)
                    {
                        if (_treePrefabIds.Contains(__result.PrefabId)
                            || Main.animateSmallTrees && _smallTreePrefabIds.Contains(__result.PrefabId)
                            || Main.animatePlants && _plantPrefabIds.Contains(__result.PrefabId))
                        {
                            if (Main.debugMode)
                                Debug.Log("Stranded Deep AnimatedFoliage mod : adding tree bender if needed " + __result.gameObject.name);
                            AddTreeBenderIfNeeded(__result);
                        }
                    }
                    //if (Main.replaceTreeLeaves
                    //    && __result.gameObject.GetComponent<FicusTextureUpdater>() == null
                    //    && (__result.PrefabId == 66
                    //    || __result.PrefabId == 67
                    //    || __result.PrefabId == 47
                    //    || __result.PrefabId == 48
                    //    || __result.PrefabId == 49))
                    //{
                    //    Debug.Log("Stranded Deep AnimatedFoliage mod : Adding FicusTextureUpdater if needed " + __result.gameObject.name);
                    //    __result.gameObject.AddComponent<FicusTextureUpdater>();
                    //}

                    if (Main.animateBushes
                        && __result.PrefabId == 205
                        && __result.gameObject.GetComponent<BushBender>() == null)
                    {
                        if (Main.debugMode)
                            Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Bush Bender component");
                        BushBender tb = __result.gameObject.AddComponent<BushBender>();
                    }
                    //if (Main.replaceBushTextures
                    //    && __result.gameObject.GetComponent<BushTextureUpdater>() == null
                    //    && __result.PrefabId == 205)
                    //{
                    //    __result.gameObject.AddComponent<BushTextureUpdater>();
                    //}
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching Prefabs.GetPrefab : " + e);
                }
            }
        }

        private static void AddTreeBenderIfNeeded(SaveablePrefab __result)
        {
            if (__result.gameObject.GetComponent<TreeBender>() == null)
            {
                TreeBender tb = null;
                if (__result.PrefabId == 180
                    || __result.PrefabId == 181)
                {
                    tb = __result.gameObject.AddComponent<SmallTreeBender>();
                }
                else
                {
                    tb = __result.gameObject.AddComponent<TreeBender>();
                }
                if (__result.PrefabId == 202
                    || __result.PrefabId == 203
                    || __result.PrefabId == 204)
                {
                    tb.BendAngle = 2;
                }
                if (__result.PrefabId == 157
                    || __result.PrefabId == 158
                    || __result.PrefabId == 159
                    || __result.PrefabId == 160
                    || __result.PrefabId == 180
                    || __result.PrefabId == 181)
                {
                    tb.IsPalm = true;

                    //if (prefab is InteractiveObject_PALM)
                    //{
                    //    InteractiveObject_PALM palm = prefab as InteractiveObject_PALM;

                    //    FieldInfo fi_fruitSpawnerPrefab = typeof(InteractiveObject_PALM).GetField("_fruitSpawnerPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
                    //    GameObject spawner = fi_fruitSpawnerPrefab.GetValue(palm) as GameObject;
                    //    if (spawner != null)
                    //    {
                    //        spawner.transform.parent = palm.gameObject.transform;


                    //        //FieldInfo fi_fruit = typeof(InteractiveObject_PALM).GetField("_fruit", BindingFlags.NonPublic | BindingFlags.Instance);
                    //        //InteractiveObject_FOOD coconut = fi_fruit.GetValue(palm) as InteractiveObject_FOOD;
                    //        //if (coconut != null)
                    //        //{
                    //        //    coconut.gameObject.transform.parent = spawner.transform;
                    //        //}

                    //        //FieldInfo fi_fruits = typeof(InteractiveObject_PALM).GetField("_fruits", BindingFlags.NonPublic | BindingFlags.Instance);
                    //        //List<InteractiveObject_FOOD> coconuts = fi_fruits.GetValue(palm) as List<InteractiveObject_FOOD>;
                    //        //if (coconuts != null)
                    //        //{
                    //        //    foreach (InteractiveObject_FOOD coco in coconuts)
                    //        //    {
                    //        //        coco.gameObject.transform.parent = spawner.transform;
                    //        //    }
                    //        //}
                    //    }
                    //}
                }

                if (__result.PrefabId == 50 // alocasia
                    || __result.PrefabId == 51 // alocasia
                    || __result.PrefabId == 58 // alocasia
                    || __result.PrefabId == 59 // ceriman
                    || __result.PrefabId == 60// ceriman
                    || __result.PrefabId == 266// aloe
                    || __result.PrefabId == 52// banana
                    || __result.PrefabId == 148// kura
                    || __result.PrefabId == 149// quwawa
                    || __result.PrefabId == 206 // pine small
                    || __result.PrefabId == 207 // pine small
                    )
                {
                    tb.IsSmallTree = true;
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
                    if (Main.animateCorals)
                    {
                        //if (Main.debugMode)
                        //    Debug.Log("Stranded Wide (Harmony edition) : ObjectPoolManager.CreatePooled " + __result.name);
                        if (__result.name.Contains("Kelp_1")
                            || __result.name.Contains("Kelp_2")
                            || __result.name.Contains("Coral_Group_Red")
                            || __result.name.Contains("Coral_Group_Pink")
                            || __result.name.Contains("Coral_Group_White"))
                        {
                            if (Main.debugMode)
                                Debug.Log("Stranded Deep AnimatedFoliage mod : adding coral bender if needed " + __result.gameObject.name);
                            AddCoralBenderIfNeeded(__result);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching ObjectPoolManager.CreatePooled : " + e);
                }
            }
        }

        private static void AddCoralBenderIfNeeded(GameObject gameObject)
        {
            if (Main.debugMode)
                Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Coral Bender component to " + gameObject.name);
            if (gameObject.GetComponent<CoralBender>() == null)
            {
                CoralBender cb = gameObject.AddComponent<CoralBender>();
            }
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        static void HandlePrefabs()
        {
            // LODGroup
            // MeshRenderer
            // 

            foreach(SaveablePrefab sp in _prefabsToHandle)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod : Prefabs.CreatePrefabFromId " + sp.gameObject.name);
                Component[] cs = sp.gameObject.GetComponents<Component>();
                foreach (Component c in cs)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : Prefabs.CreatePrefabFromId component " + sp.gameObject.name + " / " + c.GetType().Name);
                }
            }
        }
    }
}
