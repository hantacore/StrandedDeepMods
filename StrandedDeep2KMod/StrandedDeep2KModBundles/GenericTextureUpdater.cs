using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    public class GenericTextureUpdater : TextureUpdaterBase
    {
        protected override void ReplaceTextures(Renderer renderer, string gameObjectRootName)
        {
            try
            {
                if (TextureReplacerHelper.ReplaceTextures(renderer, gameObjectRootName))
                {
                    TexturesUpdated = true;
                }

                return;
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : " + this.GetType().Name + " ReplaceTextures failed : " + e);
                TexturesUpdated = true;
            }
        }
    }
}
