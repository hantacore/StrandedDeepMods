using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAlternativeEndgameMod
{
    public class RotatingSpotlight : MonoBehaviour
    {
        private Light spotlight = null;
        private float currentAngle = 0.0f;

        void Start()
        {
            spotlight = GetComponent<Light>();
            spotlight.type = LightType.Spot;
            spotlight.spotAngle = 30.0f;
            spotlight.innerSpotAngle = 30.0f;
            spotlight.color = Color.red;
            spotlight.shape = LightShape.Cone;
            spotlight.renderingLayerMask = Layers.WATER;
            spotlight.cullingMask = Layers.WATER;
            spotlight.shadows = LightShadows.Hard;
            originalRange = spotlight.range;
            spotlight.enabled = true;
        }

        float duration = 3.0f;
        float originalRange;

        void Update()
        {
            spotlight.intensity = 8.0f;// Mathf.PingPong(Time.time, 8);
            currentAngle++;
            if (currentAngle > 360.0f)
                currentAngle = 0.0f;
            spotlight.transform.Rotate(spotlight.transform.up, currentAngle);

            var amplitude = Mathf.PingPong(Time.time, duration);
            // Transform from 0..duration to 0.5..1 range.
            amplitude = amplitude / duration * 0.5f + 0.5f;
            // Set light range.
            spotlight.range = originalRange * amplitude;
        }
    }
}
