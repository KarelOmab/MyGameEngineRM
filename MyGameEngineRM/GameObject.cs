using MyGameEngineRM.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngineRM
{
    public class GameObject
    {

        public enum ObjectShape
        {
            Rectangle,
            Ellipse,
            Hexagon
        }

        public List<GameObject> ObjectChildren = new List<GameObject>();
        public string Team { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Radius { get; set; }
        public ObjectShape Shape { get; set; }
        public RectangleF RectF { get; set; }
        public PointF[] Points { get; set; }
        public bool ShowBorders { get; set; }
        public bool ShowHighlights { get; set; }
        public bool ShowVertices { get; set; }
        public Image Texture { get; set; }
        public Image TextureRot { get; set; }
        public SolidBrush BrushBg { get; set; }
        public SolidBrush BrushFg { get; set; }
        public ScreenAlignment Alignment { get; set; }
        public bool IsDraggable { get; set; }
        public bool IsDragging { get; set; }
        public bool IsFrozen { get; set; }
        private double _rotAngle { get; set; }
        public double RotAngle { 
            get { return _rotAngle; }
            set {
                _rotAngle = value;

                if (Texture != null)
                {
                    TextureRot = RotateImage((Bitmap)Texture, (float)value);
                }
            } 
        }

        public void GetRect()
        {
            RectF = new RectangleF(X, Y, Width, Height);
        }

        public void GetVertices()
        {
            if (Shape == ObjectShape.Rectangle)
            {
                if (RectF.X == 0 && RectF.Y == 0 && RectF.Width == 0 && RectF.Height == 0)
                    GetRect();

                Points[0] = new PointF(RectF.Left,RectF.Top);
                Points[1] = new PointF(RectF.Right, RectF.Top);
                Points[2] = new PointF(RectF.Left, RectF.Bottom);
                Points[3] = new PointF(RectF.Right, RectF.Bottom);

            }
            else if (Shape == ObjectShape.Ellipse)
            {
                Points[0] = new PointF(RectF.Left, RectF.Top);
                Points[1] = new PointF(RectF.Right, RectF.Top);
                Points[2] = new PointF(RectF.Left, RectF.Bottom);
                Points[3] = new PointF(RectF.Right, RectF.Bottom);
            }
            else if (Shape == ObjectShape.Hexagon)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i] = new PointF(
                        X + Radius * (float)Math.Sin(i * 60 * Math.PI / 180f),
                        Y + Radius * (float)Math.Cos(i * 60 * Math.PI / 180f));
                }
            }


        }

        private Image RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }

        public virtual void Render()
        {
            
        }

        public void DrawImageInPolygon(Graphics gr, PointF[] points, Image image)
        {
            // Get the polygon's bounds and center.
            float xmin, xmax, ymin, ymax;
            GetPolygonBounds(out xmin, out xmax, out ymin, out ymax);
            float wid = xmax - xmin;
            float hgt = ymax - ymin;
            float cx = (xmin + xmax) / 2f;
            float cy = (ymin + ymax) / 2f;

            // Calculate the scale needed to make
            // the image fill the polygon's bounds.
            float xscale = wid / image.Width;
            float yscale = hgt / image.Height;
            float scale = Math.Max(xscale, yscale);

            // Calculate the image's scaled size.
            float width = image.Width * scale;
            float height = image.Height * scale;
            float rx = width / 2f;
            float ry = height / 2f;


            if (Shape == GameObject.ObjectShape.Hexagon)
            {
                //draw polygon
                // Find the source rectangle and destination points.
                RectangleF src_rect = new RectangleF(0, 0, image.Width, image.Height);
                PointF[] dest_points =
                {
                new PointF(cx - rx,  cy - ry),
                new PointF(cx + rx,  cy - ry),
                new PointF(cx - rx,  cy + ry),
            };

                // Clip the drawing area to the polygon and draw the image.
                GraphicsPath path = new GraphicsPath();
                path.AddPolygon(Points);
                GraphicsState state = gr.Save();
                gr.SetClip(path);   // Comment out to not clip.
                gr.DrawImage(image, dest_points, src_rect, GraphicsUnit.Pixel);
                gr.Restore(state);
            } else if (Shape == GameObject.ObjectShape.Rectangle)
            {
                gr.DrawImage(Texture, RectF);
            }

            
        }

        // Return a polygon's bounds.
        public void GetPolygonBounds(out float xmin, out float xmax, out float ymin, out float ymax)
        {
            xmin = Points[0].X;
            xmax = xmin;
            ymin = Points[0].Y;
            ymax = ymin;
            foreach (PointF point in Points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }
        }

        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="Points">the vertices of polygon</param>
        /// <param name="point">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public bool IsCollisionWithPointAndPolygon(Point point)
        {

            bool result = false;
            int j = Points.Count() - 1;
            for (int i = 0; i < Points.Count(); i++)
            {
                if (Points[i].Y < point.Y && Points[j].Y >= point.Y || Points[j].Y < point.Y && Points[i].Y >= point.Y)
                {
                    if (Points[i].X + (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) * (Points[j].X - Points[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public bool IsCollisionWithPointAndRect(Point point)
        {
            return RectF.Contains(point);
        }

        public void GetPointByAlignment(RenderWindow renderWindow)
        {
            if (Alignment == ScreenAlignment.TopLeft)
            {
                X = (renderWindow.ClientSize.Width / 6);
                Y = (renderWindow.ClientSize.Height / 6);
            }
            else if (Alignment == ScreenAlignment.TopCenter)
            {
                X = (renderWindow.ClientSize.Width / 2) - (Width / 2);
                Y = (renderWindow.ClientSize.Height / 6);
            }
            else if (Alignment == ScreenAlignment.TopRight)
            {
                X = renderWindow.ClientSize.Width - (renderWindow.ClientSize.Width / 6) - Width;
                Y = (renderWindow.ClientSize.Height / 6);
            }
            else if (Alignment == ScreenAlignment.CenterLeft)
            {
                X = (renderWindow.ClientSize.Width / 6);
                Y = (renderWindow.ClientSize.Height / 2) - (Height / 2);
            }
            else if (Alignment == ScreenAlignment.Center)
            {
                X = (renderWindow.ClientSize.Width / 2) - (Width / 2);
                Y = (renderWindow.ClientSize.Height / 2) - (Height / 2);
            }
            else if (Alignment == ScreenAlignment.CenterRight)
            {
                X = renderWindow.ClientSize.Width - (renderWindow.ClientSize.Width / 6) - Width;
                Y = (renderWindow.ClientSize.Height / 2) - (Height / 2);
            }
            else if (Alignment == ScreenAlignment.BottomLeft)
            {
                X = (renderWindow.ClientSize.Width / 6);
                Y = renderWindow.ClientSize.Height - (renderWindow.ClientSize.Height / 6);
            }
            else if (Alignment == ScreenAlignment.BottomCenter)
            {
                X = (renderWindow.ClientSize.Width / 2) - (Width / 2);
                Y = (renderWindow.ClientSize.Height - (renderWindow.ClientSize.Height / 6));
            }
            else if (Alignment == ScreenAlignment.BottomRight)
            {
                X = renderWindow.ClientSize.Width - (renderWindow.ClientSize.Width / 6) - Width;
                Y = renderWindow.ClientSize.Height - (renderWindow.ClientSize.Height / 6);
            }
            else if (Alignment == ScreenAlignment.None)
            {
                X = -1;
                Y = -1;
            }

            GetRect();
        }

        public void MoveRight(float speed)
        {
            X += speed;
            GetRect();
        }

        public void MoveLeft(float speed)
        {
            X -= speed;
            GetRect();
        }
        public void MoveUp(float speed)
        {
            Y -= speed;
            GetRect();
        }

        public void MoveDown(float speed)
        {
            Y += speed;
            GetRect();
        }

        public void FadeOut(int speed)
        {
            if (BrushBg.Color.A > 0)
                BrushBg = new SolidBrush(Color.FromArgb(BrushBg.Color.A-speed, BrushBg.Color.R, BrushBg.Color.G, BrushBg.Color.B));

            if (BrushFg.Color.A > 0)
                BrushFg = new SolidBrush(Color.FromArgb(BrushFg.Color.A - speed, BrushFg.Color.R, BrushFg.Color.G, BrushFg.Color.B));
        }

        public GameObject ShallowCopy()
        {
            return (GameObject)this.MemberwiseClone();
        }

        public GameObject DeepCopy()
        {
            GameObject other = (GameObject)this.MemberwiseClone();
            other.Name = Name;
            other.Width = Width;
            other.Height = Height;
            other.X = X;
            other.Y = Y;
            other.Z = Z;
            other.Radius = Radius;
            other.Shape = Shape;
            other.RectF = RectF;
            other.Points = Points;
            other.Texture = Texture;
            other.BrushBg = BrushBg;
            other.BrushFg = BrushFg;
            other.Alignment = Alignment;
            other.IsDraggable = IsDraggable;
            //I decided NOT to copy over boolean properties such as IsDragging, ShowBorders etc

            return other;
        }

    }
}
