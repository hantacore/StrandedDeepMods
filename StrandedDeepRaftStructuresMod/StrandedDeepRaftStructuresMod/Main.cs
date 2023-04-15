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
        private static FieldInfo fi_crafterghost = typeof(Crafter).GetField("_ghost", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_placingCraftable = typeof(Crafter).GetField("_placingCraftable", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo fi_activeCollider = typeof(Ghost).GetField("_activeCollider", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fi_ghostColliders = typeof(Ghost).GetField("_ghostColliders", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo fi_constructingConnector = typeof(Constructing).GetField("_myConnector", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_constructingConnectorLayerMask = typeof(Constructing).GetField("_connectorLayerMask", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_constructingColliderType = typeof(Constructing).GetField("_colliderType", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo fi_constructionObjectConnector = typeof(ConstructionObject).GetField("_myConnector", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_constructionObjectColliderType = typeof(ConstructionObject).GetField("_colliderType", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo fi_connectortype = typeof(Connector).GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_maximumTier = typeof(Connector).GetField("_maximumTier", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_checks = typeof(Connector).GetField("_checks", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo fi_cookables = typeof(Construction_CAMPFIRE).GetField("_cookables", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static Dictionary<Type, StructureInfo> _supportedStructures = new Dictionary<Type, StructureInfo>();

        private static string configFileName = "StrandedDeepRaftStructuresMod.config";
        private static bool logDebugInfo = false;

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
        private static bool perfCheck = false;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;
                modEntry.OnUnload = OnUnload;

                ReadConfig();
                InitSupportedStructures();

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("Stranded Deep RaftStructures Mod properly loaded");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on update : " + e);
            }
            finally
            {
                if (chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep RaftStructures Mod load time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }

            return false;
        }

        private static void InitSupportedStructures()
        {
            _supportedStructures.Add(typeof(Construction_CAMPFIRE), new StructureInfo());
            _supportedStructures[typeof(Construction_CAMPFIRE)].IsRootStructureType = true;
            _supportedStructures[typeof(Construction_CAMPFIRE)].ColliderType = StructureInfo.RaftStructuresColliderType.Sphere;
            _supportedStructures[typeof(Construction_CAMPFIRE)].ContactOffset = 0.01f;
            _supportedStructures[typeof(Construction_CAMPFIRE)].SphereColliderCenter.Add(new Vector3(0, 0.1f, 0));
            _supportedStructures[typeof(Construction_CAMPFIRE)].SphereColliderRadius.Add(0.04f);
            //_supportedStructures[typeof(Construction_CAMPFIRE)].SphereColliderCenter.Add(new Vector3(0, 0.15f, 0));
            //_supportedStructures[typeof(Construction_CAMPFIRE)].SphereColliderRadius.Add(0.1f);
            _supportedStructures[typeof(Construction_CAMPFIRE)].GhostPosition = new Vector3(0, 0.37f, 0);
            //_supportedStructures[typeof(Construction_CAMPFIRE)].GhostPosition = new Vector3(0, 0.40f, 0);
            _supportedStructures[typeof(Construction_CAMPFIRE)].RepositionY = 0.25f;

            _supportedStructures.Add(typeof(Constructing_STILL), new StructureInfo());
            _supportedStructures[typeof(Constructing_STILL)].IsRootStructureType = true;
            _supportedStructures[typeof(Constructing_STILL)].ColliderType = StructureInfo.RaftStructuresColliderType.Box;
            _supportedStructures[typeof(Constructing_STILL)].ContactOffset = 0.01f;
            _supportedStructures[typeof(Constructing_STILL)].GhostPosition = new Vector3(0, 0.1f, 0);
            _supportedStructures[typeof(Constructing_STILL)].RepositionY = 0.25f;

            // unmovable raft patch
            _supportedStructures[typeof(Constructing_STILL)].BoxColliderCenter.Add(new Vector3(0, 0.3f, 0));
            _supportedStructures[typeof(Constructing_STILL)].BoxColliderSize.Add(new Vector3(0.3f, 0.1f, 0.3f));

            _supportedStructures.Add(typeof(Construction_PIT), new StructureInfo());
            _supportedStructures[typeof(Construction_PIT)].IsRootStructureType = false;
            _supportedStructures[typeof(Construction_PIT)].ColliderType = StructureInfo.RaftStructuresColliderType.Box;
            _supportedStructures[typeof(Construction_PIT)].ContactOffset = 0.00001f;
            _supportedStructures[typeof(Construction_PIT)].BoxColliderCenter.Add(new Vector3(0.0f, 0.2f, 0.0f));
            _supportedStructures[typeof(Construction_PIT)].BoxColliderSize.Add(new Vector3(0.0f, 0.0f, 0.0f));
            _supportedStructures[typeof(Construction_PIT)].RepositionY = 0.15f;


            _supportedStructures.Add(typeof(Constructing_SPIT), new StructureInfo());
            _supportedStructures[typeof(Constructing_SPIT)].IsRootStructureType = false;
            _supportedStructures[typeof(Constructing_SPIT)].ColliderType = StructureInfo.RaftStructuresColliderType.Box;
            _supportedStructures[typeof(Constructing_SPIT)].ContactOffset = 0.01f;
            //size (1.90, 0.12, 0.04)
            //center (0.00, 0.01, 0.01)
            _supportedStructures[typeof(Constructing_SPIT)].BoxColliderSize.Add(new Vector3(1.90f, 0.12f, 0.04f));//new Vector3(1.90f, 0.12f, 0.04f);
            _supportedStructures[typeof(Constructing_SPIT)].BoxColliderCenter.Add(new Vector3(0.0f, 0.01f, 0.01f));//new Vector3(0.0f, 0.01f, 0.01f);
                                                                                                                   //size (0.16, 1.38, 0.14)
                                                                                                                   //center (-0.80, -0.57, 0.01)
            _supportedStructures[typeof(Constructing_SPIT)].BoxColliderSize.Add(new Vector3(0.16f, 0.38f, 0.14f));
            _supportedStructures[typeof(Constructing_SPIT)].BoxColliderCenter.Add(new Vector3(-0.80f, 0.57f, 0.01f));
            //size (0.16, 1.38, 0.14)
            //center (0.81, -0.57, 0.01)
            _supportedStructures[typeof(Constructing_SPIT)].BoxColliderSize.Add(new Vector3(0.16f, 0.38f, 0.14f));
            _supportedStructures[typeof(Constructing_SPIT)].BoxColliderCenter.Add(new Vector3(0.81f, 0.57f, 0.01f));
            _supportedStructures[typeof(Construction_PIT)].RepositionY = 0.25f;

            _supportedStructures.Add(typeof(Constructing_FUEL_STILL_BOILER), new StructureInfo());
            _supportedStructures[typeof(Constructing_FUEL_STILL_BOILER)].IsRootStructureType = false;
            _supportedStructures[typeof(Constructing_FUEL_STILL_BOILER)].ColliderType = StructureInfo.RaftStructuresColliderType.Box;
            _supportedStructures[typeof(Constructing_FUEL_STILL_BOILER)].ContactOffset = 0.01f;

            _supportedStructures.Add(typeof(Constructing_SMOKER), new StructureInfo());
            _supportedStructures[typeof(Constructing_SMOKER)].IsRootStructureType = false;
            _supportedStructures[typeof(Constructing_SMOKER)].ColliderType = StructureInfo.RaftStructuresColliderType.Mesh;
            _supportedStructures[typeof(Constructing_SMOKER)].ContactOffset = 0.01f;
            //new Vector3(0.0f, 0.0f, 0.0f);
            _supportedStructures[typeof(Constructing_SMOKER)].MeshColliderPosition = new Vector3(0.0f, 1.8f, 0.0f);
            //new Vector3(1.0f, 1.0f, 1.0f);
            _supportedStructures[typeof(Constructing_SMOKER)].MeshColliderScale = new Vector3(1.0f, 0.5f, 1.0f);
            // ghost collider box
            // center (-0.09, -0.10, 0.27)
            _supportedStructures[typeof(Constructing_SMOKER)].BoxColliderCenter.Add(new Vector3(-0.09f, 1.80f, 0.27f));
            // size (2.15, 2.71, 1.82)
            _supportedStructures[typeof(Constructing_SMOKER)].BoxColliderSize.Add(new Vector3(1.15f, 1.71f, 0.82f));
            _supportedStructures[typeof(Constructing_SMOKER)].RepositionY = 1.5f;

#warning EXPERIMENTAL
            //_supportedStructures.Add(typeof(ConstructionObject_ITEM_PILE), new StructureInfo());
            //_supportedStructures[typeof(ConstructionObject_ITEM_PILE)].IsRootStructureType = true;
            //_supportedStructures[typeof(ConstructionObject_ITEM_PILE)].ColliderType = StructureInfo.RaftStructuresColliderType.Box;
            //_supportedStructures[typeof(ConstructionObject_ITEM_PILE)].ContactOffset = 0.01f;

            //_supportedStructures.Add(typeof(Farming_PLOT), new StructureInfo());
            //_supportedStructures[typeof(Farming_PLOT)].IsRootStructureType = true;
            //_supportedStructures[typeof(Farming_PLOT)].ColliderType = StructureInfo.RaftStructuresColliderType.Box;
            //_supportedStructures[typeof(Farming_PLOT)].ContactOffset = 0.01f;

#warning EXPERIMENTAL
            //_supportedStructures.Add(typeof(Construction_FOUNDATION), new StructureInfo());
            //_supportedStructures[typeof(Construction_FOUNDATION)].IsRootStructureType = true;
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : WOOD_FOUNDATION(Clone)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : FOUNDATION
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : ATTRIBUTE_TYPE_WOOD INTERACTIVE_TYPE_BUILDING_FOUNDATION
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : WOOD_FOUNDATION(Clone)
#warning EXPERIMENTAL


#warning EXPERIMENTAL
            //_supportedStructures.Add(typeof(Construction_FOUNDATION), new StructureInfo());
            //_supportedStructures[typeof(Construction_FOUNDATION)].IsRootStructureType = true;
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : WOOD_FOUNDATION(Clone)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : FOUNDATION
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : ATTRIBUTE_TYPE_WOOD INTERACTIVE_TYPE_BUILDING_FOUNDATION
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : WOOD_FOUNDATION(Clone)
#warning EXPERIMENTAL

            // LOOM
            //Stranded Deep RaftStructures Mod ACTIVE ghost collider: LOOM
            //Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : LOOM
            //Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : UnityEngine.BoxCollider
            //Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : (0.00, 0.88, 0.00)
            //Stranded Deep RaftStructures Mod ACTIVE ghost collider / collider : (1.58, 1.70, 0.20)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable : Beam.Crafting.Constructing
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable ConnectorLayerMask: 0
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : LOOM(Clone)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : NONE
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : INTERACTIVE_TYPE_STATION_LOOM
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector : LOOM(Clone)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : (0.00, 0.00, 0.00)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector : (0.00, 0.00, 0.00)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector Checks.Count : 2
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector check: LOOM(Clone)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector check: ConnectorCheck_Underwater
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector check: LOOM(Clone)
            //Stranded Deep RaftStructures Mod test: Crafter _placingCraftable my connector check: ConnectorCheck_CheckSnapped
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable my connector maximumTier: None
            //Stranded Deep RaftStructures Mod test : Crafter _placingCraftable snapped connector : null

            // TANNING_RACK
            // FURNACE (?)
            // BRICK STATION
            // PLANK STATION
            // FARMING PLOT (ultimate)
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            logDebugInfo = GUILayout.Toggle(logDebugInfo, "Log debug info (affects performance)");
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        private static bool isOnRaft = false;

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                if (Beam.Game.State == GameState.NEW_GAME
                    || Beam.Game.State == GameState.LOAD_GAME)
                {
                    if (logDebugInfo)
                    {
                        DebugInformation();
                    }

                    if (PlayerRegistry.LocalPlayer != null
                        && PlayerRegistry.LocalPlayer.Movement != null
                        && PlayerRegistry.LocalPlayer.Movement.ActivePlatform != null
                        && PlayerRegistry.LocalPlayer.Movement.ActivePlatform.name.Contains("STRUCTURE_RAFT"))
                    {
                        isOnRaft = true;
                    }
                    else
                    {
                        isOnRaft = false;
                    }
                    if (logDebugInfo)
                    {
                        //Debug.Log("Stranded Deep RaftStructures Mod  : isonraft : " + isOnRaft);
                    }

                    //Debug.Log("Stranded Deep RaftStructures Mod update step 1 (ms) = " + chrono.ElapsedMilliseconds);
                    ActivateRaftConstructibility();
                    //Debug.Log("Stranded Deep RaftStructures Mod update step 2 (ms) = " + chrono.ElapsedMilliseconds);
                    ExtensionsConstructibility();
                    //Debug.Log("Stranded Deep RaftStructures Mod update step 3 (ms) = " + chrono.ElapsedMilliseconds);
                    RepositionGhost();
                    //Debug.Log("Stranded Deep RaftStructures Mod update step 4 (ms) = " + chrono.ElapsedMilliseconds);

                    //if (WorldUtilities.IsWorldLoaded())
                    //{
                    //    //if (_initializeCollisionBoxes)
                    //    //{
                    //    //    InitUpdateCollisionBoxes();
                    //    //}
                    //    //else
                    //    //{
                    //    //    FillUpdateCollisionBoxesQueue();
                    //    //    UpdateCollisionBoxes();
                    //    //}

                    //    //Debug.Log("Stranded Deep RaftStructures Mod update step 5 (ms) = " + chrono.ElapsedMilliseconds);

                    //    //ReattachFoods();
                    //}
                    //Debug.Log("Stranded Deep RaftStructures Mod update step 6 (ms) = " + chrono.ElapsedMilliseconds);
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on update : " + e);
            }
            finally
            {
                if (perfCheck && chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep RaftStructures Mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        private static FieldInfo fi_placingDistance = typeof(Constructing).GetField("_placingDistance", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_proximityDistance = typeof(Constructing).GetField("_proximityDistance", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_proximityChecking = typeof(Constructing).GetField("_proximityChecking", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo fi_proximityCheck = typeof(Constructing).GetField("_proximityCheck", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void ExtensionsConstructibility()
        {
            try
            {
                IPlayer p = PlayerRegistry.LocalPlayer;
                if (p != null)
                {
                    IPlaceable ipl = fi_placingCraftable.GetValue(p.Crafter) as IPlaceable;
                    if (ipl != null)
                    {
                        if (ipl is Construction_PIT
                            || ipl is Constructing_SMOKER
                            || ipl is Constructing_SPIT
                            || ipl is Constructing_FUEL_STILL_BOILER)
                        {
                            Constructing cons = ipl as Constructing;
                            StructureInfo si = _supportedStructures[ipl.GetType()];
                            Connector conn = fi_constructingConnector.GetValue(cons) as Connector;

                            Ghost crafterghost = fi_crafterghost.GetValue(p.Crafter) as Ghost;
                            GhostCollider activeC = fi_activeCollider.GetValue(crafterghost) as GhostCollider;
                            if (activeC != null)
                            {
                                int currentg = 0;
                                foreach (Collider collider in activeC.Colliders)
                                {
                                    Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + collider.GetType());
                                    if (collider is SphereCollider)
                                    {
                                        SphereCollider sc = collider as SphereCollider;
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ghost collider radius " + sc.radius);
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ghost collider center " + sc.center);
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ghost collider contactOffset " + sc.contactOffset);
                                        sc.contactOffset = si.ContactOffset;
                                        if (si.SphereColliderCenter.Count > currentg)
                                            sc.center = si.SphereColliderCenter[currentg];
                                        if (si.SphereColliderRadius.Count > currentg)
                                            sc.radius = si.SphereColliderRadius[currentg];
                                    }
                                    else if (collider is BoxCollider)
                                    {
                                        BoxCollider bc = collider as BoxCollider;
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ghost collider size " + bc.size);
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ghost collider center " + bc.center);
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ghost collider contactOffset " + bc.contactOffset);
                                        bc.contactOffset = si.ContactOffset;
                                        if (si.BoxColliderCenter.Count > currentg)
                                            bc.center = si.BoxColliderCenter[currentg];
                                        if (si.BoxColliderSize.Count > currentg)
                                            bc.size = si.BoxColliderSize[currentg];
                                    }
                                    else if (collider is MeshCollider)
                                    {
                                        MeshCollider mc = collider as MeshCollider;
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " mesh collider position " + mc.transform.position);
                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " mesh collider localScale " + mc.transform.localScale);
                                        mc.contactOffset = si.ContactOffset;
                                        mc.transform.position = si.MeshColliderPosition;
                                        mc.transform.localScale = si.MeshColliderScale;
                                    }
                                    currentg++;
                                }
                            }

                            //Collider[] cc = cons.GetComponents<Collider>();
                            //int current = 0;
                            //foreach (Collider c in cc)
                            //{
                            //    Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider type " + c.GetType() + " / collider name " + c.name);
                            //    if (c is SphereCollider)
                            //    {
                            //        SphereCollider sc = c as SphereCollider;
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider radius " + sc.radius);
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider center " + sc.center);
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider contactOffset " + sc.contactOffset);
                            //        //sc.contactOffset = si.ContactOffset;
                            //        //if (si.SphereColliderCenter.Count > current)
                            //        //    sc.center = si.SphereColliderCenter[current];
                            //        //if (si.SphereColliderRadius.Count > current)
                            //        //    sc.radius = si.SphereColliderRadius[current];
                            //    }
                            //    else if (c is BoxCollider)
                            //    {
                            //        BoxCollider bc = c as BoxCollider;
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider size " + bc.size);
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider center " + bc.center);
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " collider contactOffset " + bc.contactOffset);
                            //        //bc.contactOffset = si.ContactOffset;
                            //        //if (si.BoxColliderCenter.Count > current)
                            //        //    bc.center = si.BoxColliderCenter[current];
                            //        //if (si.BoxColliderSize.Count > current)
                            //        //    bc.size = si.BoxColliderSize[current];
                            //    }
                            //    else if (c is MeshCollider)
                            //    {
                            //        MeshCollider mc = c as MeshCollider;
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " mesh collider position " + mc.transform.position);
                            //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " mesh collider localScale " + mc.transform.localScale);
                            //        //mc.contactOffset = si.ContactOffset;
                            //        //mc.transform.position = si.MeshColliderPosition;
                            //        //mc.transform.localScale = si.MeshColliderScale;
                            //    }
                            //    // deal only with the first collider like for the spit which has 3
                            //    current++;
                            //    //break;
                            //}

                            if (isOnRaft)
                            {
                                fi_proximityDistance.SetValue(ipl, 0.0f);
                                Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ProximityDistance " + fi_proximityDistance.GetValue(ipl));
                                fi_proximityChecking.SetValue(ipl, ConstructingProximityCheckMode.None);
                                Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ConstructingProximityCheckMode " + fi_proximityChecking.GetValue(ipl));
                                NoConstructingProximityCheck pcheck = new NoConstructingProximityCheck();
                                fi_proximityCheck.SetValue(ipl, pcheck);

                                //if (cons.SnappedConnector != null)
                                //{
                                //    //    Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " snapped connector " +cons.SnappedConnector.name);
                                //    //    Constructing futureparent = cons.SnappedConnector.Constructing;
                                //    //    if (futureparent != null)
                                //    //    {
                                //    //        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " future parent " + futureparent.GetType().Name);
                                //    //        Connector futureParentConnector = fi_myConnector.GetValue(futureparent) as Connector;
                                //    //        futureparent.transform.position = new Vector3(0, 2.0f, 0);
                                //    //    }
                                //}
                            }
                            else
                            {
                                fi_proximityDistance.SetValue(ipl, 4.5f);
                                Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ProximityDistance " + fi_proximityDistance.GetValue(ipl));
                                fi_proximityChecking.SetValue(ipl, ConstructingProximityCheckMode.Default);
                                Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " ConstructingProximityCheckMode " + fi_proximityChecking.GetValue(ipl));
                                DefaultConstructingProximityCheck pcheck = new DefaultConstructingProximityCheck();
                                fi_proximityCheck.SetValue(ipl, pcheck);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on RepositionGhost : " + e);
            }
        }

        private static void RepositionGhost()
        {
            try
            {
                IPlayer p = PlayerRegistry.LocalPlayer;
                if (p != null)
                {
                    IPlaceable ipl = fi_placingCraftable.GetValue(p.Crafter) as IPlaceable;
                    if (ipl != null)
                    {
                        if (ipl is Constructing)
                        {
                            Constructing construct = ipl as Constructing;
                            if (construct.SnappedConnector != null
                                && construct.SnappedConnector.Type == ConnectorType.VEHICLE_MAST)
                            {
                                //Debug.Log("Stranded Deep RaftStructures Mod : RepositionGhost : " + construct.SnappedConnector.Type);
                                //Debug.Log("Stranded Deep RaftStructures Mod : RepositionGhost : " + construct.SnappedConnector.transform.position);
                                //Debug.Log("Stranded Deep RaftStructures Mod : RepositionGhost : " + construct.SnappedConnector.transform.localPosition);

                                if (construct is Constructing_SAIL)
                                {
                                    construct.SnappedConnector.transform.localPosition = new Vector3(0, 1.5f, 0);
                                }
                                else if (_supportedStructures.ContainsKey(construct.GetType())
                                    && _supportedStructures[construct.GetType()].IsRootStructureType)
                                {
                                    StructureInfo si = _supportedStructures[construct.GetType()];

                                    Collider[] cc = construct.GetComponents<Collider>();
                                    int current = 0;
                                    foreach (Collider c in cc)
                                    {
                                        if (c is SphereCollider
                                            && si.ColliderType == StructureInfo.RaftStructuresColliderType.Sphere)
                                        {
                                            ((SphereCollider)c).contactOffset = si.ContactOffset;
                                            if (si.SphereColliderCenter.Count > current)
                                                ((SphereCollider)c).center = si.SphereColliderCenter[current];
                                            if (si.SphereColliderRadius.Count > current)
                                                ((SphereCollider)c).radius = si.SphereColliderRadius[current];
                                        }
                                        else if(c is BoxCollider
                                            && si.ColliderType == StructureInfo.RaftStructuresColliderType.Box)
                                        {
                                            ((BoxCollider)c).contactOffset = si.ContactOffset;
                                            if (si.BoxColliderCenter.Count > current)
                                                ((BoxCollider)c).center = si.BoxColliderCenter[current];
                                            if (si.BoxColliderSize.Count > current)
                                                ((BoxCollider)c).size = si.BoxColliderSize[current];
                                        }
                                        else if (c is MeshCollider)
                                        {
                                            MeshCollider mc = c as MeshCollider;
                                            mc.contactOffset = si.ContactOffset;
                                            mc.transform.position = si.MeshColliderPosition;
                                            mc.transform.localScale = si.MeshColliderScale;
                                        }
                                    }
                                    construct.SnappedConnector.transform.localPosition = si.GhostPosition;
                                    current++;
                                }
                                //// campfire = new Vector3(0, 0.35, 0) : -1.2f ?
                                //// still = new Vector3(0, 0.1, 0) : -1.4f ?
                                //else if (construct is Construction_CAMPFIRE)
                                //{
                                //    Construction_CAMPFIRE campfire = construct as Construction_CAMPFIRE;
                                //    Collider[] cc = campfire.GetComponents<Collider>();
                                //    foreach (Collider c in cc)
                                //    {
                                //        ((SphereCollider)c).contactOffset = 0.01f;//0.01
                                //        ((SphereCollider)c).center = new Vector3(0, 0.1f, 0);
                                //        ((SphereCollider)c).radius = 0.05f;//0.6
                                //    }
                                //    construct.SnappedConnector.transform.localPosition = new Vector3(0, 0.35f, 0); // ghost position
                                //}
                                //else if (construct is Constructing_STILL)
                                //{
                                //    Constructing_STILL campfire = construct as Constructing_STILL;
                                //    Collider[] cc = campfire.GetComponents<Collider>();
                                //    foreach (Collider c in cc)
                                //    {
                                //        ((BoxCollider)c).contactOffset = 0.01f;//0.01
                                //        ((BoxCollider)c).size = new Vector3(0.2f, 0.2f, 0.2f);
                                //    }
                                //    construct.SnappedConnector.transform.localPosition = new Vector3(0, 0.1f, 0); // ghost position
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on RepositionGhost : " + e);
            }
        }


        //this._snappedConnector.Constructing.Structure.DisableGhostCollisionOnSnap

        private static void ActivateRaftConstructibility()
        {
            try
            {
                IPlayer p = PlayerRegistry.LocalPlayer;
                if (p != null)
                {
                    Ghost crafterghost = fi_crafterghost.GetValue(p.Crafter) as Ghost;
                    if (crafterghost != null)
                    {
                        IPlaceable ipl = fi_placingCraftable.GetValue(p.Crafter) as IPlaceable;
                        if (ipl != null)
                        {
                            //Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : Crafter _placingCraftable : " + ipl.GetType().FullName);

                            if (ipl is Constructing)
                            {
                                if (_supportedStructures.ContainsKey(ipl.GetType())
                                    && _supportedStructures[ipl.GetType()].IsRootStructureType)
                                {
                                    //Debug.Log("Stranded Deep RaftStructures ActivateRaftConstructibility test : Crafter position " + crafterghost.transform.position);
                                    //Debug.Log("Stranded Deep RaftStructures ActivateRaftConstructibility test : Crafter localposition " + crafterghost.transform.localPosition);

                                    Constructing cons = ipl as Constructing;
                                    if (fi_constructingConnector.GetValue(cons) == null || ((Connector)fi_constructingConnector.GetValue(cons)).Type != ConnectorType.VEHICLE_MAST)
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : Crafter _placingCraftable : " + ipl.GetType().FullName);

                                        fi_constructingColliderType.SetValue(ipl, GhostCollider.ColliderType.VEHICLE_SAIL);
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : changed ColliderType to " + GhostCollider.ColliderType.VEHICLE_SAIL);

                                        Connector newConn = cons.gameObject.AddComponent<Connector>();
                                        newConn.Constructing = cons;
                                        fi_connectortype.SetValue(newConn, ConnectorType.VEHICLE_MAST);
                                        fi_constructingConnector.SetValue(cons, newConn);

                                        crafterghost.Initialize(ipl as ICraftable);
                                    }

                                    Connector myConnector = fi_constructingConnector.GetValue(cons) as Connector;
                                    bool hasCustomConnectorCheck = false;
                                    ConnectorCheck_CheckSnapped csnapped = null;
                                    ConnectorCheck_Underwater cunderwater = null;
                                    foreach (ConnectorCheck ccheck in myConnector.Checks)
                                    {
                                        if (ccheck is RaftStructuresConnectorCheck)
                                        {
                                            hasCustomConnectorCheck = true;
                                            ((RaftStructuresConnectorCheck)ccheck).IsOnRaft = isOnRaft;
                                        }
                                        if (ccheck is ConnectorCheck_CheckSnapped)
                                        {
                                            csnapped = ccheck as ConnectorCheck_CheckSnapped;
                                        }

                                        if (ccheck is ConnectorCheck_Underwater)
                                        {
                                            cunderwater = ccheck as ConnectorCheck_Underwater;
                                        }
                                    }
                                    if (!hasCustomConnectorCheck)
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : changed connector check to RaftStructuresConnectorCheck");
                                        List<ConnectorCheck> temp = new List<ConnectorCheck>();
                                        RaftStructuresConnectorCheck customCheck = cons.gameObject.AddComponent<RaftStructuresConnectorCheck>();
                                        customCheck.ConnectorCheck_CheckSnapped = csnapped;
                                        customCheck.ConnectorCheck_Underwater = cunderwater;
                                        temp.Add(customCheck);
                                        fi_checks.SetValue(myConnector, temp.ToArray());
                                    }
                                }
                            }
                            else if (ipl is ConstructionObject)
                            {
                                if (_supportedStructures.ContainsKey(ipl.GetType())
                                    && _supportedStructures[ipl.GetType()].IsRootStructureType)
                                {
                                    StructureInfo si = _supportedStructures[ipl.GetType()];

                                    Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : Crafter _placingCraftable : " + ipl.GetType().FullName);
                                    Debug.Log("Stranded Deep RaftStructures ActivateRaftConstructibility test : Crafter position " + crafterghost.transform.position);
                                    Debug.Log("Stranded Deep RaftStructures ActivateRaftConstructibility test : Crafter localposition " + crafterghost.transform.localPosition);

                                    ConstructionObject consobj = ipl as ConstructionObject;
                                    if (fi_constructionObjectConnector.GetValue(consobj) == null || ((Connector)fi_constructionObjectConnector.GetValue(consobj)).Type != ConnectorType.VEHICLE_MAST)
                                    {
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : Crafter _placingCraftable : " + ipl.GetType().FullName);

                                        fi_constructionObjectColliderType.SetValue(ipl, GhostCollider.ColliderType.VEHICLE_SAIL);
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : changed ColliderType to " + GhostCollider.ColliderType.VEHICLE_SAIL);

                                        Connector newConn = consobj.gameObject.AddComponent<Connector>();
                                        //newConn.Constructi = consobj;
                                        fi_connectortype.SetValue(newConn, ConnectorType.VEHICLE_MAST);
                                        fi_constructionObjectConnector.SetValue(consobj, newConn);

                                        crafterghost.Initialize(ipl as ICraftable);
                                    }

                                    //Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " tests");
                                    Connector conn = fi_constructionObjectConnector.GetValue(consobj) as Connector;
                                    GhostCollider activeC = fi_activeCollider.GetValue(crafterghost) as GhostCollider;
                                    if (activeC != null
                                        && activeC.Colliding)
                                    {
                                        FieldInfo fi_colliding = typeof(GhostCollider).GetField("_colliding", BindingFlags.NonPublic | BindingFlags.Instance);
                                        HashSet<Collider> collidersColliding = fi_colliding.GetValue(activeC) as HashSet<Collider>;
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : colliding count : " + collidersColliding.Count);
                                        foreach (Collider coll in collidersColliding)
                                        {
                                            Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " colliding collider : " + coll.name);
                                            Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " colliding collider : " + coll.transform.position);
                                            Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " colliding collider : " + coll.transform.localPosition);
                                            if (coll.attachedRigidbody != null)
                                            {
                                                Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " colliding collider : " + coll.attachedRigidbody.transform.position);
                                            }
                                            if (coll.gameObject != null)
                                            {
                                                Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " colliding collider : " + coll.gameObject.name);
                                            }
                                        }
                                        Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : " + ipl.GetType() + " connector colliding, try to reposition ghost");
                                        crafterghost.transform.position = collidersColliding.First().transform.position;
                                        crafterghost.transform.localPosition = collidersColliding.First().transform.localPosition;
                                        crafterghost.transform.rotation = collidersColliding.First().transform.rotation;
                                    }

                                    //                                    else if (ipl is Farming_PLOT
                                    //                            || ipl is ConstructionObject_ITEM_PILE)
                                    //                                    {
                                    //#warning EXPERIMENTAL
                                    //                                        Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " tests");
                                    //                                        ConstructionObject consobj = ipl as ConstructionObject;
                                    //                                        StructureInfo si = _supportedStructures[ipl.GetType()];
                                    //                                        Connector conn = fi_constructionObjectConnector.GetValue(consobj) as Connector;

                                    //                                        Ghost crafterghost = fi_crafterghost.GetValue(p.Crafter) as Ghost;
                                    //                                        GhostCollider activeC = fi_activeCollider.GetValue(crafterghost) as GhostCollider;
                                    //                                        if (activeC != null
                                    //                                            && activeC.Colliding)
                                    //                                        {
                                    //                                            Debug.Log("Stranded Deep RaftStructures Mod ExtensionsConstructibility : " + ipl.GetType() + " connector colliding, try to reposition ghost");
                                    //                                            crafterghost.transform.position = activeC.transform.position;
                                    //                                            crafterghost.transform.rotation = activeC.transform.rotation;
                                    //                                        }
                                    //                                    }

                                    Connector myConnector = fi_constructionObjectConnector.GetValue(consobj) as Connector;
                                    if (myConnector.Checks.Count > 0)
                                    {
                                        bool hasCustomConnectorCheck = false;
                                        ConnectorCheck_CheckSnapped csnapped = null;
                                        ConnectorCheck_Underwater cunderwater = null;
                                        foreach (ConnectorCheck ccheck in myConnector.Checks)
                                        {
                                            if (ccheck is RaftStructuresConnectorCheck)
                                            {
                                                hasCustomConnectorCheck = true;
                                                ((RaftStructuresConnectorCheck)ccheck).IsOnRaft = isOnRaft;
                                            }
                                            if (ccheck is ConnectorCheck_CheckSnapped)
                                            {
                                                csnapped = ccheck as ConnectorCheck_CheckSnapped;
                                            }

                                            if (ccheck is ConnectorCheck_Underwater)
                                            {
                                                cunderwater = ccheck as ConnectorCheck_Underwater;
                                            }
                                        }
                                        if (!hasCustomConnectorCheck)
                                        {
                                            Debug.Log("Stranded Deep RaftStructures Mod ActivateRaftConstructibility : changed connector check to RaftStructuresConnectorCheck");
                                            List<ConnectorCheck> temp = new List<ConnectorCheck>();
                                            RaftStructuresConnectorCheck customCheck = consobj.gameObject.AddComponent<RaftStructuresConnectorCheck>();
                                            customCheck.ConnectorCheck_CheckSnapped = csnapped;
                                            customCheck.ConnectorCheck_Underwater = cunderwater;
                                            temp.Add(customCheck);
                                            fi_checks.SetValue(myConnector, temp.ToArray());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on ActivateRaftConstructibility : " + e);
            }
        }

        private static void ReattachFoods()
        {
            try
            { 
                Construction_CAMPFIRE[] instances = Game.FindObjectsOfType<Construction_CAMPFIRE>();
                foreach (Construction_CAMPFIRE currentInstance in instances)
                {
                    ReattachFood(currentInstance);
                }
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep RaftStructures Mod : error on ReattachFoods : " + e);
            }
        }

        internal static void ReattachFood(Construction_CAMPFIRE campfire)
        {
            if (!campfire.transform.parent.name.Contains("STRUCTURE_RAFT"))
            {
                return;
            }
            List<Cooking> cookables = fi_cookables.GetValue(campfire) as List<Cooking>;
            //Debug.Log("Stranded Deep RaftStructures Mod : looking for attached food " + cookables.Count + " cookables found");
            foreach (Cooking cooking in cookables)
            {
                //Debug.Log("Stranded Deep RaftStructures Mod : attached food : " + cooking.Food.ReferenceId);
                //Debug.Log("Stranded Deep RaftStructures Mod : attached food parent name : " + cooking.Food.transform.parent.name);
                if (campfire.isActiveAndEnabled
                    && cooking.Food.transform.parent.name.Contains("SaveContainer")
                    && !System.Object.ReferenceEquals(cooking.Food.transform.parent, campfire.transform))
                {
                    AttachFoodToParentTransform(campfire, cooking.Food);
                    //Debug.Log("Stranded Deep RaftStructures Mod : food : attaching parent for " + cooking.Food.ReferenceId + " / parent " + campfire.ReferenceId);
                    //cooking.Food.transform.parent = campfire.transform;
                    //cooking.Food.DetachedEvent -= Food_DetachedEvent;
                    //cooking.Food.DetachedEvent += Food_DetachedEvent;
                }
            }
        }

        internal static void AttachFoodToParentTransform(Construction_CAMPFIRE campfire, InteractiveObject_FOOD food)
        {
            if (campfire.isActiveAndEnabled
                && !System.Object.ReferenceEquals(food.transform.parent, campfire.transform))
            {
                Debug.Log("Stranded Deep RaftStructures Mod : food : attaching parent for " + food.ReferenceId + " / parent " + campfire.ReferenceId);
                food.transform.parent = campfire.transform;
                //food.DetachedEvent -= Food_DetachedEvent;
                //food.DetachedEvent += Food_DetachedEvent;
            }
        }

        internal static void DetachFood(InteractiveObject_FOOD food)
        {
            food.transform.parent = StrandedWorld.Instance.SaveDynamicContainer;
        }

        //private static void Food_DetachedEvent(IAttachable sender)
        //{
        //    Debug.Log("Stranded Deep RaftStructures Mod : food detached from spit 1");
        //    InteractiveObject_FOOD food = sender as InteractiveObject_FOOD;
        //    food.transform.parent = StrandedWorld.Instance.SaveDynamicContainer;
        //    food.DetachedEvent -= Food_DetachedEvent;
        //}

        private static void Reset()
        {
            //worldLoaded = false;
            //_handledCollisionBoxes.Clear();
        }

        

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach (string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            if (tokens[0].Contains("logDebugInfo"))
                            {
                                logDebugInfo = bool.Parse(tokens[1]);
                            }
                        }
                    }
                }
            }
        }

        private static void WriteConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("logDebugInfo=" + logDebugInfo + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
