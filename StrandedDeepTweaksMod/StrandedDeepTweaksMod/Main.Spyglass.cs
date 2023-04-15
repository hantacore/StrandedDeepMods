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
using Beam.Events;

namespace StrandedDeepTweaksMod
{
    static partial class Main
    {
        // Token: 0x0400197C RID: 6524
        private static float MinZoom = 4f;

        // Token: 0x0400197D RID: 6525
        private static float MaxZoom = 30f;

        [HarmonyPatch(typeof(SpyGlass), "get_FULL_ZOOM_FOV")]
        class SpyGlass_get_FULL_ZOOM_FOV_Patch
        {
            static bool Prefix(SpyGlass __instance, ref float __result)
            {
                try
                {
                    if (!betterSpyglass)
                        return true;

                    __result = Mathf.Atan(Mathf.Tan(Cameras.DefaultFOV * 0.017453292f) / MaxZoom) * 57.29578f;

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching SpyGlass.get_FULL_ZOOM_FOV : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SpyGlass), "get_X2_ZOOM_FOV")]
        class SpyGlass_get_X2_ZOOM_FOV_Patch
        {
            static bool Prefix(SpyGlass __instance, ref float __result)
            {
                try
                {
                    if (!betterSpyglass)
                        return true;

                    __result = Mathf.Atan(Mathf.Tan(Cameras.DefaultFOV * 0.017453292f) / MinZoom) * 57.29578f;

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching SpyGlass.get_X2_ZOOM_FOV : " + e);
                }
                return true;
            }
        }

        static FieldInfo fi_currentZoomLevel = AccessTools.Field(typeof(SpyGlass), "_currentZoomLevel");
        static FieldInfo fi_currentNormalZoom = AccessTools.Field(typeof(SpyGlass), "_currentNormalZoom");
        static FieldInfo fi_targetFOV = AccessTools.Field(typeof(SpyGlass), "_targetFOV");
        static FieldInfo fi_currentFOV = AccessTools.Field(typeof(SpyGlass), "_currentFOV");
        static FieldInfo fi_currentNormalFOV = AccessTools.Field(typeof(SpyGlass), "_currentNormalFOV");
        static FieldInfo fi_player = AccessTools.Field(typeof(SpyGlass), "_player");
        static PropertyInfo pi_FULL_ZOOM_FOV = AccessTools.Property(typeof(SpyGlass), "FULL_ZOOM_FOV");
        static MethodInfo mi_Zoom = AccessTools.Method(typeof(SpyGlass), "Zoom");

        [HarmonyPatch(typeof(SpyGlass), "SetZoomPercent")]
        class SpyGlass_SetZoomPercent_Patch
        {
            static bool Prefix(SpyGlass __instance, float percent)
            {
                try
                {
                    if (!betterSpyglass)
                        return true;

                    //__instance._currentZoomLevel = Mathf.Lerp(__instance._currentNormalZoom, MaxZoom, percent);
                    float currentZoomLevel = Mathf.Lerp((float)fi_currentNormalZoom.GetValue(__instance), MaxZoom, percent);
                    fi_currentZoomLevel.SetValue(__instance, currentZoomLevel);
                    //__instance._targetFOV = Mathf.Lerp(__instance.FULL_ZOOM_FOV, __instance._currentNormalFOV, 1f - percent);
                    float targetFOV = Mathf.Lerp((float)pi_FULL_ZOOM_FOV.GetValue(__instance), (float)fi_currentNormalFOV.GetValue(__instance), 1f - percent);
                    fi_targetFOV.SetValue(__instance, targetFOV);
                    //__instance._currentFOV = __instance._player.PlayerCamera.Camera.fieldOfView;
                    float currentFOV = ((IPlayer)fi_player.GetValue(__instance)).PlayerCamera.Camera.fieldOfView;
                    fi_currentFOV.SetValue(__instance, currentFOV);
                    LeanTween.cancel(__instance.gameObject);
                    //LeanTween.value(__instance.gameObject, new Action<float>(__instance.Zoom), __instance._currentFOV, __instance_targetFOV, 0.3f);
                    LeanTween.value(__instance.gameObject,
                        AccessTools.MethodDelegate<Action<float>>(mi_Zoom, __instance),
                        currentFOV, targetFOV, 0.3f);
                    EventManager.RaiseEvent<SpyGlassZoomChangedEvent>(new SpyGlassZoomChangedEvent(currentZoomLevel));

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching SpyGlass.SetZoomPercent : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SpyGlass), "Setup")]
        class SpyGlass_Setup_Patch
        {
            static void Postfix(InteractiveObject __instance, bool activate)
            {
                try
                {
                    if (!betterSpyglass)
                        return;

                    fi_currentNormalZoom.SetValue(__instance, MinZoom);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching SpyGlass.Setup : " + e);
                }
            }
        }
    }
}
