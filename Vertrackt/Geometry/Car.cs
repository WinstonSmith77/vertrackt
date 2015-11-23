using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

        public static int MaxDistPossible(int numberOfIterationsLeft)
        {
            return _stepsToDist[numberOfIterationsLeft];
        }


        private static readonly Dictionary<int, int> _stepsToDist = new Dictionary<int, int>();

        static Car()
        {
            var car = new Car(Point.Zero);

            for (int i = 0; i < 100; i++)
            {
                _stepsToDist.Add(i, car.Position.X);
                car = car.Iterate(new Point(Steps.MaxAcceleation, 0));
            }
        }
    }
}
