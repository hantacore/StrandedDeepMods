using Beam;
using Beam.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepRaftStructuresMod
{
//    static partial class Main
//    {
//        private static List<Constructing> _handledCollisionBoxes = new List<Constructing>();

//        private static bool _initializeCollisionBoxes = true;

//        static Constructing[] GameObjectsToHandle = null;
//        static int lastPassGameObjectsCount = 0;
//        static int currentGameObjectIndex = -1;

//        private static void InitUpdateCollisionBoxes()
//        {
//            try
//            {
//                Debug.Log("Stranded Deep RaftStructures Mod : InitUpdateCollisionBoxes");

//                foreach (Type supportedType in _supportedStructures.Keys)
//                {
//                    StructureInfo si = _supportedStructures[supportedType];

//                    UnityEngine.Object[] instances = Game.FindObjectsOfType(supportedType);
//                    foreach (UnityEngine.Object currentInstance in instances)
//                    {
//                        Constructing construct = currentInstance as Constructing;
//                        if (construct == null || _handledCollisionBoxes.Contains(construct) || construct.transform.parent == null)
//                        {
//                            continue;
//                        }
//                        HandleOne(supportedType, si, construct);
//                        _handledCollisionBoxes.Add(construct);
//                    }
//                }

//                _initializeCollisionBoxes = false;
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep RaftStructures Mod : error on UpdateCollisionBoxes : " + e);
//            }
//        }

//        private static void FillUpdateCollisionBoxesQueue()
//        {
//            try
//            {
//                if (currentGameObjectIndex > 0
//                    || GameObjectsToHandle != null)
//                    return;

//                //List<UnityEngine.Object> allSupportedObjects = new List<UnityEngine.Object>();
//                //foreach (Type supportedType in _supportedStructures.Keys)
//                //{
//                //    UnityEngine.Object[] instances = Game.FindObjectsOfType(supportedType);
//                //    allSupportedObjects.AddRange(instances);
//                //}

//                //Debug.Log("Stranded Deep RaftStructures Mod : FillUpdateCollisionBoxesQueue found " + allSupportedObjects.Count + " supported objects");

//                //if (allSupportedObjects.Count == 0
//                //    || lastPassGameObjectsCount == allSupportedObjects.Count)
//                //{
//                //    return;
//                //}

//                //lastPassGameObjectsCount = allSupportedObjects.Count;

//                //GameObjectsToHandle = new UnityEngine.Object[lastPassGameObjectsCount];
//                //Array.Copy(allSupportedObjects.ToArray(), GameObjectsToHandle, lastPassGameObjectsCount);

//                Constructing[] objects = Beam.Game.FindObjectsOfType<Constructing>();

//                if (objects.Length == 0
//                    || lastPassGameObjectsCount == objects.Length)
//                {
//                    return;
//                }

//                lastPassGameObjectsCount = objects.Length;

//                GameObjectsToHandle = new Constructing[lastPassGameObjectsCount];
//                Array.Copy(objects, GameObjectsToHandle, lastPassGameObjectsCount);
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep RaftStructures Mod : FillUpdateCollisionBoxesQueue failed " + e);
//            }
//        }

//        private static void UpdateCollisionBoxes()
//        {
//            //Debug.Log("Stranded Deep RaftStructures Mod : UpdateCollisionBoxes current index " + currentGameObjectIndex);
//            //Debug.Log("Stranded Deep RaftStructures Mod : UpdateCollisionBoxes current index " + (GameObjectsToHandle != null ? GameObjectsToHandle.Length.ToString() : "null"));

//            if (GameObjectsToHandle == null
//                   || GameObjectsToHandle.Length == 0)
//            {
//                // game reset issue
//                if (GameObjectsToHandle != null && GameObjectsToHandle.Length == 0)
//                    GameObjectsToHandle = null;
//                return;
//            }

//            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//            sw.Start();
//            try
//            {
//                if (currentGameObjectIndex < 0)
//                    currentGameObjectIndex = GameObjectsToHandle.Length - 1;

//                while (currentGameObjectIndex >= 0
//                    && sw.ElapsedMilliseconds <= 3)
//                {
//                    Constructing construct = GameObjectsToHandle[currentGameObjectIndex];
//                    Type supportedType = construct.GetType();
//                    Debug.Log("Stranded Deep RaftStructures Mod : UpdateCollisionBoxes " + supportedType);
//                    try
//                    {
//                        if (_supportedStructures.ContainsKey(supportedType))
//                        {
//                            StructureInfo si = _supportedStructures[supportedType];
//                            HandleOne(supportedType, si, construct);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        Debug.Log("Stranded Deep RaftStructures Mod : UpdateCollisionBoxes failed for " + (construct.name == null ? "null" : construct.name) + " : " + e);
//                    }
//                    finally
//                    {
//                        _handledCollisionBoxes.Add(construct);
//                        currentGameObjectIndex--;
//                    }
//                }

//                if (currentGameObjectIndex < 0)
//                    GameObjectsToHandle = null;
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Stranded Deep RaftStructures Mod : UpdateCollisionBoxes fatally failed " + e);
//            }
//        }

//        private static void HandleOne(Type supportedType, StructureInfo si, Constructing construct)
//        {
//            Debug.Log("Stranded Deep RaftStructures " + supportedType + " parent name : " + construct.transform.parent.name);

//            if (construct.transform.parent.name.Contains("STRUCTURE_RAFT"))
//            //|| construct.transform.parent.name.Contains("STRUCTURE_SMALL"))
//            {
//                Collider[] cc = construct.GetComponents<Collider>();
//                Debug.Log("Stranded Deep RaftStructures UpdateCollisionBoxes found " + supportedType + " on raft - updating colliders (" + cc.Length + ")");
//                int current = 0;
//                foreach (Collider c in cc)
//                {
//                    Debug.Log("Stranded Deep RaftStructures UpdateCollisionBoxes found " + supportedType + " on raft - updating collider (" + c.GetType().Name + ")");
//                    if (c is SphereCollider
//                      && si.ColliderType == StructureInfo.RaftStructuresColliderType.Sphere)
//                    {
//                        ((SphereCollider)c).contactOffset = si.ContactOffset;
//                        if (si.SphereColliderCenter.Count > current)
//                            ((SphereCollider)c).center = si.SphereColliderCenter[current];
//                        if (si.SphereColliderRadius.Count > current)
//                            ((SphereCollider)c).radius = si.SphereColliderRadius[current];
//                    }
//                    else if (c is BoxCollider
//                        && si.ColliderType == StructureInfo.RaftStructuresColliderType.Box)
//                    {
//                        ((BoxCollider)c).contactOffset = si.ContactOffset;
//                        if (si.BoxColliderCenter.Count > current)
//                            ((BoxCollider)c).center = si.BoxColliderCenter[current];
//                        if (si.BoxColliderSize.Count > current)
//                            ((BoxCollider)c).size = si.BoxColliderSize[current];
//                    }
//                    else if (c is MeshCollider
//                        && si.ColliderType == StructureInfo.RaftStructuresColliderType.Mesh
//                        && supportedType != typeof(Constructing_SMOKER))
//                    {
//                        //((MeshCollider)c).contactOffset = si.ContactOffset;
//                        //((MeshCollider)c).transform.position = si.MeshColliderPosition;
//                        //((MeshCollider)c).transform.localScale = si.MeshColliderScale;
//                    }

//                    current++;
//                }

//                //if (construct is Constructing_SPIT)
//                //{
//                //    Constructing_SPIT spit = construct as Constructing_SPIT;
//                //}

//#warning EXPERIMENTAL
//                if (construct is Construction_PIT)
//                {
//                    foreach (Connector connector in construct.MyConnectors)
//                    {
//                        if (connector.Type == ConnectorType.CAMPFIRE_EXTENSION_LEVEL_2)
//                        {
//                            Debug.Log("Stranded Deep RaftStructures UpdateCollisionBoxes found " + supportedType + " on raft - updating CAMPFIRE_EXTENSION_LEVEL_2 position");
//                            //connector.Collider.center = new Vector3(0, 0.7f, 0);
//                            connector.Collider.center = new Vector3(0, 0.35f, 0);
//                        }
//                    }
//                }
//#warning EXPERIMENTAL
//            }
//        }
//    }
}
