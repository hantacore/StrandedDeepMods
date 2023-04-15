using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public class TreeMeshFiltersHandler : CollectionHandler<MeshFilter>
    {
        internal static Dictionary<uint, bool> LeavesReplaced = new Dictionary<uint, bool>();

        public TreeMeshFiltersHandler(int maxMilliseconds = 2, bool storeHandled = false)
            : base(maxMilliseconds, storeHandled)
        {

        }

        internal void ReplaceLeaves(Renderer r)
        {
            if (r != null)
            {
                foreach (Material m in r.sharedMaterials)
                {
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender material name = " + m.name);
                    foreach (string textureName in m.GetTexturePropertyNames())
                    {
                        //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender texture name = " + textureName);

                        string key = "StrandedDeepAnimatedFoliageMod.assets.Textures." + m.name + textureName + ".png";
                        //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender key = " + key);
                        if (Main._indexedTextures.ContainsKey(key))
                        {
                            //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender key = " + key + " found and replaced");
                            m.SetTexture(textureName, Main._indexedTextures[key]);
                        }
                    }
                }
            }
        }

        internal void ReplaceBushes(Renderer r)
        {
            if (r != null)
            {
                foreach (Material m in r.sharedMaterials)
                {
                    m.SetFloat("_Mode", 2);
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    m.SetInt("_ZWrite", 1);
                    m.EnableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = 2450;

                    Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender material name = " + m.name);

                    if (m.name.Contains("Bush_Leafs_MAT"))
                    {
                        m.SetFloat("_Cutoff", 0.99f);
                    }
                    else if (m.name.Contains("Bush_Tile_MAT"))
                    {
                        m.SetFloat("_Cutoff", 0.59f);

                        m.EnableKeyword("_DETAIL_MULX2");
                        //m.SetTexture("_MainTex", Main._indexedTextures["StrandedDeep2KMod.assets.Textures.Bush_Rods_MAT.png"]);
                        //m.SetTexture("_DetailAlbedoMap", Main._indexedTextures["StrandedDeep2KMod.assets.Textures.Bush_Tile_MAT.png"]);
                    }
                    m.SetFloat("_Glossiness", 0f);
                    m.SetFloat("_GlossMapScale", 0f); // Range
                    m.SetFloat("_SmoothnessTextureChannel", 1f); //Enum(Metallic Alpha,0,Albedo Alpha,1)

                    m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    m.SetFloat("_SpecularHighlights", 0f);
                    m.SetFloat("_GlossyReflections", 0f);

                    foreach (string textureName in m.GetTexturePropertyNames())
                    {
                        //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender texture name = " + textureName);

                        string key = "StrandedDeepAnimatedFoliageMod.assets.Textures." + m.name + textureName + ".png";
                        //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender key = " + key);
                        if (Main._indexedTextures.ContainsKey(key))
                        {
                            //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender key = " + key + " found and replaced");
                            m.SetTexture(textureName, Main._indexedTextures[key]);
                        }
                    }
                }
            }
        }

        internal void ReplaceLeaves(uint prefabId, Renderer r)
        {
            if (r != null
                && !LeavesReplaced.ContainsKey(prefabId))
            {
                foreach (Material m in r.materials)
                {
                    Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender material name = " + m.name);
                    string key = "StrandedDeepAnimatedFoliageMod.assets.Textures." + m.name + ".png";
                    Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender key = " + key);
                    if (Main._indexedTextures.ContainsKey(key))
                    {
                        m.mainTexture = Main._indexedTextures[key];
                        LeavesReplaced.Add(prefabId, true);
                    }
                }
            }
        }

        protected override void HandleOne(MeshFilter toHandle)
        {
            if (toHandle == null)
                return;

            if (Main.animateCorals)
            {
                //Debug.Log("Stranded Deep AnimatedFoliage mod Main meshfilter go name : " + mfilter.gameObject.name);
                if ((toHandle.gameObject.name.Contains("Kelp")
                        || toHandle.gameObject.name.Contains("Coral_Group")
                    ) && toHandle.gameObject.GetComponent<CoralBender>() == null)
                {
                    if (Main.debugMode)
                        Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Coral Bender component to " + toHandle.gameObject.name);
                    CoralBender cb = toHandle.gameObject.AddComponent<CoralBender>();
                    return;
                }
            }

            //if (toHandle.gameObject.name.Contains("ground_cover") && toHandle.gameObject.name.Contains("LOD0"))
            //{
            //    Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Tree Bender component to " + toHandle.gameObject.name);
            //    SmallTreeBender tb = toHandle.gameObject.AddComponent<SmallTreeBender>();
            //    tb.FixedRotation = true;
            //    return;
            //}

            SaveablePrefab prefab = toHandle.GetComponentInParent<SaveablePrefab>();
            if (prefab == null)
                return;

            if (Main.animateTrees)
            {
                if ((prefab.PrefabId == 157 // palm
                    || prefab.PrefabId == 158 // palm
                    || prefab.PrefabId == 159 // palm
                    || prefab.PrefabId == 160 // palm
                    || prefab.PrefabId == 66 // ficus tree
                    || prefab.PrefabId == 67 // ficus tree
                    || prefab.PrefabId == 202 // pine
                    || prefab.PrefabId == 203 // pine
                    || prefab.PrefabId == 204 // pine
                    || Main.animateSmallTrees && (prefab.PrefabId == 47 // ficus
                        || prefab.PrefabId == 48 // ficus
                        || prefab.PrefabId == 49 // ficus
                        || prefab.PrefabId == 206 // pine small
                        || prefab.PrefabId == 207 // pine small
                        || prefab.PrefabId == 180 // young palm
                        || prefab.PrefabId == 181 // young palm
                        )
                    || Main.animatePlants && (prefab.PrefabId == 50 // alocasia
                        || prefab.PrefabId == 51 // alocasia
                        || prefab.PrefabId == 58 // alocasia
                        || prefab.PrefabId == 59 // ceriman
                        || prefab.PrefabId == 60// ceriman
                        || prefab.PrefabId == 266// aloe
                        || prefab.PrefabId == 52// banana
                        || prefab.PrefabId == 148// kura
                        || prefab.PrefabId == 149// quwawa
                        )
                    )
                    && prefab.gameObject.GetComponent<TreeBender>() == null)
                {
                    if (Main.debugMode)
                        Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Tree Bender component to " + prefab.gameObject.name);
                    TreeBender tb = null;
                    if (prefab.PrefabId == 180
                        || prefab.PrefabId == 181)
                    {
                        tb = prefab.gameObject.AddComponent<SmallTreeBender>();
                    }
                    else
                    {
                        tb = prefab.gameObject.AddComponent<TreeBender>();
                    }
                    if (prefab.PrefabId == 202
                        || prefab.PrefabId == 203
                        || prefab.PrefabId == 204)
                    {
                        tb.BendAngle = 2;
                    }
                    if (prefab.PrefabId == 157
                        || prefab.PrefabId == 158
                        || prefab.PrefabId == 159
                        || prefab.PrefabId == 160
                        || prefab.PrefabId == 180
                        || prefab.PrefabId == 181)
                    {
                        tb.IsPalm = true;

                        //if (prefab is InteractiveObject_PALM)
                        //{
                        //    InteractiveObject_PALM palm = prefab as InteractiveObject_PALM;

                        //    FieldInfo fi_fruitSpawnerPrefab = typeof(InteractiveObject_PALM).GetField("_fruitSpawnerPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
                        //    GameObject spawner = fi_fruitSpawnerPrefab.GetValue(palm) as GameObject;
                        //    if (spawner != null)
                        //    {
                        //        spawner.transform.parent = palm.gameObject.transform;


                        //        //FieldInfo fi_fruit = typeof(InteractiveObject_PALM).GetField("_fruit", BindingFlags.NonPublic | BindingFlags.Instance);
                        //        //InteractiveObject_FOOD coconut = fi_fruit.GetValue(palm) as InteractiveObject_FOOD;
                        //        //if (coconut != null)
                        //        //{
                        //        //    coconut.gameObject.transform.parent = spawner.transform;
                        //        //}

                        //        //FieldInfo fi_fruits = typeof(InteractiveObject_PALM).GetField("_fruits", BindingFlags.NonPublic | BindingFlags.Instance);
                        //        //List<InteractiveObject_FOOD> coconuts = fi_fruits.GetValue(palm) as List<InteractiveObject_FOOD>;
                        //        //if (coconuts != null)
                        //        //{
                        //        //    foreach (InteractiveObject_FOOD coco in coconuts)
                        //        //    {
                        //        //        coco.gameObject.transform.parent = spawner.transform;
                        //        //    }
                        //        //}
                        //    }
                        //}
                    }

                    if (prefab.PrefabId == 50 // alocasia
                        || prefab.PrefabId == 51 // alocasia
                        || prefab.PrefabId == 58 // alocasia
                        || prefab.PrefabId == 59 // ceriman
                        || prefab.PrefabId == 60// ceriman
                        || prefab.PrefabId == 266// aloe
                        || prefab.PrefabId == 52// banana
                        || prefab.PrefabId == 148// kura
                        || prefab.PrefabId == 149// quwawa
                        || prefab.PrefabId == 206 // pine small
                        || prefab.PrefabId == 207 // pine small
                        )
                    {
                        tb.IsSmallTree = true;
                    }
                }
            }
            if (Main.replaceTreeLeaves
                && (prefab.PrefabId == 66
                || prefab.PrefabId == 67
                || prefab.PrefabId == 47
                || prefab.PrefabId == 48
                || prefab.PrefabId == 49))
            {
                ReplaceLeaves(toHandle.GetComponentInParent<Renderer>());
            }
            if (Main.animateBushes
                && prefab.PrefabId == 205
                && prefab.gameObject.GetComponent<BushBender>() == null)
            {
                if (Main.debugMode)
                    Debug.Log("Stranded Deep AnimatedFoliage Mod : adding Bush Bender component");
                BushBender tb = prefab.gameObject.AddComponent<BushBender>();
            }
            if (Main.replaceBushTextures
                && prefab.PrefabId == 205)
            {
                ReplaceBushes(toHandle.GetComponentInParent<Renderer>());
            }
        }

        public override void Reset()
        {
            base.Reset();
            LeavesReplaced.Clear();
        }
    }
}
