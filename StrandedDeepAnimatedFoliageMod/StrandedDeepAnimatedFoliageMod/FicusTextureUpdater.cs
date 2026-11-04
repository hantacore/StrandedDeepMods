using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public class FicusTextureUpdater : TextureUpdaterBase
    {
        protected static bool TexturesUpdated { get; set; }

        public override bool AreTexturesUpdated()
        {
            return TexturesUpdated;
        }

        public static void Reset()
        {
            TexturesUpdated = false;
        }

        private static readonly HashSet<Material> _patchedMaterials = new HashSet<Material>();

        protected void ReduceSkyReflections(Renderer r)
        {
            if (r == null) return;
            foreach (Material m in r.sharedMaterials)
            {
                // le HashSet évite de re-patcher le même matériau pour chaque arbre
                if (m == null || !_patchedMaterials.Add(m)) continue;

                Debug.Log("AnimatedFoliage : material " + m.name + " shader=" + m.shader.name);

                if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", 0f);
                if (m.HasProperty("_GlossyReflections")) m.SetFloat("_GlossyReflections", 0f);
                if (m.HasProperty("_SpecularHighlights")) m.SetFloat("_SpecularHighlights", 0f);
                if (m.HasProperty("_SpecColor")) m.SetColor("_SpecColor", Color.black);
                m.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
            }
        }

        protected override void ReplaceTextures(Renderer renderer)
        {
            if (renderer != null)
            {
                foreach (Material m in renderer.sharedMaterials)
                {
                    //Debug.Log("Stranded Deep AnimatedFoliage mod FicusTextureUpdater material name = " + m.name);
                    foreach (string textureName in m.GetTexturePropertyNames())
                    {
                        //Debug.Log("Stranded Deep AnimatedFoliage mod FicusTextureUpdater texture name = " + textureName);

                        string key = "StrandedDeepAnimatedFoliageMod.assets.Textures." + m.name + textureName + ".png";
                        //Debug.Log("Stranded Deep AnimatedFoliage mod FicusTextureUpdater key = " + key);
                        if (Main._indexedTextures.ContainsKey(key))
                        {
                            //Debug.Log("Stranded Deep AnimatedFoliage mod FicusTextureUpdater key = " + key + " found and replaced");
                            m.SetTexture(textureName, Main._indexedTextures[key]);
                        }
                    }
                }

                ReduceSkyReflections(renderer);

                TexturesUpdated = true;
            }
        }
    }
}
