using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace StrandedDeepMusicMod
{
    /// <summary>
    /// Charge des fichiers audio (WAV ou MP3) de façon asynchrone via UnityWebRequest.
    /// Utilise un MonoBehaviour existant pour lancer les coroutines.
    /// </summary>
    internal static class AudioLoader
    {
        private static readonly string[] SupportedExtensions = { "*.wav", "*.mp3" };

        /// <summary>
        /// Retourne le premier fichier audio trouvé dans un dossier (WAV prioritaire sur MP3).
        /// </summary>
        public static string FindFirstAudioFile(string directory)
        {
            if (!Directory.Exists(directory)) return null;

            foreach (string ext in SupportedExtensions)
            {
                foreach (string file in Directory.EnumerateFiles(directory, ext))
                    return file;
            }
            return null;
        }

        /// <summary>
        /// Retourne tous les fichiers audio d'un dossier (WAV + MP3).
        /// </summary>
        public static List<string> FindAllAudioFiles(string directory)
        {
            var result = new List<string>();
            if (!Directory.Exists(directory)) return result;

            foreach (string ext in SupportedExtensions)
                foreach (string file in Directory.EnumerateFiles(directory, ext))
                    result.Add(file);

            return result;
        }

        /// <summary>
        /// Charge un fichier audio de façon asynchrone.
        /// Appelle onLoaded(clip) une fois prêt, ou onLoaded(null) en cas d'erreur.
        /// </summary>
        public static IEnumerator LoadAsync(string filePath, Action<AudioClip> onLoaded)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogWarning("[MusicMod] File not found: " + filePath);
                onLoaded(null);
                yield break;
            }

            AudioType audioType = GetAudioType(filePath);

            // WAV PCM custom : on garde WavUtility pour la compatibilité des formats
            // non standards (24bit, 32bit) qu'UnityWebRequest ne gère pas toujours bien
            if (audioType == AudioType.WAV)
            {
                AudioClip clip = null;
                try
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    clip = WavUtility.ToAudioClip(bytes,
                        name: Path.GetFileNameWithoutExtension(filePath));
                }
                catch (Exception e)
                {
                    Debug.LogError("[MusicMod] WAV loading error: " + filePath + " : " + e.Message);
                }
                onLoaded(clip);
                yield break;
            }

            // MP3 : UnityWebRequest
            // Note : pas de 'using' ici car incompatible avec yield return en C# < 8
            string uri = "file:///" + filePath.Replace("\\", "/");
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(uri, audioType);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError("[MusicMod] MP3 loading error: " + filePath + " : " + uwr.error);
                onLoaded(null);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                if (clip != null)
                    clip.name = Path.GetFileNameWithoutExtension(filePath);
                onLoaded(clip);
            }
            uwr.Dispose();
        }

        /// <summary>
        /// Charge une liste de fichiers audio en séquence.
        /// Appelle onAllLoaded(clips) une fois tous les fichiers traités.
        /// </summary>
        /// <summary>
        /// Charge tous les fichiers en séquence via StartCoroutine sur le runner fourni.
        /// Appelle onAllLoaded une fois tous les fichiers traités.
        /// </summary>
        public static IEnumerator LoadAllAsync(
            List<string> filePaths,
            Action<List<AudioClip>> onAllLoaded,
            MonoBehaviour runner = null)
        {
            var clips = new List<AudioClip>();
            foreach (string path in filePaths)
            {
                AudioClip loaded = null;
                bool done = false;

                // LoadAsync est une coroutine : on doit la lancer via StartCoroutine
                // si on est déjà dans une coroutine imbriquée
                if (runner != null)
                {
                    runner.StartCoroutine(LoadAsync(path, clip =>
                    {
                        loaded = clip;
                        done   = true;
                    }));
                    while (!done) yield return null;
                }
                else
                {
                    // Appel direct si on est la coroutine racine (StartCoroutine externe)
                    yield return LoadAsync(path, clip =>
                    {
                        loaded = clip;
                        done   = true;
                    });
                }

                if (loaded != null)
                    clips.Add(loaded);
            }
            onAllLoaded(clips);
        }

        private static AudioType GetAudioType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".mp3") return AudioType.MPEG;
            return AudioType.WAV;
        }
    }
}
