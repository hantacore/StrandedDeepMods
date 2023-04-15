using Beam;
using Beam.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Funlabs;
using HarmonyLib;


namespace StrandedDeepRaftStructuresMod
{
    static partial class Main
    {
        private static void DebugInformation()
        {
            try
            {
                //Constructing[] css = Beam.Game.FindObjectsOfType<Constructing>();

                //css[0].ColliderType == GhostCollider.ColliderType.VEHICLE_SAIL

                //Ghost g;
                //Dictionary<GhostCollider.ColliderType, GhostCollider> _ghostColliders = new Dictionary<GhostCollider.ColliderType, GhostCollider>()

                IPlayer p = PlayerRegistry.LocalPlayer;
                if (p != null)
                {
                    //Debug.Log("Stranded Deep RaftStructures Mod test : Crafter state : " + p.Crafter.State);

                    //if (p.Crafter.IsPlacing)
                    //{
                    //    //Construction_CAMPFIRE
                    //    Debug.Log("Stranded Deep RaftStructures Mod test : Crafter.IsPlacing");
                    //}

                    //if (p.Crafter.CurrentCraftable != null)
                    //{
                    //    Debug.Log("Stranded Deep RaftStructures Mod test : Crafter CurrentCraftable : " + p.Crafter.CurrentCraftable.DisplayName);
                    //}

                    Ghost crafterghost = fi_crafterghost.GetValue(p.Crafter) as Ghost;
                    //Collider sailCollider = null;
                    //GhostCollider sailGhostCollider = null;
                    if (crafterghost != null)
                    {
                        //Dictionary<GhostCollider.ColliderType, GhostCollider> ghostColliders = fi_ghostColliders.GetValue(crafterghost) as Dictionary<GhostCollider.ColliderType, GhostCollider>;
                        //foreach (GhostCollider.ColliderType gct in ghostColliders.Keys)
                        //{
                        //    //if (gct == GhostCollider.ColliderType.VEHICLE_SAIL)
                        //    //{
                        //    //    sailGhostCollider = ghostColliders[gct];
                        //    //}

                        //    Debug.Log("Stranded Deep RaftStructures Mod ghost : found ghost colliders for key : " + gct);
                        //    foreach (Collider collider in ghostColliders[gct].Colliders)
                        //    {
                        //        Debug.Log("Stranded Deep RaftStructures Mod ghost collider : " + collider.name);
                        //        //if (gct == GhostCollider.ColliderType.VEHICLE_SAIL)
                        //        //{
                        //        //    sailCollider = collider;
                        //        //}
                        //    }
                        //}

                        //GhostCollider activeC = fi_activeCollider.GetValue(crafterghost) as GhostCollider;
                        //Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider : " + activeC.name);

                        //Layers.CONSTRUCTIONS_RAFTS;

                        IPlaceable ipl = fi_placingCraftable.GetValue(p.Crafter) as IPlaceable;
                        if (ipl != null)
                        {
                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable : " + ipl.GetType().FullName);

                            if (ipl is Constructing)
                            {
                                Constructing construct = ipl as Constructing;
                                LayerMask lm = (LayerMask)fi_constructingConnectorLayerMask.GetValue(construct);
                                Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable ConnectorLayerMask : " + lm.value);

                                FieldInfo fi_placementModifiers = typeof(Constructing).GetField("_placementModifiers", BindingFlags.NonPublic | BindingFlags.Instance);
                                Beam.Crafting.PlacementModifier[] modifiers = fi_placementModifiers.GetValue(construct) as Beam.Crafting.PlacementModifier[];
                                foreach (Beam.Crafting.PlacementModifier modifier in modifiers)
                                {
                                    Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable Constructing placementmodifier : " + modifier.GetType().Name);
                                }

                                Connector myConnector = fi_constructingConnector.GetValue(construct) as Connector;
                                Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + (myConnector == null ? "null" : myConnector.name));
                                if (myConnector != null)
                                {
                                    try
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.Type);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.Constructing.DisplayName);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.gameObject.name);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.transform.position);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.transform.localPosition);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector Checks.Count : " + myConnector.Checks.Count);
                                        foreach (ConnectorCheck ccheck in myConnector.Checks)
                                        {
                                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector check : " + ccheck.name);
                                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector check : " + ccheck.GetType().Name);
                                        }
                                        AttributeType at = (AttributeType)fi_maximumTier.GetValue(myConnector);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector maximumTier : " + at);
                                    }
                                    catch { }

                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : VEHICLE_SAIL(Clone)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : VEHICLE_MAST
                                    //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : INTERACTIVE_TYPE_BUILDING_RAFT_SAIL
                                    //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : VEHICLE_SAIL(Clone)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : (0.00, 0.00, 0.00)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : (0.00, 0.00, 0.00)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : 0
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : null
                                }

                                Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + (construct.SnappedConnector == null ? "null" : construct.SnappedConnector.name));
                                if (construct.SnappedConnector != null)
                                {
                                    try
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.Type);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.Constructing.DisplayName);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.gameObject.name);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.transform.position);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.transform.localPosition);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector Checks.Count : " + construct.SnappedConnector.Checks.Count);
                                        foreach (ConnectorCheck ccheck in construct.SnappedConnector.Checks)
                                        {
                                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector check : " + ccheck.name);
                                        }
                                        AttributeType at = (AttributeType)fi_maximumTier.GetValue(myConnector);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector maximumTier : " + at);

                                        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : VehicleConnector_Mast
                                        //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : VEHICLE_MAST
                                        //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : ATTRIBUTE_TYPE_WOOD INTERACTIVE_TYPE_BUILDING_RAFT_FLOOR
                                        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : VehicleConnector_Mast
                                        //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : (89.42, 2.39, 68.57)
                                        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : (0.00, 1.50, 0.00)
                                        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : 0
                                        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable ConnectorLayerMask: 67108864
                                    }
                                    catch { }
                                }
                                //VehicleConnector_Mast
                            }

                            if (ipl is ConstructionObject)
                            {
                                ConstructionObject consobj = ipl as ConstructionObject;
                                //LayerMask lm = (LayerMask)fi_constructingConnectorLayerMask.GetValue(construct);
                                //Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable ConnectorLayerMask : " + lm.value);

                                FieldInfo fi_placementModifiers = typeof(ConstructionObject).GetField("_placementModifiers", BindingFlags.NonPublic | BindingFlags.Instance);
                                Beam.Crafting.PlacementModifier[] modifiers = fi_placementModifiers.GetValue(consobj) as Beam.Crafting.PlacementModifier[];
                                foreach (Beam.Crafting.PlacementModifier modifier in modifiers)
                                {
                                    Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable ConstructionObject placementmodifier : " + modifier.GetType().Name);
                                }

                                Connector myConnector = fi_constructionObjectConnector.GetValue(consobj) as Connector;
                                Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + (myConnector == null ? "null" : myConnector.name));
                                if (myConnector != null)
                                {
                                    try
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.Type);
                                        if (myConnector.Constructing != null)
                                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.Constructing.DisplayName);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.gameObject.name);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.transform.position);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : " + myConnector.transform.localPosition);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector Checks.Count : " + myConnector.Checks.Count);
                                        foreach (ConnectorCheck ccheck in myConnector.Checks)
                                        {
                                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector check : " + ccheck.name);
                                            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector check : " + ccheck.GetType().Name);
                                        }
                                        AttributeType at = (AttributeType)fi_maximumTier.GetValue(myConnector);
                                        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector maximumTier : " + at);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod test : exception reading connector properties : " + e);
                                    }

                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : VEHICLE_SAIL(Clone)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : VEHICLE_MAST
                                    //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : INTERACTIVE_TYPE_BUILDING_RAFT_SAIL
                                    //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : VEHICLE_SAIL(Clone)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : (0.00, 0.00, 0.00)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : (0.00, 0.00, 0.00)
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : 0
                                    //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : null
                                }

                                //Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + (construct.SnappedConnector == null ? "null" : construct.SnappedConnector.name));
                                //if (consobj.SnappedConnector != null)
                                //{
                                //    try
                                //    {
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.Type);
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.Constructing.DisplayName);
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.gameObject.name);
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.transform.position);
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : " + construct.SnappedConnector.transform.localPosition);
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector Checks.Count : " + construct.SnappedConnector.Checks.Count);
                                //        foreach (ConnectorCheck ccheck in construct.SnappedConnector.Checks)
                                //        {
                                //            Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector check : " + ccheck.name);
                                //        }
                                //        AttributeType at = (AttributeType)fi_maximumTier.GetValue(myConnector);
                                //        Debug.Log("Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector maximumTier : " + at);

                                //        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : VehicleConnector_Mast
                                //        //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : VEHICLE_MAST
                                //        //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : ATTRIBUTE_TYPE_WOOD INTERACTIVE_TYPE_BUILDING_RAFT_FLOOR
                                //        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : VehicleConnector_Mast
                                //        //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : (89.42, 2.39, 68.57)
                                //        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : (0.00, 1.50, 0.00)
                                //        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable snapped connector : 0
                                //        //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable ConnectorLayerMask: 67108864
                                //    }
                                //    catch { }
                                //}
                                //VehicleConnector_Mast
                            }

                            //if (ipl.GetType().FullName == "Beam.Crafting.Construction_CAMPFIRE")
                            if (ipl is Construction_CAMPFIRE)
                            {
                                //MethodInfo mi_DisableAllColliders = typeof(Ghost).GetMethod("DisableAllColliders", BindingFlags.NonPublic | BindingFlags.Instance);
                                //mi_DisableAllColliders.Invoke(crafterghost, null);

                                //MethodInfo mi_InitializeCollider = typeof(Ghost).GetMethod("InitializeCollider", BindingFlags.NonPublic | BindingFlags.Instance);
                                //mi_InitializeCollider.Invoke(crafterghost, new object[] { ipl as ICraftable });

                                //((ICraftable)(ipl)).ColliderType = GhostCollider.ColliderType.VEHICLE_SAIL;

                                //fi_activeCollider.SetValue(crafterghost, sailGhostCollider);
                                //sailGhostCollider.gameObject.tag = ((ICraftable)(ipl)).gameObject.tag;
                                //sailGhostCollider.gameObject.SetActive(true);

                                //GhostCollider gc = fi_activeCollider.GetValue(crafterghost) as GhostCollider;
                                //if (gc != null && !gc.Colliders.Contains(sailCollider))
                                //{
                                //    FieldInfo fi_colliders = typeof(GhostCollider).GetField("_colliders", BindingFlags.Instance | BindingFlags.NonPublic);
                                //    List<Collider> newColliders = new List<Collider>();
                                //    Debug.Log("Stranded Deep RaftStructures Mod test : Crafter GhostCollider colliders count before : " + gc.Colliders.Length);
                                //    //newColliders.AddRange(gc.Colliders);
                                //    newColliders.Add(sailCollider);
                                //    fi_colliders.SetValue(gc, newColliders.ToArray());
                                //    Debug.Log("Stranded Deep RaftStructures Mod test : Crafter GhostCollider colliders count after : " + gc.Colliders.Length);
                                //}
                            }
                        }
                    }

                    IList<Ghost> ghosts = Game.FindObjectsOfType<Ghost>();
                    foreach (Ghost g in ghosts)
                    {
                        Debug.Log("Stranded Deep RaftStructures Mod ghost name : " + g.name);
                        Debug.Log("Stranded Deep RaftStructures Mod ghost Colliding : " + g.Colliding);
                        Dictionary<GhostCollider.ColliderType, GhostCollider> ghostColliders = fi_ghostColliders.GetValue(g) as Dictionary<GhostCollider.ColliderType, GhostCollider>;
                        //foreach (GhostCollider.ColliderType gct in ghostColliders.Keys)
                        //{
                        //    Debug.Log("Stranded Deep RaftStructures Mod ghost : found ghost colliders for key : " + gct);
                        //    foreach(Collider collider in ghostColliders[gct].Colliders)
                        //    {
                        //        Debug.Log("Stranded Deep RaftStructures Mod ghost collider : " + collider.name);
                        //    }
                        //}

                        GhostCollider activeC = fi_activeCollider.GetValue(g) as GhostCollider;
                        Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider : " + activeC.name);
                        foreach (Collider collider in activeC.Colliders)
                        {
                            Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : " + collider.name);
                            Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : " + collider.GetType());
                            if (collider is SphereCollider)
                            {
                                Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : " + ((SphereCollider)collider).center);
                                Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : " + ((SphereCollider)collider).radius);
                            }
                            else if (collider is BoxCollider)
                            {
                                Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : " + ((BoxCollider)collider).center);
                                Debug.Log("Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : " + ((BoxCollider)collider).size);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on DebugInformation : " + e);
            }
        }
    }
}
