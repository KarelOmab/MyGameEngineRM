using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Wool : BaseTerrain
    {
        public Wool(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Texture = Image.FromFile(@"resources\\wool.png");
            GetVertices();
        }
    }
}
