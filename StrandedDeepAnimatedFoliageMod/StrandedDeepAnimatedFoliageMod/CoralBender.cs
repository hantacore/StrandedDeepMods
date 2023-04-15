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
    public class CoralBender : BenderBase
    {
        //public float RotationSpeed = 0.2f;
        public Vector3 fromAngle = new Vector3(0f, 0f, -5f);
        public Vector3 toAngle = new Vector3(0f, 0f, 5f);

        public Vector3 fromScale = new Vector3(1f, 1f, 1f);
        public Vector3 toScale = new Vector3(1f, 1f, 1f);

        Mesh originalMesh;
        Mesh clonedMesh;
        Vector3[] originalVertices, displacedVertices;
        Vector3[] delta;
        Vector3[] ondulation;

        public int BendAngle { get; set; }
        //public int MinBendSpeed { get; set; }
        //public int MaxBendSpeed { get; set; }

        System.Random random = new System.Random();

        bool IsKelp1 { get; set; }
        bool IsKelp2 { get; set; }

        protected bool initOk = false;

        public void Start()
        {
            try
            {
                if (IsKelp1 || IsKelp2)
                {
                    BendAngle = 5;
                    //MinBendSpeed = 1;
                    //MaxBendSpeed = 5;

                    InitSimpleBending();
                }
                //DeformingInit();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod error in CoralBender Start : " + e);
            }
        }

        private void InitSimpleBending()
        {
            //Debug.Log("Stranded Deep AnimatedFoliage mod InitSimpleBending");
            fromAngle = new Vector3(
                (float)random.NextDouble() * random.Next(BendAngle * (-1 - (int)(Main.stormPercentage / 100f)), BendAngle * (1 + (int)(Main.stormPercentage / 100f))),
                0f,
                (float)random.NextDouble() * random.Next(-BendAngle - (int)(Main.stormPercentage * BendAngle / 100f), BendAngle * (1 + (int)(Main.stormPercentage / 100f))));
            toAngle = new Vector3(
                (float)random.NextDouble() * random.Next(BendAngle * (-1 - (int)(Main.stormPercentage / 100f)), BendAngle * (1 + (int)(Main.stormPercentage / 100f))),
                0f,
                (float)random.NextDouble() * random.Next(-BendAngle - (int)(Main.stormPercentage * BendAngle / 100f), BendAngle * (1 + (int)(Main.stormPercentage / 100f))));

            float yScale = (float)random.Next(50, 110) / 100f;
            fromScale = new Vector3((float)random.Next(98, 102) / 100f, yScale, (float)random.Next(98, 102) / 100f);
            toScale = new Vector3((float)random.Next(98, 102) / 100f, yScale, (float)random.Next(98, 102) / 100f);

            //RotationSpeed = (float)random.Next(MinBendSpeed * (1 + (int)(Main.stormPercentage / 100f)), MaxBendSpeed * (1 + (int)(Main.stormPercentage / 100f))) / (float)100;
        }

        public void Update()
        {
            try
            {
                if (!DoChecks())
                    return;

                if (IsKelp1 || IsKelp2)
                {
                    UpdateSimpleBending();
                }

                if (!initOk)
                {
                    DeformingInit();
                }
                else
                {
                    DeformingUpdate();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on CoralBender update : " + e);
            }
        }

        protected override bool IsUnderwaterObject()
        {
            return true;
        }

        //protected virtual bool CheckDistance()
        //{
        //    try
        //    {
        //        if (PlayerRegistry.LocalPlayer == null)
        //            return false;

        //        float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
        //        //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
        //        if (magnitude > 20f)
        //            return false;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender CheckDistance : " + e);
        //    }
        //    return true;
        //}

        //protected virtual bool CheckVisible()
        //{
        //    if (GetComponent<Renderer>().isVisible)
        //        return true;
        //    else
        //        return false;
        //}

        float streamt = 0.0f;
        float streamSpeed = 1.0f;
        float previousstreamSpeed = 1.0f;
        float targetstreamSpeed = 1.0f;

        private void UpdateSimpleBending()
        {
            if (previousstreamSpeed != targetstreamSpeed)
            {
                streamt += 0.001f * Time.deltaTime;
                streamSpeed = Mathf.Lerp(previousstreamSpeed, targetstreamSpeed, streamt);
                if (Mathf.Approximately(streamSpeed, targetstreamSpeed))
                {
                    previousstreamSpeed = targetstreamSpeed;
                    streamt = 0.0f;
                }
            }
            else
            {
                targetstreamSpeed = 1.0f + (1.0f * (float)Main.stormPercentage / 100f) + (float)random.NextDouble();
            }
            float phase = 0.2f;
            float t = (Mathf.Sin(streamSpeed * Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 4.0f;
            if (IsKelp1)
            {
                transform.localScale = Vector3.Lerp(fromScale, toScale, t);
            }
            transform.eulerAngles = Vector3.Lerp(fromAngle, toAngle, t);
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

                    if (mf.gameObject.name.Contains("Kelp_1")
                        || mf.gameObject.name.Contains("Kelp_2")
                        || mf.gameObject.name.Contains("Coral_Group_Red") 
                        || mf.gameObject.name.Contains("Coral_Group_Pink")
                        || mf.gameObject.name.Contains("Coral_Group_White"))
                    {
                        meshFilter = mf;
                        renderer = mf.gameObject.GetComponent<Renderer>();
                        if (mf.gameObject.name.Contains("Kelp_1"))
                        {
                            IsKelp1 = true;
                        }
                        if (mf.gameObject.name.Contains("Kelp_2"))
                        {
                            IsKelp2 = true;
                        }
                    }
                }

                if (meshFilter == null)
                    return;

                InitClonedMesh();
                RecomputeDeltas();

                initOk = true;

                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex count = " + originalVertices.Length);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on CoralBender DeformingInit : " + e);
            }
        }

        private void RecomputeDeltas()
        {
            float oscillationPhase = 0.05f;
            float oscillationDelta = (IsKelp1 || IsKelp2) ? 0.8f : 0.5f;
            delta = new Vector3[originalVertices.Length];
            ondulation = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                // move more if far from center
                //float sqrMag = (originalVertices[i] - new Vector3(0, originalVertices[i].y, 0)).sqrMagnitude;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex sqrmag = " + sqrMag);
                float ratio = 0.2f;//sqrMag / 200f;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex ratio = " + ratio);
                delta[i] = new Vector3((float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f)) * ratio * oscillationDelta;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex delta = " + delta[i]);
                ondulation[i] = new Vector3((float)Math.Sin(originalVertices[i].y), 0, (float)Math.Cos(originalVertices[i].y));
            }
        }

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
#warning performance test
            meshFilter.sharedMesh.MarkDynamic();

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
                    //if (IsKelp1 || IsKelp2)
                    //{
                    //    randomOsc = delta[vertex] * (1 + (float)Main.stormPercentage / 100f) + Vector3.Lerp(-ondulation[vertex], ondulation[vertex], t);
                    //}
                    displacedVertices[vertex] = Vector3.Lerp(originalVertices[vertex] - randomOsc, originalVertices[vertex] + randomOsc, t);
                }

                //clonedMesh.vertices = displacedVertices;
#warning performance test
                clonedMesh.SetVertices(displacedVertices);
                clonedMesh.RecalculateNormals();
            }
            else
            {
                DeformingInit();
            }
        }
    }
}
