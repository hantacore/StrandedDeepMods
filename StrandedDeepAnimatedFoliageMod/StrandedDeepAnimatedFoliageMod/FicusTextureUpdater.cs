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

                TexturesUpdated = true;
            }
        }
    }
}
