﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public static class Utils
    {
        static string[] prefixeSI = { "y", "z", "a", "f", "p", "n", "µ", "m", "", "k", "M", "G", "T", "P", "E", "Z", "Y" };
        public static string numStr(double num)
        {
            int log10 = (int)Math.Log10(Math.Abs(num));
            if (log10 < -27)
                return "0.000";
            if (log10 % -3 < 0)
                log10 -= 3;
            int log1000 = Math.Max(-8, Math.Min(log10 / 3, 8));

            var val = (double)num / Math.Pow(10, log1000 * 3);

            var v2 = Math.Abs(val);

            string t = "";

            if (v2 < 10)
                t = v2.ToString("0.00");

            else if (v2 < 100)
                t = v2.ToString("0.0");

            else if (v2 < 1000)
                t = v2.ToString("0");

            return t + prefixeSI[log1000 + 8];
        }

    }
}
