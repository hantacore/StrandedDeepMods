using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StrandedDeepMapper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool debug = true;

        //private int internalworldsize = 3000;
        private int width = 10000;
        private int height = 10000;
        private int zoneXwidth = 600;//300;
        private int zoneYwidth = 600;//300;

        internal static float _zoneSize = 1000;//500f;
        internal static float _zoneSpacing = 1.25f;

        private int offsetX = 1000;
        private int offsetY = 1000;
        private int widthratio = 0;
        private int heightratio = 0;
        private int distancescalefactor = 2;
        private Point playerPosition;

        //private double startScale = 1;
        //private double scale = 1;
        private double zoomLevel = 1;

        private bool drawUndiscoveredIslands = false;
        private bool drawAnimals = false;
        private bool drawWreckages = false;
        private bool drawMineables = false;
        private bool drawItems = false;
        private bool drawSavepoints = true;
        private bool drawRaftMaterials = false;
        private bool drawFruits = false;
        private bool drawMedicine = false;
        private bool drawZoneNames = false;
        private bool drawBuildings = false;
        private bool drawMissions = false;

        private string savefileName = "";//@"C:\Users\Micha\AppData\LocalLow\Beam Team Games\Stranded Deep\Data\Slot0\data.json";

        //private static Vector2[] generationZonePositions;

        //private static Vector2[] generatedZonePoints;

        private static Vector2New[] generationZonePositionsNew;

        private static Vector2New[] generatedZonePointsNew;

        private dynamic saveGame = null;

        public MainWindow()
        {
            InitializeComponent();

            this.Title += " - " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            InitViewPort();
            SetTimer();
        }

        #region Images

        // ZONES
        BitmapImage imageUnknown = null;
        BitmapImage imageUnknownDebug = null;

        // SAVE
        BitmapImage imageSavepoint = null;

        // RAFT
        BitmapImage imageRaft = null;

        // ROCKS
        BitmapImage imageRock = null;
        BitmapImage imageRockCliff = null;
        BitmapImage imageRockDraw = null;

        // TREES
        BitmapImage imagePalm = null;
        BitmapImage imagePalmDraw = null;
        BitmapImage imagePlant = null;
        BitmapImage imagePlantMineable = null;
        BitmapImage imagePlantDraw = null;

        // WRECKS
        BitmapImage imagePlanewreck = null;
        BitmapImage imageShipwreck = null;

        // MINABLE RESOURCES
        BitmapImage imageMinable = null;

        // MISSIONS
        BitmapImage imageInterest = null;
        BitmapImage imageMissionEel = null;
        BitmapImage imageMissionSquid = null;
        BitmapImage imageMissionShark = null;
        BitmapImage imageMissionCarrier = null;

        // PLAYER
        BitmapImage imagePlayer = null;

        // ANIMALS
        BitmapImage imageShark = null;
        BitmapImage imageSharkRagdoll = null;

        BitmapImage imageCrabRagdoll = null;
        BitmapImage imageCrabSpawner = null;
        BitmapImage imageGiantCrabRagdoll = null;
        BitmapImage imageGiantCrabSpawner = null;

        BitmapImage imageHogRagdoll = null;
        BitmapImage imageHogSpawner = null;
        BitmapImage imageBoarRagdoll = null;
        BitmapImage imageBoarSpawner = null;

        BitmapImage imageSnakeRagdoll = null;
        BitmapImage imageSnakeSpawner = null;
        BitmapImage imageSnakeHidespot = null;

        BitmapImage imageStingray = null;
        BitmapImage imageWhale = null;
        BitmapImage imageMarlin = null;

        // FRUITS
        BitmapImage imageFruit = null;
        BitmapImage imageCoconut = null;
        BitmapImage imagePotato = null;
        BitmapImage imageYucca = null;

        // MEDICINE
        BitmapImage imageFlower = null;

        // BUILDINGS
        BitmapImage imageHut = null;
        BitmapImage imageWater = null;
        BitmapImage imageFire = null;
        BitmapImage imageIndustry = null;
        BitmapImage imageFoundation = null;
        BitmapImage imageContainer = null;

        // SEAFORTS
        BitmapImage imageSeafort = null;

        // ITEMS
        BitmapImage imageTool = null;
        BitmapImage imageItem = null;
        BitmapImage imagePlanks = null;
        BitmapImage imageCorrugated = null;
        BitmapImage imageCrate = null;

        // RAFT MATERIALS
        BitmapImage imageBarrel = null;
        BitmapImage imageBuoy = null;
        BitmapImage imageTire = null;

        private void InitIconsAndImages()
        {
            // unknown
            imageUnknown = new BitmapImage(new Uri("pack://application:,,,/icons/unknown.png"));

            // unknown debug
            imageUnknownDebug = new BitmapImage(new Uri("pack://application:,,,/icons/unknown_debug.png"));

            // SAVE
            imageSavepoint = new BitmapImage(new Uri("pack://application:,,,/icons/save.png"));

            // RAFT
            imageRaft = new BitmapImage(new Uri("pack://application:,,,/icons/raft.png"));

            // ROCKS
            imageRock = new BitmapImage(new Uri("pack://application:,,,/icons/rock.png"));
            imageRockCliff = new BitmapImage(new Uri("pack://application:,,,/icons/cliff.png"));
            imageRockDraw = new BitmapImage(new Uri("pack://application:,,,/icons/rock_draw.png"));

            // TREES
            imagePalm = new BitmapImage(new Uri("pack://application:,,,/icons/palmtree.png"));
            imagePalmDraw = new BitmapImage(new Uri("pack://application:,,,/icons/palmtree_draw.png"));
            imagePlant = new BitmapImage(new Uri("pack://application:,,,/icons/plant.png"));
            imagePlantMineable = new BitmapImage(new Uri("pack://application:,,,/icons/plant_mineable.png"));
            imagePlantDraw = new BitmapImage(new Uri("pack://application:,,,/icons/plant_draw.png"));

            // WRECKS
            imagePlanewreck = new BitmapImage(new Uri("pack://application:,,,/icons/planewreck.png"));
            imageShipwreck = new BitmapImage(new Uri("pack://application:,,,/icons/shipwreck.png"));

            // MINABLE RESOURCES
            imageMinable = new BitmapImage(new Uri("pack://application:,,,/icons/resource.png"));

            // MISSIONS
            imageInterest = new BitmapImage(new Uri("pack://application:,,,/icons/point-of-interest.png"));
            imageMissionEel = new BitmapImage(new Uri("pack://application:,,,/icons/mission_eel.png"));
            imageMissionSquid = new BitmapImage(new Uri("pack://application:,,,/icons/mission_squid.png"));
            imageMissionShark = new BitmapImage(new Uri("pack://application:,,,/icons/mission_shark.png"));
            imageMissionCarrier = new BitmapImage(new Uri("pack://application:,,,/icons/mission_carrier.png"));

            // PLAYER
            imagePlayer = new BitmapImage(new Uri("pack://application:,,,/icons/player.png"));

            // ANIMALS
            imageShark = new BitmapImage(new Uri("pack://application:,,,/icons/shark.png"));
            imageSharkRagdoll = new BitmapImage(new Uri("pack://application:,,,/icons/shark_ragdoll.png"));

            imageCrabRagdoll = new BitmapImage(new Uri("pack://application:,,,/icons/crab.png"));
            imageCrabSpawner = new BitmapImage(new Uri("pack://application:,,,/icons/crab_spawner.png"));
            imageGiantCrabRagdoll = new BitmapImage(new Uri("pack://application:,,,/icons/big_crab_ragdoll.png"));
            imageGiantCrabSpawner = new BitmapImage(new Uri("pack://application:,,,/icons/big_crab_spawner.png"));

            imageHogRagdoll = new BitmapImage(new Uri("pack://application:,,,/icons/hog_ragdoll.png"));
            imageHogSpawner = new BitmapImage(new Uri("pack://application:,,,/icons/hog_spawner.png"));
            imageBoarRagdoll = new BitmapImage(new Uri("pack://application:,,,/icons/boar.png"));
            imageBoarSpawner = new BitmapImage(new Uri("pack://application:,,,/icons/boar_spawner.png"));

            imageSnakeRagdoll = new BitmapImage(new Uri("pack://application:,,,/icons/snake_ragdoll.png"));
            imageSnakeSpawner = new BitmapImage(new Uri("pack://application:,,,/icons/snake_spawner.png"));
            imageSnakeHidespot = new BitmapImage(new Uri("pack://application:,,,/icons/snake_hide.png"));

            imageStingray = new BitmapImage(new Uri("pack://application:,,,/icons/stingray.png"));
            imageWhale = new BitmapImage(new Uri("pack://application:,,,/icons/whale.png"));
            imageMarlin = new BitmapImage(new Uri("pack://application:,,,/icons/marlin.png"));

            // FRUITS
            imageFruit = new BitmapImage(new Uri("pack://application:,,,/icons/fruit.png"));
            imageCoconut = new BitmapImage(new Uri("pack://application:,,,/icons/coconut.png"));
            imagePotato = new BitmapImage(new Uri("pack://application:,,,/icons/potato.png"));
            imageYucca = new BitmapImage(new Uri("pack://application:,,,/icons/yucca.png"));

            // MEDICINE
            imageFlower = new BitmapImage(new Uri("pack://application:,,,/icons/flower.png"));

            // BUILDINGS
            imageHut = new BitmapImage(new Uri("pack://application:,,,/icons/hut.png"));
            imageWater = new BitmapImage(new Uri("pack://application:,,,/icons/water.png"));
            imageFire = new BitmapImage(new Uri("pack://application:,,,/icons/fire.png"));
            imageIndustry = new BitmapImage(new Uri("pack://application:,,,/icons/industry.png"));
            imageFoundation = new BitmapImage(new Uri("pack://application:,,,/icons/foundation.png"));
            imageContainer = new BitmapImage(new Uri("pack://application:,,,/icons/container.png"));

            // SEAFORTS
            imageSeafort = new BitmapImage(new Uri("pack://application:,,,/icons/seafort.png"));

            // ITEMS
            imageTool = new BitmapImage(new Uri("pack://application:,,,/icons/tool.png"));
            imageItem = new BitmapImage(new Uri("pack://application:,,,/icons/item.png"));
            imagePlanks = new BitmapImage(new Uri("pack://application:,,,/icons/planks.png"));
            imageCorrugated = new BitmapImage(new Uri("pack://application:,,,/icons/corrugated.png"));
            imageCrate = new BitmapImage(new Uri("pack://application:,,,/icons/crate.png"));

            // RAFT MATERIALS
            imageBarrel = new BitmapImage(new Uri("pack://application:,,,/icons/barrel.png"));
            imageBuoy = new BitmapImage(new Uri("pack://application:,,,/icons/buoy.png"));
            imageTire = new BitmapImage(new Uri("pack://application:,,,/icons/tire.png"));
        }

        #endregion

        private void InitViewPort()
        {
            InitIconsAndImages();

            widthratio = (width - offsetX) / 3000;
            heightratio = (height - offsetY) / 3000;

            var transform = (MatrixTransform)mapCanvas.RenderTransform;
            var matrix = transform.Matrix;
            var scale = 0.15;

            matrix.ScaleAtPrepend(scale, scale, 0, 0);
            transform.Matrix = matrix;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var element = (UIElement)sender;
            var position = new Point(e.GetPosition(element).X + Mouse.GetPosition(this).X, e.GetPosition(element).Y + Mouse.GetPosition(this).Y);

            var transform = (MatrixTransform)element.RenderTransform;
            var matrix = transform.Matrix;
            var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor

            zoomLevel = zoomLevel * scale;

            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            transform.Matrix = matrix;

            tbZoom.Text = zoomLevel.ToString();
        }

        public bool IsMouseDown { get; set; }
        double xOffset;
        double yOffset;
        //Point initial;
        Point previous;

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            //initial = Mouse.GetPosition(this);
            previous = Mouse.GetPosition(this);
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsMouseDown) return;

            xOffset = e.GetPosition(this).X - previous.X;
            yOffset = e.GetPosition(this).Y - previous.Y;
            previous = Mouse.GetPosition(this);
            var element = (UIElement)sender;
            var position = e.GetPosition(element);
            var transform = (MatrixTransform)element.RenderTransform;
            var matrix = transform.Matrix;

            matrix.OffsetX += xOffset;
            matrix.OffsetY += yOffset;
            transform.Matrix = matrix;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
        }

        private void LoadSavegame(string filename)
        {
            tbFileName.Text = "";
            saveGame = null;

            if (System.IO.Path.GetExtension(filename) != ".json")
            {
                // boo boo
                tbFileName.Text = "Wrong file";
                return;
            }

            // read json
            // Parse map savegame file
            tbFileName.Text = filename;
            string json = File.ReadAllText(filename);
            saveGame = JsonConvert.DeserializeObject(json);
            lastUpdate = File.GetLastWriteTime(filename);
        }

        private void ClearCanvas()
        {
            mapCanvas.Children.Clear();
        }

        //public static void CreateWorldZonePoints(int seed)
        //{
        //    int num = 49;
        //    double num2 = 500f;
        //    double num3 = num2 * 1.25f * 7f;
        //    int numSamplesBeforeRejection = 30;
        //    //generatedZonePoints = ZonePositionGenerator.GeneratePoints(seed, num2, new Vector2(num3, num3), numSamplesBeforeRejection);
        //    generatedZonePoints = ZonePositionGenerator.GeneratePoints(seed, num2, new Vector2(num3, num3), numSamplesBeforeRejection);
        //    // debug
        //    //Console.WriteLine("##################");
        //    //foreach (Vector2 vector in _generatedZonePoints)
        //    //{
        //    //    Console.WriteLine(vector.x + "$" + vector.y);
        //    //}
        //    //Console.WriteLine("##################");

        //    // debug
        //    generationZonePositions = new Vector2[num];
        //    FastRandom fastRandom = new FastRandom(seed);
        //    List<int> list = new List<int>();
        //    int upperBound = generatedZonePoints.Length;
        //    //Console.WriteLine("##################");
        //    for (int i = 0; i < num; i++)
        //    {
        //        int num4 = fastRandom.Next(0, upperBound);
        //        while (list.Contains(num4))
        //        {
        //            num4 = fastRandom.Next(0, upperBound);
        //        }
        //        if (i == 0)
        //        {
        //            num4 = 0;
        //        }
        //        list.Add(num4);
        //        generationZonePositions[i] = generatedZonePoints[num4];
        //        //Console.WriteLine(_generationZonePositons[i].x + "$" + _generationZonePositons[i].y);
        //    }
        //    //Console.WriteLine("##################");
        //    if (generationZonePositions.Length < num - 1)
        //    {
        //        //Debug.LogError("Error: Not enough island positions");
        //    }
        //}

        // Token: 0x06004146 RID: 16710 RVA: 0x000D1340 File Offset: 0x000CF540
        public static void CreateWorldZonePointsNew(int seed)
        {
            int num = 49;
            float num2 = _zoneSize;
            float num3 = num2 * _zoneSpacing * 7f;
            int numSamplesBeforeRejection = 30;
            generatedZonePointsNew = ZonePositionGeneratorNew.GeneratePoints(seed, num2, new Vector2New(num3, num3), numSamplesBeforeRejection);
            generationZonePositionsNew = new Vector2New[num];
            FastRandomNew fastRandom = new FastRandomNew(seed);
            List<int> list = new List<int>();
            int upperBound = generatedZonePointsNew.Length;
            for (int i = 0; i < num; i++)
            {
                int num4 = fastRandom.Next(0, upperBound);
                while (list.Contains(num4))
                {
                    num4 = fastRandom.Next(0, upperBound);
                }
                if (i == 0)
                {
                    num4 = 0;
                }
                list.Add(num4);
                generationZonePositionsNew[i] = generatedZonePointsNew[num4];
            }
            if (generationZonePositionsNew.Length < num - 1)
            {
                Console.WriteLine("Error: Not enough island positions");
            }
        }

        private void DrawImage(BitmapImage source, double posx, double posy)
        {
            DrawImage(source, posx, posy, source.Width, source.Height);
        }

        private void DrawImage(BitmapImage source, double posx, double posy, string tooltip, int Zindex)
        {
            DrawImage(source, posx, posy, source.Width, source.Height, tooltip, Zindex);
        }

        private void DrawImage(BitmapImage source, double posx, double posy, double width, double height, string tooltip = null, int Zindex = 0)
        {
            Image image = new Image
            {
                Width = width,
                Height = height,
                Name = "",
                Source = new BitmapImage(new Uri(source.UriSource.ToString(), UriKind.RelativeOrAbsolute)),
                ToolTip = tooltip,
            };
            mapCanvas.Children.Add(image);
            Canvas.SetLeft(image, posx);
            Canvas.SetTop(image, posy);
            Canvas.SetZIndex(image, Zindex);
        }

        private void DrawMap(bool focusOnPlayer = false)
        {
            ClearCanvas();

            if (saveGame == null)
                return;

            string stringSeed = saveGame["Persistent"]["StrandedWorld"]["WorldSeed"];
            tbSeed.Text = stringSeed;
            CreateWorldZonePointsNew(int.Parse(stringSeed));

            var transform = (MatrixTransform)mapCanvas.RenderTransform;
            var matrix = transform.Matrix;

            // loop on zones
            for (int zoneIndex = 0; zoneIndex < generationZonePositionsNew.Length; zoneIndex++)
            {
                // get computed island coordinates
                double x, y;
                x = (width / 2) + generationZonePositionsNew[zoneIndex].x * distancescalefactor;
                y = (height / 2) - generationZonePositionsNew[zoneIndex].y * distancescalefactor;

                dynamic currentZone = saveGame.Persistent.StrandedWorld.Zones[zoneIndex];

                // debug : zone label
                if (drawZoneNames)
                {

                    int fontsize = 24;
                    int blockheight = 40;

                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ccffff"));
                    rect.StrokeThickness = 2;
                    rect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ccffff"));
                    rect.Width = 550;
                    rect.Height = blockheight;
                    Canvas.SetLeft(rect, x - zoneXwidth);
                    Canvas.SetTop(rect, y - zoneYwidth + 5);
                    mapCanvas.Children.Add(rect);

                    string fullname = currentZone.Id.ToString();
                    try
                    {
                        string nameString = currentZone.Name.ToString();
                        string[] parts = nameString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 3)
                        {
                            fullname = MapDictionaries.NamesDictionary[parts[0]] + " " + MapDictionaries.NamesDictionary[parts[1]] + " " + MapDictionaries.NamesDictionary[parts[2]];
                        }
                    }
                    catch (Exception e) { }

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = fullname;
                    textBlock.FontSize = fontsize;
                    //textBlock.Foreground = new SolidColorBrush(color);
                    Canvas.SetLeft(textBlock, x - zoneXwidth + 10);
                    Canvas.SetTop(textBlock, y - zoneYwidth + 10);
                    mapCanvas.Children.Add(textBlock);
                }

                // ? icon on island position
                if (currentZone["Discovered"] == null || bool.Parse(currentZone["Discovered"].ToString()) == false)
                {
                    if (drawUndiscoveredIslands)
                    {
                        double calcx = x;
                        double calcy = y;

                        if (drawZoneNames)
                        {
                            if ((debug || drawMissions) && currentZone["Id"].ToString().Contains("MISSION"))
                            {
                                double size = imageUnknownDebug.Width;

                                DrawImage(imageUnknownDebug, calcx - size / 2, calcy - size / 2, imageUnknownDebug.Width, imageUnknownDebug.Height, currentZone.Id.ToString() + " (" + currentZone.Name.ToString() + ")", 1000);
                            }
                            else
                            {
                                double size = imageUnknownDebug.Width;
                                DrawImage(imageUnknown, calcx - size / 2, calcy - size / 2, imageUnknownDebug.Width, imageUnknownDebug.Height, currentZone.Id.ToString() + " (" + currentZone.Name.ToString() + ")", 1000);
                            }
                        }
                        else
                        {
                            if ((debug || drawMissions) && currentZone["Id"].ToString().Contains("MISSION"))
                            {
                                double size = imageUnknownDebug.Width;

                                DrawImage(imageUnknownDebug, calcx - size / 2, calcy - size / 2);
                            }
                            else
                            {
                                double size = imageUnknownDebug.Width;
                                DrawImage(imageUnknown, calcx - size / 2, calcy - size / 2);
                            }
                        }

                        if (drawZoneNames)
                        {
                            // tooltip
                            //addTooltip(calcx, calcy, currentZone.Id + " (" + currentZone.Name + ")");
                        }
                    }
                }
                else
                {
                    #region WetSand

                    // Create a blue and a black Brush  
                    SolidColorBrush wetSandBrush = new SolidColorBrush();
                    wetSandBrush.Color = (Color)ColorConverter.ConvertFromString("#c2b280");
                    // Create a Polygon  
                    Polygon wetSandPolygon = new Polygon();
                    wetSandPolygon.Stroke = wetSandBrush;
                    wetSandPolygon.Fill = wetSandBrush;
                    wetSandPolygon.StrokeThickness = 50;//28;
                    wetSandPolygon.StrokeLineJoin = PenLineJoin.Round;
                    wetSandPolygon.StrokeStartLineCap = PenLineCap.Round;
                    wetSandPolygon.StrokeEndLineCap = PenLineCap.Round;

                    PointCollection polygonPoints = new PointCollection();

                    foreach (dynamic currentvalue in currentZone["Objects"])
                    {
                        string currentItemYstring = currentvalue["Transform"]["localPosition"]["y"].ToString();
                        double currentItemY = double.Parse(currentItemYstring);
                        //double currentItemY = double.Parse(currentItemYstring.Replace(",", "."));

                        string objectName = currentvalue["name"].ToString();
                        string key = objectName.Substring(0, objectName.IndexOf("(Clone)"));
                        //string key = objectName.Substring(objectName.IndexOf("]"), objectName.IndexOf("(Clone)"));

                        // island silhouette
                        if ((MapDictionaries.AllDictionary.ContainsKey(key)
                            && MapDictionaries.AllDictionary[key].Contains("DRAW")
                            && (MapDictionaries.AllDictionary[key].Contains("ROCK")
                                || MapDictionaries.AllDictionary[key].Contains("TREE")
                                || MapDictionaries.AllDictionary[key].Contains("PALM")
                                || MapDictionaries.AllDictionary[key].Contains("PLANT")
                                || MapDictionaries.AllDictionary[key].Contains("WRECK"))
                            //) && currentItemY <= -0.1 && currentItemY >= -0.8)
                            ) && currentItemY >= -0.8)
                        {
                            string currentItemXstring = currentvalue["Transform"]["localPosition"]["x"].ToString();
                            //double currentItemX = double.Parse(currentItemXstring.Replace(",", "."));
                            double currentItemX = double.Parse(currentItemXstring);
                            string currentItemZstring = currentvalue["Transform"]["localPosition"]["z"].ToString();
                            double currentItemZ = double.Parse(currentItemZstring);
                            //double currentItemZ = double.Parse(currentItemYstring.Replace(",", "."));

                            // calculated position
                            double calcx = x + currentItemX * (width / 1000 / widthratio);
                            double calcy = y - currentItemZ * (height / 1000 / heightratio);


                            System.Windows.Point Point1 = new System.Windows.Point(calcx, calcy);
                            polygonPoints.Add(Point1);
                        }
                    }
                    // Set Polygon.Points properties  
                    wetSandPolygon.Points = polygonPoints;
                    // Add Polygon to the page  
                    mapCanvas.Children.Add(wetSandPolygon);

                    #endregion

                    #region Island Silhouette

                    // Create a blue and a black Brush  
                    SolidColorBrush sandBrush = new SolidColorBrush();
                    sandBrush.Color = (Color)ColorConverter.ConvertFromString("#ede0b8");
                    // Create a Polygon  
                    Polygon sandPolygon = new Polygon();
                    sandPolygon.Stroke = sandBrush;
                    sandPolygon.Fill = sandBrush;
                    sandPolygon.StrokeThickness = 50;//28;
                    sandPolygon.StrokeLineJoin = PenLineJoin.Round;
                    sandPolygon.StrokeStartLineCap = PenLineCap.Round;
                    sandPolygon.StrokeEndLineCap = PenLineCap.Round;

                    polygonPoints = new PointCollection();

                    foreach (dynamic currentvalue in currentZone["Objects"])
                    {
                        string currentItemYstring = currentvalue["Transform"]["localPosition"]["y"].ToString();
                        double currentItemY = double.Parse(currentItemYstring);
                        //double currentItemY = double.Parse(currentItemYstring.Replace(",", "."));

                        string objectName = currentvalue["name"].ToString();
                        string key = objectName.Substring(0, objectName.IndexOf("(Clone)"));
                        //string key = objectName.Substring(objectName.IndexOf("]"), objectName.IndexOf("(Clone)"));

                        // island silhouette
                        if ((MapDictionaries.AllDictionary.ContainsKey(key)
                            && MapDictionaries.AllDictionary[key].Contains("DRAW")
                            && (MapDictionaries.AllDictionary[key].Contains("ROCK")
                                || MapDictionaries.AllDictionary[key].Contains("TREE")
                                || MapDictionaries.AllDictionary[key].Contains("PALM")
                                || MapDictionaries.AllDictionary[key].Contains("PLANT")
                                || MapDictionaries.AllDictionary[key].Contains("WRECK"))
                            //) && currentItemY <= -0.1 && currentItemY >= -0.8)
                            ) && currentItemY >= -0.1)
                        {
                            string currentItemXstring = currentvalue["Transform"]["localPosition"]["x"].ToString();
                            //double currentItemX = double.Parse(currentItemXstring.Replace(",", "."));
                            double currentItemX = double.Parse(currentItemXstring);
                            string currentItemZstring = currentvalue["Transform"]["localPosition"]["z"].ToString();
                            double currentItemZ = double.Parse(currentItemZstring);
                            //double currentItemZ = double.Parse(currentItemYstring.Replace(",", "."));

                            // calculated position
                            double calcx = x + currentItemX * (width / 1000 / widthratio);
                            double calcy = y - currentItemZ * (height / 1000 / heightratio);


                            System.Windows.Point Point1 = new System.Windows.Point(calcx, calcy);
                            polygonPoints.Add(Point1);
                        }
                    }
                    // Set Polygon.Points properties  
                    sandPolygon.Points = polygonPoints;
                    // Add Polygon to the page  
                    mapCanvas.Children.Add(sandPolygon);

                    #endregion

                    #region Draw items

                    foreach (dynamic currentvalue in currentZone["Objects"])
                    {
                        string currentItemXstring = currentvalue["Transform"]["localPosition"]["x"].ToString();
                        double currentItemX = double.Parse(currentItemXstring);
                        string currentItemYstring = currentvalue["Transform"]["localPosition"]["y"].ToString();
                        double currentItemY = double.Parse(currentItemYstring);
                        string currentItemZstring = currentvalue["Transform"]["localPosition"]["z"].ToString();
                        double currentItemZ = double.Parse(currentItemZstring);

                        // calculated position
                        double calcx = x + currentItemX * (width / 1000 / widthratio);
                        double calcy = y - currentItemZ * (height / 1000 / heightratio);

                        string objectName = currentvalue["name"].ToString();
                        string key = objectName.Substring(0, objectName.IndexOf("(Clone)"));

                        if (!MapDictionaries.AllDictionary.ContainsKey(key))
                        {
                            if (debug)
                            {
                                //alert("Key " + key + " not found in any dictionary, add it for mapping completion");
                                //writeLog("Key " + key + " not found in any dictionary");
                                Console.WriteLine("Key " + key + " not found in any dictionary");
                            }
                            //console.log(currentvalue["name"]);
                            //console.log("Key " + key + " not found in any dictionary, add it for mapping completion");
                            continue;
                        }

                        // draw rocks
                        if (MapDictionaries.AllDictionary.ContainsKey(key)
                            && MapDictionaries.AllDictionary[key].Contains("DRAW")
                            && MapDictionaries.AllDictionary[key].Contains("ROCK")
                            )
                        {
                            if (MapDictionaries.AllDictionary[key].Contains("CLIFF"))
                            {
                                DrawImage(imageRockCliff, calcx - 12, calcy - 12, 24, 24, null, 10000);
                            }
                            else
                            {
                                DrawImage(imageRock, calcx - 12, calcy - 12, 24, 24, null, 10000);
                            }
                        }

                        // draw trees
                        if (MapDictionaries.AllDictionary.ContainsKey(key)
                            && MapDictionaries.AllDictionary[key].Contains("DRAW")
                            && MapDictionaries.AllDictionary[key].Contains("TREE") || MapDictionaries.AllDictionary[key].Contains("PALM")
                            )
                        {
                            if (key.IndexOf("PALM") != -1)
                            {
                                DrawImage(imagePalm, calcx - 12, calcy - 12, 24, 24);
                            }
                            else if ((key.IndexOf("FICUS") != -1 || key.IndexOf("PINE") != -1) && MapDictionaries.AllDictionary[key].Contains("MINEABLE"))
                            {
                                DrawImage(imagePlantMineable, calcx - 8, calcy - 8, 16, 16, null, 20000);
                            }
                            else
                            {
                                DrawImage(imagePlantMineable, calcx - 8, calcy - 8, 16, 16, null, 20000);
                            }
                        }

                        // draw wreckages
                        if (drawWreckages)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && MapDictionaries.AllDictionary[key].Contains("WRECK")
                            )
                            {
                                if (key.IndexOf("PLANEWRECK") != -1)
                                {
                                    DrawImage(imagePlanewreck, calcx - 18, calcy - 18, 36, 36, null, 30000);
                                }
                                else
                                {
                                    DrawImage(imageShipwreck, calcx - 18, calcy - 18, 36, 36, null, 30000);
                                }
                            }
                        }

                        // draw missions
                        if (MapDictionaries.AllDictionary.ContainsKey(key)
                            && MapDictionaries.AllDictionary[key].Contains("DRAW")
                            && MapDictionaries.AllDictionary[key].Contains("MISSION")
                        )
                        {
                            double size = 50;
                            if (MapDictionaries.AllDictionary[key].IndexOf("EEL") != -1)
                            {
                                if (drawMissions)
                                {
                                    DrawImage(imageMissionEel, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                                else
                                {
                                    DrawImage(imageInterest, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                            }
                            if (MapDictionaries.AllDictionary[key].IndexOf("SQUID") != -1)
                            {
                                if (drawMissions)
                                {
                                    DrawImage(imageMissionSquid, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                                else
                                {
                                    DrawImage(imageInterest, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                            }
                            if (MapDictionaries.AllDictionary[key].IndexOf("MEG") != -1)
                            {
                                if (drawMissions)
                                {
                                    DrawImage(imageMissionShark, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                                else
                                {
                                    DrawImage(imageInterest, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                            }
                            if (MapDictionaries.AllDictionary[key].IndexOf("CARRIER") != -1)
                            {
                                if (drawMissions)
                                {
                                    DrawImage(imageMissionCarrier, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                                else
                                {
                                    DrawImage(imageInterest, calcx - size / 2, calcy - size / 2, null, 40000);
                                }
                            }
                        }

                        // draw animals					
                        if (drawAnimals)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && MapDictionaries.AllDictionary[key].Contains("SHARK")
                                )
                            {
                                if (key.IndexOf("RAGDOLL") != -1)
                                {
                                    DrawImage(imageSharkRagdoll, calcx - 12, calcy - 12, 24, 24, null, 40000);
                                }
                                else if (key.IndexOf("PATROL") != -1)
                                {
                                    DrawImage(imageShark, calcx - 12, calcy - 12, 24, 24, key + " counter : " + currentvalue["respawnCounter"] + " / distance : " + currentvalue["spawnDistance"], 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key + " counter : " + currentvalue["respawnCounter"] + " / distance : " + currentvalue["spawnDistance"]);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("STINGRAY"))
                                )
                            {
                                if (key.IndexOf("RAY") != -1)
                                {
                                    DrawImage(imageStingray, calcx - 8, calcy - 8, 16, 16, null, 40000);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("WHALE"))
                                )
                            {
                                if (key.IndexOf("WHALE") != -1)
                                {
                                    DrawImage(imageWhale, calcx - 8, calcy - 8, 16, 16, null, 40000);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("MARLIN"))
                                )
                            {
                                if (key.IndexOf("MARLIN") != -1)
                                {
                                    DrawImage(imageMarlin, calcx - 8, calcy - 8, 16, 16, null, 40000);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("CRAB"))
                                )
                            {
                                // big crabs
                                if (key.IndexOf("GIANT_CRAB_RAGDOLL_RAGDOLL") != -1)
                                {
                                    DrawImage(imageGiantCrabRagdoll, calcx - 12, calcy - 12, 24, 24, null, 40000);
                                }
                                else if (key.IndexOf("GIANT_CRAB_SPAWNER_SPAWNER") != -1)
                                {
                                    DrawImage(imageGiantCrabSpawner, calcx - 12, calcy - 12, 24, 24, key, 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }

                                // crabs					
                                else if (key.IndexOf("CRAB_RAGDOLL") != -1)
                                {
                                    DrawImage(imageCrabRagdoll, calcx - 12, calcy - 12, 24, 24, null, 40000);
                                }
                                else if (key.IndexOf("CRAB_SPAWNER") != -1)
                                {
                                    DrawImage(imageCrabSpawner, calcx - 12, calcy - 12, 24, 24, key, 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("BOAR"))
                                )
                            {
                                // hogs
                                if (key.IndexOf("HOG_RAGDOLL") != -1)
                                {
                                    DrawImage(imageHogRagdoll, calcx - 12, calcy - 12, 24, 24, null, 40000);
                                }
                                else if (key.IndexOf("HOG_SPAWNER") != -1)
                                {
                                    DrawImage(imageHogSpawner, calcx - 12, calcy - 12, 24, 24, key, 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }

                                // boars					
                                else if (key.IndexOf("BOAR_RAGDOLL") != -1)
                                {
                                    DrawImage(imageBoarRagdoll, calcx - 12, calcy - 12, 24, 24, null, 40000);
                                }
                                else if (key.IndexOf("BOAR_SPAWNER") != -1)
                                {
                                    DrawImage(imageBoarSpawner, calcx - 12, calcy - 12, 24, 24, key, 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("SNAKE"))
                                )
                            {
                                if (key.IndexOf("SNAKE_RAGDOLL") != -1)
                                {
                                    DrawImage(imageSnakeRagdoll, calcx - 12, calcy - 12, 24, 24, null, 40000);
                                }
                                else if (key.IndexOf("SNAKE_SPAWNER") != -1)
                                {
                                    DrawImage(imageSnakeSpawner, calcx - 12, calcy - 12, 24, 24, key, 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                                else
                                {
                                    DrawImage(imageSnakeHidespot, calcx - 12, calcy - 12, 24, 24, key, 40000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                            }
                        }

                        // draw minable resources
                        if (drawMineables)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("RESOURCE")))
                            {
                                if (key.IndexOf("MINING") != -1 || key.IndexOf("PILE") != -1)
                                {
                                    DrawImage(imageMinable, calcx - 12, calcy - 12, 24, 24, key, 50000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                            }
                        }

                        if (drawItems)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                    && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                    && (MapDictionaries.AllDictionary[key].Contains("TOOL"))
                                )
                            {
                                DrawImage(imageTool, calcx - 10, calcy - 10, 20, 20, key, 50000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                    && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                    && (MapDictionaries.AllDictionary[key].Contains("RESOURCE"))
                                )
                            {
                                if (key.IndexOf("MINING") == -1 && key.IndexOf("PILE") == -1)
                                {
                                    if (key.IndexOf("PLANK") != -1)
                                    {
                                        DrawImage(imagePlanks, calcx - 10, calcy - 10, 20, 20, key, 50000);
                                    }
                                    else if (key.IndexOf("CORRUGATED") != -1)
                                    {
                                        DrawImage(imageCorrugated, calcx - 10, calcy - 10, 20, 20, key, 50000);
                                    }

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                    && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                    && (MapDictionaries.AllDictionary[key].Contains("ITEM"))
                                )
                            {
                                DrawImage(imageItem, calcx - 6, calcy - 6, 12, 12, key, 50000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                    && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                    && (MapDictionaries.AllDictionary[key].Contains("CRATE"))
                                )
                            {

                                if (currentItemY >= 0
                                    || drawWreckages)
                                {
                                    DrawImage(imageCrate, calcx - 6, calcy - 6, 12, 12, key, 50000);

                                    // tooltip
                                    //addTooltip(calcx, calcy, key);
                                }
                            }
                        }

                        if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("RAFT"))
                            )
                        {
                            if (key.Contains("STRUCTURE_RAFT") && currentvalue["RigidBody"] != null)
                            {
                                string currentStructureXstring = currentvalue["RigidBody"]["position"]["x"].ToString();
                                double currentStructureX = double.Parse(currentStructureXstring);
                                string currentStructureYstring = currentvalue["RigidBody"]["position"]["y"].ToString();
                                double currentStructureY = double.Parse(currentStructureYstring);
                                string currentStructureZstring = currentvalue["RigidBody"]["position"]["z"].ToString();
                                double currentStructureZ = double.Parse(currentStructureZstring);

                                // calculated position
                                calcx = x + currentStructureX * (width / 1000 / widthratio);
                                calcy = y - currentStructureZ * (height / 1000 / heightratio);

                                double size = 24;
                                DrawImage(imageRaft, calcx - 10, calcy - 10, size, size, null, 60000);
                            }
                            else
                            {
                                double size = 24;
                                DrawImage(imageRaft, calcx - 10, calcy - 10, size, size, null, 60000);
                            }
                        }

                        // draw saving points
                        if (drawSavepoints)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("SAVE"))
                                )
                            {
                                double size = 24;
                                DrawImage(imageSavepoint, calcx - size / 2, calcy - size / 2, size, size, null, 100000);

                                // if shelter has label, show island name
                                if (currentvalue["dName"] != null)
                                {
                                    int fontsize = 24;
                                    int blockheight = 40;

                                    Rectangle rect = new Rectangle();
                                    rect.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ccffff"));
                                    rect.StrokeThickness = 2;
                                    rect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ccffff"));
                                    rect.Width = 550;
                                    rect.Height = blockheight;
                                    Canvas.SetLeft(rect, x - zoneXwidth);
                                    Canvas.SetTop(rect, y - zoneYwidth + 5);
                                    mapCanvas.Children.Add(rect);

                                    string fullname = currentvalue["dName"].ToString();

                                    TextBlock textBlock = new TextBlock();
                                    textBlock.Text = fullname;
                                    textBlock.FontSize = fontsize;
                                    //textBlock.Foreground = new SolidColorBrush(color);
                                    Canvas.SetLeft(textBlock, x - zoneXwidth + 10);
                                    Canvas.SetTop(textBlock, y - zoneYwidth + 10);
                                    mapCanvas.Children.Add(textBlock);
                                }
                            }
                        }

                        if (drawRaftMaterials)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("BARREL"))
                                )
                            {
                                DrawImage(imageBarrel, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("BUOY"))
                                )
                            {
                                DrawImage(imageBuoy, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("TIRE"))
                                )
                            {
                                DrawImage(imageTire, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }
                        }

                        // draw fruits
                        if (drawFruits)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("FRUIT"))
                                )
                            {
                                DrawImage(imageFruit, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("COCONUT"))
                                )
                            {
                                DrawImage(imageCoconut, calcx - 6, calcy - 6, 12, 12, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("POTATO"))
                                )
                            {
                                DrawImage(imagePotato, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("YUCCA")))
                            {
                                DrawImage(imageYucca, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }
                        }

                        // draw flowers
                        if (drawMedicine)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("FLOWER"))
                                )
                            {
                                DrawImage(imageFlower, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }
                        }

                        // draw buildings
                        if (drawBuildings)
                        {
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("SHELTER"))
                                )
                            {
                                DrawImage(imageHut, calcx - 12, calcy - 12, 24, 24, key, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("INDUSTRY"))
                                )
                            {
                                if (currentvalue["Parent"] != null)
                                {
                                    string currentStructureXstring = currentvalue["Parent"]["Transform"]["localPosition"]["x"].ToString();
                                    double currentStructureX = double.Parse(currentStructureXstring);
                                    string currentStructureYstring = currentvalue["Parent"]["Transform"]["localPosition"]["y"].ToString();
                                    double currentStructureY = double.Parse(currentStructureYstring);
                                    string currentStructureZstring = currentvalue["Parent"]["Transform"]["localPosition"]["z"].ToString();
                                    double currentStructureZ = double.Parse(currentStructureZstring);

                                    // calculated position
                                    calcx = x + currentStructureX * (width / 1000 / widthratio);
                                    calcy = y - currentStructureZ * (height / 1000 / heightratio);
                                }


                                if (MapDictionaries.AllDictionary[key].Contains("WATER"))
                                {
                                    DrawImage(imageWater, calcx - 12, calcy - 12, 24, 24, key, 40000);
                                }
                                else if (MapDictionaries.AllDictionary[key].Contains("FIRE"))
                                {
                                    DrawImage(imageFire, calcx - 12, calcy - 12, 24, 24, key, 40000);
                                }
                                else
                                {
                                    DrawImage(imageIndustry, calcx - 12, calcy - 12, 24, 24, key, 40000);
                                }

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("SEAFORT"))
                                )
                            {
                                DrawImage(imageSeafort, calcx - 12, calcy - 12, 24, 24, null, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }

                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("STRUCTURE"))
                                )
                            {
                                DrawImage(imageFoundation, calcx - 12, calcy - 12, 24, 24, null, 60000);

                                //// contructions list
                                //$.each(currentvalue["Constructions"], function(name, currentConstruction) {
                                //	// loop on references
                                //	$.each(currentZone.Objects, function(name, currentconstructiontolookfor) {
                                //                                 // shitty inner loop to get the sub elements
                                //                                 if (currentConstruction == currentconstructiontolookfor["reference"])
                                //                                 {
                                //                                     let currentSubItemX = Number(String(currentconstructiontolookfor["Transform"]["localPosition"]["x"]).replace(",", "."));
                                //                                     let currentSubItemZ = Number(String(currentconstructiontolookfor["Transform"]["localPosition"]["z"]).replace(",", "."));

                                //                                     let subcalcx = calcx + Number(currentSubItemX);
                                //                                     let subcalcy = calcy + Number(currentSubItemZ);

                                //                                     DrawImage(imageFoundation, subcalcx - 12, subcalcy - 12, 24, 24);
                                //                                 }
                                //                             });
                                //                     });
                            }

                            // draw containers
                            if (MapDictionaries.AllDictionary.ContainsKey(key)
                                && MapDictionaries.AllDictionary[key].Contains("DRAW")
                                && (MapDictionaries.AllDictionary[key].Contains("CONTAINER"))
                            )
                            {
                                DrawImage(imageContainer, calcx - 12, calcy - 12, 24, 24, null, 60000);

                                // tooltip
                                //addTooltip(calcx, calcy, key);
                            }
                        }
                    }

                    #endregion

                    #region Draw player

                    // draw player 1 position
                    if ((saveGame.Persistent.StrandedWorld.PlayersData.Count == 1 || saveGame.Persistent.StrandedWorld.PlayersData.Count == 2))
                    {
                        // calculated position
                        string playerPositionXstring = saveGame.Persistent.StrandedWorld.PlayersData[0].Position["x"].ToString();
                        double calcx = (width / 2) + double.Parse(playerPositionXstring) * distancescalefactor;// local position conversion from center of island
                        string playerPositionZstring = saveGame.Persistent.StrandedWorld.PlayersData[0].Position["z"].ToString();
                        double calcy = (height / 2) - double.Parse(playerPositionZstring) * distancescalefactor;// local position conversion from center of island

                        double size = 36;
                        DrawImage(imagePlayer, calcx - size / 2, calcy - size / 2, size, size, "Yeah, that\'s you", 100000);

                        playerPosition = new Point(calcx, calcy);

                        if (focusOnPlayer)
                        {
                            Grid parent = mapCanvas.Parent as Grid;
                            double viewportWidth = parent.ActualWidth;
                            double viewportHeight = parent.ActualHeight;
                            Point viewportCenter = new Point(viewportWidth / 2, viewportHeight / 2);

                            MatrixTransform localtransform = (MatrixTransform)mapCanvas.RenderTransform;
                            Matrix localmatrix = localtransform.Matrix;

                            double currentheight = mapCanvas.ActualHeight * (mapCanvas.RenderTransform.Value.M11);
                            double currentwidth = mapCanvas.ActualWidth * (mapCanvas.RenderTransform.Value.M22);

                            double scaledCalcX = calcx * (mapCanvas.RenderTransform.Value.M11);
                            double scaledCalcY = calcy * (mapCanvas.RenderTransform.Value.M22);

                            double mapoffsetX = viewportCenter.X - scaledCalcX;
                            double mapoffsetY = viewportCenter.Y - scaledCalcY;

                            localmatrix.OffsetX = mapoffsetX;
                            localmatrix.OffsetY = mapoffsetY;
                            localtransform.Matrix = localmatrix;
                        }
                    }

                    // draw player 2 position
                    if (saveGame.Persistent.StrandedWorld.PlayersData.Count == 2)
                    {
                        // calculated position
                        string playerPositionXstring = saveGame.Persistent.StrandedWorld.PlayersData[1].Position["x"].ToString();
                        double calcx = (width / 2) + double.Parse(playerPositionXstring) * distancescalefactor;// local position conversion from center of island
                        string playerPositionZstring = saveGame.Persistent.StrandedWorld.PlayersData[1].Position["z"].ToString();
                        double calcy = (height / 2) - double.Parse(playerPositionZstring) * distancescalefactor;// local position conversion from center of island

                        double size = 36;
                        DrawImage(imagePlayer, calcx - size / 2, calcy - size / 2, size, size, "Yeah, that\'s your friend", 100000);
                    }

                    #endregion
                }
            }

            mapCanvas.InvalidateVisual();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // openfile dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            DialogResult result = ofd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
            {
                savefileName = ofd.FileName;
                // get file name
                LoadSavegame(savefileName);
                playerPosition = new Point();
                DrawMap();
            }
        }

        private void UpdateFlags()
        {
            if (checkItems == null)
                return;

            drawItems = checkItems.IsChecked.GetValueOrDefault(false);

            drawAnimals = checkAnimals.IsChecked.GetValueOrDefault(false);

            drawWreckages = checkWreckages.IsChecked.GetValueOrDefault(false);

            drawMineables = checkMineables.IsChecked.GetValueOrDefault(false);

            drawSavepoints = checkSavePoints.IsChecked.GetValueOrDefault(false);

            drawRaftMaterials = checkRaftMaterials.IsChecked.GetValueOrDefault(false);

            drawFruits = checkFruits.IsChecked.GetValueOrDefault(false);

            drawMedicine = checkMedicine.IsChecked.GetValueOrDefault(false);

            drawZoneNames = checkZoneNames.IsChecked.GetValueOrDefault(false);

            drawBuildings = checkBuildings.IsChecked.GetValueOrDefault(false);

            debug = checkDebug.IsChecked.GetValueOrDefault(false);

            drawMissions = checkMissions.IsChecked.GetValueOrDefault(false);

            drawUndiscoveredIslands = checkWorld.IsChecked.GetValueOrDefault(false);

            DrawMap();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFlags();
        }

        private void FindPlayer_Click(object sender, RoutedEventArgs e)
        {
            Grid parent = mapCanvas.Parent as Grid;
            double viewportWidth = parent.ActualWidth;
            double viewportHeight = parent.ActualHeight;
            Point viewportCenter = new Point(viewportWidth / 2, viewportHeight / 2);

            MatrixTransform localtransform = (MatrixTransform)mapCanvas.RenderTransform;
            Matrix localmatrix = localtransform.Matrix;

            double currentheight = mapCanvas.ActualHeight * (mapCanvas.RenderTransform.Value.M11);
            double currentwidth = mapCanvas.ActualWidth * (mapCanvas.RenderTransform.Value.M22);

            double scaledCalcX = playerPosition.X * (mapCanvas.RenderTransform.Value.M11);
            double scaledCalcY = playerPosition.Y * (mapCanvas.RenderTransform.Value.M22);

            double mapoffsetX = viewportCenter.X - scaledCalcX;
            double mapoffsetY = viewportCenter.Y - scaledCalcY;

            localmatrix.OffsetX = mapoffsetX;
            localmatrix.OffsetY = mapoffsetY;
            localtransform.Matrix = localmatrix;
        }

        private void ResetView_Click(object sender, RoutedEventArgs e)
        {
            MatrixTransform transform = mapCanvas.RenderTransform as MatrixTransform;
            Matrix m = transform.Matrix;
            m.M11 = 0.25;
            m.M22 = 0.25;

            Grid parent = mapCanvas.Parent as Grid;
            double viewportWidth = parent.ActualWidth;
            double viewportHeight = parent.ActualHeight;
            Point viewportCenter = new Point(viewportWidth / 2, viewportHeight / 2);

            double currentheight = mapCanvas.ActualHeight * (m.M11);
            double currentwidth = mapCanvas.ActualWidth * (m.M22);

            m.OffsetX = viewportCenter.X - currentwidth / 2;
            m.OffsetY = viewportCenter.Y - currentheight / 2;

            transform.Matrix = m;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MapMakerToolControl.Height = MapMakerToolControl.Parent.Height;
            MapMakerToolControl.Width = MapMakerToolControl.Parent.Width;
        }

        #region Autorefresh

        private DateTime lastUpdate = DateTime.MinValue;

        //Set and start the timer
        private void SetTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();
        }

        //Refreshes grid data on timer tick
        protected void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (checkAutorefresh.IsChecked.GetValueOrDefault(false))
            {
                try
                {
                    string filename = tbFileName.Text;
                    if (!String.IsNullOrEmpty(filename) && File.Exists(filename))
                    {
                        DateTime newLastUpdate = File.GetLastWriteTime(filename);
                        if (newLastUpdate > lastUpdate)
                        {
                            LoadSavegame(filename);
                            DrawMap(checkAutoCenter.IsChecked.GetValueOrDefault(false));
                        }
                    }
                }
                catch { }
            }
        }

        #endregion
    }
}
