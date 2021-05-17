using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGameEngineRM
{
    public class GameText : GameObject
    {



        public string Text { get; set; }
        public SizeF StringSize = new SizeF();
        public Font Font { get; set; }

        
        
        public GameText(RenderWindow renderWindow, string text, float x, float y, SolidBrush bg, SolidBrush fg, ObjectShape shape=ObjectShape.Rectangle)
        {
            this.Text = text;
            this.Font = new Font("MS Sans Serif", 32);
            Points = new PointF[4];
            this.BrushBg = bg;
            this.BrushFg = fg;
            Shape = shape;

            StringSize = renderWindow.CreateGraphics().MeasureString(Text, Font);
            this.RectF = new RectangleF(x,y, StringSize.Width, StringSize.Height);
            GetVertices();
        }

        public GameText(RenderWindow renderWindow, string text, float x, float y, float w, float h, SolidBrush bg, SolidBrush fg, ObjectShape shape = ObjectShape.Rectangle)
        {
            this.Text = text;
            this.Font = new Font("MS Sans Serif", 32);
            Points = new PointF[4];
            this.BrushBg = bg;
            this.BrushFg = fg;
            Shape = shape;
            this.RectF = new RectangleF(x, y, w, h);
            GetVertices();
        }
    }
}
