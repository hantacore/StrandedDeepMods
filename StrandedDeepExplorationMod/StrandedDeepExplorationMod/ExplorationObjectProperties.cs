using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepExplorationMod
{
    internal class ExplorationObjectProperties
    {
        public GameObject Prefab { get; set; }
        public int Scale { get; set; }
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }

        private int _maxInstances;
        public int MaxInstances
        {
            get
            {
                if (Main.debugMode)
                    return 10;

                return _maxInstances;
            }
            set
            {
                _maxInstances = value;
            }
        }

        public ExplorationObjectProperties()
        {
            MinHeight = 2.25f;
            MaxHeight = 2.35f;
            MaxInstances = 1;
        }
    }
}
