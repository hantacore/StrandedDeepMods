using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        private static List<GameObject> patchedParticleElements = new List<GameObject>();

        static GameObject[] GameObjectsToHandle = null;
        static int lastPassGameObjectsCount = 0;
        static int currentGameObjectIndex = -1;

        private static void FillParticleSpawnersQueue()
        {
            if (!addJellyFishes && !addShrimps && !addSmallFishes)
                return;

            try
            {
                if (currentGameObjectIndex > 0
                    || GameObjectsToHandle != null)
                    return;

                GameObject[] fss = Beam.Game.FindObjectsOfType<GameObject>();

                if (fss.Length == 0
                    || lastPassGameObjectsCount == fss.Length)
                {
                    //Debug.Log("Stranded Deep LOD Mod : no follow spawn found : " + fss.Length + " / " + lastPassFollowSpawnsCount);
                    return;
                }

                lastPassGameObjectsCount = fss.Length;

                GameObjectsToHandle = new GameObject[lastPassGameObjectsCount];
                Array.Copy(fss, GameObjectsToHandle, lastPassGameObjectsCount);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : FillParticleSpawnersQueue failed : " + e);
            }
        }

        private static void AddParticleSpawners()
        {
            if (!addJellyFishes && !addShrimps && !addSmallFishes)
                return;

            if (GameObjectsToHandle == null
                   || GameObjectsToHandle.Length == 0)
            {
                // game reset issue
                if (GameObjectsToHandle != null && GameObjectsToHandle.Length == 0)
                    GameObjectsToHandle = null;
                return;
            }

            FastRandom fr = new FastRandom(StrandedWorld.WORLD_SEED);

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                if (currentGameObjectIndex < 0)
                    currentGameObjectIndex = GameObjectsToHandle.Length - 1;

                while (currentGameObjectIndex >= 0
                    && sw.ElapsedMilliseconds <= 2)
                {
                    GameObject go = GameObjectsToHandle[currentGameObjectIndex];
                    try
                    {
                        try
                        {
                            // Bypass weird error
                            // System.NullReferenceException
                            // at(wrapper managed - to - native) UnityEngine.Object.GetName(UnityEngine.Object)
                            String.IsNullOrEmpty(go.name);
                        }
                        catch(Exception e)
                        {
                            continue;
                        }

                        if (String.IsNullOrEmpty(go.name) || patchedParticleElements.Contains(go))
                            continue;

                        if (addJellyFishes)
                        {
                            if (go.name.Contains("Table_Coral") && fr.Next(0, 100) >= 50)
                            {
                                //Debug.Log("Stranded Deep LOD Mod : adding jellyfish particle system to table coral");
                                ParticleSystem ps = go.AddComponent<ParticleSystem>();
                                JellyfishParticleSystem pst = go.AddComponent<JellyfishParticleSystem>();
                                ps.Play();
                            }
                        }

                        if (addShrimps)
                        {
                            if (go.name.Contains("Coral_Rock"))
                            {
                                //Debug.Log("Stranded Deep LOD Mod : adding shrimp particle system to coral rock");
                                ParticleSystem ps = go.AddComponent<ParticleSystem>();
                                ShrimpParticleSystem pst = go.AddComponent<ShrimpParticleSystem>();
                                ps.Play();
                            }
                        }

                        if (addSmallFishes)
                        {
                            if (go.name.Contains("Coral_Pink")
                                || go.name.Contains("Coral_White")
                                || go.name.Contains("Staghorn"))
                            {
                                //Debug.Log("Stranded Deep LOD Mod : adding smallfishes particle system to coral");
                                ParticleSystem ps = go.AddComponent<ParticleSystem>();
                                SmallfishesParticleSystem pst = go.AddComponent<SmallfishesParticleSystem>();
                                ps.Play();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Stranded Deep LOD Mod : AddParticleSpawners failed for " + (go.name == null ? "null" : go.name) + " : " + e);
                    }
                    finally
                    {
                        patchedParticleElements.Add(go);
                        currentGameObjectIndex--;
                    }
                }

                if (currentGameObjectIndex < 0)
                    GameObjectsToHandle = null;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep LOD Mod : AddParticleSpawners fatally failed " + e);
            }
        }

        static void ResetParticles()
        {
            GameObjectsToHandle = null;
            lastPassGameObjectsCount = 0;
            currentGameObjectIndex = -1;

            if (patchedParticleElements != null)
                patchedParticleElements.Clear();
        }
    }
}
