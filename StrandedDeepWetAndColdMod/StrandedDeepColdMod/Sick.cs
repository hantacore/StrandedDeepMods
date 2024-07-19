using Beam;
using Beam.Serialization.Json;
using StrandedDeepModsUtils;
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

        public int MillisecondsBeforeHeal = ParameterValues.HEAL_INGAME_DELAY_MINUTES;
        public int MillisecondsBeforeFever = ParameterValues.FEVER_INGAME_DELAY;

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
            PercentHealed = ParameterValues.HEAL_INGAME_DELAY_MINUTES;
            MillisecondsBeforeFever = (int)(((float)r.Next(8, 12) / 10f) * (float)ParameterValues.FEVER_INGAME_DELAY); // random time between 19 and 28 ingame hours
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
                if (!bodytemperatureEffect.HasHadFever && delta.TotalMilliseconds >= MillisecondsBeforeFever)
                {
                    // might get fever
                    Debug.Log("Stranded Deep "+Main.MODNAME+" Mod : caught a fever");
                    if (WorldUtilities.IsWorldLoaded())
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
                    PercentHealed -= elapsedMinutes * ParameterValues.SICK_HEAL_HOUSE_MULTIPLICATOR_PER_MINUTE;
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
