﻿using Beam;
using Beam.Rendering;
using Beam.Serialization;
using HarmonyLib;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        public static FieldInfo fi_Cull = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo fi_Scope = typeof(LodController).GetField("_scope", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo mi_CreateImpostor = typeof(LodController).GetMethod("CreateImpostor", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(LodController), "Start")]
        class LodController_Start_Patch
        {
            static void Postfix(LodController __instance)
            {
                try
                {
                    if (increasLODs)
                    {
                        fi_Scope.SetValue(__instance, ImposterScope.Manual);

                        int dist = (int)fi_Cull.GetValue(__instance);
                        fi_Cull.SetValue(__instance, 2000);

                        int bestLOD = 3;
                        for (int i = 0; i < __instance.LodGroup.Lods.Count; i++)
                        {
                            Lod lod = __instance.LodGroup.Lods[i];
                            if (lod.Renderers[0].name.Contains("LOD0") && bestLOD > 0)
                            {
                                bestLOD = 0;
                            }
                            else if (lod.Renderers[0].name.Contains("LOD1") && bestLOD > 1)
                            {
                                bestLOD = 1;
                            }
                            else if (lod.Renderers[0].name.Contains("LOD2") && bestLOD > 2)
                            {
                                bestLOD = 2;
                            }
                        }

                        int farthest = int.MinValue;

                        for (int i = __instance.LodGroup.Lods.Count - 1; i >= 0; i--)
                        {
                            Lod lod = __instance.LodGroup.Lods[i];

                            if (!lod.IsImpostor)
                            {
                                lod.CullingDistance = lod.CullingDistance * (Main.ultraMFBBQDistance ? 1000 : (Main.ultraDistance ? 10 : 5));
                                if (lod.CullingDistance > farthest)
                                {
                                    farthest = lod.CullingDistance + 1;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching LodController.Start : " + e);
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
                    if (increasLODs)
                    {
                        int maxCullingDistance = 1000;
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
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching LodController.ValidateLodGroup : " + e);
                }
                return true;
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
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching SingleFishRenderer.Awake : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(FlockFishRenderer), "Start")]
        class FlockFishRenderer_Awake_Patch
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
                    Debug.Log("Stranded Deep AnimatedFoliage mod : error while patching FlockFishRenderer.Start : " + e);
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

                if (String.IsNullOrEmpty(go.name) || go.GetComponent<ParticleSystem>() != null)
                    return;

                if (addJellyFishes)
                {
                    if (go.name.Contains("Table_Coral") && fr.Next(0, 100) >= 50)
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
                    if (go.name.Contains("Coral_Pink")
                        || go.name.Contains("Coral_White")
                        || go.name.Contains("Staghorn"))
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
            }
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }
    }
}