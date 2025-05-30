﻿using Beam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StrandedDeepWetAndColdMod
{
    public class Fever : PlayerEffect, IPlayerEffect
    {
        private FieldInfo dummyField = typeof(PlayerEffect).GetField("_temperaturePerHour", BindingFlags.NonPublic | BindingFlags.Instance);

        public static string FEVER = "Fever";

        public float PercentHealed
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

        public Fever()
            : base(FEVER, FEVER, false, 0, 0, 0, 0)
        {
        }

        public void Reset()
        {
            PercentHealed = ParameterValues.FEVER_HEAL_INGAME_DELAY_MINUTES;
            StartedTime = GameTime.Now;
        }

        public bool CheckHealed(IPlayer p, BodyTemperature bodytemperatureEffect, bool housingSheltered, int elapsedMinutes)
        {
            if (bodytemperatureEffect.IsCold)
            {
                return false;
            }

            if (housingSheltered)
            {
                // without cold, heal 5x faster
                PercentHealed -= elapsedMinutes * ParameterValues.FEVER_HEAL_HOUSE_MULTIPLICATOR_PER_MINUTE;
                if (PercentHealed <= 0)
                    return true;
            }
            else
            {
                // without cold
                PercentHealed -= elapsedMinutes;
                if (PercentHealed <= 0)
                    return true;
            }

            return false;
        }

        public void InitFromLoaded(PlayerEffect pe)
        {
            StartedTime = pe.StartedTime;
            PercentHealed = pe.TemperaturePerHour;
        }
    }
}
