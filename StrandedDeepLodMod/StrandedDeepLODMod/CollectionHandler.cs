﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepLODMod
{
    public class CollectionHandler<T>
    {
        T[] ListToHandle = null;
        protected List<T> _handledElements = new List<T>();
        int lastPassCount = 0;
        int currentIndex = -1;
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public int MaxMilliseconds { get; set; }

        public string ModName { get; set; }

        public bool MustFillQueue
        {
            get
            {
                if (currentIndex > 0
                    || ListToHandle != null)
                    return false;

                return true;
            }
        }

        public bool StoreHandled { get; set; }

        public CollectionHandler(int maxMilliseconds = 2, bool storeHandled = false)
        {
            StoreHandled = storeHandled;
            MaxMilliseconds = maxMilliseconds;
        }

        public virtual void InitCollectionHandler()
        {

        }

        public void FillQueue(T[] collection)
        {
            try
            {
                if (!MustFillQueue)
                    return;

                int count = collection.Count();
                if (count == 0
                    || lastPassCount == count)
                {
                    //Debug.Log("Stranded Deep LOD Mod : no fish renderer found : " + frbs.Length + " / " + lastPassFishRendererBaseCount);
                    return;
                }

                lastPassCount = count;

                ListToHandle = new T[lastPassCount];
                Array.Copy(collection, ListToHandle, lastPassCount);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep " + ModName + " : FillQueue failed : " + e);
            }
        }

        public void Handle()
        {
            try
            {
                if (ListToHandle == null
                    || ListToHandle.Length == 0)
                {
                    // game reset issue
                    if (ListToHandle != null && ListToHandle.Length == 0)
                        ListToHandle = null;
                    return;
                }
                sw.Restart();
                try
                {
                    if (currentIndex < 0)
                        currentIndex = ListToHandle.Length - 1;

                    while (currentIndex >= 0
                        && sw.ElapsedMilliseconds <= MaxMilliseconds)
                    {
                        T element = ListToHandle[currentIndex];
                        try
                        {
                            //if (element is Beam.Rendering.LodController)
                            //{
                            //    Beam.Rendering.LodController lc = element as Beam.Rendering.LodController;
                            //    if (lc != null
                            //        && lc.gameObject != null
                            //        && lc.gameObject.name.Contains("CLIFF"))
                            //    {
                            //        Debug.Log("Stranded Deep LOD mod : element " + lc.gameObject.name + " distance to player = " + Vector3.Distance(lc.gameObject.transform.position, Beam.PlayerRegistry.LocalPlayer.transform.position));
                            //        Renderer r = lc.gameObject.GetComponent<Renderer>();
                            //        if (r != null)
                            //        {
                            //            Debug.Log("Stranded Deep LOD Mod : cliff shader " + r.sharedMaterial.shader.name);
                            //            for (int property = 0; property < r.sharedMaterial.shader.GetPropertyCount(); property++)
                            //            {
                            //                Debug.Log("Stranded Deep LOD Mod : cliff shader property " + r.sharedMaterial.shader.GetPropertyName(property));
                            //                //Stranded Deep LOD Mod : grass shader property _Cull
                            //                //Stranded Deep LOD Mod : grass shader property _Mode
                            //                //Stranded Deep LOD Mod : grass shader property _DitherMode
                            //                //Stranded Deep LOD Mod : grass shader property _DitherDistance
                            //                //Stranded Deep LOD Mod : grass shader property _DitherWidth
                            //                //Stranded Deep LOD Mod : grass shader property _PackedNormals
                            //                //Stranded Deep LOD Mod : grass shader property _InvertedNormals
                            //                //Stranded Deep LOD Mod : grass shader property _Translucent
                            //                //Stranded Deep LOD Mod : grass shader property _Translucency
                            //            }
                            //        }
                            //    }
                            //}

                            if (StoreHandled && _handledElements.Contains(element))
                                continue;

                            // do stuff
                            HandleOne(element);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Stranded Deep " + ModName + " : Handle failed for " + element.ToString() + " : " + e);
                        }
                        finally
                        {
                            _handledElements.Add(element);
                            currentIndex--;
                        }
                    }

                    if (currentIndex < 0)
                        ListToHandle = null;
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep " + ModName + " : Handle failed (1) " + e);
                }
                finally
                {
                    sw.Stop();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep " + ModName + " : Handle failed (2) " + e);
            }
        }

        protected virtual void HandleOne(T toHandle)
        {

        }

        public virtual void Reset()
        {
            _handledElements.Clear();
        }
    }
}