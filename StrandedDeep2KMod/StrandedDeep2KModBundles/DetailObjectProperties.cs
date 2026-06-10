using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    internal class DetailObjectProperties
    {
        static System.Random r = new System.Random(StrandedWorld.WORLD_SEED);
        public GameObject Prefab { get; set; }
        private float _scale;
        public float Scale
        {
            get
            {
                if (!RandomizeScale)
                    return _scale;

                float scale = ((float)r.Next(80, 110) / 100f) * _scale;
                return scale;
            }
            set
            {
                _scale = value;
            }
        }
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }
        public bool RandomizeScale { get; set; }
        public bool RandomizeRotation { get; set; }
        public Quaternion ForceRotation { get; set; }
        public bool UseTerrainNormal { get; set; }
        /// <summary>
        /// 0.1 = extremely common
        /// 9.9 = extremely rare
        /// </summary>
        public float Rarity { get; set; }

        private int _maxInstances;
        public int MaxInstances
        {
            get
            {
                return _maxInstances;
            }
            set
            {
                _maxInstances = value;
            }
        }

        public DetailObjectProperties()
        {
            Scale = 1.0f;
            MinHeight = 2.25f;
            MaxHeight = 2.35f;
            MaxInstances = 1;
            Rarity = 9;
            UseTerrainNormal = true;
            RandomizeRotation = true;
        }
    }
}
