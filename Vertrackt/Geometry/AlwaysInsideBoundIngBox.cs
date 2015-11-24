using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class AlwaysInsideBoundIngBox : IBoundingBox
    {
        public bool IsInside(Point p)
        {
            return true;
        }

        public IBoundingBox ScaleDown(int scale)
        {
            return this;
        }

        public IBoundingBox ScaleUp(int scale)
        {
            return this;
        }
    }
}
