using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Brick : BaseTerrain
    {
        public Brick(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Texture = Image.FromFile(@"resources\\brick.png");
            GetVertices();
        }
    }
}
