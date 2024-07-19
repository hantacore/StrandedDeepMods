using Beam;
using Beam.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StrandedDeepWetAndColdMod
{
    public class Wet : PlayerEffect
    {
        public static string WET = "Wet";

        private FieldInfo dummyField = typeof(PlayerEffect).GetField("_temperaturePerHour", BindingFlags.NonPublic | BindingFlags.Instance);

        public int Wetness
        {
            get
            {
                return (int)TemperaturePerHour;
            }
            set
            {
                dummyField.SetValue(this, value);
            }
        }

        public Wet()
            : base(WET, WET, true, 0, 0, 0, 0)
        {

        }

        public void InitFromLoaded(PlayerEffect pe)
        {
            Wetness = (int)pe.TemperaturePerHour;
            StartedTime = pe.StartedTime;
            PositiveEffect = true;
        }
    }
}
