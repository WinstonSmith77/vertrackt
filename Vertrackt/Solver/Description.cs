using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    public class Description
    {
        public Point Start { get;}
        public Point End { get; private set; }
        public List<LineD> Obstacles { get; }
        public IBoundingBox BoundingBox { get; }
        public int Steps { get; set; }

        public Description(Point start, Point end, List<LineD>obstacles, IBoundingBox boundingBox, int steps)
        {
            Start = start;
            End = end;
            Obstacles = obstacles;
            BoundingBox = boundingBox;
            Steps = steps;
        }

        public Description(Point start, Point end, int steps)
        {
            Start = start;
            End = end;
            Obstacles = new List<LineD>();
            BoundingBox = new AlwaysInsideBoundIngBox();
            Steps = steps;
        }

        
    }
}
