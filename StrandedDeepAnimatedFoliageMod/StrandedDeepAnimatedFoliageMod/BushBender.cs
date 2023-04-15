using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace StrandedDeepAnimatedFoliageMod
{
    public class BushBender : BenderBase
    {
        public float RotationSpeed = 0.2f;
        public Vector3 from = new Vector3(0f, 0f, -5f);
        public Vector3 to = new Vector3(0f, 0f, 5f);

        Mesh originalMesh;
        Mesh clonedMesh;
        Vector3[] originalVertices, displacedVertices;

        System.Random random = new System.Random();
        Vector3[] delta;
        //float previousStormPercentage = 0;
        //float stormPercentage = 0;

        public void Start()
        {
            try
            {
                //InitSimpleBending();
                DeformingInit();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod error in BushBender Start : " + e);
            }
        }

        private void InitSimpleBending()
        {
            //Debug.Log("Stranded Deep AnimatedFoliage mod InitSimpleBending");

            from = new Vector3((float)random.Next(98, 102) / 100f, 1.0f, (float)random.Next(98, 102) / 100f);
            to = new Vector3((float)random.Next(98, 102) / 100f, 1.0f, (float)random.Next(98, 102) / 100f);
            RotationSpeed = (float)random.Next(5, 20) / (float)100;
        }

        public void Update()
        {
            try
            {
                if (!DoChecks())
                    return;

                //if (AtmosphereStorm.Instance.CurrentWeatherEvent != null)
                //{
                //    t += 0.001f * Time.deltaTime;
                //    stormPercentage = Mathf.Lerp(previousStormPercentage, AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage, t);
                //    if (Mathf.Approximately(stormPercentage, AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage))
                //    {
                //        previousStormPercentage = AtmosphereStorm.Instance.CurrentWeatherEvent.Percentage;
                //        t = 0.0f;
                //    }
                //}
                //else
                //{
                //    t += 0.5f * Time.deltaTime;
                //    stormPercentage = Mathf.Lerp(previousStormPercentage, 0, t);
                //    if (Mathf.Approximately(stormPercentage, 0))
                //    {
                //        previousStormPercentage = 0;
                //        t = 0.001f;
                //    }
                //}
                //Debug.Log("Stranded Deep AnimatedFoliage mod BushBender stormpercentage = " + stormPercentage);

                //UpdateSimpleBending();
                DeformingUpdate();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on BushBender update : " + e);
            }
        }

        private void UpdateSimpleBending()
        {
            float t = (Mathf.Sin(Time.time * RotationSpeed * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
            transform.localScale = Vector3.Lerp(from, to, t);
        }

        private void DeformingInit()
        {
            try
            {
                //Debug.Log("Stranded Deep AnimatedFoliage mod DeformingInit");

                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mf in meshFilters)
                {
                    //int mesh = 0;
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter : " + mesh + " / " + (mf == null ? "null" : "not null"));
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter shared mesh : " + (mf.sharedMesh == null ? "null" : "not null"));
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter go name : " + mf.gameObject.name);
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter shared mesh vertex count : " + mesh + " / " + (mf.sharedMesh == null ? "null" : mf.sharedMesh.vertices.Length.ToString()));
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter mesh : " + mesh + " / " + (mf.mesh == null ? "null" : "not null"));

                    if (mf.gameObject.name == "LOD0")
                    {
                        meshFilter = mf;
                        renderer = mf.gameObject.GetComponent<Renderer>();
                    }
                }

                if (meshFilter == null)
                    return;

                InitClonedMesh();

                RecomputeDeltas();

                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex count = " + originalVertices.Length);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on BushBender DeformingInit : " + e);
            }
        }

        private void RecomputeDeltas()
        {
            float oscillationPhase = 0.1f;
            float oscillationDelta = 0.5f;
            delta = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                // move more if far from center
                float sqrMag = (originalVertices[i] - new Vector3(0, originalVertices[i].y, 0)).sqrMagnitude;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex sqrmag = " + sqrMag);
                float ratio = sqrMag / 400f;//Mathf.Pow(sqrMag, 2) / 200f;
                                            //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex ratio = " + ratio);
                delta[i] = new Vector3((float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f)) * ratio * oscillationDelta;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex delta = " + delta[i]);
            }
        }

        //public Mesh Copy(Mesh mesh)
        //{
        //    var copy = new Mesh();
        //    foreach (var property in typeof(Mesh).GetProperties())
        //    {
        //        if (property.GetSetMethod() != null && property.GetGetMethod() != null)
        //        {
        //            property.SetValue(copy, property.GetValue(mesh, null), null);
        //        }
        //    }
        //    return copy;
        //}

        private void InitClonedMesh()
        {
            //obj.AddComponent<MeshFilter>().sharedMesh = Instantiate(meshFilter.sharedMesh);

            originalMesh = meshFilter.sharedMesh;

            clonedMesh = new Mesh();

            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            clonedMesh.uv2 = originalMesh.uv2;
            clonedMesh.uv3 = originalMesh.uv3;
            clonedMesh.uv4 = originalMesh.uv4;
            clonedMesh.uv5 = originalMesh.uv5;
            clonedMesh.uv6 = originalMesh.uv6;
            clonedMesh.uv7 = originalMesh.uv7;
            clonedMesh.uv8 = originalMesh.uv8;

            //Debug.Log("Stranded Deep AnimatedFoliage mod subMeshCount " + originalMesh.subMeshCount);
            clonedMesh.subMeshCount = originalMesh.subMeshCount;
            for (int i = 0; i < originalMesh.subMeshCount; i++)
            {
                SubMeshDescriptor smd = originalMesh.GetSubMesh(i);
                SubMeshDescriptor newSmd = new SubMeshDescriptor(smd.indexStart, smd.indexCount, smd.topology);
                newSmd.baseVertex = smd.baseVertex;
                newSmd.firstVertex = smd.firstVertex;
                newSmd.bounds = smd.bounds;
                newSmd.vertexCount = smd.vertexCount;
                clonedMesh.SetSubMesh(i, newSmd, MeshUpdateFlags.Default);
            }
            clonedMesh.colors = originalMesh.colors;
            clonedMesh.tangents = originalMesh.tangents;
            clonedMesh.bindposes = originalMesh.bindposes;

            meshFilter.sharedMesh = clonedMesh;

            originalVertices = originalMesh.vertices;
            displacedVertices = new Vector3[originalVertices.Length];
        }

        private void DeformingUpdate()
        {
            if (meshFilter != null)
            {
                float t = 0.0f;// (Mathf.Sin(Time.time * 0.2f * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
                int startIndex = 0; //random.Next(0, displacedVertices.Length - 2);
                int endIndex = displacedVertices.Length;//random.Next(startIndex, displacedVertices.Length);
                for (int vertex = startIndex; vertex < endIndex; vertex++)
                {
                    float phase = 0.2f * (originalVertices[vertex] - new Vector3(0, originalVertices[vertex].y, 0)).magnitude * (1 + (float)Main.stormPercentage / 100f);
                    t = (Mathf.Sin(Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

                    //Vector3 randomOsc = new Vector3((float)random.Next(0, 5) * delta[vertex].x, (float)random.Next(0, 5) * delta[vertex].y, (float)random.Next(0, 5) * delta[vertex].z);
                    Vector3 randomOsc = delta[vertex] * (1 + (float)Main.stormPercentage / 100f);
                    displacedVertices[vertex] = Vector3.Lerp(originalVertices[vertex] - randomOsc, originalVertices[vertex] + randomOsc, t);
                    //if (Mathf.Approximately(originalVertices[vertex].x, randomOsc.x)
                    //&& Mathf.Approximately(originalVertices[vertex].y, randomOsc.y)
                    //&& Mathf.Approximately(originalVertices[vertex].z, randomOsc.z))
                    //{
                    //    randomOsc = new Vector3((float)random.Next(0, 5) * delta[vertex].x, (float)random.Next(0, 5) * delta[vertex].y, (float)random.Next(0, 5) * delta[vertex].z);
                    //    delta[vertex] = randomOsc;
                    //}
                }

                clonedMesh.vertices = displacedVertices;
                clonedMesh.RecalculateNormals();
            }
            else
            {
                DeformingInit();
            }
        }

        //protected virtual bool CheckDistance()
        //{
        //    try
        //    {
        //        if (PlayerRegistry.LocalPlayer == null)
        //            return false;

        //        float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
        //        //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
        //        if (magnitude > Main.distanceRatio * 200f)
        //            return false;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender CheckDistance : " + e);
        //    }
        //    return true;
        //}
    }
}
