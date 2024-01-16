using Beam;
using Beam.Rendering;
using Beam.Utilities;
using Beam.Serialization;
using Beam.UI;
using HarmonyLib;
using SharpNeatLib.Maths;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using Beam.Serialization.Json;
using Beam.Terrain;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        public static FieldInfo fi_localImpostorCullingDistance = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo fi_Scope = typeof(LodController).GetField("_scope", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo mi_CreateImpostor = typeof(LodController).GetMethod("CreateImpostor", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(LodManager), "LoadOptions")]
        class LodManager_LoadOptions_PostFix
        {
            static void Postfix(LodController __instance)
            {
                try
                {
                    if (increaseLODs)
                    {
                        SetUltraQuality();
                    }
                    if (increaseOceanLOD)
                    {
                        SetUltraOceanQuality();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching LodManager_LoadOptions_PostFix : " + e);
                }
            }
        }

        static bool isPreLoading = false;

        [HarmonyPatch(typeof(StrandedWorld), "ZoneLoader_LoadedZone")]
        class StrandedWorld_ZoneLoader_LoadedZone_PreFix
        {
            static bool Prefix(StrandedWorld __instance, Zone zone)
            {
                try
                {
                    if (isPreLoading)
                    {
                        zone.HasVisited = hasVisited;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching StrandedWorld_ZoneLoader_LoadedZone_PreFix : " + e);
                }
                return true;
            }
        }
#warning for debug
        //[HarmonyPatch(typeof(StrandedWorld), "ZoneLoader_LoadedZone")]
        //class FlockController_StrandedWorld_ZoneLoader_LoadedZone_Patch
        //{

        //    static void Postfix(Zone zone, StrandedWorld __instance)
        //    {
        //        try
        //        {
        //            LocalizedNotification localizedNotification = new LocalizedNotification(new Notification());
        //            localizedNotification.Priority = NotificationPriority.Immediate;
        //            localizedNotification.Duration = 8f;
        //            localizedNotification.TitleText.SetTerm("Zone " + (isPreLoading ? "pre" : "") + " loaded");
        //            if (zone == StrandedWorld.Instance.NmlZone)
        //            {
        //                localizedNotification.MessageText.SetTerm("Zone NML loaded");
        //            }
        //            else
        //            {
        //                localizedNotification.MessageText.SetTerm("Zone " + zone.ZoneName + (isPreLoading ? "pre" : "") + " loaded");
        //            }
        //            localizedNotification.Raise();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("Stranded Deep LOD Mod : error while patching StrandedWorld.ZoneLoader_LoadedZone : " + e);
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(LodController), "Start")]
        class LodController_Start_Postfix
        {
            static void Postfix(LodController __instance)
            {
                try
                {
                    if (increaseLODs && !ultraDistance)
                    {
                        // WARNING : the zone unload distance must be greater than the farthest LOD cull distance, or the impostor won't show up

                        //int impostorCullDistance = 501;
                        //fi_localImpostorCullingDistance.SetValue(__instance, 501);
                        List<int> cullDistances = new List<int>();
                        cullDistances.Add(196);
                        cullDistances.Add(197);
                        cullDistances.Add(198);
                        cullDistances.Add(199);
                        for (int i = 0; i < __instance.LodGroup.Lods.Count; i++)
                        {
                            Lod lod = __instance.LodGroup.Lods[i];
                            if (lod.IsImpostor)
                            {
                                //lod.CullingDistance = impostorCullDistance;
                                //Debug.Log("Stranded Deep LOD mod LodController_Start_Postfix : impostor culling distance = " + lod.CullingDistance);
                                continue;
                            }

                            //Debug.Log("Stranded Deep LOD mod LodController_Start_Postfix : LOD " + i + " culling distance = " + lod.CullingDistance);
                            lod.CullingDistance = cullDistances[i];
                            //Debug.Log("Stranded Deep LOD mod LodController_Start_Postfix : LOD " + i + " new culling distance = " + lod.CullingDistance);
                        }
                    }
                    else if (increaseLODs && ultraDistance)
                    {
                        List<int> cullDistances = new List<int>();
                        cullDistances.Add(500);
                        cullDistances.Add(600);
                        cullDistances.Add(700);
                        cullDistances.Add(1000);
                        for (int i = 0; i < __instance.LodGroup.Lods.Count; i++)
                        {
                            Lod lod = __instance.LodGroup.Lods[i];
                            if (lod.IsImpostor)
                            {
                                //lod.CullingDistance = impostorCullDistance;
                                //Debug.Log("Stranded Deep LOD mod LodController_Start_Postfix : impostor culling distance = " + lod.CullingDistance);
                                continue;
                            }
                            //Debug.Log("Stranded Deep LOD mod LodController_Start_Postfix : ultra LOD " + i + " culling distance = " + lod.CullingDistance);
                            if (i == __instance.LodGroup.Lods.Count - 1)
                            {
                                // max LOD, should always be visible
                                lod.CullingDistance = 1500;
                            }
                            else
                            {
                                lod.CullingDistance = cullDistances[i];
                            }
                            //Debug.Log("Stranded Deep LOD mod LodController_Start_Postfix : ultra LOD " + i + " new culling distance = " + lod.CullingDistance);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching LodController_Start_Postfix : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(LodController), "ValidateLodGroup")]
        class LodController_ValidateLodGroup_Patch
        {
            static bool Prefix(LodController __instance, LodGroup lodGroup, ref string errorMessage, ref bool __result)
            {
                try
                {
                    if (increaseLODs)
                    {
                        int maxCullingDistance = 1499;
                        if (__instance.Scope.HasFlag(ImposterScope.Manual))
                        {
                            __result = true;
                        }
                        foreach (Lod lod in lodGroup.Lods)
                        {
                            if (lod.CullingDistance <= 0 || lod.CullingDistance > maxCullingDistance)
                            {
                                errorMessage = string.Format("Lod culling distances must be between 0-{0} units.", maxCullingDistance);
                                __result = false;
                            }
                        }
                        __result = true;

                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching LodController.ValidateLodGroup : " + e);
                }
                return true;
            }
        }

        static FieldInfo fi_dither = typeof(LodController).GetField("_dither", BindingFlags.NonPublic | BindingFlags.Instance);
        [HarmonyPatch(typeof(LodController), "UpdateLodGroup")]
        class LodController_UpdateLodGroup_Prefix
        {
            static bool Prefix(LodController __instance, Camera camera)
            {
                try
                {
                    float magnitude = (camera.transform.position - __instance.transform.position).magnitude;
                    Lod previousLod = null;
                    bool gotActiveLod = false;
                    for (int i = 0; i < __instance.LodGroup.Lods.Count; i++)
                    {
                        Lod currentLod = __instance.LodGroup.Lods[i];
                        if (gotActiveLod && !currentLod.IsImpostor)
                        {
                            currentLod.SetActive(false);
                            continue;
                        }
                        int? previousLodCullingDistance = (previousLod != null) ? new int?(previousLod.CullingDistance) : null;
                        float biasedPreviousLodCullingDistance = ((previousLodCullingDistance != null) ? ((float)previousLodCullingDistance.GetValueOrDefault()) : 0f) * LodManager.LOD_BIAS;
                        float biasedCurrentLodCullingDistance = (float)currentLod.CullingDistance * LodManager.LOD_BIAS;
                        bool positionFartherThanBiasedPreviousLodCullingDistance = magnitude > biasedPreviousLodCullingDistance;
                        //Debug.Log("Stranded Deep LOD mod : " + __instance.gameObject.name + " LOD " + i + " isImpostor " + lod2.IsImpostor + " magnitude > biasedPreviousLodCullingDistance " + positionFartherThanBiasedPreviousLodCullingDistance);
                        bool positionNearerThanBiasedCurrentLodCullingDistance = magnitude < biasedCurrentLodCullingDistance;
                        //Debug.Log("Stranded Deep LOD mod : " + __instance.gameObject.name + " LOD " + i + " isImpostor " + lod2.IsImpostor + " magnitude < biasedCurrentLodCullingDistance " + positionNearerThanBiasedCurrentLodCullingDistance);
                        bool active = positionFartherThanBiasedPreviousLodCullingDistance && positionNearerThanBiasedCurrentLodCullingDistance;
                        currentLod.SetActive(active);
                        if (currentLod.Active)
                        {
                            gotActiveLod = true;
                            //Debug.Log("Stranded Deep LOD mod : " + __instance.gameObject.name + " LOD " + i + " adopted");
                        }
                        if ((bool)fi_dither.GetValue(__instance) && currentLod.Active && __instance.LodGroup.Lods.IsLast(i))
                        {
                            float num4 = Mathf.Min(7f, biasedCurrentLodCullingDistance); // private float _ditherFadeWidth = 7f;
                            float num5 = biasedCurrentLodCullingDistance - num4;
                            float value = Mathf.Clamp01((magnitude - num5) / num4);
                            currentLod.Crossfade(value);
                        }
                        previousLod = currentLod;
                    }

                    return false;

                    //for (int i = 0; i < __instance.LodGroup.Lods.Count; i++)
                    //{
                    //    Lod lod = __instance.LodGroup.Lods[i];
                    //    if (lod.IsImpostor && lod.Active)
                    //    {
                    //        float magnitude = (camera.transform.position - __instance.transform.position).magnitude;
                    //        if (magnitude <= 200)
                    //        {
                    //            Debug.Log("Stranded Deep LOD mod : unintentionally active impostor " + __instance.gameObject.name);
                    //            lod.SetActive(false);
                    //        }
                    //    }
                    //}
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching LodController.ValidateLodGroup : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(LodController), "AdoptImpostor")]
        class LodController_AdoptImpostor_Postfix
        {
            static bool Prefix(LodController __instance, ImpostorBase impostor)
            {
                try
                {
                    if (!ultraDistance)
                        return true;

                    fi_Scope.SetValue(__instance, ImposterScope.Terrain);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching LodController_AdoptImpostor_Postfix : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerDetailSpawnerManager), "StrandedWorld_WorldGenerated")]
        class PlayerDetailSpawnerManager_StrandedWorld_WorldGenerated_Patch
        {
            static void Postfix(PlayerDetailSpawnerManager __instance)
            {
                try
                {
                    if (!moreFishes)
                        return;

                    if (!multiplyFishesDone)
                    {
                        MultiplyFishesNew();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching SingleFishRenderer.Awake : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(SingleFishRenderer), "Awake")]
        class SingleFishRenderer_Awake_Patch
        {
            static void Postfix(FishRendererBase __instance)
            {
                try
                {
                    if (increaseFishDrawingDistance)
                        fi_FishRendererBase_cullingDistance.SetValue(__instance, 500f);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching SingleFishRenderer.Awake : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(FlockFishRenderer), "Start")]
        class FlockFishRenderer_Start_Patch
        {
            static void Postfix(FishRendererBase __instance)
            {
                try
                {
                    if (increaseFishDrawingDistance)
                        fi_FishRendererBase_cullingDistance.SetValue(__instance, 500f);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching FlockFishRenderer.Start : " + e);
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
                    if (!addJellyFishes && !addShrimps && !addSmallFishes)
                        return;

                    //Debug.Log("Stranded Deep LOD Mod : ObjectPoolManager.CreatePooled " + __result.name);
                    AddParticlesIfNeeded(__result);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching ObjectPoolManager.CreatePooled : " + e);
                }
            }
        }

        internal static FastRandom fr = null;

        internal static void AddParticlesIfNeeded(GameObject go)
        {
            try
            {
                try
                {
                    // Bypass weird error
                    // System.NullReferenceException
                    // at(wrapper managed - to - native) UnityEngine.Object.GetName(UnityEngine.Object)
                    String.IsNullOrEmpty(go.name);
                }
                catch (Exception e)
                {
                    return;
                }

                if (fr == null || String.IsNullOrEmpty(go.name) || go.GetComponent<ParticleSystem>() != null)
                    return;

                if (addJellyFishes)
                {
                    if (go.name.Contains("Table_Coral") && fr.Next(0, 100) >= 80)
                    {
                        //Debug.Log("Stranded Deep LOD Mod : adding jellyfish particle system to table coral");
                        ParticleSystem ps = go.AddComponent<ParticleSystem>();
                        JellyfishParticleSystem pst = go.AddComponent<JellyfishParticleSystem>();
                        ps.Play();
                    }
                }

                if (addShrimps)
                {
                    if (go.name.Contains("Coral_Rock") || go.name.Contains("CORAL_ROCK"))
                    {
                        //Debug.Log("Stranded Deep LOD Mod : adding shrimp particle system to coral rock");
                        ParticleSystem ps = go.AddComponent<ParticleSystem>();
                        ShrimpParticleSystem pst = go.AddComponent<ShrimpParticleSystem>();
                        ps.Play();
                    }
                }

                if (addSmallFishes)
                {
                    if ((go.name.Contains("Coral_Pink")
                        || go.name.Contains("Coral_White")
                        || go.name.Contains("Staghorn")) && fr.Next(0, 100) >= 50)
                    {
                        //Debug.Log("Stranded Deep LOD Mod : adding smallfishes particle system to coral");
                        ParticleSystem ps = go.AddComponent<ParticleSystem>();
                        SmallfishesParticleSystem pst = go.AddComponent<SmallfishesParticleSystem>();
                        ps.Play();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : AddParticleSpawners failed for " + (go.name == null ? "null" : go.name) + " : " + e);
                try
                {
                    ParticleSystem ps = go.GetComponent<ParticleSystem>();
                    Game.Destroy(ps);
                }
                catch { }
            }
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        [HarmonyPatch(typeof(FollowSpawn), "LoadOptions")]
        class FollowSpawn_LoadOptions_Patch
        {
            static void Postfix(FollowSpawn __instance)
            {
                try
                {
                    if (!ultraUnderwaterDetail)
                        return;

                    FieldInfo fi_GridObjects2 = typeof(FollowSpawn).GetField("GridObjects2", BindingFlags.NonPublic | BindingFlags.Instance);
                    FollowSpawn.BiomeGrid[] GridObjects2 = fi_GridObjects2.GetValue(__instance) as FollowSpawn.BiomeGrid[];

                    foreach (FollowSpawn.BiomeGrid biomeGrids in GridObjects2)
                    {
                        Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : biomeGrids.name = " + biomeGrids.name);
                        foreach (FollowSpawn.GridObject gridObject in biomeGrids.biomeParameters)
                        {
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : objectsToSpawn[0].name = " + gridObject.objectsToSpawn[0].name);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : maxObjects = " + gridObject.maxObjects);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : rarity = " + gridObject.rarity);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : maxHeight = " + gridObject.maxHeight);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : minHeight = " + gridObject.minHeight);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : randomYRotation = " + gridObject.randomYRotation);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : maxslope = " + gridObject.maxslope);
                            Debug.Log("Stranded Deep LOD mod : biomeGrid tweaking : minslope = " + gridObject.minslope);

                            if (gridObject.objectsToSpawn[0].name == "Kelp_1")
                            {
                                gridObject.rarity = 30;
                                gridObject.maxObjects = 50;
                                gridObject.minHeight = -50;
                            }
                            if (gridObject.objectsToSpawn[0].name == "PARTICLE_BUBBLE_STREAM")
                            {
                                gridObject.maxObjects = 10;
                                gridObject.minHeight = -50;
                            }
                            if (gridObject.objectsToSpawn[0].name == "SHORELINE_ROCK_2")
                            {
                                gridObject.rarity = 5;
                                gridObject.maxObjects = 30;
                                gridObject.minHeight = -50;
                                gridObject.maxslope = 90;
                            }
                            if (gridObject.objectsToSpawn[0].name == "Shoreline_Seaweed")
                            {
                                gridObject.rarity = 70;
                                gridObject.maxObjects = 300;
                                gridObject.minHeight = -50;
                                gridObject.maxslope = 90;
                            }
                            if (gridObject.objectsToSpawn[0].name == "CORAL_ROCK_1")
                            {
                                gridObject.maxObjects = 10;
                                gridObject.minHeight = -50;
                            }
                            if (gridObject.objectsToSpawn[0].name == "Table_Coral" || gridObject.objectsToSpawn[0].name == "Table_Coral_Brown_1")
                            {
                                gridObject.rarity = 30;
                                gridObject.maxObjects = 30;
                                gridObject.minHeight = -50;
                            }
                            if (gridObject.objectsToSpawn[0].name == "StagHorn_Coral")
                            {
                                gridObject.maxslope = 50;
                                gridObject.maxObjects = 40;
                                gridObject.minHeight = -50;
                                gridObject.maxslope = 90;
                            }
                            if (gridObject.objectsToSpawn[0].name == "Coral_Group_White")
                            {
                                gridObject.maxslope = 50;
                                gridObject.maxObjects = 40;
                                gridObject.minHeight = -50;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching FollowSpawn_LoadOptions_Patch : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(StrandedWorld), "InZoneUnLoadingBounds")]
        class StrandedWorld_InZoneUnLoadingBounds_PreFix
        {
            static bool Prefix(StrandedWorld __instance, ref bool __result, IPlayer player, Zone zone)
            {
                try
                {
                    if (!increaseLODs && !ultraDistance)
                        return true;

#warning stranded wide interaction
                    if (!WorldUtilities.IsStrandedWide())
                    {
                        float buffedUnloadDistance = 260f * 4;
                        __result = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(zone.transform.position.x, zone.transform.position.z)) < buffedUnloadDistance;

                        if (!__result)
                        {
                            _preloadedZones.Remove(zone);
                        }

                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching StrandedWorld_InZoneUnLoadingBounds_PreFix : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(LandAnimalSpawner), "PollSpawn")]
        class LandAnimalSpawner_PollSpawn_PreFix
        {
            static bool Prefix(LandAnimalSpawner __instance)
            {
                try
                {
                    if (!increaseLODs || !ultraDistance)
                        return true;

                    Zone zone = StrandedWorld.GetZone(__instance.transform.position, false);
                    if (_preloadedZones.Contains(zone))
                        return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching LandAnimalSpawner_PollSpawn_PreFix : " + e);
                }

                return true;
            }
        }

        static List<Zone> _preloadedZones = new List<Zone>();
        static bool hasVisited = false;

        [HarmonyPatch(typeof(StrandedWorld), "PollLoad")]
        class StrandedWorld_PollLoad_PreFix
        {
            static FieldInfo fi_zoneSavedObjectsLookup = typeof(StrandedWorld).GetField("_zoneSavedObjectsLookup", BindingFlags.NonPublic | BindingFlags.Instance);
            static MethodInfo mi_LoadZone = typeof(StrandedWorld).GetMethod("LoadZone", BindingFlags.NonPublic | BindingFlags.Instance);
            static FieldInfo fi_currentZone = typeof(ZoneLoader).GetField("_currentZone", BindingFlags.NonPublic | BindingFlags.Instance);
            static MethodInfo mi_CreateLoadedPrefab = typeof(ZoneLoader).GetMethod("CreateLoadedPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

            static bool Prefix(StrandedWorld __instance, Zone zone)
            {
                try
                {
                    if (!increaseLODs || !ultraDistance)
                        return true;

#warning stranded wide interaction
                    if (!WorldUtilities.IsStrandedWide())
                    {
                        // if in standard radius, standard behavior
                        if (!zone.Loaded && PlayerRegistry.AllPlayers.Any_NonAlloc(new Func<IPlayer, Zone, bool>(InZoneLoadingVanillaBounds), zone))
                        {
                            Debug.Log("Stranded Deep LOD mod : standard Load zone " + zone.ZoneName);
                            _preloadedZones.Remove(zone);
                            return true;
                        }

                        bool hasVisited = zone.HasVisited;

                        // try and preload objects without making this zone the current zone
                        if (!LevelLoader.IsLoading()
                            && !zone.Loading 
                            && !zone.Loaded 
                            && !_preloadedZones.Contains(zone) 
                            && PlayerRegistry.AllPlayers.Any_NonAlloc(new Func<IPlayer, Zone, bool>(InZoneLoadingExtendedBounds), zone))
                        {
                            isPreLoading = true;
                            Debug.Log("Stranded Deep LOD mod : PreLoad zone " + zone.ZoneName);
                            mi_LoadZone.Invoke(__instance, new object[] { zone });

                            if (zone != null)
                            {
                                if (zone.Loaded)
                                    _preloadedZones.Add(zone);
                            }

                            zone.HasVisited = hasVisited;

                            isPreLoading = false;
                        }
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching StrandedWorld_PollLoad_PreFix : " + e);
                }
                return true;
            }

            internal static bool InZoneLoadingVanillaBounds(IPlayer player, Zone zone)
            {
                return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(zone.transform.position.x, zone.transform.position.z)) < 240f;
            }

            /// <summary>
            /// Tests if island is in a "preloading" radius
            /// </summary>
            /// <param name="player"></param>
            /// <param name="zone"></param>
            /// <returns></returns>
            internal static bool InZoneLoadingExtendedBounds(IPlayer player, Zone zone)
            {
                float buffedLoadDistance = 250f * 4;
                return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(zone.transform.position.x, zone.transform.position.z)) < buffedLoadDistance;
            }
        }

        [HarmonyPatch(typeof(Zone), "Awake")]
        class Zone_Awake_Postfix
        {
            static void Postfix(Zone __instance)
            {
                try
                {
                    if (!increaseTerrainLOD)
                        return;

                    __instance.Terrain.heightmapPixelError = 5f;
                    __instance.Terrain.detailObjectDistance = 1000f;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching Zone_Awake_Postfix : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Zone), "Enter")]
        class Zone_Enter_Postfix
        {
            static void Postfix(Zone __instance)
            {
                try
                {
                    if (!increaseTerrainLOD)
                        return;

                    __instance.Terrain.heightmapPixelError = 5f;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching Zone_Enter_Postfix : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Zone), "Exit")]
        class Zone_Exit_Postfix
        {
            static void Postfix(Zone __instance)
            {
                try
                {
                    if (!increaseTerrainLOD)
                        return;

                    __instance.Terrain.heightmapPixelError = 5f;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching Zone_Exit_Postfix : " + e);
                }
            }
        }
    }
}
