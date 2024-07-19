using Beam;
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

namespace StrandedDeepWetAndColdMod
{
    static partial class Main
    {
        [HarmonyPatch(typeof(AtmosphereStorm), "CheckWeather")]
        class AtmosphereStorm_CheckWeather_Postfix_Patch
        {
            static void Postfix(AtmosphereStorm __instance)
            {
                try
                {
                    if (!fixRainReset)
                        return;

                    Debug.Log("Stranded Deep " + MODNAME + " Mod : CheckWeather " + ((__instance.CurrentWeatherEvent == null) ? "CurrentWeatherEvent NULL" : "CurrentWeatherEvent NOT NULL"));
                    if (__instance.CurrentWeatherEvent != null)
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : CheckWeather " + __instance.CurrentWeatherEvent.IsActive);
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : CheckWeather " + __instance.CurrentWeatherEvent.Humidity);
                    }

                    if (__instance.CurrentWeatherEvent != null && __instance.CurrentWeatherEvent.IsActive && Mathf.Approximately(__instance.CurrentWeatherEvent.Humidity, 100f))
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : CheckWeather fixing rain value 1");
                        fi_rain.SetValue(__instance, 1);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : error while patching AtmosphereStorm_CheckWeather_Postfix_Patch : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(AtmosphereStorm), "StartWeatherEvent")]
        class AtmosphereStorm_StartWeatherEvent_Postfix_Patch
        {
            static void Postfix(AtmosphereStorm __instance)
            {
                try
                {
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : StartWeatherEvent");
                    if (!fixRainReset)
                        return;

                    Debug.Log("Stranded Deep " + MODNAME + " Mod : StartWeatherEvent " + ((__instance.CurrentWeatherEvent == null) ? "CurrentWeatherEvent NULL" : "CurrentWeatherEvent NOT NULL"));
                    if (__instance.CurrentWeatherEvent != null)
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : StartWeatherEvent " + __instance.CurrentWeatherEvent.IsActive);
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : StartWeatherEvent " + __instance.CurrentWeatherEvent.Humidity);
                    }

                    if (__instance.CurrentWeatherEvent != null && __instance.CurrentWeatherEvent.IsActive && Mathf.Approximately(__instance.CurrentWeatherEvent.Humidity, 100f))
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : StartWeatherEvent fixing rain value 1");
                        fi_rain.SetValue(__instance, 1);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : error while patching AtmosphereStorm_StartWeatherEvent_Postfix_Patch : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(AtmosphereStorm), "ClearWeatherEvent")]
        class AtmosphereStorm_ClearWeatherEvent_Prefix_Patch
        {
            static bool Prefix(AtmosphereStorm __instance)
            {
                try
                {
                    if (!fixRainReset)
                        return true;

                    Debug.Log("Stranded Deep " + MODNAME + " Mod : ClearWeatherEvent " + ((__instance.CurrentWeatherEvent == null) ? "CurrentWeatherEvent NULL" : "CurrentWeatherEvent NOT NULL"));
                    if (__instance.CurrentWeatherEvent != null)
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : ClearWeatherEvent " + __instance.CurrentWeatherEvent.IsActive);
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : ClearWeatherEvent " + __instance.CurrentWeatherEvent.Humidity);
                    }

                    if (__instance.CurrentWeatherEvent != null && !__instance.CurrentWeatherEvent.IsActive || __instance.Rain > 0)
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : ClearWeatherEvent resetting rain 0");
                        fi_rain.SetValue(__instance, 0);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : error while patching AtmosphereStorm_ClearWeatherEvent_Prefix_Patch : " + e);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(AtmosphereStorm), "Load")]
        class AtmosphereStorm_Load_Postfix_Patch
        {
            static void Postfix(AtmosphereStorm __instance, JObject data)
            {
                try
                {
                    if (!fixRainReset)
                        return;

                    Debug.Log("Stranded Deep " + MODNAME + " Mod : AtmosphereStorm Load " + ((__instance.CurrentWeatherEvent == null) ? "CurrentWeatherEvent NULL" : "CurrentWeatherEvent NOT NULL"));
                    if (__instance.CurrentWeatherEvent != null)
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : AtmosphereStorm Load " + __instance.CurrentWeatherEvent.IsActive);
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : AtmosphereStorm Load " + __instance.CurrentWeatherEvent.Humidity);
                    }

                    if (__instance.CurrentWeatherEvent != null && __instance.CurrentWeatherEvent.IsActive && Mathf.Approximately(__instance.CurrentWeatherEvent.Humidity, 100f))
                    {
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : Load fixing rain value 1");
                        fi_rain.SetValue(__instance, 1);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : error while patching AtmosphereStorm_Load_Postfix_Patch postfix : " + e);
                }
            }
        }
    }
}
