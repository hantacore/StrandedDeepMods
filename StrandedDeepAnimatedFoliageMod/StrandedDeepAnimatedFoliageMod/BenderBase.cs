using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepAnimatedFoliageMod
{
    public class BenderBase : MonoBehaviour
    {
        protected bool _animatedSecondaryMesh = false;
        protected Renderer renderer = null;
        protected MeshFilter meshFilter;
        
        public bool IsSmallTree { get; set; }

        protected virtual bool IsUnderwaterObject()
        {
            return false;
        }

        protected virtual bool CheckDistance()
        {
            try
            {
                if (PlayerRegistry.LocalPlayer == null)
                    return false;

                float magnitude = Vector3.Magnitude(this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position);
                //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
                _animatedSecondaryMesh = (magnitude <= Main.distanceRatio * 30f);
                if (IsSmallTree && magnitude > Main.distanceRatio * 50f
                    || magnitude > Main.distanceRatio * 200f)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AnimatedFoliage mod error on TreeBender CheckDistance : " + e);
            }
            return true;
        }

        protected virtual bool CheckVisible()
        {
            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
                if (renderer == null)
                {
                    renderer = GetComponentInChildren<Renderer>();
                }
                if (renderer == null)
                {
                    return false;
                }
            }

            if (renderer.isVisible)
                return true;
            else
                return false;
        }

        protected virtual bool DoChecks()
        {
            if (meshFilter != null
                && !meshFilter.gameObject.activeSelf)
                return false;

            if (!CheckVisible()
                || !IsUnderwaterObject() && PlayerRegistry.LocalPlayer.Movement.IsUnderwater
                || IsUnderwaterObject() && !PlayerRegistry.LocalPlayer.Movement.IsUnderwater
                || !CheckDistance())
                return false;

            return true;
        }
    }
}
