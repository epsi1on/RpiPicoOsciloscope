using SimpleOsciloscope.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOsciloscope.TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            testFreq();
            
            

        }


        static void t2()
        {
            var ifs = new RpiPicoDaqInterface();

            ifs.TargetRepository = UiState.Instance.CurrentRepo;

            ifs.StartSync();

        }

        static void test()
        {
            var l = 100;
            var t1 = new FixedLengthList<int>(l);
            var ts = ArrayPool.Int32(l);

            for (int i = 0; i < 10.5*l; i++)
            {
                t1.Add(i);
            }

            t1.CopyTo(ts);
        }

        static void testFreq()
        {
            var l = 200_000;

            var a1 = new double[l];
            var a2 = new double[l];


            var f = 1e-2;

            var pi = Math.PI;

            var dt = 1.0;

            for (var i = 0; i < l; i++)
            {
                var x = i * dt;

                var y = Math.Sin(2 * pi * f * x);// + Math.Sin(b * x);

                a1[i] = x;
                a2[i] = y;
            }

            double t;
            var t2 = new FftFrequencyDetector().TryGetFrequency(a2, out t);

            var tt = 1 / (t);

            var lcm = 2 * Math.PI;

            var diff = Math.Abs(tt - lcm);

            if (diff > 1e-3)
                throw new Exception();
        }

        public static int findLCM(int a, int b) //method for finding LCM with parameters a and b
        {
            int num1, num2;                         //taking input from user by using num1 and num2 variables
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (int i = 1; i <= num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num2;
        }


    }
}
