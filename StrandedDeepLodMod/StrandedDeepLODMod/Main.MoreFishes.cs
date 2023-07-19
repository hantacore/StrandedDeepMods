using Beam;
using Beam.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        private static List<object> multipliedFishes = new List<object>();

        static Dictionary<string, int> fishGroups = new Dictionary<string, int>();

        static string SPAWNER_FISH = "Spawner - Fish";
        static string SPAWNER_MIDFISH = "Spawner - Midfish";

        static string NEEDLE_FISH_GROUP = "Needle Fish Group"; //aiguille, poisson de côte
        static string CLOWN_FISH_GROUP = "Clown Fish Group"; // clown
        static string TANG_FISH_GROUP = "Tang Fish Group"; // chirurgien
        static string BUTTERFLY_FISH_GROUP = "Butterfly Fish Group"; // papillon
        static string REEF_SHARK_GROUP = "Reef Shark Group";
        static string DISCUS_FISH_GROUP = "Discus Fish Group";
        static string COD_FISH_GROUP = "Cod Fish Group";
        static string ARCHER_FISH_GROUP = "Archer Fish Group";
        static string SARDINE_FISH_GROUP = "Sardine Fish Group";

        //static FishSpawner[] FishSpawnersToHandle = null;
        //static int lastPassFishSpawnersCount = 0;
        //static int currentFishSpawnerIndex = -1;

        private static void InitMultiplyFishes()
        {
            // group name, max wanted instances
            fishGroups.Add(NEEDLE_FISH_GROUP, 5);
            fishGroups.Add(CLOWN_FISH_GROUP, 5);
            fishGroups.Add(BUTTERFLY_FISH_GROUP, 5);
            fishGroups.Add(TANG_FISH_GROUP, 5);
            fishGroups.Add(DISCUS_FISH_GROUP, 1);
            fishGroups.Add(REEF_SHARK_GROUP, 5);
            fishGroups.Add(COD_FISH_GROUP, 1);
            fishGroups.Add(ARCHER_FISH_GROUP, 1);
            fishGroups.Add(SARDINE_FISH_GROUP, 1);
        }

        static bool multiplyFishesDone = false;

        private static void MultiplyFishesNew()
        {
            PlayerDetailSpawnerManager pdsm = UnityEngine.Object.FindObjectOfType<PlayerDetailSpawnerManager>();
            if (pdsm == null)
                return;
            FishSpawner fs = pdsm.Fish_Spawner as FishSpawner;
            if (fs == null || fs.CellObjectsSettings == null || fs.CellObjectsSettings.Count == 0)
                return;

            for (int i = 0; i < fs.CellObjectsSettings.Count; i++)
            {
                if (fishGroups.Keys.Contains(fs.CellObjectsSettings[i].Prefab.name))
                {
                    Debug.Log("Stranded Deep LOD Mod : multiplying fishes handling fish group prefab : " + fs.CellObjectsSettings[i].Prefab.name);


                    Debug.Log("Stranded Deep LOD Mod : multiplying fishes MaximumInstances : " + fs.CellObjectsSettings[i].MaximumInstances);
                    if (fs.CellObjectsSettings[i].MaximumInstances > 25)
                        continue;
                    typeof(CellObjectSettings<FishBase>).GetField("_maximumInstances", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(fs.CellObjectsSettings[i], fs.CellObjectsSettings[i].MaximumInstances * fishGroups[fs.CellObjectsSettings[i].Prefab.name]);
                    if (ultraUnderwaterDetail)
                    {
                        typeof(CellObjectSettings<FishBase>).GetField("_minimumWorldHeight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(fs.CellObjectsSettings[i], -50f);
                    }
                    Debug.Log("Stranded Deep LOD Mod : multiplying fishes new MaximumInstances : " + fs.CellObjectsSettings[i].MaximumInstances);
                }
                else
                {
                    //Debug.Log("Stranded Deep LOD Mod : multiplying fishes unhandled fish group prefab : " + fs.CellObjectsSettings[i].Prefab.name);
                }
            }

            multiplyFishesDone = true;
        }

        //private static void FillFishSpawnersQueue()
        //{
        //    try
        //    {
        //        if (currentFishSpawnerIndex > 0
        //            || FishSpawnersToHandle != null)
        //            return;

        //        FishSpawner[] fss = Beam.Game.FindObjectsOfType<FishSpawner>();

        //        if (fss.Length == 0
        //            || lastPassFishSpawnersCount == fss.Length)
        //        {
        //            //Debug.Log("Stranded Deep LOD Mod : no fish spawner found : " + fss.Length + " / " + lastPassFishSpawnersCount);
        //            return;
        //        }

        //        Debug.Log("Stranded Deep LOD Mod : fish spawners found : " + fss.Length);
        //        lastPassFishSpawnersCount = fss.Length;

        //        FishSpawnersToHandle = new FishSpawner[lastPassFishSpawnersCount];
        //        Array.Copy(fss, FishSpawnersToHandle, lastPassFishSpawnersCount);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep LOD Mod : FillFishSpawnersQueue failed : " + e);
        //    }
        //}

        //private static void MultiplyFishes()
        //{
        //    if (FishSpawnersToHandle == null
        //        || FishSpawnersToHandle.Length == 0)
        //    {
        //        // game reset issue
        //        if (FishSpawnersToHandle != null && FishSpawnersToHandle.Length == 0)
        //            FishSpawnersToHandle = null;
        //        return;
        //    }

        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    try
        //    {
        //        if (currentFishSpawnerIndex < 0)
        //            currentFishSpawnerIndex = FishSpawnersToHandle.Length - 1;

        //        while (currentFishSpawnerIndex >= 0
        //            && sw.ElapsedMilliseconds <= 2)
        //        {
        //            FishSpawner fs = FishSpawnersToHandle[currentFishSpawnerIndex];
        //            try
        //            {
        //                if (multipliedFishes.Contains(fs))
        //                    continue;
        //                //Debug.Log("Stranded Deep LOD Mod : multiplying fishes first pass : " + fs.gameObject.name);
        //                for (int i = 0; i < fs.CellObjectsSettings.Count; i++)
        //                {
        //                    if (fishGroups.Keys.Contains(fs.CellObjectsSettings[i].Prefab.name))
        //                    {
        //                        Debug.Log("Stranded Deep LOD Mod : multiplying fishes handling fish group prefab : " + fs.CellObjectsSettings[i].Prefab.name);


        //                        Debug.Log("Stranded Deep LOD Mod : multiplying fishes MaximumInstances : " + fs.CellObjectsSettings[i].MaximumInstances);
        //                        if (fs.CellObjectsSettings[i].MaximumInstances > 25)
        //                            continue;
        //                        typeof(CellObjectSettings<FishBase>).GetField("_maximumInstances", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(fs.CellObjectsSettings[i], fs.CellObjectsSettings[i].MaximumInstances * fishGroups[fs.CellObjectsSettings[i].Prefab.name]);
        //                        Debug.Log("Stranded Deep LOD Mod : multiplying fishes new MaximumInstances : " + fs.CellObjectsSettings[i].MaximumInstances);
        //                    }
        //                    else
        //                    {
        //                        //Debug.Log("Stranded Deep LOD Mod : multiplying fishes unhandled fish group prefab : " + fs.CellObjectsSettings[i].Prefab.name);
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.Log("Stranded Deep LOD Mod : multiplying fishes failed for " + fs.gameObject.name + " : " + e);
        //            }
        //            finally
        //            {
        //                multipliedFishes.Add(fs);
        //                currentFishSpawnerIndex--;
        //            }
        //        }

        //        if (currentFishSpawnerIndex < 0)
        //            FishSpawnersToHandle = null;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep LOD Mod : multiplying fishes failed " + e);
        //    }
        //}
    }
}
