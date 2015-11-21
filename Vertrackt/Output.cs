﻿using System;
using System.Collections.Generic;
using System.Globalization;
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

            return resultLines;
        }

        public static List<Car> AllCarsInSolution(Result result, Point start)
        {
            var cars = result.Solution.Aggregate(new List<Car> {new Car(start)}, (list, acc) =>
            {
                list.Add(list.Last().Iterate(acc));
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
    }
}
