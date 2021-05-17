using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCatan.Objects
{
    public class Circle : MyGameEngineRM.GameObject
    {
        public Circle(float x, float y, float w, float h)
        {
            Z = 4;
            X = x;
            Y = y;
            Shape = ObjectShape.Rectangle;
            Points = new PointF[4];
            Texture = Image.FromFile(@"resources\\circle.png");
            Width = w;
            Height = h;
            GetRect();
        }
    }
}
