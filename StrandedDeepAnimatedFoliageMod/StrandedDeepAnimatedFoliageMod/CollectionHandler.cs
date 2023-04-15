using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public class CollectionHandler<T>
    {
        T[] ListToHandle = null;
        List<T> _handledElements = new List<T>();
        int lastPassCount = 0;
        int currentIndex = -1;
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public int MaxMilliseconds { get; set; }

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
                Debug.Log("Stranded Deep AnimatedFoliage Mod : FillQueue failed : " + e);
            }
        }

        public void Handle()
        {
            try
            {
                // PiscusManager / _spawnDistance


                ////Debug.Log("Stranded Deep LOD Mod : increasing fish drawing distance");
                //// float
                //FishRendererBase[] frbs = Beam.Game.FindObjectsOfType<FishRendererBase>();
                //foreach (FishRendererBase frb in frbs)
                //{
                //    if (_handledFishRenderers.Contains(frb))
                //        continue;

                //    //Debug.Log("Stranded Deep LOD Mod : increasing fish drawing distance to 1000 for " + frb.GetType().Name);
                //    fi.SetValue(frb, 1000f);
                //    _handledFishRenderers.Add(frb);
                //}

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
                            if (StoreHandled && _handledElements.Contains(element))
                                continue;

                            // do stuff
                            HandleOne(element);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Stranded Deep AnimatedFoliage Mod : Handle failed for " + element.ToString() + " : " + e);
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
                    Debug.Log("Stranded Deep AnimatedFoliage Mod : Handle failed (1) " + e);
                }
                finally
                {
                    sw.Stop();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod : Handle failed (2) " + e);
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
