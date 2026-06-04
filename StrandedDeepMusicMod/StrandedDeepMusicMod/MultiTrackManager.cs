using Beam;
using Beam.UI;
using Beam.Utilities;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace StrandedDeepMusicMod
{
    internal class MultiTrackGroup
    {
        public string          GroupName { get; }
        public List<AudioClip> Clips     { get; } = new List<AudioClip>();
        private int _lastIndex = -1;

        public MultiTrackGroup(string groupName) { GroupName = groupName; }

        public AudioClip PickRandom()
        {
            if (Clips.Count == 0) return null;
            if (Clips.Count == 1) return Clips[0];
            int index;
            do { index = UnityEngine.Random.Range(0, Clips.Count); }
            while (index == _lastIndex);
            _lastIndex = index;
            return Clips[index];
        }

        public bool HasClips { get { return Clips.Count > 0; } }

        public string[] GetFileNames()
        {
            string[] names = new string[Clips.Count];
            for (int i = 0; i < Clips.Count; i++)
                names[i] = Clips[i].name;
            return names;
        }
    }

    internal static class MultiTrackManager
    {
        private static readonly Dictionary<string, MultiTrackGroup> _groups
            = new Dictionary<string, MultiTrackGroup>();

        // Tracks injected directly into _tracks (permanent replacement per session)
        private static readonly HashSet<string> StaticGroups = new HashSet<string>
        {
            // Tracks present in SoundtrackManager._tracks (injected at startup)
            "MainMenu", "Player_Death",
            "Ambience_First_Arrival", "Ambience_Shark", "Ambience_Life_Threatening",
            "Ambience_Carrier", "Ambience_Cockpit",
            "Cockpit_TakeOff", "Cockpit_Crash",
            "Boss_Eel", "Boss_Shark", "Boss_Squid", "Boss_Victory", "Boss_Failed",
            // Pause_Menu: direct AudioSource in MainMenuPresenter (patched via Show())
            // Cartographer: mechanism not identified, temporarily disabled
        };

        // Groups managed via direct AudioSource in MainMenuPresenter
        private static readonly HashSet<string> AudioSourceGroups = new HashSet<string>
        {
            "Pause_Menu",
        };

        // Tracks managed by PollSoundtrack (hot reload supported)
        private static readonly HashSet<string> DynamicGroups = new HashSet<string>
        {
            "Ambience_Sunny", "Ambience_Sailing", "Ambience_Night",
        };

        public static readonly Dictionary<string, TrackType[]> SupportedGroups
            = new Dictionary<string, TrackType[]>
        {
            { "MainMenu",                  new[] { TrackType.MainMenu } },
            { "Pause_Menu",                new[] { TrackType.Pause_Menu } },
            // { "Cartographer", new[] { TrackType.Cartographer } }, // TODO: à localiser
            { "Player_Death",              new[] { TrackType.Player_Death } },
            { "Ambience_First_Arrival",    new[] { TrackType.Ambience_First_Arrival } },
            { "Ambience_Sunny",            new[] { TrackType.Ambience_Sunny_1, TrackType.Ambience_Sunny_2 } },
            { "Ambience_Sailing",          new[] { TrackType.Ambience_Sailing_1, TrackType.Ambience_Sailing_2 } },
            { "Ambience_Night",            new[] { TrackType.Ambience_Night } },
            { "Ambience_Shark",            new[] { TrackType.Ambience_Shark } },
            { "Ambience_Life_Threatening", new[] { TrackType.Ambience_Life_Threatening } },
            { "Ambience_Carrier",          new[] { TrackType.Ambience_Carrier } },
            { "Ambience_Cockpit",          new[] { TrackType.Ambience_Cockpit } },
            { "Cockpit_TakeOff",           new[] { TrackType.Cockpit_TakeOff } },
            { "Cockpit_Crash",             new[] { TrackType.Cockpit_Crash } },
            { "Boss_Eel",                  new[] { TrackType.Boss_Eel_Intro, TrackType.Boss_Eel } },
            { "Boss_Shark",                new[] { TrackType.Boss_Shark_Intro, TrackType.Boss_Shark } },
            { "Boss_Squid",                new[] { TrackType.Boss_Squid_Intro, TrackType.Boss_Squid } },
            { "Boss_Victory",              new[] { TrackType.Boss_Victory } },
            { "Boss_Failed",               new[] { TrackType.Boss_Failed } },
        };

        public static bool IsLoading { get; private set; } = false;

        private static Harmony _harmony;
        private static bool    _patched = false;

        // ─────────────────────────────────────────────────────────────
        // Dossiers
        // ─────────────────────────────────────────────────────────────

        public static void InitDirectories(string modPath)
        {
            foreach (string groupName in SupportedGroups.Keys)
            {
                string dir = Path.Combine(modPath, groupName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Debug.Log("[MusicMod] Directory created: " + dir);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        // Harmony
        // ─────────────────────────────────────────────────────────────

        public static void ApplyPatch()
        {
            if (_patched) return;
            try
            {
                _harmony = new Harmony("com.hantacore.strandeddeep.musicmod");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                _patched = true;
                Debug.Log("[MusicMod] Harmony patch applied.");
            }
            catch (Exception e)
            {
                Debug.LogError("[MusicMod] Harmony patch failed: " + e);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // Chargement initial (depuis patch Start) — statique + dynamique
        // ─────────────────────────────────────────────────────────────

        public static IEnumerator LoadAllAsync(List<Track> gameTracks, string modPath, MonoBehaviour runner, Action onComplete = null)
        {
            IsLoading = true;
            _groups.Clear();

            foreach (var kvp in SupportedGroups)
            {
                string       groupName = kvp.Key;
                TrackType[]  types     = kvp.Value;
                var          group     = new MultiTrackGroup(groupName);
                string       dir       = Path.Combine(modPath, groupName);
                List<string> files     = AudioLoader.FindAllAudioFiles(dir);

                if (files.Count == 0) continue;

                bool done = false;
                runner.StartCoroutine(AudioLoader.LoadAllAsync(files, clips =>
                {
                    group.Clips.AddRange(clips);
                    done = true;
                }, runner));
                while (!done) yield return null;

                if (!group.HasClips) continue;

                _groups[groupName] = group;
                Debug.Log("[MusicMod] Group '" + groupName + "': " + group.Clips.Count + " track(s).");

                // Pistes statiques : injecter directement dans _tracks pour la session
                if (StaticGroups.Contains(groupName))
                {
                    AudioClip clip = group.PickRandom();
                    foreach (TrackType t in types)
                    {
                        Track gameTrack = gameTracks.FirstOrDefault(tr => tr.Type == t);
                        if (gameTrack != null)
                        {
                            gameTrack.Clip = clip;
                            Debug.Log("[MusicMod] Static track injected: " + t);
                        }
                        else
                        {
                            Debug.LogWarning("[MusicMod] Track not found in _tracks: " + t
                                + " (available tracks: "
                                + string.Join(", ", gameTracks.Select(tr => tr.Type.ToString()).ToArray())
                                + ")");
                        }
                    }
                }
                // Pistes dynamiques : gérées par PollSoundtrack, pas d'injection directe
            }

            IsLoading = false;
            Debug.Log("[MusicMod] Initial loading complete.");
            if (onComplete != null) onComplete();
        }

        // ─────────────────────────────────────────────────────────────
        // Rechargement à chaud (pistes dynamiques uniquement)
        // ─────────────────────────────────────────────────────────────

        public static void Reload()
        {
            if (IsLoading || SoundtrackManager.Instance == null) return;

            FieldInfo fi = typeof(SoundtrackManager)
                .GetField("_tracks", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null) return;

            List<Track> tracks = fi.GetValue(SoundtrackManager.Instance) as List<Track>;
            if (tracks == null) return;

            SoundtrackManager.Instance.StartCoroutine(
                ReloadDynamicAsync(tracks, Main.GetModPath(), SoundtrackManager.Instance));
        }

        private static IEnumerator ReloadDynamicAsync(List<Track> gameTracks, string modPath, MonoBehaviour runner)
        {
            IsLoading = true;

            // Recharger aussi les groupes AudioSource (menu, pause) pour la GUI
            foreach (string groupName in AudioSourceGroups)
            {
                _groups.Remove(groupName);
                string       dir   = Path.Combine(modPath, groupName);
                List<string> files = AudioLoader.FindAllAudioFiles(dir);
                if (files.Count == 0) continue;

                var  group = new MultiTrackGroup(groupName);
                bool done  = false;
                runner.StartCoroutine(AudioLoader.LoadAllAsync(files, clips =>
                {
                    group.Clips.AddRange(clips);
                    done = true;
                }, runner));
                while (!done) yield return null;

                if (group.HasClips)
                {
                    _groups[groupName] = group;
                    Debug.Log("[MusicMod] Menu group reloaded: " + groupName
                        + " (" + group.Clips.Count + " track(s)).");
                }
            }

            foreach (string groupName in DynamicGroups)
            {
                _groups.Remove(groupName);

                string       dir   = Path.Combine(modPath, groupName);
                List<string> files = AudioLoader.FindAllAudioFiles(dir);
                if (files.Count == 0) continue;

                var  group = new MultiTrackGroup(groupName);
                bool done  = false;
                runner.StartCoroutine(AudioLoader.LoadAllAsync(files, clips =>
                {
                    group.Clips.AddRange(clips);
                    done = true;
                }, runner));
                while (!done) yield return null;

                if (group.HasClips)
                {
                    _groups[groupName] = group;
                    Debug.Log("[MusicMod] Dynamic group reloaded: " + groupName
                        + " (" + group.Clips.Count + " track(s)).");
                }
            }

            IsLoading = false;
            Debug.Log("[MusicMod] Dynamic reload complete.");
        }

        // ─────────────────────────────────────────────────────────────
        // Accès
        // ─────────────────────────────────────────────────────────────

        public static bool TryGetGroup(string groupName, out MultiTrackGroup group)
        {
            return _groups.TryGetValue(groupName, out group);
        }

        public static IEnumerable<string> GetAllGroupNames()
        {
            return SupportedGroups.Keys;
        }

        /// <summary>
        /// Retourne le nom du groupe qui contient ce TrackType, ou null si non géré.
        /// </summary>
        public static bool IsStaticGroup(string groupName)
        {
            return StaticGroups.Contains(groupName);
        }

        public static bool IsAudioSourceGroup(string groupName)
        {
            return AudioSourceGroups.Contains(groupName);
        }

        public static string GetGroupNameForType(TrackType trackType)
        {
            foreach (var kvp in SupportedGroups)
                foreach (TrackType t in kvp.Value)
                    if (t == trackType) return kvp.Key;
            return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Patch Awake : lance le chargement dès l'initialisation du composant
    // ─────────────────────────────────────────────────────────────────

    [HarmonyPatch(typeof(SoundtrackManager), "Awake")]
    internal static class Patch_SoundtrackManager_Awake
    {
        static void Postfix(SoundtrackManager __instance)
        {
            try
            {
                FieldInfo fi = typeof(SoundtrackManager)
                    .GetField("_tracks", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi == null) return;

                List<Track> tracks = fi.GetValue(__instance) as List<Track>;
                if (tracks == null || tracks.Count == 0) return;

                Debug.Log("[MusicMod] Awake: starting clip loading...");
                __instance.StartCoroutine(
                    MultiTrackManager.LoadAllAsync(tracks, Main.GetModPath(), __instance));
            }
            catch (Exception e)
            {
                Debug.LogError("[MusicMod] Awake patch error: " + e);
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Patch PlayTrack : si le clip statique est prêt on le joue,
    // sinon on attend la fin du chargement puis on relance
    // ─────────────────────────────────────────────────────────────────

    [HarmonyPatch(typeof(SoundtrackManager), "PlayTrack")]
    internal static class Patch_SoundtrackManager_PlayTrack
    {
        private static bool _pendingPlay = false;

        static bool Prefix(SoundtrackManager __instance, TrackType trackType)
        {
            // Si pas de chargement en cours : laisser passer normalement
            if (!MultiTrackManager.IsLoading) return true;

            // Piste non gérée par le mod : laisser passer
            string groupName = MultiTrackManager.GetGroupNameForType(trackType);
            if (groupName == null) return true;

            // Eviter les doublons : une seule coroutine d'attente à la fois
            if (_pendingPlay) return false;
            _pendingPlay = true;

            Debug.Log("[MusicMod] PlayTrack(" + trackType + ") delayed, loading in progress...");
            __instance.StartCoroutine(WaitAndPlay(__instance, trackType));
            return false;
        }

        private static IEnumerator WaitAndPlay(SoundtrackManager instance, TrackType trackType)
        {
            while (MultiTrackManager.IsLoading)
                yield return null;

            _pendingPlay = false;
            Debug.Log("[MusicMod] Retrying PlayTrack(" + trackType + ") after loading.");
            instance.PlayTrack(trackType);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Patch MainMenuPresenter.Show : remplace les AudioSource directes
    // _mainMenuTrack et _pauseMenuTrack avant lecture
    // ─────────────────────────────────────────────────────────────────

    [HarmonyPatch(typeof(MainMenuPresenter), "Show")]
    internal static class Patch_MainMenuPresenter_Show
    {
        private static FieldInfo _fi_mainMenuTrack;
        private static FieldInfo _fi_pauseMenuTrack;

        static Patch_MainMenuPresenter_Show()
        {
            var t = typeof(MainMenuPresenter);
            _fi_mainMenuTrack  = t.GetField("_mainMenuTrack",  BindingFlags.NonPublic | BindingFlags.Instance);
            _fi_pauseMenuTrack = t.GetField("_pauseMenuTrack", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static void Prefix(MainMenuPresenter __instance)
        {
            try
            {
                // Menu principal
                TryReplaceAudioSource(__instance, _fi_mainMenuTrack, "MainMenu");
                // Menu pause ingame
                TryReplaceAudioSource(__instance, _fi_pauseMenuTrack, "Pause_Menu");
            }
            catch (Exception e)
            {
                Debug.LogError("[MusicMod] MainMenuPresenter.Show patch error: " + e);
            }
        }

        private static void TryReplaceAudioSource(
            MainMenuPresenter instance, FieldInfo fi, string groupName)
        {
            if (fi == null) return;

            MultiTrackGroup group;
            if (!MultiTrackManager.TryGetGroup(groupName, out group) || !group.HasClips) return;

            AudioSource source = fi.GetValue(instance) as AudioSource;
            if (source == null) return;

            source.clip = group.PickRandom();
            Debug.Log("[MusicMod] AudioSource replaced: " + groupName + " (" + source.clip.name + ")");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Patch EventManager_TrackEvent : intercepte tous les TrackEvents
    // (couvre Cartographer, Pause_Menu, Boss, etc.)
    // ─────────────────────────────────────────────────────────────────

    [HarmonyPatch(typeof(SoundtrackManager), "EventManager_TrackEvent")]
    internal static class Patch_TrackEvent
    {
        private static FieldInfo _fi_tracks;

        static Patch_TrackEvent()
        {
            _fi_tracks = typeof(SoundtrackManager)
                .GetField("_tracks", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static void Prefix(SoundtrackManager __instance, TrackEvent ev)
        {
            // On n'intercepte que les Play, pas les Stop
            if (ev.EventType != TrackEventType.Play
                && ev.EventType != TrackEventType.Play_Instantly)
                return;

            string groupName = MultiTrackManager.GetGroupNameForType(ev.TrackType);
            if (groupName == null) return;

            // Statique seulement : les dynamiques sont gérés par PollSoundtrack
            if (!MultiTrackManager.IsStaticGroup(groupName)) return;

            MultiTrackGroup group;
            if (!MultiTrackManager.TryGetGroup(groupName, out group) || !group.HasClips) return;

            // Remplacer le clip dans _tracks juste avant que PlayTrack l'utilise
            if (_fi_tracks == null) return;
            List<Track> tracks = _fi_tracks.GetValue(__instance) as List<Track>;
            if (tracks == null) return;

            AudioClip clip = group.PickRandom();
            foreach (TrackType t in MultiTrackManager.SupportedGroups[groupName])
            {
                Track gameTrack = tracks.FirstOrDefault(tr => tr.Type == t);
                if (gameTrack != null)
                {
                    gameTrack.Clip = clip;
                    Debug.Log("[MusicMod] Clip substituted via TrackEvent: " + t);
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Patch PollSoundtrack : sélection dynamique des pistes ingame
    // ─────────────────────────────────────────────────────────────────

    [HarmonyPatch(typeof(SoundtrackManager), "PollSoundtrack_InvokeRepeating")]
    internal static class Patch_PollSoundtrack
    {
        private static FieldInfo  _fi_currentTrackType;
        private static FieldInfo  _fi_audio;
        private static FieldInfo  _fi_ambienceTimer;
        private static FieldInfo  _fi_isRaining;
        private static MethodInfo _mi_playTrack;
        private static MethodInfo _mi_getTrackPriority;

        static Patch_PollSoundtrack()
        {
            var t = typeof(SoundtrackManager);
            _fi_currentTrackType = t.GetField("_currentTrackType", BindingFlags.NonPublic | BindingFlags.Instance);
            _fi_audio            = t.GetField("_audio",            BindingFlags.NonPublic | BindingFlags.Instance);
            _fi_ambienceTimer    = t.GetField("_ambienceTimer",    BindingFlags.NonPublic | BindingFlags.Instance);
            _fi_isRaining        = t.GetField("_isRaining",        BindingFlags.NonPublic | BindingFlags.Instance);
            _mi_playTrack        = t.GetMethod("PlayTrack",        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            _mi_getTrackPriority = t.GetMethod("GetTrackPriority", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static bool Prefix(SoundtrackManager __instance)
        {
            try   { return RunPoll(__instance); }
            catch (Exception e)
            {
                Debug.LogError("[MusicMod] PollSoundtrack patch error: " + e);
                return true;
            }
        }

        private static bool RunPoll(SoundtrackManager instance)
        {
            if (LevelLoader.IsServerJoinInProgress) return false;

            var currentTrackType = (TrackType)_fi_currentTrackType.GetValue(instance);
            var audio            = (AudioSource)_fi_audio.GetValue(instance);
            var ambienceTimer    = (float)_fi_ambienceTimer.GetValue(instance);
            var isRaining        = (bool)_fi_isRaining.GetValue(instance);

            if (currentTrackType != TrackType.None && audio.isPlaying)
            {
                if (currentTrackType != TrackType.Ambience_Shark)
                    _fi_ambienceTimer.SetValue(instance, 0f);
                return false;
            }

            ambienceTimer += 10f;
            _fi_ambienceTimer.SetValue(instance, ambienceTimer);
            if (ambienceTimer <= 120f) return false;

            if (PlayerRegistry.AllPlayers.Any_NonAlloc(new Func<IPlayer, bool>(IsPlayerDying)))
            {
                PlayTrack(instance, TrackType.Ambience_Life_Threatening);
                return false;
            }

            if (isRaining) return false;

            if (PlayerRegistry.AllPlayers.Any_NonAlloc(new Func<IPlayer, bool>(IsPlayerOperating)))
            {
                PlayFromGroupOrFallback(instance, "Ambience_Sailing",
                    TrackType.Ambience_Sailing_1, TrackType.Ambience_Sailing_2);
                return false;
            }

            if (Singleton<Atmosphere>.Instance.DayTime)
            {
                PlayFromGroupOrFallback(instance, "Ambience_Sunny",
                    TrackType.Ambience_Sunny_1, TrackType.Ambience_Sunny_2);
                return false;
            }

            if (!Singleton<Atmosphere>.Instance.DayTime)
                PlayFromGroupOrFallback(instance, "Ambience_Night", TrackType.Ambience_Night);

            return false;
        }

        private static bool IsPlayerDying(IPlayer p)
            => p.IsInitialized && p.Statistics.HealthPercent < 0.15f;

        private static FieldInfo _fi_vehicle;
        private static bool _fi_vehicle_checked = false;

        private static bool IsPlayerOperating(IPlayer p)
        {
            if (!p.IsInitialized || p.Movement == null) return false;
            if (!_fi_vehicle_checked)
            {
                _fi_vehicle_checked = true;
                // Chercher la propriété véhicule par réflexion pour éviter MissingFieldException
                foreach (string name in new[] { "Vehicle", "OperatedObject", "CurrentVehicle", "Raft" })
                {
                    _fi_vehicle = p.Movement.GetType().GetField(name,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (_fi_vehicle != null)
                    {
                        Debug.Log("[MusicMod] Vehicle field found: " + name);
                        break;
                    }
                }
                if (_fi_vehicle == null)
                    Debug.LogWarning("[MusicMod] Vehicle field not found, sailing detection disabled.");
            }
            if (_fi_vehicle == null) return false;
            try { return _fi_vehicle.GetValue(p.Movement) != null; }
            catch { return false; }
        }

        private static void PlayFromGroupOrFallback(
            SoundtrackManager instance, string groupName, params TrackType[] fallbackTypes)
        {
            MultiTrackGroup group;
            if (MultiTrackManager.TryGetGroup(groupName, out group) && group.HasClips)
            {
                PlayClipDirectly(instance, group.PickRandom(), groupName);
            }
            else
            {
                TrackType chosen = fallbackTypes.Length == 1
                    ? fallbackTypes[0]
                    : (UnityEngine.Random.value < 0.5f ? fallbackTypes[0] : fallbackTypes[1]);
                PlayTrack(instance, chosen);
            }
        }

        private static void PlayClipDirectly(SoundtrackManager instance, AudioClip clip, string label)
        {
            if (clip == null) return;

            var audio            = (AudioSource)_fi_audio.GetValue(instance);
            var currentTrackType = (TrackType)_fi_currentTrackType.GetValue(instance);
            int currentPriority  = (int)_mi_getTrackPriority.Invoke(instance, new object[] { currentTrackType });

            if (audio.isPlaying && currentPriority > 0) return;

            Debug.Log("[MusicMod] Dynamic playback: " + label + " (" + clip.name + ")");
            audio.Stop();
            audio.clip   = clip;
            audio.loop   = false;
            audio.volume = 0f;
            audio.Play();
            _fi_currentTrackType.SetValue(instance, TrackType.None);
            instance.StartCoroutine(FadeIn(audio));
        }

        private static IEnumerator FadeIn(AudioSource audio)
        {
            while (audio != null && audio.volume < 1f)
            {
                audio.volume += 0.33333334f * Time.deltaTime;
                yield return null;
            }
            if (audio != null) audio.volume = 1f;
        }

        private static void PlayTrack(SoundtrackManager instance, TrackType type)
        {
            _mi_playTrack.Invoke(instance, new object[] { type });
        }
    }
}
