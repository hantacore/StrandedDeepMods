using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using Beam.Utilities;
using SharpNeatLib.Maths;
using System.Reflection;
using Beam;

namespace StrandedDeepSharksMod
{
    static partial class Main
    {
        private static Harmony harmony;

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        //[HarmonyPatch(typeof(Piscus_Creature), "PollSpawn")]
        //class Piscus_Creature_PollSpawn_Patch
        //{
        //    static bool Prefix(Piscus_Creature __instance)
        //    {
        //        try
        //        {
        //            IPlayer player = PlayerUtilities.FindClosestPlayer(__instance.transform.position, true);
        //            if (player == null)
        //            {
        //                return false;
        //            }
        //            bool flag = Vector3Tools.DistanceOnAxis(__instance.transform.position, player.transform.position, Vector3Tools.xz) < __instance._spawnDistance;
        //            if ((this._canRespawn && !flag && this._respawningTimer > this._respawnHours) || this._respawningTimer == 0)
        //            {
        //                this._respawningTimer = 0;
        //                GameObject gameObject = MultiplayerMng.Instantiate<SaveablePrefab>(this._piscusCreature.GetComponent<SaveablePrefab>().PrefabId, base.transform.position, Quaternion.identity, MiniGuid.New(), null).gameObject;
        //                gameObject.transform.parent = Singleton<PiscusInspectionManager>.Instance.transform;
        //                this._activeCreature = gameObject.GetComponent<Piscus_Creature>();
        //                this._activeCreature.BiomeSize = this._territoryRadius;
        //                this._activeCreature.EngagePlayerDistance = this._activeCreature.BiomeSize * 0.5f;
        //                this._activeCreature.Manager = this;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("Stranded Deep Shark mod : error while patching Piscus_Creature.SetDifficultyParams : " + e);
        //        }
        //        return false;
        //    }
        //}

        [HarmonyPatch(typeof(Piscus_Creature), "Initialize")]
        class Piscus_Creature_Initialize_Patch
        {
            static void Postfix(Piscus_Creature __instance)
            {
                try
                {
                    __instance.gameObject.SetLayerRecursively(Layers.WATER);

                    InteractiveObject_PISCUS ip = __instance.GetComponentInParent<InteractiveObject_PISCUS>();
                    Debug.Log("Stranded Deep Shark Mod : name " + ip.gameObject.name);
                    Debug.Log("Stranded Deep Shark Mod : prefab " + ip.PrefabId);
                    // 10 TIGER
                    // 11 WHITE
                    // 34 WHALE
                    // 331 HAMMERHEAD
                    // 332 WHALESHARK
                    // 333 GOBLIN

                    if (ip.PrefabId == 10 || ip.PrefabId == 331)
                    {
                        // random shark size
                        SharkSize size = SharkSize.Normal;
                        //int sharksize = r.Next(100);
                        int sharksize = _scaleRandomizer.Next(0, 1000) / 10;
                        Debug.Log("Stranded Deep Shark Mod : randomize scale = " + sharksize);
                        if (sharksize < 15)
                        {
                            size = SharkSize.Baby;
                        }
                        else if (sharksize < 30)
                        {
                            size = SharkSize.Small;
                        }
                        else if (sharksize < 40)
                        {
                            size = SharkSize.Medium;
                        }
                        else if (sharksize < 90)
                        {
                            size = SharkSize.Normal;
                        }
                        else if (sharksize < 98)
                        {
                            size = SharkSize.Big;
                        }
                        else
                        {
                            size = SharkSize.Huge;
                        }

                        float scale = GetScaleBySize(size);
                        // randomize scale
                        Debug.Log("Stranded Deep Shark Mod : randomize scale for " + ip.gameObject.name + " : " + scale);
                        ip.transform.localScale = new Vector3(scale, scale, scale);
                        //Piscus_Creature pc = fi_piscusCreature.GetValue(ip) as Piscus_Creature;
                        //if (pc != null)
                        //{
                        //    Main.SetAggressivityBySize(size, pc);
                        //}
                        Main.SetAggressivityBySize(size, __instance);

                        __instance.gameObject.name = __instance.gameObject.name + " - " + sharksize.ToString();
                    }
                    else if (ip.PrefabId == 332)
                    {
                        if (ip.transform != null && fi_maxSpeed != null && fi_maxSpeed != null)
                        {
                            if (ip.transform != null)
                            {
                                ip.transform.localScale = new Vector3(2, 2.5f, 3f);
                                Debug.Log("Stranded Deep Shark Mod : changing Whale Shark scale");
                            }

                            if (fi_maxSpeed != null)
                            {
                                Piscus_Creature pc = fi_piscusCreature.GetValue(ip) as Piscus_Creature;
                                if (pc != null)
                                {
                                    float currentSpeed = (float)fi_maxSpeed.GetValue(pc);
                                    Debug.Log("Stranded Deep Shark Mod : setting Whale Shark max speed to " + currentSpeed / 2f);
                                    fi_maxSpeed.SetValue(pc, currentSpeed / 2f);
                                }
                            }

                            Debug.Log("Stranded Deep Shark Mod : Whale Shark updated ");
                        }
                    }

                    __instance.CurrentState = Piscus_Creature.PiscusBehaviour.Explore;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Shark mod : error while patching Piscus_Creature.Initialize : " + e);
                }
            }
        }

        //[HarmonyPatch(typeof(Construction_CAMPFIRE), "Detach")]
        //class Construction_CAMPFIRE_Detach_Patch
        //{
        //    static bool Prefix(Construction_CAMPFIRE __instance)
        //    {
        //        try
        //        {
        //            if (WorldUtilities.IsWorldLoaded())
        //            {
        //                Debug.Log("Stranded Deep RaftStructures Mod : Construction_CAMPFIRE food detached");
        //                Main.DetachFood(__instance.Food);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("Stranded Deep Raftstructures mod : error while patching Construction_CAMPFIRE.Detach : " + e);
        //        }
        //        return true;
        //    }
        //}
    }
}
