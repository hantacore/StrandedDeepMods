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
        // Cache des LodControllers pour lesquels CreateImpostor a déjà été invoqué.
        // Évite de rappeler CreateImpostor par réflexion à chaque passe de traitement
        // (résout le #warning perf ? de la version originale).
        private readonly HashSet<LodController> _impostorCreated = new HashSet<LodController>();

        public LODHandler() : base(2, true)
        {
        }

        public override void Reset()
        {
            base.Reset();
            _impostorCreated.Clear();
        }

        protected override void HandleOne(LodController toHandle)
        {
            try
            {
                LodController lc = toHandle;

                Main.fi_Scope.SetValue(lc, ImposterScope.Manual);

                int dist = (int)Main.fi_Cull.GetValue(lc);
                Main.fi_Cull.SetValue(lc, 2000);

                int bestLOD = 3;
                for (int i = 0; i < lc.LodGroup.Lods.Count; i++)
                {
                    Lod lod = lc.LodGroup.Lods[i];
                    if (lod.Renderers[0].name.Contains("LOD0") && bestLOD > 0)
                        bestLOD = 0;
                    else if (lod.Renderers[0].name.Contains("LOD1") && bestLOD > 1)
                        bestLOD = 1;
                    else if (lod.Renderers[0].name.Contains("LOD2") && bestLOD > 2)
                        bestLOD = 2;
                }

                int farthest = int.MinValue;

                // FIX: on utilise le multiplicateur configurable (lodDistanceMultiplier) au lieu
                // de valeurs hardcodées. ultraMFBBQDistance (x1000) reste disponible pour le debug.
                int multiplier = Main.ultraMFBBQDistance ? 1000 : (Main.ultraDistance ? 10 : Main.lodDistanceMultiplier);

                for (int i = lc.LodGroup.Lods.Count - 1; i >= 0; i--)
                {
                    Lod lod = lc.LodGroup.Lods[i];

                    if (!lod.IsImpostor)
                    {
                        lod.CullingDistance = lod.CullingDistance * multiplier;
                        if (lod.CullingDistance > farthest)
                            farthest = lod.CullingDistance + 1;
                    }
                }

                if ((lc.gameObject.name.Contains("PINE_SMALL")
                    || lc.gameObject.name.Contains("PALM_")
                    || lc.gameObject.name.Contains("YUCCA")
                    || lc.gameObject.name.Contains("ROCK")
                    || lc.gameObject.name.Contains("CLIFF"))
                    && lc.Impostor == null)
                {
                    Main.fi_Scope.SetValue(lc, ImposterScope.Manual);

                    Zone z = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                    if (z == null)
                    {
                        Debug.Log("Stranded Deep LOD Mod : " + lc.gameObject.name + " : lod controller : no zone found for impostor creation");
                    }
                    else
                    {
                        // FIX: on ne crée l'impostor qu'une seule fois par LodController
                        // (cache _impostorCreated) pour éviter des appels répétés par réflexion.
                        if (!_impostorCreated.Contains(lc))
                        {
                            Main.mi_CreateImpostor.Invoke(lc, new object[] { });
                            _impostorCreated.Add(lc);
                        }
                    }

                    Main.fi_Scope.SetValue(lc, ImposterScope.Manual);
                }

                if (lc.Impostor != null && lc.Impostor.Lod != null)
                {
                    lc.Impostor.Lod.CullingDistance = farthest;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Stranded Deep LOD Mod : error while handling LodController for " + toHandle.gameObject.name + " : " + ex);
            }
        }
    }
}
