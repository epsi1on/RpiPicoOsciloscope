using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows.Media.TextFormatting;
using System.Threading;
using System.IO;
using System.Windows.Markup;
using System.Linq;

namespace SimpleOsciloscope.UI
{

    public class RpiPicoDaqInterface: IDaqInterface
    {
    
        static RpiPicoDaqInterface()
        {
            //SampleRate = (int)UiState.Instance.CurrentRepo.SampleRate;
        }

        public int SampleRate ;
        public string PortName;

        public DataRepository TargetRepository { get; set; }

        private Queue<byte[]> Readed = new Queue<byte[]>();//those are filled with data
        private Queue<byte[]> Emptied = new Queue<byte[]>();//those that content are used and ready to be reused
        private object RLock = new object();//for Readed
        private object ELock = new object();//for ELock

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
                sport.ReadBufferSize = 1024 * 1000;//1000KB
            }

            sport.Open();
            
            string ver;

            //read device identifier
            {
                var dt = new byte[] { 1, 0, 1 };

                sport.Write(dt);

                Thread.Sleep(100);

                var l = 34;

                if (sport.BytesToRead != l)
                     throw new Exception("Unexpected resonse length, try unplug and replug the PICO");

                var buf = sport.ReadExplicitLength(l);

                var pass = 4;

                ver = Encoding.ASCII.GetString(buf, pass, buf.Length - pass);

                Console.WriteLine(ver);
            }

            int bitwidthCurr;
            var blockSize = 1000;//samples per block
            var blockCount = 5000;
            var bitwidth = 12;

            var sampleRate = 384000;// (int)UiState.Instance.CurrentRepo.SampleRate;


            {//send command for ADC
                var cmd = AdcCommand.Default();

                {
                    //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                    cmd.channel_mask = 0x01;
                    cmd.blocksize = (ushort)blockSize;
                    cmd.blocks_to_send = (ushort)blockCount;
                    cmd.infinite = 0;
                    cmd.clkdiv = (ushort)(48_000_000/ sampleRate); //rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)
                }


                var cmdBin = StructTools.RawSerialize(cmd);//serialize into 9 byte binary

                var tmp = BitConverter.ToString(cmdBin);

                sport.Write(new byte[] { 0x0A }, cmdBin);

                Thread.Sleep(10);
            }

            var arrLength = (blockSize * bitwidth) / 8;

            Enumerable.Repeat(1, 100).Select(i => new byte[arrLength]).ToList().ForEach(i => Emptied.Enqueue(i));

            var arr = TargetRepository.Samples;

            {//reading data

                byte[] buf = new byte[arrLength];
                
                var cnt = 0;

                var header = new byte[24];

                var tmp = Emptied.Dequeue();

                byte[] buff;

                byte a, b, c;
                int v1, v2;

                //var chn = TargetRepository.Channel1;// Channels[0];
                TargetRepository.SampleRate = SampleRate;

                var flag = false;

                while (true)
                {
                    //read next block

                    {//header
                        sport.BaseStream.ReadArray(header);//adc_report binary

                        // var report = StructTools.RawDeserialize<ADC_Report>(adcReport, 0);
                        bitwidthCurr = header[3];// report._data_bitwidth;

                        if (bitwidthCurr != bitwidth)
                            throw new Exception();
                    }

                    {
                        buff = buf;

                        sport.BaseStream.ReadArray(buf);

                        for (var j = 0; j < arrLength; j += 3)
                        {
                            a = buff[j + 0];
                            b = buff[j + 1];
                            c = buff[j + 2];

                            v1 = a + ((b & 0xF0) << 4);

                            v2 = (c & 0xF0) / 16 + (b & 0x0F) * 16 + (c & 0x0F) * 256;//ImproveMe: replace / 16 and * 16 and * 256 etc with bitwise operators

                            arr.Add((short)v1);
                            arr.Add((short)v2);
                        }

                        if (flag)
                        {
                            short[] t2 = new short[arr.Count];

                            arr.CopyTo(t2);
                        }

                    }

                    cnt++;
                }

                
            }

            throw new NotImplementedException();
        }



    }
}
