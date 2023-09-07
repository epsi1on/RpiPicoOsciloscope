using HomographyNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SimpleOsciloscope.UI
{
    public static class Extensions
    {
        public static double KahanSum(this IEnumerable<double> data, int n)
        {
            var buf = new KahanSum();

            var cnt = 0;

            foreach (var item in data)
            {
                if (cnt == n)
                    break;

                buf.Add(item);

                cnt++;
            }

            return buf.Value;
        }

        public static void ReadArray(this Stream stream, byte[] array)
        {
            var buf = array;

            var counter = 0;

            var l = array.Length;

            while (counter < l)
            {
                var remain = l - counter;

                var rdr = stream.Read(buf, counter, remain);
                counter += rdr;
            }
        }
    }
}
