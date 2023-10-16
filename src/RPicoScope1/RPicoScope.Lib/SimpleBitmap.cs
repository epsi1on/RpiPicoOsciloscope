using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPicoScope.Lib
{
    public class SimpleBitmap
    {
        public byte[] Data; public int Width; public int Height;

        public SimpleBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new byte[Width * Height];
        }

        public SimpleBitmap(byte[] data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }

        
    }
}
