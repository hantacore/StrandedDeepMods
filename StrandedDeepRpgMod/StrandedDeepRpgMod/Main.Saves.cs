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
        private static bool firstLoad = false;
        private static GameObject editorCanvas;
        private static Text editorText;
        private static bool editorCanvasVisible;
        private static GameObject editorTextCanvas;
        private static Text editorRpgText;
        private static bool editorTextCanvasVisible;
        private static LE_GUI3dObject selectedObject = null;
        private static string editorTextPrefix = "How to use : + to show text, - to hide, * to force re-read RPG file\nRpg Object Unique Reference\n";
        private static Dictionary<string, string> indexedEditorRpgText = new Dictionary<string, string>();

        /// <summary>
        /// UPDATE
        /// </summary>
        private static void HandleSavesAndUniqueIds()
        {
            Event currentevent = Event.current;
            if (currentevent.isKey)
            {
                if (currentevent.keyCode == KeyCode.KeypadPlus)
                {
                    editorTextCanvasVisible = true;
                }
                if (currentevent.keyCode == KeyCode.KeypadMinus)
                {
                    editorTextCanvasVisible = false;
                }
                if (currentevent.keyCode == KeyCode.KeypadMultiply)
                {
                    ReloadRpgTextEditor();
                }
            }
            if (!firstLoad)
            {
                ReloadRpgTextEditor();
            }
            try
            {
                editorCanvasVisible = (Beam.Game.State == Beam.GameState.MAP_EDITOR);
                RefreshEditorCanvas();
                if (Beam.Game.State == Beam.GameState.MAP_EDITOR)
                {
                    if (selectedObject == null && Beam.Game.State == Beam.GameState.MAP_EDITOR)
                    {
                        selectedObject = typeof(LE_LevelEditorMain).GetField("m_GUI3dObject", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(LE_LevelEditorMain.Instance) as LE_GUI3dObject;
                    }

                    if (Beam.Game.State != Beam.GameState.MAP_EDITOR)
                        selectedObject = null;

                    if (selectedObject != null 
                        && selectedObject.SelectedObject != null
                        && editorText != null)
                    {
                        editorText.text = editorTextPrefix + selectedObject.SelectedObject.EditorObjectName + " : " + selectedObject.SelectedObject.name;
                    }
                    if (indexedEditorRpgText !=null
                        && selectedObject != null
                        && selectedObject.SelectedObject != null
                        && indexedEditorRpgText.ContainsKey(selectedObject.SelectedObject.name))
                    {
                        editorRpgText.text = indexedEditorRpgText[selectedObject.SelectedObject.name];
                    }
                    else
                    {
                        editorRpgText.text = "";
                    }
                }
                else
                {
                    editorTextCanvasVisible = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : refresh editor canvas failed : " + e);
            }

            if (Beam.Game.State != Beam.GameState.MAP_EDITOR
                && !uniqueIdRestorePending)
            {
                Debug.Log("Stranded Deep Rpg mod : exited map editor");
                uniqueIdRestorePending = true;
            }

            if (Beam.Game.State == Beam.GameState.MAP_EDITOR
                                && (Presenter == null || SavePresenter == null))
            {
                Beam.UI.MapEditor.MapEditorMenuPresenter mem = Beam.Game.FindObjectOfType<Beam.UI.MapEditor.MapEditorMenuPresenter>();
                if (mem != null)
                {
                    Debug.Log("Stranded Deep Rpg mod : MapEditorMenuPresenter found");
                    Presenter = mem;
                }

                Beam.UI.MapEditor.MapEditorSaveMenuPresenter mesm = Beam.Game.FindObjectOfType<Beam.UI.MapEditor.MapEditorSaveMenuPresenter>();
                if (mesm != null)
                {
                    Debug.Log("Stranded Deep Rpg mod : MapEditorSaveMenuPresenter found");
                    SavePresenter = mesm;
                }

                if (Presenter != null && SavePresenter != null)
                {
                    HookEvents();
                }
            }

            if (Presenter != null)
            {
                // find out if presenter has been initialized
                bool? memIsInitialized = null;
                // perf tweak
                if (!EditorInitialized)
                    memIsInitialized = typeof(MapEditorMenuPresenter).GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Presenter) as bool?;

                if (EditorInitialized || memIsInitialized.GetValueOrDefault(false))
                {
                    if (!EditorInitialized)
                        Debug.Log("Stranded Deep Rpg mod : MapEditorMenuPresenter finished initializing");

                    EditorInitialized = true;

                    // add the unique id to the objects based on their name + position and rotation (build a key ?)
                    Map currentMap = MapEditorMenuPresenter.MAP_TO_EDIT;//typeof(MapEditorMenuPresenter).GetField("_currentMap", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Presenter) as Map;
                    if (currentMap != null)
                    {
                        //Debug.Log("Stranded Deep Rpg mod : Map found");
                        if (lastMapId == null || String.Compare(lastMapId, currentMap.EditorData.Id) != 0)
                        {
                            lastMapId = currentMap.EditorData.Id; //<- guid de la map ?
                            Debug.Log("Stranded Deep Rpg mod : new Map id : " + lastMapId);

                            uniqueIdRestorePending = (currentMap.ObjectData != null && currentMap.ObjectData.Children != null && currentMap.ObjectData.Children.Count > 0);
                        }
                    }
                }
            }

            try
            {
                if (uniqueIdRestorePending
                    && Singleton<LE_LevelEditorMain>.Instance.ObjectDumpContainer.GetComponentsInChildren<LE_Object>(true).Count() > 0)
                {
                    Map currentMap = MapEditorMenuPresenter.MAP_TO_EDIT;
                    InjectObjectIds(currentMap.EditorData.Id);
                    uniqueIdRestorePending = false;
                }
            }
            catch { }

            if (Beam.Game.State != Beam.GameState.MAP_EDITOR
                && EditorInitialized)
            {
                // is destroying, force cleanup call
                EditorView_HideView();
            }
        }

        private static void RefreshEditorCanvas()
        {
            try
            {
                if (editorCanvas == null)
                {
                    editorCanvas = createCanvas(false, "RpgCanvas");

                    //Create your Image GameObject
                    GameObject bgEditorGO = new GameObject("RpgMod_EditorBackground_Sprite");

                    //Make the GameObject child of the Canvas
                    bgEditorGO.transform.SetParent(editorCanvas.transform);
                    Image imgBackground = bgEditorGO.AddComponent<Image>();
                    imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);
                    imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

                    Texture2D backgroundImage = new Texture2D(1920, 1357, TextureFormat.ARGB32, false);
                    backgroundImage.LoadImage(ExtractResource("StrandedDeepRpgMod.icons.rpg_background.png"));
                    Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 1920, 1357), new Vector2(960, 680));
                    imgBackground.sprite = bgSprite;
                    //Center Image to screen
                    bgEditorGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 300);//Vector2.zero;


                    Font defaultFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    GameObject textEditorGO = new GameObject("RpgMod_EditorText_Sprite");
                    textEditorGO.transform.SetParent(editorCanvas.transform);
                    editorText = textEditorGO.AddComponent<Text>();
                    editorText.horizontalOverflow = HorizontalWrapMode.Wrap;
                    editorText.verticalOverflow = VerticalWrapMode.Overflow;
                    editorText.alignment = TextAnchor.MiddleCenter;
                    editorText.font = defaultFont;
                    editorText.color = new Color(0.3f, 0.1f, 0f, 0.95f);
                    editorText.fontSize = 12;
                    editorText.text = editorTextPrefix + "OBJECT NAME : " + Guid.Empty.ToString();
                    editorText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
                    editorText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 450);
                    editorText.rectTransform.localPosition = new Vector3(bgEditorGO.GetComponent<RectTransform>().anchoredPosition.x, bgEditorGO.GetComponent<RectTransform>().anchoredPosition.y);
                }
                else
                {
                    editorCanvas.SetActive(editorCanvasVisible);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : RpgCanvas handling failed" + e);
            }
            try
            {
                if (editorTextCanvas == null)
                {
                    editorTextCanvas = createCanvas(false, "EditorTextCanvas");

                    //Create your Image GameObject
                    GameObject bgEditorRpgGO = new GameObject("RpgMod_EditorRpgBackground_Sprite");

                    //Make the GameObject child of the Canvas
                    bgEditorRpgGO.transform.SetParent(editorTextCanvas.transform);
                    Image imgBackground = bgEditorRpgGO.AddComponent<Image>();
                    imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
                    imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);

                    Texture2D backgroundImage = new Texture2D(1920, 1357, TextureFormat.ARGB32, false);
                    backgroundImage.LoadImage(ExtractResource("StrandedDeepRpgMod.icons.rpg_background.png"));
                    Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 1920, 1357), new Vector2(960, 680));
                    imgBackground.sprite = bgSprite;
                    //Center Image to screen
                    bgEditorRpgGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);//Vector2.zero;


                    Font defaultFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    GameObject textEditorRpgGO = new GameObject("RpgMod_EditorRpgText_Sprite");
                    textEditorRpgGO.transform.SetParent(editorTextCanvas.transform);
                    editorRpgText = textEditorRpgGO.AddComponent<Text>();
                    editorRpgText.horizontalOverflow = HorizontalWrapMode.Wrap;
                    editorRpgText.verticalOverflow = VerticalWrapMode.Overflow;
                    editorRpgText.alignment = TextAnchor.MiddleCenter;
                    editorRpgText.font = defaultFont;
                    editorRpgText.color = new Color(0.3f, 0.1f, 0f, 0.95f);
                    editorRpgText.fontSize = 15;
                    editorRpgText.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
                    editorRpgText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);
                    editorRpgText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 350);
                    editorRpgText.rectTransform.localPosition = new Vector3(bgEditorRpgGO.GetComponent<RectTransform>().anchoredPosition.x, bgEditorRpgGO.GetComponent<RectTransform>().anchoredPosition.y);
                }
                else
                {
                    editorTextCanvas.SetActive(editorTextCanvasVisible);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : EditorCanvas handling failed" + e);
            }
        }

        #region Map saving hook

        private static IMapEditorMenuViewAdapter GetView(Beam.UI.MapEditor.MapEditorMenuPresenter mem)
        {
            if (mem == null)
                return null;

            IMapEditorMenuViewAdapter viewVariable = typeof(Beam.UI.MapEditor.MapEditorMenuPresenter).GetField("_view", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mem) as IMapEditorMenuViewAdapter;
            return viewVariable;
        }

        private static IMapEditorSaveMenuViewAdapter GetView(Beam.UI.MapEditor.MapEditorSaveMenuPresenter mesm)
        {
            if (mesm == null)
                return null;

            IMapEditorSaveMenuViewAdapter viewVariable = typeof(Beam.UI.MapEditor.MapEditorSaveMenuPresenter).GetField("_view", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mesm) as IMapEditorSaveMenuViewAdapter;
            return viewVariable;
        }

        private static void HookEvents()
        {
            if (Presenter == null || SavePresenter == null)
                return;

            Debug.Log("Stranded Deep Rpg mod : hooking events");

            IMapEditorMenuViewAdapter view = GetView(Presenter);
            view.HideView -= EditorView_HideView;
            view.HideView += EditorView_HideView;

            // find detach events method and call
            IMapEditorSaveMenuViewAdapter sview = GetView(SavePresenter);
            sview.SaveButton.ClearEventSubscribers();
            // hook new event handler
            sview.SaveButton.Click += SaveButtonOverload_Click;
        }

        private static void EditorView_HideView()
        {
            if (Presenter != null)
            {
                Debug.Log("Stranded Deep Rpg mod : editor closed or MAP_TO_EDIT null, cleaning up");
                IMapEditorMenuViewAdapter view = GetView(Presenter);
                view.HideView -= EditorView_HideView;
                Presenter = null;
            }

            if (SavePresenter != null)
            {
                IMapEditorSaveMenuViewAdapter sview = GetView(SavePresenter);
                sview.SaveButton.ClearEventSubscribers();
                SavePresenter = null;
            }

            if (Presenter == null && SavePresenter == null)
                EditorInitialized = false;
        }

        private static void ReloadRpgTextEditor()
        {
            indexedEditorRpgText.Clear();
            try
            {
                Map currentMap = MapEditorMenuPresenter.MAP_TO_EDIT;
                if (currentMap == null)
                    return;

                string currentMapDirectory = Path.Combine(mapsDirectory, "MAP_" + currentMap.EditorData.Id);
                if (!Directory.Exists(currentMapDirectory))
                    return;

                string editorRpgTextFile = Path.Combine(currentMapDirectory, "MAP_" + currentMap.EditorData.Id + "_RPG.map");
                if (!File.Exists(editorRpgTextFile))
                    return;

                firstLoad = true;

                string[] rpgLines = System.IO.File.ReadAllLines(editorRpgTextFile);
                foreach (string line in rpgLines)
                {
                    // Format is <guid>=text
                    string[] tokens = line.Split(new string[] { "=", "@" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length > 0 && tokens.Length <= 2)
                    {
                        //Debug.Log("Stranded Deep Rpg mod: skipping empty entry");
                    }
                    else if (tokens.Length == 3)
                    {
                        string text = tokens[2];
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = text.Replace("\\n", "\n");
                        }
                        indexedEditorRpgText.Add(tokens[0], text);
                    }
                    else if (tokens.Length >= 3)
                    {
                        if (!indexedEditorRpgText.ContainsKey(tokens[0]))
                            indexedEditorRpgText.Add(tokens[0], "Stranded Deep Rpg mod: invalid line format in RPG file (text token must not contain '@' or '=')");
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod :could not reload rpg text : " + e);
            }
        }

        private static void InjectObjectIds(string id)
        {
            try
            {
                // Get map file
                string currentMapDirectory = Path.Combine(mapsDirectory, "MAP_" + id);
                Debug.Log("Stranded Deep Rpg mod : reading map directory on editor load : " + currentMapDirectory);
                if (!Directory.Exists(currentMapDirectory))
                    return;

                string objectFile = Path.Combine(currentMapDirectory, "MAP_" + id + "_OBJECT.map");
                Debug.Log("Stranded Deep Rpg mod : reading map object file on editor load : " + objectFile);
                if (!File.Exists(objectFile))
                    return;

                string objectjson = File.ReadAllText(objectFile);
                Dictionary<string, string> indexedGuids = new Dictionary<string, string>();
                if (!String.IsNullOrEmpty(objectFile) && objectjson != "null")
                {
                    List<ModMapObject> prefabs = JsonConvert.DeserializeObject<List<ModMapObject>>(objectjson);
                    Debug.Log("Stranded Deep Rpg mod : reading prefabs on editor load");
                    foreach (ModMapObject currentObject in prefabs)
                    {
                        // get the name field
                        string uniqueId = currentObject.rpgModUniqueId;
                        if (String.IsNullOrEmpty(uniqueId))
                            continue;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(currentObject.prefab);
                        sb.Append(currentObject.Transform.localPosition.x);
                        sb.Append(currentObject.Transform.localPosition.y);
                        sb.Append(currentObject.Transform.localPosition.z);
                        sb.Append(currentObject.Transform.localRotation.x);
                        sb.Append(currentObject.Transform.localRotation.y);
                        sb.Append(currentObject.Transform.localRotation.z);
                        sb.Append(currentObject.Transform.localRotation.w);
                        string key = sb.ToString();
                        indexedGuids.Add(key, uniqueId);
                        Debug.Log("Stranded Deep Rpg mod : adding to dictionary : " + key + " / " + currentObject.rpgModUniqueId);
                    }
                }

                // inject it in the right object
                Debug.Log("Stranded Deep Rpg mod : restoring unique ids on editor load");
                Debug.Log("Stranded Deep Rpg mod : currently loaded objects : " + Singleton<LE_LevelEditorMain>.Instance.ObjectDumpContainer.GetComponentsInChildren<LE_Object>(true).Count());
                Singleton<LE_LevelEditorMain>.Instance.ObjectDumpContainer.GetComponentsInChildren<LE_Object>(true).Slinq<LE_Object>().ForEach(delegate (LE_Object le)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Beam.Serialization.Prefabs.ConvertPrefabIdToLegacy(le.Id));
                    sb.Append(le.transform.localPosition.x);
                    sb.Append(le.transform.localPosition.y);
                    sb.Append(le.transform.localPosition.z);
                    sb.Append(le.transform.localRotation.x);
                    sb.Append(le.transform.localRotation.y);
                    sb.Append(le.transform.localRotation.z);
                    sb.Append(le.transform.localRotation.w);
                    string key = sb.ToString();
                    Debug.Log("Stranded Deep Rpg mod : looking for key : " + key);
                    if (indexedGuids.ContainsKey(key))
                    {
                        le.name = indexedGuids[key];
                        Debug.Log("Stranded Deep Rpg mod : restoring unique id : " + key + " / " + indexedGuids[key]);
                    }
                });
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : object unique id injection failed on editor load : " + e);
            }
        }

        private static void UpdateRpgFile(string id)
        {
            // Get map file
            string currentMapDirectory = Path.Combine(mapsDirectory, "MAP_" + id);
            if (!Directory.Exists(currentMapDirectory))
                return;

            string objectFile = Path.Combine(currentMapDirectory, "MAP_" + id + "_OBJECT.map");
            if (!File.Exists(objectFile))
                return;

            Dictionary<string, string> uniqueObjectEntries = new Dictionary<string, string>();
            Singleton<LE_LevelEditorMain>.Instance.ObjectDumpContainer.GetComponentsInChildren<LE_Object>(true).Slinq<LE_Object>().ForEach(delegate (LE_Object le)
            {
                if (!uniqueObjectEntries.ContainsKey(le.name))
                    uniqueObjectEntries.Add(le.name + "@" + le.EditorObjectName, "");
            });

            string rpgFile = Path.Combine(currentMapDirectory, "MAP_" + id + "_RPG.map");
            if (File.Exists(rpgFile))
            {
                string[] config = System.IO.File.ReadAllLines(rpgFile);
                foreach (string line in config)
                {
                    string[] tokens = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 1)
                    {
                        if (!uniqueObjectEntries.ContainsKey(tokens[0]))
                        {
                            if (autoCleanRpgReferences)
                                uniqueObjectEntries.Remove(tokens[0]);
                            else
                                uniqueObjectEntries.Add(tokens[0], "");
                        }
                        else
                        {
                            uniqueObjectEntries[tokens[0]] = "";
                        }
                    }
                    else if (tokens.Length == 2)
                    {
                        if (!uniqueObjectEntries.ContainsKey(tokens[0]))
                        {
                            if (autoCleanRpgReferences)
                                uniqueObjectEntries.Remove(tokens[0]);
                            else
                                uniqueObjectEntries.Add(tokens[0], tokens[1]);
                        }
                        else
                        {
                            uniqueObjectEntries[tokens[0]] = tokens[1];
                        }
                    }
                    else
                    {
                        Debug.Log("Stranded Deep Rpg mod: invalid line format in RPG file (text token must not contain @ or =) : " + line);
                        continue;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (string key in uniqueObjectEntries.Keys)
            {
                sb.AppendLine(key + "=" + uniqueObjectEntries[key]);
            }
            File.WriteAllText(rpgFile, sb.ToString());

            try
            {
                CheckAndCopyRpgFiles();
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Rpg mod : CheckAndCopyRpgFiles failed after editor save");
            }
        }

        private static void SaveButtonOverload_Click(object sender)
        {
            Debug.Log("Stranded Deep Rpg mod : called SaveButtonOverload_Click");

            if (string.IsNullOrEmpty(GetView(SavePresenter).NameInputField.Text))
            {
                DialogueBox.Show(ReInput.players.GetPlayer(0), "MAP_EDITOR_SAVE_MENU_DB_SORRY_TITLE", "MAP_EDITOR_SAVE_MENU_DB_SAVE_NAME_DESC", DialogueButton.Ok, DialogueIcon.Warning, DialogueType.Base);
                return;
            }
            if (string.IsNullOrEmpty(GetView(SavePresenter).DescriptionInputField.Text))
            {
                DialogueBox.Show(ReInput.players.GetPlayer(0), "MAP_EDITOR_SAVE_MENU_DB_SORRY_TITLE", "MAP_EDITOR_SAVE_MENU_DB_SAVE_DESCRIPTION_DESC", DialogueButton.Ok, DialogueIcon.Warning, DialogueType.Base);
                return;
            }
            if (Maps.DefaultMapList.FirstOrDefault((Map map) => map.EditorData.Id == SavePresenter.CurrentMap.EditorData.Id) != null)
            {
                DialogueBox.Show(ReInput.players.GetPlayer(0), "MAP_EDITOR_SAVE_MENU_DB_SAVE_TITLE", "MAP_EDITOR_SAVE_MENU_DB_SAVE_RESERVED_DESC", DialogueButton.Ok, DialogueIcon.Warning, DialogueType.Base);
                return;
            }
            if (Maps.UserMapList.FirstOrDefault((Map map) => map.EditorData.Id == SavePresenter.CurrentMap.EditorData.Id) != null)
            {
                DialogueBox.Show(ReInput.players.GetPlayer(0), "MAP_EDITOR_SAVE_MENU_DB_SAVE_TITLE", "MAP_EDITOR_SAVE_MENU_DB_SAVE_EXISTING_DESC", DialogueButton.YesNo, DialogueIcon.None, DialogueType.Base).Accept += SaveMapOverload;
                return;
            }
            SaveMapOverload();
        }

        private static void SaveMapOverload()
        {
            Debug.Log("Stranded Deep Rpg mod : called SAVE OVERLOAD");

            SavePresenter.CurrentMap.HeightmapData = Singleton<LE_ZoneTileGenerator>.Instance.terrain.terrainData.GetHeights(0, 0, 257, 257);
            JObject objects = new JObject();
            Singleton<LE_LevelEditorMain>.Instance.ObjectDumpContainer.GetComponentsInChildren<LE_Object>(true).Slinq<LE_Object>().ForEach(delegate (LE_Object le)
            {
                JObject jobject = new JObject();
                jobject.AddField("prefab", Beam.Serialization.Prefabs.ConvertPrefabIdToLegacy(le.Id));
                JObject data = JSerializer.Serialize(le.transform);
                jobject.AddField("Transform", data);
                
                // MAGIC IS HERE
                Guid currentId = Guid.Empty;
                if (String.IsNullOrEmpty(le.name)
                    || !Guid.TryParse(le.name, out currentId))
                {
                    currentId = Guid.NewGuid();
                    le.name = currentId.ToString();
                }
                jobject.AddField("rpgModUniqueId", currentId.ToString());
                Debug.Log("Stranded Deep Rpg mod : object unique id : " + currentId);
                // END MAGIC

                objects.Add(jobject);
            });
            SavePresenter.CurrentMap.ObjectData = objects;
            SavePresenter.CurrentMap.EditorData.Author = GAccountService.Get().LocalUser.UserInfo.DisplayName;
            SavePresenter.CurrentMap.EditorData.Name = GetView(SavePresenter).NameInputField.Text.Trim();
            SavePresenter.CurrentMap.EditorData.Description = GetView(SavePresenter).DescriptionInputField.Text.Trim();
            SavePresenter.CurrentMap.EditorData.VersionNumber++;
            SavePresenter.CurrentMap.EditorData.DateEdited = DateTime.Now.ToShortDateString();
            bool flag = true;
            try
            {
                Maps.SaveMap(SavePresenter.CurrentMap, FilePath.LOCAL_MAPS_FOLDER);
                // maintain RPG file
                try
                {
                    UpdateRpgFile(SavePresenter.CurrentMap.EditorData.Id);
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Rpg mod : update rpg file failed : " + e);
                }
            }
            catch (Exception ex)
            {
                string message = Localization.GetLanguageHandler().Localize("MAP_EDITOR_SAVE_MENU_DB_ERROR_SAVING_MAP_DESC", new string[]
                {
                    ex.Message
                });
                DialogueBox.Show(ReInput.players.GetPlayer(0), "MAP_EDITOR_SAVE_MENU_DB_ERROR_TITLE", message, DialogueButton.Ok, DialogueIcon.None, DialogueType.Base);
                flag = false;
            }
            if (flag)
            {
                // reflection to call a protected method
                MethodInfo dynMethod = SavePresenter.GetType().GetMethod("OnSaved", BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(SavePresenter, new object[] { });

                SavePresenter.Hide();
            }
        }

        #endregion
    }
}
