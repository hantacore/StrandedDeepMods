using Beam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace StrandedDeepMusicMod
{
    static class Main
    {
        // ─────────────────────────────────────────────
        // Chargement du mod
        // ─────────────────────────────────────────────

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnGUI = OnGUI;

            InitDirectories();

            // Harmony est appliqué immédiatement au chargement du mod,
            // avant que SoundtrackManager.Start() soit appelé
            MultiTrackManager.ApplyPatch();

            return true;
        }

        // ─────────────────────────────────────────────
        // Dossiers
        // ─────────────────────────────────────────────

        private static void InitDirectories()
        {
            string modPath = GetModPath();
            try
            {
                MigrateLegacyDirectories(modPath);
                MultiTrackManager.InitDirectories(modPath);
                WriteHowTo(modPath);
            }
            catch (Exception e)
            {
                Debug.LogError("[MusicMod] Directory creation error: " + e);
            }
        }

        private static void MigrateLegacyDirectories(string modPath)
        {
            var migrations = new Dictionary<string, string>
            {
                { "Ambience_Sunny_1",            "Ambience_Sunny"            },
                { "Ambience_Sunny_2",            "Ambience_Sunny"            },
                { "Ambience_Sailing_1",          "Ambience_Sailing"          },
                { "Ambience_Sailing_2",          "Ambience_Sailing"          },
                { "Boss_Eel_Intro",              "Boss_Eel"                  },
                { "Boss_Shark_Intro",            "Boss_Shark"                },
                { "Boss_Squid_Intro",            "Boss_Squid"                },
            };

            foreach (var kvp in migrations)
            {
                string legacyDir = Path.Combine(modPath, kvp.Key);
                string targetDir = Path.Combine(modPath, kvp.Value);

                if (!Directory.Exists(legacyDir)) continue;

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                foreach (string file in AudioLoader.FindAllAudioFiles(legacyDir))
                {
                    string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                    if (File.Exists(destFile))
                        destFile = Path.Combine(targetDir,
                            Path.GetFileNameWithoutExtension(file)
                            + "_from_" + kvp.Key
                            + Path.GetExtension(file));

                    File.Move(file, destFile);
                    Debug.Log("[MusicMod] Migration : " + Path.GetFileName(file)
                        + " -> " + kvp.Value + "/");
                }

                if (Directory.GetFiles(legacyDir).Length == 0
                    && Directory.GetDirectories(legacyDir).Length == 0)
                {
                    Directory.Delete(legacyDir);
                    Debug.Log("[MusicMod] Legacy folder deleted: " + kvp.Key);
                }
                else
                {
                    Debug.LogWarning("[MusicMod] Legacy folder not empty, skipping deletion: " + legacyDir);
                }
            }
        }

        private static void WriteHowTo(string modPath)
        {
            File.WriteAllText(Path.Combine(modPath, "HowToUse.txt"),
                "Stranded Deep Music Mod - How To Use\n\n" +
                "── How it works ──\n" +
                "Each game track has its own folder.\n" +
                "Place one or more audio files in a folder to replace the corresponding track.\n" +
                "If multiple files are present, the mod will pick one at random each time.\n\n" +
                "── Supported formats ──\n" +
                "MP3, WAV (uncompressed PCM, 44100 Hz, 16 bits).\n"
            );
        }

        // ─────────────────────────────────────────────
        // GUI
        // ─────────────────────────────────────────────

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            try
            {
                GUILayout.Label("Stranded Deep Music Mod by Hantacore");
                GUILayout.Space(6);

                bool loading = MultiTrackManager.IsLoading;

                DrawGroupSection("── Menus ──", loading, new string[]
                {
                    "MainMenu", "Pause_Menu", "Player_Death"
                });

                GUILayout.Space(4);
                DrawGroupSection("── Ambience ──", loading, new string[]
                {
                    "Ambience_First_Arrival", "Ambience_Sunny", "Ambience_Night",
                    "Ambience_Sailing", "Ambience_Shark", "Ambience_Life_Threatening",
                    "Ambience_Carrier", "Ambience_Cockpit"
                });

                GUILayout.Space(4);
                DrawGroupSection("── Boss ──", loading, new string[]
                {
                    "Boss_Eel", "Boss_Shark", "Boss_Squid",
                    "Boss_Victory", "Boss_Failed"
                });

                GUILayout.Space(4);
                DrawGroupSection("── Cockpit ──", loading, new string[]
                {
                    "Cockpit_TakeOff", "Cockpit_Crash"
                });

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Open mod folder", GUILayout.Width(200f)))
                    System.Diagnostics.Process.Start("explorer.exe", GetModPath());

                GUI.enabled = !loading;
                if (GUILayout.Button("Reload files", GUILayout.Width(180f)))
                    MultiTrackManager.Reload();
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                if (loading)
                    GUILayout.Label("... loading en cours");
            }
            catch (Exception e)
            {
                Debug.LogError("[MusicMod] GUI error: " + e);
            }
        }

        private static void DrawGroupSection(string title, bool loading, string[] groupNames)
        {
            GUILayout.Label(title);
            foreach (string groupName in groupNames)
            {
                MultiTrackGroup group;
                bool hasGroup = MultiTrackManager.TryGetGroup(groupName, out group);
                bool isAudioSource = MultiTrackManager.IsAudioSourceGroup(groupName);

                string status;
                if (loading)
                {
                    status = "... loading";
                }
                else if (!hasGroup)
                {
                    status = "(empty folder)";
                }
                else if (group.Clips.Count == 1)
                {
                    status = "✓ " + group.GetFileNames()[0];
                    if (isAudioSource)
                        status += "  [menu]";
                }
                else
                {
                    string[] fileNames = group.GetFileNames();
                    status = group.Clips.Count + " track(s)  |  " + string.Join(", ", fileNames);
                    if (isAudioSource)
                        status += "  [menu]";
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(groupName, GUILayout.Width(200f));
                GUILayout.TextField(status, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }
        }

        // ─────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────

        public static string GetModPath()
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
