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
            stormdelta.Clear();


            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];

                // move more if far from center
                //Mathf.Min(originalVertices[i].y, transform.localPosition.y)
                float sqrMag = (originalVertices[i] - new Vector3(0, originalVertices[i].y, 0)).sqrMagnitude;
                //if (originalVertices[i].y >= transform.localPosition.y)
                //{
                //    sqrMag = (originalVertices[i] - new Vector3(0, 2 * originalVertices[i].y, 0)).sqrMagnitude;
                //}
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex sqrmag = " + sqrMag);
                float ratio = sqrMag / 100f;//Mathf.Pow(sqrMag, 2) / 200f;
                                            //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex ratio = " + ratio);
                delta[i] = new Vector3((float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                    (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f)) * ratio * oscillationDelta;
                //Debug.Log("Stranded Deep AnimatedFoliage mod TreeBender vertex delta = " + delta[i]);
            }


            if (secondaryMeshFilter != null)
            {
                secondaryDelta = new Vector3[secondaryOriginalVertices.Length];
                for (int i = 0; i < secondaryOriginalVertices.Length; i++)
                {
                    secondaryDisplacedVertices[i] = secondaryOriginalVertices[i];

                    // move more if far from center
                    float sqrMag = (secondaryOriginalVertices[i] - new Vector3(0, secondaryOriginalVertices[i].y, 0)).sqrMagnitude;
                    float ratio = sqrMag / 100f;
                    secondaryDelta[i] = new Vector3((float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                        (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f),
                        (float)random.NextDouble() * Mathf.Sin(DateTime.Now.Second * oscillationPhase * Mathf.PI * 2.0f)) * ratio * oscillationDelta;
                }
            }
        }

        protected override bool CheckDistance()
        {
            try
            {
                if (PlayerRegistry.LocalPlayer == null)
                    return false;

                float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
                //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
                if (magnitude > Main.distanceRatio * 50f)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on SmallTreeBender CheckDistance : " + e);
            }
            return true;
        }
    }
}
