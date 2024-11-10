using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using static System.Net.Mime.MediaTypeNames;

namespace SimpleOsciloscope.TestConsole
{
    internal class CopyTest
    {
        public static void Test()
        {
            var n = 10_000_000;
            var src = new short[n];
            var dst = new short[n];

            var tests = new test[] { TestForLoop, TestForLoopInv, TestArrayCopy , TestBlockCopy ,};

            foreach (var item in tests)
            {
                var sp = System.Diagnostics.Stopwatch.StartNew();

                sp.Start();

                item(src, dst);

                sp.Stop();

                Console.WriteLine(item.Method.Name + ","+ sp.ElapsedTicks);
            }
            
        }

        delegate void test(short[] source, short[] dest);

        private static void TestForLoop(short[] source, short[] dest)
        {
            var n = source.Length;

            for (int i = 0; i < n; i++)
            {
                dest[i] = source[i];
            }
        }

        private static void TestForLoopInv(short[] source, short[] dest)
        {
            var n = source.Length;

            for (int i = n - 1; i > 0; i--)
            {
                dest[i] = source[i];
            }
        }

        private static void TestArrayCopy(short[] source, short[] dest)
        {
            Array.Copy(source, dest, source.Length);
        }

        private static void TestBlockCopy(short[] source, short[] dest)
        {
            Buffer.BlockCopy(source, 0, dest, 0, source.Length);
        }

       
    }
}
