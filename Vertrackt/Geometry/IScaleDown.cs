using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public  interface IScaleDown<T>
    {
        T ScaleDown(int scale);
        T ScaleUp(int scale);
    }
}
