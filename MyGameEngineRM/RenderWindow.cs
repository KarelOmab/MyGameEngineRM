using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGameEngineRM
{
    public partial class RenderWindow : Form
    {

        //essential objects
        public GameLoop gameLoop;
        public Bitmap ScreenBuffer { get; set; }

        //render profiles
        public enum ScreenSize
        {
            SMALL,
            MEDIUM,
            LARGE,
            FULLSCREEN
        };

        //debug objects
        public bool ShowGridLines { get; set; }
        public const int GRID_DEFAULT_SQUARE_SIZE = 32;
        private List<Rectangle> listGridRectangles = new List<Rectangle>();
        public Color GridSquareColorBorder { get; set; }

        public RenderWindow()
        {
            InitializeComponent();
            ScreenBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);

        }

        public RenderWindow(string windowTitle = "", int screenWidth = 800, int screenHeight = 480)
        {
            InitializeComponent();
            Text = windowTitle;
            Width = screenWidth;
            Height = screenHeight;
            ScreenBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        public void Draw()
        {
            Screen.Image = ScreenBuffer;   //draw the backbuffer to the screen
        }

        public void Clear()
        {
            using (Graphics g = Graphics.FromImage(ScreenBuffer))
            {
                g.Clear(Color.Black);   //clear the backbuffer
            }
        }

        public void DrawGameObjects(GameObject[] objects)
        {
            using (Graphics g = Graphics.FromImage(ScreenBuffer))
            {
                g.Clear(Color.Black);   //clear the backbuffer

                if (ShowGridLines)
                    foreach (Rectangle gridRect in listGridRectangles)
                        g.DrawRectangle(new Pen(GridSquareColorBorder, 1), gridRect);


                //draw objects (background)
                foreach (GameObject o in objects)
                {
                    if (o.GetType() == typeof(GameText))
                    {
                        GameText gameText = o as GameText;
                        if (gameText.BrushBg != null)
                        {
                            if (gameText.Shape == GameObject.ObjectShape.Rectangle)
                                g.FillRectangle(gameText.BrushBg, gameText.RectF);
                            else if (gameText.Shape == GameObject.ObjectShape.Ellipse)
                                g.FillEllipse(gameText.BrushBg, gameText.RectF);
                        }


                        // Set format of string.
                        StringFormat drawFormat = new StringFormat();
                        drawFormat.Alignment = StringAlignment.Center;

                        g.DrawString(gameText.Text, gameText.Font, gameText.BrushFg, gameText.RectF, drawFormat);

                    }
                    else if (o.Texture != null)
                    {
                        if (o.Shape == GameObject.ObjectShape.Hexagon)
                            o.DrawImageInPolygon(g, o.Points, o.Texture);
                        else if (o.Shape == GameObject.ObjectShape.Rectangle)
                        {
                            if (o.RotAngle == 0)
                                g.DrawImage(o.Texture, o.RectF);
                            else g.DrawImage(o.TextureRot, o.RectF);
                        }
                            
                    }
                    else g.FillRectangle(o.BrushBg, o.RectF);
                }

                //draw highlights (foreground)
                foreach (GameObject o in objects)
                {
                    
                    
                    if (o.ShowHighlights)
                    {
                        if (o.Shape == GameObject.ObjectShape.Hexagon)
                            g.FillPolygon(new SolidBrush(Color.FromArgb(128, 242, 230, 162)), o.Points);
                        else if (o.Shape == GameObject.ObjectShape.Rectangle)
                            g.FillRectangle(new SolidBrush(Color.FromArgb(128, 242, 230, 162)), o.RectF);
                    }
                        

                    if (o.ShowBorders)
                        if (o.Shape == GameObject.ObjectShape.Hexagon)
                        {
                            g.DrawPolygon(Pens.Red, o.Points);
                        } else if (o.Shape == GameObject.ObjectShape.Rectangle)
                        {
                            g.DrawRectangle(Pens.Red, Rectangle.Truncate(o.RectF));
                        }


                    if (o.ShowVertices)
                    {
                        foreach (PointF p in o.Points)
                        {
                            g.DrawEllipse(new Pen(Color.White, 4), p.X - ((o.Radius / 4) / 2), p.Y - ((o.Radius / 4) / 2), o.Radius / 4, o.Radius / 4);
                        }
                    }

                    if (o.IsDragging)
                    {
                        //draw object at mouse point
                        if (o.Texture != null)
                        {
                            //Offset to center of texture
                            g.DrawImage(o.Texture, Screen.PointToClient(Cursor.Position).X - (o.Texture.Width / 2), Screen.PointToClient(Cursor.Position).Y - (o.Texture.Height / 2));
                        }
                    }
                }

            }

            Screen.Image = ScreenBuffer;   //draw the backbuffer to the screen
        }



        public void SetScreenSize(ScreenSize screenSize)
        {
            switch (screenSize)
            {
                case ScreenSize.SMALL:
                    Width = 640;
                    Height = 480;
                    break;
                case ScreenSize.MEDIUM:
                    Width = 960;
                    Height = 780;
                    break;
                case ScreenSize.LARGE:
                    Width = 1920;
                    Height = 1080;
                    break;
                case ScreenSize.FULLSCREEN:
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            CenterToScreen();
            ScreenBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        //debug functions
        public void GridGenerateBlocksBySize(Color lineColor, int blockSize = GRID_DEFAULT_SQUARE_SIZE)
        {
            if (blockSize == 0)
                if (Width > Height)
                    blockSize = Width / GRID_DEFAULT_SQUARE_SIZE; //default 32 squares per screen
                else blockSize = Height / GRID_DEFAULT_SQUARE_SIZE; //default 32 squares per screen

            int nGridSquaresH = Width / blockSize;
            int nGridSquaresV = Height / blockSize;

            for (int i = nGridSquaresH; i >= 0; i--)
            {
                for (int j = nGridSquaresV; j >= 0; j--)
                    listGridRectangles.Add(new Rectangle(i * blockSize, j * blockSize, blockSize, blockSize));
            }

            GridSquareColorBorder = lineColor;
        }

        public void GridGenerateColsRows(Color lineColor, int cols = 10, int rows = 10)
        {
            int w = ClientSize.Width / cols;
            int h = ClientSize.Height / rows;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                    listGridRectangles.Add(new Rectangle(i * w, j * h, w, h));
            }

            GridSquareColorBorder = lineColor;

        }
    }
}
