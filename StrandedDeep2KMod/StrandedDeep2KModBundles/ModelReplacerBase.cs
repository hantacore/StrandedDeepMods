using Beam;
using Funlabs;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    public class ModelReplacerBase : MonoBehaviour
    {
        private bool handled = false;
        private FastRandom fr = new FastRandom(StrandedWorld.WORLD_SEED);

        public void Update()
        {
            try
            {
                if (handled)
                    return;

                CustomLogger.Log("Stranded Deep 2K Mod : ModelReplacerBase " + gameObject.name.ToLower());
                if (Main.replaceBushes && Main.newBush3Prefab != null && gameObject.name.ToLower().Contains("bush") && gameObject.transform.rotation != new Quaternion(0,0,0,1))
                {
                    //Zone zone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                    //if (zone != null && zone.Loaded)
                    //{
                        CustomLogger.Log("Stranded Deep 2K Mod : replacing bush " + gameObject.transform.position + " / " + gameObject.transform.rotation);

                        //GameObject go = Main.InstantiatePrefab(Main.newBushPrefab, gameObject.GetComponent<SaveablePrefab>().transform.position, gameObject.GetComponent<SaveablePrefab>().transform.rotation, 5.0f);
                        //go.transform.SetParent(gameObject.transform.parent);

                        //GameObject go3 = Main.InstantiatePrefab(Main.newBush2Prefab, gameObject.GetComponent<SaveablePrefab>().transform.position, gameObject.GetComponent<SaveablePrefab>().transform.rotation, 100.0f);
                        //go3.transform.SetParent(gameObject.transform.parent);

                        Vector3 offset = new Vector3();//new Vector3(fr.Next(-5, 5), 0, fr.Next(-5, 5));

                        Quaternion rotation = gameObject.GetComponent<SaveablePrefab>().transform.rotation;
                        rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + fr.Next(0, 359), rotation.eulerAngles.z);
                        GameObject go2 = Main.InstantiatePrefab(Main.newBush3Prefab, gameObject.GetComponent<SaveablePrefab>().transform.position + offset, rotation, 1.0f);
                        go2.transform.SetParent(gameObject.transform.parent);

                    gameObject.SetActive(false);
                    //}
                    handled = true;
                }
                if (Main.replaceFicus && Main.oldTreePrefab != null && gameObject.name.ToLower().Contains("ficus") && gameObject.transform.rotation != new Quaternion(0, 0, 0, 1))
                {
                    //Zone zone = StrandedWorld.GetZone(PlayerRegistry.LocalPlayer.transform.position, false);
                    //if (zone != null && zone.Loaded)
                    //{
                    CustomLogger.Log("Stranded Deep 2K Mod : replacing ficus " + gameObject.transform.position + " / " + gameObject.transform.rotation);

                    //GameObject go = Main.InstantiatePrefab(Main.mangrove1Prefab, gameObject.GetComponent<SaveablePrefab>().transform.position, gameObject.GetComponent<SaveablePrefab>().transform.rotation, 1.0f);
                    Quaternion rotation = gameObject.GetComponent<SaveablePrefab>().transform.rotation;
                    rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + fr.Next(0, 359), rotation.eulerAngles.z);
                    GameObject go = Main.InstantiatePrefab(Main.oldTreePrefab, gameObject.GetComponent<SaveablePrefab>().transform.position, rotation, 5.0f);
                    go.transform.SetParent(gameObject.transform.parent);
                    gameObject.SetActive(false);
                    //}
                    handled = true;
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod error in ModelReplacerBase.Update " + e);
            }
        }
    }
}
