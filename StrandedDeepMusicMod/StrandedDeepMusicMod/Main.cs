using Beam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepMusicMod
{
    static class Main
    {
        //private static AudioClip original;
        //private static AudioClip jaws;
        private static List<Track> AllTracks { get; set; }
        private static Dictionary<string, string> foundAudioFiles;
        private static Dictionary<string, byte[]> foundAudioContent;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            modEntry.OnHideGUI = OnHideGUI;

            foundAudioFiles = new Dictionary<string, string>();
            foundAudioContent = new Dictionary<string, byte[]>();

            InitDirectories();

            LoadFiles();

            ReplaceAudio();

            return true;
        }

        private static void InitDirectories()
        {
            try
            {
                var values = Enum.GetValues(typeof(TrackType));
                string currentPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string modPath = Path.GetDirectoryName(currentPath);
                foreach (TrackType trackType in values)
                {
                    if (trackType == TrackType.Intro
                        || trackType == TrackType.MainMenu)
                        continue;

                    string targetPath = Path.Combine(modPath, trackType.ToString());
                    if (!Directory.Exists(targetPath))
                    {
                        Debug.Log("Stranded Deep Music Mod : creating directory : " + targetPath);
                        Directory.CreateDirectory(targetPath);
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Stranded Deep Music Mod HowTo");
                sb.AppendLine("");
                sb.AppendLine("Place a single music file in every directory for the tracks you wish to replace (the mod will take the first file it finds)");
                sb.AppendLine("");
                sb.AppendLine("WARNING");
                sb.AppendLine("The file must be a WAV file format, uncompressed and PCM");
                sb.AppendLine("");
                sb.AppendLine("Use these settings : 44100Hz sampling, 16bits sample size (i have not tested other formats)");
                sb.AppendLine("");
                sb.AppendLine("You may use NCH's WavePad to convert an MP3 to this WAV format (given only as an example, any other audio software will do)");

                string targetFileName = Path.Combine(modPath, "HowToUse.txt");
                File.WriteAllText(targetFileName, sb.ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Music Mod : directory creation error : " + e);
            }
        }

        private static void LoadFiles()
        {
            try
            {
                foundAudioFiles.Clear();
                foundAudioContent.Clear();

                var values = Enum.GetValues(typeof(TrackType));
                string currentPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string modPath = Path.GetDirectoryName(currentPath);
                foreach (TrackType trackType in values)
                {
                    if (trackType == TrackType.Intro
                        || trackType == TrackType.MainMenu)
                        continue;

                    string targetPath = Path.Combine(modPath, trackType.ToString());
                    if (Directory.Exists(targetPath))
                    {
                        foreach (string wavFile in Directory.EnumerateFiles(targetPath, "*.wav"))
                        {
                            if (!String.IsNullOrEmpty(wavFile) && File.Exists(wavFile))
                            {
                                try
                                {
                                    Debug.Log("Stranded Deep Music Mod : audio file found for : " + trackType + " (" + Path.GetFileName(wavFile) + ")");
                                    foundAudioFiles.Add(trackType.ToString(), Path.GetFileName(wavFile));
                                    foundAudioContent.Add(trackType.ToString(), File.ReadAllBytes(wavFile));
                                }
                                catch { }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Music Mod : audio files loading error : " + e);
            }
        }

        private static void ReplaceAudio()
        {
            if (AllTracks == null && SoundtrackManager.Instance != null)
            {
                if (SoundtrackManager.Instance != null)
                {
                    Debug.Log("Stranded Deep Music mod : audio object found");
                    FieldInfo fi = typeof(SoundtrackManager).GetField("_tracks", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fi != null)
                    {
                        Debug.Log("Stranded Deep Music mod : injecting sample");
                        List<Track> tracks = fi.GetValue(SoundtrackManager.Instance) as List<Track>;
                        if (tracks != null && tracks.Count > 0)
                            AllTracks = tracks;

                        var values = Enum.GetValues(typeof(TrackType));
                        foreach (TrackType trackType in values)
                        {
                            if (trackType == TrackType.Intro
                                || trackType == TrackType.MainMenu)
                                continue;

                            if (foundAudioFiles.ContainsKey(trackType.ToString())
                                && foundAudioContent.ContainsKey(trackType.ToString()))
                            {
                                Debug.Log("Stranded Deep Music mod : injecting sample in : " + trackType.ToString());
                                try
                                {
                                    Track currentTrack = GetTrack(trackType);
                                    if (currentTrack != null)
                                    {
                                        AudioClip clip = WavUtility.ToAudioClip(foundAudioContent[trackType.ToString()]);
                                        currentTrack.Clip = clip;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Stranded Deep Music mod : sample injection failed for : " + trackType.ToString() + " : " + e);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Track GetTrack(TrackType trackType)
        {
            if (AllTracks == null)
                return null;

            return AllTracks.FirstOrDefault((Track t) => t.Type == trackType);
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            try
            {
                GUILayout.Label("Stranded Deep Music mod by Hantacore");

                // add button to open the target directory to help the player
                if (GUILayout.Button("Open target directory"))
                {
                    System.Diagnostics.Process.Start("explorer.exe", @"e:\");
                }

                var values = Enum.GetValues(typeof(TrackType));
                foreach (TrackType trackType in values)
                {
                    if (foundAudioFiles.ContainsKey(trackType.ToString()))
                    {
                        string bidon = foundAudioFiles[trackType.ToString()];
                        GUILayout.Label(trackType.ToString());
                        bidon = GUILayout.TextField(bidon, GUILayout.Width(300f));
                    }
                    else
                    {
                        GUILayout.Label(trackType.ToString());
                        string bidon = GUILayout.TextField("", GUILayout.Width(300f));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep Music Mod : GUI loading error : " + e);
            }
        }

        static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (Beam.Game.State != GameState.MAP_EDITOR)
            {
                ReplaceAudio();
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
