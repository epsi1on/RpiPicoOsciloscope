using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public static class ByteArrayPool
    {
        public static readonly int Length = 1500;


        public static byte[] Borrow()
        {
            throw new NotImplementedException();
        }

        public static void Return(byte[] data)
        {

        }
    }
}
