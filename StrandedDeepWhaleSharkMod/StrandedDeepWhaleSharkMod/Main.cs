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

namespace StrandedDeepWhaleSharkMod
{
    static class Main
    {
        private static SharkHandler handler = new SharkHandler();

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        internal static bool perfCheck = true;

        private static StrandedWorld previousWorldInstance = null;
        private static bool worldLoaded = false;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;

            handler.ModName = "Whale Shark Mod";

            Debug.Log("Stranded Deep Shark Mod properly loaded");

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Realistic Whale Shark mod by Hantacore");
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
                        if (!WorldUtilities.IsStrandedWide())
                        {
                            ReplaceSharks(StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false));
                        }

                        if (handler.MustFillQueue)
                        {
                            handler.FillQueue(Game.FindObjectsOfType<Piscus_Creature>());
                        }
                        handler.Handle();
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
            if (zone.IsUserMap || _handledZones.Contains(zone))
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
            FastRandom _scaleRandomizer = new FastRandom(zone.Seed);

            int radius = 80;//120;// 170;
            int offset = 0;// 110;// (_islandSize / 2);
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

                ((PiscusManager)instance).SpawnDistance = 160;//320;

                LevelLoader.Instance.HashCodes.Add(instance.gameObject.GetHashCode());
                instance.transform.SetParent(zone.SaveContainer, false);
                instance.transform.position = position + zone.transform.position;
                instance.gameObject.transform.SetParent(zone.SaveContainer, true);

                Debug.Log("Stranded Deep Shark Mod : adding " + sharkType + " spawner to island " + zone.name + " at " + position);
                zone.OnObjectCreated(gameObject);
            }

            _handledZones.Add(zone);
        }

        private static void Reset()
        {
            _handledZones.Clear();
            handler.Reset();
        }

        //private static void Instance_WorldGenerated()
        //{
        //    Debug.Log("Stranded Deep Shark Mod : World Loaded event");
        //    worldLoaded = true;
        //}
    }
}
