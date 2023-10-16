using HomographyNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.FrequencyDetection
{
    public class SimpleCrossCorrelate
    {
        public double[] ys;
        public int n;
        public double Tau;
        public double Dx = 1;

        public double CrossCollerate()
        {
            var np = (int)Math.Floor(Tau / Dx);

            var tu = Tau;
            var dx = Dx;

            var buf = new KahanSum();

            for (var i = 0; i < n - np - 3; i++)
            {
                var x1 = i * dx + tu;// xs[i] + tu;
                var x2 = x1 + dx;// xs[i + 1] + tu;

                var y1 = ys[i];
                var y2 = ys[i + 1];


                var xl = (i + np) * dx;// xs[i + np];
                var xc = xl + dx;//xs[i + np + 1];
                var xr = xc + dx;//xs[i + np + 2];

                var yl = ys[i + np];
                var yc = ys[i + np + 1];
                var yr = ys[i + np + 2];

                {
                    var yp1 = Interpolate(xl, yl, xc, yc, x1);
                    var yp2 = Interpolate(xc, yc, xr, yr, x2);

                    var xm = xc;
                    var ym = Interpolate(x1, y1, x2, y2, xm);

                    var i1 = Integrate(x1, xc, y1 - yp1, yc - ym);
                    var i2 = Integrate(xc, x2, yc - ym, y2 - yp2);

                    buf.Add(i1);
                    buf.Add(i2);
                }

                

            }

            return buf.Value;
        }

        private static double Interpolate(double x1, double y1, double x2, double y2,double x)
        {
            return y1 + (y1 - y2) / (x1 - x2) * (x - x1);
        }

        private static double Integrate(double x1, double x2, double y1, double y2)
        {
            //int of abs of f, where f cross from x1,y1 to x2,y2 i.e.: int(abs(f),x1,x2)
            //x2 must be higher than x1

            var dx = x2 - x1;

            if (y1 == 0 || y2 == 0)//any one zero
                return Math.Abs(y1 + y2) * dx;

            //none are zero

            if (y1 < 0 && y2 < 0 || y1 > 0 && y2 > 0)//both same sign
                return Math.Abs(y1 + y2) * 0.5 * dx;

            //y1 and y2 are nonzero, different signs

            {//unify, y1 always positive, y2 always negative
                y1 = Math.Abs(y1);
                y2 = -Math.Abs(y2);
            }

            {
                var epsilon = 1e-5;

                if (y1 - y2 < epsilon)//y1 and y2 both are near zero
                {
                    return dx * (y1 - y2) * 0.5;//always positive, limit of below formula when y1,y2 => zero
                }
                else
                {
                    return dx / (y1 - y2) * (y1 * y1 + y2 * y2) * 0.5;
                }

            }
        }

    }
}
