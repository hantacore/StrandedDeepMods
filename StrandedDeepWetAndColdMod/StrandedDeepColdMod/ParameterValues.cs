using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrandedDeepWetAndColdMod
{
    public static class ParameterValues
    {
        public static float BALANCING_FACTOR = 2f;

        public static int MAX_ENERGY = 700;
        public static int MAX_WETNESS = 200;
        public static int MAX_TEMP = 100;

        public static int COMFORT_MAX_TEMPERATURE_DEGREES_AIR = 26; // degrees
        public static int COMFORT_MIN_TEMPERATURE_DEGREES_AIR = 22; // degrees
        public static int NEUTRAL_TEMPERATURE_DEGREES_WATER = 33; // degrees
        public static float NATURAL_TEMPERATURE_REGULATION_PER_MINUTE = 2 / BALANCING_FACTOR;

        //public static int HEAT_PERCEPTION = 6;// degrees per minute
        //public static int COLD_PERCEPTION = -6;// degrees per minute

        public static int HOT_THRESHOLD = 65;// percent
        public static int SWEAT_THRESHOLD = 75;// percent
        public static int COLD_THRESHOLD = -65;// percent
        public static int CALORIES_THRESHOLD = -75;// percent

        public static float DISTANCE_TO_FIRE_HEAT = 6f;//2f;
        public static float REGULATION_CONSUMPTION_DIVIDE_FACTOR = 4f;
        public static float NATURAL_TEMPERATURE_REGULATION_SLEEP_EFFICIENCY = 0.5f;

        /// <summary>
        /// default is 4 ingame hours
        /// </summary>
        public static int SICK_INGAME_DELAY = 4 * 60 * 60 * 1000; // 4 ingame hours
        /// <summary>
        /// default is 1 ingame day
        /// </summary>
        public static int FEVER_INGAME_DELAY = 60 * 60 * 1000 * 24; // default is 1 ingame day
        /// <summary>
        /// default is 4 ingame hour
        /// </summary>
        public static int HEAL_INGAME_DELAY_MINUTES = 4 * 60; // 4 ingame hours
        /// <summary>
        /// default is 4 ingame hour
        /// </summary>
        public static int FEVER_HEAL_INGAME_DELAY_MINUTES = 4 * 60; // 4 ingame hours

        /// <summary>
        /// Technical parameters
        /// </summary>
        public static int SHELTER_CHECK_INTERVAL_SECONDS = 2;
        public static float TOP_SHELTER_CHECK_DISTANCE = 20f;
        public static float SHELTER_CHECK_DISTANCE = 10f;//5f;
        public static int STATUS_CHECK_INTERVAL_GAMETIME_MINUTES = 1;
        public static int MINIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS = 10000;
        public static int MAXIMAL_SOUND_EFFECT_INTERVAL_MILLISECONDS = 60000;

        /// <summary>
        /// Fixed parameters
        /// </summary>
        public static int UNDERWATER_WETNESS_MODIFICATOR_PER_MINUTE = 100;
        public static int WATER_TOUCH_WETNESS_MODIFICATOR_PER_MINUTE = 10;
        public static int UNSHELTERED_RAIN_WETNESS_MODIFICATOR_PER_MINUTE = 2;
        public static int DAYLIGHT_SUN_WETNESS_MODIFICATOR_PER_MINUTE = -2;
        public static int NIGHT_WETNESS_MODIFICATOR_PER_MINUTE = -1;
        public static int DAY_WETNESS_MODIFICATOR_PER_MINUTE = -1;
        public static int HEATSOURCE_WETNESS_MODIFICATOR_PER_MINUTE = -7;

        /// <summary>
        /// Balanced parameters
        /// </summary>
        public static float DAYLIGHT_DAWN_TEMP_MODIFICATOR_PER_MINUTE = 1 / BALANCING_FACTOR;
        public static float DAYLIGHT_MID_SUN_TEMP_MODIFICATOR_PER_MINUTE = 2 / BALANCING_FACTOR;
        public static float DAYLIGHT_FULL_SUN_TEMP_MODIFICATOR_PER_MINUTE = 3 / BALANCING_FACTOR;

        public static float HOUSING_SHELTER_TEMP_MODIFICATOR_PER_MINUTE = 2 / BALANCING_FACTOR;
        public static float HEATSOURCE_TEMP_MODIFICATOR_PER_MINUTE = 5 / BALANCING_FACTOR;
        public static float ACTIVITY_TEMP_MODIFICATOR_PER_MINUTE = 3 / BALANCING_FACTOR;
        public static float WETNESS_TEMP_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float EVAPORATION_TEMP_MODIFICATOR_PER_MINUTE = 5 / BALANCING_FACTOR;
        //public static int UNDERWATER_TEMP_MODIFICATOR = -1;
        public static float STORM_TEMP_MODIFICATOR_PER_MINUTE = -2 / BALANCING_FACTOR;
        public static float NIGHT_TEMP_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float SICK_TEMP_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float FEVER_TEMP_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;

        public static float COLD_CALORIES_CONSUMPTION_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float SWEAT_WATER_CONSUMPTION_PER_MINUTE = -1 / BALANCING_FACTOR;

        public static float SICK_ENERGY_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float REST_ENERGY_MODIFICATOR_PER_MINUTE = 1 / BALANCING_FACTOR;
        public static float ACTIVITY_ENERGY_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float FEVER_ENERGY_MODIFICATOR_PER_MINUTE = -1 / BALANCING_FACTOR;
        public static float TIME_ENERGY_MODIFICATOR_PER_MINUTE = 0.3f / BALANCING_FACTOR;

        public static float SICK_HEAL_HOUSE_MULTIPLICATOR_PER_MINUTE = 5 / BALANCING_FACTOR;
        public static float FEVER_HEAL_HOUSE_MULTIPLICATOR_PER_MINUTE = 10 / BALANCING_FACTOR;


        public static int ICONSIZE = 50;

        //public static Vector2 COLDMETER_POSITION = new Vector2(550, -300);
        //public static Vector2 CALORIESINDICATOR_POSITION = new Vector2(490, -300);
        //public static Vector2 HOTMETER_POSITION = new Vector2(550, -240);
        //public static Vector2 SWEATINDICATOR_POSITION = new Vector2(490, -240);
        //public static Vector2 WETMETER_POSITION = new Vector2(550, -180);
        //public static Vector2 ENERGYMETER_POSITION = new Vector2(550, -120);
        //public static Vector2 HEATFLAG_POSITION = new Vector2(550, -60);
        //public static Vector2 ACTIVITYFLAG_POSITION = new Vector2(490, -60);
        //public static Vector2 SHELTERFLAG_POSITION = new Vector2(550, 0);
        //public static Vector2 HOUSINGFLAG_POSITION = new Vector2(550, 60);

        public static Vector2 COLDMETER_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 +(ICONSIZE + 10));
        public static Vector2 CALORIESINDICATOR_POSITION = new Vector2(610 - 2 * (ICONSIZE + 10), -360 + (ICONSIZE + 10));
        public static Vector2 HOTMETER_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 + 2 * (ICONSIZE + 10));
        public static Vector2 SWEATINDICATOR_POSITION = new Vector2(610 - 2 * (ICONSIZE + 10), -360 + 2 * (ICONSIZE + 10));
        public static Vector2 WETMETER_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 + 3 * (ICONSIZE + 10));
        public static Vector2 ENERGYMETER_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 + 4 * (ICONSIZE + 10));
        public static Vector2 HEATFLAG_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 + 5 * (ICONSIZE + 10));
        public static Vector2 ACTIVITYFLAG_POSITION = new Vector2(610 - 2 *( ICONSIZE + 10), -360 + 5 * (ICONSIZE + 10));
        public static Vector2 SHELTERFLAG_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 + 6 * (ICONSIZE + 10));
        public static Vector2 HOUSINGFLAG_POSITION = new Vector2(610 - (ICONSIZE + 10), -360 + 7 * (ICONSIZE + 10));
    }
}
