using Beam;
using Beam.Serialization;
using Funlabs;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace StrandedDeepAnimatedFoliageMod
{
    //https://blog.logrocket.com/deforming-mesh-unity/

    [RequireComponent(typeof(MeshFilter))]
    public class TreeBender : BenderBase
    {
        // simple rotation bending
        public float xRotation = 90.0f;
        public float xRotation1 = 0.0f;
        public float RotationSpeed = 0.2f;
        public Vector3 from = new Vector3(0f, 0f, -5f);
        public Vector3 to = new Vector3(0f, 0f, 5f);

        protected Mesh originalMesh;
        protected Mesh clonedMesh;
        protected Vector3[] originalVertices, displacedVertices;
        protected Vector3[] delta;
        // OPT: replaced Dictionary<int,Vector3> with a plain array — O(1) index access,
        //      no boxing/hashing overhead, ContainsKey() check replaced by a bool array
        protected Vector3[] stormdelta;
        protected bool[] hasstormdelta;

        protected MeshFilter secondaryMeshFilter;
        protected Mesh secondaryOriginalMesh;
        protected Mesh secondaryClonedMesh;
        protected Vector3[] secondaryOriginalVertices, secondaryDisplacedVertices;
        protected Vector3[] secondaryDelta;

        public bool IsPalm { get; set; }
        public int BendAngle { get; set; }
        public int MinBendSpeed { get; set; }
        public int MaxBendSpeed { get; set; }
        public int MinOscillationSpeed { get; set; }
        public int MaxOscillationSpeed { get; set; }
        public bool FixedRotation { get; set; }

        protected System.Random random = new System.Random();

        protected bool initOk = false;

        // OPT: track consecutive init failures to avoid hammering DeformingInit every frame
        private int _initFailCount = 0;
        private const int MaxInitRetries = 5;

        // FIX: oscillation continue par double sinusoïde déphasée — remplace le timer + réinitialisation
        //      qui causaient des sauts visuels. Les fréquences et le déphasage sont randomisés une seule
        //      fois au Start() et ne changent plus jamais — mouvement naturel et infini sans artefacts.
        private float _freqX       = 0.2f;
        private float _freqZ       = 0.15f;
        private float _phaseOffset = 0f;

        public void Start()
        {
            try
            {
                BendAngle = 1;
                MinBendSpeed = 5;
                MaxBendSpeed = 20;

                MinOscillationSpeed = 1;
                MaxOscillationSpeed = 5;

                // FIX: randomise les fréquences et le déphasage une seule fois —
                //      aucune réinitialisation périodique ne sera nécessaire
                _freqX       = (float)random.NextDouble() * 0.3f + 0.1f;  // 0.1 à 0.4 Hz
                _freqZ       = (float)random.NextDouble() * 0.3f + 0.1f;
                _phaseOffset = (float)random.NextDouble() * Mathf.PI * 2f;
                //DeformingInit();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender Start : " + e);
            }
        }

        public void Update()
        {
            try
            {
                if (!DoChecks())
                    return;

                if (!FixedRotation)
                {
                    RotationBendUpdate();
                }

                if (!initOk)
                {
                    // OPT: limit retries so a persistent failure doesn't waste CPU every frame
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
                Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender update : " + e);
            }
        }

        #region simple bending

        private void RotationBendUpdate()
        {
            // FIX: double sinusoïde déphasée — oscillation continue et naturelle sans timer ni réinitialisation.
            //      _freqX et _freqZ légèrement différentes donnent un mouvement elliptique organique.
            //      _phaseOffset randomisé au Start() évite que tous les arbres soient en phase.
            //      BendAngle modulé par la tempête pour plus d'amplitude par mauvais temps.
            float storm  = 1f + Main.stormPercentage / 100f;
            float angleX = Mathf.Sin(Time.time * _freqX) * BendAngle * storm;
            float angleZ = Mathf.Sin(Time.time * _freqZ + _phaseOffset) * BendAngle * storm;
            transform.eulerAngles = new Vector3(angleX, 0f, angleZ);
        }

        #endregion

        //public static Texture2D DuplicateTexture(Texture2D source)
        //{
        //    RenderTexture renderTex = RenderTexture.GetTemporary(
        //                source.width,
        //                source.height,
        //                0,
        //                RenderTextureFormat.Default,
        //                RenderTextureReadWrite.Linear);

        //    Graphics.Blit(source, renderTex);
        //    RenderTexture previous = RenderTexture.active;
        //    RenderTexture.active = renderTex;
        //    Texture2D readableText = new Texture2D(source.width, source.height);
        //    readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        //    readableText.Apply();
        //    RenderTexture.active = previous;
        //    RenderTexture.ReleaseTemporary(renderTex);
        //    return readableText;
        //}

        //private static void ExportTexture(Texture2D texture, string name = null)
        //{
        //    if (texture != null)
        //    {
        //        string targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Mods/StrandedDeepAnimatedFoliageMod/TextureExport/");
        //        if (!Directory.Exists(targetDirectory))
        //        {
        //            Directory.CreateDirectory(targetDirectory);
        //        }

        //        string targetFile = Path.Combine(targetDirectory, (String.IsNullOrEmpty(name) ? texture.ToString() : name) + ".png");
        //        if (!File.Exists(targetFile))
        //        {
        //            Texture2D t = DuplicateTexture(texture);
        //            byte[] bytes = t.EncodeToPNG();
        //            File.WriteAllBytes(targetFile, bytes);
        //        }
        //    }
        //}

        private void DeformingInit()
        {
            try
            {
                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mf in meshFilters)
                {
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter : " + mesh + " / " + (mf == null ? "null" : "not null"));
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter shared mesh : " + (mf.sharedMesh == null ? "null" : "not null"));
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter go name : " + mf.gameObject.name);
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter shared mesh vertex count : " + mesh + " / " + (mf.sharedMesh == null ? "null" : mf.sharedMesh.vertices.Length.ToString()));
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender submeshfilter mesh : " + mesh + " / " + (mf.mesh == null ? "null" : "not null"));


                    // FICUS_2 - Trunk
                    // Palm_Top_LOD0
                    // Palm_Top_LOD1
                    if (mf.gameObject.name == "Palm_Top_LOD0"
                        || mf.gameObject.name == "Ficus_Tree_1_LOD0"
                        //|| mf.gameObject.name == "Ficus_Tree_1_LOD1"
                        || mf.gameObject.name == "Fixus_Tree_2_LOD0"
                        //|| mf.gameObject.name == "Fixus_Tree_2_LOD1"
                        || mf.gameObject.name == "LOD0")
                        //|| mf.gameObject.name == "ground_cover_a_LOD0"
                        //|| mf.gameObject.name == "ground_cover_c_LOD0"
                    {
                        if (Main.debugMode)
                            Debug.Log("Stranded Deep AnimatedFoliage mod DeformingInit for " + mf.gameObject.name);

                        meshFilter = mf;

                        renderer = mf.gameObject.GetComponent<Renderer>();
                    }
                    if (mf.gameObject.name == "Climber_Foliage_LOD0")
                    {
                        secondaryMeshFilter = mf;
                    }
                }

                if (meshFilter == null)
                    return;

                InitClonedMeshes();
                RecomputeDeltas();

                initOk = true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender DeformingInit : " + e);
            }
        }

        protected virtual void RecomputeDeltas()
        {
            float oscillationPhase = 0.1f;
            float oscillationDelta = 3.0f;
            delta = new Vector3[originalVertices.Length];

            // OPT: replaced Dictionary with arrays for O(1) access without boxing/hashing
            stormdelta  = new Vector3[originalVertices.Length];
            hasstormdelta = new bool[originalVertices.Length];

            // OPT: cache Time.time once instead of calling DateTime.Now per vertex
            float timeSeed = Time.time;

            if (IsPalm)
            {
                Vector3 windDirection = Main.Wind;
                float angle = Vector3.SignedAngle(transform.forward, windDirection, Vector3.up);
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender wind angle to tree = " + angle);

                // OPT: precompute trig values shared across all vertices
                float sinAngle = Mathf.Sin(angle);
                float cosAngle = Mathf.Cos(angle);
                float sqrSin   = sinAngle * sinAngle;
                float sqrCos   = cosAngle * cosAngle;
                float sinPhase = Mathf.Sin(timeSeed * oscillationPhase * Mathf.PI * 2.0f);

                for (int i = 0; i < originalVertices.Length; i++)
                {
                    displacedVertices[i] = originalVertices[i];

                    // OPT: avoid allocating a new Vector3 just to zero the Y component
                    float vx = originalVertices[i].x;
                    float vz = originalVertices[i].z;
                    float sqrMag = vx * vx + vz * vz;
                    float ratio = sqrMag / 200f;

                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender wind ratio = " + ratio);
                    float angle2 = Vector3.SignedAngle(originalVertices[i], windDirection, Vector3.up);
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender wind angle to vertex = " + angle2);

                    //if (angle2 > 110 || angle2 < -110)
                    //{
                        delta[i] = Quaternion.AngleAxis(angle, Vector3.up) 
                                        * new Vector3(
                                            (float)random.NextDouble() * sinPhase,
                                            (float)random.NextDouble() * sinPhase,
                                            (float)random.NextDouble() * sinPhase) * ratio * oscillationDelta;
                    //}
                    //else
                    // FIX: était "angle2 < 110 || angle2 > -110" — toujours vraie (aucun angle ne peut
                    //      satisfaire les deux conditions inverses simultanément), donc stormdelta était
                    //      peuplé pour TOUS les vertices, écrasant l'oscillation de base (delta) et
                    //      empêchant les palmiers d'osciller. Corrigé en && pour ne cibler que les
                    //      vertices dans le cône ±110° face au vent.
                    if (angle2 > -110 && angle2 < 110)
                    {
                        ratio = sqrMag / 10;
                        //float offsetCos = Mathf.Cos(angle) + 1;
                        if (angle2 > 45 || angle2 < -45)
                        {
                            stormdelta[i] = new Vector3(Main.Wind.x * ratio * sqrSin,
                                0.0f,//-1.0f * ratio,
                                Main.Wind.z * ratio * sqrSin);// * ratio * oscillationDelta;
                        }
                        else
                        {
                            stormdelta[i] = new Vector3(Main.Wind.x * ratio * sqrSin,
                                -1.0f * ratio * sqrCos,
                                Main.Wind.z * ratio * sqrSin);// * ratio * oscillationDelta;
                        }
                        hasstormdelta[i] = true;
                    }
                    //else
                    //{
                    //    stormdelta[i] = delta[i];
                    //}
                }
            }
            else
            {
                // OPT: cache sinPhase once for all vertices
                float sinPhase = Mathf.Sin(timeSeed * oscillationPhase * Mathf.PI * 2.0f);

                for (int i = 0; i < originalVertices.Length; i++)
                {
                    displacedVertices[i] = originalVertices[i];

                    // move more if far from center
                    //Mathf.Min(originalVertices[i].y, transform.localPosition.y)
                    // OPT: avoid allocating a new Vector3 just to zero the Y component
                    float vx = originalVertices[i].x;
                    float vz = originalVertices[i].z;
                    float sqrMag = vx * vx + vz * vz;
                    //if (originalVertices[i].y >= transform.localPosition.y)
                    //{
                    //    sqrMag = (originalVertices[i] - new Vector3(0, 2 * originalVertices[i].y, 0)).sqrMagnitude;
                    //}
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex sqrmag = " + sqrMag);

                    float ratio = Mathf.Min(sqrMag, 25) / 200f;//Mathf.Pow(sqrMag, 2) / 200f;
#warning magic number
                    //float ratio = Math.Min(0.1f, sqrMag / 200f);

                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex ratio = " + ratio);
                    delta[i] = new Vector3(
                        (float)random.NextDouble() * sinPhase,
                        (float)random.NextDouble() * sinPhase,
                        (float)random.NextDouble() * sinPhase) * ratio * oscillationDelta;
                    //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex delta = " + delta[i]);
                }
            }

            if (secondaryMeshFilter != null)
            {
                secondaryDelta = new Vector3[secondaryOriginalVertices.Length];
                // OPT: cache sinPhase once for secondary vertices too
                float sinPhase = Mathf.Sin(timeSeed * oscillationPhase * Mathf.PI * 2.0f);

                for (int i = 0; i < secondaryOriginalVertices.Length; i++)
                {
                    secondaryDisplacedVertices[i] = secondaryOriginalVertices[i];

                    // move more if far from center
                    float vx = secondaryOriginalVertices[i].x;
                    float vz = secondaryOriginalVertices[i].z;
                    float sqrMag = vx * vx + vz * vz;
                    float ratio = sqrMag / 200f;
                    secondaryDelta[i] = new Vector3(
                        (float)random.NextDouble() * sinPhase,
                        (float)random.NextDouble() * sinPhase,
                        (float)random.NextDouble() * sinPhase) * ratio * oscillationDelta;
                }
            }
        }

        private void InitClonedMeshes()
        {
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

            if (secondaryMeshFilter != null)
            {
                secondaryOriginalMesh = secondaryMeshFilter.sharedMesh;

                secondaryClonedMesh = new Mesh();

                secondaryClonedMesh.name = "clone";
                secondaryClonedMesh.vertices = secondaryOriginalMesh.vertices;
                secondaryClonedMesh.triangles = secondaryOriginalMesh.triangles;
                secondaryClonedMesh.normals = secondaryOriginalMesh.normals;
                secondaryClonedMesh.uv = secondaryOriginalMesh.uv;
                secondaryClonedMesh.uv2 = secondaryOriginalMesh.uv2;
                secondaryClonedMesh.uv3 = secondaryOriginalMesh.uv3;
                secondaryClonedMesh.uv4 = secondaryOriginalMesh.uv4;
                secondaryClonedMesh.uv5 = secondaryOriginalMesh.uv5;
                secondaryClonedMesh.uv6 = secondaryOriginalMesh.uv6;
                secondaryClonedMesh.uv7 = secondaryOriginalMesh.uv7;
                secondaryClonedMesh.uv8 = secondaryOriginalMesh.uv8;

                //Debug.Log("Stranded Deep AnimatedFoliage mod subMeshCount " + secondaryOriginalMesh.subMeshCount);
                secondaryClonedMesh.subMeshCount = secondaryOriginalMesh.subMeshCount;
                for (int i = 0; i < secondaryOriginalMesh.subMeshCount; i++)
                {
                    SubMeshDescriptor smd = secondaryOriginalMesh.GetSubMesh(i);
                    SubMeshDescriptor newSmd = new SubMeshDescriptor(smd.indexStart, smd.indexCount, smd.topology);
                    newSmd.baseVertex = smd.baseVertex;
                    newSmd.firstVertex = smd.firstVertex;
                    newSmd.bounds = smd.bounds;
                    newSmd.vertexCount = smd.vertexCount;
                    secondaryClonedMesh.SetSubMesh(i, newSmd, MeshUpdateFlags.Default);
                }
                secondaryClonedMesh.colors = secondaryOriginalMesh.colors;
                secondaryClonedMesh.tangents = secondaryOriginalMesh.tangents;
                secondaryClonedMesh.bindposes = secondaryOriginalMesh.bindposes;
                secondaryMeshFilter.sharedMesh = secondaryClonedMesh;

                secondaryOriginalVertices = secondaryOriginalMesh.vertices;
                secondaryDisplacedVertices = new Vector3[secondaryOriginalVertices.Length];
            }
        }

        float windt = 0.0f;
        float windSpeed = 1.0f;
        float previousWindSpeed = 1.0f;
        float targetWindSpeed = 1.0f;

        private void DeformingUpdate()
        {
            if (meshFilter != null)
            {
                //if (Main.stormPercentage > 0)
                //{
                    if (previousWindSpeed != targetWindSpeed)
                    {
                        windt += 0.001f * Time.deltaTime;
                        windSpeed = Mathf.Lerp(previousWindSpeed, targetWindSpeed, windt);
                        if (Mathf.Approximately(windSpeed, targetWindSpeed))
                        {
                            previousWindSpeed = targetWindSpeed;
                            windt = 0.0f;
                        }
                    }
                    else
                    {
                        if (IsPalm)
                            targetWindSpeed = 2.0f + (2.0f * (float)Main.stormPercentage / 100f) + (float)random.NextDouble();
                        else
                            // OPT: replaced DateTime.Now.Millisecond with Time.time — same randomness, no system call
                            targetWindSpeed = (float)Math.Abs(Math.Sin(Time.time)) + (2.0f * (float)Main.stormPercentage / 100f) + (float)random.NextDouble();
                }
                //}

                // OPT: cache storm ratio once per frame instead of recomputing per vertex
                float stormRatio = (float)Main.stormPercentage / 100f;
                float windTimeProduct = windSpeed * Time.time;

                float t = 0.0f;// (Mathf.Sin(Time.time * 0.2f * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
                int startIndex = 0; //random.Next(0, displacedVertices.Length - 2);
                int endIndex = displacedVertices.Length;//random.Next(startIndex, displacedVertices.Length);
                //float windSpeed = 2;
                for (int vertex = startIndex; vertex < endIndex; vertex++)
                {
                    // OPT: array lookup replaces Dictionary.ContainsKey — no hashing, no boxing
                    if (IsPalm 
                        && Main.stormPercentage > 0
                        && hasstormdelta[vertex])
                    {
                        // FIX: t était calculé APRÈS le premier Lerp, donc ce Lerp utilisait toujours
                        //      t=0.0 et retournait systématiquement la borne gauche — les palmiers
                        //      n'oscillaient jamais. t est maintenant calculé avant les deux Lerp.
                        float phase = 0.2f;
                        t = (Mathf.Sin(windTimeProduct * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

                        displacedVertices[vertex] = Vector3.Lerp(
                                originalVertices[vertex] - Mathf.PingPong(windTimeProduct, 3.0f) * (stormdelta[vertex] * stormRatio), 
                                originalVertices[vertex] + Mathf.PingPong(windTimeProduct, 3.0f) * (stormdelta[vertex] * stormRatio),
                            t);

                        Vector3 randomOsc = delta[vertex] * (1 + stormRatio);
                        displacedVertices[vertex] = Vector3.Lerp(displacedVertices[vertex] - randomOsc, displacedVertices[vertex] + randomOsc, t);
                    }
                    else
                    {
                        // OPT: avoid allocating a new Vector3 just to zero the Y component
                        float vx = originalVertices[vertex].x;
                        float vz = originalVertices[vertex].z;
                        float phase = 0.2f * Mathf.Sqrt(vx * vx + vz * vz) * (1 + stormRatio);
                        t = (Mathf.Sin(windTimeProduct * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

                        Vector3 randomOsc = delta[vertex] * (1 + stormRatio);
                        displacedVertices[vertex] = Vector3.Lerp(originalVertices[vertex] - randomOsc, originalVertices[vertex] + randomOsc, t);
                    }
                }

                //clonedMesh.vertices = displacedVertices;
#warning performance test
                clonedMesh.SetVertices(displacedVertices);
                clonedMesh.RecalculateNormals();

                if (_animatedSecondaryMesh
                    && secondaryMeshFilter != null)
                {
                    t = 0.0f;
                    startIndex = 0;
                    endIndex = secondaryDisplacedVertices.Length;
                    for (int vertex = startIndex; vertex < endIndex; vertex++)
                    {
                        // OPT: avoid allocating a new Vector3 just to zero the Y component
                        float vx = secondaryOriginalVertices[vertex].x;
                        float vz = secondaryOriginalVertices[vertex].z;
                        float phase = 0.2f * Mathf.Sqrt(vx * vx + vz * vz) * (1 + stormRatio);
                        t = (Mathf.Sin(Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

                        Vector3 randomOsc = secondaryDelta[vertex] * (1 + stormRatio);
                        secondaryDisplacedVertices[vertex] = Vector3.Lerp(secondaryOriginalVertices[vertex] - randomOsc, secondaryOriginalVertices[vertex] + randomOsc, t);
                    }

                    //secondaryClonedMesh.vertices = secondaryDisplacedVertices;
#warning performance test
                    secondaryClonedMesh.SetVertices(secondaryDisplacedVertices);
                    secondaryClonedMesh.RecalculateNormals();
                }
            }
            else
            {
                DeformingInit();
            }
        }

        public void Create(uint prefabId, float offset = 0f)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            string text;
            bool flag = Prefabs.TryGetMultiplayerPrefabName(prefabId, out text);
            Vector3 vector = base.transform.position + forward + Vector3.up * offset;
            if (!flag)
            {
                MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, vector, Quaternion.identity, MiniGuid.New(), null);
                //new Beam.DeveloperMode.ReplicateCreate
                //{
                //    PrefabId = (short)prefabId,
                //    Pos = vector
                //}.Post();
                return;
            }
            if (Game.Mode.IsClient())
            {
                //new Beam.DeveloperMode.ReplicateCreate
                //{
                //    PrefabId = (short)prefabId,
                //    Pos = vector
                //}.Post();
                return;
            }
            MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, vector, Quaternion.identity, MiniGuid.New(), null);
        }
    }
}
