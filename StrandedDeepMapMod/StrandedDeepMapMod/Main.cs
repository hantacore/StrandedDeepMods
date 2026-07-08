using Beam;
using Funlabs;
using StrandedDeepModsUtils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepMapMod
{
    static class Main
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const float MAP_CANVAS_DEFAULT_WIDTH  = 1024f;
        private const float MAP_CANVAS_DEFAULT_HEIGHT = 768f;
        private const float MAP_AREA_HEIGHT           = 650f;
        private const float MAP_AREA_WIDTH            = 650f;
        private const float ISLAND_SILHOUETTE_SIZE    = 40f;
        private const float ICON_SIZE                 = 40f;
        private const float MISSION_ICON_SIZE         = 60f;
        private const string ENDGAME_CARGO_NAME       = "EVERGREEN";
        private const string INFO_JSON_URL            = "https://raw.githubusercontent.com/hantacore/StrandedDeepMods/main/StrandedDeepMapMod/StrandedDeepMapMod/Info.json";
        // Abyss GUID — skip silhouette for this island type
        private const string ABYSS_GUID              = "b314cf21-7374-4dd7-9ed9-b0c74846c293";

        // ─── Config (persisted) ───────────────────────────────────────────────────

        private static float viewDistance              = 1000f;
        private static float playerIconSize            = 50f;
        private static bool  showPlayers               = true;
        private static bool  revealWorld               = false;
        private static bool  revealMissions            = false;
        private static bool  showAlternativeEndingCargo = false;

        // ─── Runtime state ────────────────────────────────────────────────────────

        private static float ingameAreaSize            = 3000f;
        private static float screenRatioConversion     = 1f;
        private static float mapScaleX                 = 1f;
        private static float mapScaleY                 = 1f;
        private static bool  debugMode                 = false;

        public static bool visible;

        private static int  currentlyLoadedSlot        = -1;
        private static bool isAlternativeEndingModLoaded = false;
        private static bool _forceReloadPlayer          = false;

        // Tracks last values so we know when to rebuild
        private static bool lastRevealWorld    = false;
        private static bool lastRevealMissions = false;
        private static bool lastDebugMode      = false;

        // FIX: cargo search throttled — FindObjectsOfType every frame was very expensive
        private static float _cargoSearchTimer         = 0f;
        private const  float CARGO_SEARCH_INTERVAL     = 5f;

        // ─── UI objects ───────────────────────────────────────────────────────────

        private static GameObject canvas;
        private static Image      imgBackground;

        // FIX: switched from Dictionary<int,X> iterated by index to List<T> —
        //      simpler, faster iteration, no accidental O(n²) via ElementAt()
        private static readonly List<GameObject> goPlayers          = new List<GameObject>();
        private static readonly List<GameObject> goDirectionPlayers = new List<GameObject>();
        private static readonly List<Image>      imgPlayers         = new List<Image>();
        private static readonly List<Image>      imgDirectionPlayers = new List<Image>();

        private static readonly List<GameObject> goIslands               = new List<GameObject>();
        private static readonly List<Image>      imgIslands              = new List<Image>();
        private static readonly List<Texture2D>  islandSilhouetteTextures = new List<Texture2D>();
        private static readonly List<bool>       islandDiscovered         = new List<bool>();
        private static readonly List<bool>       islandInSight            = new List<bool>();

        // FIX: shared icon textures loaded once, not re-created on every rebuild
        private static Texture2D _texUnknown        = null;
        private static Texture2D _texUnknown2       = null;
        private static Texture2D _texUnknown3       = null;
        private static Texture2D _texUnknownMission = null;
        private static Texture2D _texMission0       = null;
        private static Texture2D _texMission1       = null;
        private static Texture2D _texMission2       = null;
        private static Texture2D _texMission3       = null;
        private static Texture2D _texPlayer         = null;
        private static Texture2D _texDirection      = null;
        private static Texture2D _texBackground     = null;
        private static Texture2D _texCompass        = null;
        private static Texture2D _texCargo          = null;

        private static SaveablePrefab cargoInstance = null;
        private static Image          imgCargo      = null;
        private static GameObject     goCargo       = null;

        internal static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();

        private static System.Random _iconRandom = new System.Random();

        // ─── Mod entry points ─────────────────────────────────────────────────────

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                chrono.Start();

                modEntry.OnUpdate  = OnUpdate;
                modEntry.OnGUI     = OnGUI;
                modEntry.OnHideGUI = OnHideGUI;

                try
                {
                    VersionChecker.CheckVersion(modEntry, INFO_JSON_URL);
                    ReadConfig();
                }
                catch { }

                if (WorldUtilities.IsStrandedWide())
                    ComputeGameAreaSize();

                PreloadSharedTextures();
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
                    Debug.Log("Stranded Deep Map mod load time (ms) = " + chrono.ElapsedMilliseconds);
            }
            return false;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                chrono.Reset();
                chrono.Start();

                bool mapNotActive = Beam.Game.State != Beam.GameState.NEW_GAME
                                 && Beam.Game.State != Beam.GameState.LOAD_GAME;
                if (mapNotActive)
                {
                    Reset();
                    return;
                }

                if (!WorldUtilities.IsWorldLoaded())
                    return;

                // FIX: Input.GetKeyDown instead of Event.current (which is OnGUI-only)
                if (Input.GetKeyDown(KeyCode.F8))
                {
                    visible = true;
                    if (WorldUtilities.IsStrandedWide())
                        ComputeGameAreaSize();
                }
                if (Input.GetKeyDown(KeyCode.F9))
                {
                    visible = false;
                }

                try
                {
                    InitMainCanvas();
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Map mod : canvas instantiation failed : " + e);
                }

                if (canvas != null)
                {
                    if (!isAlternativeEndingModLoaded)
                    {
                        UnityModManager.ModEntry me = UnityModManager.FindMod("StrandedDeepAlternativeEndgameMod");
                        if (me != null && me.Active && me.Loaded)
                            isAlternativeEndingModLoaded = true;
                    }
                    canvas.SetActive(visible);
                    RefreshMap(dt);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod OnUpdate failed : " + e);
            }
            finally
            {
                if (chrono.ElapsedMilliseconds >= 10)
                    Debug.Log("Stranded Deep Map mod update time (ms) = " + chrono.ElapsedMilliseconds);
            }
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Map mod by Hantacore");
            GUILayout.Label("How to use : F8 = show map / F9 = hide map");
            GUILayout.Label("View distance");
            viewDistance  = GUILayout.HorizontalSlider(viewDistance, 600f, 1500f);
            showPlayers   = GUILayout.Toggle(showPlayers,   "Show players positions");
            revealWorld   = GUILayout.Toggle(revealWorld,   "Reveal world (cheat)");
            revealMissions = GUILayout.Toggle(revealMissions, "Reveal missions (cheat)");
            GUILayout.Label("Stranded Wide World mode : " + WorldUtilities.IsStrandedWide() + " (restart if changed)");
            showAlternativeEndingCargo = GUILayout.Toggle(showAlternativeEndingCargo, "Show endgame cargo (cheat - only use with Stranded Deep Alternative Ending)");
            GUILayout.Label("Player icon size");
            playerIconSize = GUILayout.HorizontalSlider(playerIconSize, 10f, 100f);
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            WriteConfig();
            if (WorldUtilities.IsStrandedWide())
                ComputeGameAreaSize();
            _forceReloadPlayer = true;
        }

        // ─── Initialisation ───────────────────────────────────────────────────────

        private static void ComputeGameAreaSize()
        {
            float zoneSize    = WorldUtilities.ZoneSize;
            float zoneSpacing = WorldUtilities.ZoneSpacing;
            float diameter    = 8f;
            ingameAreaSize = (zoneSize * zoneSpacing * (diameter + 1))
                             / (WorldUtilities.ZoneSize == 512 ? 1 : 2);
        }

        // FIX: all shared textures loaded once here instead of inside loops/rebuilds
        private static void PreloadSharedTextures()
        {
            _texUnknown        = LoadTexture("StrandedDeepMapMod.icons.unknown.png",        350, 350);
            _texUnknown2       = LoadTexture("StrandedDeepMapMod.icons.unknown2.png",       350, 350);
            _texUnknown3       = LoadTexture("StrandedDeepMapMod.icons.unknown3.png",       350, 350);
            _texUnknownMission = LoadTexture("StrandedDeepMapMod.icons.unknown_mission.png",350, 350);
            _texMission0       = LoadTexture("StrandedDeepMapMod.icons.eel.png",            350, 350);
            _texMission1       = LoadTexture("StrandedDeepMapMod.icons.shark.png",          350, 350);
            _texMission2       = LoadTexture("StrandedDeepMapMod.icons.squid.png",          350, 350);
            _texMission3       = LoadTexture("StrandedDeepMapMod.icons.endgame.png",        350, 350);
            _texPlayer         = LoadTexture("StrandedDeepMapMod.icons.player.png",         200, 200);
            _texDirection      = LoadTexture("StrandedDeepMapMod.icons.direction.png",      250, 250);
            _texBackground     = LoadTexture("StrandedDeepMapMod.icons.background.png",    2000,2000);
            _texCompass        = LoadTexture("StrandedDeepMapMod.icons.compass.png",        350, 350);
            _texCargo          = LoadTexture("StrandedDeepMapMod.icons.cargo.png",          200, 200);
        }

        private static void InitMainCanvas()
        {
            if (canvas != null)
                return;

            Debug.Log("Stranded Deep Map mod: init main canvas");

            float defaultScreenRatio  = MAP_CANVAS_DEFAULT_WIDTH / MAP_CANVAS_DEFAULT_HEIGHT;
            float currentScreenRatio  = (float)Screen.width / Screen.height;
            screenRatioConversion     = currentScreenRatio / defaultScreenRatio;
            mapScaleX = (float)Screen.width  / (MAP_CANVAS_DEFAULT_WIDTH  * screenRatioConversion);
            mapScaleY = (float)Screen.height /  MAP_CANVAS_DEFAULT_HEIGHT;

            canvas = CreateCanvas();

            // Background
            GameObject bgGO = CreateUIObject("Background_Sprite", canvas.transform);
            imgBackground = bgGO.AddComponent<Image>();
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   MAP_AREA_HEIGHT);
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MAP_AREA_WIDTH);
            if (_texBackground != null)
                imgBackground.sprite = MakeSprite(_texBackground, 2000, 2000, 1000, 1000);
            bgGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Compass
            GameObject compassGO = CreateUIObject("Compass_Sprite", canvas.transform);
            Image imgCompass = compassGO.AddComponent<Image>();
            if (_texCompass != null)
                imgCompass.sprite = MakeSprite(_texCompass, 350, 350, 170, 170);
            imgCompass.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   MISSION_ICON_SIZE);
            imgCompass.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MISSION_ICON_SIZE);
            imgCompass.rectTransform.localPosition = WorldUtilities.IsStrandedWide()
                ? TransformToScreenCoordinates((ingameAreaSize / 2) + 300, (ingameAreaSize / 2) + 200)
                : TransformToScreenCoordinates(2300, 2200);

            canvas.SetActive(false);
        }

        private static void Reset()
        {
            visible = false;
            currentlyLoadedSlot = -1;
            ClearMainCanvas();
        }

        private static void ClearMainCanvas()
        {
            try
            {
                if (canvas == null)
                    return;

                visible = false;
                ClearIslandImages();
                ClearPlayerImages(true);
                islandDiscovered.Clear();
                islandInSight.Clear();

                Beam.Game.Destroy(canvas);
                canvas = null;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod: clear all failed : " + e);
            }
        }

        private static void ClearIslandImages()
        {
            // FIX: foreach on List instead of index-based Dictionary iteration
            foreach (Image img in imgIslands)
            {
                if (img != null && img.sprite != null)
                    Beam.Game.Destroy(img.sprite);
                Beam.Game.Destroy(img);
            }
            imgIslands.Clear();

            foreach (GameObject go in goIslands)
                Beam.Game.Destroy(go);
            goIslands.Clear();

            // FIX: silhouette textures (island-specific, generated per-game) are destroyed here;
            //      shared icon textures (_texUnknown etc.) are NOT destroyed — they persist for the session
            foreach (Texture2D tex in islandSilhouetteTextures)
                Beam.Game.Destroy(tex);
            islandSilhouetteTextures.Clear();
        }

        private static void ClearPlayerImages(bool forceReloadPlayer)
        {
            Debug.Log("Stranded Deep Map mod : clearing player sprites " + forceReloadPlayer);
            _forceReloadPlayer = forceReloadPlayer;

            foreach (Image img in imgPlayers)      Beam.Game.Destroy(img);
            foreach (Image img in imgDirectionPlayers) Beam.Game.Destroy(img);
            foreach (GameObject go in goPlayers)   Beam.Game.Destroy(go);
            foreach (GameObject go in goDirectionPlayers) Beam.Game.Destroy(go);

            imgPlayers.Clear();
            imgDirectionPlayers.Clear();
            goPlayers.Clear();
            goDirectionPlayers.Clear();
        }

        // ─── Per-frame refresh ────────────────────────────────────────────────────

        private static void RefreshMap(float dt)
        {
            if (!visible)
                return;

            RefreshMapIconsIfNeeded();
            ReloadPlayersSpritesIfNeeded();
            RefreshPlayerPositions();

            // Cargo
            try
            {
                if (isAlternativeEndingModLoaded && showAlternativeEndingCargo)
                {
                    // FIX: throttled search — FindObjectsOfType every frame was very expensive
                    if (cargoInstance == null)
                    {
                        _cargoSearchTimer += dt;
                        if (_cargoSearchTimer >= CARGO_SEARCH_INTERVAL)
                        {
                            _cargoSearchTimer = 0f;
                            RetrieveCargoObject();
                        }
                    }
                    if (cargoInstance != null)
                    {
                        InitCargoSprite();
                        if (goCargo != null)
                            imgCargo.rectTransform.localPosition = TransformToScreenCoordinates(
                                cargoInstance.transform.position.x,
                                cargoInstance.transform.position.z);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : cargo position drawing failed : " + e);
            }

            // Islands
            try
            {
                if (Beam.Terrain.World.MapList != null
                    && Beam.Terrain.World.MapList.Length >= WorldUtilities.IslandsCount
                    && islandDiscovered.Count >= WorldUtilities.IslandsCount
                    && islandInSight.Count     >= WorldUtilities.IslandsCount
                    && imgIslands.Count        >= WorldUtilities.IslandsCount
                    && islandSilhouetteTextures.Count >= WorldUtilities.IslandsCount)
                {
                    Beam.Terrain.Map[] maps = Beam.Terrain.World.MapList;
                    for (int idx = 0; idx < maps.Length; idx++)
                    {
                        if (StrandedWorld.Instance.Zones.Length <= idx)
                            continue;

                        bool discovered = StrandedWorld.Instance.Zones[idx].HasVisited
                                       || StrandedWorld.Instance.Zones[idx].IsStartingIsland
                                       || debugMode;

                        // Discovery state changed → swap sprite
                        if (islandDiscovered[idx] != discovered)
                        {
                            islandDiscovered[idx] = discovered;
                            ApplyIslandSprite(imgIslands[idx], maps[idx], idx, discovered);
                            _forceReloadPlayer = true;
                        }

                        // Visibility check
                        try
                        {
                            Vector2 pos    = Beam.Terrain.World.GenerationZonePositons[idx];
                            bool    inSight = IsNearEnough(pos.x, pos.y, islandDiscovered[idx]);
                            if (islandInSight[idx] != inSight)
                            {
                                imgIslands[idx].enabled = inSight || islandDiscovered[idx] || revealWorld || debugMode;
                                islandInSight[idx] = inSight;
                            }
                            imgIslands[idx].rectTransform.localPosition = TransformToScreenCoordinates(pos.x, pos.y);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Stranded Deep Map mod : island visibility check failed : " + e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : refresh islands failed : " + e);
            }
        }

        private static void RefreshMapIconsIfNeeded()
        {
            if (Beam.Terrain.World.MapList == null
                || Beam.Terrain.World.MapList.Length < WorldUtilities.IslandsCount
                || Beam.Terrain.World.GenerationZonePositons == null)
                return;

            bool needsRebuild =
                currentlyLoadedSlot != Options.GeneralSettings.LastSaveSlotUsed
                || imgIslands.Count != Beam.Terrain.World.MapList.Length
                || (imgIslands.Count == Beam.Terrain.World.MapList.Length
                    && (lastRevealWorld != revealWorld || lastRevealMissions != revealMissions || lastDebugMode != debugMode))
                || islandSilhouetteTextures.Count == 0
                || (islandSilhouetteTextures.Count > 0 && islandSilhouetteTextures[0] != null
                    && islandSilhouetteTextures[0].width != WorldUtilities.IslandSize + 1);

            if (!needsRebuild)
                return;

            try
            {
                if (lastRevealWorld != revealWorld || lastRevealMissions != revealMissions || lastDebugMode != debugMode)
                    WriteConfig();
            }
            catch { }

            if (StrandedWorld.Instance == null
                || StrandedWorld.Instance.Zones == null
                || StrandedWorld.Instance.Zones.Length < WorldUtilities.IslandsCount)
                return;

            Debug.Log("Stranded Deep Map mod : reload zones");
            try
            {
                ClearIslandImages();

                // FIX: simple List.Clear() + repopulate — no O(n²) ElementAt() loop
                islandDiscovered.Clear();
                islandInSight.Clear();

                Beam.Terrain.Map[] maps      = Beam.Terrain.World.MapList;
                Vector2[]          positions = Beam.Terrain.World.GenerationZonePositons;

                for (int idx = 0; idx < maps.Length; idx++)
                {
                    islandDiscovered.Add(false);
                    islandInSight.Add(false);

                    Vector2 position = positions.Length > idx ? positions[idx] : Vector2.zero;
                    Beam.Terrain.Map map = maps[idx];

                    // Generate silhouette once per island
                    if (idx >= islandSilhouetteTextures.Count)
                    {
                        Texture2D silhouette = GenerateThumb(map, map.EditorData.Id.Contains("MISSION"));
                        islandSilhouetteTextures.Add(silhouette);
                    }

                    GameObject islandGO = CreateUIObject("Island" + idx + "_Sprite", canvas.transform);
                    goIslands.Add(islandGO);

                    Image imgIsland = islandGO.AddComponent<Image>();
                    imgIsland.enabled = false;
                    imgIslands.Add(imgIsland);

                    bool discovered = StrandedWorld.Instance.Zones.Length > idx
                        && (StrandedWorld.Instance.Zones[idx].HasVisited
                            || StrandedWorld.Instance.Zones[idx].IsStartingIsland
                            || debugMode);

                    ApplyIslandSprite(imgIsland, map, idx, discovered);
                    islandDiscovered[idx] = discovered;

                    bool inSight = revealWorld || IsNearEnough(position.x, position.y, discovered);
                    imgIsland.enabled = inSight || discovered || revealWorld || debugMode;
                    islandInSight[idx] = inSight;

                    imgIsland.rectTransform.localPosition = TransformToScreenCoordinates(position.x, position.y);
                }

                currentlyLoadedSlot  = Options.GeneralSettings.LastSaveSlotUsed;
                lastRevealWorld      = revealWorld;
                lastRevealMissions   = revealMissions;
                lastDebugMode        = debugMode;
                _forceReloadPlayer   = true;
                Debug.Log("Stranded Deep Map mod : reload zones completed");
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : islands drawing failed : " + e);
            }
        }

        // FIX: ApplyIslandSprite centralise toute la logique de sélection de sprite —
        //      élimine les blocs if/else dupliqués dans RefreshMap et RefreshMapIconsIfNeeded
        private static void ApplyIslandSprite(Image imgIsland, Beam.Terrain.Map map, int idx, bool discovered)
        {
            int islandSize = WorldUtilities.IslandSize;
            bool isWide    = WorldUtilities.IsStrandedWide();

            // FIX: l'Abyss est un placeholder pour l'océan — ne jamais l'afficher,
            //      qu'il soit découvert ou non.
            //      imgIsland.enabled = false ne suffit pas — Unity affiche un carré blanc
            //      car le composant Image a une couleur blanche par défaut sans sprite.
            //      On désactive le GameObject entier pour qu'il soit complètement invisible.
            if (map.EditorData.Id.Contains(ABYSS_GUID))
            {
                imgIsland.gameObject.SetActive(false);
                return;
            }

            if (discovered || debugMode)
            {
                if (map.EditorData.Id.Contains("MISSION_0") && _texMission0 != null)
                {
                    imgIsland.sprite = MakeSprite(_texMission0, 350, 350, 170, 170);
                    SetIconSize(imgIsland, MISSION_ICON_SIZE);
                }
                else if (map.EditorData.Id.Contains("MISSION_1") && _texMission1 != null)
                {
                    imgIsland.sprite = MakeSprite(_texMission1, 350, 350, 170, 170);
                    SetIconSize(imgIsland, MISSION_ICON_SIZE);
                }
                else if (map.EditorData.Id.Contains("MISSION_2") && _texMission2 != null)
                {
                    imgIsland.sprite = MakeSprite(_texMission2, 350, 350, 170, 170);
                    SetIconSize(imgIsland, MISSION_ICON_SIZE);
                }
                else if (map.EditorData.Id.Contains("MISSION_3") && _texMission3 != null)
                {
                    imgIsland.sprite = MakeSprite(_texMission3, 350, 350, 170, 170);
                    SetIconSize(imgIsland, MISSION_ICON_SIZE);
                }
                else if (idx < islandSilhouetteTextures.Count && islandSilhouetteTextures[idx] != null)
                {
                    float sz = isWide ? islandSize + 1 : 257;
                    imgIsland.sprite = MakeSprite(islandSilhouetteTextures[idx], (int)sz, (int)sz, sz / 2, sz / 2);
                    SetIconSize(imgIsland, ISLAND_SILHOUETTE_SIZE);
                }
            }
            else if (revealWorld)
            {
                // FIX: _texUnknownMission était appliqué à toutes les îles quand revealMissions = true —
                //      il faut vérifier que l'île EST une mission avant d'utiliser l'icône mission
                bool isMission = map.EditorData.Id.Contains("MISSION");
                Texture2D tex = (isMission && revealMissions) ? _texUnknownMission : _texUnknown;
                if (tex != null)
                {
                    imgIsland.sprite = MakeSprite(tex, 350, 350, 170, 170);
                    SetIconSize(imgIsland, ICON_SIZE);
                }
            }
            else
            {
                // FIX: random icon chosen from pre-loaded textures, no allocation
                Texture2D[] choices = { _texUnknown, _texUnknown2, _texUnknown3 };
                Texture2D   tex     = choices[_iconRandom.Next(0, choices.Length)];
                if (tex != null)
                {
                    imgIsland.sprite = MakeSprite(tex, 350, 350, 170, 170);
                    SetIconSize(imgIsland, ICON_SIZE);
                }
            }
        }

        private static void ReloadPlayersSpritesIfNeeded()
        {
            try
            {
                IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
                if (players == null || players.Count == 0)
                    return;

                // FIX: _forceReloadPlayer was reset to false BEFORE being passed to ClearPlayerImages,
                //      so forceReloadPlayer always received false — captured before reset now
                bool needsReload = imgPlayers.Count == 0
                                || imgPlayers.Count != players.Count
                                || _forceReloadPlayer;
                if (!needsReload)
                    return;

                bool wasForced = _forceReloadPlayer;
                _forceReloadPlayer = false;
                ClearPlayerImages(wasForced);

                if (!showPlayers)
                    return;

                Sprite playerSprite    = _texPlayer    != null ? MakeSprite(_texPlayer,    200, 200, 100, 100) : null;
                Sprite directionSprite = _texDirection != null ? MakeSprite(_texDirection, 250, 250, 125, 125) : null;

                for (int i = 0; i < players.Count; i++)
                {
                    GameObject playerGO = CreateUIObject("Player" + i + "_Sprite", canvas.transform);
                    goPlayers.Add(playerGO);
                    Image imgPlayer = playerGO.AddComponent<Image>();
                    imgPlayers.Add(imgPlayer);
                    imgPlayer.sprite = playerSprite;
                    imgPlayer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   playerIconSize);
                    imgPlayer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerIconSize);
                    imgPlayer.rectTransform.localPosition = Vector3.zero;

                    GameObject dirGO = CreateUIObject("PlayerDirection" + i + "_Sprite", canvas.transform);
                    goDirectionPlayers.Add(dirGO);
                    Image imgDir = dirGO.AddComponent<Image>();
                    imgDirectionPlayers.Add(imgDir);
                    imgDir.sprite = directionSprite;
                    imgDir.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   playerIconSize);
                    imgDir.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerIconSize);
                    imgDir.rectTransform.localPosition = Vector3.zero;
                }

                Debug.Log("Stranded Deep Map mod : reload players completed");
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : player sprites initialization failed : " + e);
            }
        }

        private static void RefreshPlayerPositions()
        {
            try
            {
                if (imgPlayers.Count == 0 || imgDirectionPlayers.Count == 0)
                    return;

                IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
                for (int i = 0; i < players.Count && i < imgPlayers.Count; i++)
                {
                    Vector3 pos = players[i].transform.localPosition;
                    imgPlayers[i].rectTransform.localPosition = TransformToScreenCoordinates(pos.x, pos.z);
                    imgPlayers[i].enabled = showPlayers;

                    try
                    {
                        if (imgDirectionPlayers[i] != null)
                        {
                            imgDirectionPlayers[i].rectTransform.localPosition = TransformToScreenCoordinates(pos.x, pos.z);
                            imgDirectionPlayers[i].enabled = showPlayers;

                            Vector3 forward     = players[i].transform.forward;
                            Vector3 trueUp      = new Vector3(0f, 0f, 1f);
                            float   angleToNorth = -Vector3.SignedAngle(trueUp, forward, players[i].transform.up);
                            imgDirectionPlayers[i].transform.rotation = Quaternion.Euler(0, 0, angleToNorth);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Stranded Deep Map mod : player direction drawing failed : " + ex);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : player position drawing failed : " + e);
            }
        }

        private static void InitCargoSprite()
        {
            if (goCargo != null)
                return;

            goCargo = CreateUIObject("Cargo_Sprite", canvas.transform);
            imgCargo = goCargo.AddComponent<Image>();
            if (_texCargo != null)
                imgCargo.sprite = MakeSprite(_texCargo, 200, 200, 100, 100);
            imgCargo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   playerIconSize);
            imgCargo.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerIconSize);
            imgCargo.rectTransform.localPosition = Vector3.zero;
        }

        private static void RetrieveCargoObject()
        {
            foreach (SaveablePrefab sp in Game.FindObjectsOfType<SaveablePrefab>())
            {
                try
                {
                    if (sp != null
                        && sp.name != null
                        && string.Equals(sp.name, ENDGAME_CARGO_NAME, StringComparison.Ordinal))
                    {
                        Debug.Log("Stranded Deep Map Mod : retrieved the endgame cargo");
                        cargoInstance = sp;
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Stranded Deep Map Mod : retrieving the endgame cargo failed : " + e);
                }
            }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private static Vector3 TransformToScreenCoordinates(float x, float y)
        {
            float convertedX = x * ((MAP_AREA_WIDTH  - 150) / (ingameAreaSize * mapScaleX));
            float convertedY = y * ((MAP_AREA_HEIGHT - 150) / (ingameAreaSize * mapScaleY));
            return new Vector3(convertedX, convertedY);
        }

        // FIX: sqrMagnitude instead of Sqrt+Pow — no allocation, no sqrt
        private static bool IsNearEnough(float islandX, float islandY, bool discovered)
        {
            if (discovered) return true;
            float sqrDist = viewDistance * viewDistance;
            IList<Beam.IPlayer> players = Beam.PlayerRegistry.AllPlayers;
            for (int i = 0; i < players.Count; i++)
            {
                Vector3 pos = players[i].transform.localPosition;
                float dx = islandX - pos.x;
                float dz = islandY - pos.z;
                if (dx * dx + dz * dz <= sqrDist)
                    return true;
            }
            return false;
        }

        private static void SetIconSize(Image img, float size)
        {
            img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   size);
            img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }

        // FIX: helper to make sprite from pre-loaded texture — no new Texture2D, no LoadImage
        private static Sprite MakeSprite(Texture2D tex, int w, int h, float pivotX, float pivotY)
        {
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(pivotX, pivotY));
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            return go;
        }

        // ─── Texture generation ───────────────────────────────────────────────────

        public static Texture2D GenerateThumb(Beam.Terrain.Map map, bool isMission)
        {
            try
            {
                int size = WorldUtilities.IsStrandedWide() ? WorldUtilities.IslandSize + 1 : 257;
                Texture2D islandSilhouette = new Texture2D(size, size, TextureFormat.ARGB32, false);

                float[,] heightmap = map.HeightmapData;
                int      h0        = map.HeightmapData.GetLength(0);
                int      w0        = map.HeightmapData.GetLength(1);

                // FIX: SetPixels32 bulk write instead of SetPixel per pixel — far faster
                Color32[] pixels  = new Color32[size * size];
                System.Random alphaRandom = new System.Random();

                for (int y = 0; y < h0 && y < size; y++)
                {
                    for (int x = 0; x < w0 && x < size; x++)
                    {
                        if (heightmap[y, x] >= 0.667f)
                        {
                            byte alpha = (byte)alphaRandom.Next(230, 255);
                            pixels[y * size + x] = isMission
                                ? new Color32(255, 0, 0, alpha)
                                : new Color32(51, 13, 0, alpha);
                        }
                        else
                        {
                            pixels[y * size + x] = new Color32(0, 0, 0, 0);
                        }
                    }
                }

                islandSilhouette.SetPixels32(pixels);
                islandSilhouette.Apply();
                return islandSilhouette;
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Map mod : silhouette reading error : " + e);
            }
            return null;
        }

        public static byte[] ExtractResource(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream s = a.GetManifestResourceStream(filename))
            {
                if (s == null) return null;
                byte[] ba = new byte[s.Length];
                s.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        // FIX: LoadTexture helper — single place to create+load a texture from embedded resource
        private static Texture2D LoadTexture(string resource, int w, int h)
        {
            byte[] data = ExtractResource(resource);
            if (data == null) return null;
            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            tex.LoadImage(data);
            return tex;
        }

        // ─── Canvas factory ───────────────────────────────────────────────────────

        private static GameObject CreateCanvas()
        {
            GameObject go   = new GameObject("Canvas");
            Canvas     cnvs = go.AddComponent<Canvas>();
            cnvs.renderMode   = RenderMode.ScreenSpaceOverlay;
            cnvs.pixelPerfect = false;
            cnvs.sortingOrder = 12;
            cnvs.targetDisplay = 0;

            CanvasScaler cvsl = go.AddComponent<CanvasScaler>();
            cvsl.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cvsl.referenceResolution  = new Vector2(MAP_CANVAS_DEFAULT_WIDTH, MAP_CANVAS_DEFAULT_HEIGHT);
            cvsl.matchWidthOrHeight   = 0.5f;
            cvsl.screenMatchMode      = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cvsl.referencePixelsPerUnit = 100f;

            // FIX: GraphicRaycaster removed — map has no clickable elements,
            //      raycasting every frame was pure overhead
            return go;
        }

        // ─── Config ───────────────────────────────────────────────────────────────

        private static void ReadConfig()
        {
            string dir = FilePath.SAVE_FOLDER;
            if (!System.IO.Directory.Exists(dir)) return;
            string path = System.IO.Path.Combine(dir, "StrandedDeepMapperMod.config");
            if (!System.IO.File.Exists(path)) return;

            foreach (string line in System.IO.File.ReadAllLines(path))
            {
                string[] tokens = line.Split(new[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2) continue;
                string key = tokens[0].Trim();
                string val = tokens[1].Trim();
                try
                {
                    if      (key == "revealWorld")                revealWorld                = bool.Parse(val);
                    else if (key == "revealMissions")             revealMissions             = bool.Parse(val);
                    else if (key == "showPlayers")                showPlayers                = bool.Parse(val);
                    else if (key == "showAlternativeEndingCargo") showAlternativeEndingCargo = bool.Parse(val);
                    else if (key == "viewDistance")
                    {
                        viewDistance = Mathf.Clamp(float.Parse(val), 600f, 1500f);
                    }
                    else if (key == "playerIconSize")
                    {
                        playerIconSize = Mathf.Clamp(float.Parse(val), 10f, 100f);
                    }
                }
                catch { }
            }
        }

        private static void WriteConfig()
        {
            string dir = FilePath.SAVE_FOLDER;
            if (!System.IO.Directory.Exists(dir)) return;
            string path = System.IO.Path.Combine(dir, "StrandedDeepMapperMod.config");
            var sb = new StringBuilder();
            sb.AppendLine("viewDistance="              + viewDistance              + ";");
            sb.AppendLine("revealWorld="               + revealWorld               + ";");
            sb.AppendLine("revealMissions="            + revealMissions            + ";");
            sb.AppendLine("playerIconSize="            + playerIconSize            + ";");
            sb.AppendLine("showPlayers="               + showPlayers               + ";");
            sb.AppendLine("showAlternativeEndingCargo=" + showAlternativeEndingCargo + ";");
            System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
