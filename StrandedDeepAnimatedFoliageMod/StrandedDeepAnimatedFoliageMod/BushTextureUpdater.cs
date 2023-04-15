using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public class BushTextureUpdater : TextureUpdaterBase
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

        protected override void ReplaceTextures(Renderer renderer)
        {
            if (renderer != null)
            {
                foreach (Material m in renderer.sharedMaterials)
                {
                    m.SetFloat("_Mode", 2);
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    m.SetInt("_ZWrite", 1);
                    m.EnableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = 2450;

                    //Debug.Log("Stranded Deep AnimatedFoliage mod BushTextureUpdater material name = " + m.name);

                    if (m.name.Contains("Bush_Leafs_MAT"))
                    {
                        m.SetFloat("_Cutoff", 0.99f);
                    }
                    else if (m.name.Contains("Bush_Tile_MAT"))
                    {
                        m.SetFloat("_Cutoff", 0.59f);

                        m.EnableKeyword("_DETAIL_MULX2");
                        //m.SetTexture("_MainTex", Main._indexedTextures["StrandedDeep2KMod.assets.Textures.Bush_Rods_MAT.png"]);
                        //m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["StrandedDeep2KMod.assets.Textures.Bush_Tile_MAT.png"]);
                    }
                    m.SetFloat("_Glossiness", 0f);
                    m.SetFloat("_GlossMapScale", 0f); // Range
                    m.SetFloat("_SmoothnessTextureChannel", 1f); //Enum(Metallic Alpha,0,Albedo Alpha,1)

                    m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    m.SetFloat("_SpecularHighlights", 0f);
                    m.SetFloat("_GlossyReflections", 0f);

                    foreach (string textureName in m.GetTexturePropertyNames())
                    {
                        //Debug.Log("Stranded Deep AnimatedFoliage mod BushTextureUpdater texture name = " + textureName);

                        string key = "StrandedDeepAnimatedFoliageMod.assets.Textures." + m.name + textureName + ".png";
                        //Debug.Log("Stranded Deep AnimatedFoliage mod BushTextureUpdater key = " + key);
                        if (Main._indexedTextures.ContainsKey(key))
                        {
                            //Debug.Log("Stranded Deep AnimatedFoliage mod BushTextureUpdater key = " + key + " found and replaced");
                            m.SetTexture(textureName, Main._indexedTextures[key]);
                        }
                    }
                }

                TexturesUpdated = true;
            }
        }
    }
}
