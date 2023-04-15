using Beam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepWetAndColdMod
{
    public static partial class Main
    {
        private static bool IsInTheSun(IPlayer p)
        {
            try
            {
                Sunburn s = p.Statistics.GetPlayerEffect<Sunburn>();
                if (s != null)
                    return !s.Protected && !IsStorm();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Wet and Cold Mod : error IsInTheSun : " + e);
            }
            return !IsStorm();
        }

        private static bool IsNight()
        {
            if (Singleton<Atmosphere>.Instance != null)
            {
                return !Singleton<Atmosphere>.Instance.DayTime;
            }
            return false;
        }

        private static bool IsNearLitFire(Player p)
        {
            // get all firecamp and fire objects
            // if lit
            // compute distance
            SaveablePrefab[] saveables = Beam.Game.FindObjectsOfType<SaveablePrefab>();
            if (saveables != null
                && saveables.Length > 0)
            {
                foreach (SaveablePrefab sp in saveables)
                {
                    if (firePrefabs.Contains(sp.PrefabId))
                    {
                        if (Vector3.Distance(p.transform.position, sp.transform.position) < ParameterValues.DISTANCE_TO_FIRE_HEAT)
                        {
                            Beam.Crafting.Construction_CAMPFIRE fire = sp as Beam.Crafting.Construction_CAMPFIRE;
                            if (fire != null)
                            {
                                return fire.IsBurning;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static void IsSheltered(Player p, out bool simpleShelter, out bool housingShelter)
        {
            simpleShelter = false;
            housingShelter = false;

            // MOST IMPORTANT SECTION : which object are we looking at
            if (Beam.Game.State == GameState.NEW_GAME || Beam.Game.State == GameState.LOAD_GAME)
            {
                // check if something above
                RaycastHit hit;
                Vector3 origin = p.transform.position + new Vector3(0, 1, 0);
                if (Physics.Raycast(origin, new Vector3(0, 1, 0), out hit, ParameterValues.TOP_SHELTER_CHECK_DISTANCE))
                {
                    simpleShelter = true;

                    //if (hit.transform != null)
                    //    Debug.Log("Stranded Deep Wet and Cold Mod : I'm under " + hit.transform.name);
                    GameObject go = hit.transform.gameObject;
                    BaseObject sp = null;
                    if (go != null)
                    {
                        if (go != null)
                        {
                            sp = go.GetComponent<BaseObject>();
                            if (sp == null || !roofPrefabs.Contains(sp.PrefabId))
                            {
                                return;
                            }
                        }
                    }

                    // check for more complex shelter (raycast in 4 directions)
                    if (Physics.Raycast(p.transform.position, new Vector3(0, -1, 0), out hit, ParameterValues.SHELTER_CHECK_DISTANCE))
                    {
                        // is there a foundation
                        if (hit.transform != null && hit.transform.gameObject != null)
                        {
                            //Debug.Log("Stranded Deep Wet and Cold Mod : I walk on " + hit.transform.name);
                            go = hit.transform.gameObject;
                            sp = null;
                            if (go != null)
                            {
                                sp = go.GetComponent<BaseObject>();
                                if (sp != null && floorPrefabs.Contains(sp.PrefabId))
                                {
                                    if (Physics.Raycast(p.transform.position, new Vector3(1, 0, 0), out hit, ParameterValues.SHELTER_CHECK_DISTANCE))
                                    {
                                        if (hit.transform != null && hit.transform.gameObject != null)
                                        {
                                            go = hit.transform.gameObject;
                                            sp = go.GetComponent<BaseObject>();
                                            if (sp == null || !wallPrefabs.Contains(sp.PrefabId))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                    if (Physics.Raycast(p.transform.position, new Vector3(-1, 0, 0), out hit, ParameterValues.SHELTER_CHECK_DISTANCE))
                                    {
                                        if (hit.transform != null && hit.transform.gameObject != null)
                                        {
                                            go = hit.transform.gameObject;
                                            sp = go.GetComponent<BaseObject>();
                                            if (sp == null || !wallPrefabs.Contains(sp.PrefabId))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                    if (Physics.Raycast(p.transform.position, new Vector3(0, 0, 1), out hit, ParameterValues.SHELTER_CHECK_DISTANCE))
                                    {
                                        if (hit.transform != null && hit.transform.gameObject != null)
                                        {
                                            go = hit.transform.gameObject;
                                            sp = go.GetComponent<BaseObject>();
                                            if (sp == null || !wallPrefabs.Contains(sp.PrefabId))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                    if (Physics.Raycast(p.transform.position, new Vector3(0, 0, -1), out hit, ParameterValues.SHELTER_CHECK_DISTANCE))
                                    {
                                        if (hit.transform != null && hit.transform.gameObject != null)
                                        {
                                            go = hit.transform.gameObject;
                                            sp = go.GetComponent<BaseObject>();
                                            if (sp == null || !wallPrefabs.Contains(sp.PrefabId))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                                housingSheltered = true;
                            }
                        }
                    }
                }
            }
        }

        private static bool IsStorm()
        {
            WeatherEvent currentWeatherEvent = Singleton<AtmosphereStorm>.Instance.CurrentWeatherEvent;
            return (currentWeatherEvent != null && Singleton<AtmosphereStorm>.Instance.CurrentWeatherEvent.IsActive); //&& currentWeatherEvent.Humidity > (float)90
        }

        private static bool HasPhysicalActivity(IPlayer p)
        {
            try
            {
                if (p.Movement == null)
                    return false;
                if (p.Movement.ClimbingLadder)
                    return true;
                if (p.Movement.sprinting)
                    return true;
                //if (p.Movement.IsBusy)
                //    return true;

                // check if chopping wood and shit
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Wet and Cold Mod : HasPhysicalActivity failed : " + e);
            }

            return false;
        }
    }
}
