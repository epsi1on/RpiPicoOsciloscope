using System.Runtime.Serialization;
using System;

namespace SimpleOsciloscope.UI
{
    [Serializable]
    public class RgbBitmap : ISerializable
    {
        public byte[] Data;

        public int Width, Height;



        public RgbBitmap()
        {

        }

        public RgbBitmap(int w, int h)
        {
            this.Width = w;
            this.Height = h;
            this.Data = new byte[w * h * 3];
        }


        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            var index = x + (y * Width);
            index *= 3;

            Data[index] = b;
            Data[index + 1] = g;
            Data[index + 2] = r;
            //Data[index + 3] = a;
        }


        public int GetPixel(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                throw new Exception();

            var index = x + (y * Width);
            index *= 3;

            var res = BitConverter.ToInt32(Data, index);

            return res;
        }

        public void Clear()
        {
            var dt = Data;

            for (int i = Data.Length-1; i >=0; i--)
            {
                dt[i] = 0;
            }
        }

        public RgbBitmap Clone()
        {
            var buf = new RgbBitmap(Width, Height);

            this.Data.CopyTo(buf.Data, 0);

            return buf;
        }


        public void CopyTo(RgbBitmap bmp)
        {
            if (bmp.Data.Length != this.Data.Length)
                throw new ArgumentException();

            System.Buffer.BlockCopy(this.Data, 0, bmp.Data, 0, bmp.Data.Length);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("Data", Data);
        }

        protected RgbBitmap(SerializationInfo info, StreamingContext context)
        {
            Width = info.GetInt32("Width");
            Height = info.GetInt32("Height");
            Data = (byte[])info.GetValue("Data", typeof(byte[]));
        }
    }
}
