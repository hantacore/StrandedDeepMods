using System.Collections.Generic;
using UnityEngine;

namespace StrandedDeepBetterBushesMod
{
    public class BushTextureUpdater : TextureUpdaterBase
    {
        // Les matériaux sont partagés entre tous les buissons : on ne les patche qu'une fois.
        private static readonly HashSet<Material> _patchedMaterials = new HashSet<Material>();

        // Réglages des renderers : à refaire pour CHAQUE buisson (ce ne sont pas des propriétés de matériau).
        private bool _instanceDone;

        // Mettre à false pour tester si le noir de la face arrière vient des ombres reçues.
        private const bool DisableReceiveShadows = true;

        private static readonly Color LeafEmission = new Color(0.18f, 0.22f, 0.12f);

        public override bool AreTexturesUpdated()
        {
            return _instanceDone;
        }

        public static void Reset()
        {
            _patchedMaterials.Clear();
        }

        protected override void ReplaceTextures(Renderer ignored)
        {
            // La base ne fournit que le premier renderer : on les prend tous.
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                    continue;

                if (DisableReceiveShadows)
                    renderer.receiveShadows = false;

                foreach (Material m in renderer.sharedMaterials)
                {
                    if (m == null || !_patchedMaterials.Add(m))
                        continue;

                    PatchMaterial(m);
                }
            }

            _instanceDone = true;
            enabled = false; // plus besoin d'Update pour cette instance
        }

        private static void PatchMaterial(Material m)
        {
            m.SetFloat("_Mode", 1); // Cutout
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            m.SetInt("_ZWrite", 1);
            m.EnableKeyword("_ALPHATEST_ON");
            m.DisableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 2450;

            bool isLeaf = m.name.Contains("Bush_Leafs_MAT");
            bool isTile = m.name.Contains("Bush_Tile_MAT");

            if (isLeaf)
            {
                m.SetFloat("_Cutoff", 0.99f);
                m.SetShaderPassEnabled("ShadowCaster", false);
            }
            else if (isTile)
            {
                m.SetFloat("_Cutoff", 0.59f);
                m.EnableKeyword("_DETAIL_MULX2");
            }

            m.SetFloat("_Glossiness", 0f);
            m.SetFloat("_GlossMapScale", 0f);
            m.SetFloat("_SmoothnessTextureChannel", 1f);
            m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
            m.SetFloat("_SpecularHighlights", 0f);
            m.SetFloat("_GlossyReflections", 0f);
            m.EnableKeyword("_GLOSSYREFLECTIONS_OFF");

            foreach (string textureName in m.GetTexturePropertyNames())
            {
                string key = "StrandedDeepBetterBushesMod.assets.Textures." + m.name + textureName + ".png";
                Texture2D tex;
                if (Main._indexedTextures.TryGetValue(key, out tex))
                    m.SetTexture(textureName, tex);
            }

            // Fausse translucidité : à faire APRÈS le remplacement des textures
            if (isLeaf)
            {
                m.EnableKeyword("_EMISSION");
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                m.SetColor("_EmissionColor", LeafEmission);

                Texture albedo = m.GetTexture("_MainTex");
                if (albedo != null)
                    m.SetTexture("_EmissionMap", albedo);
            }
        }
    }
}