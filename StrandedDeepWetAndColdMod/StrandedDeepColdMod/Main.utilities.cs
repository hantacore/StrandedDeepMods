using Beam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace StrandedDeepWetAndColdMod
{
    public static partial class Main
    {
        private static float FahrenheitToCelsius(float fahrenheit)
        {
            return (fahrenheit - 32f) * 5f / 9f;
        }

        private static float CelsiusToFahrenheit(float celsius)
        {
            return (celsius * 9f) / 5f + 32f;
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

        #region images instanciation

        private static int iconSize = 70;//50

        private static void InitWetnessMeter()
        {
            //Create your Image GameObject
            GameObject bgWetnessGO = new GameObject("Wetness_Background_Sprite");

            //Make the GameObject child of the Canvas
            bgWetnessGO.transform.SetParent(modCanvas.transform);
            Image imgBackground = bgWetnessGO.AddComponent<Image>();
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.drop-silhouette.png"));
            Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            imgBackground.sprite = bgSprite;
            //Center Image to screen
            bgWetnessGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.WETMETER_POSITION;


            //Create your Image GameObject
            GameObject bgWetnessGO2 = new GameObject("Wetness_Value_Sprite");

            //Make the GameObject child of the Canvas
            bgWetnessGO2.transform.SetParent(modCanvas.transform);
            wetMeterImage = bgWetnessGO2.AddComponent<Image>();
            wetMeterImage.type = Image.Type.Filled;
            wetMeterImage.fillMethod = Image.FillMethod.Vertical;
            wetMeterImage.fillOrigin = 0;
            wetMeterImage.fillAmount = 0;
            wetMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            wetMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage2 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage2.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.drop-silhouette_blue.png"));
            Sprite bgSprite2 = Sprite.Create(backgroundImage2, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            wetMeterImage.sprite = bgSprite2;
            //Center Image to screen
            bgWetnessGO2.GetComponent<RectTransform>().anchoredPosition = ParameterValues.WETMETER_POSITION;
        }

        private static void InitTemperatureMeter()
        {
            //Create your Image GameObject
            GameObject bgColdGO = new GameObject("Cold_Background_Sprite");

            //Make the GameObject child of the Canvas
            bgColdGO.transform.SetParent(modCanvas.transform);
            Image imgBackground = bgColdGO.AddComponent<Image>();
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.snowflake.png"));
            Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            imgBackground.sprite = bgSprite;
            //Center Image to screen
            bgColdGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.COLDMETER_POSITION;


            //Create your Image GameObject
            GameObject bgColdGO2 = new GameObject("Cold_Value_Sprite");

            //Make the GameObject child of the Canvas
            bgColdGO2.transform.SetParent(modCanvas.transform);
            coldMeterImage = bgColdGO2.AddComponent<Image>();
            coldMeterImage.type = Image.Type.Filled;
            coldMeterImage.fillMethod = Image.FillMethod.Vertical;
            coldMeterImage.fillOrigin = 1;
            coldMeterImage.fillAmount = 0;
            coldMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            coldMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage2 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage2.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.snowflake_blue.png"));
            Sprite bgSprite2 = Sprite.Create(backgroundImage2, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            coldMeterImage.sprite = bgSprite2;
            //Center Image to screen
            bgColdGO2.GetComponent<RectTransform>().anchoredPosition = ParameterValues.COLDMETER_POSITION;

            //Create your Image GameObject
            GameObject bgHotGO = new GameObject("Hot_Background_Sprite");

            //Make the GameObject child of the Canvas
            bgHotGO.transform.SetParent(modCanvas.transform);
            Image imgBackground3 = bgHotGO.AddComponent<Image>();
            imgBackground3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            imgBackground3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage3 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage3.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.burn.png"));
            Sprite bgSprite3 = Sprite.Create(backgroundImage3, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            imgBackground3.sprite = bgSprite3;
            //Center Image to screen
            bgHotGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.HOTMETER_POSITION;

            //Create your Image GameObject
            GameObject bgHotGO2 = new GameObject("Cold_Value_Sprite");

            //Make the GameObject child of the Canvas
            bgHotGO2.transform.SetParent(modCanvas.transform);
            hotMeterImage = bgHotGO2.AddComponent<Image>();
            hotMeterImage.type = Image.Type.Filled;
            hotMeterImage.fillMethod = Image.FillMethod.Vertical;
            hotMeterImage.fillOrigin = 0;
            hotMeterImage.fillAmount = 0;
            hotMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            hotMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage4 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage4.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.burn_orange.png"));
            Sprite bgSprite4 = Sprite.Create(backgroundImage4, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            hotMeterImage.sprite = bgSprite4;
            //Center Image to screen
            bgHotGO2.GetComponent<RectTransform>().anchoredPosition = ParameterValues.HOTMETER_POSITION;
        }

        private static void InitEnergyMeter()
        {
            //Create your Image GameObject
            GameObject bgEnergyGO = new GameObject("Energy_Background_Sprite");

            //Make the GameObject child of the Canvas
            bgEnergyGO.transform.SetParent(modCanvas.transform);
            Image imgBackground = bgEnergyGO.AddComponent<Image>();
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            imgBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.energy.png"));
            Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            imgBackground.sprite = bgSprite;
            //Center Image to screen
            bgEnergyGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.ENERGYMETER_POSITION;


            //Create your Image GameObject
            GameObject bgEnergyGO2 = new GameObject("Energy_Value_Sprite");

            //Make the GameObject child of the Canvas
            bgEnergyGO2.transform.SetParent(modCanvas.transform);
            energyMeterImage = bgEnergyGO2.AddComponent<Image>();
            energyMeterImage.type = Image.Type.Filled;
            energyMeterImage.fillMethod = Image.FillMethod.Vertical;
            energyMeterImage.fillOrigin = 0;
            energyMeterImage.fillAmount = 0;
            energyMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            energyMeterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage2 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage2.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.energy-yellow.png"));
            Sprite bgSprite2 = Sprite.Create(backgroundImage2, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            energyMeterImage.sprite = bgSprite2;
            //Center Image to screen
            bgEnergyGO2.GetComponent<RectTransform>().anchoredPosition = ParameterValues.ENERGYMETER_POSITION;
        }

        private static void InitStaticImages()
        {
            //Create your Image GameObject
            GameObject bgSimpleShelterGO = new GameObject("SimpleShelter_Sprite");

            //Make the GameObject child of the Canvas
            bgSimpleShelterGO.transform.SetParent(modCanvas.transform);
            simpleShelterImage = bgSimpleShelterGO.AddComponent<Image>();
            simpleShelterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            simpleShelterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.shelter.png"));
            Sprite bgSprite = Sprite.Create(backgroundImage, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            simpleShelterImage.sprite = bgSprite;
            //Center Image to screen
            bgSimpleShelterGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.SHELTERFLAG_POSITION;

            simpleShelterImage.enabled = false;


            //Create your Image GameObject
            GameObject bgHousingShelterGO = new GameObject("HousingShelter_Sprite");

            //Make the GameObject child of the Canvas
            bgHousingShelterGO.transform.SetParent(modCanvas.transform);
            housingShelterImage = bgHousingShelterGO.AddComponent<Image>();
            housingShelterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            housingShelterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage2 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage2.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.house.png"));
            Sprite bgSprite2 = Sprite.Create(backgroundImage2, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            housingShelterImage.sprite = bgSprite2;
            //Center Image to screen
            bgHousingShelterGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.HOUSINGFLAG_POSITION;

            housingShelterImage.enabled = false;


            //Create your Image GameObject
            GameObject bgHeatingGO = new GameObject("Heating_Sprite");

            //Make the GameObject child of the Canvas
            bgHeatingGO.transform.SetParent(modCanvas.transform);
            heatingImage = bgHeatingGO.AddComponent<Image>();
            heatingImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            heatingImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage3 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage3.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.heating.png"));
            Sprite bgSprite3 = Sprite.Create(backgroundImage3, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            heatingImage.sprite = bgSprite3;
            //Center Image to screen
            bgHeatingGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.HEATFLAG_POSITION;

            heatingImage.enabled = false;


            //Create your Image GameObject
            GameObject bgActivityGO = new GameObject("Activity_Sprite");

            //Make the GameObject child of the Canvas
            bgActivityGO.transform.SetParent(modCanvas.transform);
            activityImage = bgActivityGO.AddComponent<Image>();
            activityImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            activityImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage3_2 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage3_2.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.activity.png"));
            Sprite bgSprite3_2 = Sprite.Create(backgroundImage3_2, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            activityImage.sprite = bgSprite3_2;
            //Center Image to screen
            bgActivityGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.ACTIVITYFLAG_POSITION;

            activityImage.enabled = false;


            //Create your Image GameObject
            GameObject bgFeverOverlayGO = new GameObject("FeverOverlay_Sprite");

            //Make the GameObject child of the Canvas
            bgFeverOverlayGO.transform.SetParent(modCanvas.transform);
            feverOverlayImage = bgFeverOverlayGO.AddComponent<Image>();
            feverOverlayImage.raycastTarget = false;
            //feverOverlayImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, coldCanvasDefaultScreenWidth * 0.9f * screenRatioConversion);
            feverOverlayImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            //feverOverlayImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, coldCanvasDefaultScreenHeight * 0.9f);
            feverOverlayImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

            Texture2D backgroundImage4 = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
            backgroundImage4.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.fever_overlay.png"));
            Sprite bgSprite4 = Sprite.Create(backgroundImage4, new Rect(0, 0, 1920, 1080), new Vector2(860, 540));
            feverOverlayImage.sprite = bgSprite4;
            //Center Image to screen
            bgFeverOverlayGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);



            //Create your Image GameObject
            GameObject bgSweatingGO = new GameObject("Sweating_Sprite");

            //Make the GameObject child of the Canvas
            bgSweatingGO.transform.SetParent(modCanvas.transform);
            sweatingImage = bgSweatingGO.AddComponent<Image>();
            sweatingImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            sweatingImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage5 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage5.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.sweat.png"));
            Sprite bgSprite5 = Sprite.Create(backgroundImage5, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            sweatingImage.sprite = bgSprite5;
            //Center Image to screen
            bgSweatingGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.SWEATINDICATOR_POSITION;

            sweatingImage.enabled = false;


            //Create your Image GameObject
            GameObject bgCaloriesGO = new GameObject("BurningCalories_Sprite");

            //Make the GameObject child of the Canvas
            bgCaloriesGO.transform.SetParent(modCanvas.transform);
            caloriesImage = bgCaloriesGO.AddComponent<Image>();
            caloriesImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            caloriesImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);

            Texture2D backgroundImage6 = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            backgroundImage6.LoadImage(ExtractResource("StrandedDeepWetAndColdMod.assets.calories.png"));
            Sprite bgSprite6 = Sprite.Create(backgroundImage6, new Rect(0, 0, 512, 512), new Vector2(256, 256));
            caloriesImage.sprite = bgSprite6;
            //Center Image to screen
            bgCaloriesGO.GetComponent<RectTransform>().anchoredPosition = ParameterValues.CALORIESINDICATOR_POSITION;

            caloriesImage.enabled = false;
        }

        #endregion

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
            cvsl.referenceResolution = new Vector2(coldCanvasDefaultScreenWidth, coldCanvasDefaultScreenHeight);

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

        #region HandleConfig

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
                            if (tokens[0].Contains("hardFeverEffect"))
                            {
                                hardFeverEffect = bool.Parse(tokens[1]);
                            }
                            //else if (tokens[0].Contains("revealMissions"))
                            //{
                            //    revealMissions = bool.Parse(tokens[1]);
                            //}
                            //else if (tokens[0].Contains("debugMode"))
                            //{
                            //    debugMode = bool.Parse(tokens[1]);
                            //}
                            //if (tokens[0].Contains("viewDistance"))
                            //{
                            //    viewDistance = float.Parse(tokens[1]);
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
                //sb.AppendLine("viewDistance=" + viewDistance + ";");
                //sb.AppendLine("revealWorld=" + revealWorld + ";");
                //sb.AppendLine("revealMissions=" + revealMissions + ";");

                sb.AppendLine("hardFeverEffect=" + hardFeverEffect + ";");

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        #endregion

        #region speech

        public static void ShowSubtitles(IPlayer p, string text = "WnC event")
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
