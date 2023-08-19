using System;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace SimpleOsciloscope.UI
{
    public class SnifferSerial : SerialPort
    {

        public bool LogToConsole = true;

        public SnifferSerial(string portName, int baudRate) : base(portName, baudRate)
        {
        }

        public byte[] ReadAvailable()
        {
            return Read(this.BytesToRead);
        }

        public byte[] Read(int length)
        {
            var buf = new byte[length];

            var rd = this.Read(buf, 0, buf.Length);

            if(LogToConsole)
            {
                var sb = new StringBuilder();

                for (var i = 0; i < rd; i++)
                    sb.AppendFormat(" {0:x2}", buf[i]);

                Console.WriteLine("Reading {0} bytes: {1}", rd, sb.ToString());
            }

            Array.Resize(ref buf, rd);

            return buf;
        }

        public void Write(params byte[][] data)
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

            this.Write(buf, 0, buf.Length);

            if (LogToConsole)
            {
                foreach (var b in buf)
                    sb.AppendFormat(" {0:x2}", b);

                Console.WriteLine("Writing {0} bytes: {1}", l, sb.ToString());
            }
        }
    }
}
