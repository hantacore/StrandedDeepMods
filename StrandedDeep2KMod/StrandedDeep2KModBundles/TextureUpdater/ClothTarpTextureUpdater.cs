using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles.TextureUpdater
{
    class ClothTarpTextureUpdater : TextureUpdaterBase
    {
        protected override void ReplaceTextures(Renderer renderer, string gameObjectRootName)
        {
            try
            {
                if (renderer != null)
                {
                    foreach (Material m in renderer.sharedMaterials)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : " + this.GetType().Name + " material name = " + m.name);

                        if (m.name.Contains("Tarp_MAT") || m.name.Contains("Cloth_MAT"))
                        {
                            m.SetTexture("_MainTex", Main._indexedTextures["tarp_cloth_mat"]);
                            m.SetTexture("_BumpMap", Main._indexedTextures["tarp_cloth_mat"]);
                        }

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
