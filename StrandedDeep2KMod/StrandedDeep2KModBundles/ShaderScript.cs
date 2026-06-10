using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    public class ShaderScript : MonoBehaviour
    {
        void Start()
        {
            try
            {
                MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in mrs)
                {
                    Shader shader = Shader.Find("Standard");
                    if (shader == null)
                    {
                        CustomLogger.Log("Stranded Deep 2K Mod : ShaderScript Standard shader is NULL", Main.debugLog);
                        return;
                    }
                    mr.sharedMaterial.shader = Shader.Find("Standard");
                    mr.material.shader = Shader.Find("Standard");
                    foreach (Material m in mr.sharedMaterials)
                    {
                        m.shader = Shader.Find("Standard");
                    }
                    foreach (Material m in mr.materials)
                    {
                        m.shader = Shader.Find("Standard");
                    }
                }
                CustomLogger.Log("Stranded Deep 2K Mod : ShaderScript shader updated for " + gameObject.name, Main.debugLog);
            }
            catch (Exception e)
            {
                CustomLogger.Log("Stranded Deep 2K Mod : error on ShaderScript Start : " + e);
            }
        }

        void Update()
        {
        }
    }
}
