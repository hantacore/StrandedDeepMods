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
    [RequireComponent(typeof(MeshFilter))]
    class GroundCoverBender : BenderBase
    {
        Mesh originalMesh;
        Mesh clonedMesh;
        Vector3[] originalVertices, displacedVertices;
        Vector3[] delta;

        System.Random random = new System.Random();

        protected bool initOk = false;

        private int _initFailCount = 0;
        private const int MaxInitRetries = 5;

        // Fréquences d'oscillation randomisées une seule fois au Start()
        // pour que chaque instance ait un mouvement légèrement différent
        private float _freqX = 0.1f;
        private float _freqZ = 0.1f;
        private float _freqY = 0.05f;
        private float _phaseX = 0f;
        private float _phaseZ = 0f;
        private float _phaseY = 0f;

        public void Start()
        {
            try
            {
                // Randomise les fréquences et phases une seule fois —
                // oscillation continue sans réinitialisation ni saut
                _freqX = (float)random.NextDouble() * 0.15f + 0.05f; // 0.05 à 0.20 Hz
                _freqZ = (float)random.NextDouble() * 0.15f + 0.05f;
                _freqY = (float)random.NextDouble() * 0.08f + 0.02f; // plus lent en Y, soulèvement subtil
                _phaseX = (float)random.NextDouble() * Mathf.PI * 2f;
                _phaseZ = (float)random.NextDouble() * Mathf.PI * 2f;
                _phaseY = (float)random.NextDouble() * Mathf.PI * 2f;

                // FIX: cible explicitement le child LOD0 — le MeshFilter racine a sharedMesh = null,
                //      seul ground_cover_X_LOD0 contient le mesh à animer
                MeshFilter[] allMf = GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter mf in allMf)
                {
                    if (mf.gameObject.name.Contains("LOD0"))
                    {
                        meshFilter = mf;
                        renderer = mf.gameObject.GetComponent<Renderer>();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GroundCoverBender Start : " + e);
            }
        }

        public void Update()
        {
            try
            {
                if (!DoChecks())
                    return;

                if (!initOk)
                {
                    if (_initFailCount < MaxInitRetries)
                    {
                        DeformingInit();
                        if (!initOk) _initFailCount++;
                    }
                }
                else
                {
                    DeformingUpdate();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GroundCoverBender Update : " + e);
            }
        }

        protected override bool IsUnderwaterObject()
        {
            return false;
        }

        protected override bool CheckDistance()
        {
            try
            {
                if (PlayerRegistry.LocalPlayer == null)
                    return false;

                float sqrMagnitude = (this.gameObject.transform.position
                    - PlayerRegistry.LocalPlayer.transform.position).sqrMagnitude;
                float maxDist = Main.distanceRatio * 15f;
                if (sqrMagnitude > maxDist * maxDist)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GroundCoverBender CheckDistance : " + e);
            }
            return true;
        }

        private void DeformingInit()
        {
            try
            {
                if (meshFilter == null)
                    return;

                InitClonedMesh();
                RecomputeDeltas();

                initOk = true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GroundCoverBender DeformingInit : " + e);
            }
        }

        private void RecomputeDeltas()
        {
            float oscillationDelta = 0.5f;
            float oscillationDeltaY = 0.4f;

            delta = new Vector3[originalVertices.Length];

            float maxY = 0f;
            float minY = float.MaxValue;
            for (int i = 0; i < originalVertices.Length; i++)
            {
                if (originalVertices[i].y > maxY) maxY = originalVertices[i].y;
                if (originalVertices[i].y < minY) minY = originalVertices[i].y;
            }

            if (Main.debugMode)
                Debug.Log("GroundCoverBender : minY=" + minY + " maxY=" + maxY
                    + " vertexCount=" + originalVertices.Length);

            if (maxY <= 0f) maxY = 1f;

            float sinPhase = Mathf.Sin(Time.time * 0.05f * Mathf.PI * 2.0f);

            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                float ratio = Mathf.Max(0f, originalVertices[i].y) / maxY;

                delta[i] = new Vector3(
                    (float)random.NextDouble() * sinPhase * ratio * oscillationDelta,
                    (float)random.NextDouble() * Mathf.Abs(sinPhase) * ratio * oscillationDeltaY,
                    (float)random.NextDouble() * sinPhase * ratio * oscillationDelta);
            }
        }

        private void InitClonedMesh()
        {
            // FIX: guard null sur sharedMesh — peut être null si FollowSpawn
            //      n'a pas encore assigné le mesh au moment de l'init
            if (meshFilter.sharedMesh == null)
                throw new Exception("sharedMesh is null — mesh not yet loaded");

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
            if (meshFilter == null)
            {
                if (_initFailCount < MaxInitRetries)
                {
                    DeformingInit();
                    if (!initOk) _initFailCount++;
                }
                return;
            }

            float stormRatio = Main.stormPercentage / 100f;

            // Sinusoïdes XZ — oscillation latérale
            float tX = Mathf.Sin(Time.time * _freqX + _phaseX);
            float tZ = Mathf.Sin(Time.time * _freqZ + _phaseZ);

            // FIX: Y plus chaotique — combinaison de deux sinusoïdes à fréquences
            //      différentes (fondamentale + harmonique décalée) pour casser la régularité.
            //      Abs() pour que le mouvement reste vers le haut uniquement.
            float tY = Mathf.Abs(
                Mathf.Sin(Time.time * _freqY + _phaseY) * 0.6f +
                Mathf.Sin(Time.time * _freqY * 2.3f + _phaseY * 1.7f) * 0.4f);

            // L'amplitude augmente légèrement avec la tempête mais reste toujours subtile
            float amplitudeMultiplier = 1f + stormRatio * 0.8f;

            for (int vertex = 0; vertex < displacedVertices.Length; vertex++)
            {
                Vector3 d = delta[vertex] * amplitudeMultiplier;

                displacedVertices[vertex] = new Vector3(
                    originalVertices[vertex].x + d.x * tX,
                    originalVertices[vertex].y + d.y * tY,
                    originalVertices[vertex].z + d.z * tZ);
            }

#warning performance test
            clonedMesh.SetVertices(displacedVertices);
            clonedMesh.RecalculateNormals();
        }
    }
}