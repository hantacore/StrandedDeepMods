using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public class TextureUpdaterBase : MonoBehaviour
    {
        public virtual bool AreTexturesUpdated()
        { return false; }

        public void Update()
        {
            try
            {
                if (AreTexturesUpdated())
                    return;

                if (gameObject != null)
                {
                    //Debug.Log("Stranded Deep AnimatedFoliage Mod trying to update texture for " + gameObject.name);
                    Renderer r = GetComponentInChildren<Renderer>();
                    if (r != null)
                    {
                        //Debug.Log("Stranded Deep AnimatedFoliage Mod update texture : renderer found");
                        ReplaceTextures(r);
                    }
                    //else
                    //{
                    //    Debug.Log("Stranded Deep AnimatedFoliage Mod update texture : renderer NULL");
                    //}
                }
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod error in TextureUpdaterBase.Update " + e);
            }
        }

        protected virtual void ReplaceTextures(Renderer renderer)
        { }
    }
}
