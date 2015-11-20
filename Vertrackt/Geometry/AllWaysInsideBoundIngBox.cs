using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class AllWaysInsideBoundIngBox : IBoundingBox
    {
        public bool IsInside(Point p)
        {
            return true;
        }
    }
}
