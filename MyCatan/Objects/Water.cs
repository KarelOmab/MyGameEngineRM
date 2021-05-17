using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Water : BaseTerrain
    {
        public Water(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Texture = Image.FromFile(@"resources\\water.png");
            GetVertices();
        }
    }
}
