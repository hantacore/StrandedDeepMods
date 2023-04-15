using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrandedDeepExplorationMod
{
    public class ShaderScript : MonoBehaviour
    {
        void Start()
        {
            try
            {
                MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
                Shader shader = Shader.Find("Standard");
                if (shader == null)
                {
                    Debug.Log("Stranded Deep " + Main._modName + " Mod : ShaderScript Standard shader is NULL");
                    return;
                }
                mr.sharedMaterial.shader = Shader.Find("Standard");
                mr.material.shader = Shader.Find("Standard");
                Debug.Log("Stranded Deep " + Main._modName + " Mod : ShaderScript shader updated for " + gameObject.name);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep " + Main._modName + " Mod : error on ShaderScript Start : " + e);
            }
        }

        void Update()
        {
        }
    }
}
