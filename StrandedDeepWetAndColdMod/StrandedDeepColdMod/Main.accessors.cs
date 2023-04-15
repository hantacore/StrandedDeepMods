using Beam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepWetAndColdMod
{
    public static partial class Main
    {
        #region statistics accessors

        private static float GetWater(IPlayer p)
        {
            if (p == null || p.Statistics == null)
                return 0;

            return (float)playerStatisticsWaterField.GetValue(p.Statistics);
        }

        private static void SetWater(IPlayer p, float water)
        {
            if (p == null)
                return;

            playerStatisticsWaterField.SetValue(p.Statistics, water);
        }

        private static float GetCalories(IPlayer p)
        {
            if (p == null || p.Statistics == null)
                return 0;

            return (float)playerStatisticsCaloriesField.GetValue(p.Statistics);
        }

        private static void SetCalories(IPlayer p, float calories)
        {
            if (p == null)
                return;

            playerStatisticsCaloriesField.SetValue(p.Statistics, calories);
        }

        private static float GetSleep(IPlayer p)
        {
            if (p == null || p.Statistics == null)
                return 0;

            return (float)playerStatisticsSleepField.GetValue(p.Statistics);
        }

        private static void SetSleep(IPlayer p, float sleep)
        {
            if (p == null)
                return;

            playerStatisticsSleepField.SetValue(p.Statistics, sleep);
        }

        #endregion
    }
}
