using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Grain : BaseTerrain
    {
        public Grain(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Texture = Image.FromFile(@"resources\\grain.png");
            GetVertices();
        }
    }
}
