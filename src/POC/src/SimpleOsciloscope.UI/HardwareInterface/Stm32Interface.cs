using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.HardwareInterface
{
    public class Stm32Interface : IDaqInterface
    {

        static Stm32Interface()
        {
            //SampleRate = (int)UiState.Instance.CurrentRepo.SampleRate;
        }

        public Stm32Interface(string portName, long adcSampleRate)
        {
            //AdcResolutionBits = adcResolutionBits;
            AdcSampleRate = adcSampleRate;
            PortName = portName;
        }

        public double AdcMaxVoltage { get { return 3.3; } }

        public int AdcResolutionBits
        {
            get { return resolutionBits; }
            set
            {
                resolutionBits = value;
            }
        }


        public long AdcSampleRate { get; set; }


        private int resolutionBits = 12;

        private static readonly int HeaderLength = 32;
        private static readonly int BodyLength = 10*1024;


        //public int SampleRate ;
        public string PortName;

        public long TotalReads;

        public DataRepository TargetRepository { get; set; }


        //private Queue<byte[]> Readed = new Queue<byte[]>();//those are filled with data
        //private Queue<byte[]> Emptied = new Queue<byte[]>();//those that content are used and ready to be reused
        //private object RLock = new object();//for Readed
        //private object ELock = new object();//for ELock

        public void StartSync()
        {
            var sport = new SnifferSerial(PortName, 268435456);

            {//https://stackoverflow.com/a/73668856
                sport.Handshake = Handshake.None;
                sport.DtrEnable = true;
                sport.RtsEnable = true;
                sport.StopBits = StopBits.One;
                sport.DataBits = 8;
                sport.Parity = Parity.None;
                sport.NewLine = "\n";
                sport.ReadBufferSize = 1024 * 1000;//1000KB
            }

            sport.Open();

            var arrLength = BodyLength;

            sport.WriteLine("");


            //Enumerable.Repeat(1, 100).Select(i => new byte[arrLength]).ToList().ForEach(i => Emptied.Enqueue(i));

            var arr = TargetRepository.Samples;

            var sp = System.Diagnostics.Stopwatch.StartNew();


            {//reading data

                byte[] buf = new byte[BodyLength];

                var cnt = 0;

                var header = new byte[32];

                //var tmp = Emptied.Dequeue();

                //byte[] buff;

                byte a, b, c;
                int v1, v2;

                //var chn = TargetRepository.Channel1;// Channels[0];
                //TargetRepository.AdcSampleRate = SampleRate;

                var flag = false;

                Console.WriteLine("Starting read");

                var lastCounter = -1;

                while (true)
                {
                    //read next block
                    if (sport.BytesToRead == 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    {//header
                        sport.BaseStream.ReadArray(header);//adc_report binary

                        // var report = StructTools.RawDeserialize<ADC_Report>(adcReport, 0);
                        var cntr = BitConverter.ToInt32(header, 0);

                        var m1 = BitConverter.ToInt32(header, 4);

                        var m2 = BitConverter.ToInt32(header, 8);

                        if (cntr != lastCounter + 1)
                            //throw new Exception("Packet Loss! Try Reconnect...");

                        if (m1 != 0 || m2 != 0)
                            //throw new Exception("Packet Loss! Try Reconnect...");

                        lastCounter = cntr;

                    }

                    {
                        var buff = buf;
                        var data = buf;

                        sport.BaseStream.ReadArray(buf);

                        for (var j = 0; j < arrLength; j ++)
                        {
                            a = buff[j + 0];
                            //b = buff[j + 1];

                            //v1 = a + b * 256;

                            arr.Add((short)(a*16));

                            TotalReads += 1;

                        }
                    }


                    var ratio = TotalReads / sp.Elapsed.TotalSeconds;

                    cnt++;
                }


            }

            throw new NotImplementedException();
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
