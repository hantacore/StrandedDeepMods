using Beam;
using Beam.AccountServices;
using Beam.Language;
using Beam.Serialization.Json;
using Beam.Terrain;
using Beam.UI;
using Beam.UI.MapEditor;
using LE_LevelEditor;
using LE_LevelEditor.Core;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Smooth.Slinq;
using System.Reflection;
using LE_LevelEditor.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.IO;

namespace StrandedDeepRpgMod
{
    static partial class Main
    {
        #region Variables

        private static int islandCount = 49;
        private static string configFileName = "StrandedDeepRpgMod.config";

        private static bool autoUpdateSharkSpawners = false;
        private static bool autoCleanRpgReferences = false;

        public static Beam.UI.MapEditor.MapEditorMenuPresenter Presenter { get; set; }

        public static Beam.UI.MapEditor.MapEditorSaveMenuPresenter SavePresenter { get; set; }

        public static bool EditorInitialized { get; set; }

        private static string lastMapId = null;

        private static string steamInstallPath = null;
        private static string steamLibraryPath = null;
        private static string strandedDeepInstallPath = null;
        private static string workshopPath = null;
        private static string mapsDirectory = null;
        private static string dataDirectory = null;

        private static bool uniqueIdRestorePending = false;

        private static int strandedDeepSteamId = 313120;

        #endregion

        #region config handling

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, "StrandedDeepRpgMod.config");
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach (string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            if (tokens[0].Contains("autoCleanRpgReferences"))
                            {
                                autoCleanRpgReferences = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("autoUpdateSharkSpawners"))
                            {
                                autoUpdateSharkSpawners = bool.Parse(tokens[1]);
                            }
                        }
                    }
                }
            }
        }

        private static void WriteConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, "StrandedDeepRpgMod.config");
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("autoCleanRpgReferences=" + autoCleanRpgReferences + ";");
                sb.AppendLine("autoUpdateSharkSpawners=" + autoUpdateSharkSpawners + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        #endregion

        #region Main functions

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;

            try
            {
                ReadConfig();

                AutoDetectDirectories();

                InitRpg();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Stranded Deep Rpg mod : load exception " + e);
                return false;
            }

            Debug.Log("Stranded Deep Rpg mod properly loaded");

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Rpg mod by Hantacore");
            //autoUpdateSharkSpawners = GUILayout.Toggle(autoUpdateSharkSpawners, "Auto-update shark spawners (WIP)");
            autoCleanRpgReferences = GUILayout.Toggle(autoCleanRpgReferences, "Auto-clean Rpg missing references");
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                HandleSavesAndUniqueIds();

                RunRpg();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        #endregion

        private static void AutoDetectDirectories()
        {
            dataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (Directory.Exists(dataDirectory))
            {
                if (Directory.Exists(Path.Combine(dataDirectory, "Maps\\")))
                {
                    mapsDirectory = Path.Combine(dataDirectory, "Maps\\");
                    Debug.Log("Maps directory auto-detection success");
                }
            }
            else
            {
                dataDirectory = null;
            }
            if (string.IsNullOrEmpty(dataDirectory))
            {
                Debug.Log("Data directory auto-detection failed");
                throw new Exception("Data directory auto-detection failed");
            }
            if (string.IsNullOrEmpty(mapsDirectory))
            {
                Debug.Log("Maps directory auto-detection failed");
                throw new Exception("Maps directory auto-detection failed");
            }

            try
            {
                steamInstallPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string;
                steamLibraryPath = "";
                strandedDeepInstallPath = "";
                workshopPath = "";
                try
                {
                    if (!String.IsNullOrEmpty(steamInstallPath) && Directory.Exists(steamInstallPath))
                    {
                        Debug.Log("Steam directory auto-detection success : " + steamInstallPath);

                        // default library dir
                        if (Directory.Exists(Path.Combine(steamInstallPath, "steamapps\\common\\Stranded Deep\\")))
                        {
                            steamLibraryPath = steamInstallPath;
                            strandedDeepInstallPath = Path.Combine(steamInstallPath, "steamapps\\common\\Stranded Deep\\");
                            // found ! hurray !
                            Debug.Log("Stranded Deep directory auto-detection success : " + strandedDeepInstallPath);
                        }
                        else
                        {
                            // relocated steam library
                            string pathsFile = "libraryfolders.vdf";
                            if (File.Exists(Path.Combine(steamInstallPath, "steamapps/", pathsFile)))
                            {
                                string[] content = File.ReadAllLines(Path.Combine(steamInstallPath, "steamapps/", pathsFile));
                                foreach (string line in content)
                                {
                                    if (line.Contains("path"))
                                    {
                                        string[] tokens = line.Split(new string[] { "\t", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                                        if (tokens.Length == 2)
                                        {
                                            string potentialPath = tokens[1].Replace("\\\\", "\\");
                                            if (Directory.Exists(Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\")))
                                            {
                                                steamLibraryPath = potentialPath;
                                                strandedDeepInstallPath = Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\");
                                                // found ! hurray !
                                                Debug.Log("Stranded Deep directory auto-detection success : " + strandedDeepInstallPath);
                                                break;
                                            }
                                            else
                                            {
                                                Debug.Log("Stranded Deep directory auto-detection failed");
                                                steamInstallPath = "";
                                                steamLibraryPath = "";
                                                strandedDeepInstallPath = "";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (String.IsNullOrEmpty(strandedDeepInstallPath))
                        {
                            Debug.Log("Stranded Deep directory auto-detection failed");
                            steamInstallPath = "";
                            steamLibraryPath = "";
                            strandedDeepInstallPath = "";
                        }
                    }
                    else
                    {
                        Debug.Log("Steam directory auto-detection failed");
                    }

                    // Get Workshop maps
                    if (!String.IsNullOrEmpty(steamLibraryPath) && Directory.Exists(steamLibraryPath))
                    {
                        if (Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\"))
                            && Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId)))
                        {
                            workshopPath = Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId);
                        }
                    }
                    if (String.IsNullOrEmpty(workshopPath))
                    {
                        Debug.Log("Could not auto-detect Workshop path");
                    }
                }
                catch
                {
                    Debug.Log("Steam directory auto-detection failed");
                    steamInstallPath = "";
                    steamLibraryPath = "";
                    strandedDeepInstallPath = "";
                    workshopPath = "";
                }
            }
            catch (Exception ex2)
            {
                // cannot read user library
                Debug.Log("Steam directory auto-detection failed");
                throw;
            }
        }

        public static byte[] ExtractResource(String filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}
