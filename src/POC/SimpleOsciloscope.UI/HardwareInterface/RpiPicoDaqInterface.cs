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
using Accord.MachineLearning;
using Accord;
using MathNet.Numerics.Distributions;

namespace SimpleOsciloscope.UI
{

    public class RpiPicoDaqInterface: IDaqInterface
    {
    
        static RpiPicoDaqInterface()
        {
            SampleRate = (int)UiState.Instance.CurrentRepo.SampleRate;
        }
        static int SampleRate ;

        public DataRepository TargetRepository { get; set; }


        public void AdcReadTest()
        {
            var sport = new SnifferSerial("COM6", 268435456);

            var bufl = new List<int>();

            {//https://stackoverflow.com/a/73668856
                sport.Handshake = Handshake.None;
                sport.DtrEnable = true;
                sport.RtsEnable = true;
                sport.StopBits = StopBits.One;
                sport.DataBits = 8;
                sport.Parity = Parity.None;
            }

            sport.Open();

            string ver;

            //read device identifier
            {
                var dt = new byte[] { 1, 0, 1 };

                sport.Write(dt);

                var ttp = sport.BytesToWrite;

                Thread.Sleep(100);

                var l = 34;

                if (sport.BytesToRead != l)
                    throw new Exception("Unexpected resonse length");

                var buf = sport.ReadExplicitLength(l);

                var pass = 4;

                ver = Encoding.ASCII.GetString(buf, pass, buf.Length - pass);
                Console.WriteLine(ver);
            }

            int bitwidth;
            var blockSize = 1000;//samples per block
            var blockCount = 9;

            {//send command for ADC
                var cmd = AdcCommand.Default();

                {
                    //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                    cmd.channel_mask = 0x01;
                    cmd.blocksize = (ushort)blockSize;
                    cmd.blocks_to_send = (ushort)blockCount;
                    cmd.infinite = 1;
                    cmd.clkdiv = (ushort)(48_000_000 / SampleRate); //rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)

                }


                var cmdBin = StructTools.RawSerialize(cmd);//serialize into 9 byte binary

                sport.Write(new byte[] { 0x0A }, cmdBin);

                Thread.Sleep(1000);

                
            }

            var fl = System.IO.File.OpenWrite("c:\\temp\\fl.bin");
            {//reading data
                var l = blockSize + blockSize / 2;

                var buf = new byte[l];

                var counter = 0;

                int v1, v2;
                uint tmp;
                byte a, b, c;

                //var chn = TargetRepository.Channel1;// Channels[0];

                //chn.SampleRate = SampleRate;
                var cnt = 0;


                while(true)
                {
                    if (sport.BytesToRead == 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                        


                    l = Math.Min(buf.Length, sport.BytesToRead);

                    var rdr = sport.Read(buf, 0, l);
                    fl.Write(buf, 0, rdr);

                    cnt += rdr;
                }


                for (int i = 0; i < blockCount; i++)
                {
                    var adcReport = sport.ReadExplicitLength(24);//adc_report binary

                    var report = StructTools.RawDeserialize<ADC_Report>(adcReport, 0);
                    bitwidth = report._data_bitwidth;

                    //read block
                    counter = 0;

                    while (counter < l)
                    {
                        var remain = l - counter;

                        var rdr = sport.Read(buf, counter, remain);
                        counter += rdr;
                    }

                    for (var j = 0; j < l; j += 3)
                    {
                        a = buf[j + 0];
                        b = buf[j + 1];
                        c = buf[j + 2];

                        v1 = a + ((b & 0xF0) << 4);

                        v2 = (c & 0xF0) / 16 + (b & 0x0F) * 16 + (c & 0x0F) * 256;//todo replace / 16 and * 16 with bitwise operators

                        bufl.Add(v1);
                        bufl.Add(v2);
                        //throw new NotImplementedException();
                        //chn.Add((short)v1);
                        //chn.Add((short)v2);
                        //cnt++;
                        //cnt++;
                    }
                }

                /*while (bufl.Count<blockSize*(blockCount-1))
                {
                    counter = 0;

                    while (counter < l)
                    {
                        var remain = l - counter;

                        var rdr = sport.Read(buf, counter, remain);
                        counter += rdr;
                    }


                    for (var j = 0; j < l; j += 3)
                    {
                        a = buf[j + 0];
                        b = buf[j + 1];
                        c = buf[j + 2];

                        v1 = a + ((b & 0xF0) << 4);

                        v2 = (c & 0xF0) /16 + (b & 0x0F) *16 + (c & 0x0F) *256;//todo replace / 16 and * 16 with bitwise operators

                        bufl.Add(v1);
                        bufl.Add(v2);
                        //throw new NotImplementedException();
                        //chn.Add((short)v1);
                        //chn.Add((short)v2);
                        //cnt++;
                        //cnt++;
                    }
                }*/

                Guid.NewGuid();
            }
        }

        public void StartASync()
        {
            //new Thread(StartRead) { Priority = ThreadPriority.Highest }.Start();

            //new Thread(StartProcess).Start();
        }

        

        private Queue<byte[]> Readed = new Queue<byte[]>();//those are filled with data
        private Queue<byte[]> Emptied = new Queue<byte[]>();//those that content are used and ready to be reused
        private object RLock = new object();//for Readed
        private object ELock = new object();//for ELock

        public void StartSync()
        {
            var sport = new SnifferSerial("COM6", 268435456);
            
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
                     throw new Exception("Unexpected resonse length");

                var buf = sport.ReadExplicitLength(l);

                var pass = 4;

                ver = Encoding.ASCII.GetString(buf, pass, buf.Length - pass);

                Console.WriteLine(ver);
            }

            int bitwidthCurr;
            var blockSize = 1000;//samples per block
            var blockCount = 15;
            var bitwidth = 12;

            {//send command for ADC
                var cmd = AdcCommand.Default();

                {
                    //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                    cmd.channel_mask = 0x01;
                    cmd.blocksize = (ushort)blockSize;
                    cmd.blocks_to_send = (ushort)blockCount;
                    cmd.infinite = 1;
                    cmd.clkdiv = (ushort)(48_000_000/SampleRate); //rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)
                }


                var cmdBin = StructTools.RawSerialize(cmd);//serialize into 9 byte binary

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
