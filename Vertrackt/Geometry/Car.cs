using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class Car
    {
        public override string ToString()
        {
              return $"(Position={Position}, Speed={Speed})";
        }

        public Point Position { get; }

        public Point Speed { get; }

        public Car(Point position)
        {
            Position = position;
            Speed = new Point(0, 0);
        }

        private Car(Point position, Point speed) 
            : this(position)
        {
            Speed = speed;
        }

        public Car Iterate(Point acceleration)
        {
            var newSpeed = Speed + acceleration;
            var newPosition = Position + newSpeed;

            return new Car(newPosition, newSpeed);
        }
    }
}
