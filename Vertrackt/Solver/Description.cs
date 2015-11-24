using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    public class Description : IScaleDown<Description>
    {
        public Point Start { get; }
        public Point End { get; private set; }
        public List<LineD> Obstacles { get; }
        public IBoundingBox BoundingBox { get; }
        public int Steps { get; }

        public Description(Point start, Point end, List<LineD> obstacles, IBoundingBox boundingBox, int steps, Func<Point, Point> auxEnd = null)
        {

            if (auxEnd == null)
            {
                auxEnd = point => end;
            }

            AuxEnd = auxEnd;


            Start = start;
            End = end;
            Obstacles = obstacles;
            BoundingBox = boundingBox;
            Steps = steps;
        }

        public Func<Point, Point> AuxEnd { get; }

        public Description(Point start, Point end, int steps) :
            this(start, end, new List<LineD>(), new AlwaysInsideBoundIngBox(), steps, null)
        {
            Start = start;
            End = end;
            Obstacles = new List<LineD>();
            BoundingBox = new AlwaysInsideBoundIngBox();
            Steps = steps;
        }


        public Description ScaleDown(int scale)
        {
           return new Description(
               Start.ScaleDown(scale),
               End.ScaleDown(scale),  
               Obstacles.Select(ob => ob.ScaleDown(scale)).ToList(),
               BoundingBox.ScaleDown(scale), 
               Steps,
               AuxEnd);
        }

        public Description ScaleUp(int scale)
        {
            throw new NotImplementedException();
        }
    }
}
