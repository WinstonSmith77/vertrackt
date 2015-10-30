using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class Point
    {
        private readonly int _xPos;
        private readonly int _yPos;
        private int _xSpeed;
        private int _ySpeed;

        public Point(int xPos, int yPos)
        {
            _xPos = xPos;
            _yPos = yPos;
        }

        public Point Iterate(int xAcc, int yAcc)
        {
            var newX = _xPos + _xSpeed;
            var newY = _yPos + _ySpeed;

            return new Point(newX, newY);
        }
    }
}
