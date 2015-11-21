using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class Car
    {
        public Point Acceleration { get; }

        public override string ToString()
        {
            return $"{Acceleration};;{Speed};;{Position}";
        }

        public Point Position { get; }

        public Point Speed { get; }

        public Car(Point position)
        {
            Position = position;
            Speed = new Point(0, 0);
        }

        private Car(Point position, Point speed, Point acceleration)
            : this(position)
        {
            Acceleration = acceleration;
            Speed = speed;
        }

        public Car Iterate(Point acceleration)
        {
            var newSpeed = Speed + acceleration;
            var newPosition = Position + newSpeed;

            return new Car(newPosition, newSpeed, acceleration);
        }
    }
}
