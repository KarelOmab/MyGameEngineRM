using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCatan.Objects
{
    public class City : MyGameEngineRM.GameObject
    {
        public City(string team, float x, float y)
        {
            Team = team;
            Z = 5;
            X = x;
            Y = y;
            Shape = ObjectShape.Rectangle;
            Points = new PointF[4];
            Texture = Image.FromFile(@"resources\\city.png");
            Width = Texture.Width;
            Height = Texture.Height;
            IsDraggable = true;
            GetVertices();
        }
    }
}
