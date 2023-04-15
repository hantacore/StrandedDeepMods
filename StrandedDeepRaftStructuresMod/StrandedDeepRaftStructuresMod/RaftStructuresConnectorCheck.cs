using Beam;
using Beam.Crafting;
using Beam.Utilities;
using Ceto;
using System.Reflection;
using UnityEngine;

namespace StrandedDeepRaftStructuresMod
{
    public class RaftStructuresConnectorCheck : ConnectorCheck
    {
        public ConnectorCheck_CheckSnapped ConnectorCheck_CheckSnapped { get; set; }

        public ConnectorCheck_Underwater ConnectorCheck_Underwater { get; set; }

        public bool IsOnRaft { get; set; }

        private static FieldInfo fi_ignoredSnappedObjectIfConnected = typeof(ConnectorCheck_CheckSnapped).GetField("_ignoredSnappedObjectIfConnected", BindingFlags.Instance | BindingFlags.NonPublic);

        public RaftStructuresConnectorCheck()
        {
            //_layerMask = LayerMask.NameToLayer();
        }

        public override bool Check(Beam.Crafting.Placement placement, Connector connector)
        {
            //Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck Check called");
            if (ConnectorCheck_CheckSnapped == null
                || ConnectorCheck_Underwater == null)
            {
                //Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck Checks null");
                return true;
            }

            //Debug.Log("Stranded Deep RaftStructures Mod test : changed ConnectorCheck_CheckSnapped to _ignoredSnappedObjectIfConnected : " + IsOnRaft);
            fi_ignoredSnappedObjectIfConnected.SetValue(ConnectorCheck_CheckSnapped, IsOnRaft);

            //if (connector != null)
            //    Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck connector name : " + connector.name);
            //else
            //    Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck connector null");

            //if (placement != null)
            //    Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck placement position : " + placement.Position);
            //else
            //    Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck placement null");

            bool snapped = ConnectorCheck_CheckSnapped.Check(placement, connector);
            if (connector == null && snapped)
            {
                bool underwater = ConnectorCheck_Underwater.Check(placement, connector);
                //Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck connector underwater : " + underwater);
                return underwater;
            }

            //Debug.Log("Stranded Deep RaftStructures Mod test : RaftStructuresConnectorCheck connector snapped : " + snapped);
            return snapped;
        }
    }
}
