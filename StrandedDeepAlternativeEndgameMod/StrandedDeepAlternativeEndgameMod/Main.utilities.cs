using Beam;
using Beam.Events;
using Beam.Rendering;
using Beam.Serialization;
using Beam.Serialization.Json;
using Beam.Terrain;
using Beam.UI;
using Beam.Utilities;
using Funlabs;
using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityModManagerNet;

namespace StrandedDeepAlternativeEndgameMod
{
    static partial class Main
    {
        public static Vector3 CalculatePointOnSquare(float r, float angleInDegrees, float altitude)
        {
            Vector3 position = new Vector3(0, altitude, 0);

            float angle = (angleInDegrees % 360) * Mathf.PI / 180; //double angle = angleInDegrees * Math.PI /180;//?

            float angleModPiOverTwo = angle % (Mathf.PI / 4);

            if (angle >= 0 && angle < Mathf.PI / 4)
            {
                position.x = r;
                position.z = r * Mathf.Tan(angle);
            }
            else if (angle >= Mathf.PI / 4 && angle < Mathf.PI / 2)
            {
                position.x = r * Mathf.Tan(Mathf.PI / 2 - angle);
                position.z = r;
            }
            else if (angle >= Mathf.PI / 2 && angle < 3 * Mathf.PI / 4)
            {
                position.x = -1 * r * Mathf.Tan(angle % (Mathf.PI / 4));
                position.z = r;
            }
            else if (angle >= 3 * Mathf.PI / 4 && angle < Mathf.PI)
            {
                position.x = -1 * r;
                position.z = r * Mathf.Tan(Mathf.PI - angle);
            }
            else if (angle >= Mathf.PI && angle < 5 * Mathf.PI / 4)
            {
                position.x = -1 * r;
                position.z = -1 * r * Mathf.Tan(angle % (Mathf.PI / 4));
            }
            else if (angle >= 5 * Mathf.PI / 4 && angle < 3 * Mathf.PI / 2)
            {
                position.x = -1 * r * Mathf.Tan(3 * Mathf.PI / 2 - angle);
                position.z = -1 * r;
            }
            else if (angle >= 3 * Mathf.PI / 2 && angle < 7 * Mathf.PI / 4)
            {
                position.x = r * Mathf.Tan(angle % (Mathf.PI / 4));
                position.z = -1 * r;
            }
            else
            {
                position.x = r;
                position.z = -1 * r * Mathf.Tan(2 * Mathf.PI - angle);
            }

            return position;
        }

        #region Canvas instanciation

        //Creates Hidden GameObject and attaches Canvas component to it
        private static GameObject createCanvas(bool hide, string name = "AlternativeEndgameCanvas")
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
            cvsl.referenceResolution = new Vector2(alternativeEndingCanvasDefaultScreenWidth, alternativeEndingCanvasDefaultScreenHeight);

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

        private static void FreezePlayer(IPlayer player)
        {
            if (!player.IsOwner)
            {
                return;
            }
            Movement movement = player.Movement;
            movement.IsBusy = ++movement.IsBusy;
            player.Movement.IsInCutscene = true;
            player.Movement.CharacterController.enabled = false;
        }

        public static SaveablePrefab Create(uint prefabId, Vector3 position)
        {
            string text;
            bool flag = Prefabs.TryGetMultiplayerPrefabName(prefabId, out text);
            if (!flag)
            {
                SaveablePrefab result = MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, position, Quaternion.identity, MiniGuid.New(), null);
                new ReplicateCreate
                {
                    PrefabId = (short)prefabId,
                    Pos = position
                }.Post();
                return result;
            }
            if (Game.Mode.IsClient())
            {
                new ReplicateCreate
                {
                    PrefabId = (short)prefabId,
                    Pos = position
                }.Post();
                return null;
            }
            return MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, position, Quaternion.identity, MiniGuid.New(), null);
        }

        private class ReplicateCreate : MultiplayerMessage
        {
            // Token: 0x06005F5C RID: 24412 RVA: 0x00109DC4 File Offset: 0x00107FC4
            public override void OnPeer()
            {
                MultiplayerMng.Instantiate<SaveablePrefab>((uint)this.PrefabId, this.Pos, Quaternion.identity, MiniGuid.New(), null);
            }

            // Token: 0x04003CA7 RID: 15527
            [Replicate]
            public short PrefabId;

            // Token: 0x04003CA8 RID: 15528
            [Replicate]
            public Vector3 Pos;
        }

        private static SaveablePrefab CreateSaveablePrefabLight(uint prefabId, Vector3 position, Quaternion rotation, Zone zone)
        {
            GameObject gameObject;
            MiniGuid referenceId = new MiniGuid(new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"));//MiniGuid.NewFrom(position, prefabId, 48879);
            SaveablePrefab saveablePrefab = MultiplayerMng.Instantiate<SaveablePrefab>(prefabId, position, rotation, referenceId, null);
            gameObject = saveablePrefab.gameObject;
            gameObject.transform.SetParent(zone.SaveContainer, false);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            LootContainerSpawner loot = gameObject.GetComponent<LootContainerSpawner>();
            if (loot != null)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod : loot spawner found, bypassing");
                fi_containersGenerated.SetValue(loot, true);
            }

            zone.OnObjectCreated(gameObject);
            return saveablePrefab;
        }

        private static void TestLodGroups()
        {
            foreach (Camera cam in Camera.allCameras)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod lod tests near clip plane " + cam.name + " / " + cam.nearClipPlane);
                Debug.Log("Stranded Deep AlternativeEndgame Mod lod tests far clip plane " + cam.name + " / " + cam.farClipPlane);
                //foreach(float layer in cam.layerCullDistances)
                //if (cam.farClipPlane < 1000)
                //    cam.farClipPlane = 1000;
            }

            IImpostorParent impostorParenttest = cargoInstance.GetInterface<IImpostorParent>();
            LodController lctest = impostorParenttest as LodController;
            Vector3 position = PlayerRegistry.AllPlayers[0].transform.position;
            float magnitude = (position - cargoInstance.transform.position).magnitude;
            Lod lod = null;
            for (int i = 0; i < lctest.LodGroup.Lods.Count; i++)
            {
                Lod lod2 = lctest.LodGroup.Lods[i];
                int? num = (lod != null) ? new int?(lod.CullingDistance) : null;
                float num2 = ((num != null) ? ((float)num.GetValueOrDefault()) : 0f) * LodManager.LOD_BIAS;
                float num3 = (float)lod2.CullingDistance * LodManager.LOD_BIAS;
                bool flag = magnitude > num2;
                bool flag2 = magnitude < num3;
                bool active = flag && flag2;
                Debug.Log("Stranded Deep AlternativeEndgame Mod lod tests " + i + " / " + active);
                lod2.SetActive(active);
                lod = lod2;
            }
        }

        private static void AttachChildrenRecursive(Transform cargo, Transform transform, int level = 0)
        {
            try
            {
                int nextlevel = level++;
                StringBuilder sb = new StringBuilder();
                for (int tab = 0; tab < level; tab++)
                {
                    sb.Append("\t");
                }
                for (var i = transform.childCount; i-- > 0;)
                {
                    GameObject child = transform.GetChild(i).gameObject;
                    Debug.Log("Stranded Deep AlternativeEndgame Mod child object : " + sb.ToString() + child.name);
                    Debug.Log("Stranded Deep AlternativeEndgame Mod child object : " + sb.ToString() + child.GetType().Name);
                    if (!child.name.Contains("LOD"))
                    {
                        child.transform.parent = cargo;
                        child.gameObject.transform.parent = cargo.transform;

                        //AttachChildrenRecursive(cargo, child.transform, nextlevel);

                        //Game.Destroy(child);

                        //SaveableReference[] srs = child.GetComponents<SaveableReference>();
                        //for (var j = srs.Length; j-- > 0;)
                        //{
                        //    Debug.Log("Stranded Deep AlternativeEndgame Mod child object : destroying saveable reference " + sb.ToString() + srs[j].name);
                        //    BeamTeam.Destroy<SaveableReference>(srs[j]);
                        //}
                        //Debug.Log("Stranded Deep AlternativeEndgame Mod child object : destroying child " + sb.ToString() + child.name);
                        //BeamTeam.Destroy<GameObject>(child.gameObject);
                        //BeamTeam.Destroy<GameObject>(child);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Stranded Deep AlternativeEndgame Mod child object : destroying children failed for " + transform.name);
            }
        }

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach (string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            //if (tokens[0].Contains("revealWorld"))
                            //{
                            //    revealWorld = bool.Parse(tokens[1]);
                            //}
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
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine("revealWorld=" + revealWorld + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        #region speech

        public static void ShowSubtitles(IPlayer p, string text = "AE event", int delayInMs = 3000)
        {
            PlayerSpeech ps = p.PlayerSpeech;
            //ps.View
            ps.View.SubtitlesLabel.Text = text;
            ps.View.Show();
            Task.Delay(3000).ContinueWith(t => HideSubtitles(p));
        }

        static void HideSubtitles(IPlayer p)
        {
            PlayerSpeech ps = p.PlayerSpeech;
            ps.View.Hide();
        }

        #endregion
    }
}
