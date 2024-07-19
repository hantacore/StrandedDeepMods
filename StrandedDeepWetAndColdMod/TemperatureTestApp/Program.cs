using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TemperatureTestApp
{
    class Program
    {
        static DateTime currentTime;
    
        static void Main(string[] args)
        {
            currentTime = DateTime.Now;
            while (true)
            {
                UpdateAtmosphereTemperature();
                Thread.Sleep(5);
            }
        }

        private static void UpdateAtmosphereTemperature()
        {
            currentTime = currentTime.AddMinutes(1);
            // use GameCalendar.Instance.Month !

            //Debug.Log("Stranded Deep "+MODNAME+" Mod : current month in GameTime.Now = " + now.Month + " / current month in GameTime.Instance.Month : " + GameCalendar.Instance.Month);

            //GameCalendar.Instance.CurrentMonthData; // <- USE THIS DATA

            // periode (1440 minutes par jour) = 2 * Pi / B => B = (2 * Pi) / 1440

            float period = ((float)Math.PI * 2f) / 1440f;
            float timeShiftLeft = 16f * 60f;// -119.905f;// -120; //(2am is coldest, 2pm is hottest)
            float minTemperature = 14f;
            float maxTemperature = 25f;
            float amplitude = (maxTemperature - minTemperature);
            float verticalShift = minTemperature;

            float currentTemperature = amplitude * ((float)Math.Sin(period * (currentTime.Hour * 60 + currentTime.Minute + timeShiftLeft)) + 1) / 2 + verticalShift;

            if (currentTemperature >= 0.9999 || currentTemperature <= 0.0001)
                Console.WriteLine("Time is : " + currentTime.ToShortTimeString() + " / temperature is : " + currentTemperature);

            //Debug.Log("Stranded Deep "+MODNAME+" Mod : current month data Min : " + GameCalendar.Instance.CurrentMonthData.MinTemp + " / MAx " + GameCalendar.Instance.CurrentMonthData.MaxTemp);

            //GameCalendar.Instance.MonthsData
            //Debug.Log("Stranded Deep "+MODNAME+" Mod : month by index ("+ now.Month +") data Min : " + GameCalendar.Instance.MonthsData[now.Month].MinTemp + " / MAx " + GameCalendar.Instance.MonthsData[now.Month].MaxTemp);

        }
    }
}
