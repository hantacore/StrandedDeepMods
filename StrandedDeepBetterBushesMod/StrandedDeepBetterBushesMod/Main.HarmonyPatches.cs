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

namespace StrandedDeepBetterBushesMod
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
            if (Main.replaceBushTextures
                && __result.gameObject.GetComponent<BushTextureUpdater>() == null
                && __result.PrefabId == 205)
            {
                __result.gameObject.AddComponent<BushTextureUpdater>();
            }
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }
    }
}