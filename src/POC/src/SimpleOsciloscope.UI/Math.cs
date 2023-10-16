using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public static class MathUtil
    {
        public static int MaxValueForBits(int bitCount)
        {
            //https://www.codeproject.com/Questions/148093/2-to-the-power-of-n
            //2 to power n

            return 1 << bitCount;
        }
    }
}
