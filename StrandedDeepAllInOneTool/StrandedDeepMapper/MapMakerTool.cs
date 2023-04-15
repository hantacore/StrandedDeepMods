using ImageMagick;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrandedDeepMapper
{
    public partial class MapMakerTool : UserControl
    {
        public MapMakerTool()
        {
            InitializeComponent();

            lAlreadyspawners.Visible = false;
            lNoshark.Visible = false;
            lOldFormat.Visible = false;
            bAddSharkSpawners.Enabled = false;

            cbTigerShark.Enabled = false;
            cbGoblinShark.Enabled = false;
            cbGreatWhite.Enabled = false;
            cbHammerhead.Enabled = false;
            cbWhaleShark.Enabled = false;

            tsiOpenMapDirectory.Click += TsiOpenMapDirectory_Click;

            InitObjectRandomizer();

#warning only for publication
            //tcTools.TabPages.Remove(tpRandomize);
            //tcTools.TabPages.Remove(tpPopulate);
        }

        const string OLD_EDITOR = "0.17";

        const string PATROL_TIGERSHARK = "PATROL_TIGERSHARK";
        const string PATROL_HAMMERHEAD = "PATROL_HAMMERHEAD";

        const string GOBLINSHARK_prefab = "ece551ac-ce6f-43c1-a6a7-62a2898e7ad9";
        const string GREATWHITE_prefab = "6587fe7a-5ff0-4929-8d64-38bb1881a241";
        const string HAMMERHEAD_prefab = "076ba5de-bbda-4d8e-ba7f-0ad2369ab499";
        const string TIGERSHARK_prefab = "87cf2163-f1a9-4260-ba20-ce583ac8922d";
        const string WHALESHARK_prefab = "7b304494-2996-42d1-a442-0d02c6cebaf5";

        const string PATROL_HAMMERHEAD_prefab = "334";//"4b5344ac-7019-4fec-95eb-01c3afab966b";
        const string PATROL_TIGERSHARK_prefab = "218";//"9fd88c0a-f6ba-4c74-b387-8394151b9ab3";
        const string PATROL_GOBLIN_prefab = "335";//"d05cae70-0925-4452-9ff4-86b54d0eaf61";

        Dictionary<string, string> mapDictionary = new Dictionary<string, string>();
        Dictionary<string, string> proceduralMapDictionary = new Dictionary<string, string>();
        Dictionary<string, string> proceduralMapContentDictionary = new Dictionary<string, string>();
        bool isLoaded = false;
        string steamId = null;

        private void MapMakerTool_Load(object sender, EventArgs e)
        {
            if (isLoaded)
                return;

            try
            {
                this.Text += " - " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // Looking for SD user directory
                DirectoriesDetector.AutoDetectDirectories(tbSDDirectory, tbMapsDirectory, null, null, null, null, Log);
                //tbSDDirectory.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\"); ;
                //tbMapsDirectory.Text = Path.Combine(tbSDDirectory.Text, "Maps\\");


                // Look for savegame
                //Regex regex = new Regex(@"^\d{17}$"); // steam id formats
                //IEnumerable<string> dirList = Directory.EnumerateDirectories(tbSDDirectory.Text);
                //foreach (string dir in dirList)
                //{
                //    if (regex.IsMatch(dir.Substring(dir.LastIndexOf("\\") + 1)))
                //    {
                //        steamId = dir.Substring(dir.LastIndexOf("\\") + 1);
                //        tbSavegamePath.Text = Path.Combine(dir, steamId + ".json");
                //    }
                //}

#warning : tools not really functionning
                tcTools.TabPages.Remove(tpRandomize);
                tcTools.TabPages.Remove(tpPopulate);

                LoadGenerator();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            isLoaded = true;
        }

        private void bSearchDataDirectory_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.UserProfile;
                fbd.SelectedPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tbSDDirectory.Text = fbd.SelectedPath;
                    tbMapsDirectory.Text = Path.Combine(tbSDDirectory.Text, "Maps\\");

                    LoadGenerator();
                }
            }
        }

        private void LoadGenerator()
        {
            lbMaps.Items.Clear();
            mapDictionary.Clear();
            proceduralMapDictionary.Clear();
            proceduralMapContentDictionary.Clear();
            bAddSharkSpawners.Enabled = false;
            bImport.Enabled = false;

            #region load editor maps

            // Locating Maps directory and listing compatible maps in checkedlistbox
            IEnumerable<string> mapsList = Directory.EnumerateDirectories(tbMapsDirectory.Text);
            foreach (string mapDirectory in mapsList)
            {
                //Log("Parsing map : " + mapDirectory);
                foreach (string mapFile in Directory.EnumerateFiles(mapDirectory))
                {
                    if (mapFile.Contains("EDITOR"))
                    {
                        string key = null;
                        try
                        {
                            //key = MapParser.Parse_EDITOR_FileNew(mapFile);
                            //key = MapParser.Parse_EDITOR_File_revamp(mapFile);
                            key = MapParser.Parse_EDITOR_FileDeserialize(mapFile);
                        }
                        catch (FormatException fe)
                        {
                            Log(fe.Message);
                            break;
                        }
                        if (!String.IsNullOrEmpty(key))
                        {
                            lbMaps.Items.Add(key);
                            mapDictionary.Add(key, mapDirectory);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            #endregion


        }

        private void Log(string message)
        {
            Console.WriteLine(message);
            tbLog.AppendText(message + "\r\n");
        }

        #region handle sharks

        private void lbMaps_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        lbMaps.SelectedIndex = lbMaps.IndexFromPoint(e.X, e.Y);
                        contextMenuOpenDirectory.Show(this, new Point(lbMaps.Left + e.X, lbMaps.Top + e.Y));
                    }
                    break;
            }
        }

        private void TsiOpenMapDirectory_Click(object sender, EventArgs e)
        {
            if (lbMaps.SelectedItem == null)
                return;

            string mapDirectoryName = mapDictionary[lbMaps.SelectedItem.ToString()];

            // open explorer on this directory
            Process.Start(mapDirectoryName);
        }

        private void lbMaps_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(ToolStripMenuItem tsiBackup in contextMenuOpenDirectory.Items)
            {
                tsiBackup.Click -= TsiBackup_Click;
            }
            contextMenuOpenDirectory.Items.Clear();

            if (lbMaps.SelectedItem == null)
                return;

            contextMenuOpenDirectory.Items.Add(tsiOpenMapDirectory);
            string selectedItem = lbMaps.SelectedItem.ToString();

            // read backups
            List<string> backups = DirectoriesDetector.GetBackupNames(mapDictionary[lbMaps.SelectedItem.ToString()]);
            foreach (string file in backups)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    int objects = 0;
                    if (!String.IsNullOrEmpty(json) && json != "null")
                    {
                        dynamic prefabs = JsonConvert.DeserializeObject(json);
                        objects = prefabs.Count;
                    }

                    ToolStripMenuItem tsiBackup = new ToolStripMenuItem("Restore " + Path.GetFileName(file) + " (" + objects + " objects)");
                    tsiBackup.Tag = file;
                    contextMenuOpenDirectory.Items.Add(tsiBackup);
                    tsiBackup.Click += TsiBackup_Click;
                }
                catch
                {
                    Log("Could not read backup file : " + file);
                }
            }

            UpdateSharkUI(selectedItem);
            UpdateObjectRandomizer(selectedItem);
        }

        private void TsiBackup_Click(object sender, EventArgs e)
        {
            string backupName = ((ToolStripMenuItem)sender).Tag.ToString();
            // Restore the backup file stored in the tag
            DirectoriesDetector.RestoreBackup(backupName, Log);
        }

        private void UpdateSharkUI(string selectedItem)
        {
            if (selectedItem.Contains(OLD_EDITOR))
            {
                lAlreadyspawners.Visible = false;
                lNoshark.Visible = false;

                lOldFormat.Visible = true;

                bAddSharkSpawners.Enabled = false;

                cbTigerShark.Enabled = false;
                cbGoblinShark.Enabled = false;
                cbGreatWhite.Enabled = false;
                cbHammerhead.Enabled = false;
                cbWhaleShark.Enabled = false;

                return;
            }
            else
            {
                lOldFormat.Visible = false;
            }

            string sourceFileName = "";
            string sourceDirectory = mapDictionary[selectedItem];
            foreach (string fileOrDir in Directory.EnumerateFiles(sourceDirectory))
            {
                if (fileOrDir.Contains("_OBJECT.map") && !fileOrDir.Contains(".bak"))
                {
                    sourceFileName = fileOrDir;
                    break;
                }
            }

            // Parse map OBJECT file
            string json = File.ReadAllText(sourceFileName);
            dynamic array = JsonConvert.DeserializeObject(json);

            // Look for shark spawner prefab
            if (json.Contains(PATROL_TIGERSHARK_prefab) || json.Contains(PATROL_HAMMERHEAD_prefab))
            {
                lAlreadyspawners.Visible = true;
            }
            else
            {
                lAlreadyspawners.Visible = false;
            }

            if (!json.Contains(TIGERSHARK_prefab)
                && !json.Contains(HAMMERHEAD_prefab)
                && !json.Contains(GREATWHITE_prefab)
                && !json.Contains(WHALESHARK_prefab)
                && !json.Contains(GOBLINSHARK_prefab)
                )
            {
                lNoshark.Visible = true;
                bAddSharkSpawners.Enabled = false;
            }
            else
            {
                lNoshark.Visible = false;
                bAddSharkSpawners.Enabled = true;
            }

            cbTigerShark.SelectedIndex = -1;
            cbGoblinShark.SelectedIndex = -1;
            cbGreatWhite.SelectedIndex = -1;
            cbHammerhead.SelectedIndex = -1;
            cbWhaleShark.SelectedIndex = -1;

            cbTigerShark.Enabled = (json.Contains(TIGERSHARK_prefab));
            cbGoblinShark.Enabled = (json.Contains(GOBLINSHARK_prefab));
            cbGreatWhite.Enabled = (json.Contains(GREATWHITE_prefab));
            cbHammerhead.Enabled = (json.Contains(HAMMERHEAD_prefab));
            cbWhaleShark.Enabled = (json.Contains(WHALESHARK_prefab));
        }

        private void bAddSharkSpawners_Click(object sender, EventArgs e)
        {
            Log("");
            string selectedMap = lbMaps.SelectedItem.ToString();
            if (String.IsNullOrEmpty(selectedMap))
            {
                Log("Error : no map selected");
                return;
            }

            string destFileName = DirectoriesDetector.GetOBJECTFile(mapDictionary[selectedMap]);
            if (!File.Exists(destFileName))
            {
                Log("Error : OBJECT file not found");
                return;
            }

            Log("Adding spawners to " + selectedMap);

            // backup
            Log("Making backup");
            if (!DirectoriesDetector.MakeBackup(destFileName, "BeforeSharkSpawners"))
                return;

            // add sharks spawners
            string json = File.ReadAllText(destFileName);
            dynamic array = JsonConvert.DeserializeObject(json);

            for (int itemIndex = 0; itemIndex < array.Count; itemIndex++)
            {
                dynamic item = array[itemIndex];

                string prefab = null;
                if (item["prefab"] != null)
                {
                    prefab = item["prefab"].ToString();
                }

                string addPrefab = null;
                JObject itemToAdd = null;
                if (!String.IsNullOrEmpty(prefab) && prefab.Contains(TIGERSHARK_prefab)
                    && !String.IsNullOrEmpty(cbTigerShark.Text))
                {
                    addPrefab = cbTigerShark.Text;
                    Log("Found TIGERSHARK node");
                }
                if (!String.IsNullOrEmpty(prefab) && prefab.Contains(GOBLINSHARK_prefab)
                    && !String.IsNullOrEmpty(cbGoblinShark.Text))
                {
                    addPrefab = cbGoblinShark.Text;
                }
                if (!String.IsNullOrEmpty(prefab) && prefab.Contains(GREATWHITE_prefab)
                    && !String.IsNullOrEmpty(cbGreatWhite.Text))
                {
                    addPrefab = cbGreatWhite.Text;
                }
                if (!String.IsNullOrEmpty(prefab) && prefab.Contains(HAMMERHEAD_prefab)
                    && !String.IsNullOrEmpty(cbHammerhead.Text))
                {
                    addPrefab = cbHammerhead.Text;
                }
                if (!String.IsNullOrEmpty(prefab) && prefab.Contains(WHALESHARK_prefab)
                    && !String.IsNullOrEmpty(cbWhaleShark.Text))
                {
                    addPrefab = cbWhaleShark.Text;
                }

                if (!String.IsNullOrEmpty(addPrefab))
                {
                    itemToAdd = new JObject();
                    if (PATROL_TIGERSHARK == addPrefab)
                    {
                        itemToAdd["prefab"] = PATROL_TIGERSHARK_prefab;
                        Log("Adding PATROL_TIGERSHARK node");
                    }
                    if (PATROL_HAMMERHEAD == addPrefab)
                    {
                        itemToAdd["prefab"] = PATROL_HAMMERHEAD_prefab;
                        Log("Adding PATROL_HAMMERHEAD node");
                    }
                }

                if (itemToAdd != null)
                {
                    itemToAdd["Transform"] = new JObject();
                    itemToAdd["Transform"]["localPosition"] = new JObject();
                    itemToAdd["Transform"]["localPosition"]["x"] = item["Transform"]["localPosition"]["x"];
                    itemToAdd["Transform"]["localPosition"]["y"] = item["Transform"]["localPosition"]["y"];
                    itemToAdd["Transform"]["localPosition"]["z"] = item["Transform"]["localPosition"]["z"];

                    itemToAdd["Transform"]["localRotation"] = new JObject();
                    itemToAdd["Transform"]["localRotation"]["x"] = item["Transform"]["localRotation"]["x"];
                    itemToAdd["Transform"]["localRotation"]["y"] = item["Transform"]["localRotation"]["y"];
                    itemToAdd["Transform"]["localRotation"]["z"] = item["Transform"]["localRotation"]["z"];
                    itemToAdd["Transform"]["localRotation"]["w"] = item["Transform"]["localRotation"]["w"];

                    itemToAdd["Transform"]["localScale"] = new JObject();
                    itemToAdd["Transform"]["localScale"]["x"] = item["Transform"]["localScale"]["x"];
                    itemToAdd["Transform"]["localScale"]["y"] = item["Transform"]["localScale"]["y"];
                    itemToAdd["Transform"]["localScale"]["z"] = item["Transform"]["localScale"]["z"];

                    itemToAdd["respawnCounter"] = "0";
                    itemToAdd["spawnDistance"] = "110";

                    array.Add(itemToAdd);
                    Log("JSON udpated");
                }
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(destFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, array);
            }
            Log("File written, all done");
            MessageBox.Show("All done, Enjoy your sharks !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string selectedItem = lbMaps.SelectedItem.ToString();
            UpdateSharkUI(selectedItem);
        }

        #endregion

        #region map import

        private void bImport_Click(object sender, EventArgs e)
        {
            string mapPath = proceduralMapDictionary[lbProcmaps.SelectedItem.ToString()];
            string mapContent = "null";
            if (proceduralMapContentDictionary.ContainsKey(lbProcmaps.SelectedItem.ToString()))
            {
                mapContent = proceduralMapContentDictionary[lbProcmaps.SelectedItem.ToString()];
            }
            ImportMap(mapPath, mapContent);
        }

        private void ImportMap(string mapPath, string objectFileContent)
        {
            Log("");
            // create Guid
            Guid guid = Guid.NewGuid();
            Log("Create new Guid : " + guid.ToString());
            string localMapDirectory = Path.Combine(Directory.GetCurrentDirectory().ToString(), "MAP_" + guid.ToString());

            // create local directory with new Guid
            Log("Create local directory with new Guid");
            Directory.CreateDirectory(localMapDirectory);

            // copy heightmap and object files and change the name
            Log("Copy heightmap and object files and change the file names");
            foreach (string fileOrDir in Directory.EnumerateFiles(mapPath))
            {
                if (fileOrDir.Contains("_OBJECT.map") && !fileOrDir.Contains(".bak"))
                {
                    File.Copy(fileOrDir, Path.Combine(localMapDirectory, "MAP_" + guid.ToString() + "_OBJECT.map"), true);
                }
                else if (fileOrDir.Contains("_HEIGHT.map") && !fileOrDir.Contains(".bak"))
                {
                    File.Copy(fileOrDir, Path.Combine(localMapDirectory, "MAP_" + guid.ToString() + "_HEIGHT.map"), true);
                }
            }

            // copy the generic editor file and change its name
            Log("Copy the generic EDITOR file and change its name");
            string target_EDITOR_File = Path.Combine(localMapDirectory, "MAP_" + guid.ToString() + "_EDITOR.map");
            File.Copy(Path.Combine(Directory.GetCurrentDirectory().ToString(), "assets\\MAP_DUMMY_EDITOR.map"), target_EDITOR_File, true);

            // update the editor file
            Log("Update the dummy EDITOR file");
            string editorFileContent = File.ReadAllText(target_EDITOR_File, Encoding.Default);
            editorFileContent = editorFileContent.Replace("00000000-0000-0000-0000-000000000000", guid.ToString());
            editorFileContent = editorFileContent.Replace("IMPORTED2020-01-01-00:00:00", "IMPORTED" + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"));
            editorFileContent = editorFileContent.Replace("11/19/2020", DateTime.Now.ToString("dd/MM/yyyy"));

            File.WriteAllText(target_EDITOR_File, editorFileContent, Encoding.Default);

            // import the objects found in the savegame
            Log("Importing the objects found in the savegame");
            string locaObjectFile = Path.Combine(localMapDirectory, "MAP_" + guid.ToString() + "_OBJECT.map");
            File.WriteAllText(locaObjectFile, objectFileContent);

            // copy the result in the Editor maps
            Log("Copy the result in the Editor maps");
            string targetDirectory = Path.Combine(tbMapsDirectory.Text, "MAP_" + guid.ToString());
            Directory.CreateDirectory(targetDirectory);
            CopyFilesRecursively(new DirectoryInfo(localMapDirectory),
                new DirectoryInfo(targetDirectory));

            Log("All done, enjoy your map");
            MessageBox.Show("All done, enjoy your map in the editor !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Log("Cleaning up");
            Directory.Delete(localMapDirectory, true);
            Log("");

            // update UI
            LoadGenerator();
        }

        #endregion

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        private void lbProcmaps_SelectedIndexChanged(object sender, EventArgs e)
        {
            bImport.Enabled = (lbProcmaps.SelectedIndex != -1);
        }

        private void bLocate_savegame_Click(object sender, EventArgs e)
        {
            proceduralMapDictionary.Clear();
            proceduralMapContentDictionary.Clear();
            lbProcmaps.Items.Clear();

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.UserProfile;
                fbd.SelectedPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string selectedPath = fbd.SelectedPath;
                    if (!selectedPath.Contains("Slot0") && !selectedPath.Contains("Slot1") && !selectedPath.Contains("Slot2") && !selectedPath.Contains("Slot3"))
                    {
                        // bad path
                        Log("Bad path, no savegame slot directory found");
                        return;
                    }

                    tbSavegamePath.Text = selectedPath;

                    //if (false)
                    //{
                    //    return;
                    //}

                    #region load procedural maps of the world

                    string worldDirectory = Path.Combine(selectedPath, "World\\");
                    IEnumerable<string> worldmapsList = Directory.EnumerateDirectories(worldDirectory);
                    int currentIslandIndex = -1;

                    string saveFile = Path.Combine(tbSavegamePath.Text, "data.json");
                    if (File.Exists(saveFile))
                    {
                        Log("Analyzing savegame for discovered islands");

                        // Parse map savegame file
                        string json = File.ReadAllText(saveFile);
                        dynamic array = JsonConvert.DeserializeObject(json);

                        foreach (string positionDirectory in worldmapsList)
                        {
                            //Log("Parsing world map : " + positionDirectory);
                            currentIslandIndex = int.Parse(positionDirectory.Substring(positionDirectory.LastIndexOf('\\') + 1));

                            IEnumerable<string> currentMapDirectoryList = Directory.EnumerateDirectories(positionDirectory);
                            foreach (string mapDirectory in currentMapDirectoryList)
                            {
                                if (!String.IsNullOrEmpty(mapDirectory)
                                    && mapDirectory.Contains("PROCEDURAL"))
                                {
                                    string key = null;

                                    foreach (string mapFile in Directory.EnumerateFiles(mapDirectory))
                                    {
                                        if (mapFile.Contains("EDITOR"))
                                        {
                                            try
                                            {
                                                //key = MapParser.Parse_EDITOR_FileNew(mapFile);
                                                //key = MapParser.Parse_EDITOR_File_revamp(mapFile);
                                                key = MapParser.Parse_EDITOR_FileDeserialize(mapFile);
                                            }
                                            catch (FormatException fe)
                                            {
                                                Log(fe.Message);
                                            }
                                            break;
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(key))
                                    {
                                        // Check if island is discovered in the json
                                        if (array["Persistent"] != null
                                            && array["Persistent"]["StrandedWorld"] != null
                                            && array["Persistent"]["StrandedWorld"]["Zones"] != null)
                                        {
                                            if (array["Persistent"]["StrandedWorld"]["Zones"][currentIslandIndex] != null
                                                && array["Persistent"]["StrandedWorld"]["Zones"][currentIslandIndex]["Discovered"] == "true"
                                                && array["Persistent"]["StrandedWorld"]["Zones"][currentIslandIndex]["Objects"] != null)
                                            {
                                                string keyWithIndex = "(" + currentIslandIndex + ") " + key;
                                                proceduralMapContentDictionary.Add(keyWithIndex, array["Persistent"]["StrandedWorld"]["Zones"][currentIslandIndex]["Objects"].ToString());

                                                lbProcmaps.Items.Add(keyWithIndex);
                                                proceduralMapDictionary.Add(keyWithIndex, mapDirectory);
                                            }
                                        }
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
                        // bad path
                        Log("No savegame found in directory");
                    }
                    #endregion
                }
            }
        }

        #region Randomize objects

        private Dictionary<string, string> prefabs = new Dictionary<string, string>();

        private void InitObjectRandomizer()
        {
            tbMinScale.Text = "1";
            tbMaxScale.Text = "1";
            tbMinScalePopulate.Text = "1";
            tbMaxScalePopulate.Text = "1";

            //prefabs.Add("CRAB", "43411343-c9ce-4c39-9e60-21cd0267babf");//1"
            //prefabs.Add("ARCHER", "ccbbd8a5-297b-4c92-8bc2-433e64592e87");//2"
            //prefabs.Add("CLOWN_TRIGGERFISH", "0d6d7598-05b5-42eb-a1fd-b321601abdc3");//3"
            //prefabs.Add("COD", "acb93be3-317f-4ba7-8696-f83c73de7dbd");//4"
            //prefabs.Add("DISCUS", "460f86e0-d4d7-4aee-aece-90a401d496b7");//5"
            //prefabs.Add("PILCHARD", "18c3332b-db42-4549-8a09-ff35ba9e3981");//6"
            //prefabs.Add("SARDINE", "bcf2cd92-e1a8-4818-97b3-d3b968877924");//7"
            //prefabs.Add("SHARK_GREAT WHITE_RAGDOLL", "842c3380-2a07-4d77-b510-346db96eec15");//8"
            //prefabs.Add("SHARK_TIGER_SHARK_RAGDOLL", "e31952be-642f-4daa-adda-7cb24fcccf75");//9"
            //prefabs.Add("SHARK - TIGER", "87cf2163-f1a9-4260-ba20-ce583ac8922d");//10"
            //prefabs.Add("SHARK - GREAT WHITE", "6587fe7a-5ff0-4929-8d64-38bb1881a241");//11"
            //prefabs.Add("PALM_FROND", "9fc25c22-8d48-46af-bf33-2436872f7238");//12"
            //prefabs.Add("ROCK", "395145d6-667f-444c-8759-8730e0dde3ea");//13"
            //prefabs.Add("ROPE_COIL", "a3e06b32-a034-4c90-9988-2e4614f1db2e");//14"
            //prefabs.Add("STICK", "7b6c4e93-8240-48d4-8cda-8e03de8534d3");//15"
            //prefabs.Add("WATER_BOTTLE", "acbd2636-a99e-42c8-97d0-03abdff3bba8");//16"
            //prefabs.Add("CAN_BEANS", "d54f6384-078a-4944-9261-3b4a0b9684fb");//17"
            //prefabs.Add("COCONUT_DRINKABLE", "e143272f-0645-41c4-a718-f4e9afd8eca7");//18"
            //prefabs.Add("COCONUT_HALF", "55310fe7-f654-4556-b46b-98b54f3bc5d5");//19"
            prefabs.Add("SHIPWRECK_2A", "7011c1ea-19dc-4659-b322-55e8c19810cf");//20"
            prefabs.Add("SHIPWRECK_3A", "a9362e9a-6b51-4933-a94d-009fae158a78");//21"
            prefabs.Add("SHIPWRECK_4A", "dafddf11-8b85-45ad-96ee-483ebc60189d");//22"
            prefabs.Add("SHIPWRECK_5A", "67cbd440-1cc7-4256-ba5a-2ecdd51bfc94");//23"
            prefabs.Add("SHIPWRECK_6A", "afac356f-1673-4393-926c-524e43c1b7b7");//24"
            //prefabs.Add("COMPASS", "057bf96c-b164-4235-82ee-420c9f84b86d");//25"
            //prefabs.Add("DUCTTAPE", "8f1b26d9-df83-4c9a-91b5-2d44bc75504f");//26"
            //prefabs.Add("FLARE_GUN", "60253c31-9eea-4126-98d7-585e94171176");//27"
            //prefabs.Add("FUELCAN", "4351ef7e-df4b-4ff4-88c4-2ae4ba0133db");//28"
            //prefabs.Add("LANTERN", "25445bfd-5e14-4151-ba29-4f7e486c04fd");//29"
            prefabs.Add("YUCCA", "c2aee3d0-81c7-48ee-82c5-7799da276c0a");//30"
            //prefabs.Add("EGG_DEADEX", "ca64a5ed-4270-4335-b34a-368e01d84af5");//31"
            //prefabs.Add("EGG_WOLLIE", "c3b5237e-e76b-4878-9312-e42042b1ef1d");//32"
            //prefabs.Add("MARLIN", "511a4a85-917a-4706-bd13-8cb4808fd46a");//33"
            //prefabs.Add("WHALE", "9fa22c4e-43fc-406e-b995-5561e6690ab9");//34"
            prefabs.Add("ROWBOAT_3", "c1675d85-48ab-4832-9c81-ec6653413c02");//35"
            //prefabs.Add("MARLIN_RAGDOLL", "f3e83145-55db-4e2b-9d27-7eab413c517f");//36"
            //prefabs.Add("LIONFISH", "5eadd99e-c39c-480d-b5c4-f81e4237a65a");//37"
            prefabs.Add("SHIPWRECK_7A", "c920ab66-5bc3-4dd8-8bd4-068f9e7870b6");//38"
            //prefabs.Add("CLOTH", "23ea146d-f2e7-41ae-91a4-fb548c0567d4");//39"
            //prefabs.Add("TORCH", "9a9eda86-3bbd-47fe-8822-d869479674c8");//40"
            //prefabs.Add("LABEL_MAKER", "fef469ed-ee46-4819-8be6-b7a11e40cf6b");//41"
            //prefabs.Add("SeaFort_1", "bd2f1b13-6462-4d8a-85c4-5740a73efc84");//42"
            //prefabs.Add("SeaFort_2", "29801f3f-44a6-44ed-a285-eb9b2453dde2");//43"
            //prefabs.Add("SeaFort_Brige", "7c3ae2af-efda-4bf4-a3b3-f2dfd7eb8dab");//44"
            //prefabs.Add("SeaFort_Brige_Broken", "301438df-5d31-4216-be65-79b1cfb7db12");//45"
            prefabs.Add("DRIFTWOOD_PILE", "fa750994-3baa-4c82-912e-6b0dc41512ad");//46"
            prefabs.Add("FICUS_1", "21b69409-771d-402a-b467-33a17d25f60d");//47"
            prefabs.Add("FICUS_2", "cb5007cb-2793-4aa4-b0a6-32571b88e2bd");//48"
            prefabs.Add("FICUS_3", "d1393df4-aa6a-45e4-bf4e-c65ccfdd0a18");//49"
            prefabs.Add("ALOCASIA_1", "e111961d-6f88-448b-846a-0cb8361bd3c7");//50"
            prefabs.Add("ALOCASIA_2", "619cda60-8a6a-4033-b710-c47ede04294b");//51"
            prefabs.Add("BANANA_PLANT", "ab424fff-939f-4c1a-b37d-3a531c9eddcd");//52"
            prefabs.Add("BIGROCK_1", "a4dfc2d9-1943-49d8-b038-480c66833178");//53"
            prefabs.Add("BIGROCK_2", "eb674e64-8c3b-46fb-9849-df66950afd2f");//54"
            prefabs.Add("BIGROCK_3", "a3cee4f7-2141-4ab6-90b7-4aa9a781c38a");//55"
            prefabs.Add("BIGROCK_4", "f03bf993-1acd-47e5-bd13-b6bb9f393432");//56"
            prefabs.Add("BIGROCK_5", "33471b70-b7b7-4a8d-8fc4-ba259f796142");//57"
            prefabs.Add("CERIMAN_1", "39e21b01-d179-4ead-bfa5-6880c781f95e");//58"
            prefabs.Add("CERIMAN_2", "d21a9aa9-9ed6-4857-94f1-f01a2233eac7");//59"
            prefabs.Add("CERIMAN_3", "210a6276-ae4d-40ef-8d7a-b5292c624886");//60"
            prefabs.Add("SHORELINE_ROCK_1", "8174b310-dc11-4e0a-bb1a-eb7d7f949e34");//61"
            prefabs.Add("SHORELINE_ROCK_2", "b83e383f-4e85-472d-a558-64cd4688681b");//62"
            prefabs.Add("SMALLROCK_1", "a35e77b5-362d-40b4-b692-b0132c2e8597");//63"
            prefabs.Add("SMALLROCK_2", "98fcdff5-c3a7-4f7f-8cbe-23703b532229");//64"
            prefabs.Add("SMALLROCK_3", "f387c318-e153-4467-a532-d000aadac3bc");//65"
            prefabs.Add("FICUS_TREE", "196fb328-44e1-4c79-97cb-9e035dd74625");//66"
            prefabs.Add("FICUS_TREE_2", "7b756d2e-ea68-471a-b57b-43b7c494a636");//67"
            prefabs.Add("CLIFF_001", "8c73c480-6889-4629-8abb-c1535a8b4820");//68"
            prefabs.Add("CLIFF_002", "6a987283-e1fa-4360-a499-537cd6952073");//69"
            prefabs.Add("CLIFF_003", "9e9c04b5-9c0f-4822-837b-287ea36193a3");//70"
            prefabs.Add("CLIFF_004", "c7bfa28b-4b9e-4d8e-8022-46bf2c7c9f8d");//71"
            prefabs.Add("CLIFF_005", "1a1c5490-c90b-4043-9bdf-5a75142e24bf");//72"
            prefabs.Add("CLIFF_006", "50ef3473-a3c9-4937-8b33-3b91eafbe695");//73"
            //prefabs.Add("BANDAGE", "d4dce0bc-6132-48c2-82c1-064e4aaa0302");//74"
            //prefabs.Add("CORRUGATED_FLOOR", "cb83abc6-ea98-4636-82de-0e9923131f23");//75"
            //prefabs.Add("CORRUGATED_FOUNDATION", "28756137-1362-45bd-968b-19e125da3ec3");//76"
            //prefabs.Add("CORRUGATED_PANEL_STATIC", "71ee2b7f-34a5-4eda-a491-989dd5f2d403");//77"
            //prefabs.Add("DRIFTWOOD_FOUNDATION", "8ba9ee34-a2fc-4c0b-8daf-1bb65f1e73c0");//78"
            //prefabs.Add("DRIFTWOOD_FLOOR", "842f27b6-7eac-41a2-9b25-8f83c530b1fd");//79"
            //prefabs.Add("DRIFTWOOD_PANEL_STATIC", "9f459001-5b97-42d9-953e-e159bb472a18");//80"
            //prefabs.Add("WOOD_FOUNDATION", "4ad6223f-2d8c-458c-a90b-bd6590c42da2");//81"
            //prefabs.Add("WOOD_FLOOR", "88ea294a-5dac-4bd2-80eb-d9d4fd4f84e6");//82"
            //prefabs.Add("WOOD_PANEL_STATIC", "a74ebc5c-bafd-4c0d-b47f-fbf1c744cfcb");//83"
            //prefabs.Add("WOOD_STEPS", "997dda40-fffc-4d1f-8d5b-9782adc8eab9");//84"
            //prefabs.Add("TARP_PANEL", "a5a45df3-7561-4def-a619-60cb5133cf88");//85"
            //prefabs.Add("TARP_PANEL_STATIC", "bb0c8f70-6c83-40cf-acae-0acb4ee8c09a");//86"
            //prefabs.Add("PLANK_FOUNDATION", "e2f44ae2-96c1-4afe-b3fe-0b59e7a8a928");//87"
            //prefabs.Add("PLANK_FLOOR", "4e0a891b-08f9-4a0c-baa2-b20abf8d385a");//88"
            //prefabs.Add("PLANK_PANEL_STATIC", "46835218-0fed-438e-85f4-890660c1c1b5");//89"
            //prefabs.Add("SHIPPING_CONTAINER_DOOR", "2c3433db-2991-44f7-80c6-03fdf6b3bd60");//90"
            //prefabs.Add("SHIPPING_CONTAINER_DOOR_STATIC", "a2d764ea-8c47-41f1-9e65-820cdc3f72d7");//91"
            //prefabs.Add("SHIPPING_CONTAINER_PANEL", "4ae673a7-9f89-4792-9da7-2b50ee857fee");//92"
            //prefabs.Add("SHIPPING_CONTAINER_PANEL_STATIC", "79d08faf-6735-41f5-9f08-b63d592c2868");//93"
            //prefabs.Add("SHIPPING_CONTAINER_1", "48e3d2d3-19aa-485a-920f-13d14e06b1ed");//94"
            //prefabs.Add("SHIPPING_CONTAINER_2", "1ca90151-b3f4-4c70-afdc-3fa24f280d92");//95"
            //prefabs.Add("SHIPPING_CONTAINER_3", "17e3e8b4-66ec-4a67-9d94-1513becfdcb6");//96"
            //prefabs.Add("STRUCTURE", "377e8597-9c12-42b6-87e1-1fc470cc6e05");//97"
            //prefabs.Add("SCRAP_PLANK", "f2d74763-2a81-442f-8dc6-24fd032a7d70");//98"
            //prefabs.Add("SCRAP_CORRUGATED", "c5b6a13f-3200-4656-a378-d0099a7786c8");//99"
            //prefabs.Add("CORRUGATED_STEPS", "4d0b51c2-bfa6-4136-954c-2134d7ea6525");//100"
            //prefabs.Add("CORRUGATED_WEDGE_FLOOR", "ca19619e-ec45-438b-bb7b-24ebc036b5d3");//101"
            //prefabs.Add("CORRUGATED_WEDGE_FOUNDATION", "7bfc7093-d326-451d-a265-b3c1b9ac18fe");//102"
            //prefabs.Add("DRIFTWOOD_STEPS", "d6c48eba-ea03-40c2-9472-9eddedd82fe9");//103"
            //prefabs.Add("DRIFTWOOD_WEDGE_FLOOR", "fa091d36-bd43-49a7-a8e3-2034fa3aff8c");//104"
            //prefabs.Add("DRIFTWOOD_WEDGE_FOUNDATION", "60fe78d6-f570-4033-bf15-3e32b373fdee");//105"
            //prefabs.Add("PLANK_STEPS", "f31a7d04-dd37-4b00-9f1c-98e44c845f32");//106"
            //prefabs.Add("PLANK_WEDGE_FLOOR", "df047588-8c34-466e-b1e0-66c00094b1a5");//107"
            //prefabs.Add("PLANK_WEDGE_FOUNDATION", "ca457558-ee7c-42b3-8187-a392b746e20e");//108"
            //prefabs.Add("WOOD_WEDGE_FLOOR", "d6c6f38a-ea22-4027-8eb3-3c50c564f36e");//109"
            //prefabs.Add("WOOD_WEDGE_FOUNDATION", "247cb7a3-cbb1-401d-833f-518e79bfd590");//110"
            //prefabs.Add("DRIFTWOOD_STICK", "c19dfbbc-1dc0-40ff-9197-ea1721bc067a");//111"
            //prefabs.Add("DRIFTWOOD_ARCH", "eec0e2c1-59f0-4c1d-8ae9-1657dbc1235a");//112"
            //prefabs.Add("DRIFTWOOD_DOOR", "9d66d2fa-4d25-41d8-ae18-37889af87808");//113"
            //prefabs.Add("WOOD_ARCH", "0476e02c-986e-48ce-b719-764d7b9bce24");//114"
            //prefabs.Add("WOOD_DOOR", "a88e99d0-6dc0-449a-a5a7-cf633d339ceb");//115"
            //prefabs.Add("PLANK_ARCH", "49c648c8-215c-4df9-9422-c6c925c13a42");//116"
            //prefabs.Add("PLANK_DOOR", "5e055da7-2dd4-4bad-8607-612bec8cc509");//117"
            //prefabs.Add("CORRUGATED_ARCH", "65c76f93-a6c2-4963-9c04-8f6605e1b9f0");//118"
            //prefabs.Add("CORRUGATED_DOOR", "392231ce-1f0d-4802-ad3b-b03251c05def");//119"
            //prefabs.Add("DRIFTWOOD_WALL_HALF", "fed5650f-e0f1-4572-8819-517887229d2c");//120"
            //prefabs.Add("WOOD_WALL_HALF", "c0c318c7-845b-49e1-a6d5-7361fe5c3db4");//121"
            //prefabs.Add("PLANK_WALL_HALF", "4a387ca8-a3eb-4c4d-8a55-d902782f9fb1");//122"
            //prefabs.Add("CORRUGATED_WALL_HALF", "5cec9d23-157a-4465-b7b7-7912ab6a2ea4");//123"
            //prefabs.Add("DRIFTWOOD_WALL_WINDOW", "e6fd076c-f98b-4e49-839d-b5cf90f32139");//124"
            //prefabs.Add("WOOD_WALL_WINDOW", "e614ee17-4467-4d51-a3f2-d4faa61de89e");//125"
            //prefabs.Add("PLANK_WALL_WINDOW", "8b8ae65b-d44c-4a6f-8ca1-a04bef6ba244");//126"
            //prefabs.Add("CORRUGATED_WALL_WINDOW", "94bf455d-36f2-429e-801d-c6b5c9ff1d74");//127"
            //prefabs.Add("VEHICLE_LIFERAFT", "330f40ee-b992-416f-96f9-763903d02364");//128"
            //prefabs.Add("VEHICLE_SAIL", "56e1a425-ffc9-425a-b77b-7fe9573637f6");//129"
            //prefabs.Add("VEHICLE_MOTOR", "5328828f-45db-47c6-bcb3-480fb840e927");//130"
            //prefabs.Add("STRUCTURE_RAFT", "2357c1a3-c30a-4bd5-a9a6-502f25dbdc4e");//131"
            //prefabs.Add("STRUCTURE_SMALL", "7163a266-f608-433f-87b2-ed6c912f689c");//132"
            //prefabs.Add("RAFT_BASE_WOOD_BUNDLE", "0e3c0d69-4c20-454f-8b79-7bd1dbdf545d");//133"
            //prefabs.Add("RAFT_BASE_BALLS", "567f43b8-8173-49ce-9a49-34c9a0830731");//134"
            //prefabs.Add("RAFT_BASE_BARREL", "409f5db6-782f-4515-a730-c01a2bb5ef07");//135"
            //prefabs.Add("RAFT_BASE_TYRE", "1f60cdb4-7389-44c4-a0f8-330d8e1ec880");//136"
            //prefabs.Add("RAFT_FLOOR_DRIFTWOOD", "eeb19123-7bd6-4aa9-8066-bf286da34de6");//137"
            //prefabs.Add("RAFT_FLOOR_WOOD", "b20d0523-abb4-4fb8-a04d-34db818c8309");//138"
            //prefabs.Add("RAFT_FLOOR_PLANK", "c2bdfa59-c998-49b1-9c58-f5c3a557375c");//139"
            //prefabs.Add("RAFT_FLOOR_CORRUGATED", "9700fd3f-6280-47ea-9397-a537d0fbb25c");//140"
            //prefabs.Add("RAFT_FLOOR_CLAY", "4b73c15d-f1ab-4f64-8fa3-d42d12d4a02c");//141"
            //prefabs.Add("BARREL", "89f85c94-973f-4258-9765-d62597765de4");//142"
            //prefabs.Add("BARREL_PILE", "09e012c4-f30e-46af-95e1-710e076f4bdc");//143"
            //prefabs.Add("TYRE", "5393ee38-d96f-4f6c-af28-bad4d00dfa29");//144"
            //prefabs.Add("TYRE_PILE", "dd067ddf-32e5-4eaa-97be-6a4f528a3430");//145"
            //prefabs.Add("BUOYBALL", "182a94dd-f2c3-4c35-ac82-d2c425a73f27");//146"
            //prefabs.Add("BUOYBALL_PILE", "86e5dca2-b0e8-4066-b921-8913aadb7837");//147"
            prefabs.Add("KURA_TREE", "0acf9803-d90e-40e7-8ea6-4c9a202b70bc");//148"
            prefabs.Add("QUWAWA_TREE", "a8985b09-e0d0-4190-b7f9-d317fd6f52dc");//149"
            prefabs.Add("KURA_FRUIT", "e72cb7b4-bbc7-4ba9-ac52-ab1f3e6c708c");//150"
            prefabs.Add("QUWAWA_FRUIT", "b62aa0c2-ebe8-4d61-9062-a2f98328c201");//151"
            //prefabs.Add("FARMING_PLOT_WOOD", "f76188e9-d2b6-4d10-995d-6823259034a8");//152"
            //prefabs.Add("FARMING_PLOT_PLANK", "23e8f2bb-6b7b-48c4-a678-026f03377d39");//153"
            //prefabs.Add("FARMING_PLOT_CORRUGATED", "1e8f0786-8edf-46d8-8f5a-3ec8c5f29310");//154"
            //prefabs.Add("FARMING_HOE", "b7dcd1d1-e507-4ea0-aa2e-54b44d7d3efe");//155"
            //prefabs.Add("COCONUT_ORANGE", "1fa27dd5-56b2-4053-993b-fd796f992621");//156"
            prefabs.Add("PALM_1", "ce11fcdf-42c9-4e27-bb42-3a095968604d");//157"
            prefabs.Add("PALM_2", "251f7106-f305-46bd-8dcd-1fe069a3710b");//158"
            prefabs.Add("PALM_3", "ed21d921-caf8-444d-a699-c13aae7dc5c6");//159"
            prefabs.Add("PALM_4", "dc6a69b9-98af-4a0f-93b1-28214d92378f");//160"
            //prefabs.Add("PALM_TOP", "0dd07e20-fb76-4a10-83b8-fa5d6e294178");//161"
            //prefabs.Add("PALM_LOG_1", "8c7caa9c-11b7-4c9c-8dd9-2e4c2ae3dbcd");//162"
            //prefabs.Add("PALM_LOG_2", "28bfcbcd-882e-405b-82fb-34b2e7ab4e32");//163"
            //prefabs.Add("PALM_LOG_3", "375627c9-5266-4351-95c9-aac41afe91e8");//164"
            //prefabs.Add("DOOR", "aa4d693d-2940-4fe2-815e-4e816a829f08");//165"
            //prefabs.Add("BED", "c977b6d0-c57d-441d-87a0-09680b5e5607");//166"
            //prefabs.Add("BED_SHELTER", "b0d05625-1220-4076-8b99-f030a2138444");//167"
            //prefabs.Add("NEW_CAMPFIRE", "8395c366-006f-4bff-86c3-338cc6f78897");//168"
            //prefabs.Add("NEW_CAMPFIRE_PIT", "c6809782-c736-4150-8712-9c34694a25a9");//169"
            //prefabs.Add("NEW_CAMPFIRE_SPIT", "2c30ef2e-7ba2-49f0-bae1-944c841defc1");//170"
            //prefabs.Add("HOBO_STOVE", "330fc98c-21db-4a50-8b37-b477a5111bff");//171"
            //prefabs.Add("SMOKER", "7f8adcd1-cc14-4c72-8d9b-c06deb668f90");//172"
            //prefabs.Add("STONE_TOOL", "8102b4fe-1891-4938-8eda-39d9ffc3ad85");//173"
            //prefabs.Add("LEAVES_FIBROUS", "c2286bd6-26ec-4ed3-8ba7-2cc3cedbf6d4");//174"
            //prefabs.Add("WATER_STILL", "e0407767-8e6f-4f52-8deb-b1b0c9bd94ec");//175"
            //prefabs.Add("NEW_CRUDE_SPEAR", "ee24fc1f-a3c8-442d-84f4-aa56e712d9ca");//176"
            //prefabs.Add("NEW_ARROW", "fb0e55f2-36ae-48bd-ac31-1f1fec6d3fe3");//177"
            //prefabs.Add("BAT", "56193a0a-28ec-49d3-ba08-8bd934b34600");//178"
            //prefabs.Add("SEAGULL", "7e623581-5234-44e6-b0e2-bd75504477c7");//179"
            prefabs.Add("YOUNG_PALM_1", "86c84e0d-3966-42dc-ba7a-0ba08aad6886");//180"
            prefabs.Add("YOUNG_PALM_2", "b04c4d3e-194b-4c2e-826e-39856f3486f3");//181"
            //prefabs.Add("BOAR", "225d0de8-76a7-4f95-9853-f318b3d20e48");//182"
            //prefabs.Add("BOAR_RAGDOLL", "2cb8dafd-4787-4a6e-9658-8b3d7e3a086c");//183"
            //prefabs.Add("SNAKE", "cbf681ba-9e43-498f-8e6a-6e4a0cf83b5e");//184"
            //prefabs.Add("SNAKE_RAGDOLL", "4d3eabfe-afad-4d87-9007-e0d8a4a8cf78");//185"
            //prefabs.Add("HIDESPOT_SNAKE", "4f660c7a-61da-4e94-8c18-6ab1e232d869");//186"
            //prefabs.Add("KINDLING", "5e356d39-bf61-47e6-a8cf-d1258a0e0139");//187"
            //prefabs.Add("MEAT_LARGE", "b7a43ec6-6027-4436-942b-06bc2bac527a");//188"
            //prefabs.Add("MEAT_MEDIUM", "5c684512-eb68-4862-8853-47d4a8868e37");//189"
            //prefabs.Add("MEAT_SMALL", "a97788af-d6b9-482b-bccf-1f9606455219");//190"
            //prefabs.Add("NEW_AIRTANK", "fdaa4887-b28f-4913-b105-00d903a4334a");//191"
            //prefabs.Add("NEW_CRUDE_AXE", "bcd0ea5b-378e-4797-92b4-fc5e1fc77382");//192"
            //prefabs.Add("NEW_CRUDE_BOW", "660b9b3-1df3-444d-9f5e-b23b5e8570b8");//193"
            //prefabs.Add("NEW_CRUDE_HAMMER", "dd078b94-88f1-4ffd-b8c3-35799e71437f");//194"
            //prefabs.Add("NEW_FISHING_SPEAR", "501a274a-9cb9-4bb0-9be3-4dc6d2973969");//195"
            //prefabs.Add("NEW_REFINED_AXE", "fe4524c8-f759-41e9-802e-bbc1040bfbed");//196"
            //prefabs.Add("NEW_REFINED_HAMMER", "6894f864-2488-4e49-9c1a-776f0033230f");//197"
            //prefabs.Add("NEW_REFINED_KNIFE", "3001dcd8-7a96-46cf-8e6d-43caf79a6ce9");//198"
            //prefabs.Add("NEW_REFINED_SPEAR", "fd1adb5c-da4d-405f-8a1f-61f45205887e");//199"
            //prefabs.Add("NEW_SPEARGUN", "104b457b-6612-4a98-b6ed-b095f9ea9f66");//200"
            //prefabs.Add("NEW_SPEARGUN_ARROW", "7a47e18e-81bd-42cc-8354-920a38ed1f3d");//201"
            prefabs.Add("PINE_1", "4fafe01c-57c2-4e61-a2b6-b6489af1fbf9");//202"
            prefabs.Add("PINE_2", "391b9669-a801-45ce-b598-20de010c6f94");//203"
            prefabs.Add("PINE_3", "b23a2f9d-ed63-4547-acc7-8ff6f8d56396");//204"
            prefabs.Add("COCA_BUSH", "0fcbd1c9-bad3-4aae-b560-9e768fa91c77");//205"
            prefabs.Add("PINE_SMALL_2", "7f5a8b67-0256-4c55-8055-bd930d85a762");//206"
            prefabs.Add("PINE_SMALL_3", "432c2a11-e62f-4b3a-981e-7be0dc37703d");//207"
            //prefabs.Add("FIRE_TORCH", "06f9eeb9-5d24-47f0-a8bd-1a5f8ae5b866");//208"
            //prefabs.Add("WOOD_HOOK", "8d105451-9c1c-415e-adae-cfbaa6d245d0");//209"
            //prefabs.Add("WOOD_SHELF", "ac34d78d-46be-4f74-9108-0a43517d32bc");//210"
            //prefabs.Add("PLANK_SHELF", "56bfcb0c-b36c-4359-bd21-a4286a7740c0");//211"
            //prefabs.Add("CORRUGATED_SHELF", "439847ec-e0ef-438c-9e1f-03fbf330a388");//212"
            //prefabs.Add("WOOD_CHAIR", "3b069099-8525-46b9-b11e-ccb2c8897840");//213"
            //prefabs.Add("PLANK_CHAIR", "7145e07b-f76c-49a1-964f-6903cc1e80fb");//214"
            //prefabs.Add("WOOD_TABLE", "cfba026a-3fad-443f-b0c6-371c5b99213f");//215"
            //prefabs.Add("PLANK_TABLE", "ca20db61-8507-4d9b-8a10-362f85b9eb1a");//216"
            //prefabs.Add("CORRUGATED_TABLE", "3886bcb6-e2e0-4b6d-ab79-a9def2e98b23");//217"
            //prefabs.Add("PATROL_TIGERSHARK", "9fd88c0a-f6ba-4c74-b387-8394151b9ab3");//218"
            //prefabs.Add("LOOM", "b369b1f1-efcd-492a-afb1-d513c3845e40");//219"
            //prefabs.Add("TANNING_RACK", "6aa4202a-c0eb-48dc-b9a9-2d7029f94761");//220"
            //prefabs.Add("LEATHER", "b23a72c0-18ce-4bac-a678-83584366e1cb");//221"
            //prefabs.Add("RAWHIDE", "f72e18ff-1ef6-4838-8281-8b05f12a806b");//222"
            //prefabs.Add("CONTAINER_CONSOLE", "d8894f72-ebbb-4392-b4f7-5831475a7aac");//223"
            //prefabs.Add("CONTAINER_CRATE", "5c238ebb-9eb1-4d5d-94d5-26d717b4e0af");//224"
            //prefabs.Add("CONTAINER_LOCKER_LARGE", "64b36c01-79a1-440a-8609-3f40810fa509");//225"
            //prefabs.Add("CONTAINER_LOCKER_SMALL", "72861676-9050-4495-9172-7bf0bd70b8b9");//226"
            //prefabs.Add("BRICK_STATION", "dc0ed048-cb40-4ceb-91e1-e1c2393abf77");//227"
            //prefabs.Add("CLAY", "abded059-a64c-4334-8ee2-9b098436e575");//228"
            //prefabs.Add("FURNACE", "af0b831c-5efe-4e79-8e20-de3f1aea93fb");//229"
            //prefabs.Add("BRICKS", "63b2be86-1c37-4218-aad9-acbf5dca1914");//230"
            //prefabs.Add("BRICK_ARCH", "cc743a21-3e91-43f5-a852-c746c7a048c0");//231"
            //prefabs.Add("BRICK_FLOOR", "0a4388c6-644c-46d0-aa4c-4ebce70a279d");//232"
            //prefabs.Add("BRICK_FOUNDATION", "393ea3a6-5ac7-435d-8ca8-5898d4038421");//233"
            //prefabs.Add("BRICK_PANEL_STATIC", "540a6c48-6fee-4cae-8762-0fe59caf05e8");//234"
            //prefabs.Add("BRICK_WALL_HALF", "d73498c2-e2f0-459f-8cce-2a1cea78d769");//235"
            //prefabs.Add("BRICK_WALL_WINDOW", "a59f5b71-2b9a-45a2-85bb-ec29a47f78aa");//236"
            //prefabs.Add("BRICK_WEDGE_FLOOR", "ff1e7909-95db-4c0a-b1fc-432bb8459a84");//237"
            //prefabs.Add("BRICK_WEDGE_FOUNDATION", "1e3f61df-6e6f-4c8e-aff2-7ec3944192c7");//238"
            //prefabs.Add("WOOD_ROOF_CAP", "2ad07d38-8fb1-4f96-ac3c-3ff5ad3b035a");//239"
            //prefabs.Add("WOOD_ROOF_CORNER", "eebd9211-7281-4ae2-a65f-ae7ca853d0e8");//240"
            //prefabs.Add("WOOD_ROOF_MIDDLE", "0f2a474d-1750-4d08-8070-aed92a36874b");//241"
            //prefabs.Add("WOOD_ROOF_WEDGE", "b0dbaefe-8b4f-4948-9b05-596a71f810c7");//242"
            //prefabs.Add("BRICK_ROOF_CAP", "3ec51ef8-7a5a-4912-8bf6-a7fec599ed13");//243"
            //prefabs.Add("BRICK_ROOF_CORNER", "0924596d-0a71-40c0-ae87-1133d73845af");//244"
            //prefabs.Add("BRICK_ROOF_MIDDLE", "4e133989-56c6-4c65-9076-2dd54e559d33");//245"
            //prefabs.Add("BRICK_ROOF_WEDGE", "ae7596a1-a347-46c2-b2e3-a97350c45c74");//246"
            //prefabs.Add("NEW_REFINED_PICK", "2e3c284c-5c95-40c0-a866-d05d45eaf348");//247"
            prefabs.Add("MINING_ROCK", "c4156b3a-e461-4053-b7da-858b41381bc8");//248"
            prefabs.Add("MINING_ROCK_CLAY", "0655abbc-1704-4b4f-9038-605df9a0e710");//249"
            //prefabs.Add("WATER_SKIN", "5e4b6c0e-f0de-4c22-bab4-66320a2fa735");//250"
            //prefabs.Add("PLANK_STATION", "e8ea244b-7c9c-4215-b447-70f28e7dedad");//251"
            //prefabs.Add("MISSION_TROPHY_SHARK", "3bd43f55-ccdf-43d5-85d2-2b20a254b3a3");//252"
            //prefabs.Add("MISSION_TROPHY_EEL", "48845712-50d3-4c6f-abbe-ca539ea3285a");//253"
            //prefabs.Add("MISSION_EEL", "6047f88f-0c0b-4c19-8206-26711be30687");//254"
            //prefabs.Add("MISSION_SHARK", "b8773d99-6e39-456a-8f0a-b598450cbcb1");//255"
            //prefabs.Add("MISSION_SQUID", "141d80ad-a08b-46f1-8e0b-d336c91db11d");//256"
            //prefabs.Add("MISSION_TROPHY_SQUID", "64fbbcd2-a6d6-4408-b055-1a9485a82789");//257"
            //prefabs.Add("GYRO_BASE_1", "308e3949-70b8-4d88-a72e-bf3faac023b1");//258"
            //prefabs.Add("GYRO_COCKPIT_4", "3c632a30-f341-4707-90f7-335df8184817");//259"
            //prefabs.Add("GYRO_MOTOR_3", "f5e47d7a-1158-4b43-9c27-d98b6ee31f7e");//260"
            //prefabs.Add("GYRO_ROTORS_5", "402cdb48-0bd7-4500-98cb-e6f9815cbb66");//261"
            //prefabs.Add("GYRO_SEAT_2", "2da12c26-1679-4cf9-b798-ac2d9ca39de8");//262"
            //prefabs.Add("GYRO_STRUCTURE", "5d1a4c77-91b1-4e8a-a321-8131327d7224");//263"
            //prefabs.Add("VEHICLE_HELICOPTER", "f4ee47ef-656b-4892-b2da-b3fd36140ca5");//264"
            //prefabs.Add("ALOE_VERA_FRUIT", "3091fdef-ceaf-493c-b9d2-923bc2413efa");//265"
            prefabs.Add("ALOE_VERA_PLANT", "cb2353d9-6da7-4396-a71f-761f2d471803");//266"
            prefabs.Add("POTATO_PLANT", "19edc888-b2f0-4a35-ae79-ad1812f24ac2");//267"
            //prefabs.Add("POTATO", "6ac0f54f-7e53-47ec-876b-da4612f9b6da");//268"
            //prefabs.Add("COCONUT_FLASK", "97981adb-d24e-4041-b1f3-5fa7278fb47e");//269"
            //prefabs.Add("MEDICAL_ALOE_SALVE", "72f0ee2c-1ab4-4877-b932-5baa9a6367e9");//270"
            //prefabs.Add("MEDICAL_GAUZE", "c731c5e3-4137-40b8-9453-81185b57da01");//271"
            //prefabs.Add("MEDICAL_ANTIDOTE", "796876ca-9211-49b1-b4e0-1d4ab275d8f1");//272"
            prefabs.Add("AJUGA_PLANT", "0149fd97-51f0-45bb-82c0-833496c228b7");//273"
            //prefabs.Add("AJUGA", "ac6bb617-8915-4309-9246-edf06033b9bd");//274"
            //prefabs.Add("MEDICAL_BREATH_BOOST", "6d6b6f15-b16c-4c7a-9950-fca4f87f9a2a");//275"
            //prefabs.Add("MEDICAL_REPELLENT", "2db601b5-bace-4c4e-9ffb-e04507add06d");//276"
            //prefabs.Add("WAVULAVULA", "259cbbce-f005-4d3e-a04a-10c08e547879");//277"
            prefabs.Add("WAVULAVULA_PLANT", "c1b97f74-a0aa-437f-88f1-50a1e1baa367");//278"
            //prefabs.Add("PIPI", "09e10982-2f77-41c0-a33f-5f4c65d90a57");//279"
            prefabs.Add("PIPI_PLANT", "a7e1a566-0fc5-4033-8c9b-f876aa0266a2");//280"
            //prefabs.Add("PADDLE", "cd10e725-5233-4dbd-bc4a-f0154c41abc4");//281"
            //prefabs.Add("VEHICLE_RUDDER", "4b4d1273-76b3-4b30-a478-df397b3a3180");//282"
            //prefabs.Add("BOBBER", "7e453d06-a17d-4f60-b21d-dae86a5de35b");//283"
            //prefabs.Add("FISHING_ROD", "5128d498-eb94-4567-90f1-7a2dff7e1ff4");//284"
            //prefabs.Add("PART_ENGINE", "d3d2812a-447f-41d7-b412-08d8b21336af");//285"
            //prefabs.Add("PART_ELECTRICAL", "562456b5-3835-469a-9286-d4221b0b4d26");//286"
            //prefabs.Add("PART_FILTER", "75d9c0ba-c732-4e59-ac64-5bcf19e0c2df");//287"
            //prefabs.Add("PART_GYRO", "aa5bb8d8-fd1b-4902-83a9-94b375d0d0dc");//288"
            //prefabs.Add("PART_FUEL", "5fefc2c2-794c-487c-b127-5892aacbe458");//289"
            //prefabs.Add("FUEL_STILL", "122d3677-c08f-4805-a0df-401cd46f8f55");//290"
            //prefabs.Add("BRICK_DOOR", "001c2e9f-a853-45ed-a52b-bdbbf88b92d3");//291"
            //prefabs.Add("BRICK_STEPS", "4d66f936-3ced-4a46-b49c-2ed272d3f023");//292"
            //prefabs.Add("TRAP_FISH", "6f32037e-a406-4d75-aae9-dc2d3fc983f0");//293"
            //prefabs.Add("TRAP_BIRD", "434d09d3-84da-4c87-bf89-dbf515b7f9af");//294"
            //prefabs.Add("RAFT_ANCHOR", "baea55db-a4c9-4e49-90b2-031a0f03e106");//295"
            //prefabs.Add("SPLINT", "a5596363-f84e-4e63-bda8-3bff19fe2e8b");//296"
            //prefabs.Add("AIRCRAFT_ENGINE_PART", "21099cd8-af46-48d7-b682-2d77c5bd43a7");//297"
            //prefabs.Add("AIRCRAFT_PROPELLER_PART", "932d1e56-9fa4-4fae-b388-b8143aacb377");//298"
            //prefabs.Add("AIRCRAFT_RUDDER_PART", "5e8d0aa4-ddb4-4e38-b36f-8c98201514fc");//299"
            //prefabs.Add("AIRCRAFT", "e9d968cf-a418-4db2-bb59-f3ee1e754a5c");//300"
            //prefabs.Add("YUCCA_HARVEST", "c6d87c60-1c67-4169-a03e-2763c610a19d");//301"
            //prefabs.Add("YUCCA_CUTTING", "e8dfb37a-e846-4f7b-899d-f9d10d4e25de");//302"
            //prefabs.Add("RAFT_CANOPY", "1289d12b-8756-476f-97ec-2d14bc45dcfe");//303"
            //prefabs.Add("GROUPER_RAGDOLL", "87f08513-e73e-4460-83bc-d4434f11122d");//304"
            //prefabs.Add("SHARK_GOBLIN_SHARK_RAGDOLL", "2de47dc8-b5f5-4d0e-80f6-9a05f93dda8c");//305"
            //prefabs.Add("SHARK_HAMMERHEAD_RAGDOLL", "2833f731-0c4b-4c88-9e88-94368d6afc72");//306"
            //prefabs.Add("SHARK_WHALE_RAGDOLL", "7507e119-11ae-4621-8bb7-6eef9faf8bcc");//307"
            prefabs.Add("PLANEWRECK_1A", "b8f7794f-77df-4c44-9b59-1375865eb715");//308"
            prefabs.Add("SHIPWRECK_8A", "0434fd5f-bcf5-4acd-9ad6-0f025cf4267c");//309"
            prefabs.Add("SHIPWRECK_12A", "5d5e3f50-08ba-401a-bc7d-10457ab256ff");//310"
            prefabs.Add("SHIPWRECK_13A", "7fc5c4b3-73e0-4f52-a42c-e1247e9d4f67");//311"
            prefabs.Add("Shelter01", "dca4e27f-9ac1-4ba2-8ecb-f6eb3bdaa356");//312"
            prefabs.Add("Shelter02", "2432fbe0-f3a5-4c41-9f06-1a7e61422fdc");//313"
            prefabs.Add("Shelter03", "93d2d96c-1ecb-4924-a4ef-80cc16d2c5bd");//314"
            prefabs.Add("Shelter04", "53aeecd6-713a-4a74-9f44-bb0659149213");//315"
            prefabs.Add("Shelter05", "0f65202e-950a-4bc4-baaa-69fe92943226");//316"
            prefabs.Add("Shelter06", "afae3aea-f08e-4940-928c-d63e7c88c199");//317"
            prefabs.Add("Shelter07", "f8180b94-c127-4761-826b-c76042440d34");//318"
            prefabs.Add("Shelter08", "eac681be-393a-461b-8d27-ab8dd51c7411");//319"
            prefabs.Add("Shelter09", "09751c28-1cff-481c-be01-7ad3623b9863");//320"
            //prefabs.Add("DOOR_13_85d", "500e874f-e3c5-4cfb-acce-745606cc175b");//321"
            //prefabs.Add("DOOR_13_165d", "32a11ad6-f51a-41c9-abe2-29d6a6fc6abe");//322"
            //prefabs.Add("DOOR_13D1", "daec909c-123b-4dba-9a30-6e4334bb470f");//323"
            //prefabs.Add("DOOR_13D2", "b24fc05f-2308-4eb7-919d-ad26c22e0ee6");//324"
            //prefabs.Add("DOOR_13D3", "7be127a1-228c-45bc-8718-98a8c969221d");//325"
            //prefabs.Add("DOOR_13M_85d", "978b53b6-42c0-4b5b-8df2-040e5bd86462");//326"
            //prefabs.Add("DOOR_13M_165d", "5bbbf6df-8173-4282-a262-650fce7d0456");//327"
            //prefabs.Add("SPYGLASS", "f4ac70bc-beae-4f38-b84c-ffce429acc37");//328"
            //prefabs.Add("CONTAINER_SHELF", "3b080d4b-4597-4184-b8fc-25c870575d37");//329"
            //prefabs.Add("SHIPPING_CONTAINER", "d2096833-9c6a-4cdf-b15a-1d9287b93206");//330"
            //prefabs.Add("SHARK - HAMMERHEAD", "076ba5de-bbda-4d8e-ba7f-0ad2369ab499");//331"
            //prefabs.Add("SHARK - WHALE", "7b304494-2996-42d1-a442-0d02c6cebaf5");//332"
            //prefabs.Add("SHARK - GOBLIN", "ece551ac-ce6f-43c1-a6a7-62a2898e7ad9");//333"
            //prefabs.Add("PATROL_HAMMERHEAD", "4b5344ac-7019-4fec-95eb-01c3afab966b");//334"
            //prefabs.Add("PATROL_GOBLIN", "d05cae70-0925-4452-9ff4-86b54d0eaf61");//335"
            //prefabs.Add("GIANT_CRAB_RAGDOLL", "1f23785e-d5bd-43de-991d-7a4c9a67608a");//336"
            //prefabs.Add("GIANT_CRAB", "32a665fc-eb34-425c-b487-b83e2ed1f606");//337"
            //prefabs.Add("CRAB_SPAWNER", "e0db308b-fa50-4408-9517-054137154062");//338"
            //prefabs.Add("GIANT_CRAB_SPAWNER", "1639f581-3b4c-4b3c-b5d1-f588917920b9");//339"
            //prefabs.Add("BOAR_SPAWNER", "59b6878c-a951-424e-9e7a-298d7d5552b5");//340"
            //prefabs.Add("SNAKE_SPAWNER", "958836f6-5604-4406-9300-35392baf1b59");//341"
            //prefabs.Add("HOG", "6f0e8a06-4259-4708-b328-e047a9589bd5");//342"
            //prefabs.Add("HOG_RAGDOLL", "4cb0ab0a-5ba5-46fc-81bf-f2999acb821e");//343"
            //prefabs.Add("HOG_SPAWNER", "1b2cc034-d30c-4dcb-9a10-5ff82650dd80");//344"
            //prefabs.Add("LIGHT_HOOK", "67a0e997-7ef8-43c9-b260-b460b125035d");//345"
            //prefabs.Add("MACHETE", "98559a8d-eaeb-4665-912f-10806c54b40f");//346"
            //prefabs.Add("FLASHLIGHT", "c86d89ea-95a1-4245-a339-7e0daff5b449");//347"
            //prefabs.Add("RUBBER_DUCK", "dc55b91f-df40-476f-a9b5-3c0f9a8c998d");//348"
            //prefabs.Add("SUNSCREEN", "059bc012-5e8d-4f4d-afd3-4ad862284913");//349"
            //prefabs.Add("CONTAINER_CRATE_ENDGAME", "ec2ba585-ebf5-4625-840c-3203a2462276");//350"
            //prefabs.Add("CONTAINER_CRATE_ENDGAME_2", "e0988c94-e16c-457b-bf26-3ffb7cc90125");//351"
            //prefabs.Add("NEW_SPEARGUN_CARBON", "1dc40876-184a-4433-b0ce-b73020a0e65b");//352"
            //prefabs.Add("NEW_SPEARGUN_CARBON_ARROW", "7d08a613-1ea5-40ec-bfc3-0da08b3ab348");//353"

            ((Control)this.tpRandomize).Enabled = false;
            ((Control)this.tpPopulate).Enabled = false;

            foreach(string key in prefabs.Keys)
            {
                if (!String.IsNullOrEmpty(prefabs[key]))
                    clbRandomizePrefabs.Items.Add(key);
            }
        }

        private void UpdateObjectRandomizer(string selectedItem)
        {
            clbRandomizeSizes.Items.Clear();
            if (string.IsNullOrEmpty(selectedItem))
            {
                ((Control)this.tpRandomize).Enabled = false;
                ((Control)this.tpPopulate).Enabled = false;
            }
            else
            {
                ((Control)this.tpRandomize).Enabled = true;
                ((Control)this.tpPopulate).Enabled = true;
            }

            string sourceFileName = "";
            string sourceDirectory = mapDictionary[selectedItem];
            foreach (string fileOrDir in Directory.EnumerateFiles(sourceDirectory))
            {
                if (fileOrDir.Contains("_OBJECT.map") && !fileOrDir.Contains(".bak"))
                {
                    sourceFileName = fileOrDir;
                    break;
                }
            }

            // Parse map OBJECT file
            string json = File.ReadAllText(sourceFileName);
            dynamic array = JsonConvert.DeserializeObject(json);

            // Look for prefabs
            foreach (string key in prefabs.Keys)
            {
                if (json.Contains(prefabs[key]))
                {
                    clbRandomizeSizes.Items.Add(key);
                }
            }
        }

        private void tbarMinScale_ValueChanged(object sender, EventArgs e)
        {
            if (tbarMaxScale.Value < tbarMinScale.Value)
                tbarMaxScale.Value = tbarMinScale.Value;
            double minScale = 1 + (double)tbarMinScale.Value / (double)10;
            tbMinScale.Text = minScale.ToString();
        }

        private void tbarMaxScale_ValueChanged(object sender, EventArgs e)
        {
            if (tbarMaxScale.Value < tbarMinScale.Value)
                tbarMinScale.Value = tbarMaxScale.Value;
            double maxScale = 1 + (double)tbarMaxScale.Value / (double)10;
            tbMaxScale.Text = maxScale.ToString();
        }

        private void bRandomizeObjects_Click(object sender, EventArgs e)
        {
            Log("");
            string selectedMap = lbMaps.SelectedItem.ToString();
            if (String.IsNullOrEmpty(selectedMap))
            {
                Log("Error : no map selected");
                return;
            }
            
            string destFileName = DirectoriesDetector.GetOBJECTFile(mapDictionary[selectedMap]);
            if (!File.Exists(destFileName))
            {
                Log("Error : OBJECT file not found");
                return;
            }

            Log("Randomizing objects sizes and spin on : " + selectedMap);

            // backup
            Log("Making backup");
            if (!DirectoriesDetector.MakeBackup(destFileName, "BeforeRandomize"))
                return;

            // randomize objects
            int minScale = (int)(double.Parse(tbMinScale.Text) * 100);
            int maxScale = (int)(double.Parse(tbMaxScale.Text) * 100);
            Random r = new Random(maxScale);

            string json = File.ReadAllText(destFileName);
            dynamic array = JsonConvert.DeserializeObject(json);

            for (int itemIndex = 0; itemIndex < array.Count; itemIndex++)
            {
                dynamic item = array[itemIndex];

                string prefab = null;
                if (item["prefab"] != null)
                {
                    prefab = item["prefab"].ToString();
                }

                // loop on selected objects
                foreach (object o in clbRandomizeSizes.CheckedItems)
                {
                    string key = o.ToString();
                    if (!String.IsNullOrEmpty(prefab) && prefab.Contains(prefabs[key]))
                    {
                        Log("Found " + key + " node");
                        if (cbRandomizeScale.Checked)
                        {
                            // add an bell curve effect
                            int distance = r.Next(0, 100);
                            // extreme = 0-10
                            // high = 10-50
                            // medium = 40-100
                            double newScale = 1;
                            if (distance <= 10)
                            {
                                newScale = (double)r.Next(minScale, maxScale) / 100;
                            }
                            else if (distance <= 40)
                            {
                                int adjustedmin = (minScale + 100) / 2;
                                int adjustedmax = (maxScale + 100) / 2;
                                newScale = (double)r.Next(adjustedmin, adjustedmax) / 100;
                            }
                            else
                            {
                                int adjustedmin = (minScale + 300) / 4;
                                int adjustedmax = (maxScale + 300) / 4;
                                newScale = (double)r.Next(adjustedmin, adjustedmax) / 100;
                            }
                            bool flip = r.Next(0, 100) > 50;
                            Log("New scale for " + key + " = " + newScale);
                            item["Transform"]["localScale"]["x"] = flip ? -newScale : newScale;
                            item["Transform"]["localScale"]["y"] = newScale;
                            item["Transform"]["localScale"]["z"] = newScale;
                        }
                        if (cbRandomizeRotate.Checked)
                        {
                            double newRotation = (double)r.Next(-100, 100) / 100;
                            double newW = (double)r.Next(0, 100) / 100;
                            Log("New rotation for " + key + " = " + newRotation);
                            item["Transform"]["localRotation"]["x"] = 0;
                            item["Transform"]["localRotation"]["z"] = 0;
                            item["Transform"]["localRotation"]["w"] = Math.Abs(newW);
                            item["Transform"]["localRotation"]["y"] = newRotation;
                        }
                    }
                }
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(destFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, array);
            }
            Log("File written, all done");
            MessageBox.Show("All done, Enjoy your random objects !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Random populate

        private void bPopulate_Click(object sender, EventArgs e)
        {
            Log("");
            string selectedMap = lbMaps.SelectedItem.ToString();
            if (String.IsNullOrEmpty(selectedMap))
            {
                Log("Error : no map selected");
                return;
            }

            string destFileName = DirectoriesDetector.GetOBJECTFile(mapDictionary[selectedMap]);
            if (!File.Exists(destFileName))
            {
                Log("Error : OBJECT file not found");
                return;
            }

            string heightMapFileName = DirectoriesDetector.GetHEIGHTFile(mapDictionary[selectedMap]);
            if (!File.Exists(heightMapFileName))
            {
                Log("Error : HEIGHT file not found");
                return;
            }

            if (!File.Exists(destFileName))
            {
                Log("OBJECT file not found, aborting");
                return;
            }
            Log("Populating objects on : " + selectedMap);
            // backup
            Log("Making backup");
            if (!DirectoriesDetector.MakeBackup(destFileName, "BeforePopulate"))
                return;

            // randomize objects
            int minScale = (int)(double.Parse(tbMinScalePopulate.Text) * 100);
            int maxScale = (int)(double.Parse(tbMaxScalePopulate.Text) * 100);
            Random r = new Random(maxScale);

            string json = File.ReadAllText(destFileName);
            dynamic array = JsonConvert.DeserializeObject(json);
            if (array == null)
            {
                array = new JArray();
            }

            // Read heightmap file 257x257
            MagickReadSettings mr = new MagickReadSettings();
            mr.Format = MagickFormat.Gray;
            mr.Height = 257;
            mr.Width = 257;
            mr.Endian = Endian.LSB;
            mr.Depth = 16;

            using (var image = new MagickImage(heightMapFileName, mr))
            {
                IPixelCollection<ushort> ipc = image.GetPixels();
                //Bitmap bitmap = new Bitmap(257, 257);
                //bitmap.SetPixel(x, y, Color.Black);
                for (int y = 0; y < image.Width; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // read pixel value
                        ushort[] color = ipc.GetValue(x, y);
                        if (color.Length == 1)
                        {
                            // check if cliff of high slope
                            bool isCliff = false;
                            int highSlopeDelta = 1000;
                            // in all 8 directions
                            for (int ny = -1; ny <= 1; ny++)
                            {
                                for (int nx = -1; nx <= 1; nx++)
                                {
                                    // exclude limits
                                    if (x + nx < 0
                                        || y + ny < 0
                                        || x + nx >= image.Width
                                        || y + ny >= image.Height)
                                    {
                                        continue;
                                    }
                                    // read neighbour pixel value and compare
                                    ushort[] neighbourcolor = ipc.GetValue(x + nx, y + ny);
                                    if (neighbourcolor.Length == 1
                                        && Math.Abs(neighbourcolor[0] - color[0]) > highSlopeDelta)
                                    {
                                        isCliff = true;
                                        break;
                                    }
                                }
                                if (isCliff)
                                    break;
                            }

                            if (isCliff)
                                continue;

                            // TESTS !
                            // Add random vegetation
                            ushort grayLevel = color[0];
                            // Convert to map coordinates 127,127 = 0,0 ?
                            float xMap = x - 128.5f;
                            float zMap = y - 128.5f;

                            int deepwater = 41500;
                            int intermediatewater = 42200;
                            int shallowwater = 43200;
                            int waterLevel = 43500;
                            int sandThreshold = 44000;
                            int grassThreshold = 44500;
                            int dirtThreshold = 45000;
                            //float yMap = (float)(grayLevel - dirtThreshold) / 512 + (float)grayLevel / 15000;
                            float yMap = (float)(grayLevel - dirtThreshold) / 500 + (float)grayLevel / 15000 - 0.1f;

                            // check density
                            int density = tbDensity.Value;
                            int random = r.Next(0, 5001);
                            if (density >= random)
                            {
                                // check what to put
                                int nbSelected = clbRandomizePrefabs.CheckedItems.Count;
                                int choose = r.Next(0, nbSelected);
                                string prefabKeyToPopulate = clbRandomizePrefabs.CheckedItems[choose].ToString();

                                if (grayLevel > dirtThreshold)
                                {
                                    if (cbDirtLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                                else if (grayLevel > grassThreshold)
                                {
                                    if (cbGrassLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                                else if (grayLevel > sandThreshold)
                                {
                                    if (cbSandLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                                else if (grayLevel > waterLevel)
                                {
                                    if (cbWaterLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                                else if (grayLevel > shallowwater)
                                {
                                    if (cbShallowWaterLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                                else if (grayLevel > intermediatewater)
                                {
                                    if (cbIntermediateWaterLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                                else if (grayLevel > deepwater)
                                {
                                    if (cbUnderwaterLevel.Checked)
                                    {
                                        JObject toAdd = AddObject(prefabKeyToPopulate, xMap, zMap, yMap, cbRandomizeScalePopulate.Checked, minScale, maxScale, r);
                                        if (toAdd != null)
                                            array.Add(toAdd);
                                    }
                                }
                            }
                        }
                    }

                }
                //bitmap.Save("test.bmp");
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            Log("JSON udpated");
            using (StreamWriter sw = new StreamWriter(destFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, array);
            }
            Log("File written, all done");
            MessageBox.Show("All done, Enjoy your random objects !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private JObject AddObject(string prefabKey, float xMap, float zMap, float yMap, bool randomizeScale, int minScale, int maxScale, Random r)
        {
            if (!prefabs.ContainsKey(prefabKey))
                return null;

            JObject itemToAdd = new JObject();
            itemToAdd["prefab"] = prefabs[prefabKey];
            itemToAdd["Transform"] = new JObject();

            itemToAdd["Transform"]["localPosition"] = new JObject();
            itemToAdd["Transform"]["localPosition"]["x"] = xMap;
            itemToAdd["Transform"]["localPosition"]["y"] = yMap;
            itemToAdd["Transform"]["localPosition"]["z"] = zMap;

            itemToAdd["Transform"]["localScale"] = new JObject();
            if (randomizeScale)
            {
                // add an bell curve effect
                int distance = r.Next(0, 100);
                // extreme = 0-10
                // high = 10-50
                // medium = 40-100
                double newScale = 1;
                if (distance <= 10)
                {
                    newScale = (double)r.Next(minScale, maxScale) / 100;
                }
                else if (distance <= 40)
                {
                    int adjustedmin = (minScale + 100) / 2;
                    int adjustedmax = (maxScale + 100) / 2;
                    newScale = (double)r.Next(adjustedmin, adjustedmax) / 100;
                }
                else
                {
                    int adjustedmin = (minScale + 300) / 4;
                    int adjustedmax = (maxScale + 300) / 4;
                    newScale = (double)r.Next(adjustedmin, adjustedmax) / 100;
                }
                bool flip = r.Next(0, 100) > 50;
                itemToAdd["Transform"]["localScale"]["x"] = flip ? -newScale : newScale;
                itemToAdd["Transform"]["localScale"]["y"] = newScale;
                itemToAdd["Transform"]["localScale"]["z"] = newScale;
            }
            else
            {
                itemToAdd["Transform"]["localScale"]["x"] = 1;
                itemToAdd["Transform"]["localScale"]["y"] = 1;
                itemToAdd["Transform"]["localScale"]["z"] = 1;
            }

            itemToAdd["Transform"]["localRotation"] = new JObject();
            if (cbRandomizeRotationPopulate.Checked)
            {
                double newRotation = (double)r.Next(-100, 100) / 100;
                double newW = (double)r.Next(0, 100) / 100;
                itemToAdd["Transform"]["localRotation"]["x"] = 0;
                itemToAdd["Transform"]["localRotation"]["z"] = 0;
                itemToAdd["Transform"]["localRotation"]["w"] = Math.Abs(newW);
                itemToAdd["Transform"]["localRotation"]["y"] = newRotation;
            }
            else
            {
                itemToAdd["Transform"]["localRotation"] = new JObject();
                itemToAdd["Transform"]["localRotation"]["x"] = 0;
                itemToAdd["Transform"]["localRotation"]["y"] = 0;
                itemToAdd["Transform"]["localRotation"]["z"] = 0;
                itemToAdd["Transform"]["localRotation"]["w"] = 1;
            }

            return itemToAdd;
        }

        #endregion

        private void tbarMinScalePopulate_ValueChanged(object sender, EventArgs e)
        {
            if (tbarMaxScalePopulate.Value < tbarMinScalePopulate.Value)
                tbarMaxScalePopulate.Value = tbarMinScalePopulate.Value;
            double minScale = 1 + (double)tbarMinScalePopulate.Value / (double)10;
            tbMinScalePopulate.Text = minScale.ToString();
        }

        private void tbarMaxScalePopulate_ValueChanged(object sender, EventArgs e)
        {
            if (tbarMaxScalePopulate.Value < tbarMinScalePopulate.Value)
                tbarMinScalePopulate.Value = tbarMaxScalePopulate.Value;
            double maxScale = 1 + (double)tbarMaxScalePopulate.Value / (double)10;
            tbMaxScalePopulate.Text = maxScale.ToString();
        }
    }
}
