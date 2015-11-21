using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;
using Vertrackt.Solver;

namespace Vertrackt
{
    public static class Output
    {

        public static List<string> CtOutput(this Result result)
        {
            return result.Solution.Select(acc => acc.ToStringCt()).ToList();
        }

        public static List<string> OutputCSV(this Result result, Point start)
        {
            var car = new Car(start);
            var resultLines = new List<string> { car.ToString() };

            foreach (var acc in result.Solution)
            {
                car = car.Iterate(acc);
                resultLines.Add(car.ToString());
            }

            return resultLines;
        }

        public static string GetPathToSave(this DateTime startTime)
        {
            var folderName = Path.Combine("Vertrackt", startTime.ToString("yyyy-dd-M--HH-mm-ss"));
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
