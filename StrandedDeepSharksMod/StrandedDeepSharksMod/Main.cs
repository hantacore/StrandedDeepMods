using Beam.Rendering;
using Beam.Serialization;
using Beam.Serialization.Json;
using Beam.Utilities;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using Sirenix.Utilities;
using Funlabs;
using MEC;
using Beam;
using HarmonyLib;

namespace StrandedDeepSharksMod
{
    static partial class Main
    {
        //private static SharkHandler handler = new SharkHandler();

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        internal static bool perfCheck = true;

        //private static FastRandom r = new FastRandom();
        private static FastRandom _scaleRandomizer = new FastRandom();

        //prefabs.Add("SHARK - WHALE", "7b304494-2996-42d1-a442-0d02c6cebaf5");//332"
        private static string whaleSharkPrefabGuid = "7b304494-2996-42d1-a442-0d02c6cebaf5";
        private static uint whaleSharkPrefabId = 332;

        private static FieldInfo fi_piscusCreature = typeof(InteractiveObject_PISCUS).GetField("_piscusCreature", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo fi_minSpeed = null;//typeof(Piscus_Creature).GetField("_minSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_maxSpeed = null;//typeof(Piscus_Creature).GetField("_maxSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttack = null;//typeof(Piscus_Creature).GetField("_canAttack", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttackRafts = null;//typeof(Piscus_Creature).GetField("_canAttackRafts", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttackJump = null;//typeof(Piscus_Creature).GetField("_canAttackJump", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_canAttackNudge = null;//typeof(Piscus_Creature).GetField("_canAttackNudge", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_attackDamage = null;//typeof(Piscus_Creature).GetField("_attackDamage ", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_attackCritDamage = null;//typeof(Piscus_Creature).GetField("_attackCritDamage", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static FieldInfo fi_territoryRadius = typeof(PiscusManager).GetField("_territoryRadius", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static FieldInfo fi_respawningTimer = typeof(PiscusManager).GetField("_respawningTimer", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static FieldInfo fi_canRespawn = typeof(PiscusManager).GetField("_canRespawn", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static MethodInfo mi_PollSpawn = typeof(PiscusManager).GetMethod("PollSpawn", BindingFlags.Instance | BindingFlags.NonPublic);
        internal static MethodInfo mi_Activate_Creature = typeof(PiscusManager).GetMethod("Activate_Creature", BindingFlags.Instance | BindingFlags.NonPublic);

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;
            modEntry.OnUnload = OnUnload;

            fi_minSpeed = typeof(Piscus_Creature).GetField("_minSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_maxSpeed = typeof(Piscus_Creature).GetField("_maxSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_canAttack = typeof(Piscus_Creature).GetField("_canAttack", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_canAttackRafts = typeof(Piscus_Creature).GetField("_canAttackRafts", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_canAttackJump = typeof(Piscus_Creature).GetField("_canAttackJump", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_canAttackNudge = typeof(Piscus_Creature).GetField("_canAttackNudge", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_attackCritDamage = typeof(Piscus_Creature).GetField("_attackCritDamage", BindingFlags.NonPublic | BindingFlags.Instance);
            fi_attackDamage = typeof(Piscus_Creature).GetField("_attackDamage", BindingFlags.NonPublic | BindingFlags.Instance);

            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //handler.ModName = "Shark Mod";

            Debug.Log("Stranded Deep Shark Mod properly loaded");

            return true;
        }

        static int previousSharkIndex = 0;

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Sharks mod by Hantacore");
            //if (GUILayout.Button("Find shark"))
            //{
            //    if (handler != null)
            //    {
            //        if (handler.Sharks.Count < previousSharkIndex)
            //        {
            //            previousSharkIndex = 0;
            //        }
            //        Piscus_Creature shark = handler.Sharks[previousSharkIndex];
            //        previousSharkIndex++;
            //        if (shark.gameObject == null
            //            || shark.gameObject.transform == null)
            //        {
            //            return;
            //        }
            //        PlayerRegistry.LocalPlayer.transform.position = new Vector3(shark.transform.position.x, shark.transform.position.y, shark.transform.position.z); ;
            //    }
            //}
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                if (Beam.Game.State == GameState.NEW_GAME || Beam.Game.State == GameState.LOAD_GAME)
                {
                    //// anti memory leak
                    //if (previousWorldInstance != null
                    //    && !System.Object.ReferenceEquals(previousWorldInstance, StrandedWorld.Instance))
                    //{
                    //    Debug.Log("Stranded Deep AnimatedFoliage Mod : world instance changed, clearing events");
                    //    previousWorldInstance.WorldGenerated -= Instance_WorldGenerated;
                    //    previousWorldInstance = null;
                    //    worldLoaded = false;
                    //}
                    //// to reattach at the right moment
                    //if (StrandedWorld.Instance != null
                    //    && !System.Object.ReferenceEquals(StrandedWorld.Instance, previousWorldInstance))
                    //{
                    //    Debug.Log("Stranded Deep AnimatedFoliage Mod : world instance found, registering events");
                    //    previousWorldInstance = StrandedWorld.Instance;
                    //    StrandedWorld.Instance.WorldGenerated -= Instance_WorldGenerated;
                    //    StrandedWorld.Instance.WorldGenerated += Instance_WorldGenerated;
                    //}

                    if (WorldUtilities.IsWorldLoaded())
                    {
                        //r = new FastRandom(StrandedWorld.WORLD_SEED);
                        _scaleRandomizer = new FastRandom(StrandedWorld.WORLD_SEED);
                        //if (!WorldUtilities.IsStrandedWide() && PlayerRegistry.LocalPlayer != null)
                        if (PlayerRegistry.LocalPlayer != null)
                        {
                            Zone zone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                            if (zone != null)
                            {
                                //ReplaceSharks(zone);
                            }
                        }

                        //if (handler.MustFillQueue)
                        //{
                        //    handler.FillQueue(Game.FindObjectsOfType<Piscus_Creature>());
                        //}
                        //handler.Handle();
                    }
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Shark Mod : error when updating Whale Shark : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep Shark Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        internal static List<Zone> _handledZones = new List<Zone>();

        private static void ReplaceSharks(Zone zone)
        {
            if (zone == null || zone.IsUserMap || _handledZones.Contains(zone))
                return;

            // destroy existing spawners
            PiscusManager[] piscusManagers = Game.FindObjectsOfType<PiscusManager>();
            foreach(PiscusManager pm in piscusManagers)
            {
                if (pm.PrefabId == 218
                    || pm.PrefabId == 334
                    || pm.PrefabId == 335)
                {
                    Debug.Log("Stranded Deep Shark Mod : destroying shark spawner");
                    UnityEngine.Object.Destroy(pm.gameObject);
                }
            }

            GameObject gameObject = null;

            int radius = 80;//120;// 170;
            int offset = 0;// 110;// (_islandSize / 2);
            if (WorldUtilities.IsStrandedWide())
            {
                Debug.Log("Stranded Deep Shark Mod : Stranded Wide detected");
                radius = 110;
                offset = 110;
            }
            List<Vector3> positions = new List<Vector3>();
            positions.Add(new Vector3(offset + radius * Mathf.Cos(Mathf.Deg2Rad * 0), 0, offset + radius * Mathf.Sin(Mathf.Deg2Rad * 0)));
            positions.Add(new Vector3(offset + radius * Mathf.Cos(Mathf.Deg2Rad * 60), 0, offset + radius * Mathf.Sin(Mathf.Deg2Rad * 60)));
            positions.Add(new Vector3(offset + radius * Mathf.Cos(Mathf.Deg2Rad * 120), 0, offset + radius * Mathf.Sin(Mathf.Deg2Rad * 120)));
            positions.Add(new Vector3(offset + radius * Mathf.Cos(Mathf.Deg2Rad * 180), 0, offset + radius * Mathf.Sin(Mathf.Deg2Rad * 180)));
            positions.Add(new Vector3(offset + radius * Mathf.Cos(Mathf.Deg2Rad * 240), 0, offset + radius * Mathf.Sin(Mathf.Deg2Rad * 240)));
            positions.Add(new Vector3(offset + radius * Mathf.Cos(Mathf.Deg2Rad * 300), 0, offset + radius * Mathf.Sin(Mathf.Deg2Rad * 300)));

            int sharks_count = _scaleRandomizer.Next(100);
            if (sharks_count < 5)
            {
                sharks_count = 0;
            }
            else if (sharks_count < 10)
            {
                sharks_count = 1;
            }
            else if (sharks_count < 20)
            {
                sharks_count = 2;
            }
            else if (sharks_count < 30)
            {
                sharks_count = 3;
            }
            else if (sharks_count < 80)
            {
                sharks_count = 4;
            }
            else if (sharks_count < 90)
            {
                sharks_count = 5;
            }
            else
            {
                sharks_count = 6;
            }

            if (zone.IsStartingIsland)
                sharks_count = 3;

            //sharks_count = 6;

            Debug.Log("Stranded Deep Shark Mod : adding " + sharks_count + " shark spawners to island " + zone.name);

            for (int sharkindex = 0; sharkindex < sharks_count; sharkindex++)
            {
                int positionIndex = _scaleRandomizer.Next(0, positions.Count - 1);
                Vector3 position = positions[positionIndex];
                positions.RemoveAt(positionIndex);
                //Vector3 position = positions[sharkindex];
                Quaternion rotation = new Quaternion(0, 0, 0, 1);

                uint sharkType = (uint)_scaleRandomizer.Next(100);
                if (sharkType < 60)
                    // 218 TIGER
                    sharkType = 218;
                else if (sharkType < 98)
                    // 334 HAMMER
                    sharkType = 334;
                else
                    // 335 GOBLIN
                    sharkType = 335;

                //UnityEngine.Debug.Log("StrandedWorld::CreateWorld:: Stranded Wide World shark type = " + sharkType);

                SaveablePrefab sp = null;

                string text;
                bool flag = Prefabs.TryGetMultiplayerPrefabName(sharkType, out text);
                //UnityEngine.Debug.Log("StrandedWorld::CreateWorld:: Stranded Wide World flag = " + flag);
                MiniGuid referenceId = MiniGuid.NewFrom(position, sharkType, 48879);
                //UnityEngine.Debug.Log("StrandedWorld::CreateWorld:: Stranded Wide World CreateGeneratedPrefab referenceId = " + referenceId);
                SaveablePrefab instance = MultiplayerMng.Instantiate<SaveablePrefab>(sharkType, referenceId, null);

                ((PiscusManager)instance).SpawnDistance = 60;
                fi_territoryRadius.SetValue((PiscusManager)instance, 160);

                LevelLoader.Instance.HashCodes.Add(instance.gameObject.GetHashCode());
                instance.transform.SetParent(zone.SaveContainer, false);
                instance.transform.position = position + zone.transform.position;
                instance.gameObject.transform.SetParent(zone.SaveContainer, true);

                Debug.Log("Stranded Deep Shark Mod : adding " + sharkType + " spawner to island " + zone.name + " at " + position);
                zone.OnObjectCreated(gameObject);
            }

            _handledZones.Add(zone);
        }

        private static void ForceSpawn(PiscusManager instance)
        {
            // set high value to timer
            fi_respawningTimer.SetValue(instance, 0);
            fi_canRespawn.SetValue(instance, true);
            // PollSpawn();
            mi_PollSpawn.Invoke(instance, new object[] { });

            mi_Activate_Creature.Invoke(instance, new object[] { true });
        }

        private static void Reset()
        {
            _scaleRandomizer.Reinitialise(StrandedWorld.WORLD_SEED);
            //r.Reinitialise(StrandedWorld.WORLD_SEED);
            _handledZones.Clear();
        }

        internal static float GetScaleBySize(SharkSize size)
        {
            switch (size)
            {
                case SharkSize.Baby:
                    {
                        return 0.5f;
                    }
                case SharkSize.Small:
                    {
                        return 0.7f;
                    }
                case SharkSize.Medium:
                    {
                        return 0.85f;
                    }
                case SharkSize.Normal:
                    {
                        return 1.0f;
                    }
                case SharkSize.Big:
                    {
                        return 1.1f;
                    }
                case SharkSize.Huge:
                    {
                        return 1.2f;
                    }
                default:
                    {
                        break;
                    }
            }
            return 1.0f;
        }

        internal static void SetAggressivityBySize(SharkSize size, Piscus_Creature piscus)
        {
            if (piscus == null)
                return;

            //if (fi_attackDamage == null)
            //{
            //    fi_attackDamage = piscus.GetType().GetField("_attackDamage ", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //    Debug.Log("Stranded Deep Shark Mod : fi_attackDamage " + fi_attackDamage);
            //}

            float multiplier = 1.0f;
            if (PlayerUtilities.GetServerGameDifficulty() == EGameDifficultyMode.Hard)
            {
                multiplier = 1.2f;
            }

            switch (size)
            {
                case SharkSize.Baby:
                case SharkSize.Small:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, false);
                        fi_canAttackNudge.SetValue(piscus, false);
                        fi_canAttackJump.SetValue(piscus, false);
                        fi_attackDamage.SetValue(piscus, 0.0f);
                        fi_attackCritDamage.SetValue(piscus, 0.0f);
                        break;
                    }
                case SharkSize.Medium:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, false);
                        fi_canAttackNudge.SetValue(piscus, false);
                        fi_canAttackJump.SetValue(piscus, false);
                        fi_attackDamage.SetValue(piscus, 80.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 100.0f * multiplier);
                        break;
                    }
                case SharkSize.Normal:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, false);
                        fi_canAttackNudge.SetValue(piscus, true);
                        fi_canAttackJump.SetValue(piscus, false);
                        fi_attackDamage.SetValue(piscus, 100.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 200.0f * multiplier);
                        break;
                    }
                case SharkSize.Big:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, true);
                        fi_canAttackNudge.SetValue(piscus, true);
                        fi_canAttackJump.SetValue(piscus, false);
                        fi_attackDamage.SetValue(piscus, 150.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 300.0f * multiplier);
                        break;
                    }
                case SharkSize.Huge:
                    {
                        fi_canAttack.SetValue(piscus, true);
                        fi_canAttackRafts.SetValue(piscus, true);
                        fi_canAttackNudge.SetValue(piscus, true);
                        fi_canAttackJump.SetValue(piscus, true);
                        fi_attackDamage.SetValue(piscus, 200.0f * multiplier);
                        fi_attackCritDamage.SetValue(piscus, 400.0f * multiplier);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
