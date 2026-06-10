using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    internal static class TextureReplacerHelper
    {
        internal static bool ReplaceTextures(Renderer renderer, string gameObjectRootName)
        {
            try
            {
                if (renderer != null)
                {
                    foreach (Material m in renderer.sharedMaterials)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : " + renderer.gameObject.name + " material name = " + m.name);

                        if (m == null)
                        {
                            CustomLogger.Log("Stranded Deep 2K Mod : TextureReplacerHelper " + renderer.gameObject.name + " null sharedmaterial");
                            return false;
                        }

                        if (m.HasProperty("_MainTex"))
                        {
                            CustomLogger.Log("Stranded Deep 2K Mod : TextureReplacerHelper " + renderer.gameObject.name + " sharedmaterial name : " + m.name);
                            string key = m.name.ToLower();
                            Texture2D tex = null;
                            //if (Main._indexedTextures.ContainsKey(key))
                            //    tex = Main._indexedTextures[key];
                            //if (tex == null 
                            //    && Main._indexedTextures.ContainsKey(key + "_MainTex"))
                            //    tex = Main._indexedTextures[key + "_MainTex"];
                            //Texture2D norm = null;
                            //if (Main._indexedTextures.ContainsKey(key + "_BumpMap"))
                            //    norm = Main._indexedTextures[key + "_BumpMap"];
                            //Texture2D occlusion = null;
                            //if (Main._indexedTextures.ContainsKey(key + "_OcclusionMap"))
                            //    occlusion = Main._indexedTextures[key + "_OcclusionMap"];
                            //Texture2D height = null;
                            //if (Main._indexedTextures.ContainsKey(key + "_ParallaxMap"))
                            //    height = Main._indexedTextures[key + "_ParallaxMap"];
                            //Texture2D detail = null;
                            //if (Main._indexedTextures.ContainsKey(key + "_DetailAlbedoMap"))
                            //    detail = Main._indexedTextures[key + "_DetailAlbedoMap"];
                            //Texture2D metallic = null;
                            //if (Main._indexedTextures.ContainsKey(key + "_BumpMap"))
                            //    tex = Main._indexedTextures[key];

                            if (Main._indexedTextures.ContainsKey(key))
                                tex = Main._indexedTextures[key];
                            if (tex == null
                                && Main._indexedTextures.ContainsKey(key + "_maintex"))
                                tex = Main._indexedTextures[key + "_maintex"];
                            Texture2D norm = null;
                            if (Main._indexedTextures.ContainsKey(key + "_bumpmap"))
                                norm = Main._indexedTextures[key + "_bumpmap"];
                            Texture2D occlusion = null;
                            if (Main._indexedTextures.ContainsKey(key + "_occlusionmap"))
                                occlusion = Main._indexedTextures[key + "_occlusionmap"];
                            else if(Main._indexedTextures.ContainsKey(key))
                            {

                                occlusion = Main._indexedTextures[key]; //test
                            }

                            Texture2D height = null;
                            if (Main._indexedTextures.ContainsKey(key + "_parallaxmap"))
                                height = Main._indexedTextures[key + "_parallaxmap"];
                            Texture2D detail = null;
                            if (Main._indexedTextures.ContainsKey(key + "_detailalbedomap"))
                                detail = Main._indexedTextures[key + "_detailalbedomap"];

                            if (tex != null)
                            {
                                m.SetTexture("_MainTex", tex);
                                CustomLogger.Log("Stranded Deep 2K Mod : shared " + renderer.gameObject.name + " texture key : " + key + "_MainTex updated");
                            }
                            else
                            {
                                CustomLogger.Log("Stranded Deep 2K Mod : shared " + renderer.gameObject.name + " texture key not found (" + key + ")");
                            }
                            if (norm != null)
                            {
                                CustomLogger.Log("Stranded Deep 2K Mod : shared " + m.name + " has BumpMap");
                                //CreateBumpedMaterial(material, height != null, occlusion != null);
                                //if (_indexedGlossyMaterialNames.Contains(material.name))
                                //    CreateMetallicGlossBumpedMaterial(material, height != null, occlusion != null);

                                m.SetTexture("_BumpMap", norm);

                                //material.SetTexture("_DetailNormalMap", norm);

                                CustomLogger.Log("Stranded Deep 2K Mod : shared " + renderer.gameObject.name + " texture key : " + key + "_BumpMap updated");
                            }

                            //if (occlusion != null)
                            //{

                            //    m.SetTexture("_OcclusionMap", occlusion);

                            //    CustomLogger.Log("Stranded Deep 2K Mod : shared " + renderer.gameObject.name + " texture key : " + key + "_OcclusionMap updated");
                            //}

                            //if (height != null)
                            //{
                            //    m.SetTexture("_ParallaxMap", height);
                            //    if (!m.shaderKeywords.Contains("_PARALLAXMAP"))
                            //       m.EnableKeyword("_PARALLAXMAP");
                            //    CustomLogger.Log("Stranded Deep 2K Mod : shared " + renderer.gameObject.name + " texture key : " + key + "_ParallaxMap updated");
                            //}

                            if (detail != null)
                            {
                                m.SetTexture("_DetailAlbedoMap", detail);

                                CustomLogger.Log("Stranded Deep 2K Mod : shared " + renderer.gameObject.name + " texture key : " + key + "_DetailAlbedoMap updated");
                            }

                            //if (_indexedShaderColors.ContainsKey(material.name))
                            //{
                            //    CustomLogger.Log("Stranded Deep 2K Mod : shared " + material.name + " setting custom shader color");
                            //    material.SetColor("_Color", _indexedShaderColors[material.name]);
                            //}
                            TextureReplacerHelper.TweakMaterial(m);
                        }

                        //return true;
                    }
                    TextureReplacerHelper.TweakRenderer(renderer, gameObjectRootName);
                }
                return true;
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : TextureReplacerHelper " + (renderer != null ? renderer.gameObject.name : "null renderer") + " ReplaceTextures failed : " + e);
                throw e;
            }

            return false;
        }

        internal static void TweakRenderer(Renderer renderer, string gameObjectRootName)
        {
            CustomLogger.Log("Stranded Deep 2K Mod : TweakRenderer renderer name " + renderer.name);
            CustomLogger.Log("Stranded Deep 2K Mod : TweakRenderer renderer gameobject name " + renderer.gameObject.name);
            CustomLogger.Log("Stranded Deep 2K Mod : TweakRenderer renderer gameobject root name " + gameObjectRootName);
            CustomLogger.Log("Stranded Deep 2K Mod : TweakRenderer renderer sharedMaterial name " + renderer.sharedMaterial.name);

            if (renderer.gameObject.name.ToLower().Contains("ficus_tree"))
            {
                if (renderer.sharedMaterial.name == "BARK")
                {
                    renderer.sharedMaterial.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    renderer.sharedMaterial.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                    renderer.sharedMaterial.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                    renderer.sharedMaterial.SetFloat("_SpecularHighlights", 0f);
                    renderer.sharedMaterial.SetFloat("_Glossiness", 0f);
                    renderer.sharedMaterial.SetFloat("_Metallic", 0f);
                    renderer.sharedMaterial.SetFloat("_GlossyReflections", 0f);

                    renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(5f, 5f));
                    renderer.sharedMaterial.SetFloat("_BumpScale", 0.3f);

                    if (Main._indexedTextures.ContainsKey("detail_normal_noise"))
                    {
                        renderer.sharedMaterial.EnableKeyword("_DETAIL_MULX2");
                        renderer.sharedMaterial.SetTexture("_DetailNormalMap", Main._indexedTextures["detail_normal_noise"]);
                    }
                }
            }
            if (renderer.sharedMaterial.name.ToLower().Contains("palm") || renderer.sharedMaterial.name.ToLower().Contains("pine"))
            {
                CustomLogger.Log("Stranded Deep 2K Mod : tweaking material " + renderer.sharedMaterial.name);
                CustomLogger.Log("Stranded Deep 2K Mod : material " + renderer.sharedMaterial.name + " shader = " + renderer.sharedMaterial.shader.name);
                foreach (Material mat in renderer.sharedMaterials)
                {
                    mat.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    mat.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                    mat.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                    mat.SetFloat("_SpecularHighlights", 0f);
                    mat.SetFloat("_Glossiness", 0f);
                    mat.SetFloat("_GlossyReflections", 0f);
                }
            }
            if (renderer.sharedMaterial.name == "SmallRocks")
            {
                if (Main._indexedTextures.ContainsKey("smallRocks_detailalbedomap"))
                {
                    renderer.sharedMaterial.EnableKeyword("_DETAIL_MULX2");
                    renderer.sharedMaterial.SetTexture("_DetailNormalMap", Main._indexedTextures["smallRocks_detailalbedomap"]);
                }
            }
            if (renderer.sharedMaterial.name == "tile_debris")
            {
                renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(20f, 20f));
            }
            if (renderer.sharedMaterial.name == "Rock6")
            {
                renderer.sharedMaterial.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                renderer.sharedMaterial.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                renderer.sharedMaterial.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                renderer.sharedMaterial.SetFloat("_Glossiness", 0f);
                renderer.sharedMaterial.SetFloat("_SpecularHighlights", 0f);
            }
            if (renderer.sharedMaterial.name == "CLIFFS")
            {
                CustomLogger.Log("Stranded Deep 2K Mod : " + renderer.sharedMaterial.name + " shader name = " + renderer.sharedMaterial.shader.name);
                if (renderer != null)
                {
                    foreach (Material m in renderer.sharedMaterials)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : material name = " + m.name);

                        m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                        m.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                        m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                        m.SetFloat("_Glossiness", 0f);
                        m.SetFloat("_SpecularHighlights", 0f);

                        if (!m.shaderKeywords.Contains("_NORMALMAP"))
                            m.EnableKeyword("_NORMALMAP");

                        m.SetTexture("_BumpMap", Main._indexedTextures["cliffs_bumpmap".ToLower()]);

                        // stone
                        m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["cliffs_detailalbedomap".ToLower()]);
                        //m.SetTextureScale("_DetailAlbedoMap", new Vector2(3f, 3f));
                        // grass
                        m.SetTexture("_TertiaryAlbedoMap", Main._indexedTextures["cliffs_tertiaryalbedo".ToLower()]);

                        m.EnableKeyword("_DETAIL_MULX2");
                        m.SetTexture("_DetailNormalMap", Main._indexedTextures["cliffs_detailnormalmap"]);
                    }
                }
            }
            if (gameObjectRootName.ToLower().Contains("pine_small"))
            {
                if (renderer.sharedMaterial.name.ToLower().CompareTo("smallpines_mat") != 0)
                {
                    renderer.sharedMaterial = new Material(renderer.sharedMaterial);
                    renderer.sharedMaterial.name = "SmallPines_MAT";
                    renderer.sharedMaterial.SetTexture("_MainTex", Main._indexedTextures["smallpines_mat"]);
                    renderer.sharedMaterial.SetTexture("_OcclusionMap", Main._indexedTextures["smallpines_mat"]);
                    renderer.sharedMaterial.SetTexture("_BumpMap", Main._indexedTextures["smallpines_mat_bumpmap"]);
                    //renderer.sharedMaterial.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    //renderer.sharedMaterial.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                    //renderer.sharedMaterial.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                    //renderer.sharedMaterial.SetFloat("_SpecularHighlights", 0f);
                    //renderer.sharedMaterial.SetFloat("_Glossiness", 0f);
                    //renderer.sharedMaterial.SetFloat("_GlossyReflections", 0f);
                }
            }
            //if (renderer.gameObject.name.Contains("grass"))
            //{
            //    renderer.sharedMaterial.shader = Shader.Find("Beam Team/Fish");
            //    renderer.sharedMaterial.EnableKeyword("_VERTEX_ANIMATION");

            //    renderer.sharedMaterial.SetVector("_Speed", new Vector4(0.2f, 0.0f, 0.2f, 1.00f));
            //    renderer.sharedMaterial.SetVector("_Frequency", new Vector4(0.30f, 0.00f, 0.30f, 1.00f));
            //    renderer.sharedMaterial.SetVector("_Amplitude", new Vector4(0.1f, 0f, 0.1f, 1.00f));
            //    renderer.sharedMaterial.SetFloat("_HeadLimit", 0.0f);
            //    renderer.sharedMaterial.SetFloat("_Cutoff", 0.5f);

            //    renderer.sharedMaterial.SetFloat("_Mode", 2);
            //    renderer.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            //    renderer.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            //    renderer.sharedMaterial.SetInt("_ZWrite", 1);
            //    renderer.sharedMaterial.EnableKeyword("_ALPHATEST_ON");
            //    renderer.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
            //    renderer.sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //    renderer.sharedMaterial.renderQueue = 2450;
            //}
            if (renderer.sharedMaterial.name.ToLower().Contains("bush_leafs_mat") || renderer.sharedMaterial.name.ToLower().Contains("bush_tile_mat"))
            {
                renderer.sharedMaterial.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                renderer.sharedMaterial.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                renderer.sharedMaterial.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                renderer.sharedMaterial.SetFloat("_Glossiness", 0f);
                renderer.sharedMaterial.SetFloat("_SpecularHighlights", 0f);

                if (renderer.sharedMaterial.name.ToLower().Contains("bush_tile_mat"))
                {
                    if (!Main._indexedTextures.ContainsKey("bush_rods_mat"))
                        CustomLogger.Log("Stranded Deep 2K Mod : bush rods texture not found");

                    Texture2D tex = Main._indexedTextures["bush_rods_mat"];
                    Texture2D norm = Main._indexedTextures["bush_rods_mat_bumpmap"];
                    Texture2D occlusion = Main._indexedTextures["bush_rods_mat_occlusionmap"];
                    Texture2D height = Main._indexedTextures["bush_rods_mat_heightmap"];
                    Texture2D detail = null;// Main._indexedTextures["bush_rods_mat"];
                    Texture2D metallic = null;// Main._indexedTextures["bush_rods_mat"];

                    renderer.sharedMaterial.SetFloat("_Mode", 2);
                    renderer.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    renderer.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    renderer.sharedMaterial.SetInt("_ZWrite", 1);
                    renderer.sharedMaterial.EnableKeyword("_ALPHATEST_ON");
                    renderer.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
                    renderer.sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer.sharedMaterial.renderQueue = 2450;

                    renderer.sharedMaterial.SetFloat("_Glossiness", 1f);
                    renderer.sharedMaterial.SetFloat("_Parallax", 0.0116f);

                    if (tex != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : adding cloned bush rods texture");
                        renderer.sharedMaterial.SetTexture("_MainTex", tex);
                        CustomLogger.Log("Stranded Deep 2K Mod : bush texture scale : " + renderer.sharedMaterial.GetTextureScale("_MainTex").ToString());
                        renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(14f, 14f));


                        if (occlusion != null)
                            renderer.sharedMaterial.SetTexture("_OcclusionMap", occlusion);

                        if (height != null)
                            renderer.sharedMaterial.SetTexture("_ParallaxMap", height);

                        if (detail != null)
                            renderer.sharedMaterial.SetTexture("_DetailAlbedoMap", detail);
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : bush_rods_mat texture not found");
                    }
                }
            }

//            if (renderer.gameObject.name.ToLower().Contains("ground_cover_a") || renderer.gameObject.name.ToLower().Contains("ground_cover_c"))
//            {
//#warning ground cover
//                renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(2f, 2f));
//            }
        }

        internal static void TweakMaterial(Material m)
        {
            try
            {
                CustomLogger.Log("Stranded Deep 2K Mod : TweakMaterial renderer material name " + m.name);

                //if (m.name.ToLower().Contains("Bush_Leafs_MAT") || m.name.ToLower().Contains("Bush_Tile_MAT"))
                //{
                //    m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                //    m.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                //    m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                //    m.SetFloat("_Glossiness", 0f);
                //    m.SetFloat("_SpecularHighlights", 0f);

                //    if (m.name.ToLower().Contains("Bush_Tile_MAT"))
                //    {
                //        Texture2D tex = Main._indexedTextures["bush_rods_mat"];
                //        Texture2D norm = Main._indexedTextures["bush_rods_mat"];
                //        Texture2D occlusion = Main._indexedTextures["bush_rods_mat"];
                //        Texture2D height = Main._indexedTextures["bush_rods_mat"];
                //        Texture2D detail = Main._indexedTextures["bush_rods_mat"];
                //        Texture2D metallic = Main._indexedTextures["bush_rods_mat"];

                //        m.SetFloat("_Mode", 2);
                //        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                //        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                //        m.SetInt("_ZWrite", 1);
                //        m.EnableKeyword("_ALPHATEST_ON");
                //        m.DisableKeyword("_ALPHABLEND_ON");
                //        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //        m.renderQueue = 2450;

                //        if (tex != null)
                //        {
                //            CustomLogger.Log("Stranded Deep 2K Mod : adding cloned bush rods texture");
                //                m.SetTexture("_MainTex", tex);

                //            if (occlusion != null)
                //                m.SetTexture("_OcclusionMap", occlusion);

                //            if (height != null)
                //                m.SetTexture("_ParallaxMap", height);

                //            if (detail != null)
                //                m.SetTexture("_DetailAlbedoMap", detail);
                //        }
                //        else
                //        {
                //            CustomLogger.Log("Stranded Deep 2K Mod : bush_rods_mat texture not found");
                //        }
                //    }
                //}
                if (m.name.ToLower().Contains("barrel_mat"))
                {

                }
                // barrel chain transparency
                if (m.name.ToLower().Contains("rope_mat"))
                {
                    m.SetFloat("_Mode", 2);
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    m.SetInt("_ZWrite", 1);
                    m.EnableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = 2450;
                }
                if (m.name.ToLower().Contains("plank_mat"))
                {
                    m.SetFloat("_Glossiness", 0f);
                }
                if (m.name.ToLower().Contains("smallrocks"))
                {
                    m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    m.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                    m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                    m.SetFloat("_Glossiness", 0f);
                    m.SetFloat("_SpecularHighlights", 0f);
                }
                if (m.name == "Rock_MAT")
                {
                    m.SetTextureScale("_MainTex", new Vector2(3f, 3f));
                    if (Main._indexedTextures.ContainsKey("detail_normal_noise"))
                    {
                        m.EnableKeyword("_DETAIL_MULX2");
                        m.SetTexture("_DetailNormalMap", Main._indexedTextures["detail_normal_noise"]);
                    }

                }
                if (m.name == "PALM_BARK_DIFF")
                {
                    if (Main._indexedTextures.ContainsKey("palm_bark_detailnormalmap"))
                    {
                        m.EnableKeyword("_DETAIL_MULX2");
                        m.SetTexture("_DetailNormalMap", Main._indexedTextures["palm_bark_detailnormalmap"]);
                    }
                }
                if (m.name == "Rock6")
                {
                    //m.SetTexture("_MainTex", Main._indexedTextures["mire"]);

                    m.SetTextureScale("_BumpMap", new Vector2(1f, 1f));

                    m.EnableKeyword("_DETAIL_MULX2");
                    //m.SetTexture("_DetailMask", Main._indexedTextures["random_mask"]);
                    //m.SetTextureScale("_DetailMask", new Vector2(3f, 3f));
                    //m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["terrain_sand_wet_diff"]);
                    //m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["rock_0004_color_"]);
                    //m.SetTexture("_DetailNormalMap", Main._indexedTextures["rock_0004_normal_opengl_"]);
                    m.SetTextureScale("_DetailAlbedoMap", new Vector2(10f, 10f));
                    m.SetTextureScale("_DetailNormalMap", new Vector2(10f, 10f));
                }

                if (m.name == "Kelp_2_MAT")
                {
                    //if (Main._indexedTextures.ContainsKey("detail_normal_noise"))
                    //{
                    //    m.EnableKeyword("_DETAIL_MULX2");
                    //    m.SetTexture("_DetailNormalMap", Main._indexedTextures["detail_normal_noise"]);
                    //}
                    m.SetFloat("_Mode", 2);
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    m.SetInt("_ZWrite", 1);
                    m.EnableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = 2450;
                }
                if (m.name.ToLower().Contains("metal") 
                    || m.name.ToLower().Contains("rust") 
                    || m.name.ToLower().Contains("wreck") 
                    || m.name.ToLower().Contains("smokestack") 
                    || m.name.ToLower().Contains("submarine")
                    || m.name.ToLower().Contains("plank")
                    || m.name.ToLower().Contains("vegetation"))
                    //|| m.name.ToLower().Contains("ledge"))
                {
                    if (Main._indexedTextures.ContainsKey("detail_normal_noise"))
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : adding detail_normal_noise to wreck texture : " + m.name);
                        m.EnableKeyword("_DETAIL_MULX2");
                        m.SetTexture("_DetailNormalMap", Main._indexedTextures["detail_normal_noise"]);
                    }
                }

                if (m.name.Contains("BRANCH"))
                {
                    CustomLogger.Log("Stranded Deep 2K Mod : handling BRANCH shader values");

                    m.SetFloat("_Glossiness", 0f);
                    m.SetFloat("_Metallic", 0f);

                    m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    m.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                    m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                    m.DisableKeyword("_SPECGLOSSMAP");
                    m.SetFloat("_SpecularHighlights", 0.0f);

                    m.SetTexture("_BumpMap", m.mainTexture);
                    //material.SetTexture("_OcclusionMap", material.mainTexture);
                    //material.SetTexture("_DetailAlbedoMap", material.mainTexture);
                    //m.SetTexture("_DetailAlbedoMap", _indexedTextures["StrandedDeep2KMod.assets.Textures.Detail_Normal_Noise.png"]);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : " + m.name + " TweakMaterial failed : " + ex);
            }
        }

        public static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}
