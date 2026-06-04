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
    class GrassBender : BenderBase
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
        // OPT: ondulation supprimé — alloué dans RecomputeDeltas mais jamais lu
        //      dans DeformingUpdate (bloc commenté)
        //Vector3[] ondulation;

        public int BendAngle { get; set; }
        //public int MinBendSpeed { get; set; }
        //public int MaxBendSpeed { get; set; }

        System.Random random = new System.Random();

        protected bool initOk = false;

        // OPT: limite de retries pour éviter d'appeler DeformingInit() à chaque frame en cas d'échec
        private int _initFailCount = 0;
        private const int MaxInitRetries = 5;

        public void Start()
        {
            try
            {
                // FIX: Start() était vide — meshFilter n'était jamais assigné, donc
                //      DeformingInit() retournait immédiatement à chaque frame sans jamais
                //      initialiser le mesh. On cherche le MeshFilter ici au démarrage.
                meshFilter = GetComponentInChildren<MeshFilter>();
                renderer = meshFilter != null ? meshFilter.gameObject.GetComponent<Renderer>() : GetComponentInChildren<Renderer>();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage Mod error in GrassBender Start : " + e);
            }
        }

        // FIX: InitSimpleBending() n'était jamais appelée (Start() était vide, Update() ne l'appelait pas)
        //      Conservée en commentaire pour référence — à réactiver si le bending simple est souhaité
        //private void InitSimpleBending()
        //{
        //    //Debug.Log("Stranded Deep AnimatedFoliage mod InitSimpleBending");
        //    fromAngle = new Vector3(
        //        (float)random.NextDouble() * random.Next(BendAngle * (-1 - (int)(Main.stormPercentage / 100f)), BendAngle * (1 + (int)(Main.stormPercentage / 100f))),
        //        0f,
        //        (float)random.NextDouble() * random.Next(-BendAngle - (int)(Main.stormPercentage * BendAngle / 100f), BendAngle * (1 + (int)(Main.stormPercentage / 100f))));
        //    toAngle = new Vector3(
        //        (float)random.NextDouble() * random.Next(BendAngle * (-1 - (int)(Main.stormPercentage / 100f)), BendAngle * (1 + (int)(Main.stormPercentage / 100f))),
        //        0f,
        //        (float)random.NextDouble() * random.Next(-BendAngle - (int)(Main.stormPercentage * BendAngle / 100f), BendAngle * (1 + (int)(Main.stormPercentage / 100f))));
        //
        //    float yScale = (float)random.Next(50, 110) / 100f;
        //    fromScale = new Vector3((float)random.Next(98, 102) / 100f, yScale, (float)random.Next(98, 102) / 100f);
        //    toScale = new Vector3((float)random.Next(98, 102) / 100f, yScale, (float)random.Next(98, 102) / 100f);
        //
        //    //RotationSpeed = (float)random.Next(MinBendSpeed * (1 + (int)(Main.stormPercentage / 100f)), MaxBendSpeed * (1 + (int)(Main.stormPercentage / 100f))) / (float)100;
        //}

        public void Update()
        {
            try
            {
                if (!DoChecks())
                    return;

                // FIX: UpdateSimpleBending() n'était jamais appelée non plus — même sort qu'InitSimpleBending
                //      à réactiver ici si besoin :
                //UpdateSimpleBending();

                if (!initOk)
                {
                    // OPT: limite les retries pour ne pas saturer le CPU en cas d'échec persistant
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
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GrassBender update : " + e);
            }
        }

        protected override bool IsUnderwaterObject()
        {
            return false;
        }

        // FIX: distance réduite pour l'herbe — le deforming mesh est coûteux et l'herbe
        //      n'est visible de près que. On limite à distanceRatio * 20f au lieu de 200f.
        protected override bool CheckDistance()
        {
            try
            {
                if (PlayerRegistry.LocalPlayer == null)
                    return false;

                float sqrMagnitude = (this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position).sqrMagnitude;
                // changer ici la distance d'animation de l'herbe
                float maxDist = Main.distanceRatio * 50f;
                if (sqrMagnitude > maxDist * maxDist)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GrassBender CheckDistance : " + e);
            }
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

        // FIX: UpdateSimpleBending() n'était jamais appelée — conservée en commentaire pour référence
        //private void UpdateSimpleBending()
        //{
        //    if (previousstreamSpeed != targetstreamSpeed)
        //    {
        //        streamt += 0.001f * Time.deltaTime;
        //        streamSpeed = Mathf.Lerp(previousstreamSpeed, targetstreamSpeed, streamt);
        //        if (Mathf.Approximately(streamSpeed, targetstreamSpeed))
        //        {
        //            previousstreamSpeed = targetstreamSpeed;
        //            streamt = 0.0f;
        //        }
        //    }
        //    else
        //    {
        //        targetstreamSpeed = 1.0f + (1.0f * (float)Main.stormPercentage / 100f) + (float)random.NextDouble();
        //    }
        //    float phase = 0.2f;
        //    float t = (Mathf.Sin(streamSpeed * Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 4.0f;
        //    transform.eulerAngles = Vector3.Lerp(fromAngle, toAngle, t);
        //}

        private void DeformingInit()
        {
            try
            {
                if (meshFilter == null)
                    return;

                InitClonedMesh();
                RecomputeDeltas();

                initOk = true;

                //Debug.Log("Stranded Deep AnimatedFoliage mod GrassBender vertex count = " + originalVertices.Length);
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on GrassBender DeformingInit : " + e);
            }
        }

        private void RecomputeDeltas()
        {
            float oscillationPhase = 0.05f;
            float oscillationDelta = 0.15f; // FIX: réduit de 0.5 à 0.15 — amplitude trop importante
            delta = new Vector3[originalVertices.Length];
            // OPT: ondulation supprimé — jamais lu dans DeformingUpdate (bloc commenté)
            //ondulation = new Vector3[originalVertices.Length];

            // OPT: Time.time mis en cache — remplace DateTime.Now.Second par vertex
            float sinPhase = Mathf.Sin(Time.time * oscillationPhase * Mathf.PI * 2.0f);

            // FIX: ratio basé sur la hauteur Y du vertex normalisée par la hauteur max de l'objet —
            //      la formule héritée des arbres utilisait la distance XZ au centre (éloignement horizontal),
            //      ce qui n'a pas de sens pour de l'herbe. Ici on veut que les sommets oscillent beaucoup
            //      et la base reste ancrée : ratio = 0 en bas, ratio = 1 en haut.
            float maxY = 0f;
            for (int i = 0; i < originalVertices.Length; i++)
                if (originalVertices[i].y > maxY) maxY = originalVertices[i].y;
            if (maxY <= 0f) maxY = 1f; // guard division par zéro si tous les vertices sont à y=0

            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                // move more if far from center
                //float sqrMag = (originalVertices[i] - new Vector3(0, originalVertices[i].y, 0)).sqrMagnitude;
                //Debug.Log("Stranded Deep AnimatedFoliage mod GrassBender vertex sqrmag = " + sqrMag);
                // FIX: ratio = hauteur normalisée — base immobile (y=0 → ratio=0), sommet max amplitude (y=maxY → ratio=1)
                float ratio = Mathf.Max(0f, originalVertices[i].y) / maxY;
                //float ratio = 0.2f;//sqrMag / 200f;
                //Debug.Log("Stranded Deep AnimatedFoliage mod GrassBender vertex ratio = " + ratio);
                delta[i] = new Vector3(
                    (float)random.NextDouble() * sinPhase,
                    (float)random.NextDouble() * sinPhase,
                    (float)random.NextDouble() * sinPhase) * ratio * oscillationDelta;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex delta = " + delta[i]);

                // OPT: ondulation supprimé
                //ondulation[i] = new Vector3((float)Math.Sin(originalVertices[i].y), 0, (float)Math.Cos(originalVertices[i].y));
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
                // OPT: stormRatio mis en cache une fois par frame au lieu de recalculer par vertex
                float stormRatio = (float)Main.stormPercentage / 100f;

                float t = 0.0f;// (Mathf.Sin(Time.time * 0.2f * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
                int startIndex = 0; //random.Next(0, displacedVertices.Length - 2);
                int endIndex = displacedVertices.Length;//random.Next(startIndex, displacedVertices.Length);
                for (int vertex = startIndex; vertex < endIndex; vertex++)
                {
                    // OPT: évite d'allouer un new Vector3 juste pour annuler le composant Y
                    float vx = originalVertices[vertex].x;
                    float vz = originalVertices[vertex].z;
                    float phase = 0.2f * Mathf.Sqrt(vx * vx + vz * vz) * (1 + stormRatio);
                    t = (Mathf.Sin(Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

                    //Vector3 randomOsc = new Vector3((float)random.Next(0, 5) * delta[vertex].x, (float)random.Next(0, 5) * delta[vertex].y, (float)random.Next(0, 5) * delta[vertex].z);
                    Vector3 randomOsc = delta[vertex] * (1 + stormRatio);
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
