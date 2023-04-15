using Beam;
using Beam.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeepTweaksMod
{
    public class BuoyancyHandler : CollectionHandler<InteractiveObject>
    {
        private FieldInfo fi_buoyancyDensity = typeof(Buoyancy).GetField("_density", BindingFlags.NonPublic | BindingFlags.Instance);
        private PropertyInfo fi_LootSpawned = typeof(Interactive_STORAGE).GetProperty("LootSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
        private FieldInfo fi_rigidbody = typeof(Buoyancy).GetField("_rigidbody", BindingFlags.NonPublic | BindingFlags.Instance);

        public BuoyancyHandler()
            : base(2, true)
        {
        }

        protected override void HandleOne(InteractiveObject io)
        {
            //Debug.Log("Stranded Deep Tweaks Mod : " + io.name);
            Buoyancy buoy = io.GetComponent<Buoyancy>();
            if (buoy == null
                && (io.name.Contains("STICK")
                || io.name.Contains("COCONUT")
                || io.name.Contains("SCRAP_PLANK")
                || io.name.Contains("CONTAINER_CRATE")
                || io.name.Contains("FROND")
                || io.name.Contains("PALM_TOP")
                || io.name.Contains("PALM_LOG")
                || io.name.Contains("PADDLE")
                || io.name.Contains("FUELCAN")
                || io.name.Contains("LEAVES_FIBROUS")
                || io.name.Contains("WOLLIE")
                || io.name.Contains("COCONUT_FLASK")
                || io.name.Contains("MEDICAL")))
            {
                if (io.name.Contains("CONTAINER_CRATE")
                    && io is Interactive_STORAGE)
                {
                    Interactive_STORAGE istor = io as Interactive_STORAGE;
                    //Debug.Log("Stranded Deep Tweaks Mod : CRATE spawnItemLoot = " + fi_LootSpawned.GetValue(istor));
                    if (!(bool)fi_LootSpawned.GetValue(istor))
                    {
                        return;
                    }
                }

                io.gameObject.SetActive(false);
                Debug.Log("Stranded Deep Tweaks Mod : adding buoyancy to " + io.name);
                Buoyancy newbuoy = io.gameObject.AddComponent<Buoyancy>();
                fi_buoyancyDensity.SetValue(newbuoy, 600.0f);
                Rigidbody rb = fi_rigidbody.GetValue(newbuoy) as Rigidbody;
                if (rb == null)
                {
                    //Debug.Log("Stranded Deep Tweaks Mod : buoyancy rigidbody is null");
                    fi_rigidbody.SetValue(newbuoy, io.rigidbody);
                }
                io.gameObject.SetActive(true);
            }
        }
    }
}
