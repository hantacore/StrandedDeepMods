using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    [RequireComponent(typeof(MeshFilter))]
    public class SmallTreeBender : TreeBender
    {
        protected override void RecomputeDeltas()
        {
            float oscillationPhase = 0.1f;
            float oscillationDelta = 3.0f;
            delta = new Vector3[originalVertices.Length];

            // OPT: stormdelta est maintenant un tableau (plus un Dictionary) — pas de .Clear() nécessaire,
            //      la réallocation ci-dessous initialise déjà tout à zéro
            stormdelta = new Vector3[originalVertices.Length];
            hasstormdelta = new bool[originalVertices.Length];

            // OPT: Time.time mis en cache une fois — remplace DateTime.Now.Second par vertex
            float sinPhase = Mathf.Sin(Time.time * oscillationPhase * Mathf.PI * 2.0f);

            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                // move more if far from center
                //Mathf.Min(originalVertices[i].y, transform.localPosition.y)
                // OPT: évite d'allouer un new Vector3 juste pour annuler le composant Y
                float vx = originalVertices[i].x;
                float vz = originalVertices[i].z;
                float sqrMag = vx * vx + vz * vz;
                //if (originalVertices[i].y >= transform.localPosition.y)
                //{
                //    sqrMag = (originalVertices[i] - new Vector3(0, 2 * originalVertices[i].y, 0)).sqrMagnitude;
                //}
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex sqrmag = " + sqrMag);
                float ratio = sqrMag / 100f;//Mathf.Pow(sqrMag, 2) / 200f;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex ratio = " + ratio);
                delta[i] = new Vector3(
                    (float)random.NextDouble() * sinPhase,
                    (float)random.NextDouble() * sinPhase,
                    (float)random.NextDouble() * sinPhase) * ratio * oscillationDelta;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex delta = " + delta[i]);
            }

            if (secondaryMeshFilter != null)
            {
                secondaryDelta = new Vector3[secondaryOriginalVertices.Length];
                for (int i = 0; i < secondaryOriginalVertices.Length; i++)
                {
                    secondaryDisplacedVertices[i] = secondaryOriginalVertices[i];

                    // move more if far from center
                    // OPT: même optimisation Y-zeroing que ci-dessus
                    float vx = secondaryOriginalVertices[i].x;
                    float vz = secondaryOriginalVertices[i].z;
                    float sqrMag = vx * vx + vz * vz;
                    float ratio = sqrMag / 100f;
                    secondaryDelta[i] = new Vector3(
                        (float)random.NextDouble() * sinPhase,
                        (float)random.NextDouble() * sinPhase,
                        (float)random.NextDouble() * sinPhase) * ratio * oscillationDelta;
                }
            }
        }

        // OPT: override supprimé — BenderBase.CheckDistance() gère déjà le cas IsSmallTree
        //      (branche "IsSmallTree && sqrMagnitude > _sqrDistanceSmallTree") avec sqrMagnitude,
        //      ce qui est plus efficace que Vector3.Magnitude. Il suffit de s'assurer que
        //      IsSmallTree = true est bien positionné à l'initialisation dans le code appelant.
        //
        // Code original conservé ci-dessous pour référence :
        //protected override bool CheckDistance()
        //{
        //    try
        //    {
        //        if (PlayerRegistry.LocalPlayer == null)
        //            return false;
        //
        //        float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
        //        //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
        //        if (magnitude > Main.distanceRatio * 50f)
        //            return false;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Stranded Deep AnimatedFoliage mod error on SmallTreeBender CheckDistance : " + e);
        //    }
        //    return true;
        //}
    }
}