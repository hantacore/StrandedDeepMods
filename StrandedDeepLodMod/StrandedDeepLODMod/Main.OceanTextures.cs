using Beam;
using Beam.Rendering;
using Beam.Serialization;
using Ceto;
using HarmonyLib;
using SharpNeatLib.Maths;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepLODMod
{
    static partial class Main
    {
        [HarmonyPatch(typeof(StrandedWorld), "AddWaveOverlay")]
        class StrandedWorld_AddWaveOverlay_Patch
        {
            static void Postfix(StrandedWorld __instance, GameObject zone, Texture2D height, Texture2D shore, ref AddWaveOverlay __result)
            {
                try
                {
                    if (!betterFoam)
                        return;

                    AddWaveOverlay addWaveOverlay = __result;
                    float size = WorldUtilities.ZoneTerrainSize + 0;
                    addWaveOverlay.width = size;
                    addWaveOverlay.height = size;

                    addWaveOverlay.heightTexture.tex = shore;
                    addWaveOverlay.heightTexture.mask = height;

                    addWaveOverlay.normalTexture.tex = shore;
                    addWaveOverlay.normalTexture.mask = height;

                    //Main.ExportTexture(shore, @"AddWaveOverlay shore");
                    Texture2D resizedShore = EnlargeTexture(shore, shore.width, shore.height, 1.1f);
                    //Main.ExportTexture(resizedShore, @"AddWaveOverlay resizedShore");
                    Texture2D resizedHeight = EnlargeTexture(height, height.width, height.height, 0.5f);
                    //Main.ExportTexture(resizedHeight, @"AddWaveOverlay resizedHeight");
                    Texture2D foamMask = MergeShorelines(resizedShore, resizedHeight, _indexedTextures["StrandedDeepLODMod.assets.Textures.random_mask.png"]);
                    //Main.ExportTexture(foamMask, @"AddWaveOverlay foamMask");
                    addWaveOverlay.foamTexture.tex = foamMask;
                    addWaveOverlay.foamTexture.mask = height;

                    addWaveOverlay.clipTexture.tex = shore;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching StrandedWorld.AddWaveOverlay : " + e);
                }
            }
        }

        static bool foamTexturesUpdated = false;

        [HarmonyPatch(typeof(Ocean), "Update")]
        class Ocean_Update_Patch
        {
            static void Postfix(Ocean __instance)
            {
                try
                {
                    if (!betterFoam)
                        return;

                    if (!foamTexturesUpdated)
                    {
                        __instance.foamBlendMode = OVERLAY_BLEND_MODE.MAX;
                        __instance.foamOverlaySize = OVERLAY_MAP_SIZE.DOUBLE;

                        __instance.foamTexture0.tex = _indexedTextures["StrandedDeepLODMod.assets.Textures.foam.png"];
                        __instance.foamTexture0.scale = new Vector2(4f, 4f);
                        __instance.foamTexture1.tex = _indexedTextures["StrandedDeepLODMod.assets.Textures.foam2.png"];
                        __instance.foamTexture0.scale = new Vector2(3f, 3f);
                        foamTexturesUpdated = true;
                    }

                    Vector4 value2 = default(Vector4);
                    value2.x = ((__instance.foamTexture0.scale.x != 0f) ? (1f / __instance.foamTexture0.scale.x) : 1f);// + 0.02f * Mathf.Cos(__instance.OceanTime.Now/10);
                    value2.y = ((__instance.foamTexture0.scale.y != 0f) ? (1f / __instance.foamTexture0.scale.y) : 1f);// + 0.03f * Mathf.Sin(__instance.OceanTime.Now/8);
                    value2.z = __instance.foamTexture0.scrollSpeed * Mathf.Sin(__instance.OceanTime.Now) * 0.5f;
                    value2.w = 0f;
                    Vector4 value3 = default(Vector4);
                    value3.x = ((__instance.foamTexture1.scale.x != 0f) ? (1f / __instance.foamTexture1.scale.x) : 1f);
                    value3.y = ((__instance.foamTexture1.scale.y != 0f) ? (1f / __instance.foamTexture1.scale.y) : 1f);
                    value3.z = __instance.foamTexture1.scrollSpeed * Mathf.Sin(__instance.OceanTime.Now) * 0.5f;
                    value3.w = 0f;
                    Shader.SetGlobalTexture("Ceto_FoamTexture0", (__instance.foamTexture0.tex != null) ? __instance.foamTexture0.tex : Texture2D.whiteTexture);
                    Shader.SetGlobalVector("Ceto_FoamTextureScale0", value2);
                    Shader.SetGlobalTexture("Ceto_FoamTexture1", (__instance.foamTexture1.tex != null) ? __instance.foamTexture1.tex : Texture2D.whiteTexture);
                    Shader.SetGlobalVector("Ceto_FoamTextureScale1", value3);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep LOD mod : error while patching StrandedWorld.AddWaveOverlay : " + e);
                }
            }
        }
    }
}
