﻿using Beam;
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

namespace StrandedDeepTweaksMod
{
    static partial class Main
    {
        private static int stackSizeRatio = 4;

        [HarmonyPatch(typeof(SlotStorage), "GetStackSize")]
        class SlotStorage_GetStackSize_Patch
        {
            static bool Prefix(SlotStorage __instance, CraftingType type, ref int __result)
            {
                try
                {
                    int result;
                    if (STACK_SIZES.TryGetValue(type.InteractiveType, out result))
                    {
                        __result = biggerStackSizes ? stackSizeRatio * result : result;
                    }
                    else
                    {
                        __result = biggerStackSizes ? stackSizeRatio * 4 : 4;
                    }

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching SlotStorage.GetStackSize : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(InteractiveObject), nameof(InteractiveObject.Use))]
        class InteractiveObject_Use_ReversePatch
        {
            [HarmonyReversePatch]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void Use(InteractiveObject instance)
            {
            }
        }

        [HarmonyPatch(typeof(InteractiveObject_AIRTANK), nameof(InteractiveObject_AIRTANK.Use))]
        class InteractiveObject_AIRTANK_Use_Patch
        {

            static bool Prefix(InteractiveObject_AIRTANK __instance)
            {
                try
                {
                    if (!infiniteAir)
                        return true;

                    float durabilityPoints = __instance.DurabilityPoints;
                    // infiniteAir
                    //__instance.DurabilityPoints = durabilityPoints - 1f;
                    //if (__instance.DurabilityPoints <= 0f)
                    //{
                    //    __instance.DurabilityPoints = 0f;
                    //    __instance.CanUse = false;
                    //    __instance.DisplayNamePrefixes.AddOrIgnore("ITEM_DISPLAY_NAME_PREFIX_EMPTY", -1);
                    //}
                    //__instance.Owner.Statistics.AddOxygen(__instance.Owner.Statistics.MaxOxygen);
                    FieldInfo fi_owner = typeof(InteractiveObject).GetField("_owner", BindingFlags.Instance | BindingFlags.NonPublic);
                    IPlayer owner = fi_owner.GetValue(__instance) as IPlayer;
                    owner.Statistics.AddOxygen(owner.Statistics.MaxOxygen);
                    //base.Use();
                    InteractiveObject_Use_ReversePatch.Use(__instance);

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching InteractiveObject_AIRTANK.Use : " + e);
                }
                return true;
            }
        }

        private static float biggerGasTankSize = 10f;

        [HarmonyPatch(typeof(MotorVehicleMovement), "get_FuelCapacity", new Type[] { })]
        class MotorVehicleMovement_get_FuelCapacity_Patch
        {
            static bool Prefix(RudderVehicleMovement __instance, ref float __result)
            {
                try
                {
                    if (!biggerGasTank)
                        return true;

                    __result = biggerGasTankSize;

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching MotorVehicleMovement.get_FuelCapacity : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MotorVehicleMovement), "Move", new Type[] { })]
        class MotorVehicleMovement_Move_Patch
        {
            static bool Prefix(MotorVehicleMovement __instance)
            {
                try
                {
                    if (!smallerRaftTurnAngle)
                        return true;

                    float newPropellerTurningAngle = 10.0f;
                    float newRudderDeflection = 10.0f;

                    float rudderInput = (float)fi_motorRudderInput.GetValue(__instance);

                    if (Mathf.Approximately(__instance.Steering, 0f))
                    {
                        //__instance._rudderInput = Mathf.Lerp(__instance._rudderInput, 0f, Time.deltaTime * 3f);
                        rudderInput = Mathf.Lerp(rudderInput, 0f, Time.deltaTime * 3f);
                    }
                    else
                    {
                        //__instance._rudderInput += Time.deltaTime * (__instance.Steering * 2f);
                        rudderInput += Time.deltaTime * (__instance.Steering * 2f);
                        //__instance._rudderInput = Mathf.Clamp(__instance._rudderInput, -1f, 1f);
                        rudderInput = Mathf.Clamp(rudderInput, -1f, 1f);
                    }
                    fi_motorRudderInput.SetValue(__instance, rudderInput);

                    float num = rudderInput * newPropellerTurningAngle * 0.017453292f;
                    float y = (-65f * rudderInput / 2f + num) % 360f;

                    //__instance._rudder.localEulerAngles = new Vector3(0f, y, 0f);
                    Transform rudder = fi_motorRudder.GetValue(__instance) as Transform;
                    rudder.localEulerAngles = new Vector3(0f, y, 0f);

                    //__instance._fuel -= ((__instance.Throttle > 0f) ? 0.45454544f : 0.1f) * GameTime.DeltaTimeHour;

                    float fuel = (float)fi_fuel.GetValue(__instance);
                    if (!infiniteGas)
                    {
                        fuel -= ((__instance.Throttle > 0f) ? 0.45454544f : 0.1f) * GameTime.DeltaTimeHour;
                        fi_fuel.SetValue(__instance, fuel);
                    }
                    else if (fuel != __instance.FuelCapacity)
                    {
                        fi_fuel.SetValue(__instance, __instance.FuelCapacity);
                    }

                    if (fuel > 0f)
                    {
                        bool anchorsDeployed = (bool)mi_AnchorsDeployed.Invoke(__instance, new object[] { });
                        //if (!__instance.AnchorsDeployed())
                        if (!anchorsDeployed)
                        {
                            Vector3 a = __instance.transform.forward * Mathf.Cos(num) - __instance.Rigidbody.transform.right * Mathf.Sin(num);
                            float engineForce = (float)fi_engineForce.GetValue(__instance);
                            //__instance.Rigidbody.AddForce(a * __instance._engineForce * __instance.Throttle);
                            __instance.Rigidbody.AddForce(a * engineForce * __instance.Throttle);
                        }
                    }
                    else if ((bool)fi_ranOutOfFuel.GetValue(__instance))
                    {
                        //__instance._fuel = 0f;
                        fi_fuel.SetValue(__instance, 0f);
                        //__instance._ranOutOfFuel = true;
                        fi_ranOutOfFuel.SetValue(__instance, true);
                        //__instance.OnFuelRanOut();
                        mi_OnFuelRanOut.Invoke(__instance, new object[] { });
                    }
                    float d = Vector3.Dot(__instance.Rigidbody.velocity, __instance.transform.forward);
                    __instance.Rigidbody.AddTorque(__instance.transform.up * d * newRudderDeflection * rudderInput);
                    __instance.OperatingPosition.transform.rotation = __instance.Operator.Character.PlayerCharacterPosition.rotation;

                    //__instance.OnUsing();
                    mi_OnUsing.Invoke(__instance, new object[] { });

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching MotorVehicleMovement.Move : " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RudderVehicleMovement), "Move", new Type[] { })]
        class RudderVehicleMovement_Move_Patch
        {
            static bool Prefix(RudderVehicleMovement __instance)
            {
                try
                {
                    if (!smallerRaftTurnAngle)
                        return true;

                    float newTurningAngle = 10.0f;
                    float newRudderDeflection = 10.0f;

                    float rudderInput = (float)fi_rudderInput.GetValue(__instance);

                    if (Mathf.Approximately(__instance.Steering, 0f))
                    {
                        //__instance._rudderInput = Mathf.Lerp(__instance._rudderInput, 0f, Time.deltaTime * 3f);
                        rudderInput = Mathf.Lerp(rudderInput, 0f, Time.deltaTime * 3f);
                    }
                    else
                    {
                        //__instance._rudderInput += Time.deltaTime * (__instance.Steering * 1f);
                        rudderInput += Time.deltaTime * (__instance.Steering * 1f);
                        //__instance._rudderInput = Mathf.Clamp(__instance._rudderInput, -1f, 1f);
                        rudderInput = Mathf.Clamp(rudderInput, -1f, 1f);
                    }
                    fi_rudderInput.SetValue(__instance, rudderInput);

                    float num = rudderInput * newTurningAngle * 0.017453292f;
                    float y = (-60f * rudderInput / 2f + num) % 360f;

                    //__instance._rudder.localEulerAngles = new Vector3(0f, y, 0f);
                    Transform rudder = fi_rudder.GetValue(__instance) as Transform;
                    rudder.localEulerAngles = new Vector3(0f, y, 0f);

                    float d = Vector3.Dot(__instance.Rigidbody.velocity, __instance.transform.forward);
                    __instance.Rigidbody.AddTorque(__instance.transform.up * d * newRudderDeflection * rudderInput);
                    __instance.OperatingPosition.transform.rotation = __instance.Operator.Character.PlayerCharacterPosition.rotation;

                    //__instance.OnUsing();
                    mi_OnUsing.Invoke(__instance, new object[] { });

                    // skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching RudderVehicleMovement.Move : " + e);
                }
                return true;
            }
        }

        private static int originalGameAirCapacity = 3;
        private static float biggerAirTankCapacity = 10f;

        [HarmonyPatch(typeof(InteractiveObject), "Awake")]
        class InteractiveObject_Awake_Patch
        {
            static void Postfix(InteractiveObject __instance)
            {
                try
                {
                    if (!biggerAirTank)
                        return;

                    if (__instance is InteractiveObject_AIRTANK)
                    {
                        var propertyInfo = typeof(InteractiveObject).GetProperty("OriginalDurabilityPoints", BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfo != null)
                        {
                            //Debug.Log("Stranded Deep Tweaks Mod : OriginalDurabilityPoints Property found");
                            MethodInfo mi = propertyInfo.GetSetMethod(true);
                            if (mi != null)
                            {
                                //Debug.Log("Stranded Deep Tweaks Mod : OriginalDurabilityPoints Property private setter found");
                                mi.Invoke(__instance, new object[] { biggerAirTankCapacity });

                                float newCharges = biggerAirTankCapacity - (originalGameAirCapacity - __instance.DurabilityPoints);
                                Debug.Log("Stranded Deep Tweaks Mod : setting airtank max capacity to : " + newCharges);
                                __instance.DurabilityPoints = newCharges;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod :  error while patching InteractiveObject.Awake : " + e);
                }
            }
        }

        //(io.name.Contains("STICK")
        //                    || io.name.Contains("COCONUT")
        //                    || io.name.Contains("SCRAP_PLANK")
        //                    || io.name.Contains("CONTAINER_CRATE")
        //                    || io.name.Contains("FROND")
        //                    || io.name.Contains("PALM_TOP")
        //                    || io.name.Contains("PALM_LOG")
        //                    || io.name.Contains("PADDLE")
        //                    || io.name.Contains("FUELCAN")
        //                    || io.name.Contains("LEAVES_FIBROUS")
        //                    || io.name.Contains("WOLLIE")
        //                    || io.name.Contains("COCONUT_FLASK")
        //                    || io.name.Contains("MEDICAL")
        //                }
        static List<uint> _floatingPrefabIds = new List<uint>() { 15, 18, 156, 98, 12, 161, 162, 163, 164, 281, 28, 29, 174, 32, 269, 270, 271, 272 };

        private static void AddBuoyancy(SaveablePrefab __result)
        {
            if (!addBuoyancies)
                return;

            InteractiveObject io = __result.gameObject.GetComponent<InteractiveObject>();
            if (io == null)
                return;

            Debug.Log("Stranded Deep Tweaks Mod : adding buoyancy to " + io.name);
            io.gameObject.SetActive(false);
            Buoyancy newbuoy = io.gameObject.AddComponent<Buoyancy>();
            fi_buoyancyDensity.SetValue(newbuoy, 600.0f);
            Rigidbody rb = fi_rigidbody.GetValue(newbuoy) as Rigidbody;
            if (rb == null)
            {
                //Debug.Log("Stranded Deep Tweaks Mod : buoyancy rigidbody is null");
                fi_rigidbody.SetValue(newbuoy, io.rigidbody);
            }
            io.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(SaveablePrefab), "Attached")]
        class World_LoadWorldMaps_Patch
        {
            static void Postfix(SaveablePrefab __instance)
            {
                try
                {
                    //Debug.Log("Stranded Deep Tweaks Mod : patching SaveablePrefab.Attached " + __instance.name);
                    if (addBuoyancies
                        && _floatingPrefabIds.Contains(__instance.PrefabId)
                        && __instance.gameObject.GetComponent<Buoyancy>() == null)
                    {
                        AddBuoyancy(__instance);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod : error while patching SaveablePrefab.Attached : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Interactive_STORAGE), "OnOpened")]
        class Interactive_STORAGE_OnOpened_Patch
        {
            static void Postfix(Interactive_STORAGE __instance, Interactive_STORAGE storage)
            {
                try
                {
                    //Debug.Log("Stranded Deep Tweaks Mod : patching Interactive_STORAGE_OnOpened " + __instance.name);
                    if (addBuoyancies
                        && __instance.gameObject.GetComponent<Buoyancy>() == null)
                    {
                        AddBuoyancy(__instance);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod : error while patching Interactive_STORAGE_OnOpened : " + e);
                }
            }
        }


        [HarmonyPatch(typeof(StrandedWorld), "ZoneLoader_LoadedZone")]
        class FlockController_StrandedWorld_ZoneLoader_LoadedZone_Patch
        {

            static void Postfix(Zone zone, StrandedWorld __instance)
            {
                try
                {
                    Debug.Log("Stranded Deep Tweaks Mod : ZoneLoader_LoadedZone " + zone.name);
                    typeof(StrandedWorld).GetMethod("OnZoneEntered", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(StrandedWorld.Instance, new object[] { zone });
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod : error while patching FlockController.StrandedWorld_ZoneEntered : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(FlockController), "StrandedWorld_ZoneEntered")]
        class FlockController_StrandedWorld_ZoneEntered_Patch
        {
            static void Postfix(Zone zone, FlockController __instance)
            {
                try
                {
                    Debug.Log("Stranded Deep Tweaks Mod : FLOCK CONTROLLER zone entered " + zone.name);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Tweaks Mod : error while patching FlockController.StrandedWorld_ZoneEntered : " + e);
                }
            }
        }
    }
}