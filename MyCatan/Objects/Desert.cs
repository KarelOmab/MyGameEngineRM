using System;
using System.Drawing;

namespace MyCatan.Objects
{
    public class Desert : BaseTerrain
    {
        public Desert(string name, int x, int y)
        {
           Name = name;
           X = x;
           Y = y;
           Texture = Image.FromFile(@"resources\\desert.png");
           GetVertices();
        }
    }
}
