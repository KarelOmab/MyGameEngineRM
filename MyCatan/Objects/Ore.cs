using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Ore : BaseTerrain
    {
        public Ore(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Texture = Image.FromFile(@"resources\\ore.png");
            GetVertices();
        }
    }
}
