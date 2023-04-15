using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepRealStormsMod
{
    static partial class Main
    {
        static Mesh originalMesh;
        static Mesh clonedMesh;
        static MeshFilter meshFilter;
        //static int[] triangles;
        static Vector3[] originalVertices, displacedVertices;

        public static int MinOscillationSpeed { get; set; }
        public static int MaxOscillationSpeed { get; set; }

        static System.Random random = new System.Random();
        static Vector3[] delta;

        private static void DeformingPalmTopInit(SaveablePrefab prefab)
        {
            if (meshFilter != null)
                return;

            MeshFilter[] meshFilters = prefab.gameObject.GetComponentsInChildren<MeshFilter>();
            //int mesh = 0;
            foreach (MeshFilter mf in meshFilters)
            {
                // FICUS_2 - Trunk
                // Palm_Top_LOD0
                // Palm_Top_LOD1

                //Debug.Log("Stranded Deep RealStorms mod TreeBender submeshfilter : " + mesh + " / " + (mf == null ? "null" : "not null"));
                //Debug.Log("Stranded Deep RealStorms mod TreeBender submeshfilter go name : " + mf.gameObject.name);
                //Debug.Log("Stranded Deep RealStorms mod TreeBender submeshfilter shared mesh : " + mesh + " / " + (mf.sharedMesh == null ? "null" : "not null"));
                //Debug.Log("Stranded Deep RealStorms mod TreeBender submeshfilter shared mesh vertex count : " + mesh + " / " + (mf.sharedMesh == null ? "null" : mf.sharedMesh.vertices.Length.ToString()));
                //Debug.Log("Stranded Deep RealStorms mod TreeBender submeshfilter mesh : " + mesh + " / " + (mf.mesh == null ? "null" : "not null"));

                if (mf.gameObject.name == "Palm_Top_LOD0")
                {
                    meshFilter = mf;
                }
                //mesh++;
            }

            if (meshFilter == null)
                return;

            //originalMesh = meshFilter.mesh;
            originalMesh = meshFilter.sharedMesh;

            clonedMesh = new Mesh(); //2

            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            //meshFilter.mesh = clonedMesh;
            meshFilter.sharedMesh = clonedMesh;

            originalVertices = originalMesh.vertices;
            displacedVertices = new Vector3[originalVertices.Length];

            float oscillationPhase = 0.1f;
            float oscillationDelta = 4.0f;
            delta = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                // move more if far from center
                float sqrMag = (originalVertices[i] - new Vector3(0, originalVertices[i].y, 0)).sqrMagnitude;
                Debug.Log("Stranded Deep RealStorms mod TreeBender vertex sqrmag = " + sqrMag);
                float ratio = Mathf.Pow(sqrMag, 2) / 200f;
                Debug.Log("Stranded Deep RealStorms mod TreeBender vertex ratio = " + ratio);
                delta[i] = new Vector3((float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f)) * ratio * oscillationDelta;
                Debug.Log("Stranded Deep RealStorms mod TreeBender vertex delta = " + delta[i]);
            }

            //vertexVelocities = new Vector3[originalVertices.Length];
            Debug.Log("Stranded Deep RealStorms mod TreeBender vertex count = " + originalVertices.Length);
        }

        private static void DeformingPalmTopUpdate()
        {
            if (meshFilter == null)
                return;

            //AddForceToVertex(1, new Vector3(0.5f, 0.5f, 0), (Mathf.Sin(Time.time * RotationSpeed * Mathf.PI * 2.0f) + 1.0f) / 2.0f);
            //Vector3 delta = new Vector3(0.1f * Mathf.Sin(DateTime.Now.Millisecond * RotationSpeed * Mathf.PI * 2.0f), 0.1f * Mathf.Sin(DateTime.Now.Millisecond * RotationSpeed * Mathf.PI * 2.0f), 0.1f * Mathf.Sin(DateTime.Now.Millisecond * RotationSpeed * Mathf.PI * 2.0f));
            //displacedVertices[1] = originalVertices[1] + delta;

            //float t = (Mathf.Sin(Time.time * (float)random.NextDouble() * random.Next(MinOscillationSpeed, MaxOscillationSpeed) * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
            //float t = (Mathf.Sin(Time.time * 1 * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
            //float t = (Mathf.Sin(Time.time * 100 * Mathf.PI * 2.0f) + 1.0f);
            Debug.Log("Stranded Deep RealStorms mod update time " + Time.time);
            float t = (Mathf.Sin(Time.time * 0.2f * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

                    int startIndex = 0; //random.Next(0, displacedVertices.Length - 2);
            int endIndex = displacedVertices.Length;//random.Next(startIndex, displacedVertices.Length);
            for (int vertex = startIndex; vertex < endIndex; vertex++)
            {
                //Vector3 randomOsc = new Vector3((float)random.Next(0, 5) * delta[vertex].x, (float)random.Next(0, 5) * delta[vertex].y, (float)random.Next(0, 5) * delta[vertex].z);
                Vector3 randomOsc = delta[vertex];
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

        static void DeformingPalmTopReset()
        {
            originalMesh = null;
            clonedMesh = null;
            meshFilter = null;
        }
    }
}
