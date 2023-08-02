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
using MadWizard.WinUSBNet;
using System.Threading;
using System.IO;
using System.Windows.Markup;

namespace SimpleOsciloscope.UI
{

    public class DaqInterface
    {
        public DataRepository TargetRepository;

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

                var buf = sport.Read(l);

                var pass = 4;

                ver = Encoding.ASCII.GetString(buf, pass, buf.Length - pass);
                Console.WriteLine(ver);
            }

            int bitwidth;
            var blockSize = 100;//samples per block
            var blockCount = 10;

            {//send command for ADC
                var cmd = Command.Default();

                {
                    //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                    cmd.channel_mask = 0x02;
                    cmd.blocksize = (ushort)blockSize;
                    cmd.blocks_to_send = (ushort)blockCount;
                    cmd.infinite = 1;
                    cmd.clkdiv = 96;
                }


                var cmdBin = StructTools.RawSerialize(cmd);//serialize into 9 byte binary

                sport.Write(new byte[] { 0x0A }, cmdBin);

                Thread.Sleep(1000);

                var adcReport = sport.Read(24);//adc_report binary

                var report = StructTools.RawDeserialize<ADC_Report>(adcReport, 0);
                bitwidth = report._data_bitwidth;
            }

            {//reading data
                var l = blockSize + blockSize / 2;

                var buf = new byte[l];

                var counter = 0;

                int v1, v2;
                uint tmp;
                byte a, b, c;

                var chn = TargetRepository.Channels[0];

                var cnt = 0;

                //for (int i = 0; i < blockCount; i++)
                while(true)
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
                        a = buf[j];
                        b = buf[j + 1];
                        c = buf[j + 2];

                        v1 = a + ((b & 0xF0) << 4);
                        v2 = (c & 0xF0) / 16 + (b & 0x0F) * 16 + (c & 0x0F) * 256;

                        chn.Add((short)v1);
                        chn.Add((short)v2);
                        cnt++;
                    }
                }
            }

            throw new NotImplementedException();
        }



    }
}
