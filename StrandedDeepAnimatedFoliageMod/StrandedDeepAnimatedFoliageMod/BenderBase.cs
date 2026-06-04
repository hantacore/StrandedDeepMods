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

        // OPT: cache squared distance thresholds to avoid recomputing every frame
        private float _sqrDistanceSmallTree;
        private float _sqrDistanceFar;
        private float _sqrDistanceSecondary;

        public bool IsSmallTree { get; set; }

        protected virtual void Awake()
        {
            RefreshDistanceThresholds();
        }

        // Call this if Main.distanceRatio can change at runtime
        protected void RefreshDistanceThresholds()
        {
            float secondary = Main.distanceRatio * 30f;
            float smallTree = Main.distanceRatio * 50f;
            float far = Main.distanceRatio * 200f;
            _sqrDistanceSecondary = secondary * secondary;
            _sqrDistanceSmallTree = smallTree * smallTree;
            _sqrDistanceFar = far * far;
        }

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

                // OPT: sqrMagnitude avoids a sqrt compared to Vector3.Magnitude
                float sqrMagnitude = (this.gameObject.transform.position - PlayerRegistry.LocalPlayer.transform.position).sqrMagnitude;
                //Debug.Log("Stranded Deep AnimatedFoliage : CheckDistance magnitude = " + magnitude);
                _animatedSecondaryMesh = (sqrMagnitude <= _sqrDistanceSecondary);
                if (IsSmallTree && sqrMagnitude > _sqrDistanceSmallTree
                    || sqrMagnitude > _sqrDistanceFar)
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

            // OPT: guard LocalPlayer null before accessing Movement
            if (PlayerRegistry.LocalPlayer == null)
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