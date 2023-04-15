using Beam;
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
        //static List<FishRendererBase> _handledFishRenderers = new List<FishRendererBase>();
        static FieldInfo fi_FishRendererBase_cullingDistance = typeof(FishRendererBase).GetField("_cullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);

        //static FishRendererBase[] FishRendererBaseToHandle = null;
        //static int lastPassFishRendererBaseCount = 0;
        //static int currentFishRendererBaseIndex = -1;

        //private static void InitIncreaseFishRendererDistance()
        //{

        //}

        //private static void FillFishRenderersQueue()
        //{
        //    try
        //    {
        //        if (currentFishRendererBaseIndex > 0
        //            || FishRendererBaseToHandle != null)
        //            return;

        //        FishRendererBase[] frbs = Beam.Game.FindObjectsOfType<FishRendererBase>();

        //        if (frbs.Length == 0
        //            || lastPassFishRendererBaseCount == frbs.Length)
        //        {
        //            //Debug.Log("Stranded Deep LOD Mod : no fish renderer found : " + frbs.Length + " / " + lastPassFishRendererBaseCount);
        //            return;
        //        }

        //        lastPassFishRendererBaseCount = frbs.Length;

        //        FishRendererBaseToHandle = new FishRendererBase[lastPassFishRendererBaseCount];
        //        Array.Copy(frbs, FishRendererBaseToHandle, lastPassFishRendererBaseCount);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep LOD Mod : FillFishRenderersQueue failed : " + e);
        //    }
        //}

        //private static void IncreaseFishDrawingDistance()
        //{
        //    try
        //    {
        //        // PiscusManager / _spawnDistance


        //        ////Debug.Log("Stranded Deep LOD Mod : increasing fish drawing distance");
        //        //// float
        //        //FishRendererBase[] frbs = Beam.Game.FindObjectsOfType<FishRendererBase>();
        //        //foreach (FishRendererBase frb in frbs)
        //        //{
        //        //    if (_handledFishRenderers.Contains(frb))
        //        //        continue;

        //        //    //Debug.Log("Stranded Deep LOD Mod : increasing fish drawing distance to 1000 for " + frb.GetType().Name);
        //        //    fi.SetValue(frb, 1000f);
        //        //    _handledFishRenderers.Add(frb);
        //        //}

        //        if (FishRendererBaseToHandle == null
        //            || FishRendererBaseToHandle.Length == 0)
        //        {
        //            // game reset issue
        //            if (FishRendererBaseToHandle != null && FishRendererBaseToHandle.Length == 0)
        //                FishRendererBaseToHandle = null;
        //            return;
        //        }

        //        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //        sw.Start();
        //        try
        //        {
        //            if (currentFishRendererBaseIndex < 0)
        //                currentFishRendererBaseIndex = FishRendererBaseToHandle.Length - 1;

        //            while (currentFishRendererBaseIndex >= 0
        //                && sw.ElapsedMilliseconds <= 2)
        //            {
        //                FishRendererBase frb = FishRendererBaseToHandle[currentFishRendererBaseIndex];
        //                try
        //                {
        //                    if (_handledFishRenderers.Contains(frb))
        //                        continue;

        //                    fi_FishRendererBase_cullingDistance.SetValue(frb, 1000f);
        //                    //Debug.Log("Stranded Deep LOD Mod : increasing fish drawing distance to 1000 for " + frb.GetType().Name);
        //                }
        //                catch (Exception e)
        //                {
        //                    Debug.Log("Stranded Deep LOD Mod : IncreaseFishDrawingDistance failed for " + frb.gameObject.name + " : " + e);
        //                }
        //                finally
        //                {
        //                    _handledFishRenderers.Add(frb);
        //                    currentFishRendererBaseIndex--;
        //                }
        //            }

        //            if (currentFishRendererBaseIndex < 0)
        //                FishRendererBaseToHandle = null;
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("Stranded Deep LOD Mod : IncreaseFishDrawingDistance failed (1) " + e);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep LOD Mod : IncreaseFishDrawingDistance failed (2) " + e);
        //    }
        //}
    }
}
