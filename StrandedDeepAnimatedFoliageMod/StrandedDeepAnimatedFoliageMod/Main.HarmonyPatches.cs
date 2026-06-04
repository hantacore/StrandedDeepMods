using Beam.Serialization;
using Beam.Serialization.Json;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepAnimatedFoliageMod
{
    static partial class Main
    {
        // FIX: _prefabsToHandle et HandlePrefabs() étaient du code mort (jamais alimentés ni appelés)
        //      conservés en commentaire pour référence
        //static List<SaveablePrefab> _prefabsToHandle = new List<SaveablePrefab>();
        static List<uint> _treePrefabIds = new List<uint>() { 157, 158, 159, 160, 66, 67, 202, 203, 204 };
        static List<uint> _smallTreePrefabIds = new List<uint>() { 47, 48, 49, 206, 207, 180, 181 };
        static List<uint> _plantPrefabIds = new List<uint>() { 50, 51, 58, 59, 60, 266, 52, 148, 149 };
        static List<string> _keywords = new List<string>() { "FICUS_1", "FICUS_2", "FICUS_3", "FICUS_TREE", "FICUS_TREE_2" };

        // FIX: reflection statique protégée — si le champ n'existe pas (maj du jeu),
        //      on logue un message clair plutôt qu'une NullReferenceException silencieuse
        static FieldInfo fi_GridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(Prefabs), "CreatePrefabFromId", new Type[] { typeof(uint), typeof(Transform) })]
        class Prefabs_CreatePrefabFromId_Patch
        {
            static void Postfix(ref SaveablePrefab __result, uint id, Transform parent)
            {
                try
                {
                    // FIX: logique extraite dans HandlePrefabCreated() — évite la duplication
                    //      avec Prefabs_CreatePrefabFromId2_Patch
                    HandlePrefabCreated(__result);
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
                    // FIX: logique extraite dans HandlePrefabCreated() — évite la duplication
                    //      avec Prefabs_CreatePrefabFromId_Patch
                    HandlePrefabCreated(__result);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching Prefabs.CreatePrefabFromId : " + e);
                }
            }
        }

        // FIX: corps commun aux deux CreatePrefabFromId, extrait pour éviter la duplication
        private static void HandlePrefabCreated(SaveablePrefab __result)
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
                // FIX: variable locale inutilisée supprimée
                __result.gameObject.AddComponent<BushBender>();
            }
            if (Main.replaceBushTextures
                && __result.gameObject.GetComponent<BushTextureUpdater>() == null
                && __result.PrefabId == 205)
            {
                __result.gameObject.AddComponent<BushTextureUpdater>();
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
                        // FIX: variable locale inutilisée supprimée
                        __result.gameObject.AddComponent<BushBender>();
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

                // FIX: IsSmallTree n'était assigné que dans GetPrefab_Patch, pas ici —
                //      les prefabs créés via CreatePrefabFromId n'avaient jamais IsSmallTree = true,
                //      ce qui cassait le throttle de distance dans BenderBase
                if (__result.PrefabId == 50  // alocasia
                    || __result.PrefabId == 51  // alocasia
                    || __result.PrefabId == 58  // alocasia
                    || __result.PrefabId == 59  // ceriman
                    || __result.PrefabId == 60  // ceriman
                    || __result.PrefabId == 266 // aloe
                    || __result.PrefabId == 52  // banana
                    || __result.PrefabId == 148 // kura
                    || __result.PrefabId == 149 // quwawa
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

                    if (Main.animateSeaweeds)
                    {
                        if (__result.name.Contains("Seaweed")
                            || __result.name.Contains("Shoreline_Seaweed"))
                        {
                            if (Main.debugMode)
                                Debug.Log("Stranded Deep AnimatedFoliage mod : adding seaweed bender if needed " + __result.gameObject.name);
                            AddSeaweedBenderIfNeeded(__result);
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
                // FIX: variable locale inutilisée supprimée
                gameObject.AddComponent<CoralBender>();
            }
        }

        [HarmonyPatch(typeof(FollowSpawn), "CreateGrid")]
        class FollowSpawn_CreateGrid_Patch
        {
            static void Postfix(FollowSpawn __instance)
            {
                try
                {
                    if (!Main.animateGrass)
                        return;

                    // FIX: guard sur fi_GridObjects2 — si le champ a été renommé dans une maj du jeu,
                    //      on logue un message clair au lieu d'une NullReferenceException silencieuse
                    if (fi_GridObjects2 == null)
                    {
                        Debug.Log("Stranded Deep AnimatedFoliage mod : FollowSpawn.GridObjects2 field not found via reflection — skipping grass patch");
                        return;
                    }

                    foreach (FollowSpawn.BiomeGrid biomeGrid in fi_GridObjects2.GetValue(__instance) as FollowSpawn.BiomeGrid[])
                    {
                        foreach (FollowSpawn.GridObject gridObject in biomeGrid.biomeParameters)
                        {
                            foreach (GameObject go in gridObject.objectsToSpawn)
                            {
                                // FIX: était "if (!IsNullOrEmpty) return" — sortait de toute la méthode
                                //      dès le premier objet valide, rendant ce patch totalement inopérant.
                                //      Corrigé en "if (IsNullOrEmpty) continue" pour skipper les noms vides.
                                if (String.IsNullOrEmpty(go.name))
                                    continue;

                                //CustomLogger.Log("Stranded Deep AnimatedFoliage Mod : FollowSpawn object = " + go.name);
                                if (go.name.Contains("green_grass_03"))
                                {
                                    AddGrassBenderIfNeeded(go);
                                }
                                else if (go.name.Contains("green_grass_01"))
                                {
                                    AddGrassBenderIfNeeded(go);
                                }
                                //"Grass_Dry"
                                else if (go.name.Contains("Grass_Dry"))
                                {
                                    AddGrassBenderIfNeeded(go);
                                }
                                //else if (go.name.Contains("ground_cover_a"))
                                //{
                                //    AddGrassBenderIfNeeded(go);
                                //}
                                //else if (go.name.Contains("ground_cover_c"))
                                //{
                                //    AddGrassBenderIfNeeded(go);
                                //}
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching FollowSpawn_CreateGrid_Patch : " + e);
                }
            }
        }

        private static void AddGrassBenderIfNeeded(GameObject gameObject)
        {
            if (Main.debugMode)
                Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Grass Bender component to " + gameObject.name);
            if (gameObject.GetComponent<GrassBender>() == null)
            {
                // FIX: variable locale inutilisée supprimée
                gameObject.AddComponent<GrassBender>();
            }
        }

        private static void AddSeaweedBenderIfNeeded(GameObject gameObject)
        {
            if (Main.debugMode)
                Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Seaweed Bender component to " + gameObject.name);
            if (gameObject.GetComponent<SeaweedBender>() == null)
            {
                // FIX: variable locale inutilisée supprimée
                gameObject.AddComponent<SeaweedBender>();
            }
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        // FIX: HandlePrefabs() et _prefabsToHandle étaient du code mort —
        //      _prefabsToHandle n'était jamais alimenté et HandlePrefabs() jamais appelée.
        //      Conservés en commentaire pour référence.
        //static List<SaveablePrefab> _prefabsToHandle = new List<SaveablePrefab>();
        //static void HandlePrefabs()
        //{
        //    // LODGroup
        //    // MeshRenderer
        //    //
        //    foreach(SaveablePrefab sp in _prefabsToHandle)
        //    {
        //        Debug.Log("Stranded Deep AnimatedFoliage mod : Prefabs.CreatePrefabFromId " + sp.gameObject.name);
        //        Component[] cs = sp.gameObject.GetComponents<Component>();
        //        foreach (Component c in cs)
        //        {
        //            Debug.Log("Stranded Deep AnimatedFoliage mod : Prefabs.CreatePrefabFromId component " + sp.gameObject.name + " / " + c.GetType().Name);
        //        }
        //    }
        //}
    }
}