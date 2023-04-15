using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StrandedDeepMapper
{
    public partial class WorldRandomizerTool : UserControl
    {
        int strandedDeepSteamId = 313120;
        string workshopFlag = " (workshop)";
        string localFlag = " (local)";

        public WorldRandomizerTool()
        {
            InitializeComponent();

            tsiOpenMapDirectory.Click += TsiOpenMapDirectory_Click;
        }

        const int maxIslands = 49;
        const string mission0 = "MAP_INTERNAL_MISSION_0";
        const string mission1 = "MAP_INTERNAL_MISSION_1";
        const string mission2 = "MAP_INTERNAL_MISSION_2";
        const string mission3 = "MAP_INTERNAL_MISSION_3";

        internal enum WorldType
        {
            Ocean = 10,
            Scarse = 24,
            Standard = 48
        }

        Dictionary<string, string> mapDictionary = new Dictionary<string, string>();
        bool isLoaded = false;

        private void WorldRandomizerTool_Load(object sender, EventArgs e)
        {
            if (isLoaded)
                return;

            try
            {
                //AutoDetectDirectories();
                DirectoriesDetector.AutoDetectDirectories(tbDataDirectory, tbMapsDirectory, tbSDDefaultDirectory, tbMissionsPath, tbWorkshopPath, ReadMissions, Log);

                LoadGeneratorNew();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            isLoaded = true;
        }

        //private void AutoDetectDirectories()
        //{
        //    Log("Initiating paths auto-detection");
        //    tbDataDirectory.Text = "";
        //    tbMapsDirectory.Text = "";
        //    tbWorkshopPath.Text = "";

        //    // Looking for SD user directory
        //    string dataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
        //    if (Directory.Exists(dataDirectory))
        //    {
        //        tbDataDirectory.Text = dataDirectory;
        //        if (Directory.Exists(Path.Combine(tbDataDirectory.Text, "Maps\\")))
        //        {
        //            tbMapsDirectory.Text = Path.Combine(tbDataDirectory.Text, "Maps\\");

        //            Log("Maps directory auto-detection success : " + tbMapsDirectory.Text);
        //        }
        //        else
        //        {
        //            Log("Maps directory auto-detection failed");
        //            tbDataDirectory.Text = "";
        //            tbMapsDirectory.Text = "";
        //        }
        //    }
        //    else
        //    {
        //        Log("Maps directory auto-detection failed");
        //        tbDataDirectory.Text = "";
        //        tbMapsDirectory.Text = "";
        //    }

        //    try
        //    {
        //        string steamInstallPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string;
        //        string steamLibraryPath = "";
        //        string strandedDeepInstallPath = "";
        //        string missionsPath = "";
        //        try
        //        {
        //            string pathsFile = "libraryfolders.vdf";
        //            if (File.Exists(Path.Combine(steamInstallPath, "steamapps/", pathsFile)))
        //            {
        //                Log("Steam directory auto-detection success : " + steamInstallPath);
        //                string[] content = File.ReadAllLines(Path.Combine(steamInstallPath, "steamapps/", pathsFile));
        //                foreach (string line in content)
        //                {
        //                    if (line.Contains("path"))
        //                    {
        //                        string[] tokens = line.Split(new string[] { "\t", "\"" }, StringSplitOptions.RemoveEmptyEntries);
        //                        if (tokens.Length == 2)
        //                        {
        //                            string potentialPath = tokens[1].Replace("\\\\", "\\");
        //                            if (Directory.Exists(Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\"))
        //                                && Directory.Exists(Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\Stranded_Deep_Data\\Data\\Default Maps\\")))
        //                            {
        //                                steamLibraryPath = potentialPath;
        //                                strandedDeepInstallPath = Path.Combine(potentialPath, "steamapps\\common\\Stranded Deep\\");
        //                                // found ! hurray !
        //                                Log("Stranded Deep directory auto-detection success : " + strandedDeepInstallPath);
        //                                missionsPath = Path.Combine(strandedDeepInstallPath, "Stranded_Deep_Data\\Data\\Default Maps\\");
        //                                ReadMissions(missionsPath);
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                Log("Stranded Deep directory auto-detection failed");
        //                                steamInstallPath = "";
        //                                steamLibraryPath = "";
        //                                strandedDeepInstallPath = "";
        //                                missionsPath = "";
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // Get Workshop maps
        //            if (!String.IsNullOrEmpty(steamLibraryPath) && Directory.Exists(steamLibraryPath))
        //            {
        //                if (Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\"))
        //                    && Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId)))
        //                {
        //                    tbWorkshopPath.Text = Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId);
        //                }
        //            }
        //            if (String.IsNullOrEmpty(tbWorkshopPath.Text))
        //            {
        //                Log("Could not auto-detect Workshop path");
        //            }
        //        }
        //        catch
        //        {
        //            Log("Steam directory auto-detection failed");
        //            steamInstallPath = "";
        //            steamLibraryPath = "";
        //            strandedDeepInstallPath = "";
        //            missionsPath = "";
        //        }

        //        tbSDDefaultDirectory.Text = strandedDeepInstallPath;
        //        tbMissionsPath.Text = missionsPath;
        //    }
        //    catch (Exception ex2)
        //    {
        //        // cannot read user library
        //        Log("Steam directory auto-detection failed");
        //        tbSDDefaultDirectory.Text = "";
        //        tbMissionsPath.Text = "";
        //        tbWorkshopPath.Text = "";
        //    }
        //}

        private void ClearWorkshopMaps()
        {
            if (clbMaps.Items.Count == 0)
                return;
            for (int i = 0; i < clbMaps.Items.Count; i++)
            {
                if (clbMaps.Items[i].ToString().Contains(workshopFlag))
                {
                    clbMaps.Items.RemoveAt(i);
                    i--;
                }
            }
        }

        private void ClearLocalMaps()
        {
            if (clbMaps.Items.Count == 0)
                return;
            for (int i = 0; i < clbMaps.Items.Count; i++)
            {
                if (clbMaps.Items[i].ToString().Contains(localFlag))
                {
                    clbMaps.Items.RemoveAt(i);
                    i--;
                }
            }
        }

        private void LoadGeneratorNew()
        {
            clbMaps.Items.Clear();
            mapDictionary.Clear();

            DetectSlots();

            #region load local maps
            try
            {
                if (!String.IsNullOrEmpty(tbMapsDirectory.Text))
                {
                    Log("Looking for local islands in : " + tbMapsDirectory.Text);
                    // Locating Maps directory and listing compatible maps in checkedlistbox
                    IEnumerable<string> localIslandsList = Directory.EnumerateDirectories(tbMapsDirectory.Text);
                    foreach (string mapDirectory in localIslandsList)
                    {
                        //Log("Parsing map : " + mapDirectory);
                        foreach (string mapFile in Directory.EnumerateFiles(mapDirectory))
                        {
                            if (mapFile.Contains("EDITOR") && !mapFile.Contains(".bak"))
                            {
                                string key = null;
                                try
                                {
                                    //key = MapParser.Parse_EDITOR_FileNew(mapFile);
                                    //key = MapParser.Parse_EDITOR_File_revamp(mapFile);
                                    key = MapParser.Parse_EDITOR_FileDeserialize(mapFile);
                                }
                                catch (Exception fe)
                                {
                                    Log(fe.Message);
                                    break;
                                }
                                if (!String.IsNullOrEmpty(key)
                                    && !mapDictionary.ContainsKey(key))
                                {
                                    clbMaps.Items.Add(key + localFlag);
                                    mapDictionary.Add(key + localFlag, mapDirectory);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log("Local islands directory not set");
                }
            }
            catch
            {
                Log("Failed to load local islands");
            }

            #endregion

            #region load workshop maps
            try
            {
                if (!String.IsNullOrEmpty(tbWorkshopPath.Text))
                {
                    Log("Looking for workshop islands in : " + tbWorkshopPath.Text);
                    IEnumerable<string> workshopIslandsList = Directory.EnumerateDirectories(tbWorkshopPath.Text);
                    foreach (string mapDirectory in workshopIslandsList)
                    {
                        // Warning, workshop islands are in their own sub-directory
                        IEnumerable<string> workshopIslands = Directory.EnumerateDirectories(mapDirectory);
                        foreach (string mapSubDirectory in workshopIslands)
                        {
                            foreach (string mapFile in Directory.EnumerateFiles(mapSubDirectory))
                            {
                                if (mapFile.Contains("EDITOR") && !mapFile.Contains(".bak"))
                                {
                                    string key = null;
                                    try
                                    {
                                        //key = MapParser.Parse_EDITOR_FileNew(mapFile);
                                        //key = MapParser.Parse_EDITOR_File_revamp(mapFile);
                                        key = MapParser.Parse_EDITOR_FileDeserialize(mapFile);
                                    }
                                    catch (Exception fe)
                                    {
                                        Log(fe.Message);
                                        break;
                                    }
                                    if (!String.IsNullOrEmpty(key)
                                        && !mapDictionary.ContainsKey(key))
                                    {
                                        clbMaps.Items.Add(key + workshopFlag);
                                        mapDictionary.Add(key + workshopFlag, mapSubDirectory);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log("Workshop islands directory not set");
                }
            }
            catch (Exception e)
            {
                Log("Failed to load workshop islands");
            }

            #endregion
        }

        //private string CleanupNew(string input)
        //{
        //    List<char> toReplace = new List<char>();
        //    toReplace.Add('\u0001');
        //    toReplace.Add('\u0002');
        //    toReplace.Add('\u0005');
        //    toReplace.Add('\u0006');
        //    toReplace.Add('\u0011');
        //    toReplace.Add('\u0003');
        //    toReplace.Add('\u0004');
        //    toReplace.Add('\u0015');
        //    toReplace.Add('\u0016');
        //    toReplace.Add('\u000e');
        //    toReplace.Add('\u001a');
        //    toReplace.Add('\u0010');
        //    toReplace.Add('\u000f');
        //    toReplace.Add('\v');
        //    toReplace.Add('\n');
        //    toReplace.Add('\t');
        //    toReplace.Add('\u001e');
        //    toReplace.Add('\r');
        //    toReplace.Add('\f');
        //    toReplace.Add('\a');
        //    //toReplace.Add('\0');
        //    toReplace.Add('\b');
        //    toReplace.Add('\u0014');
        //    toReplace.Add('\u0013');
        //    toReplace.Add('\u0012');
        //    toReplace.Add('\u001b');

        //    string buffer = input;
        //    buffer = buffer.Replace("\0", "");
        //    foreach (char c in toReplace)
        //    {
        //        buffer = buffer.Replace(c, ';');
        //    }

        //    return buffer;
        //}

        //private string Parse_EDITOR_FileNew(string mapFilePath)
        //{
        //    if (String.IsNullOrEmpty(mapFilePath) || !mapFilePath.Contains("EDITOR"))
        //    {
        //        return null;
        //    }

        //    string mapFileContent = File.ReadAllText(mapFilePath);
        //    Dictionary<string, string> mapCharacteristics = new Dictionary<string, string>();

        //    // debug
        //    //Console.WriteLine(mapFileContent);

        //    string buffer = CleanupNew(mapFileContent);
        //    string[] columns = buffer.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (columns.Length != 27 && columns.Length != 28 && columns.Length != 33 && columns.Length != 34)
        //    {
        //        Log("This maps seems incompatible, or parsing was wrong (step 2) : " + mapFilePath);
        //        return null;
        //    }

        //    foreach (string column in columns)
        //    {
        //        if (column == "Id"
        //            || column == "VersionNumber"
        //            || column == "Name"
        //            || column == "Author"
        //            || column == "Description"
        //            || column == "DateCreated"
        //            || column == "DateEdited"
        //            || column == "GameVersionNumber")
        //        {
        //            mapCharacteristics.Add(column, "");
        //        }
        //    }

        //    if (columns.Length == 27)
        //    {
        //        mapCharacteristics["Id"] = columns[16];
        //        mapCharacteristics["VersionNumber"] = columns[17];
        //        mapCharacteristics["Name"] = columns[21];
        //        mapCharacteristics["Author"] = columns[22];
        //        mapCharacteristics["Description"] = columns[23];
        //        mapCharacteristics["DateCreated"] = columns[24];
        //        mapCharacteristics["DateEdited"] = columns[25];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 28)
        //    {
        //        mapCharacteristics["Id"] = columns[16];
        //        mapCharacteristics["VersionNumber"] = columns[17];
        //        mapCharacteristics["Name"] = columns[21];
        //        mapCharacteristics["Author"] = columns[22];
        //        mapCharacteristics["Description"] = columns[23];
        //        mapCharacteristics["DateCreated"] = columns[24];
        //        mapCharacteristics["DateEdited"] = columns[25];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 33)
        //    {
        //        // Procedural ?
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];
        //        mapCharacteristics["DateCreated"] = columns[31];
        //        mapCharacteristics["DateEdited"] = columns[32];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 34)
        //    {
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];
        //        mapCharacteristics["DateCreated"] = columns[31];
        //        mapCharacteristics["DateEdited"] = columns[32];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    // try to read editor version
        //    double versionNumber = 1.0;
        //    if (mapCharacteristics.ContainsKey("VersionNumber"))
        //    {
        //        string versionNumberString = mapCharacteristics["VersionNumber"];
        //        if (!String.IsNullOrEmpty(versionNumberString)
        //            && versionNumberString.Count(f => f == '.') == 2)
        //        {
        //            Double.TryParse(versionNumberString.Substring(0, versionNumberString.LastIndexOf('.')), out versionNumber);
        //        }
        //    }

        //    string key = mapCharacteristics["Name"] + " (by " + mapCharacteristics["Author"] + ") " + mapCharacteristics["VersionNumber"];
        //    //Log("Map name found : " + key);
        //    return key;
        //}

        private void bGenerate_Click(object sender, EventArgs e)
        {
            string slot = GetSlot();
            if (String.IsNullOrEmpty(slot))
            {
                Log("No slot selected, aborting");
                return;
            }

            // initialization
            Log("Initializations");
            bool useMissions = cbUseMissions.Checked;
            bool startDeep = cbDeep.Checked;
            string currentWorldDirectory = Path.Combine(tbDataDirectory.Text, slot, "World\\");
            string localWorldDirectory = Path.Combine(Directory.GetCurrentDirectory().ToString(), "World\\");

            Dictionary<string, string> selectedCustomMaps = new Dictionary<string, string>();
            foreach (string selectedElement in clbMaps.CheckedItems)
            {
                if (mapDictionary.ContainsKey(selectedElement))
                {
                    selectedCustomMaps.Add(selectedElement, mapDictionary[selectedElement]);
                }
            }

            // check wanted world
            WorldType worldType = WorldType.Standard;
            if (rbDense.Checked)
            {
                worldType = WorldType.Standard;
            }
            else if (rbScarse.Checked)
            {
                worldType = WorldType.Scarse;
            }
            else if (rbSea.Checked)
            {
                worldType = WorldType.Ocean;
            }

            int bigIslandsProportion = tbBigIslandsProportion.Value * 10;

            // confirmation
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Please be aware that this may absolutely destroy your current savegame, be sure you know exactly what you are trying to do.");
            sb.AppendLine("You have selected following parameters : ");
            sb.AppendLine("- " + worldType + " world type (" + (int)worldType + " islands)");
            sb.AppendLine(useMissions ? "- with missions and endgame" : "- WITHOUT missions and endgame");
            sb.AppendLine(startDeep ? "- player starts in the middle of the ocean" : "- players starts near an island (normal game)");
            if (cbMixMaps.Checked)
            {
                sb.AppendLine("- you want to mix custom and procedural islands");
            }
            else
            {
                sb.AppendLine("- you want only custom islands, the algorithm will complete with procedural islands if you have not selected enough");
            }
            sb.AppendLine("- you have selected " + selectedCustomMaps.Count + " custom islands");
            //sb.AppendLine("- you want " + bigIslandsProportion + "% of big procedural islands (if possible)");
            if (MessageBox.Show(sb.ToString(), "Please confirm world generation", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                Log("User cancelled");
                return;
            }

            if (selectedCustomMaps.Count <= 5
                && worldType == WorldType.Standard)
            {
                // generation is useless
                MessageBox.Show("Cannot generate à full world without at least 5 custom maps selected (algorithm limit due to mission slots)");
                return;
            }

            // check if stranded deep is running
            Process[] pname = Process.GetProcessesByName("Stranded_Deep");
            if (pname.Length == 0)
            {
                //MessageBox.Show("nothing");
            }
            else
            {
                Log("Stranded Deep is running, please exit");
                MessageBox.Show("Stranded Deep is running, please exit");
                //return;
            }

            // prepare world locally
            if (Directory.Exists(localWorldDirectory))
            {
                try
                {
                    Directory.Delete(localWorldDirectory, true);
                }
                catch (Exception)
                {
                    Log("Could not delete working directory : \n" + Path.Combine(Directory.GetCurrentDirectory().ToString(), localWorldDirectory));
                    MessageBox.Show("Could not delete working directory : \n" + Path.Combine(Directory.GetCurrentDirectory().ToString(), localWorldDirectory));
                    return;
                }
            }
            Directory.CreateDirectory(localWorldDirectory);

            // check prerequisites
            Log("Checking prerequisistes");

            if (useMissions)
            {
                try
                {
                    int missionCount = 0;
                    IEnumerable<string> temp = Directory.EnumerateDirectories(tbMissionsPath.Text);
                    foreach (string fileOrDir in temp)
                    {
                        missionCount++;
                    }
                    if (missionCount != 4)
                        throw new Exception();
                }
                catch (Exception)
                {
                    Log("Invalid Mission maps directory : \n" + tbSDDefaultDirectory.Text);
                    MessageBox.Show("Invalid Mission maps directory : \n" + tbSDDefaultDirectory.Text);
                    return;
                }
            }

            if (!CheckCurrentWorldDirectory(currentWorldDirectory))
            {
                return;
            }

            // check existing world valid
            Log("Checking existing world status");
            //Dictionary<int, int> proceduralIslandsSizes = new Dictionary<int, int>();
            try
            {
                // Iterate through the 49 directories
                for (int islandIndex = 0; islandIndex < maxIslands; islandIndex++)
                {
                    int mapFilesCount = 0;
                    IEnumerable<string> temp = Directory.EnumerateDirectories(Path.Combine(currentWorldDirectory, islandIndex.ToString()));
                    foreach (string dir in temp)
                    {
                        foreach (string file in Directory.EnumerateFiles(dir))
                        {
                            mapFilesCount++;
                            //try
                            //{
                            //    string editorFileContent = File.ReadAllText(file);
                            //}
                            //catch
                            //{
                            //    proceduralIslandsSizes.Add(islandIndex, 0);
                            //}
                        }
                        if (mapFilesCount < 3)
                            throw new Exception();
                    }
                }
                Log("World directory structure validated");
            }
            catch (Exception)
            {
                Log("Invalid world directory structure : \n" + currentWorldDirectory);
                MessageBox.Show("Invalid world directory structure : \n" + currentWorldDirectory);
                return;
            }

            if (!File.Exists(currentWorldDirectory + "Seed.sdd"))
            {
                Log("No current world seed found, aborting generation. \n Generate a new world in game and retry");
                MessageBox.Show("No current world seed found, aborting generation. \n Generate a new world in game and retry");
                return;
            }

            // backup existing world
            if (!BackupCurrentWorld(currentWorldDirectory))
            {
                return;
            }

            // Main algorithm
            Log("Entering algorithm");

            try
            {
                Generate(currentWorldDirectory, localWorldDirectory, useMissions, cbMixMaps.Checked, startDeep, selectedCustomMaps, worldType);
                //GenerateWithSize(currentWorldDirectory, localWorldDirectory, useMissions, cbMixMaps.Checked, startDeep, selectedCustomMaps, worldType, bigIslandsProportion, proceduralIslandsSizes);
            }
            catch (Exception ex)
            {
                Log("World generation failed with error : \n" + ex.ToString());
                MessageBox.Show("World generation failed with error : \n" + ex.ToString());
                return;
            }

            // Delete current world
            if (!DeleteCurrentWorld(currentWorldDirectory))
            {
                return;
            }

            // Copy generated world
            Log("Copying generated world");
            try
            {
                Directory.CreateDirectory(currentWorldDirectory);
                CopyFilesRecursively(new DirectoryInfo(localWorldDirectory),
                    new DirectoryInfo(currentWorldDirectory));
            }
            catch (Exception)
            {
                Log("Error copying world to target directory (worst case, please restore auto generated backup) : \n From : "
                    + localWorldDirectory
                    + " to "
                    + currentWorldDirectory);
                MessageBox.Show("Error copying world to target directory (worst case, please restore auto generated backup) : \n From : "
                    + localWorldDirectory
                    + " to "
                    + currentWorldDirectory);
                return;
            }

            // checks
            Log("Post generation checks");

            for (int islandIndex = 0; islandIndex < maxIslands; islandIndex++)
            {
                IEnumerable<string> temp = Directory.EnumerateFiles(Path.Combine(currentWorldDirectory, islandIndex.ToString()));
                foreach (string fileOrDir in temp)
                {
                    string target = Path.Combine(currentWorldDirectory, islandIndex.ToString());
                    if (!Directory.Exists(target)
                        || Directory.EnumerateDirectories(target).Count() == 0)
                    {
                        Log("Current world seems to be broken. \n Generate a new world in game and retry");
                        Log("Directory structure messed up in : " + target);
                        MessageBox.Show("Current world seems to be broken. \n Generate a new world in game and retry");
                        return;
                    }
                }
            }

            if (!File.Exists(currentWorldDirectory + "Seed.sdd"))
            {
                Log("No current world seed found. \n Generate a new world in game and retry");
                Log("No Seed found after check");
                MessageBox.Show("No current world seed found. \n Generate a new world in game and retry");
                return;
            }

            Log("Done");
            MessageBox.Show("All done ! Congratulations", "World generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region existing algorithm

        private void Generate(string currentWorldDirectory, string localWorldDirectory, bool useMissions, bool mixMapsChecked, bool startDeep, Dictionary<string, string> selectedCustomMaps, WorldType worldType)
        {
            // how many selected islands do we want to have ?
            Random r = new Random();
            int customsMapscount = selectedCustomMaps.Keys.Count;
            int currentIslandsCount = 0;
            int customIslandProbability = Math.Min((int)((double)customsMapscount / (double)worldType * 100), 100);
            // custom map density adjustment
            bool mixMaps = mixMapsChecked && (customsMapscount >= (int)((double)worldType / 2.0));

            // if mixMaps, we want an equal probability of customMaps and non custom maps
            // if the number of selected custom islands is lower than the size of the world mixmaps is set to false
            if (mixMaps || customIslandProbability > 50 && mixMapsChecked)
            {
                customIslandProbability = 50;
            }

            // special case : not enough custom islands to populate, algorithm forced to mix
            if (customsMapscount < (int)worldType)
            {
                mixMaps = true;
            }

            // new dictionary for big and small islands
            List<int> availableProceduralIslands = new List<int>();
            // First pass, place missions
            List<int> missionPositions = new List<int>();
            if (useMissions)
            {
                int missionPosition = r.Next(1, 48);
                missionPositions.Add(missionPosition);
                Log("Adding mission : " + mission0);
                AddMission(missionPosition, mission0);
                CheckAndAddAsAvailable(currentWorldDirectory, availableProceduralIslands, missionPosition);

                while (missionPositions.Contains(missionPosition))
                {
                    missionPosition = r.Next(1, 48);
                }
                missionPositions.Add(missionPosition);
                Log("Adding mission : " + mission1);
                AddMission(missionPosition, mission1);
                CheckAndAddAsAvailable(currentWorldDirectory, availableProceduralIslands, missionPosition);

                while (missionPositions.Contains(missionPosition))
                {
                    missionPosition = r.Next(1, 48);
                }
                missionPositions.Add(missionPosition);
                Log("Adding mission : " + mission2);
                AddMission(missionPosition, mission2);
                CheckAndAddAsAvailable(currentWorldDirectory, availableProceduralIslands, missionPosition);

                while (missionPositions.Contains(missionPosition))
                {
                    missionPosition = r.Next(1, 48);
                }
                missionPositions.Add(missionPosition);
                Log("Adding mission : " + mission3);
                AddMission(missionPosition, mission3);
                CheckAndAddAsAvailable(currentWorldDirectory, availableProceduralIslands, missionPosition);
            }

            // main algorithm loop
            List<int> burntProceduralIslands = new List<int>();
            for (int islandIndex = 0; islandIndex < maxIslands; islandIndex++)
            {
                // skip positionned missions
                if (missionPositions.Contains(islandIndex))
                {
                    continue;
                }

                string target = Path.Combine(localWorldDirectory, islandIndex.ToString());
                Directory.CreateDirectory(target);

                //int islandProbability = r.Next(0, 25);
                int islandProbability = r.Next(0, maxIslands);
                bool putIsland = (islandProbability <= (int)worldType);

                if (islandIndex == 0)
                {
                    // if start in the open, add abyss
                    if (startDeep)
                    {
                        AddAbyss(islandIndex);
                        continue;
                    }
                    // else spawn on island
                    putIsland = true;
                }

                // if there are already too many islands, add water
                if (currentIslandsCount >= (int)worldType)
                {
                    AddAbyss(islandIndex);
                    continue;
                }

                // if random probability puts island, add island
                if (putIsland)
                {
                    int customIslandProbabilityToken = r.Next(0, 100);
                    // putCustomIsland says if the island to be added should be a procedural island
                    bool putCustomIsland = (customIslandProbabilityToken <= customIslandProbability);
                    // the returned string is the name of the custom map that was added if it was not a procedural island
                    string addedCustomMapAvoidRedundency = AddIsland(currentWorldDirectory, islandIndex, putCustomIsland, selectedCustomMaps, ref availableProceduralIslands, ref burntProceduralIslands);
                    // if we added a custom island
                    if (!String.IsNullOrEmpty(addedCustomMapAvoidRedundency))
                    {
                        // a custom island was added, we track the procedural island in that position to be available for further use
                        if (selectedCustomMaps.ContainsKey(addedCustomMapAvoidRedundency))
                            selectedCustomMaps.Remove(addedCustomMapAvoidRedundency);
                        CheckAndAddAsAvailable(currentWorldDirectory, availableProceduralIslands, islandIndex);
                    }
                    else
                    {
                        // procedural island cannot be used anymore to avoid redundancy
                        burntProceduralIslands.Add(islandIndex);
                    }
                    // keep track of how many islands were added
                    currentIslandsCount++;
                    continue;
                }
                else
                {
                    AddAbyss(islandIndex);
                    continue;
                }
            }
            // Clone world seed (needed file)
            Log("Copying seed");
            try
            {
                File.Copy(currentWorldDirectory + "Seed.sdd",
                    localWorldDirectory + "Seed.sdd");
            }
            catch (Exception)
            {
                Log("Error copying seed, file not found : \n" + localWorldDirectory + "Seed.sdd");
                MessageBox.Show("Error copying seed, file not found : \n" + localWorldDirectory + "Seed.sdd");
                return;
            }
        }

        private static void CheckAndAddAsAvailable(string currentWorldDirectory, List<int> availableProceduralIslands, int missionPosition)
        {
            foreach (string mapDirectory in Directory.EnumerateDirectories(Path.Combine(currentWorldDirectory, missionPosition.ToString())))
            {
                if (mapDirectory.Contains("MISSION"))
                    return;
                foreach (string mapFile in Directory.EnumerateFiles(mapDirectory))
                {
                    if (mapFile.Contains("MISSION"))
                        return;
                }
            }

            availableProceduralIslands.Add(missionPosition);
        }

        public string AddIsland(string currentWorldDirectory, int islandIndex, bool useCustomIsland, Dictionary<string, string> customMaps, ref List<int> availableIslands, ref List<int> burntIslands)
        {
            string target = Path.Combine(Directory.GetCurrentDirectory().ToString(), "World\\", islandIndex.ToString());

            // Trying to get custom island if asked for
            if (useCustomIsland && customMaps.Count > 0)
            {
                if (customMaps.Count == 0)
                {
                    Log("No custom island left in list, switching to procedural island");
                }
                else
                {
                    return AddCustomIsland(islandIndex, ref customMaps, ref availableIslands, target);
                }
            }

            // Trying to add a procedural island
            Log("Adding a procedural island");
            int sourceIsland = islandIndex;
            string sourceIslandDirectory = Path.Combine(currentWorldDirectory, islandIndex.ToString());
            if (burntIslands.Contains(sourceIsland))
            {
                if (availableIslands.Count > 0)
                {
                    sourceIsland = availableIslands[0];
                }
                else
                {
                    Log("No procedural island left in list, switching to custom");
                    if (customMaps.Count == 0)
                    {
                        // algorithm stuck
                        throw new Exception();
                    }
                    else
                    {
                        return AddCustomIsland(islandIndex, ref customMaps, ref availableIslands, target);
                    }
                }
            }
            else
            {
                IEnumerable<string> temp = Directory.EnumerateDirectories(sourceIslandDirectory);
                foreach (string fileOrDir in temp)
                {
                    if (fileOrDir.Contains(mission0)
                        || fileOrDir.Contains(mission1)
                        || fileOrDir.Contains(mission2)
                        || fileOrDir.Contains(mission3))
                    {
                        if (availableIslands.Count > 0)
                        {
                            sourceIsland = availableIslands[0];
                        }
                        else
                        {
                            Log("No procedural island left in list, switching to custom");
                            if (customMaps.Count == 0)
                            {
                                // algorithm stuck
                                throw new Exception();
                            }
                            else
                            {
                                return AddCustomIsland(islandIndex, ref customMaps, ref availableIslands, target);
                            }
                        }
                        break;
                    }
                }
            }
            sourceIslandDirectory = Path.Combine(currentWorldDirectory, sourceIsland.ToString());
            // Add the island (which is not a mission) from the original world
            CopyFilesRecursively(new DirectoryInfo(sourceIslandDirectory),
                new DirectoryInfo(target));
            availableIslands.Remove(sourceIsland);
            burntIslands.Add(sourceIsland);
            return null;
        }

        private string AddCustomIsland(int islandIndex, ref Dictionary<string, string> customMaps, ref List<int> availableIslands, string target)
        {
            Log("Adding a custom island");
            // pick a selected random custom map
            Random randomMap = new Random();
            int customMapKeyIndex = randomMap.Next(0, customMaps.Count - 1);
            string customMapKey = customMaps.Keys.ElementAt(customMapKeyIndex);

            // add the island from the currently available custom maps
            string mapDirectoryName = customMaps[customMapKey];
            mapDirectoryName = mapDirectoryName.Substring(mapDirectoryName.LastIndexOf('\\') + 1);
            Directory.CreateDirectory(Path.Combine(target, mapDirectoryName));
            CopyFilesRecursively(new DirectoryInfo(customMaps[customMapKey]),
                new DirectoryInfo(Path.Combine(target, mapDirectoryName)));

            availableIslands.Add(islandIndex);

            return customMapKey;
        }

        #endregion

        #region New algorithm

        //private void GenerateWithSize(string currentWorldDirectory, string localWorldDirectory, bool useMissions, bool mixMapsChecked, bool startDeep, Dictionary<string, string> selectedCustomMaps, WorldType worldType, int bigIslandsProportion, Dictionary<int, int> proceduralIslandsSizes)
        //{
        //    // how many selected islands do we want to have ?
        //    Random r = new Random();
        //    int customsMapscount = selectedCustomMaps.Keys.Count;
        //    int currentIslandsCount = 0;
        //    int customIslandProbability = Math.Min((int)((double)customsMapscount / (double)worldType * 100), 100);
        //    // custom map density adjustment
        //    bool mixMaps = mixMapsChecked && (customsMapscount >= (int)((double)worldType / 2.0));

        //    // if mixMaps, we want an equal probability of customMaps and non custom maps
        //    // if the number of selected custom islands is lower than the size of the world mixmaps is set to false
        //    if (mixMaps || customIslandProbability > 50 && mixMapsChecked)
        //    {
        //        customIslandProbability = 50;
        //    }

        //    // special case : not enough custom islands to populate, algorithm forced to mix
        //    if (customsMapscount < (int)worldType)
        //    {
        //        mixMaps = true;
        //    }

        //    // new dictionary for big and small islands
        //    // dictionary where value : 0 small, 1 big
        //    Dictionary<int, int> availableProceduralIslandsWithSize = new Dictionary<int, int>();

        //    // First pass, place missions
        //    List<int> missionPositions = new List<int>();
        //    if (useMissions)
        //    {
        //        int missionPosition = r.Next(1, 48);
        //        missionPositions.Add(missionPosition);
        //        Log("Adding mission : " + mission0);
        //        AddMission(missionPosition, mission0);
        //        CheckAndAddAsAvailableWithSize(currentWorldDirectory, availableProceduralIslandsWithSize, missionPosition);

        //        while (missionPositions.Contains(missionPosition))
        //        {
        //            missionPosition = r.Next(1, 48);
        //        }
        //        missionPositions.Add(missionPosition);
        //        Log("Adding mission : " + mission1);
        //        AddMission(missionPosition, mission1);
        //        CheckAndAddAsAvailableWithSize(currentWorldDirectory, availableProceduralIslandsWithSize, missionPosition);

        //        while (missionPositions.Contains(missionPosition))
        //        {
        //            missionPosition = r.Next(1, 48);
        //        }
        //        missionPositions.Add(missionPosition);
        //        Log("Adding mission : " + mission2);
        //        AddMission(missionPosition, mission2);
        //        CheckAndAddAsAvailableWithSize(currentWorldDirectory, availableProceduralIslandsWithSize, missionPosition);

        //        while (missionPositions.Contains(missionPosition))
        //        {
        //            missionPosition = r.Next(1, 48);
        //        }
        //        missionPositions.Add(missionPosition);
        //        Log("Adding mission : " + mission3);
        //        AddMission(missionPosition, mission3);
        //        CheckAndAddAsAvailableWithSize(currentWorldDirectory, availableProceduralIslandsWithSize, missionPosition);
        //    }

        //    // main algorithm loop
        //    List<int> burntProceduralIslands = new List<int>();
        //    for (int islandIndex = 0; islandIndex < maxIslands; islandIndex++)
        //    {
        //        // skip positionned missions
        //        if (missionPositions.Contains(islandIndex))
        //        {
        //            continue;
        //        }

        //        string target = Path.Combine(localWorldDirectory, islandIndex.ToString());
        //        Directory.CreateDirectory(target);

        //        //int islandProbability = r.Next(0, 25);
        //        int islandProbability = r.Next(0, maxIslands);
        //        bool putIsland = (islandProbability <= (int)worldType);

        //        if (islandIndex == 0)
        //        {
        //            // if start in the open, add abyss
        //            if (startDeep)
        //            {
        //                AddAbyss(islandIndex);
        //                continue;
        //            }
        //            // else spawn on island
        //            putIsland = true;
        //        }

        //        // if there are already too many islands, add water
        //        if (currentIslandsCount >= (int)worldType)
        //        {
        //            AddAbyss(islandIndex);
        //            continue;
        //        }

        //        // if random probability puts island, add island
        //        if (putIsland)
        //        {
        //            int customIslandProbabilityToken = r.Next(0, 100);
        //            // putCustomIsland says if the island to be added should be a procedural island
        //            bool putCustomIsland = (customIslandProbabilityToken <= customIslandProbability);
        //            // the returned string is the name of the custom map that was added if it was not a procedural island
        //            string addedCustomMapAvoidRedundency = AddIslandWithSize(currentWorldDirectory, islandIndex, putCustomIsland, selectedCustomMaps, ref availableProceduralIslandsWithSize, ref burntProceduralIslands);
        //            // if we added a custom island
        //            if (!String.IsNullOrEmpty(addedCustomMapAvoidRedundency))
        //            {
        //                // a custom island was added, we track the procedural island in that position to be available for further use
        //                if (selectedCustomMaps.ContainsKey(addedCustomMapAvoidRedundency))
        //                    selectedCustomMaps.Remove(addedCustomMapAvoidRedundency);
        //                CheckAndAddAsAvailableWithSize(currentWorldDirectory, availableProceduralIslandsWithSize, islandIndex);
        //            }
        //            else
        //            {
        //                // procedural island cannot be used anymore to avoid redundancy
        //                burntProceduralIslands.Add(islandIndex);
        //            }
        //            // keep track of how many islands were added
        //            currentIslandsCount++;
        //            continue;
        //        }
        //        else
        //        {
        //            AddAbyss(islandIndex);
        //            continue;
        //        }
        //    }
        //    // Clone world seed (needed file)
        //    Log("Copying seed");
        //    try
        //    {
        //        File.Copy(currentWorldDirectory + "Seed.sdd",
        //            localWorldDirectory + "Seed.sdd");
        //    }
        //    catch (Exception)
        //    {
        //        Log("Error copying seed, file not found : \n" + localWorldDirectory + "Seed.sdd");
        //        MessageBox.Show("Error copying seed, file not found : \n" + localWorldDirectory + "Seed.sdd");
        //        return;
        //    }
        //}

        //private static void CheckAndAddAsAvailableWithSize(string currentWorldDirectory, Dictionary<int, int> availableProceduralIslandsWithSize, int missionPosition)
        //{
        //    foreach (string mapDirectory in Directory.EnumerateDirectories(Path.Combine(currentWorldDirectory, missionPosition.ToString())))
        //    {
        //        if (mapDirectory.Contains("MISSION"))
        //            return;
        //        foreach (string mapFile in Directory.EnumerateFiles(mapDirectory))
        //        {
        //            if (mapFile.Contains("MISSION"))
        //                return;
        //        }

        //        // check here if big or small island

        //    }

        //    availableProceduralIslandsWithSize.Add(missionPosition, 0);
        //}

        //public string AddIslandWithSize(string currentWorldDirectory, int islandIndex, bool useCustomIsland, Dictionary<string, string> customMaps, ref Dictionary<int, int> availableIslandsWithSize, ref List<int> burntIslands)
        //{
        //    string target = Path.Combine(Directory.GetCurrentDirectory().ToString(), "World\\", islandIndex.ToString());

        //    // Trying to get custom island if asked for
        //    if (useCustomIsland && customMaps.Count > 0)
        //    {
        //        if (customMaps.Count == 0)
        //        {
        //            Log("No custom island left in list, switching to procedural island");
        //        }
        //        else
        //        {
        //            return AddCustomIslandWithSize(islandIndex, ref customMaps, ref availableIslandsWithSize, target);
        //        }
        //    }

        //    // Trying to add a procedural island
        //    Log("Adding a procedural island");
        //    int sourceIsland = islandIndex;
        //    string sourceIslandDirectory = Path.Combine(currentWorldDirectory, islandIndex.ToString());
        //    if (burntIslands.Contains(sourceIsland))
        //    {
        //        if (availableIslandsWithSize.Keys.Count > 0)
        //        {
        //            sourceIsland = availableIslandsWithSize.Keys.First();
        //        }
        //        else
        //        {
        //            Log("No procedural island left in list, switching to custom");
        //            if (customMaps.Count == 0)
        //            {
        //                // algorithm stuck
        //                throw new Exception();
        //            }
        //            else
        //            {
        //                return AddCustomIslandWithSize(islandIndex, ref customMaps, ref availableIslandsWithSize, target);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        IEnumerable<string> temp = Directory.EnumerateDirectories(sourceIslandDirectory);
        //        foreach (string fileOrDir in temp)
        //        {
        //            if (fileOrDir.Contains(mission0)
        //                || fileOrDir.Contains(mission1)
        //                || fileOrDir.Contains(mission2)
        //                || fileOrDir.Contains(mission3))
        //            {
        //                if (availableIslandsWithSize.Keys.Count > 0)
        //                {
        //                    sourceIsland = availableIslandsWithSize.Keys.First();
        //                }
        //                else
        //                {
        //                    Log("No procedural island left in list, switching to custom");
        //                    if (customMaps.Count == 0)
        //                    {
        //                        // algorithm stuck
        //                        throw new Exception();
        //                    }
        //                    else
        //                    {
        //                        return AddCustomIslandWithSize(islandIndex, ref customMaps, ref availableIslandsWithSize, target);
        //                    }
        //                }
        //                break;
        //            }
        //        }
        //    }
        //    sourceIslandDirectory = Path.Combine(currentWorldDirectory, sourceIsland.ToString());
        //    // Add the island (which is not a mission) from the original world
        //    CopyFilesRecursively(new DirectoryInfo(sourceIslandDirectory),
        //        new DirectoryInfo(target));
        //    availableIslandsWithSize.Remove(sourceIsland);
        //    burntIslands.Add(sourceIsland);
        //    return null;
        //}

        //private string AddCustomIslandWithSize(int islandIndex, ref Dictionary<string, string> customMaps, ref Dictionary<int, int> availableIslandsWithSize, string target)
        //{
        //    Log("Adding a custom island");
        //    // pick a selected random custom map
        //    Random randomMap = new Random();
        //    int customMapKeyIndex = randomMap.Next(0, customMaps.Count - 1);
        //    string customMapKey = customMaps.Keys.ElementAt(customMapKeyIndex);

        //    // add the island from the currently available custom maps
        //    string mapDirectoryName = customMaps[customMapKey];
        //    mapDirectoryName = mapDirectoryName.Substring(mapDirectoryName.LastIndexOf('\\') + 1);
        //    Directory.CreateDirectory(Path.Combine(target, mapDirectoryName));
        //    CopyFilesRecursively(new DirectoryInfo(customMaps[customMapKey]),
        //        new DirectoryInfo(Path.Combine(target, mapDirectoryName)));

        //    availableIslandsWithSize.Add(islandIndex, 0);

        //    return customMapKey;
        //}

        #endregion

        private void LogClear()
        {
            tbLog.Clear();
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
            tbLog.AppendText(message + "\r\n");
        }

        public void AddMission(int islandIndex, string missionName)
        {
            string target = Path.Combine(Directory.GetCurrentDirectory().ToString(), "World\\", islandIndex.ToString());

            string missionsPath = tbMissionsPath.Text;
            IEnumerable<string> temp = Directory.EnumerateDirectories(missionsPath);

            foreach (string fileOrDir in temp)
            {
                if (fileOrDir.Contains(missionName))
                {
                    Directory.CreateDirectory(Path.Combine(target, missionName));
                    // copy files
                    CopyFilesRecursively(new DirectoryInfo(Path.Combine(missionsPath, missionName)),
                        new DirectoryInfo(Path.Combine(target, missionName)));
                    break;
                }
            }
        }

        

        public void AddAbyss(int islandIndex)
        {
            Log("Adding an abyss");
            string target = Path.Combine(Directory.GetCurrentDirectory().ToString(), "World\\", islandIndex.ToString());
            CopyFilesRecursively(new DirectoryInfo("./assets/abyss"), new DirectoryInfo(target));
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
            {
                if (file.Name.Contains(".bak"))
                    continue;

                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }
        }

        private void bBackupWorld_Click(object sender, EventArgs e)
        {
            string slot = GetSlot();
            if (String.IsNullOrEmpty(slot))
            {
                Log("No slot selected !");
                return;
            }

            string currentWorldDirectory = Path.Combine(tbDataDirectory.Text, slot, "World\\");
            if (CheckCurrentWorldDirectory(currentWorldDirectory))
            {
                BackupCurrentWorld(currentWorldDirectory);
            }
        }

        private bool CheckCurrentWorldDirectory(string currentWorldDirectory)
        {
            if (!Directory.Exists(currentWorldDirectory))
            {
                Log("Invalid World directory : \n" + tbSDDefaultDirectory.Text);
                MessageBox.Show("Invalid World directory : \n" + tbSDDefaultDirectory.Text);
                return false;
            }

            return true;
        }

        private bool BackupCurrentWorld(string currentWorldDirectory)
        {
            string slot = GetSlot();
            if (String.IsNullOrEmpty(slot))
            {
                Log("No slot selected !");
                return false;
            }

            Log("Backup existing world");
            if (!Directory.Exists(currentWorldDirectory))
            {
                Log("Invalid World directory : \n" + currentWorldDirectory);
                MessageBox.Show("Invalid World directory : \n" + currentWorldDirectory);
                return false;
            }
            string targetBackupDirectory = Path.Combine(tbDataDirectory.Text, slot, "World_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            CopyFilesRecursively(new DirectoryInfo(currentWorldDirectory),
                new DirectoryInfo(targetBackupDirectory));

            Log("Backup complete as : " + targetBackupDirectory);

            return true;
        }

        private void bDeleteWorld_Click(object sender, EventArgs e)
        {
            string slot = GetSlot();
            if (String.IsNullOrEmpty(slot))
            {
                Log("No slot selected !");
                return;
            }

            string currentWorldDirectory = Path.Combine(tbDataDirectory.Text, slot, "World\\");
            if (CheckCurrentWorldDirectory(currentWorldDirectory))
            {
                DeleteCurrentWorld(currentWorldDirectory);
            }
        }

        private bool DeleteCurrentWorld(string currentWorldDirectory)
        {
            Log("Deleting current world");
            if (Directory.Exists(currentWorldDirectory))
            {
                try
                {
                    try
                    {
                        DeleteDirectory(currentWorldDirectory);                //throws if directory doesn't exist.
                    }
                    catch
                    {
                        //HACK because the recursive delete will throw with an "Directory is not empty." 
                        //exception after it deletes all the contents of the diretory if the directory
                        //is open in the left nav of Windows's explorer tree.  This appears to be a caching
                        //or queuing latency issue.  Waiting 2 secs for the recursive delete of the directory's
                        //contents to take effect solved the issue for me.  Hate it I do, but it was the only
                        //way I found to work around the issue.
                        System.Threading.Thread.Sleep(2000);     //wait 2 seconds
                        DeleteDirectory(currentWorldDirectory);
                    }
                }
                catch (Exception ex)
                {
                    Log("Could not delete game directory : \n" + currentWorldDirectory);
                    MessageBox.Show("Could not delete game directory : \n" + currentWorldDirectory);
                    return false;
                }
            }
            Log("Current world deleted");
            return true;
        }

        // Directory deletion hack
        public static void DeleteDirectory(string target_dir)
        {
            if (!Directory.Exists(target_dir))
                return;

            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        private void bSearchDataDirectory_Click(object sender, EventArgs e)
        {
            LogClear();
            ClearLocalMaps();
            //AutoDetectDirectories();
            DirectoriesDetector.AutoDetectDirectories(tbDataDirectory, tbMapsDirectory, tbSDDefaultDirectory, tbMissionsPath, tbWorkshopPath, ReadMissions, Log);

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.UserProfile;
                fbd.SelectedPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tbDataDirectory.Text = fbd.SelectedPath;
                    Log("New Data path set : " + tbDataDirectory.Text);
                    tbMapsDirectory.Text = Path.Combine(tbDataDirectory.Text, "Maps\\");
                }
            }

            LoadGeneratorNew();
        }

        private void bSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbMaps.Items.Count; i++)
            {
                clbMaps.SetItemChecked(i, true);
            }
        }

        private void bUnselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbMaps.Items.Count; i++)
            {
                clbMaps.SetItemChecked(i, false);
            }
        }

        private void bRestore_Click(object sender, EventArgs e)
        {
            string slot = GetSlot();
            if (String.IsNullOrEmpty(slot))
            {
                Log("No slot selected !");
                return;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                fbd.SelectedPath = Path.Combine(tbDataDirectory.Text, slot);
                fbd.Description = "Select a world backup directory in the Editor directory, with the following format : World_<DATE>_<TIME>, for example World_20201116_200251";
                DialogResult result = fbd.ShowDialog();

                Regex regex = new Regex(@"^*\\World_\d{8}_\d{6}$");

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && regex.IsMatch(fbd.SelectedPath))
                {
                    // confirmation
                    if (MessageBox.Show("Are you sure you want to restore the game world, from this directory : \n" 
                        + fbd.SelectedPath 
                        + "\nto \n" 
                        + Path.Combine(tbDataDirectory.Text, slot, "World")
                        + " ?", "Please confirm world restoration", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        return;
                    }

                    string worldBackupDirectory = fbd.SelectedPath;
                    try
                    {
                        Log("Checking backup structure");
                        // Iterate through the 49 directories
                        for (int islandIndex = 0; islandIndex < maxIslands; islandIndex++)
                        {
                            IEnumerable<string> temp = Directory.EnumerateFiles(Path.Combine(worldBackupDirectory, islandIndex.ToString()));
                            foreach (string fileOrDir in temp)
                            {

                            }
                        }
                        try
                        {
                            string gameWorldDirectory = Path.Combine(tbDataDirectory.Text, slot, "World\\");
                            for (int islandIndex = 0; islandIndex < maxIslands; islandIndex++)
                            {
                                // Clean current island in world directory
                                DeleteDirectory(Path.Combine(gameWorldDirectory, islandIndex.ToString()));
                                // Replace with backuped one
                                Log("Copying Island " + islandIndex);
                                string target = Path.Combine(gameWorldDirectory, islandIndex.ToString());
                                DirectoryCopy(Path.Combine(worldBackupDirectory, islandIndex.ToString()), target, true);
                            }
                            // Restore seed
                            Log("Restoring seed");
                            try
                            {
                                File.Copy(worldBackupDirectory + "\\Seed.sdd",
                                    gameWorldDirectory + "\\Seed.sdd", true);
                            }
                            catch (Exception)
                            {
                                Log("Error copying seed, file not found : \n" + worldBackupDirectory + "\\Seed.sdd");
                                MessageBox.Show("Error copying seed, file not found : \n" + worldBackupDirectory + "\\Seed.sdd");
                                return;
                            }

                            Log("World Restored");
                            MessageBox.Show("World restored");
                        }
                        catch (Exception ex)
                        {
                            Log("Error restoring world : " + ex.Message);
                            MessageBox.Show("Error restoring world : " + ex.Message);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        Log("Invalid world directory structure : \n" + worldBackupDirectory);
                        MessageBox.Show("Invalid world directory structure : \n" + worldBackupDirectory);
                        return;
                    }
                }
                else
                {
                    Log("Invalid world backup directory");
                    MessageBox.Show("Invalid world backup directory");
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private void bSearchInstallationDir_Click(object sender, EventArgs e)
        {
            LogClear();
            ClearWorkshopMaps();
            //AutoDetectDirectories();
            DirectoriesDetector.AutoDetectDirectories(tbDataDirectory, tbMapsDirectory, tbSDDefaultDirectory, tbMissionsPath, tbWorkshopPath, ReadMissions, Log);

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                fbd.Description = "Locate the installation directory of Stranded Deep, usually : <SteamDirectory>\\steamapps\\common\\Stranded Deep";

                string steamDir = "";
                try
                {
                    string steamInstallPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string;
                    steamDir = steamInstallPath;
                }
                catch { }

                fbd.SelectedPath = String.IsNullOrEmpty(tbSDDefaultDirectory.Text) && Directory.Exists(tbSDDefaultDirectory.Text) ? "" : tbSDDefaultDirectory.Text;

                DialogResult result = fbd.ShowDialog();
                Regex regex = new Regex(@"^*\\Stranded Deep|\\StrandedDeep$");

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && regex.IsMatch(fbd.SelectedPath))
                {
                    tbSDDefaultDirectory.Text = fbd.SelectedPath;
                    Log("New Stranded Deep path set : " + tbSDDefaultDirectory.Text);

                    string possibleMissionsPath = Path.Combine(fbd.SelectedPath, "Stranded_Deep_Data\\Data\\Default Maps");
                    ReadMissions(possibleMissionsPath);

                    // locate workshop maps
                    tbWorkshopPath.Clear();
                    try
                    {
                        if (!String.IsNullOrEmpty(tbSDDefaultDirectory.Text) && Directory.Exists(tbSDDefaultDirectory.Text))
                        {
                            string sdInstallPath = tbSDDefaultDirectory.Text;
                            if (sdInstallPath.IndexOf("steamapps") > 0)
                            {
                                string steamLibraryPath = sdInstallPath.Substring(0, sdInstallPath.IndexOf("steamapps"));
                                if (Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\"))
                                    && Directory.Exists(Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId)))
                                {
                                    tbWorkshopPath.Text = Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId);
                                }
                                else
                                {
                                    Log("Could not locate workshop (Directory : " + Path.Combine(steamLibraryPath, "steamapps\\workshop\\content\\" + strandedDeepSteamId) + " does not exist)");
                                }
                            }
                        }
                    }
                    catch { }

                    if (String.IsNullOrEmpty(tbWorkshopPath.Text))
                    {
                        Log("Could not guess Workshop path from installation dir");
                    }
                }
                else
                {
                    Log("Invalid path selected");
                    tbSDDefaultDirectory.Clear();
                    tbMissionsPath.Clear();
                    tbWorkshopPath.Clear();
                }
            }

            LoadGeneratorNew();
        }

        private void ReadMissions(string possibleMissionsPath)
        {
            if (!Directory.Exists(possibleMissionsPath))
            {
                Log("Could not locate missions (Directory " + possibleMissionsPath + " does not exist)");
                tbMissionsPath.Text = "";
                return;
            }

            IEnumerable<string> temp = Directory.EnumerateDirectories(possibleMissionsPath);

            int missionsFound = 0;

            foreach (string fileOrDir in temp)
            {
                if (fileOrDir.Contains(mission0)
                    || fileOrDir.Contains(mission1)
                    || fileOrDir.Contains(mission2)
                    || fileOrDir.Contains(mission3))
                    missionsFound++;
            }

            bool isValidDir = (missionsFound == 4);
            if (isValidDir)
            {
                Log("Missions successfully found in : " + possibleMissionsPath);
                tbMissionsPath.Text = Path.Combine(possibleMissionsPath);
                cbUseMissions.Enabled = true;
                cbUseMissions.Checked = true;
            }
            else
            {
                Log("Missions could not be found in : " + possibleMissionsPath);
                tbMissionsPath.Text = "";
                cbUseMissions.Checked = false;
                cbUseMissions.Enabled = false;
            }
        }

        private void clbMaps_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        clbMaps.SelectedIndex = clbMaps.IndexFromPoint(e.X, e.Y);
                        contextMenuOpenDirectory.Show(this, new Point(clbMaps.Left + e.X, clbMaps.Top + e.Y));
                    }
                    break;
            }
        }

        private void TsiOpenMapDirectory_Click(object sender, EventArgs e)
        {
            if (clbMaps.SelectedItem == null)
                return;

            string mapDirectoryName = mapDictionary[clbMaps.SelectedItem.ToString()];

            // open explorer on this directory
            Process.Start(mapDirectoryName);
        }

        private void bHowto_Click(object sender, EventArgs e)
        {
            RandomizerInstructions instructions = new RandomizerInstructions();
            instructions.ShowDialog();
        }

        private void bRefreshSlots_Click(object sender, EventArgs e)
        {
            DetectSlots();
        }

        private void DetectSlots()
        {
            if (!Directory.Exists(tbDataDirectory.Text))
                return;

            rbSlot0.Enabled = false;
            rbSlot0.Checked = false;

            rbSlot1.Enabled = false;
            rbSlot1.Checked = false;

            rbSlot2.Enabled = false;
            rbSlot2.Checked = false;

            rbSlot3.Enabled = false;
            rbSlot3.Checked = false;

            foreach (string directory in Directory.EnumerateDirectories(tbDataDirectory.Text))
            {
                if (directory.Contains("Slot0"))
                    rbSlot0.Enabled = true;
                if (directory.Contains("Slot1"))
                    rbSlot1.Enabled = true;
                if (directory.Contains("Slot2"))
                    rbSlot2.Enabled = true;
                if (directory.Contains("Slot3"))
                    rbSlot3.Enabled = true;
            }
        }

        private string GetSlot()
        {
            if (rbSlot0.Checked)
                return "Slot0";

            if (rbSlot1.Checked)
                return "Slot1";

            if (rbSlot2.Checked)
                return "Slot2";

            if (rbSlot3.Checked)
                return "Slot3";

            return null;
        }
    }
}
