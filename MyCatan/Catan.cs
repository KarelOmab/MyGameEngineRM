using MyCatan.Objects;
using MyGameEngineRM;
using MyGameEngineRM.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCatan
{
    public class Catan : MyGameEngineRM.Game
    {
        List<GameObject> listObjects = new List<GameObject>();
        List<GameObject> listObjectsDrag = new List<GameObject>();



        public Catan(RenderWindow rWindow)
        {
            renderWindow = rWindow;
            //renderWindow.ShowGridLines = true;
            //renderWindow.GridGenerateColsRows(Color.White);
            renderWindow.Screen.MouseClick += renderWindow_MouseClicked;
            GameStart();
        }

        private void renderWindow_MouseClicked(object sender, MouseEventArgs e)
        {
            

            if (listObjectsDrag.Count == 1)
            {
                GameObject newObject = null;

                //handle drag and drop
                //We are only dragging and dropping new game objects
                newObject = listObjectsDrag[0].DeepCopy();

                //update x and y position temporarily to cur mouse position
                newObject.X = e.Location.X - (newObject.Width / 2);
                newObject.Y = e.Location.Y - (newObject.Height / 2);
                newObject.GetRect();

                //validate surface
                List<GameObject> listCircles = new List<GameObject>();
                listCircles.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Circle)));

                bool isValid = false;

                foreach(Circle c in listCircles)
                {
                    if (c.RectF.IntersectsWith(newObject.RectF))
                    {
                        isValid = true;

                        //lets also update rect
                        newObject.X = c.RectF.X - (c.Width / 2);
                        newObject.Y = c.RectF.Y - (c.Height / 2);
                        newObject.GetRect();
                        newObject.RotAngle = c.RotAngle;



                        break;
                    }
                }

                if (isValid)
                    listObjects.Add(newObject);
                
                listObjectsDrag.Clear();
                BuildableCirclesClear();

                foreach (GameObject go in listObjects.FindAll(x => x.IsDragging))
                    go.IsDragging = false;
                
                return;


            }


            bool isShowBuildableBuildings = false;
            bool isShowBuildableRoads = false;

            foreach (GameObject go in listObjects)
            {

                bool isColl = false;

                if (go.Shape == GameObject.ObjectShape.Hexagon)
                {
                    if (go.IsCollisionWithPointAndPolygon(e.Location))
                        isColl = true;
                } else if (go.Shape == GameObject.ObjectShape.Rectangle)
                {
                    if (go.IsCollisionWithPointAndRect(e.Location))
                        isColl = true;
                }

                if (isColl)
                {
                    if (go.GetType() == typeof(Settlement) || go.GetType() == typeof(City))
                    {
                        //IsShowTerrainVertices(true);    //show vertices(to place settlements/ cities)
                        isShowBuildableBuildings = true;
                    } else if (go.GetType() == typeof(Road))
                    {
                        isShowBuildableRoads = true;
                    }


                    if (go.IsDraggable)
                    {
                        go.IsDragging = true;
                        listObjectsDrag.Add(go);
                    }
                    
                    
                    
                } else

                {
                    go.IsDragging = false;
                }




            }

            if (isShowBuildableBuildings)
                BuildablesBuildingsShow();
            else if (isShowBuildableRoads)
                BuildablesRoadsShow();

        }

        /// <summary>
        /// Start game here
        /// </summary>
        private void GameStart()
        {
            renderWindow.gameLoop = new GameLoop(this);
            renderWindow.gameLoop.Start();
        }

        /// <summary>
        /// Update objects here
        /// </summary>
        public override void Update()
        {
            //foreach (GameObject o in listObjects)
            //{
            //    //o.MoveRight(1);
            //    //o.FadeOut(1);

            //    if (o.GetType() == typeof(BaseTerrain))
            //    {
            //        o.ShowVertices = true;
            //    }
            //}

        }

        /// <summary>
        /// Draw objects here
        /// </summary>
        public override void Draw()
        {

            renderWindow.DrawGameObjects(listObjects.OrderBy(o=>o.Z).ToArray());
        }

        /// <summary>
        /// Load game here
        /// </summary>
        public override void Load()
        {
            Console.WriteLine("Load game here...");

            LoadTerrainFromFile("resources\\map.txt");

            LoadPanels();

        }

        private void LoadTerrainFromFile(string file)
        {
            List<int> tileGrid = new List<int>() { 4, 5, 6, 7, 6, 5, 4 };
            int curRow = 0;
            int START_X = renderWindow.Width / 2;
            int POS_X = START_X;
            int POS_Y = renderWindow.Height / 5;

            // Read a text file line by line.
            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                string[] tiles = line.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < tiles.Length; i++)
                {
                    string tile = tiles[i];
                    string tileValue = string.Empty;
                    BaseTerrain t = new BaseTerrain();
                    GameText tVal = null;

                    if (i == 0)
                    {
                        //set starting X pos
                        POS_X = Convert.ToInt32(START_X - (tileGrid[curRow] * (0.875 * t.Radius)));
                    }

                    if (tile.Equals("W"))
                    {
                        t = new Water("Water", POS_X, POS_Y);

                    }
                    else if (tile.Equals("D"))
                    {
                        t = new Desert("Desert", POS_X, POS_Y);
                    }
                    else
                    {
                        //resource tile so we need to read the number now as well
                        string tileType = tile.Substring(0, 1);
                        tileValue = tile.Substring(1, tile.Length - 1);

                        switch (tileType)
                        {
                            case "L":
                                t = new Lumber("Lumber", POS_X, POS_Y);
                                break;
                            case "w":
                                t = new Wool("Wool", POS_X, POS_Y);
                                break;
                            case "G":
                                t = new Grain("Grain", POS_X, POS_Y);
                                break;
                            case "O":
                                t = new Ore("Ore", POS_X, POS_Y);
                                break;
                            case "B":
                                t = new Brick("Brick", POS_X, POS_Y);
                                break;
                        }
                    }



                    if (tileValue.Length > 0)
                    {

                        t.Value = tileValue;

                        //refactor below to object children
                        tVal = new GameText(renderWindow, t.Value, POS_X, POS_Y, t.BrushBg, t.BrushFg, GameObject.ObjectShape.Ellipse);
                        tVal.RectF = new RectangleF(POS_X - (tVal.RectF.Width / 2), POS_Y - (tVal.RectF.Height / 2), tVal.RectF.Width, tVal.RectF.Height);
                        tVal.Z = 3;

                        if (t.Value == "6" || t.Value == "8")
                            tVal.BrushFg = new SolidBrush(Color.FromArgb(255, 255, 0, 0));


                        listObjects.Add(t); //tile
                        listObjects.Add(tVal);  //tileValue 

                    }
                    else listObjects.Add(t);


                    //update point
                    POS_X += Convert.ToInt32(t.Radius + t.Radius * 0.75);

                    if (i == tiles.Length - 1)
                    {
                        curRow += 1;
                        POS_Y += Convert.ToInt32(t.Radius * 1.5156);
                    }
                }
            }

            
        }

        private void BuildablesBuildingsShow()
        {
            //create highlights for buildable points (white circles)
            List<GameObject> listHighlights = new List<GameObject>();
            List<GameObject> listTerrains = new List<GameObject>();
            listTerrains.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Lumber)));
            listTerrains.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Brick)));
            listTerrains.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Ore)));
            listTerrains.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Wool)));
            listTerrains.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Grain)));


            foreach (GameObject go in listTerrains)
            {
                foreach (PointF p in go.Points)
                {
                    Circle newVertex = new Circle(p.X - ((go.Radius / 4) / 2), p.Y - ((go.Radius / 4) / 2), go.Radius / 4, go.Radius / 4);

                    bool isColl = false;

                    foreach (Circle v in listHighlights)
                    {
                        if (v.RectF.IntersectsWith(newVertex.RectF))
                        {
                            isColl = true;
                            break;
                        }
                    }

                    if (!isColl || listHighlights.Count == 0)
                    {
                        //lets also verify that there are no other settlements nor cities on this point
                        List<GameObject> listBuildings = new List<GameObject>();
                        listBuildings.AddRange(listObjects.FindAll(x => x.GetType() == typeof(Settlement)));
                        listBuildings.AddRange(listObjects.FindAll(x => x.GetType() == typeof(City)));

                        foreach (GameObject b in listBuildings)
                        {
                            if (b.RectF.IntersectsWith(newVertex.RectF))
                            {
                                isColl = true;
                                break;
                            }
                        }

                        if (!isColl)
                        {
                            bool isAdj = false;

                            //ensure adjacency
                            foreach (GameObject existBuilding in listBuildings)
                            {
                                //find distance
                                double d = Math.Sqrt((Math.Pow(newVertex.X - existBuilding.X, 2) + Math.Pow(newVertex.Y - existBuilding.Y, 2)));

                                if (d <= go.Radius + newVertex.Width)
                                {
                                    isAdj = true;
                                    break;
                                }
                            }



                            if (!isAdj)
                                listHighlights.Add(newVertex);
                        }
                            

                        
                    }

                        

                }
            }

            listObjects.AddRange(listHighlights);
        }

        private void BuildablesRoadsShow()
        {
            //create highlights for buildable roads (white circles)
            List<GameObject> listHighlights = new List<GameObject>();

            List<GameObject> listTerrains = new List<GameObject>();  
            listTerrains.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Lumber)));
            listTerrains.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Brick)));
            listTerrains.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Ore)));
            listTerrains.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Wool)));
            listTerrains.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Grain)));

            foreach (GameObject go in listTerrains)
            {
                
                for (int i=0; i < go.Points.Length; i++)
                {

                    PointF a = new PointF();
                    PointF b = new PointF();
 
                    if (i == go.Points.Length-1)
                    {
                        a = go.Points[0];
                        b = go.Points[i];
                    } else
                    {
                        a = go.Points[i];
                        b = go.Points[i + 1];
                    }

                    
                    PointF m = new PointF((a.X + b.X) / 2, ((a.Y + b.Y) / 2));  //find the middle point between two points

                    Circle newVertex = new Circle(m.X - ((go.Radius / 4) / 2), m.Y - ((go.Radius / 4) / 2), go.Radius / 4, go.Radius / 4);

                    //ensure unique highlight creation
                    bool isColl = false;

                    foreach (Circle v in listHighlights)
                    {
                        if (v.RectF.IntersectsWith(newVertex.RectF))
                        {
                            isColl = true;
                            break;
                        }
                    }


                    if (!isColl || listHighlights.Count == 0)
                    {

                        List<GameObject> listBuildings = new List<GameObject>();
                        listBuildings.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Road)));

                        foreach (GameObject r in listBuildings)
                        {
                            //collision with another (existing) road
                            if (r.RectF.IntersectsWith(newVertex.RectF))
                            {
                                isColl = true;
                                break;
                            }
                        }

                        if (!isColl)
                        {
                            //finally lets also verify that that the point is adjacent to player owned object
                            listBuildings.AddRange(listObjects.FindAll(t => t.GetType() == typeof(Settlement) && t.Team == "Red"));
                            listBuildings.AddRange(listObjects.FindAll(t => t.GetType() == typeof(City) && t.Team == "Red"));

                            bool isAdj = false;

                            //ensure adjacency
                            foreach (GameObject existBuilding in listBuildings)
                            {
                                //find distance
                                double d = Math.Sqrt((Math.Pow(newVertex.X - existBuilding.X, 2) + Math.Pow(newVertex.Y - existBuilding.Y, 2)));

                                if (d <= go.Radius + (newVertex.Width / 2))
                                {
                                    isAdj = true;
                                    break;
                                }
                            }

                            if (isAdj)
                            {
                                float xDiff = b.X - a.X;
                                float yDiff = b.Y - a.Y;
                                double angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
                                newVertex.RotAngle = angle;
                                listHighlights.Add(newVertex);
                            }   
                        }
                            
                    }

                }
                
                
            }

            listObjects.AddRange(listHighlights);
        }

        private void BuildableCirclesClear()
        {
            listObjects.RemoveAll(x => x.GetType() == typeof(Circle));
        }

        private void LoadPanels()
        {
            PanelBoardPieces();
        }

        private void PanelBoardPieces()
        {
            List<GameObject> pieces = new List<GameObject>();
            listObjects.Add(new Settlement("Red", 5, 10));
            listObjects.Add(new City("Red", 35, 10));
            listObjects.Add(new Road("Red", 65, 10));


        }




    }
}
