using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows.Media.TextFormatting;
using MadWizard.WinUSBNet;
using System.Threading;
using System.IO;

namespace SimpleOsciloscope.UI
{

    public class DaqInterface
    {
        public static void Test()
        {
            var sport = new SerialPort("COM6", 9600);

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

                sport.Write(dt, 0, dt.Length);

                var ttp = sport.BytesToWrite;

                Thread.Sleep(100);

                var l = 34;

                if (sport.BytesToRead != l)
                    throw new Exception("Unexpected resonse length");

                var buf = new byte[l];

                sport.Read(buf, 0, l);

                var pass = 4;

                ver = Encoding.ASCII.GetString(buf, pass, buf.Length - pass);
            }


            {//send command for ADC
                var cmd = Command.Default();

                cmd.channel_mask = 16;

                var cmdBin = StructTools.RawSerialize(cmd);//serialize into 9 byte binary

                var tmp = ByteArrayToString(cmdBin);

                sport.Write(cmdBin, 0, cmdBin.Length);

                Thread.Sleep(1000);

                var adcReport = new byte[24];

                

                sport.Read(adcReport, 0, adcReport.Length);//adc_report binary

                tmp = ByteArrayToString(adcReport);

                var report = StructTools.RawDeserialize<ADC_Report>(adcReport, 0);
            }

            {
                var dt2 = new byte[sport.BytesToRead];

                sport.Read(dt2, 0, dt2.Length);

                var t23=ByteArrayToString(dt2);
            }

            throw new NotImplementedException();
        }


        //convert binary data to hex string
        //https://stackoverflow.com/a/311179
        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
           
            foreach (byte b in ba)
                hex.AppendFormat("0x{0:x2},", b);

            return hex.ToString();
        }


     
    }
}
