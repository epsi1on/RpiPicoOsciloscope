using HomographyNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.FrequencyDetection
{
    public class CrossCorrelate
    {
        public double[] xs1, ys1, xs2, ys2;
        public int n1, n2;

        public double CrossCollerate()
        {
            double I;

            {
                var xsc = ArrayPool.Double(n1 + n2);
                int nsc, nsc2;

                Merge(xs1, xs2, n1, n2, xsc, out nsc);

                var ysc1 = ArrayPool.Double(xsc.Length);
                var ysc2 = ArrayPool.Double(xsc.Length);

                Interpolate(xs1, ys1, n1, xsc, ysc1, nsc);
                Interpolate(xs2, ys2, n2, xsc, ysc2, nsc);

                var sum = new KahanSum();

                for (var i = 0; i < nsc - 1; i++)
                {
                    var i1 = i;
                    var i2 = i + 1;

                    var y1 = ysc1[i1] - ysc2[i1];
                    var y2 = ysc1[i2] - ysc2[i2];

                    var x1 = xsc[i1];
                    var x2 = xsc[i2];

                    var ii = Integrate(x1, x2, y1, y2);

#if DEBUG

                    if (ii < 0)
                        throw new Exception("algorithm failure");

                    if(double.IsNaN(ii))
                        throw new Exception("algorithm failure");
#endif

                    sum.Add(ii);
                }


                ArrayPool.Return(xsc);
                ArrayPool.Return(ysc1);
                ArrayPool.Return(ysc2);

                I = sum.Value;
            }

            return I;
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
                return Math.Abs(y1 + y2) / 2 * dx;

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

        public static void Merge(double[] x1, double[] x2,int n1,int n2, double[] x,out int N)
        {
            //https://www.dotnetlovers.com/article/212/merge-two-sorted-arrays-in-one

            //note: x1 and x2 should be sorted, also x1 should not contains consecutive duplicates. same for x2. otherwise this do not return right answer.

            //index for result
            int n = 0;
            //index for array1
            int i = 0;
            //index for array2
            int j = 0;

            var array1 = x1;
            var array2 = x2;
            var arrayResult = x;


            //until any one of arrays all elements are traversed
            while (i < n1 && j < n2)
            {
                //comparing items of array1 and array2
                if (array1[i] < array2[j])
                {
                    //inserting array1 item since it is lower
                    arrayResult[n] = array1[i];
                    //incrementing i, because array1's item is traversed
                    i++;
                    goto done;
                }
                if (array1[i] > array2[j])
                {
                    arrayResult[n] = array2[j];
                    j++;
                    goto done;
                }
                if (array1[i] == array2[j])
                {
                    arrayResult[n] = array2[j];
                    j++;i++;
                    goto done;
                }

                done:
                //incrementing since one item has been inserted to result array
                n++;
            }

            //if any elements are left in array1, simply inserting all to result
            while (i < n1)
            {
                arrayResult[n] = array1[i];
                i++;
                n++;
            }

            //if any elements are left in array2, simply inserting all to result
            while (j < n2)
            {
                arrayResult[n] = array2[j];
                j++;
                n++;
            }

            N = n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xs">input xs</param>
        /// <param name="ys">input ys</param>
        /// <param name="n">len</param>
        /// <param name="x2s">input x2s (to be interpolated)</param>
        /// <param name="y2s">input y2s (filled with method)</param>
        /// <param name="n2">len</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Interpolate(double[] xs, double[] ys, int n1, double[] x2s, double[] y2s, int n2)
        {
            //note: all members of xs are presented in x2s

            //index for result
            int n = 0;
            //index for array1
            int i = 0;
            //index for array2
            int j = 0;

            var array1 = xs;
            var array2 = x2s;
            //var arrayResult = x2s; 


            //until any one of arrays all elements are traversed
            while (i < n1 && j < n2)
            {
                double x = double.NaN;
                double y = double.NaN;

                //comparing items of array1 and array2
                if (array1[i] < array2[j])
                {
                    //inserting array1 item since it is lower
                    x = array1[i];
                    y = ys[i];
                    //incrementing i, because array1's item is traversed
                    i++;
                    goto done;
                }

                if (array1[i] > array2[j])
                {
                    x = array2[j];
                    {
                        var i1 = i - 1;
                        var i2 = i;

                        if (i == 0)
                        {
                            i1 = 0;
                            i2 = 1;
                        }
                        else if (i == n1 - 1)
                        {
                            i1 = n1 - 2;
                            i2 = n1 - 1;
                        }

                        var x1 = array1[i1];
                        var x2 = array1[i2];

                        var y1 = ys[i1];
                        var y2 = ys[i2];

                        y = y1 + (y2 - y1) / (x2 - x1) * (x - x1);
                        //y = double.NaN;
                    }
                    
                    j++;
                    goto done;
                }

                if (array1[i] == array2[j])
                {
                    x = array1[i];
                    y = ys[i];
                    j++; i++;
                    goto done;
                }

                done:
                //arrayResult[n] = x;
                y2s[n] = y;




                //incrementing since one item has been inserted to result array
                n++;
            }
            //if any elements are left in array1, simply inserting all to result
            while (i < n1)
            {
                var x = array1[i];
                var y = ys[i];

                //arrayResult[n] = x;
                y2s[n] = y;

                i++;
                n++;
            }
            //if any elements are left in array2, simply inserting all to result
            while (j < n2)
            {
                var x = array2[j];
                var y = double.NaN;//need to be extrapolated

                var x1_ = array1[n1 - 2];
                var x2_ = array1[n1 - 1];

                var y1_ = ys[n1 - 1];
                var y2_ = ys[n1 - 2];

                y = y1_ + (y2_ - y1_) / (x2_ - x1_) * (x - x1_);

                //arrayResult[n] = x;
                y2s[n] = y;

                j++;
                n++;
            }

            //N = n;

            //throw new NotImplementedException();
        }

    }
}
