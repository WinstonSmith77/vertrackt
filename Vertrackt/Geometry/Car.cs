using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class Car : IScaleDown<Car>
    {
        public Point Acceleration { get; }

        public override string ToString()
        {
            return $"{Acceleration};;{Speed};;{Position}";
        }

        public Point Position { get; }

        public Point Speed { get; }

        public Car(Point position) : this(position, Point.Zero)
        {
        }

        public Car(Point position, Point speed)
        {
            Position = position;
            Speed = speed;
        }

        private Car(Point position, Point speed, Point acceleration)
            : this(position, speed)
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

        public Car ScaleDown(int scale)
        {
            return new Car(Position.ScaleDown(scale), Speed, Acceleration);
        }

        public Car ScaleUp(int scale)
        {
            return new Car(Position.ScaleUp(scale), Speed, Acceleration);
        }
    }
}
