using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    public class TextureUpdaterBase : MonoBehaviour
    {
        internal static Dictionary<string, bool> _texturesUpdated = new Dictionary<string, bool>();

        protected bool TexturesUpdated
        {
            get
            {
                return AreTexturesUpdated();
            }
            set
            {
                SetTexturesUpdated(value);
            }
        }

        public bool AreTexturesUpdated()
        {
            if (this.gameObject == null)
                return false;

            if (String.IsNullOrEmpty(this.gameObject.name))
            {
                CustomLogger.Log("Stranded Deep 2K Mod cannot get texture update status, gameObject null or empty");
                return false;
            }

            if (_texturesUpdated.ContainsKey(this.gameObject.name))
                return _texturesUpdated[this.gameObject.name];

            return false;
        }

        private void SetTexturesUpdated(bool value)
        {
            if (this.gameObject == null)
                return;

            if (String.IsNullOrEmpty(this.gameObject.name))
            {
                CustomLogger.Log("Stranded Deep 2K Mod cannot set texture update status, gameObject null or empty");
                return;
            }

            if (_texturesUpdated.ContainsKey(this.gameObject.name))
                _texturesUpdated[this.gameObject.name] = value;
            else
                _texturesUpdated.Add(this.gameObject.name, value);
        }

        public static void Reset()
        {
            _texturesUpdated.Clear();
        }

        public void Update()
        {
            try
            {
                if (AreTexturesUpdated())
                    return;

                if (gameObject != null)
                {
                    //Renderer r = null;
                    Renderer[] rs = null;
                    bool hasRenderer = false;


                    CustomLogger.Log("Stranded Deep 2K Mod trying to update texture for " + gameObject.name);

                    rs = gameObject.GetComponentsInChildren<MeshRenderer>();
                    if (rs != null && rs.Length > 0)
                    {
                        CustomLogger.Log("Stranded Deep 2K  Mod update texture : MeshRenderers found in children " + rs.Length);
                        foreach (Renderer r2 in rs)
                        {
                            CustomLogger.Log("Stranded Deep 2K  Mod update texture : MeshRenderer found in children " + r2.GetType().Name);
                            ReplaceTextures(r2, gameObject.name);
                        }
                        hasRenderer = true;
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : MeshRenderers NULL or empty in children");
                    }

                    rs = GetComponentsInChildren<SkinnedMeshRenderer>();
                    if (rs != null && rs.Length > 0)
                    {
                        CustomLogger.Log("Stranded Deep 2K  Mod update texture : SkinnedMeshRenderers found in children " + rs.Length);
                        foreach (Renderer r2 in rs)
                        {
                            CustomLogger.Log("Stranded Deep 2K  Mod update texture : SkinnedMeshRenderer found in children " + r2.GetType().Name);
                            ReplaceTextures(r2, gameObject.name);
                        }
                        hasRenderer = true;
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : SkinnedMeshRenderer NULL in children");
                    }

                    rs = GetComponentsInChildren<ParticleSystemRenderer>();
                    if (rs != null && rs.Length > 0)
                    {
                        CustomLogger.Log("Stranded Deep 2K  Mod update texture : ParticleSystemRenderers found in children " + rs.Length);
                        foreach (Renderer r2 in rs)
                        {
                            CustomLogger.Log("Stranded Deep 2K Mod update texture : ParticleSystemRenderer found in children " + r2.GetType().Name);
                            if (r2.name.CompareTo("oceanParticles") != 0
                                && r2.name.CompareTo("jellyfishParticles") != 0
                                && r2.name.CompareTo("smallfishesParticles") != 0
                                && r2.name.CompareTo("shrimpParticles") != 0)
                            {
                                ReplaceTextures(r2, gameObject.name);
                            }
                            else
                            {
                                CustomLogger.Log("Stranded Deep 2K Mod update texture : ParticleSystemRenderer from LOD mod skipped");
                            }
                            hasRenderer = true;
                        }
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : ParticleSystemRenderer NULL in children");
                    }

                    if (!hasRenderer)
                        TexturesUpdated = true;
                }

            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod error in TextureUpdaterBase.Update " + e);
            }
        }

        #region backup

        public void Update_backup()
        {
            try
            {
                if (AreTexturesUpdated())
                    return;

                if (gameObject != null)
                {
                    Renderer r = null;
                    Renderer[] rs = null;
                    bool hasRenderer = false;


                    CustomLogger.Log("Stranded Deep 2K Mod trying to update texture for " + gameObject.name);

                    r = GetComponentInChildren<MeshRenderer>();
                    if (r != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : renderer found in children " + r.GetType().Name);
                        ReplaceTextures(r, gameObject.name);
                        hasRenderer = true;
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : renderer NULL in children");

                        //rs = gameObject.GetComponentsInChildren<MeshRenderer>();
                        //if (rs != null && rs.Length > 0)
                        //{
                        //    CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderers found in children " + rs.Length);
                        //    foreach (Renderer r2 in rs)
                        //    {
                        //        CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderer found in children " + r2.GetType().Name);
                        //        ReplaceTextures(r2);
                        //    }
                        //}
                        //else
                        //{
                        //    CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderers NULL in children");

                        //    r = GetComponent<MeshRenderer>();
                        //    if (r != null)
                        //    {
                        //        CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderer found in self");
                        //        ReplaceTextures(r);
                        //    }
                        //    else
                        //    {
                        //        CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderer NULL in self");

                        //        rs = GetComponents<MeshRenderer>();
                        //        if (rs != null && rs.Length > 0)
                        //        {
                        //            CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderers found in self " + rs.Length);
                        //            foreach (Renderer r2 in rs)
                        //            {
                        //                ReplaceTextures(r2);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderers NULL in self");
                        //            r = GetComponentInParent<MeshRenderer>();
                        //            if (r != null && rs.Length > 0)
                        //            {
                        //                CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderer found in parent");
                        //                ReplaceTextures(r);
                        //            }
                        //            else
                        //            {
                        //                CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderer NULL in parent");
                        //                rs = GetComponentsInParent<MeshRenderer>();
                        //                if (rs != null)
                        //                {
                        //                    CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderers found in parent " + rs.Length);
                        //                    foreach (Renderer r2 in rs)
                        //                    {
                        //                        ReplaceTextures(r2);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    CustomLogger.Log("Stranded Deep 2K  Mod update texture : renderers NULL in parent");
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    r = GetComponentInChildren<SkinnedMeshRenderer>();
                    if (r != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : SkinnedMeshRenderer found in children " + r.GetType().Name);
                        ReplaceTextures(r, gameObject.name);
                        hasRenderer = true;
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : SkinnedMeshRenderer NULL in children");
                    }

                    r = GetComponentInChildren<ParticleSystemRenderer>();
                    if (r != null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : ParticleSystemRenderer found in children " + r.GetType().Name);
                        if (r.name.CompareTo("oceanParticles") != 0
                            && r.name.CompareTo("jellyfishParticles") != 0
                            && r.name.CompareTo("smallfishesParticles") != 0
                            && r.name.CompareTo("shrimpParticles") != 0)
                        {
                            ReplaceTextures(r, gameObject.name);
                            hasRenderer = true;
                        }
                        else
                        {
                            CustomLogger.Log("Stranded Deep 2K Mod update texture : ParticleSystemRenderer from LOD mod skipped");
                        }
                    }
                    else
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod update texture : ParticleSystemRenderer NULL in children");
                    }

                    if (!hasRenderer)
                        TexturesUpdated = true;
                }

            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod error in TextureUpdaterBase.Update " + e);
            }
        }

        #endregion

        protected virtual void ReplaceTextures(Renderer renderer, string gameObjectRootName)
        { }
    }
}
