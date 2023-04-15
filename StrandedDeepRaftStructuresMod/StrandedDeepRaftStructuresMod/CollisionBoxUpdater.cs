using Beam;
using Beam.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepRaftStructuresMod
{
    public class CollisionBoxUpdater : MonoBehaviour
    {
        public bool Updated { get; set; }

        public Constructing Constructing { get; set; }

        // Campfire
        //"localPosition": {
        //    "x": 0.02684345,
        //    "y": 0.3454199,
        //    "z": 1.491731
        //}

        // pit
        //localPosition": {
        //    "x": 0.04643604,
        //    "y": 0.2428298,
        //    "z": 1.50367
        //}

        // spit
        //"localPosition": {
        //    "x": 0.1126008,
        //    "y": 1.656161,
        //    "z": 1.445152
        //}

        // smoker
        //"localPosition": {
        //    "x": 1.267075,
        //    "y": 1.585868,
        //    "z": -1.426996
        //},

        // Still
        //"localPosition": {
        //    "x": -1.50431,
        //    "y": 0.2117652,
        //    "z": 1.507894
        //}

        public void Start()
        {
            if (Constructing == null)
                Constructing = this.GetComponentInChildren<Constructing>();
        }

        public void Update()
        {
            try
            {
                if (Constructing == null)
                    Constructing = this.GetComponentInChildren<Constructing>();

                if (Constructing == null)
                    return;

                if (WorldUtilities.IsWorldLoaded()
                    && Constructing is Construction_CAMPFIRE)
                {
                    Main.ReattachFood(Constructing as Construction_CAMPFIRE);
                }

                if (Updated)
                    return;

                if (!Main._supportedStructures.ContainsKey(Constructing.GetType())
                    || this.transform.parent == null)
                { 
                    if (this.transform.parent == null)
                        Debug.Log("Stranded Deep RaftStructures " + Constructing.GetType() + " parent is NULL");

                    return;
                }

                Type supportedType = Constructing.GetType();
                StructureInfo si = Main._supportedStructures[supportedType];

                //Debug.Log("Stranded Deep RaftStructures " + supportedType + " parent name : " + construct.transform.parent.name);
                if (Constructing.transform.parent.name.Contains("STRUCTURE_RAFT"))
                //|| construct.transform.parent.name.Contains("STRUCTURE_SMALL"))
                {
                    // Correct position
                    if (si.RepositionY >= 0)
                    {
                        Constructing.transform.localPosition = new Vector3(Constructing.transform.localPosition.x, si.RepositionY, Constructing.transform.localPosition.z);
                    }

                    Collider[] cc = Constructing.GetComponents<Collider>();
                    Debug.Log("Stranded Deep RaftStructures UpdateCollisionBoxes found " + supportedType + " on raft - updating colliders (" + cc.Length + ")");
                    int current = 0;
                    foreach (Collider c in cc)
                    {
                        Debug.Log("Stranded Deep RaftStructures UpdateCollisionBoxes found " + supportedType + " on raft - updating collider (" + c.GetType().Name + ")");
                        if (c is SphereCollider
                          && si.ColliderType == StructureInfo.RaftStructuresColliderType.Sphere)
                        {
                            ((SphereCollider)c).contactOffset = si.ContactOffset;
                            if (si.SphereColliderCenter.Count > current)
                                ((SphereCollider)c).center = si.SphereColliderCenter[current];
                            if (si.SphereColliderRadius.Count > current)
                                ((SphereCollider)c).radius = si.SphereColliderRadius[current];
                        }
                        else if (c is BoxCollider
                            && si.ColliderType == StructureInfo.RaftStructuresColliderType.Box)
                        {
                            ((BoxCollider)c).contactOffset = si.ContactOffset;
                            if (si.BoxColliderCenter.Count > current)
                                ((BoxCollider)c).center = si.BoxColliderCenter[current];
                            if (si.BoxColliderSize.Count > current)
                                ((BoxCollider)c).size = si.BoxColliderSize[current];
                        }
                        else if (c is MeshCollider
                            && si.ColliderType == StructureInfo.RaftStructuresColliderType.Mesh
                            && supportedType != typeof(Constructing_SMOKER))
                        {
                            //((MeshCollider)c).contactOffset = si.ContactOffset;
                            //((MeshCollider)c).transform.position = si.MeshColliderPosition;
                            //((MeshCollider)c).transform.localScale = si.MeshColliderScale;
                        }

                        current++;
                    }

                    //if (construct is Constructing_SPIT)
                    //{
                    //    Constructing_SPIT spit = construct as Constructing_SPIT;
                    //}

#warning EXPERIMENTAL
                    if (Constructing is Construction_PIT)
                    {
                        foreach (Connector connector in Constructing.MyConnectors)
                        {
                            if (connector.Type == ConnectorType.CAMPFIRE_EXTENSION_LEVEL_2)
                            {
                                Debug.Log("Stranded Deep RaftStructures UpdateCollisionBoxes found " + supportedType + " on raft - updating CAMPFIRE_EXTENSION_LEVEL_2 position");
                                //connector.Collider.center = new Vector3(0, 0.7f, 0);
                                connector.Collider.center = new Vector3(0, 0.35f, 0);
                            }
                        }
                    }
#warning EXPERIMENTAL

                    Updated = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Raftstructures mod : error while patching Constructing.Load : " + e);
            }
        }
    }
}
