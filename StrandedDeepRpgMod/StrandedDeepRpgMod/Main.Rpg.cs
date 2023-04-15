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
        //private static bool pendingRpgUpdate = false;

        private static bool rpgCanvasVisible = false;
        private static GameObject textCanvas;
        private static Text rpgText;

        private static float rpgCanvasDefaultScreenWitdh = 1024f;
        private static float rpgCanvasDefaultScreenHeight = 768f;

        private static Dictionary<string, string> indexedRpgText;

        private static DateTime askedHide = DateTime.MinValue;
        private static int millisecondsBeforeHide = 1000;

        // most important
        private static int currentlyLoadedSlot = -1;

        private static Dictionary<int, bool> islandDiscovered;

        private static UCartographerMenuViewAdapter Cartographer { get; set; }

        private static int rpgTextTriggerDistance = 10;

        static void InitRpg()
        {
            islandDiscovered = new Dictionary<int, bool>();
            indexedRpgText = new Dictionary<string, string>();

            CheckAndCopyRpgFiles();
        }

        static void CheckAndCopyRpgFiles()
        {
            // list all maps for this game slot
            for (int slot = 0; slot < 3; slot++)
            {
                string currentWorldDirectory = Path.Combine(dataDirectory, FilePath.SAVE_SLOT_FOLDER_PREFIX + slot.ToString(), "World");
                Debug.Log("Stranded Deep Rpg mod : checking world directory : " + currentWorldDirectory);
                if (Directory.Exists(currentWorldDirectory))
                {
                    // hard cleanup
                    CheckConfigFileExpiration(Path.Combine(dataDirectory, FilePath.SAVE_SLOT_FOLDER_PREFIX + slot.ToString()));

                    for (int islandIndex = 0; islandIndex < islandCount; islandIndex++)
                    {
                        if (!islandDiscovered.ContainsKey(islandIndex))
                        {
                            islandDiscovered.Add(islandIndex, false);
                        }

                        if (Directory.Exists(Path.Combine(currentWorldDirectory, islandIndex.ToString())))
                        {
                            string currentMapDirectory = Path.Combine(currentWorldDirectory, islandIndex.ToString());
                            foreach (string targetdir in Directory.EnumerateDirectories(currentMapDirectory))
                            {
                                //Debug.Log("Stranded Deep Rpg mod : current map directory : " + dir);

                                if (targetdir.Contains("PROCEDURAL"))
                                    continue;

                                if (targetdir.Contains("MAP") && !targetdir.Contains("MISSION"))
                                {
                                    // convert to MAP_XXXXXXX-XXXX..
                                    string mapGuid = new DirectoryInfo(targetdir).Name; 
                                    //Debug.Log("Stranded Deep Rpg mod : initialization map guid : " + mapGuid);
                                    CopyRpgFileIfExists(mapGuid, targetdir);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckConfigFileExpiration(string dataSlotDirectory)
        {
            if (String.IsNullOrEmpty(dataSlotDirectory)
                || !Directory.Exists(dataSlotDirectory))
                return;

            string configPath = Path.Combine(dataSlotDirectory, configFileName);
            if (!File.Exists(configPath))
                return;
            
            string seedPath = Path.Combine(dataSlotDirectory, "World/", "Seed.sdd");
            if (!File.Exists(seedPath))
            {
                if (File.Exists(configPath))
                    File.Delete(configPath);
                return;
            }

            DateTime lwtconfig = File.GetLastWriteTime(configPath);
            DateTime lwtseed = File.GetLastWriteTime(seedPath);
            // if config is older than seed
            if (DateTime.Compare(lwtconfig, lwtseed) < 0)
            {
                File.Delete(configPath);
            }
        }

        private static void CopyRpgFileIfExists(string mapGuidWithPrefix, string targetDir)
        {
            string mapDirectory = "";
            if (Directory.Exists(Path.Combine(mapsDirectory, mapGuidWithPrefix)))
            {
                mapDirectory = Path.Combine(mapsDirectory, mapGuidWithPrefix);
                Debug.Log("Stranded Deep Rpg mod : map found in local directory : " + mapDirectory);
            }
            // local maps priority
            if (Directory.Exists(workshopPath) 
                && string.IsNullOrEmpty(mapDirectory))
            {
                foreach(string subdirectory in Directory.EnumerateDirectories(workshopPath))
                {
                    string potentialPath = Path.Combine(subdirectory, mapGuidWithPrefix);
                    if (Directory.Exists(potentialPath))
                    {
                        Debug.Log("Stranded Deep Rpg mod : map found in workshop directory : " + potentialPath);
                        mapDirectory = potentialPath;
                    }
                }
            }
            if (String.IsNullOrEmpty(mapDirectory))
                return;

            foreach (string file in Directory.EnumerateFiles(mapDirectory, "*_RPG.map"))
            {
                Debug.Log("Stranded Deep Rpg mod : map has RPG file, copying to world : " + mapGuidWithPrefix);
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                if (File.Exists(targetFile))
                {
                    DateTime lwtsource = File.GetLastWriteTime(file);
                    DateTime lwttarget = File.GetLastWriteTime(targetFile);
                    // if target is older than source
                    if (DateTime.Compare(lwttarget, lwtsource) < 0)
                    {
                        File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);
                    }
                }
                else
                {
                    File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);
                }
            }
        }

        /// <summary>
        /// UPDATE
        /// </summary>
        static void RunRpg()
        {
            Event currentevent = Event.current;
            if (currentevent.isKey)
            {
                if (currentevent.keyCode == KeyCode.F6)
                {
                    rpgCanvasVisible = true;
                }
                if (currentevent.keyCode == KeyCode.F7)
                {
                    rpgCanvasVisible = false;
                }
            }

            try
            {
                UpdateRpgContent();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : UpdateRpgContent failed : " + e);
            }

            RefreshRpgTextCanvas();

            if (Cartographer == null)
            {
                UCartographerMenuViewAdapter carto = Beam.Game.FindObjectOfType<UCartographerMenuViewAdapter>();
                if (carto != null)
                {
                    Debug.Log("Stranded Deep Rpg mod : UCartographerMenuViewAdapter found, hooking event to reload RPG files after Apply");
                    Cartographer = carto;
                    Cartographer.HideView -= Cartographer_HideView;
                    Cartographer.HideView += Cartographer_HideView;
                }
            }

                if (Beam.Game.State == GameState.LOAD_GAME)
            {
                // show current guid on top of the screen
                
            }

            // MOST IMPORTANT SECTION : which object are we looking at
            if (Beam.Game.State != GameState.MAP_EDITOR)
            {
                if (Camera.main != null)
                {
                    // Get the ray going through the GUI position
                    Ray r = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                    // Do a raycast
                    RaycastHit hit;
                    string text = null;
                    if (Physics.Raycast(r, out hit))
                    {
                        // check player distance
                        float dist = Vector3.Distance(PlayerRegistry.LocalPlayer.transform.position, hit.transform.position);
                        if (dist <= rpgTextTriggerDistance)
                        {
                            // "I'm looking at " + hit.transform.name;
                            text = CheckRpgContent(hit.transform.gameObject);
                        }
                        else
                        {
                            text = "";
                        }
                    }
                    else
                    {
                        text = CheckRpgContent();
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        rpgText.text = text;
                        rpgCanvasVisible = true;
                        askedHide = DateTime.MinValue;
                    }
                    else if (DateTime.Compare(askedHide, DateTime.MinValue) == 0)
                    {
                        askedHide = DateTime.Now.AddMilliseconds(millisecondsBeforeHide);
                    }
                }
            }

            if (rpgCanvasVisible && DateTime.Compare(askedHide, DateTime.MinValue) != 0)
            {
                rpgCanvasVisible = DateTime.Compare(DateTime.Now, askedHide) <= 0;
                if (!rpgCanvasVisible)
                {
                    rpgText.text = "";
                    askedHide = DateTime.MinValue;
                }
            }
        }

        private static void Cartographer_HideView()
        {
            CheckAndCopyRpgFiles();
        }

        private static void RefreshRpgTextCanvas()
        {
            if (textCanvas == null)
            {
                textCanvas = createCanvas(false, "EditorCanvas");

                //Create your Image GameObject
                GameObject bgRpgGO = new GameObject("RpgMod_RpgBackground_Sprite");

                //Make the GameObject child of the Canvas
                bgRpgGO.transform.SetParent(textCanvas.transform);
                Image imgBackground = bgRpgGO.AddComponent<Image>();
                imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
                imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);

                Texture2D backgroundImage = new Texture2D(1920, 1357, TextureFormat.ARGB32, false);
                backgroundImage.LoadImage(ExtractResource("StrandedDeepRpgMod.icons.rpg_background.png"));
                Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 1920, 1357), new Vector2(960, 680));
                imgBackground.sprite = bgSprite;
                //Center Image to screen
                bgRpgGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 220);//Vector2.zero;


                Font defaultFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                //try
                //{
                //    Font font = Resources.GetBuiltinResource(typeof(Font), "BarlowCondensed-SemiBold.ttf") as Font;
                //    if (font != null)
                //        defaultFont = font;
                //}
                //catch { }

                GameObject textRpgGO = new GameObject("RpgMod_RpgText_Sprite");
                textRpgGO.transform.SetParent(textCanvas.transform);
                rpgText = textRpgGO.AddComponent<Text>();
                rpgText.horizontalOverflow = HorizontalWrapMode.Wrap;
                rpgText.verticalOverflow = VerticalWrapMode.Overflow;
                rpgText.alignment = TextAnchor.MiddleCenter;
                rpgText.font = defaultFont;
                rpgText.color = new Color(0.3f, 0.1f, 0f, 0.95f);
                rpgText.fontSize = 15;
                rpgText.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
                rpgText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);
                rpgText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 350);
                rpgText.rectTransform.localPosition = new Vector3(bgRpgGO.GetComponent<RectTransform>().anchoredPosition.x, bgRpgGO.GetComponent<RectTransform>().anchoredPosition.y);

                //GameObject textGO = CreateText(canvas.transform, 0, 0, "Hello world", 32, Color.red);
                //textGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                textCanvas.SetActive(rpgCanvasVisible);
            }
        }

        private static void UpdateRpgContent()
        {
            // Check if RPG file exists if necessary
            if (StrandedWorld.Instance != null
                && StrandedWorld.Instance.Zones != null
                && StrandedWorld.Instance.Zones.Length >= islandCount
                && Beam.Terrain.World.MapList != null
                && Beam.Terrain.World.MapList.Length >= islandCount
                && (indexedRpgText.Count == 0
                || currentlyLoadedSlot != Options.GeneralSettings.LastSaveSlotUsed))
            {
                //Debug.Log("Stranded Deep Rpg mod : StrandedWorld.Instance.Zones.Length : " + StrandedWorld.Instance.Zones.Length);
                currentlyLoadedSlot = Options.GeneralSettings.LastSaveSlotUsed;

                // reload
                indexedRpgText.Clear();
                CheckAndCopyRpgFiles(); // if changed game slot and/or world was recreated
                // must be done once before events are registered
                ReLoadRpgText();

                StrandedWorld.Instance.ZoneEntered -= World_ZoneEntered;
                StrandedWorld.Instance.ZoneEntered += World_ZoneEntered;

                RegisterZoneLoader();
            }
        }

        private static void ReLoadRpgText()
        {
            string rpgTextFile = Path.Combine(dataDirectory, FilePath.SAVE_SLOT_FOLDER_PREFIX + currentlyLoadedSlot.ToString(), configFileName);

            if (indexedRpgText.Count == 0)
            {
                //indexedRpgText.Add("None", "A dull feeling of loneliness comes over me. As I stare into the sky, I wonder if I will ever see home again, or if this despair has become home.");
                indexedRpgText.Add("None", "");
            }

            if (File.Exists(rpgTextFile))
            {
                Debug.Log("Stranded Deep Rpg mod : reloading local rpg text file");
                string[] config = System.IO.File.ReadAllLines(rpgTextFile);
                foreach (string line in config)
                {
                    // Format is <guid>=text
                    string[] tokens = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 1)
                    {
                        if (!indexedRpgText.ContainsKey(tokens[0]))
                            indexedRpgText.Add(tokens[0], "");
                        else
                            indexedRpgText[tokens[0]] = "";
                    }
                    else if (tokens.Length == 2)
                    {
                        string text = tokens[1];
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = text.Replace("\\n", "\n");
                        }
                        if (!indexedRpgText.ContainsKey(tokens[0]))
                            indexedRpgText.Add(tokens[0], text);
                        else
                            indexedRpgText[tokens[0]] = text;
                    }
                    else
                    {
                        // malformed line
                        continue;
                    }
                }
            }
        }

        private static void RegisterZoneLoader()
        {
            ZoneLoader loader = typeof(StrandedWorld).GetField("_zoneLoader", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(StrandedWorld.Instance) as ZoneLoader;
            if (loader != null)
            {
                loader.LoadedZone -= Loader_LoadedZone;
                loader.LoadedZone += Loader_LoadedZone;
            }
        }

        private static void UnregisterZoneLoader()
        {
            ZoneLoader loader = typeof(StrandedWorld).GetField("_zoneLoader", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(StrandedWorld.Instance) as ZoneLoader;
            if (loader != null)
            {
                loader.LoadedZone -= Loader_LoadedZone;
            }
        }

        private static void Loader_LoadedZone(Zone obj)
        {
            try
            {
                Debug.Log("Stranded Deep Rpg mod : zone loaded hook : " + obj.Id);

                MaintainRpgTextReferences();
                ReLoadRpgText();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : zone loaded exception : " + e);
            }
        }

        private static void World_ZoneEntered(Zone obj)
        {
            try
            {
                Debug.Log("Stranded Deep Rpg mod : zone entered hook : " + obj.Id);

                MaintainRpgTextReferences();
                ReLoadRpgText();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : zone entered exception : " + e);
            }
        }

        private static void MaintainRpgTextReferences()
        {
            Debug.Log("Stranded Deep Rpg mod : rpg text maintenance");

            // When islands are discovered, update references
            // Check if island newly discovered
            if (Beam.Terrain.World.MapList != null
                && Beam.Terrain.World.MapList.Length >= islandCount
                && islandDiscovered.Count >= islandCount)
            {
                string rpgTextFile = Path.Combine(dataDirectory, FilePath.SAVE_SLOT_FOLDER_PREFIX + currentlyLoadedSlot.ToString(), configFileName);

                Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;
                for (int islandIndex = 0; islandIndex < maps.Length; islandIndex++)
                {
                    if (StrandedWorld.Instance.Zones.Length > islandIndex)
                    {
                        if (!StrandedWorld.Instance.Zones[islandIndex].IsUserMap)
                            continue;
                        Debug.Log("Stranded Deep Rpg mod : loaded map is user map");
                        bool discovered = StrandedWorld.Instance.Zones[islandIndex].HasVisited || StrandedWorld.Instance.Zones[islandIndex].IsStartingIsland;
                        if (islandDiscovered.ContainsKey(islandIndex))
                        {
                            if (islandDiscovered[islandIndex] != discovered)
                            {
                                Debug.Log("Stranded Deep Rpg mod : loaded map is discovered");
                                islandDiscovered[islandIndex] = discovered;

                                // Update references
                                string currentWorldDirectory = FilePath.WORLD_FOLDER;
                                if (Directory.Exists(Path.Combine(currentWorldDirectory, islandIndex.ToString())))
                                {
                                    string currentMapDirectory = Path.Combine(currentWorldDirectory, islandIndex.ToString());
                                    foreach (string dir in Directory.EnumerateDirectories(currentMapDirectory))
                                    {
                                        if (dir.Contains("PROCEDURAL"))
                                            continue;

                                        if (dir.Contains("MAP") && !dir.Contains("MISSION"))
                                        {
                                            // Read objects file
                                            Map map = maps[islandIndex];
                                            string objectFile = Path.Combine(dir, "MAP_" + map.EditorData.Id + "_OBJECT.map");
                                            if (!File.Exists(objectFile))
                                                continue;

                                            Debug.Log("Stranded Deep Rpg mod : reading map object file on zone entered : " + objectFile);

                                            string rpgFile = Path.Combine(dir, "MAP_" + map.EditorData.Id + "_RPG.map");
                                            if (!File.Exists(rpgFile))
                                            {
                                                Debug.Log("Stranded Deep Rpg mod : zone has no rpg file, skipping");
                                                continue;
                                            }

                                            string objectjson = File.ReadAllText(objectFile);
                                            Dictionary<string, string> indexedGuids = new Dictionary<string, string>();
                                            if (!String.IsNullOrEmpty(objectFile) && objectjson != "null")
                                            {
                                                List<ModMapObject> prefabs = JsonConvert.DeserializeObject<List<ModMapObject>>(objectjson);
                                                Debug.Log("Stranded Deep Rpg mod : reading prefabs on zone entered");
                                                foreach (ModMapObject currentObject in prefabs)
                                                {
                                                    // get the name field
                                                    string uniqueId = currentObject.rpgModUniqueId;
                                                    if (String.IsNullOrEmpty(uniqueId))
                                                        continue;
                                                    StringBuilder sb = new StringBuilder();
                                                    sb.Append(Beam.Serialization.Prefabs.ConvertLegacyPrefabId(currentObject.prefab));
                                                    sb.Append(currentObject.Transform.localPosition.x);
                                                    sb.Append(currentObject.Transform.localPosition.y);
                                                    sb.Append(currentObject.Transform.localPosition.z);
                                                    sb.Append(currentObject.Transform.localRotation.x);
                                                    sb.Append(currentObject.Transform.localRotation.y);
                                                    sb.Append(currentObject.Transform.localRotation.z);
                                                    sb.Append(currentObject.Transform.localRotation.w);
                                                    string key = sb.ToString();
                                                    indexedGuids.Add(key, uniqueId);
                                                    Debug.Log("Stranded Deep Rpg mod : adding to dictionary : " + key + " / unique mod object id (in _OBJECT.map file) : " + currentObject.rpgModUniqueId);
                                                }
                                            }

                                            // Read rpg file
                                            Dictionary<string, string> uniqueObjectEntries = new Dictionary<string, string>();
                                            if (File.Exists(rpgFile))
                                            {
                                                Debug.Log("Stranded Deep Rpg mod : reading map rpg file to get text lines (in _RPG.map file)");
                                                string[] config = System.IO.File.ReadAllLines(rpgFile);
                                                foreach (string line in config)
                                                {
                                                    // Format is <guid>@<type>=text
                                                    string[] tokens = line.Split(new string[] { "=", "@" }, StringSplitOptions.RemoveEmptyEntries);
                                                    // skip objects without text
                                                    if (tokens.Length > 0 && tokens.Length <= 2)
                                                    {
                                                        //uniqueObjectEntries.Add(tokens[0], "");
                                                        Debug.Log("Stranded Deep Rpg mod: skipping empty entry");
                                                    }
                                                    else
                                                    if (tokens.Length == 3)
                                                    {
                                                        uniqueObjectEntries.Add(tokens[0], tokens[2]);
                                                    }
                                                    else
                                                    {
                                                        Debug.Log("Stranded Deep Rpg mod: invalid line format in RPG file (text token must not contain @ or =) : " + line);
                                                        continue;
                                                    }
                                                }
                                            }

                                            StringBuilder sb2 = new StringBuilder();
                                            // Loop on objects, get their references
                                            (from gameobject in UnityEngine.Object.FindObjectsOfType<BaseObject>()
                                             where gameobject is SaveablePrefab
                                             select gameobject).Cast<SaveablePrefab>().ToList<SaveablePrefab>().ForEach(delegate (SaveablePrefab saveableObject)
                                                 {
                                                     Debug.Log("Stranded Deep Rpg mod : found reference on island : " + saveableObject.ReferenceId + " / of prefab : " + saveableObject.PrefabId);
                                                     if (!indexedRpgText.ContainsKey(saveableObject.ReferenceId))
                                                     {
                                                         StringBuilder sb = new StringBuilder();
                                                         sb.Append(saveableObject.PrefabId);
                                                         sb.Append(saveableObject.transform.localPosition.x);
                                                         sb.Append(saveableObject.transform.localPosition.y);
                                                         sb.Append(saveableObject.transform.localPosition.z);
                                                         sb.Append(saveableObject.transform.localRotation.x);
                                                         sb.Append(saveableObject.transform.localRotation.y);
                                                         sb.Append(saveableObject.transform.localRotation.z);
                                                         sb.Append(saveableObject.transform.localRotation.w);
                                                         string key2 = sb.ToString();
                                                         Debug.Log("Stranded Deep Rpg mod : in memory generated object key : " + key2 + " / legacy prefab = " + Beam.Serialization.Prefabs.ConvertPrefabIdToLegacy(saveableObject.PrefabId));
                                                         if (indexedGuids.ContainsKey(key2) 
                                                            && uniqueObjectEntries.ContainsKey(indexedGuids[key2])
                                                            && !indexedRpgText.ContainsKey(saveableObject.ReferenceId))
                                                         {
                                                             sb2.AppendLine(saveableObject.ReferenceId + "=" + uniqueObjectEntries[indexedGuids[key2]]);
                                                             Debug.Log("Stranded Deep Rpg mod : saving rpg text for reference : " + saveableObject.ReferenceId);
                                                         }
                                                         else
                                                         {
                                                            Debug.Log("Stranded Deep Rpg mod : key not found, but reference exists, object most likely not handled by the mod or moved");
                                                         }
                                                     }
                                                 });

                                            // insert into local world rpg file
                                            File.AppendAllText(rpgTextFile, sb2.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string CheckRpgContent(GameObject go = null)
        {
            BaseObject sp = null;
            if (go != null)
                sp = go.GetComponent<BaseObject>();
            string reference = "None";
            if (sp != null)
            {
                reference = sp.ReferenceId;
            }
            else if (go != null)
            {
                reference = go.name;
            }
            if (indexedRpgText.ContainsKey(reference))
            {
                return indexedRpgText[reference];
            }
            return String.Empty;
        }

        #region Canvas instanciation

        //Creates Hidden GameObject and attaches Canvas component to it
        private static GameObject createCanvas(bool hide, string name = "Canvas")
        {
            //Create Canvas GameObject
            GameObject tempCanvas = new GameObject(name);
            if (hide)
            {
                tempCanvas.hideFlags = HideFlags.HideAndDontSave;
            }

            //Create and Add Canvas Component
            Canvas cnvs = tempCanvas.AddComponent<Canvas>();
            cnvs.renderMode = RenderMode.ScreenSpaceOverlay;
            cnvs.pixelPerfect = false;

            //Set Cavas sorting order to be above other Canvas sorting order
            cnvs.sortingOrder = 12;

            cnvs.targetDisplay = 0;

            addCanvasScaler(tempCanvas);
            addGraphicsRaycaster(tempCanvas);
            return tempCanvas;
        }

        //Adds CanvasScaler component to the Canvas GameObject 
        private static void addCanvasScaler(GameObject parentaCanvas)
        {
            CanvasScaler cvsl = parentaCanvas.AddComponent<CanvasScaler>();
            cvsl.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cvsl.referenceResolution = new Vector2(rpgCanvasDefaultScreenWitdh, rpgCanvasDefaultScreenHeight);
            cvsl.matchWidthOrHeight = 0.5f;
            cvsl.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cvsl.referencePixelsPerUnit = 100f;
        }

        //Adds GraphicRaycaster component to the Canvas GameObject 
        private static void addGraphicsRaycaster(GameObject parentaCanvas)
        {
            GraphicRaycaster grcter = parentaCanvas.AddComponent<GraphicRaycaster>();
            grcter.ignoreReversedGraphics = true;
            grcter.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        }

        #endregion

        //static GameObject CreateText(Transform canvas_transform, float x, float y, string text_to_print, int font_size, Color text_color)
        //{
        //    GameObject UItextGO = new GameObject("Text2");
        //    UItextGO.transform.SetParent(canvas_transform);

        //    RectTransform trans = UItextGO.AddComponent<RectTransform>();
        //    trans.anchoredPosition = new Vector2(x, y);

        //    Text text = UItextGO.AddComponent<Text>();
        //    text.text = text_to_print;
        //    text.fontSize = font_size;
        //    text.color = text_color;

        //    return UItextGO;
        //}

        #region RPG intelligence

        //private static LE_GUI3dObject GetSelectedObject()
        //{
        //    //m_GUI3dObject
        //    LE_GUI3dObject selectedObject = typeof(LE_LevelEditorMain).GetField("m_GUI3dObject", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<LE_LevelEditorMain>.Instance) as LE_GUI3dObject;

        //    return selectedObject;
        //}

        //static MethodInfo gcfgo = null;
        ////EventSystem.current.GetComponent<StandaloneInputModule>().GetCurrentFocusedGameObject()
        //public static GameObject GetCurrentFocusedGameObject(this StandaloneInputModule sim)
        //{
        //    if (gcfgo == null)
        //        gcfgo = typeof(StandaloneInputModule).GetMethod("GetCurrentFocusedGameObject", BindingFlags.NonPublic | BindingFlags.Instance);
        //    return gcfgo.Invoke(sim, null) as GameObject;
        //}

        






        // "dName": "TEST TEXT",
        // public class LabelmakerController : ControllerBase, INeedPlayerComponent
        // TMPTextScreenViewAdapter
        // TMPHudViewAdapter _itemNameLabel
        // ILabelViewAdapter ItemNameLabel

        // MapLoader <- where the map is loaded

        #endregion
    }
}
