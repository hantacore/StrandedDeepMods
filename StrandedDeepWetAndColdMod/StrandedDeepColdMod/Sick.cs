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
    public class Sick : PlayerEffect, IPlayerEffect
    {
        private FieldInfo dummyField = typeof(PlayerEffect).GetField("_temperaturePerHour", BindingFlags.NonPublic | BindingFlags.Instance);

        public static string SICK = "Sick";

        System.Random r = new System.Random();

        public int millisecondsBeforeHeal = 10000;//8 * 60 * 60 * 1000; // 8 ingame hour
        public int millisecondsBeforeFever = 60 * 60 * 1000; // default is 1 ingame hour

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

        public Sick()
            : base(SICK, SICK, false, 0, 0, 0, 0)
        {
            
        }

        public void Reset()
        {
            PercentHealed = 100;
            millisecondsBeforeFever = (r.Next(4, 13) * 10) * 60 * 1000; // randome time between 40 and 120 ingame minutes
            StartedTime = GameTime.Now;
        }

        public bool CheckHealed(IPlayer p, BodyTemperature bodytemperatureEffect, Fever feverEffect, bool housingSheltered, int elapsedMinutes)
        {
            if (p.Statistics.HasStatusEffect(feverEffect))
            {
                return false;
            }
            else if (bodytemperatureEffect.IsCold)
            {
                // if player is still cold after fever delta, he gets fever
                TimeSpan delta = GameTime.Now.Subtract(StartedTime);
                if (!bodytemperatureEffect.HasHadFever && delta.TotalMilliseconds >= millisecondsBeforeFever)
                {
                    // might get fever
                    Debug.Log("Stranded Deep Wet and Cold Mod : caught a fever");
                    if (Main.worldLoaded)
                    {
                        Main.ShowSubtitles(p, Main.FEVER_MESSAGE);
                    }
                    feverEffect.Reset();
                    p.Statistics.ApplyStatusEffect(feverEffect);
                    bodytemperatureEffect.HasHadFever = true;
                    return false;
                }
            }
            else
            {
                if (housingSheltered)
                {
                    // without cold, heal 5x faster
                    PercentHealed -= elapsedMinutes * ParameterValues.SICK_HEAL_HOUSE_MULTIPLICATOR;
                    if (PercentHealed <= 0)
                    {
                        bodytemperatureEffect.HasHadFever = false;
                        return true;
                    }
                }
                else
                {
                    // without cold
                    PercentHealed -= elapsedMinutes;
                    if (PercentHealed <= 0)
                    {
                        bodytemperatureEffect.HasHadFever = false;
                        return true;
                    }
                }
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
