using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Lumber : BaseTerrain
    {
        public Lumber(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Texture = Image.FromFile(@"resources\\lumber.png");
            GetVertices();
        }
    }
}
