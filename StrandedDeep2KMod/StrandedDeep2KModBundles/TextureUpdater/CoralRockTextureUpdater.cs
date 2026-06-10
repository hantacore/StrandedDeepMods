using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles.TextureUpdater
{
    public class CoralRockTextureUpdater : TextureUpdaterBase
    {
        //static Dictionary<string, string> _texturesMapping = new Dictionary<string, string>();

        //static CoralRockTextureUpdater()
        //{
        //    _texturesMapping.Add("", "Assets/Sea/CoralRock_Diff4K_colorized");
        //    _texturesMapping.Add("", "Assets/Sea/CoralRock_NRM");
        //    _texturesMapping.Add("", "Assets/Sea/Coral_RockDetail (Instance)8K.png");
        //}

        protected override void ReplaceTextures(Renderer renderer, string gameObjectRootName)
        {
            try
            {
//Stranded Deep 2K Mod : adding CoralRockTextureUpdater component to CORAL_ROCK3(Clone)
//Stranded Deep 2K Mod : adding CoralRockTextureUpdater component to CORAL_ROCK_2(Clone)
//Stranded Deep 2K Mod : adding CoralRockTextureUpdater component to CORAL_ROCK_1(Clone)

                if (renderer != null)
                {
                    foreach (Material m in renderer.sharedMaterials)
                    {
                        if (!string.IsNullOrEmpty(m.name))
                            CustomLogger.Log("Stranded Deep 2K Mod : " + this.GetType().Name + " material name = " + m.name);

                        //m.SetTexture("_MainTex", Main._indexedTextures["Assets/Sea/2_Coral_Rock1 (Instance)".ToLower()]);
                        //m.SetTexture("_BumpMap", Main._indexedTextures["Assets/Sea/2_Coral_Rock1 (Instance)_BumpMap".ToLower()]);

                        //m.SetColor("_Color", new Color(1, 1, 1, 1));
                        m.SetTexture("_MainTex", Main._indexedTextures["coralrock_diff_colorized"]);
                        m.SetTexture("_BumpMap", Main._indexedTextures["coralrock_nrm"]);
                          m.SetTexture("_OcclusionMap", Main._indexedTextures["coralrock_diff_colorized"]);
                        m.SetTexture("_ParallaxMap", Main._indexedTextures["coralrock_occ"]);


                        m.SetTextureScale("_MainTex", new Vector2(10f, 10f));
                        m.SetTextureScale("_BumpMap", new Vector2(10f, 10f));
                          m.SetTextureScale("_OcclusionMap", new Vector2(4f, 4f));
                        m.SetTextureScale("_ParallaxMap", new Vector2(10f, 10f));
                        //m.SetFloat("_Parallax", 0.0104f);
                        //m.SetFloat("_Glossiness", 0f);
                        //m.SetFloat("_BumpScale", 0.5f);

                        //m.SetTexture("_DetailMask", Main._indexedTextures["coral_rockdetail"]);
                        //m.EnableKeyword("_DETAIL_MULX2");
                        //m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["coral_rockdetail"]);
                        ////m.SetTexture("_DetailNormalMap", Main._indexedTextures["coral_rock_texture_norm"]);
                        //m.SetTextureScale("_DetailAlbedoMap", new Vector2(24f, 24f));

                        TexturesUpdated = true;
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : " + this.GetType().Name + " ReplaceTextures failed : " + e);
                TexturesUpdated = true;
            }
        }
    }
}
