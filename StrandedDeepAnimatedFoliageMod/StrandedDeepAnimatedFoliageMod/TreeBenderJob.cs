using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public struct TreeBenderJob : IJobParallelFor
    {
        public void Execute(int vertex)
        {
            //if (IsPalm
            //    && Main.stormPercentage > 0
            //    && stormdelta.ContainsKey(vertex))
            //{
            //    displacedVertices[vertex] = Vector3.Lerp(
            //            originalVertices[vertex] - Mathf.PingPong(windSpeed * Time.time, 3.0f) * (stormdelta[vertex] * ((float)Main.stormPercentage / 100f)),
            //            originalVertices[vertex] + Mathf.PingPong(windSpeed * Time.time, 3.0f) * (stormdelta[vertex] * ((float)Main.stormPercentage / 100f)),
            //        t);

            //    float phase = 0.2f;// * (originalVertices[vertex] - new Vector3(0, originalVertices[vertex].y, 0)).magnitude * (1 + (float)Main.stormPercentage / 100f);
            //    t = (Mathf.Sin(windSpeed * Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
            //    Vector3 randomOsc = delta[vertex] * (1 + (float)Main.stormPercentage / 100f);
            //    displacedVertices[vertex] = Vector3.Lerp(displacedVertices[vertex] - randomOsc, displacedVertices[vertex] + randomOsc, t);
            //}
            //else
            //{
            //    float phase = 0.2f * (originalVertices[vertex] - new Vector3(0, originalVertices[vertex].y, 0)).magnitude * (1 + (float)Main.stormPercentage / 100f);
            //    t = (Mathf.Sin(windSpeed * Time.time * phase * Mathf.PI * 2.0f) + 1.0f) / 2.0f;

            //    Vector3 randomOsc = delta[vertex] * (1 + (float)Main.stormPercentage / 100f);
            //    displacedVertices[vertex] = Vector3.Lerp(originalVertices[vertex] - randomOsc, originalVertices[vertex] + randomOsc, t);
            //}
        }
    }
}
