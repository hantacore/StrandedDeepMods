using Beam;
using Beam.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Funlabs;

namespace StrandedDeepOneHitBuildMod
{
    static class Main
    {
        private static string configFileName = "StrandedDeepOneHitBuildMod.config";

        //private static bool infiniteAir = false;
        //private static bool biggerAirTank = false;

        //private static bool infiniteGas = false;
        //private static bool biggerGasTank = false;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;

            ReadConfig();

            Debug.Log("Stranded Deep OneHitBuild Mod properly loaded");

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            //infiniteAir = GUILayout.Toggle(infiniteAir, "Inifinite air in tank");
            //infiniteGas = GUILayout.Toggle(infiniteGas, "Inifinite gas in vehicles");
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static List<ConstructionObject_HAMMER> _handledHammers = new List<ConstructionObject_HAMMER>();
        static int pass = 0;
        static FieldInfo fi_HealthPointsToAdd = typeof(ConstructionObject_HAMMER).GetField("HealthPointsToAdd", BindingFlags.NonPublic | BindingFlags.Instance);

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                if (Beam.Game.State == GameState.NEW_GAME
                    || Beam.Game.State == GameState.LOAD_GAME)
                {
                    if (pass > 100)
                    {
                        ConstructionObject_HAMMER[] hammers = Beam.Game.FindObjectsOfType<ConstructionObject_HAMMER>();
                        if (hammers != null)
                        {
                            if (_handledHammers.Count == hammers.Length)
                            {
                                pass = 0;
                                return;
                            }

                            //Debug.Log("Stranded Deep OneHitBuild Mod : hammers found");
                            foreach (ConstructionObject_HAMMER hammer in hammers)
                            {
                                if (_handledHammers.Contains(hammer))
                                    continue;
                                fi_HealthPointsToAdd.SetValue(hammer, 1000f);
                                Debug.Log("Stranded Deep OneHitBuild Mod : hammer updated HealthPointsToAdd = " + fi_HealthPointsToAdd.GetValue(hammer));
                                _handledHammers.Add(hammer);
                            }
                        }
                        pass = 0;
                    }
                    else
                        pass++;
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep OneHitBuild Mod : error on update : " + e);
            }
        }

        private static void Reset()
        {
            _handledHammers.Clear();
            pass = 0;
        }

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach (string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            //if (tokens[0].Contains("revealWorld"))
                            //{
                            //    revealWorld = bool.Parse(tokens[1]);
                            //}
                            //else if (tokens[0].Contains("revealMissions"))
                            //{
                            //    revealMissions = bool.Parse(tokens[1]);
                            //}
                            //else if (tokens[0].Contains("debugMode"))
                            //{
                            //    debugMode = bool.Parse(tokens[1]);
                            //}
                            //if (tokens[0].Contains("viewDistance"))
                            //{
                            //    viewDistance = float.Parse(tokens[1]);
                            //}
                        }
                    }
                }
            }
        }

        private static void WriteConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine("viewDistance=" + viewDistance + ";");
                //sb.AppendLine("revealWorld=" + revealWorld + ";");
                //sb.AppendLine("revealMissions=" + revealMissions + ";");
                //sb.AppendLine("debugMode=" + debugMode + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
