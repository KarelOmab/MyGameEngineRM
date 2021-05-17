using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCatan.Objects
{
    public class Settlement : MyGameEngineRM.GameObject
    {

        public Settlement(string team, float x, float y)
        {
            Team = team;
            Z = 5;
            X = x;
            Y = y;
            Shape = ObjectShape.Rectangle;
            Points = new PointF[4];
            Texture = Image.FromFile(@"resources\\settlement.png");
            Width = Texture.Width;
            Height = Texture.Height;
            IsDraggable = true;
            GetVertices();
        }
    }
}
