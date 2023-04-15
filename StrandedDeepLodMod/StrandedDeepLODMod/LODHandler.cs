using Beam;
using Beam.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepLODMod
{
    public class LODHandler : CollectionHandler<LodController>
    {
        public LODHandler() : base(2, true)
        {

        }

        //FieldInfo fiCull = typeof(LodController).GetField("_localImpostorCullingDistance", BindingFlags.NonPublic | BindingFlags.Instance);
        //FieldInfo fiScope = typeof(LodController).GetField("_scope", BindingFlags.NonPublic | BindingFlags.Instance);
        //MethodInfo mi = typeof(LodController).GetMethod("CreateImpostor", BindingFlags.NonPublic | BindingFlags.Instance);

        protected override void HandleOne(LodController toHandle)
        {
            try
            {
                LodController lc = toHandle;

                //Debug.Log("Stranded Deep LOD Mod : lod info : " + lc.gameObject.name);
                Main.fi_Scope.SetValue(lc, ImposterScope.Manual);

                int dist = (int)Main.fi_Cull.GetValue(lc);
                Main.fi_Cull.SetValue(lc, 2000);
                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : new _localImpostorCullingDistance " + fiCull.GetValue(lc));

                int bestLOD = 3;
                for (int i = 0; i < lc.LodGroup.Lods.Count; i++)
                {
                    Lod lod = lc.LodGroup.Lods[i];
                    if (lod.Renderers[0].name.Contains("LOD0") && bestLOD > 0)
                    {
                        bestLOD = 0;
                    }
                    else if (lod.Renderers[0].name.Contains("LOD1") && bestLOD > 1)
                    {
                        bestLOD = 1;
                    }
                    else if (lod.Renderers[0].name.Contains("LOD2") && bestLOD > 2)
                    {
                        bestLOD = 2;
                    }
                }
                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : best LOD found : " + bestLOD);

                int farthest = int.MinValue;

                //for (int i = 0; i < lc.LodGroup.Lods.Count; i++)
                for (int i = lc.LodGroup.Lods.Count - 1; i >= 0; i--)
                {
                    Lod lod = lc.LodGroup.Lods[i];
                    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name);
                    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / cull distance " + lod.CullingDistance);
                    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / IsImpostor " + lod.IsImpostor);

                    if (!lod.IsImpostor)
                    {
                        lod.CullingDistance = lod.CullingDistance * (Main.ultraMFBBQDistance ? 1000 : (Main.ultraDistance ? 10 : 5));
                        //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " new cull distance " + lod.CullingDistance);
                        if (lod.CullingDistance > farthest)
                        {
                            farthest = lod.CullingDistance + 1;
                        }
                    }

                    //if (!lod.Renderers[0].name.Contains("LOD" + bestLOD) && !lod.IsImpostor)
                    //{
                    //    // remove
                    //    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / cull distance " + lod.CullingDistance);
                    //    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " / removing LOD at " + i);
                    //    //lc.LodGroup.Lods.RemoveAt(i);
                    //}
                    //else
                    //{
                    //    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " old cull distance " + lod.CullingDistance);
                    //    if (!lod.IsImpostor)
                    //    {
                    //        lod.CullingDistance = lod.CullingDistance * (Main.ultraMFBBQDistance ? 1000 : (Main.ultraDistance ? 10 : 5));
                    //        //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod info : " + lod.Renderers[0].name + " new cull distance " + lod.CullingDistance);
                    //    }
                    //}
                }

                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : " + (lc.Impostor != null ? " has impostor " : "has no impostor"));
                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : _localImpostorCullingDistance " + fiCull.GetValue(lc));
                if ((lc.gameObject.name.Contains("PINE_SMALL")
                    || lc.gameObject.name.Contains("PALM_")
                    || lc.gameObject.name.Contains("YUCCA")
                    || lc.gameObject.name.Contains("ROCK")
                    || lc.gameObject.name.Contains("CLIFF"))
                    && lc.Impostor == null)
                {
                    Main.fi_Scope.SetValue(lc, ImposterScope.Manual);

                    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller position : " + lc.transform.position);
                    Zone z = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                    if (z == null)
                    {
                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : no zone found for impostor creation");
                    }
                    else
                    {
#warning perf ?
                        Main.mi_CreateImpostor.Invoke(lc, new object[] { });
                    }

                    Main.fi_Scope.SetValue(lc, ImposterScope.Manual);
                }

                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : Scope " + lc.Scope);
                //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : Mode " + lc.Mode);
                if (lc.Impostor != null
                    && lc.Impostor.Lod != null)
                {
                    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : lc.Impostor.Lod.CullingDistance " + lc.Impostor.Lod.CullingDistance);
                    //lc.Impostor.Lod.CullingDistance = (int)(lc.Impostor.Lod.CullingDistance / (Main.ultraMFBBQDistance ? 1000 : (Main.ultraDistance ? 10 : 5)));
                    lc.Impostor.Lod.CullingDistance = farthest;
                    //Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : new lc.Impostor.Lod.CullingDistance " + lc.Impostor.Lod.CullingDistance);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Stranded Deep LOD Mod : error while handling LodController for " + toHandle.gameObject.name + " : " + ex);
            }
        }
    }
}
