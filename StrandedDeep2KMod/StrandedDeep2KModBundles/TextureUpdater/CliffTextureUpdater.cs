//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace StrandedDeep2KModBundles.TextureUpdater
//{
//    public class CliffTextureUpdater : TextureUpdaterBase
//    {
//        protected override void ReplaceTextures(Renderer renderer)
//        {
//            try
//            {
//                if (renderer != null)
//                {
//                    foreach (Material m in renderer.sharedMaterials)
//                    {
//                        Debug.Log("Stranded Deep 2K Mod : " + this.GetType().Name + " material name = " + m.name);

//                        if (!m.shaderKeywords.Contains("_NORMALMAP"))
//                            m.EnableKeyword("_NORMALMAP");
//                        if (!m.shaderKeywords.Contains("_PARALLAXMAP"))
//                            m.EnableKeyword("_PARALLAXMAP");

//                        m.SetTexture("_BumpMap", Main._indexedTextures["cliffs_bumpmap".ToLower()]);

//                        // stone
//                        m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["cliffs_detailalbedomap".ToLower()]);
//                        //m.SetTextureScale("_DetailAlbedoMap", new Vector2(3f, 3f));
//                        // grass
//                        m.SetTexture("_TertiaryAlbedoMap", Main._indexedTextures["cliffs_tertiaryalbedo".ToLower()]);

//                        TexturesUpdated = true;
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep 2K Mod : " + this.GetType().Name + " ReplaceTextures failed : " + e);
//                TexturesUpdated = true;
//            }
//        }
//    }
//}
