using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.HardwareInterface
{
    public class ArduinoInterface : IDaqInterface
    {
        public DataRepository TargetRepository { get; set; }


        public double AdcMaxVoltage { get { return 3.3; } }
        public int AdcResolutionBits
        {
            get { return 10; }
        }

        public long AdcSampleRate
        {
            get
            {
                return 115200 / 16;
            }
        }

        public int SampleRate = 115200 / 16;

        public string PortName;

        public double SampleToVoltage(short sample)
        {
            var max = 1024;//10 bit
            return (sample * 3.3 / max);
        }

        public void StartSync()
        {
            var sport = new SnifferSerial(PortName, 115200);

            {//https://stackoverflow.com/a/73668856
                sport.Handshake = Handshake.None;
                sport.DtrEnable = true;
                sport.RtsEnable = true;
                sport.StopBits = StopBits.One;
                sport.DataBits = 8;
                sport.Parity = Parity.None;
                sport.ReadBufferSize = 1024 * 1000;//1000KB
            }


            sport.Open();

            short value;

            var dt = new byte[2];

            var arr = TargetRepository.Samples;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            var cnt = 0;

            while (true)
            {
                if (sport.BytesToRead > 1)
                {
                    sport.Read(dt, 0, 2);
                    /*
                    if (cnt == 100)
                    {
                        if (dt[0] != 0 || dt[1] != 0)
                            Guid.NewGuid();

                        cnt = 0;
                    }
                    else
                    */
                    {

                        if (dt[0] > 0)
                            Guid.NewGuid();

                        value = (short)(dt[0] * 256 + dt[1]);// (short)BitConverter.ToUInt16(dt, 0);

                        arr.Add(value);
                    }
                    cnt++;
                }
            }

        }

        public void StopAdc()
        {
            throw new NotImplementedException();
        }

        public void DisConnect()
        {
            throw new NotImplementedException();
        }
    }
}
