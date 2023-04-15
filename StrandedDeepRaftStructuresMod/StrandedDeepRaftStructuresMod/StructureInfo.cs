using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepRaftStructuresMod
{
    public class StructureInfo
    {
        public bool IsRootStructureType { get; set; }

        public Vector3 GhostPosition { get; set; }

        public Vector3 GhostScale { get; set; }

        public RaftStructuresColliderType ColliderType { get; set; }

        public float ContactOffset { get; set; }

        public List<Vector3> SphereColliderCenter { get; set; }
        public List<float> SphereColliderRadius { get; set; }

        public List<Vector3> BoxColliderCenter { get; set; }
        public List<Vector3> BoxColliderSize { get; set; }

        public Vector3 MeshColliderScale { get; set; }
        public Vector3 MeshColliderPosition { get; set; }

        public float RepositionY { get; set; }

        public enum RaftStructuresColliderType
        {
            Sphere,
            Box,
            Mesh
        }

        public StructureInfo()
        {
            SphereColliderCenter = new List<Vector3>();
            SphereColliderRadius = new List<float>();

            BoxColliderCenter = new List<Vector3>();
            BoxColliderSize = new List<Vector3>();

            ContactOffset = 0.1f;

            GhostScale = new Vector3(1f, 1f, 1f);
            MeshColliderScale = new Vector3(1f, 1f, 1f);

            RepositionY = -1;
        }
    }
}
