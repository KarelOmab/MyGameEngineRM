using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCatan.Objects
{
    public class BaseTerrain : MyGameEngineRM.GameObject
    {
        
        public string Value { get; set; }
        public BaseTerrain()
        {
            Z = 1;
            Radius = 64;
            Points = new PointF[6];
            Shape = ObjectShape.Hexagon;

            BrushBg = new SolidBrush(Color.FromArgb(255, 201, 196, 157));
            BrushFg = new SolidBrush(Color.FromArgb(255, 255, 255, 255));


        }

        
    }
}
