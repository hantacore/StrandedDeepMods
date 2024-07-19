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
        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        internal static FieldInfo fi_player = typeof(Statistics).GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPatch(typeof(Statistics), "Load")]
        class Statistics_Load_Postfix_Patch
        {
            static void Postfix(Statistics __instance, JObject data)
            {
                try
                {
                    Debug.Log("Stranded Deep " + MODNAME + " load statistics");
                    InitEffects();

                    IPlayer player = fi_player.GetValue(__instance) as IPlayer;
                    int currentPlayerIndex = PlayerRegistry.AllPlayers.IndexOf(player);

                    Debug.Log("Stranded Deep " + MODNAME + " Mod : check and replace loaded effects");
                    for (int effectIndex = 0; effectIndex < __instance.PlayerEffects.Count(); effectIndex++)
                    {
                        PlayerEffect pe = __instance.PlayerEffects.ElementAt(effectIndex);
                        Debug.Log("Stranded Deep " + MODNAME + " Mod : found " + pe.Name);
                        if (pe.Name == Wet.WET && !System.Object.ReferenceEquals(pe, wetEffect))
                        {
                            Debug.Log("Stranded Deep " + MODNAME + " Mod : replacing wet effect");
                            __instance.RemoveStatusEffect(pe);
                            wetEffect[currentPlayerIndex].InitFromLoaded(pe);
                            __instance.ApplyStatusEffect(wetEffect[currentPlayerIndex]);
                        }
                        if (pe.Name == BodyTemperature.BODY_TEMPERATURE && !System.Object.ReferenceEquals(pe, bodytemperatureEffect))
                        {
                            Debug.Log("Stranded Deep " + MODNAME + " Mod : replacing temperature effect");
                            __instance.RemoveStatusEffect(pe);
                            bodytemperatureEffect[currentPlayerIndex].InitFromLoaded(pe);
                            __instance.ApplyStatusEffect(bodytemperatureEffect[currentPlayerIndex]);
                            nextCough = DateTime.Now;
                        }
                        if (pe.Name == Sick.SICK && !System.Object.ReferenceEquals(pe, sickEffect))
                        {
                            Debug.Log("Stranded Deep " + MODNAME + " Mod : replacing sick effect");
                            __instance.RemoveStatusEffect(pe);
                            sickEffect[currentPlayerIndex].InitFromLoaded(pe);
                            __instance.ApplyStatusEffect(sickEffect[currentPlayerIndex]);
                        }
                        if (pe.Name == Fever.FEVER && !System.Object.ReferenceEquals(pe, feverEffect))
                        {
                            Debug.Log("Stranded Deep " + MODNAME + " Mod : replacing fever effect");
                            __instance.RemoveStatusEffect(pe);
                            feverEffect[currentPlayerIndex].InitFromLoaded(pe);
                            __instance.ApplyStatusEffect(feverEffect[currentPlayerIndex]);
                        }
                    }

                    Main.lastCheck = DateTime.MinValue;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep " + MODNAME + " Mod : error while patching Statistics_Load_Postfix_Patch postfix : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Statistics), "UpdateVitals")]
        class Statistics_UpdateVitals_Postfix_Patch
        {
            static void Postfix(Statistics __instance, bool healthy = true)
            {
                try
                {
                    DateTime nextCheck = lastCheck.AddMinutes(ParameterValues.STATUS_CHECK_INTERVAL_GAMETIME_MINUTES);
                    bool shouldUpdate = (DateTime.Compare(GameTime.Now, nextCheck) >= 0);
                    IPlayer player = fi_player.GetValue(__instance) as IPlayer;
                    if (!shouldUpdate || player.Movement == null)
                        return;

                    //if (player.Movement.GodMode || __instance.Invincible)
                    //{
                    //    debugText.text = "GodMode true";
                    //    return;
                    //}

                    //Debug.Log("Stranded Deep " + MODNAME + " update vitals");
                    temperatureStringBuilder.Clear();
                    wetnessStringBuilder.Clear();

                    int currentPlayerIndex = PlayerRegistry.AllPlayers.IndexOf(player);
                    TimeSpan elapsed = GameTime.Now.Subtract(lastCheck);
                    int elapsedMinutes = (int)elapsed.TotalMinutes;
                    //int elapsedMinutes = (int)GameTime.DeltaTimeMinute;
                    //Debug.Log("Stranded Deep " + MODNAME + " update vitals elapsedMinutes = " + elapsedMinutes); 
                    UpdateWetness(player, currentPlayerIndex, elapsedMinutes, AtmosphereStorm.Instance.Rain, Main.CurrentRain);
                    UpdateBodyTemperature(player, currentPlayerIndex, elapsedMinutes, false);
                    UpdateEnergy(player, currentPlayerIndex, elapsedMinutes, false);
                    UpdateSick(player, currentPlayerIndex, elapsedMinutes, false);
                    UpdateFever(player, currentPlayerIndex, elapsedMinutes);

                    lastCheck = GameTime.Now;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep "+ MODNAME +" Mod : error while patching Statistics_UpdateVitals_Postfix_Patch postfix : " + e);
                }
            }
        }
    }
}
