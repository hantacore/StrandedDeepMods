using Beam;
using Beam.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Beam.Rendering;
using UnityEngine;

namespace StrandedWideMod_Harmony
{
    public static partial class Main
    {
        [HarmonyPatch(typeof(Lod), "SetActive")]
        class Lod_SetActive_Patch
        {
            static bool Prefix(Lod __instance, bool active)
            {
                try
                {
                    if (__instance.Renderers == null)
                        return false;
                    foreach(Renderer r in __instance.Renderers)
                    {
                        if (r == null)
                            return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Wide (Harmony edition) : error while patching Lod_SetActive_Patch : " + e);
                }
                return true;
            }
        }
    }
}
