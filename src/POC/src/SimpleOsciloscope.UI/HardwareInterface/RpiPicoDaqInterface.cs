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
using System.CodeDom;
using System.IO.IsolatedStorage;

namespace SimpleOsciloscope.UI.HardwareInterface
{

    public class RpiPicoDaqInterface: IDaqInterface
    {
        static readonly byte Ch1_10x_pin = 19;
        static readonly byte Ch1_acdc_pin = 19;

        static readonly double Ch1_exp_pullup_val = 10e3;//channel1 expand domain resistor (see schematics)
        static readonly double Ch1_exp_pulldown_val = 10e3;


        static RpiPicoDaqInterface()
        {
            //SampleRate = (int)UiState.Instance.CurrentRepo.SampleRate;
        }

        public RpiPicoDaqInterface(string portName, long adcSampleRate)
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
                sport.ReadBufferSize = 1024 * 1000;//1000KB
            }

            sport.Open();
            
            

            int bitwidthCurr;
            var blockSize = 100;//samples per block
            var blockCount = 1;
            var bitwidth = 12;
            var sampleRate = (int)this.AdcSampleRate;
            var infinite = true;

            {//send command for ADC init
                var cmd = AdcConfig.Default();

                {
                    //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                    cmd.channel_mask = 0x01;
                    cmd.blocksize = (ushort)blockSize;
                    cmd.blocks_to_send = (uint)blockCount;
                    cmd.infinite = infinite ? (byte)1 : (byte)0;
                    cmd.clkdiv = (ushort)(48_000_000 / sampleRate); //rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)
                }


                var cmdBin = cmd.ToArray();// StructTools.RawSerialize(cmd);//serialize into 9 byte binary

                var tmp = BitConverter.ToString(cmdBin);

                sport.Write(new byte[] { 0x0A }, cmdBin);

                var tmpp = new byte[10000];

                while(true)
                {
                    if (sport.BytesToRead == 0)
                        Thread.Sleep(100);

                    if (sport.BytesToRead == 0)
                        break;
                    
                    sport.Read(tmpp, 0, tmp.Length);

                }

                Thread.Sleep(10);
            }

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



            if (ver != "rp2daq_231005_E662588817786A23")
                throw new Exception("Invalid firmware version");

            {//set gpio push button callback
                //var pins = new byte[] {19,21,2 };

                {//for pin 21
                    byte gp = 21;
                    var cmd = new byte[] { 0x05, 0x03, gp, 0x01, 0x01 };
                    sport.Write(cmd);
                }

            }

            {//send command for ADC
                var cmd = AdcConfig.Default();

                {
                    //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                    cmd.channel_mask = 2;
                    cmd.blocksize =  (ushort)blockSize;
                    cmd.blocks_to_send = 1;// (ushort)blockCount;
                    cmd.infinite = 1;
                    cmd.clkdiv =  (ushort)(48_000_000/ sampleRate); //rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)

                    cmd.waits_for_trigger = 0;
                    cmd.waits_for_usb = 0;
                }


                var cmdBin = cmd.ToArray();// StructTools.RawSerialize(cmd);//serialize into 9 byte binary

                var tmp = BitConverter.ToString(cmdBin);

                sport.Write(new byte[] { 0x0e, 0x04 }, cmdBin);

                Thread.Sleep(10);
            }

            var arrLength = (blockSize * bitwidth) / 8;

            //Enumerable.Repeat(1, 100).Select(i => new byte[arrLength]).ToList().ForEach(i => Emptied.Enqueue(i));

            var arr = TargetRepository.Samples;

            {//reading data

                byte[] buf = new byte[arrLength];
                
                var cnt = 0;

                var adcHeaderLength = 25;
                var gpioHeaderLength = 9;

                var adcReportHeader = new byte[adcHeaderLength];
                var gpioReportHeader = new byte[gpioHeaderLength];

                var adc1_alpha = 1.0;
                var adc1_beta = 1.0;   //volt = adc1*adc1_alpha + adc1_beta

                //var tmp = Emptied.Dequeue();

                byte[] buff;

                byte a, b, c;
                int v1, v2;

                //var chn = TargetRepository.Channel1;// Channels[0];
                //TargetRepository.AdcSampleRate = SampleRate;

                var flag = false;

                Console.WriteLine("Starting read");


                var blocks = 0;


                //var str = System.IO.File.OpenWrite("c:\\temp\\tt.bin");

                //sport.BaseStream.CopyTo(str);


                var tmp = new byte[1];

                while (true)
                {
                    //read next block
                    if (sport.BytesToRead == 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    byte packageCode;

                    {
                        sport.BaseStream.ReadArray(tmp);
                        packageCode = tmp[0];
                    }

                    if (packageCode == 0x03)//gpio change
                    {
                        sport.BaseStream.ReadArray(gpioReportHeader);

                        var pin = gpioReportHeader[0];
                        var newVal = gpioReportHeader[4];

                        if (newVal != 4 && newVal != 8) 
                        {
                            throw new Exception();
                        }

                        var isPressed = newVal == 4;


                        if (pin == Ch1_10x_pin)
                        {
                            if(isPressed)
                            {
                                adc1_alpha = 1.0/3;
                                adc1_beta = 3.3/2;
                            }
                        }

                        if (pin == Ch1_acdc_pin)
                        {

                        }
                    }


                    if (packageCode == 0x04)//adc report
                    {
                        sport.BaseStream.ReadArray(adcReportHeader);//adc_report binary

                        bitwidthCurr = adcReportHeader[2];// report._data_bitwidth;

                        if (bitwidthCurr != bitwidth)
                            throw new Exception("Packet Loss! Try Reconnect...");

                        buff = buf;

                        sport.BaseStream.ReadArray(buf);

                        {
                            //var tm = new byte[2];
                            //sport.BaseStream.ReadArray(tm);

                            //if (tm[0] != 4) throw new Exception();
                        }


                        for (var j = 0; j < arrLength; j += 3)
                        {
                            a = buff[j + 0];
                            b = buff[j + 1];
                            c = buff[j + 2];

                            v1 = a + ((b & 0xF0) << 4);

                            v2 = (c & 0xF0) / 16 + (b & 0x0F) * 16 + (c & 0x0F) * 256;//ImproveMe: replace / 16 and * 16 and * 256 etc with bitwise operators

                            arr.Add((short)v1);
                            arr.Add((short)v2);

                            TotalReads += 2;
                        }
                    }

                    {
                        //var tm = new byte[1];
                        //sport.BaseStream.ReadArray(tm);

                        //if (tm[0] != 4) throw new Exception();
                    }

                    cnt++;
                }

                
            }

            throw new NotImplementedException();
        }

       
    }
}
