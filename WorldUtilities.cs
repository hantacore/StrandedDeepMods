using Beam;
using Beam.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepModsUtils
{
    public static class WorldUtilities
    {
        private static bool worldLoaded = false;
        private static StrandedWorld previousInstance = null;

        public static bool IsStrandedWide()
        {
            UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
            return (mewide != null && mewide.Active && mewide.Loaded);
        }

        public static float ZoneSize
        {
            get
            {
                UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
                if (mewide != null && mewide.Active && mewide.Loaded)
                {
                    Type strandedWideMainType = mewide.Assembly.GetType("StrandedWideMod_Harmony.Main");
                    if (strandedWideMainType != null)
                    {
                        PropertyInfo pi_zoneSize = strandedWideMainType.GetProperty("ZoneSize", BindingFlags.Static);
                        if (pi_zoneSize != null)
                        {
                            float swideZoneSize = (float)pi_zoneSize.GetValue(null);
                            Debug.Log("Stranded Deep World Utilities : Stranded Wide zone size retrieved : " + swideZoneSize);
                            return swideZoneSize;
                        }
                        else
                        {
                            Debug.Log("Stranded Deep World Utilities : Stranded Wide pi_zoneSize null");
                        }
                    }
                    else
                    {
                        Debug.Log("Stranded Deep World Utilities : Stranded Wide type null");
                    }
                }

                return StrandedWorld.ZONE_SIZE;
            }
        }

        public static float ZoneSpacing
        {
            get
            {
                UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
                if (mewide != null && mewide.Active && mewide.Loaded)
                {
                    Type strandedWideMainType = mewide.Assembly.GetType("StrandedWideMod_Harmony.Main");
                    if (strandedWideMainType != null)
                    {
                        PropertyInfo pi_zoneSpacing = strandedWideMainType.GetProperty("ZoneSpacing", BindingFlags.Static);
                        if (pi_zoneSpacing != null)
                        {
                            float swideZoneSpacing = (float)pi_zoneSpacing.GetValue(null);
                            Debug.Log("Stranded Deep World Utilities : Stranded Wide zone spacing retrieved : " + swideZoneSpacing);
                            return swideZoneSpacing;
                        }
                        else
                        {
                            Debug.Log("Stranded Deep World Utilities : Stranded Wide pi_zoneSpacing null");
                        }
                    }
                    else
                    {
                        Debug.Log("Stranded Deep World Utilities : Stranded Wide type null");
                    }
                }

                return StrandedWorld.ZONE_SPACING;
            }
        }


        public static int IslandCount
        {
            get
            {
                UnityModManager.ModEntry mewide = UnityModManager.FindMod("StrandedWideMod");
                if (mewide != null && mewide.Active && mewide.Loaded)
                {
                    Type strandedWideMainType = mewide.Assembly.GetType("StrandedWideMod_Harmony.Main");
                    if (strandedWideMainType != null)
                    {
                        FieldInfo pi_zoneSpacing = strandedWideMainType.GetField("IslandCount", BindingFlags.Static);
                        if (pi_zoneSpacing != null)
                        {
                            int swideIslandCount = (int)pi_zoneSpacing.GetValue(null);
                            Debug.Log("Stranded Deep World Utilities : Stranded Wide island count retrieved : " + swideIslandCount);
                            return swideIslandCount;
                        }
                        else
                        {
                            Debug.Log("Stranded Deep World Utilities : Stranded Wide pi_zoneSpacing null");
                        }
                    }
                    else
                    {
                        Debug.Log("Stranded Deep World Utilities : Stranded Wide type null");
                    }
                }

                return StrandedWorld.WORLD_ZONES_SQUARED;
            }
        }
        public static bool IsWorldLoaded()
        {
            if (Beam.Game.State == GameState.NEW_GAME
                || Beam.Game.State == GameState.LOAD_GAME)
            {
                // anti memory leak
                if (previousInstance != null
                && !System.Object.ReferenceEquals(previousInstance, StrandedWorld.Instance))
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : world instance changed, clearing events");
                    previousInstance.WorldGenerated -= Instance_WorldGenerated;
                    previousInstance = null;
                    worldLoaded = false;
                }

                if (StrandedWorld.Instance != null
                    && !System.Object.ReferenceEquals(StrandedWorld.Instance, previousInstance))
                {
                    Debug.Log("Stranded Deep AlternativeEndgame Mod : world instance found, registering events");
                    previousInstance = StrandedWorld.Instance;
                    StrandedWorld.Instance.WorldGenerated -= Instance_WorldGenerated;
                    StrandedWorld.Instance.WorldGenerated += Instance_WorldGenerated;
                    worldLoaded = false;
                }
            }
            else
            {
                Reset();
            }

            return worldLoaded;
        }

        private static void Reset()
        {
            worldLoaded = false;
        }

        private static void Instance_WorldGenerated()
        {
            Debug.Log("Stranded Deep AlternativeEndgame Mod : World Loaded event");
            worldLoaded = true;
        }
    }
}