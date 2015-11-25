using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using Vertrackt.Geometry;
using Vertrackt.Solver;

namespace Vertrackt
{
    public static class Output
    {
        public static List<string> CtOutput(this Result result)
        {
            return result.Solution.Select(acc => acc.Direction.ToStringCt()).ToList();
        }

        public static List<string> OutputCSV(this Result result, Point start, DateTime startTime)
        {
            var resultLines = new List<string>
            {
                (DateTime.Now - startTime).TotalSeconds.ToString(CultureInfo.InvariantCulture),
            };

            var cars = AllCarsInSolution(result, start);


            foreach (var car in cars)
            {
                resultLines.Add(car.ToString());
            }

            resultLines.Add("");
            resultLines.Add("");

            resultLines.Add(string.Join(";", cars.Select(item => new[] { item.Position.X.ToString(), item.Position.Y.ToString()}).SelectMany(item => item)));

            return resultLines;
        }

        public static List<Car> AllCarsInSolution(Result result, Point start)
        {
            var cars = result.Solution.Aggregate(new List<Car> { new Car(start) }, (list, acc) =>
              {
                  list.Add(list.Last().Iterate(acc.Direction));
                  return list;
              });
            return cars;
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


        public static double CalcPercentage(IEnumerable<Iteration> iterations , int startAt)
        {

            var data = iterations.Select((it, index) => new { Percentage = (double)it.Index / it.Steps.Count, Total = it.Steps.Count }).Skip(startAt).ToList();

            var result = (double)0;

            for (int i = 0; i < data.Count; i++)
            {
                var partDone = data[i].Percentage;
                for (int j = i - 1; j >= 0; j--)
                {
                    partDone /= data[j].Total;
                }
                result += partDone;
            }

            return result;
        }
    }
}
