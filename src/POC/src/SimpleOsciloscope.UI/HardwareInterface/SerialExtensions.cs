using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.HardwareInterface
{
    public static class SerialExtensions
    {
        public static bool LogToConsole = true;

        public static byte[] ReadAvailable(this SerialPort port)
        {
            return ReadExplicitLength(port, port.BytesToRead);
        }

        public static byte[] ReadExplicitLength(this SerialPort port, int length)
        {
            var buf = new byte[length];

            var counter = 0;

            var l = length;

            while (counter < l)
            {
                var remain = l - counter;

                var rdr = port.Read(buf, counter, remain);
                counter += rdr;
            }


            if (LogToConsole)
            {
                var sb = new StringBuilder();

                for (var i = 0; i < length; i++)
                    sb.AppendFormat(" {0:x2}", buf[i]);

                Console.WriteLine("Reading {0} bytes: {1}", length, sb.ToString());
            }

            Array.Resize(ref buf, length);

            return buf;
        }

        public static void ReadExplicitLength(this SerialPort port, int length, byte[] data)
        {
            var buf = data;

            var counter = 0;

            var l = length;

            while (counter < l)
            {
                var remain = l - counter;

                var rdr = port.Read(buf, counter, remain);
                counter += rdr;
            }
        }


        public static void Write(this SerialPort port, params byte[][] data)
        {
            var sb = new StringBuilder();

            var l = data.Sum(i => i.Length);
            var buf = new byte[l];

            var cnt = 0;

            foreach (var item in data)
            {
                item.CopyTo(buf, cnt);
                cnt += item.Length;
            }

            port.Write(buf, 0, buf.Length);

            if (LogToConsole)
            {
                foreach (var b in buf) sb.AppendFormat(" {0:x2}", b);

                Console.WriteLine("Writing {0} bytes: {1}", l, sb.ToString());
            }
        }
    }
}
