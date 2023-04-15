using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StrandedDeepMapper
{
    public static class DirectoriesDetector
    {
        static int strandedDeepSteamId = 313120;

        public static void AutoDetectDirectories(TextBox tbDataDirectory, TextBox tbMapsDirectory, TextBox tbSDDefaultDirectory, TextBox tbMissionsPath, TextBox tbWorkshopPath, Action<string> ReadMissions, Action<string> Log)
        {
            if (Log == null)
                Log = LogLocal;

            if (ReadMissions == null)
                ReadMissions = ReadMissionsLocal;

            Log("Initiating paths auto-detection");
            if (tbDataDirectory != null)
                tbDataDirectory.Text = "";
            if (tbMapsDirectory != null)
                tbMapsDirectory.Text = "";
            if (tbWorkshopPath != null)
                tbWorkshopPath.Text = "";

            // Looking for SD user directory
            if (tbDataDirectory != null && tbMapsDirectory != null)
            {
                string dataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
                if (Directory.Exists(dataDirectory))
                {
                    tbDataDirectory.Text = dataDirectory;
                    if (Directory.Exists(Path.Combine(tbDataDirectory.Text, "Maps\\")))
                    {
                        tbMapsDirectory.Text = Path.Combine(tbDataDirectory.Text, "Maps\\");

                        Log("Maps directory auto-detection success : " + tbMapsDirectory.Text);
                    }
                    else
                    {
                        Log("Maps directory auto-detection failed");
                        tbDataDirectory.Text = "";
                        tbMapsDirectory.Text = "";
                    }
                }
                else
                {
                    Log("Maps directory auto-detection failed");
                    tbDataDirectory.Text = "";
                    tbMapsDirectory.Text = "";
                }
            }

            if (tbSDDefaultDirectory != null && tbMissionsPath != null && tbWorkshopPath != null)
            {
                try
                {
                    string steamInstallPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string;
                    string steamLibraryPath = "";
                    string strandedDeepInstallPath = "";
                    string missionsPath = "";
                    try
                    {
                        if (!String.IsNullOrEmpty(steamInstallPath) && Directory.Exists(steamInstallPath))
                        {
                            Log("Steam directory auto-detection success : " + steamInstallPath);

                            // default library dir
                            if (Directory.Exists(Path.Combine(steamInstallPath, "steamapps\\common\\Stranded Deep\\"))
                                && Directory.Exists(Path.Combine(steamInstallPath, "steamapps\\common\\Stranded Deep\\Stranded_Deep_Data\\Data\\Default Maps\\")))
                            {
                                steamLibraryPath = steamInstallPath;
                                strandedDeepInstallPath = Path.Combine(steamInstallPath, "steamapps\\common\\Stranded Deep\\");
                                // found ! hurray !
                                Log("Stranded Deep directory auto-detection success : " + strandedDeepInstallPath);
                                missionsPath = Path.Combine(strandedDeepInstallPath, "Stranded_Deep_Data\\Data\\Default Maps\\");
                                ReadMissions(missionsPath);
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
                                                if (Directory.Exists(Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\"))
                                                    && Directory.Exists(Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\Stranded_Deep_Data\\Data\\Default Maps\\")))
                                                {
                                                    steamLibraryPath = potentialPath;
                                                    strandedDeepInstallPath = Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\");
                                                    // found ! hurray !
                                                    Log("Stranded Deep directory auto-detection success : " + strandedDeepInstallPath);
                                                    missionsPath = Path.Combine(strandedDeepInstallPath, "Stranded_Deep_Data\\Data\\Default Maps\\");
                                                    ReadMissions(missionsPath);
                                                    break;
                                                }
                                                else
                                                {
                                                    Log("Stranded Deep directory auto-detection failed");
                                                    steamInstallPath = "";
                                                    steamLibraryPath = "";
                                                    strandedDeepInstallPath = "";
                                                    missionsPath = "";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (String.IsNullOrEmpty(strandedDeepInstallPath))
                            {
                                Log("Stranded Deep directory auto-detection failed");
                                steamInstallPath = "";
                                steamLibraryPath = "";
                                strandedDeepInstallPath = "";
                                missionsPath = "";
                            }
                        }
                        else
                        {
                            Log("Steam directory auto-detection failed");
                        }

                        // Get Workshop maps
                        if (!String.IsNullOrEmpty(steamLibraryPath) && Directory.Exists(steamLibraryPath))
                        {
                            if (Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\"))
                                && Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId)))
                            {
                                tbWorkshopPath.Text = Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId);
                            }
                        }
                        if (String.IsNullOrEmpty(tbWorkshopPath.Text))
                        {
                            Log("Could not auto-detect Workshop path");
                        }
                    }
                    catch
                    {
                        Log("Steam directory auto-detection failed");
                        steamInstallPath = "";
                        steamLibraryPath = "";
                        strandedDeepInstallPath = "";
                        missionsPath = "";
                    }

                    tbSDDefaultDirectory.Text = strandedDeepInstallPath;
                    tbMissionsPath.Text = missionsPath;
                }
                catch (Exception ex2)
                {
                    // cannot read user library
                    Log("Steam directory auto-detection failed");
                    tbSDDefaultDirectory.Text = "";
                    tbMissionsPath.Text = "";
                    tbWorkshopPath.Text = "";
                }
            }
        }

        public static void LogLocal(string message)
        {
            Console.WriteLine(message);
        }

        public static void ReadMissionsLocal(string path)
        {
            Console.WriteLine("No action passed as parameter");
        }

        public static string GetOBJECTFile(string destDirectory)
        {
            string destFileName = "";
            foreach (string fileOrDir in Directory.EnumerateFiles(destDirectory))
            {
                if (fileOrDir.Contains("_OBJECT.map") && !fileOrDir.Contains(".bak"))
                {
                    destFileName = fileOrDir;
                    break;
                }
            }
            return destFileName;
        }

        public static string GetHEIGHTFile(string destDirectory)
        {
            string destFileName = "";
            foreach (string fileOrDir in Directory.EnumerateFiles(destDirectory))
            {
                if (fileOrDir.Contains("_HEIGHT.map") && !fileOrDir.Contains(".bak"))
                {
                    destFileName = fileOrDir;
                    break;
                }
            }
            return destFileName;
        }

        public static bool MakeBackup(string destFileName, string reason)
        {
            string backupFileName = destFileName + "_" + reason + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH\\hmm\\mss\\s") + ".bak";
            while (File.Exists(backupFileName))
            {
                backupFileName = destFileName + "_" + reason + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH\\hmm\\mss\\s") + ".bak";
            }
            File.Copy(destFileName, backupFileName);
            return true;
        }

        public static List<string> GetBackupNames(string mapDirectory)
        {
            List<string> backups = new List<string>();
            // get all *.bak files

            foreach(string file in Directory.EnumerateFiles(mapDirectory, "*.bak"))
            {
                backups.Add(file);
            }

            return backups;
        }

        public static bool RestoreBackup(string backupFileName, Action<string> Log)
        {
            if (Log == null)
                Log = LogLocal;

            string directory = Path.GetDirectoryName(backupFileName);

            string json = File.ReadAllText(backupFileName);
            int backupObjects = 0;
            if (!String.IsNullOrEmpty(json) && json != "null")
            {
                dynamic prefabs = JsonConvert.DeserializeObject(json);
                backupObjects = prefabs.Count;
            }

            int currentObjects = 0;
            string currentObjectFileName = DirectoriesDetector.GetOBJECTFile(directory);
            if (File.Exists(currentObjectFileName))
            {
                json = File.ReadAllText(currentObjectFileName);
                if (!String.IsNullOrEmpty(json) && json != "null")
                {
                    dynamic prefabs = JsonConvert.DeserializeObject(json);
                    currentObjects = prefabs.Count;
                }
            }

            if (MessageBox.Show("You are about to restore this backup\n" 
                + Path.GetFileName(backupFileName)
                + " (" + backupObjects + " objects)\nover the current OBJECT file ("
                + currentObjects + " objects) which will be lost.\nContinue ?", "Confirm restore", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Log("Restoring " + backupFileName);
                try
                {
                    if (File.Exists(currentObjectFileName))
                    {
                        File.Delete(currentObjectFileName);
                    }
                    File.Copy(backupFileName, currentObjectFileName);
                    //File.Delete(backupFile);
                    Log("Restore complete");
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Log("Restore failed");
                    return false;
                }
            }

            return true;
        }
    }
}
