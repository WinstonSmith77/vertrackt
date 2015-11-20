using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public static class GaussSolver
    {
        //Ax = B    m -> ["A""B"]
        public static double[] Solve(double[,] m)
        {
            var order = m.GetLength(0);

            double[] result;

            if (order == 2)
            {
                result = OrderTwo(m);
            }
            else if (order == 3)
            {
                result = OrderThree(m);
            }
            else
            {
                result = DefaultAlgorithm(m, order);
            }

            return result;
        }

        private static double[] OrderThree(double[,] m)
        {
            var x3 = -m[1, 1] * m[2, 0] * m[0, 3] + m[0, 0] * m[1, 1] * m[2, 3] - m[0, 0] * m[2, 1] * m[1, 3] +
                        m[2, 1] * m[1, 0] * m[0, 3] + m[2, 0] * m[0, 1] * m[1, 3] - m[1, 0] * m[0, 1] * m[2, 3];
            var x2 =
                -(-m[0, 0] * m[2, 2] * m[1, 3] + m[0, 0] * m[2, 3] * m[1, 2] + m[2, 0] * m[0, 2] * m[1, 3] + m[2, 2] * m[1, 0] * m[0, 3] -
                  m[2, 3] * m[1, 0] * m[0, 2] - m[2, 0] * m[0, 3] * m[1, 2]);
            var x1 = -m[0, 1] * m[2, 2] * m[1, 3] + m[0, 1] * m[2, 3] * m[1, 2] - m[0, 2] * m[1, 1] * m[2, 3] +
                        m[0, 2] * m[2, 1] * m[1, 3] - m[0, 3] * m[2, 1] * m[1, 2] + m[0, 3] * m[1, 1] * m[2, 2];

            var denom = -m[0, 0] * m[2, 1] * m[1, 2] + m[0, 0] * m[1, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2]
                           - m[1, 1] * m[2, 0] * m[0, 2] + m[2, 1] * m[1, 0] * m[0, 2]
                           - m[1, 0] * m[0, 1] * m[2, 2];

            x1 /= denom;
            x2 /= denom;
            x3 /= denom;


            var result = new[] { x1, x2, x3 };
            return result;
        }

        private static double[] OrderTwo(double[,] m)
        {
            var x2 = m[0, 0] * m[1, 2] - m[1, 0] * m[0, 2];
            var x1 = -(m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]);

            var denom = m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1];
            x1 /= denom;
            x2 /= denom;

            var result = new[] { x1, x2 };
            return result;
        }

        private static double[] DefaultAlgorithm(double[,] m, int order)
        {
            var result = new double[order];

            double temp;
            var orderPlusOne = order + 1;

            //first triangulize the matrix
            for (var i = 0; i < order; i++)
            {
                var iMax = i;
                temp = Math.Abs(m[iMax, i]);
                //find the line with the largest absvalue in this row
                for (var j = i + 1; j < order; j++)
                {
                    if (temp < Math.Abs(m[j, i]))
                    {
                        iMax = j;
                    }
                    temp = Math.Abs(m[iMax, i]);
                }
                if (i < iMax)
                {
                    for (var k = i; k < orderPlusOne; k++)
                    {
                        Helpers.Swap(ref m[i, k], ref m[iMax, k]);
                    }
                }

                //scale  for leading zero
                for (var j = i + 1; j < order; j++)
                {
                    temp = m[j, i] / m[i, i];
                    m[j, i] = 0;
                    for (var k = i + 1; k < orderPlusOne; k++)
                    {
                        m[j, k] = m[j, k] - m[i, k] * temp;
                    }
                }
            }

            //then substitute the coefficients
            for (var j = order - 1; j >= 0; j--)
            {
                temp = m[j, orderPlusOne - 1];
                for (var k = j + 1; k < order; k++)
                {
                    temp = temp - m[j, k] * result[k];
                }
                result[j] = temp / m[j, j];
            }
            return result;
        }
    }
}
