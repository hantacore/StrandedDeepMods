using Beam;
using Beam.Crafting;
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

namespace StrandedDeepRaftStructuresMod
{
    static partial class Main
    {
        private static Harmony harmony;


        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        [HarmonyPatch(typeof(Constructing), "Awake")]
        class Constructing_Awake_Patch
        {
            static void Postfix(Constructing __instance)
            {
                try
                {
                    if (_supportedStructures.ContainsKey(__instance.GetType()))
                    {
                        CollisionBoxUpdater updater = __instance.gameObject.AddComponent<CollisionBoxUpdater>();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Constructing.Awake : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Construction_CAMPFIRE), "Attach")]
        class Construction_CAMPFIRE_Attach_Patch
        {
            static void Postfix(InteractiveObject_FOOD food, int playerID, Construction_CAMPFIRE __instance)
            {
                try
                {
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        Main.AttachFoodToParentTransform(__instance, food);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Construction_CAMPFIRE.Attach : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Construction_CAMPFIRE), "Detach")]
        class Construction_CAMPFIRE_Detach_Patch
        {
            static bool Prefix(Construction_CAMPFIRE __instance)
            {
                try
                {
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        Debug.Log("Stranded Deep RaftStructures Mod : Construction_CAMPFIRE food detached");
                        Main.DetachFood(__instance.Food);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Construction_CAMPFIRE.Detach : " + e);
                }
                return true;
            }
        }

        private static FieldInfo fi_spitfire = typeof(Constructing_SPIT).GetField("_fire", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPatch(typeof(Constructing_SPIT), "Attach")]
        class Constructing_SPIT_Attach_Patch
        {
            static void Postfix(InteractiveObject_FOOD food, Constructing_SPIT __instance)
            {
                try
                {
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        Construction_CAMPFIRE fire = fi_spitfire.GetValue(__instance) as Construction_CAMPFIRE;
                        Main.AttachFoodToParentTransform(fire, food);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Constructing_SPIT.Attach : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Constructing_SPIT), "Detach")]
        class Constructing_SPIT_Detach_Patch
        {
            static bool Prefix(Constructing_SPIT __instance)
            {
                try
                {
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        Debug.Log("Stranded Deep RaftStructures Mod : Constructing_SPIT food detached");
                        Main.DetachFood(__instance.Food);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Constructing_SPIT.Detach : " + e);
                }
                return true;
            }
        }

        private static FieldInfo fi_smokerfire = typeof(Constructing_SMOKER).GetField("_fire", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPatch(typeof(Constructing_SMOKER), "Attach")]
        class Constructing_SMOKER_Attach_Patch
        {
            static void Postfix(InteractiveObject_FOOD food, Constructing_SMOKER __instance)
            {
                try
                {
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        Construction_CAMPFIRE fire = fi_smokerfire.GetValue(__instance) as Construction_CAMPFIRE;
                        Main.AttachFoodToParentTransform(fire, food);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Constructing_SMOKER.Attach : " + e);
                }
            }
        }

        [HarmonyPatch(typeof(Constructing_SMOKER), "Detach")]
        class Constructing_SMOKER_Detach_Patch
        {
            static bool Prefix(IAttachable attachable, Constructing_SMOKER __instance)
            {
                try
                {
                    if (WorldUtilities.IsWorldLoaded())
                    {
                        InteractiveObject_FOOD food = attachable as InteractiveObject_FOOD;
                        if (food != null)
                        {
                            Debug.Log("Stranded Deep RaftStructures Mod : Constructing_SMOKER food detached");
                            Main.DetachFood(food);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Raftstructures mod : error while patching Constructing_SPIT.Detach : " + e);
                }
                return true;
            }
        }
    }
}
