using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAlternativeEndgameMod
{
    public class BridgeSpotlight : MonoBehaviour
    {
        private Light spotlight = null;

        void Start()
        {
            spotlight = GetComponent<Light>();
            spotlight.type = LightType.Point;
            spotlight.spotAngle = 30.0f;
            spotlight.innerSpotAngle = 30.0f;
            spotlight.color = Color.white;
            spotlight.shape = LightShape.Box;
            spotlight.range = 50.0f;
            spotlight.intensity = 8.0f;
            spotlight.renderingLayerMask = Layers.WATER;
            spotlight.cullingMask = Layers.WATER;
            spotlight.shadows = LightShadows.Hard;
            spotlight.enabled = true;
        }

        void Update()
        {

        }
    }
}
