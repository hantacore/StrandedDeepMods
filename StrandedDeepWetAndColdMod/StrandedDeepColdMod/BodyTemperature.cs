using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StrandedDeepWetAndColdMod
{
    public class BodyTemperature : PlayerEffect
    {
        public static string BODY_TEMPERATURE = "Body Temperature Monitor";
        public static string FEVER_INDICATOR_WORKAROUND = "*";

        private FieldInfo dummyField = typeof(PlayerEffect).GetField("_temperaturePerHour", BindingFlags.NonPublic | BindingFlags.Instance);
        private FieldInfo displayNameField = typeof(PlayerEffect).GetField("_displayName", BindingFlags.NonPublic | BindingFlags.Instance);
        System.Random r = new System.Random();

        public float CurrentTemperature
        {
            get
            {
                return TemperaturePerHour;
            }
            set
            {
                dummyField.SetValue(this, value);
            }
        }

        public bool HasHadFever
        {
            get
            {
                return DisplayName.Contains(FEVER_INDICATOR_WORKAROUND);
            }
            set
            {
                if (value)
                {
                    displayNameField.SetValue(this, String.Concat(BODY_TEMPERATURE, " ", FEVER_INDICATOR_WORKAROUND));
                }
                else
                {
                    displayNameField.SetValue(this, BODY_TEMPERATURE);
                }
            }
        }

        public bool IsHot
        {
            get
            {
                return CurrentTemperature >= ParameterValues.HOT_THRESHOLD;
            }
        }

        public bool IsCold
        {
            get
            {
                return CurrentTemperature <= ParameterValues.COLD_THRESHOLD;
            }
        }

        //public int millisecondsBeforeSickness = 60 * 60 * 1000; // 1 ingame hour
        public bool PlayRelief { get; set; }
        public bool PlayShiver { get; set; }
        public bool PlayedShiver { get; set; }

        public BodyTemperature()
            : base(BODY_TEMPERATURE, BODY_TEMPERATURE, true, 0, 0, 0, 0)
        {

        }

        public void Reset()
        {
            //CurrentTemperature = 0;
            PlayRelief = false;
            PlayShiver = false;
            PlayedShiver = false;
            // Defines the cold start
            StartedTime = DateTime.MinValue;
        }

        public bool CheckSick(bool isSleeping, float energy)
        {
            if (!IsCold)
                return false;

            TimeSpan delta = GameTime.Now.Subtract(StartedTime);
            if (delta.TotalMilliseconds >= ParameterValues.SICK_INGAME_DELAY)
            {
                if (isSleeping && r.Next(0, ParameterValues.MAX_ENERGY + 1) < (250 - CurrentTemperature))
                {
                    return true;
                }
                else if (r.Next(Math.Max(0, Mathf.Clamp(ParameterValues.MAX_ENERGY - (int)energy, 0, ParameterValues.MAX_ENERGY)), ParameterValues.MAX_ENERGY + 1) < (250 - CurrentTemperature))
                {
                    return true;
                }
            }

            return false;
        }

        public void InitFromLoaded(PlayerEffect pe)
        {
            displayNameField.SetValue(this, pe.DisplayName);
            CurrentTemperature = (int)pe.TemperaturePerHour;
            StartedTime = pe.StartedTime;
        }
    }
}
