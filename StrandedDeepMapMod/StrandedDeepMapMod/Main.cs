using Beam;
using Funlabs;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepMapMod
{
    static class Main
    {
        private static float ingameAreaSize = 3000f;
        private static float viewDistance = 1000f;

        private static float mapCanvasDefaultScreenWitdh = 1024f;
        private static float mapCanvasDefaultScreenHeight = 768f;

        private static float mapAreaHeight = 650f;
        private static float mapAreaWidth = 650f;

        private static float islandSilhouetteSize = 40f;
        private static float iconSize = 40f;
        private static float missionIconSize = 60f;
        private static float playerIconSize = 50f;

        private static bool showPlayers = true;
        private static bool revealWorld = false;
        private static bool revealMissions = false;
        private static bool debugMode = false;
        private static bool showAlternativeEndingCargo = false;
        private static bool isAlternativeEndingModLoaded = false;
        private static bool lastRevealWorld = false;
        private static bool lastRevealMissions = false;
        private static bool lastDebugMode = false;
        private static bool _forceReloadPlayer = false;

        private static float screenRatioConversion = 1f;

        private static float mapScaleX = 1f;
        private static float mapScaleY = 1f;

        public static bool visible;
        //public static UnityModManager.ModEntry mod;
        private static GameObject canvas;
        private static Image imgBackground;
        private static Dictionary<int, GameObject> goPlayers;
        private static Dictionary<int, GameObject> goDirectionPlayers;
        private static Dictionary<int, Image> imgPlayers;
        private static Dictionary<int, Image> imgDirectionPlayers;
        private static Dictionary<int, GameObject> goIslands;
        private static Dictionary<int, Image> imgIslands;
        private static Dictionary<int, Texture2D> islandSilhouetteTextures;
        private static Dictionary<int, bool> islandDiscovered;
        private static Dictionary<int, bool> islandInSight;

        private static string ENDGAME_CARGO_NAME = "EVERGREEN";
        private static SaveablePrefab cargoInstance = null;
        private static Image imgCargo;
        private static GameObject goCargo;

        // most important
        private static int currentlyLoadedSlot = -1;

        //static bool needsReset = false;

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();

        static string _infojsonlocation = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedDeepMapMod/StrandedDeepMapMod/Info.json";

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;

                goPlayers = new Dictionary<int, GameObject>();
                imgPlayers = new Dictionary<int, Image>();
                goDirectionPlayers = new Dictionary<int, GameObject>();
                imgDirectionPlayers = new Dictionary<int, Image>();
                goIslands = new Dictionary<int, GameObject>();
                imgIslands = new Dictionary<int, Image>();
                islandSilhouetteTextures = new Dictionary<int, Texture2D>();
                islandDiscovered = new Dictionary<int, bool>();
                islandInSight = new Dictionary<int, bool>();

                try
                {
                    VersionChecker.CheckVersion(modEntry, _infojsonlocation);

                    ReadConfig();
                }
                catch { }

                if (WorldUtilities.IsStrandedWide())
                {
                    ComputeGameAreaSize();
                }

                InitMainCanvas();

                Debug.Log("Stranded Deep Map mod properly loaded");

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod error on load : " + e);
            }
            finally
            {
                if (chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep Map mod load time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }

            return false;
        }

        private static void ComputeGameAreaSize()
        {
            float zoneSize = WorldUtilities.ZoneSize;
            Debug.Log("Stranded Deep Map mod zone size = " + WorldUtilities.ZoneSize);
            float zoneSpacing = WorldUtilities.ZoneSpacing;
            float numberOfIslandsOnDiameter = 8f;
            Debug.Log("Stranded Deep Map mod size ratio = " + WorldUtilities.IslandSizeRatio);
            ingameAreaSize = (zoneSize * zoneSpacing * (numberOfIslandsOnDiameter + 1)) / (WorldUtilities.ZoneSize == 256 ? 2 : (WorldUtilities.ZoneSize == 512 ? 1 : 2));
            Debug.Log("Stranded Deep Map mod ingameAreaSize = " + ingameAreaSize);
        }

        private static void InitMainCanvas()
        {
            if (canvas != null)
            {
                return;
            }

            Debug.Log("Stranded Deep Map mod: init main canvas");

            // init calculations
            float defaultScreenRatio = mapCanvasDefaultScreenWitdh / mapCanvasDefaultScreenHeight;
            float currentScreenRatio = (float)Screen.width / (float)Screen.height;
            screenRatioConversion = currentScreenRatio / defaultScreenRatio;
            mapScaleX = (float)Screen.width / (mapCanvasDefaultScreenWitdh * screenRatioConversion);
            mapScaleY = (float)Screen.height / mapCanvasDefaultScreenHeight;

            //Create main Canvas
            canvas = createCanvas(false);

            // Stupid easter egg
            //GameObject bonusGO = new GameObject("Bonus_Sprite");
            //Image imgBonus = bonusGO.AddComponent<Image>();
            //bonusGO.transform.SetParent(canvas.transform);
            //imgBonus.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);
            //imgBonus.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);

            //Texture2D bonusImage = new Texture2D(1016, 1226, TextureFormat.ARGB32, false);
            //bonusImage.LoadImage(ExtractResource("StrandedDeepMapMod.icons.bonus.png"));
            //Sprite bonusSprite = Sprite.Create(bonusImage, new Rect(0, 0, 1016, 1226), new Vector2(508, 613));
            //imgBonus.sprite = bonusSprite;
            //imgBonus.rectTransform.localPosition = TransformToScreenCoordinates(-2300, 0);



            //Create your Image GameObject
            GameObject bgGO = new GameObject("Background_Sprite");

            //Make the GameObject child of the Canvas
            bgGO.transform.SetParent(canvas.transform);

            //Add Image Component to it(This will add RectTransform as-well)
            imgBackground = bgGO.AddComponent<Image>();
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapAreaHeight);
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapAreaWidth);

            Texture2D backgroundImage = new Texture2D(2000, 2000, TextureFormat.ARGB32, false);
            backgroundImage.LoadImage(ExtractResource("StrandedDeepMapMod.icons.background.png"));
            Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 2000, 2000), new Vector2(1000, 1000));
            imgBackground.sprite = bgSprite;

            //Center Image to screen
            bgGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Add compass
            GameObject compassGO = new GameObject("Compass_Sprite");
            compassGO.transform.SetParent(canvas.transform);
            Texture2D canvasIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
            canvasIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.compass.png"));
            Sprite compassSprite = Sprite.Create(canvasIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170)); ;
            Image imgCompass = compassGO.AddComponent<Image>();
            imgCompass.sprite = compassSprite;
            imgCompass.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
            imgCompass.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
            if (WorldUtilities.IsStrandedWide())
            {
                imgCompass.rectTransform.localPosition = TransformToScreenCoordinates((ingameAreaSize/2) + 300, (ingameAreaSize / 2) + 200);
            }
            else
                imgCompass.rectTransform.localPosition = TransformToScreenCoordinates(2300, 2200);

            // default hide
            if (canvas != null)
                canvas.SetActive(false);
        }

        private static void ClearMainCanvas()
        {
            try
            {
                if (canvas == null)
                {
                    return;
                }
                //Debug.Log("Stranded Deep Map mod: clear all");
                visible = false;
                ClearIslandImages();
                ClearPlayerImages(true);
                if (islandSilhouetteTextures.Count > 0)
                {
                    for (int i = 0; i < islandSilhouetteTextures.Count; i++)
                    {
                        Beam.Game.Destroy(islandSilhouetteTextures[i]);
                    }
                    islandSilhouetteTextures.Clear();
                }

                islandDiscovered.Clear();
                islandInSight.Clear();

                if (canvas != null)
                {
                    Beam.Game.Destroy(canvas);
                }
                canvas = null;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod: clear all failed : " + e);
            }
        }

        private static void ClearIslandImages()
        {
            if (imgIslands.Count > 0)
            {
                for (int i = 0; i < imgIslands.Count; i++)
                {
                    Beam.Game.Destroy(imgIslands[i]);
                }
                imgIslands.Clear();
            }
            if (goIslands.Count > 0)
            {
                for (int i = 0; i < goIslands.Count; i++)
                {
                    Beam.Game.Destroy(goIslands[i]);
                }
                goIslands.Clear();
            }
            if (islandSilhouetteTextures.Count > 0)
            {
                for (int i = 0; i < islandSilhouetteTextures.Count; i++)
                {
                    Beam.Game.Destroy(islandSilhouetteTextures[i]);
                }
                islandSilhouetteTextures.Clear();
            }
        }

        private static void ClearPlayerImages(bool forceReloadPlayer)
        {
            Debug.Log("Stranded Deep Map mod : clearing player sprites " + forceReloadPlayer);
            _forceReloadPlayer = forceReloadPlayer;
            if (imgPlayers.Count > 0)
            {
                for (int i = 0; i < imgPlayers.Count; i++)
                {
                    Beam.Game.Destroy(imgPlayers[i]);
                }
                imgPlayers.Clear();
            }

            if (imgDirectionPlayers.Count > 0)
            {
                for (int i = 0; i < imgDirectionPlayers.Count; i++)
                {
                    Beam.Game.Destroy(imgDirectionPlayers[i]);
                }
                imgDirectionPlayers.Clear();
            }
            if (goPlayers.Count > 0)
            {
                for (int i = 0; i < goPlayers.Count; i++)
                {
                    Beam.Game.Destroy(goPlayers[i]);
                }
                goPlayers.Clear();
            }
            if (goDirectionPlayers.Count > 0)
            {
                for (int i = 0; i < goDirectionPlayers.Count; i++)
                {
                    Beam.Game.Destroy(goDirectionPlayers[i]);
                }
                goDirectionPlayers.Clear();
            }
        }

        #region Canvas instanciation

        //Creates Hidden GameObject and attaches Canvas component to it
        private static GameObject createCanvas(bool hide)
        {
            //Create Canvas GameObject
            GameObject tempCanvas = new GameObject("Canvas");
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

            //CanvasGroup cg = tempCanvas.AddComponent<CanvasGroup>();
            //cg.alpha = 0.75f;

            return tempCanvas;
        }

        //Adds CanvasScaler component to the Canvas GameObject 
        private static void addCanvasScaler(GameObject parentaCanvas)
        {
            CanvasScaler cvsl = parentaCanvas.AddComponent<CanvasScaler>();
            cvsl.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cvsl.referenceResolution = new Vector2(mapCanvasDefaultScreenWitdh, mapCanvasDefaultScreenHeight);
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

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Map mod by Hantacore");
            GUILayout.Label("How to use : F8 = show map / F9 = hide map");
            GUILayout.Label("View distance");
            viewDistance = GUILayout.HorizontalSlider(viewDistance, 600f, 1500f);
            showPlayers = GUILayout.Toggle(showPlayers, "Show players positions");
            revealWorld = GUILayout.Toggle(revealWorld, "Reveal world (cheat)");
            revealMissions = GUILayout.Toggle(revealMissions, "Reveal missions (cheat)");
            GUILayout.Label("Stranded Wide World mode : " + WorldUtilities.IsStrandedWide() + " (restart if changed)");
            showAlternativeEndingCargo = GUILayout.Toggle(showAlternativeEndingCargo, "Show endgame cargo (cheat - only use with Stranded Deep Alternative Ending)");
            GUILayout.Label("Player icon size");
            playerIconSize = GUILayout.HorizontalSlider(playerIconSize, 10f, 100f);

            //IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
            //// show only if ingame
            //if (players.Count > 0)
            //{
            //    Beam.Crafting.CraftingCombination craftingCombination3 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "AIRCRAFT ENGINE PART");
            //    craftingCombination3.Unlocked = GUILayout.Toggle(craftingCombination3.Unlocked, "Unlock AIRCRAFT ENGINE PART for player 1 (cheat)");

            //    Beam.Crafting.CraftingCombination craftingCombination6 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "AIRCRAFT RUDDER PART");
            //    craftingCombination6.Unlocked = GUILayout.Toggle(craftingCombination6.Unlocked, "Unlock AIRCRAFT RUDDER PART for player 1 (cheat)");

            //    Beam.Crafting.CraftingCombination craftingCombination9 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "AIRCRAFT PROPELLER PART");
            //    craftingCombination9.Unlocked = GUILayout.Toggle(craftingCombination9.Unlocked, "Unlock AIRCRAFT PROPELLER PART for player 1 (cheat)");

            //    // Seems enabled by default
            //    //Beam.Crafting.CraftingCombination craftingCombination2 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "GYROCOPTER FRAME");
            //    //craftingCombination2.Unlocked = GUILayout.Toggle(craftingCombination2.Unlocked, "Unlock GYROCOPTER FRAME for player 1 (cheat)");

            //    //Beam.Crafting.CraftingCombination craftingCombination5 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "GYROCOPTER MOTOR");
            //    //craftingCombination5.Unlocked = GUILayout.Toggle(craftingCombination5.Unlocked, "Unlock GYROCOPTER MOTOR for player 1 (cheat)");

            //    //Beam.Crafting.CraftingCombination craftingCombination8 = players[0].Crafter.CraftingCombinations.Combinations.FirstOrDefault((Beam.Crafting.CraftingCombination combo) => combo.Name == "GYROCOPTER ROTORS");
            //    //craftingCombination8.Unlocked = GUILayout.Toggle(craftingCombination8.Unlocked, "Unlock GYROCOPTER ROTORS for player 1 (cheat)");
            //}
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();

            if (WorldUtilities.IsStrandedWide())
            {
                ComputeGameAreaSize();
            }

            _forceReloadPlayer = true;
        }

        private static Vector3 TransformToScreenCoordinates(float x, float y)
        {
            float convertedX = x * ((mapAreaWidth - 150) / (ingameAreaSize * mapScaleX));
            float convertedY = y * ((mapAreaHeight - 150) / (ingameAreaSize * mapScaleY));

            return new Vector3(convertedX, convertedY);
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                //Debug.Log("Stranded Deep Map mod : Beam.Game.State = " + Beam.Game.State);
                bool mapNotActive = Beam.Game.State != Beam.GameState.NEW_GAME && Beam.Game.State != Beam.GameState.LOAD_GAME;
                //Debug.Log("Stranded Deep Map mod : mapNotActive = " + mapNotActive);

                if (mapNotActive)
                {
                    //Debug.Log("Stranded Deep Map mod : map not active");
                    Reset();
                    return;
                }

                if (!WorldUtilities.IsWorldLoaded())
                    return;

                Event currentevent = Event.current;
                if (currentevent.isKey)
                {
                    if (currentevent.keyCode == KeyCode.F8)
                    {
                        visible = true;
                        if (WorldUtilities.IsStrandedWide())
                        {
                            ComputeGameAreaSize();
                        }
                    }
                    if (currentevent.keyCode == KeyCode.F9)
                    {
                        visible = false;
                    }
                }

                try
                {
                    InitMainCanvas();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log("Stranded Deep Map mod : canvas instanciation failed");
                }

                if (canvas != null)
                {
                    if (!isAlternativeEndingModLoaded)
                    {
                        UnityModManager.ModEntry me = UnityModManager.FindMod("StrandedDeepAlternativeEndgameMod");
                        if (me != null
                            && me.Active
                            && me.Loaded)
                        {
                            isAlternativeEndingModLoaded = true;
                        }
                    }
                    canvas.SetActive(visible);
                    RefreshMap();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod OnUpdate failed : " + e);
            }
            finally
            {
                if (chrono.ElapsedMilliseconds >= 10)
                {
                    Debug.Log("Stranded Deep Map mod update time (ms) = " + chrono.ElapsedMilliseconds);
                }
            }
        }

        private static void Reset()
        {
            visible = false;
            // back to main menu bug fix
            currentlyLoadedSlot = -1;

            ClearMainCanvas();
        }

        private static void RefreshMap()
        {
            // if canvas is not visible, proportions are wrong
            if (visible)
            {
                RefreshMapIconsIfNeeded();

                ReloadPlayersSpritesIfNeeded();
                RefreshPlayerPosition();

                // Refresh cargo position
                try
                {
                    if (isAlternativeEndingModLoaded && showAlternativeEndingCargo)
                    {
                        RetrieveCargoObject();
                        if (cargoInstance != null)
                        {
                            InitCargoSprite();
                            if (goCargo != null)
                            {
                                imgCargo.rectTransform.localPosition = TransformToScreenCoordinates(cargoInstance.transform.position.x, cargoInstance.transform.position.z);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Map mod : cargo position drawing failed : " + e);
                }

                // refresh islands
                try
                {
                    //Debug.Log("Beam.Terrain.World.MapList.Length : " + Beam.Terrain.World.MapList.Length);
                    //Debug.Log("islandDiscovered.Count : " + islandDiscovered.Count);
                    //Debug.Log("imgIslands.Count : " + imgIslands.Count);
                    //Debug.Log("islandSilhouetteSprites.Count : " + islandSilhouetteSprites.Count);

                    // Check if island newly discovered
                    if (Beam.Terrain.World.MapList != null
                        && Beam.Terrain.World.MapList.Length >= WorldUtilities.IslandsCount
                        && islandDiscovered.Count >= WorldUtilities.IslandsCount
                        && islandInSight.Count >= WorldUtilities.IslandsCount
                        && imgIslands.Count >= WorldUtilities.IslandsCount
                        && islandSilhouetteTextures.Count >= WorldUtilities.IslandsCount)
                    {
                        Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;
#warning IslandsCount to check here ?
                        for (int islandIndex = 0; islandIndex < maps.Length; islandIndex++)
                        {
                            if (StrandedWorld.Instance.Zones.Length > islandIndex)
                            {
                                bool discovered = StrandedWorld.Instance.Zones[islandIndex].HasVisited || StrandedWorld.Instance.Zones[islandIndex].IsStartingIsland || debugMode;
                                if (islandDiscovered.ContainsKey(islandIndex))
                                {
                                    if (islandDiscovered[islandIndex] != discovered)
                                    {
                                        //Debug.Log("Stranded Deep Map mod : discovered status changed for island " + islandIndex);
                                        islandDiscovered[islandIndex] = discovered;
                                        Image imgIsland = imgIslands[islandIndex];

                                        // replace sprite
                                        Beam.Terrain.Map map = maps[islandIndex];
                                        Sprite missionSprite = null;
                                        if (map.EditorData.Id.Contains("MISSION_0"))
                                        {
                                            Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                            missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.eel.png"));
                                            missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                            Beam.Game.Destroy(imgIsland.sprite);
                                            imgIsland.sprite = missionSprite;
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                        }
                                        else if (map.EditorData.Id.Contains("MISSION_1"))
                                        {
                                            Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                            missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.shark.png"));
                                            missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                            Beam.Game.Destroy(imgIsland.sprite);
                                            imgIsland.sprite = missionSprite;
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                        }
                                        else if (map.EditorData.Id.Contains("MISSION_2"))
                                        {
                                            Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                            missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.squid.png"));
                                            missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                            Beam.Game.Destroy(imgIsland.sprite);
                                            imgIsland.sprite = missionSprite;
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                        }
                                        else if (map.EditorData.Id.Contains("MISSION_3"))
                                        {
                                            Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                            missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.endgame.png"));
                                            missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                            Beam.Game.Destroy(imgIsland.sprite);
                                            imgIsland.sprite = missionSprite;
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                        }
                                        else if (map.EditorData.Id.Contains("b314cf21-7374-4dd7-9ed9-b0c74846c293"))
                                        {
                                            Debug.Log("Stranded Deep Map mod : skipping abyss");
                                            if (WorldUtilities.IsStrandedWide())
                                            {
                                                Beam.Game.Destroy(imgIsland.sprite);
                                                imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, WorldUtilities.IslandSize + 1, WorldUtilities.IslandSize + 1), new Vector2(WorldUtilities.IslandSize / 2, WorldUtilities.IslandSize / 2));
                                            }
                                            else
                                            {
                                                Beam.Game.Destroy(imgIsland.sprite);
                                                imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, 257, 257), new Vector2(128, 128));
                                            }
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, islandSilhouetteSize);
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, islandSilhouetteSize);
                                        }
                                        else
                                        {
                                            if (WorldUtilities.IsStrandedWide())
                                            {
                                                Beam.Game.Destroy(imgIsland.sprite);
                                                imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, WorldUtilities.IslandSize + 1, WorldUtilities.IslandSize + 1), new Vector2(WorldUtilities.IslandSize / 2, WorldUtilities.IslandSize / 2));
                                            }
                                            else
                                            {
                                                Beam.Game.Destroy(imgIsland.sprite);
                                                imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, 257, 257), new Vector2(128, 128));
                                            }
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, islandSilhouetteSize);
                                            imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, islandSilhouetteSize);
                                        }
                                        _forceReloadPlayer = true;
                                    }
                                }

                                try
                                {
                                    Vector2 position = Beam.Terrain.World.GenerationZonePositons[islandIndex];
                                    bool inSight = IsNearEnough(position.x, position.y, islandDiscovered[islandIndex]);
                                    if (islandInSight[islandIndex] != inSight)
                                    {
                                        // make island visible
                                        imgIslands[islandIndex].enabled = inSight || islandDiscovered[islandIndex] || revealWorld || debugMode;
                                        islandInSight[islandIndex] = inSight;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Stranded Deep Map mod : island discovered status check failed : " + e);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Map mod : refresh islands failed : " + e);
                }
            }
        }

        private static void RefreshPlayerPosition()
        {
            // Refresh player position
            try
            {
                if (imgPlayers.Count > 0 && imgDirectionPlayers.Count > 0)
                {
                    IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
                    for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
                    {
                        Vector3 playerPosition = players[playerIndex].transform.localPosition;
                        if (imgPlayers.ContainsKey(playerIndex))
                        {
                            imgPlayers[playerIndex].rectTransform.localPosition = TransformToScreenCoordinates(playerPosition.x, playerPosition.z);
                            imgPlayers[playerIndex].enabled = showPlayers;

                            try
                            {
                                if (imgDirectionPlayers[playerIndex] != null)
                                {
                                    imgDirectionPlayers[playerIndex].rectTransform.localPosition = TransformToScreenCoordinates(playerPosition.x, playerPosition.z);
                                    imgDirectionPlayers[playerIndex].enabled = showPlayers;

                                    Vector3 playerForward = players[playerIndex].transform.forward;
                                    Vector3 trueUp = new Vector3(0f, 0f, 1f);

                                    float angleToNorth = -Vector3.SignedAngle(trueUp, playerForward, players[playerIndex].transform.up);

                                    imgDirectionPlayers[playerIndex].transform.rotation = Quaternion.Euler(0, 0, angleToNorth);
                                }
                            }
                            catch(Exception ex)
                            {
                                Debug.Log("Stranded Deep Map mod : player direction drawing failed : " + ex);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : player position drawing failed : " + e);
            }
        }

        private static void RefreshMapIconsIfNeeded()
        {
            if (Beam.Terrain.World.MapList == null || Beam.Terrain.World.MapList.Length < WorldUtilities.IslandsCount || Beam.Terrain.World.GenerationZonePositons == null)
            {
                return;
            }

            if (currentlyLoadedSlot != Options.GeneralSettings.LastSaveSlotUsed 
                || imgIslands.Count != Beam.Terrain.World.MapList.Length 
                || imgIslands.Count == Beam.Terrain.World.MapList.Length && (lastRevealWorld != revealWorld || lastRevealMissions != revealMissions || lastDebugMode != debugMode)
                || islandSilhouetteTextures.Count == 0 
                || islandSilhouetteTextures.Count > 0 && islandSilhouetteTextures[0].width != WorldUtilities.IslandSize + 1) // new with Swide configurable size
            {
                try
                {
                    if (lastRevealWorld != revealWorld
                        || lastRevealMissions != revealMissions
                        || lastDebugMode != debugMode)
                    {
                        WriteConfig();
                    }
                }
                catch { }

                if (StrandedWorld.Instance != null && StrandedWorld.Instance.Zones != null && StrandedWorld.Instance.Zones.Length >= WorldUtilities.IslandsCount)
                {
                    Debug.Log("Stranded Deep Map mod : reload zones " + (islandSilhouetteTextures.Count > 0 ? islandSilhouetteTextures[0].width.ToString() : "other") );
                    // Reload zones
                    try
                    {
                        // clear existing sprites
                        //Debug.Log("Stranded Deep Map mod : clear start");
                        ClearIslandImages();
                        //Debug.Log("Stranded Deep Map mod : clear end");
                        for (int i = 0; i < islandDiscovered.Keys.Count; i++)
                        {
                            int islandKey = islandDiscovered.Keys.ElementAt(i);
                            islandDiscovered[islandKey] = false;
                        }
                        for (int i = 0; i < islandInSight.Keys.Count; i++)
                        {
                            int islandKey = islandInSight.Keys.ElementAt(i);
                            islandInSight[islandKey] = false;
                        }


                        //Debug.Log("Stranded Deep Map mod : rebuild sprites start");

                        Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;
                        Vector2[] positions = Beam.Terrain.World.GenerationZonePositons;

                        Texture2D unknownIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                        unknownIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.unknown.png"));
                        Texture2D unknownIcon2 = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                        unknownIcon2.LoadImage(ExtractResource("StrandedDeepMapMod.icons.unknown2.png"));
                        Texture2D unknownIcon3 = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                        unknownIcon3.LoadImage(ExtractResource("StrandedDeepMapMod.icons.unknown3.png"));
                        Sprite islandSprite = null;

                        Texture2D unknownMissionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                        unknownMissionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.unknown_mission.png"));
                        Sprite missionSprite = null;

                        //Debug.Log("Stranded Deep Map mod : islands loop in");
                        System.Random iconRandom = new System.Random();
                        for (int islandIndex = 0; islandIndex < maps.Length; islandIndex++)
                        {
                            if (!islandDiscovered.ContainsKey(islandIndex))
                            {
                                islandDiscovered.Add(islandIndex, false);
                            }
                            if (!islandInSight.ContainsKey(islandIndex))
                            {
                                islandInSight.Add(islandIndex, false);
                            }
                            Vector2 position = new Vector2();
                            Beam.Terrain.Map map = maps[islandIndex];
                            if (positions.Length >= islandIndex)
                            {
                                position = positions[islandIndex];
                            }
                            // Compute all silhouettes
                            if (!islandSilhouetteTextures.ContainsKey(islandIndex))
                            {
                                //Debug.Log("Stranded Deep Map mod : generate silhouette for island " + islandIndex);
                                Texture2D silhouette = GenerateThumb(map, map.EditorData.Id.Contains("MISSION"));
                                if (silhouette != null)
                                {
                                    islandSilhouetteTextures.Add(islandIndex, silhouette);
                                }
                            }

                            GameObject islandGO = new GameObject("Island" + islandIndex + "_Sprite");
                            goIslands.Add(islandIndex, islandGO);
                            islandGO.transform.SetParent(canvas.transform);

                            Image imgIsland = islandGO.AddComponent<Image>();
                            imgIsland.enabled = false;
                            imgIslands.Add(islandIndex, imgIsland);

                            //Debug.Log("Stranded Deep Map mod : sprite selection for island : " + islandIndex);
                            //Debug.Log("Stranded Deep Map mod : map.EditorData.Id : " + map.EditorData.Id);

                            if (map.EditorData.Id.Contains("MISSION"))
                            {
                                if (debugMode)
                                {
                                    if (map.EditorData.Id.Contains("MISSION_0"))
                                    {
                                        Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                        missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.eel.png"));
                                        missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        imgIsland.sprite = missionSprite;
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                    }
                                    else if (map.EditorData.Id.Contains("MISSION_1"))
                                    {
                                        Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                        missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.shark.png"));
                                        missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        imgIsland.sprite = missionSprite;
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                    }
                                    else if (map.EditorData.Id.Contains("MISSION_2"))
                                    {
                                        Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                        missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.squid.png"));
                                        missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        imgIsland.sprite = missionSprite;
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                    }
                                    else if (map.EditorData.Id.Contains("MISSION_3"))
                                    {
                                        Texture2D missionIcon = new Texture2D(350, 350, TextureFormat.ARGB32, false);
                                        missionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.endgame.png"));
                                        missionSprite = Sprite.Create(missionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        imgIsland.sprite = missionSprite;
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionIconSize);
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionIconSize);
                                    }
                                    else
                                    {
                                        if (WorldUtilities.IsStrandedWide())
                                        {
                                            imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, WorldUtilities.IslandSize + 1, WorldUtilities.IslandSize + 1), new Vector2(WorldUtilities.IslandSize / 2, WorldUtilities.IslandSize / 2));
                                        }
                                        else
                                        {
                                            imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, 257, 257), new Vector2(128, 128));
                                        }
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, islandSilhouetteSize);
                                        imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, islandSilhouetteSize);
                                    }
                                    imgIsland.enabled = true;
                                }
                                else if (revealWorld)
                                {
                                    if (revealMissions)
                                    {
                                        missionSprite = Sprite.Create(unknownMissionIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        imgIsland.sprite = missionSprite;
                                    }
                                    else
                                    {
                                        islandSprite = Sprite.Create(unknownIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        imgIsland.sprite = islandSprite;
                                    }
                                    imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
                                    imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
                                    imgIsland.enabled = true;
                                }
                                else
                                {
                                    islandSprite = Sprite.Create(unknownIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                    imgIsland.sprite = islandSprite;
                                    imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
                                    imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
                                }
                            }
                            else if (debugMode)
                            {
                                if (WorldUtilities.IsStrandedWide())
                                {
                                    imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, WorldUtilities.IslandSize + 1, WorldUtilities.IslandSize + 1), new Vector2(WorldUtilities.IslandSize / 2, WorldUtilities.IslandSize / 2));
                                }
                                else
                                {
                                    imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, 257, 257), new Vector2(128, 128));
                                }
                                imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, islandSilhouetteSize);
                                imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, islandSilhouetteSize);
                                imgIsland.enabled = true;
                            }
                            else if (map.EditorData.Id.Contains("b314cf21-7374-4dd7-9ed9-b0c74846c293"))
                            {
                                // Empty silhouette for abysses
                                if (WorldUtilities.IsStrandedWide())
                                {
                                    imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, WorldUtilities.IslandSize + 1, WorldUtilities.IslandSize + 1), new Vector2(WorldUtilities.IslandSize / 2, WorldUtilities.IslandSize / 2));
                                }
                                else
                                {
                                    imgIsland.sprite = Sprite.Create(islandSilhouetteTextures[islandIndex], new Rect(0, 0, 257, 257), new Vector2(128, 128));
                                }
                                imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, islandSilhouetteSize);
                                imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, islandSilhouetteSize);
                                imgIsland.enabled = true;
                            }
                            else
                            {
                                int icon = iconRandom.Next(0, 3);
                                switch (icon)
                                {
                                    case 0:
                                        islandSprite = Sprite.Create(unknownIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        break;
                                    case 1:
                                        islandSprite = Sprite.Create(unknownIcon2, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        break;
                                    case 2:
                                        islandSprite = Sprite.Create(unknownIcon3, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        break;
                                    default:
                                        islandSprite = Sprite.Create(unknownIcon, new Rect(0, 0, 350, 350), new Vector2(170, 170));
                                        break;
                                }

                                imgIsland.sprite = islandSprite;
                                imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
                                imgIsland.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

                                if (revealWorld || IsNearEnough(position.x, position.y, islandDiscovered[islandIndex]))
                                {
                                    islandInSight[islandIndex] = true;
                                    imgIsland.enabled = true;
                                }
                                else
                                {
                                    imgIsland.enabled = false;
                                }
                            }

                            // convert world coordinate into map coordinates
                            imgIsland.rectTransform.localPosition = TransformToScreenCoordinates(position.x, position.y);
                        }
                        //Debug.Log("Stranded Deep Map mod : islands loop out");

                        //Debug.Log("Stranded Deep Map mod : rebuild sprites end");

                        currentlyLoadedSlot = Options.GeneralSettings.LastSaveSlotUsed;
                        lastRevealWorld = revealWorld;
                        lastRevealMissions = revealMissions;
                        lastDebugMode = debugMode;
                        _forceReloadPlayer = true;
                        Debug.Log("Stranded Deep Map mod : reload zones completed");
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Stranded Deep Map mod : islands drawing failed : " + e);
                    }
                }
                else
                {
                    //Debug.Log("Stranded Deep Map mod : some objects not loaded");
                }
            }
        }

        private static void ReloadPlayersSpritesIfNeeded()
        {
            try
            {
                IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;

                if (players != null
                    && players.Count > 0
                    && (imgPlayers.Count == 0 || imgPlayers.Count != players.Count)
                    || _forceReloadPlayer)
                {
                    Debug.Log("Stranded Deep Map mod : reload players");
                    _forceReloadPlayer = false;
                    // clear existing sprites
                    ClearPlayerImages(_forceReloadPlayer);

                    if (showPlayers)
                    {
                        // init player sprite
                        Texture2D playerIcon = new Texture2D(200, 200, TextureFormat.ARGB32, false);
                        playerIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.player.png"));

                        Sprite playerSprite = Sprite.Create(playerIcon, new Rect(0, 0, 200, 200), new Vector2(100, 100));

                        // init player direction
                        Texture2D playerDirectionIcon = new Texture2D(250, 250, TextureFormat.ARGB32, false);
                        playerDirectionIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.direction.png"));

                        Sprite playerDirectionSprite = Sprite.Create(playerDirectionIcon, new Rect(0, 0, 250, 250), new Vector2(125, 125));

                        for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
                        {
                            //Debug.Log("Stranded Deep Map mod : adding player sprite");

                            GameObject playerGO = new GameObject("Player" + playerIndex + "_Sprite");
                            goPlayers.Add(playerIndex, playerGO);
                            playerGO.transform.SetParent(canvas.transform);

                            Image imgPlayer = playerGO.AddComponent<Image>();
                            // keep track of the images
                            imgPlayers.Add(playerIndex, imgPlayer);
                            imgPlayer.sprite = playerSprite;
                            imgPlayer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, playerIconSize);
                            imgPlayer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerIconSize);
                            imgPlayer.rectTransform.localPosition = new Vector3(0, 0);

                            //Debug.Log("Stranded Deep Map mod : spawned a player icon " + playerGO.name);

                            GameObject playerDirectionGO = new GameObject("PlayerDirection" + playerIndex + "_Sprite");
                            goDirectionPlayers.Add(playerIndex, playerDirectionGO);
                            playerDirectionGO.transform.SetParent(canvas.transform);

                            Image imgPlayerDirection = playerDirectionGO.AddComponent<Image>();
                            // keep track of the images
                            imgDirectionPlayers.Add(playerIndex, imgPlayerDirection);
                            imgPlayerDirection.sprite = playerDirectionSprite;
                            imgPlayerDirection.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, playerIconSize);
                            imgPlayerDirection.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerIconSize);
                            imgPlayerDirection.rectTransform.localPosition = new Vector3(0, 0);

                            //Debug.Log("Stranded Deep Map mod : spawned a player direction icon " + playerDirectionGO.name);
                        }
                    }
                    Debug.Log("Stranded Deep Map mod : reload players completed");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : player sprites initialization failed : " + e);
            }
        }

        private static void RetrieveCargoObject()
        {
            if (cargoInstance == null)
            {
                foreach (SaveablePrefab sp in Game.FindObjectsOfType<SaveablePrefab>())
                {
                    try
                    {
                        if (sp != null)
                        {
                            //if (new MiniGuid(new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")).ToString().CompareTo(sp.ReferenceId.ToString()) == 0)
                            //{
                            //    Debug.Log("Stranded Deep Map Mod : cargo reference found " + sp.name);
                            //}
                            if (sp.name != null
                                && sp.name.CompareTo(ENDGAME_CARGO_NAME) == 0)
                            {
                                Debug.Log("Stranded Deep Map Mod : retrieved the endgame cargo");
                                cargoInstance = sp;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Stranded Deep Map Mod : retrieving the endgame cargo failed : " + e);
                    }
                }
            }
            if (cargoInstance == null)
            {
                return;
            }
        }

        private static void InitCargoSprite()
        {
            if (goCargo != null)
                return;

            Texture2D playerIcon = new Texture2D(200, 200, TextureFormat.ARGB32, false);
            playerIcon.LoadImage(ExtractResource("StrandedDeepMapMod.icons.cargo.png"));

            Sprite cargoSprite = Sprite.Create(playerIcon, new Rect(0, 0, 200, 200), new Vector2(100, 100));

            goCargo = new GameObject("Cargo_Sprite");
            goCargo.transform.SetParent(canvas.transform);

            imgCargo = goCargo.AddComponent<Image>();
            // keep track of the images
            imgCargo.sprite = cargoSprite;
            imgCargo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, playerIconSize);
            imgCargo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerIconSize);
            imgCargo.rectTransform.localPosition = new Vector3(0, 0);
        }

        private static bool IsNearEnough(float islandx, float islandy, bool discovered)
        {
            IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                Vector3 playerPosition = players[playerIndex].transform.localPosition;
                // pythagor on island position
                float distance = Mathf.Sqrt(Mathf.Pow(islandx - playerPosition.x, 2) + Mathf.Pow(islandy - playerPosition.z, 2));
                if (distance <= viewDistance || discovered)
                    return true;
            }

            return false;
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

        public static Texture2D GenerateThumb(Beam.Terrain.Map map, bool isMission)
        {
            try
            {
                System.Random alphaRandom = new System.Random();
                //Debug.Log("Stranded Deep Map mod : read island silhouette");
                Texture2D islandSilhouette = null;

                if (WorldUtilities.IsStrandedWide())
                {
                    islandSilhouette = new Texture2D(WorldUtilities.IslandSize + 1, WorldUtilities.IslandSize + 1, TextureFormat.ARGB32, false);
                }
                else
                    islandSilhouette = new Texture2D(257, 257, TextureFormat.ARGB32, false);

                float[,] heightmap = map.HeightmapData;
                for (int y = 0; y < map.HeightmapData.GetLength(0); y++)
                {
                    for (int x = 0; x < map.HeightmapData.GetLength(1); x++)
                    {
                        //Debug.Log("Gray value "+x+"/"+y+" : " + heightmap[y, x]);
                        if (heightmap[y, x] >= 0.667f)
                        {
                            int alpha = alphaRandom.Next(230, 255);
                            Color c = new Color(0.2f, 0.05f, 0, (float)alpha/255f);
                            if (isMission)
                            {
                                c = new Color(1, 0, 0, (float)alpha / 255f);
                            }
                            islandSilhouette.SetPixel(x, y, c);
                        }
                        else
                        {
                            islandSilhouette.SetPixel(x, y, Color.clear);
                        }
                    }
                }
                islandSilhouette.Apply();
                return islandSilhouette;
            }
            catch(Exception e)
            {
                Debug.Log("Stranded Deep Map mod : silhouette reading error : " + e);
            }
            return null;
        }

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, "StrandedDeepMapperMod.config");
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach(string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            if (tokens[0].Contains("revealWorld"))
                            {
                                revealWorld = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("revealMissions"))
                            {
                                revealMissions = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("showPlayers"))
                            {
                                showPlayers = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("showAlternativeEndingCargo"))
                            {
                                showAlternativeEndingCargo = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("viewDistance"))
                            {
                                viewDistance = float.Parse(tokens[1]);
                                if (viewDistance < 600)
                                    viewDistance = 600;
                                if (viewDistance > 1500)
                                    viewDistance = 1500;
                            }
                            else if (tokens[0].Contains("playerIconSize"))
                            {
                                playerIconSize = float.Parse(tokens[1]);
                                if (playerIconSize < 10)
                                    playerIconSize = 10;
                                if (playerIconSize > 100)
                                    playerIconSize = 100;
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
                string configFilePath = System.IO.Path.Combine(dataDirectory, "StrandedDeepMapperMod.config");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("viewDistance=" + viewDistance + ";");
                sb.AppendLine("revealWorld=" + revealWorld + ";");
                sb.AppendLine("revealMissions=" + revealMissions + ";");
                sb.AppendLine("playerIconSize=" + playerIconSize + ";");
                sb.AppendLine("showPlayers=" + showPlayers + ";");
                sb.AppendLine("showAlternativeEndingCargo=" + showAlternativeEndingCargo + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
