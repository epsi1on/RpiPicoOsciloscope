using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static SimpleOsciloscope.UI.HardwareInterface.RpiPicoDaqInterface;

namespace SimpleOsciloscope.UI
{
    public class UiState
    {
        public static class AdcConfig
        {
            public static int ResolutionBits { get;private set; }
            public static double MaxVoltage { get; private set; }
            public static long SampleRate;

            public static void Set(IDaqInterface ifs)
            {
                ResolutionBits = ifs.AdcResolutionBits;
                MaxVoltage = ifs.AdcMaxVoltage;
                SampleRate = ifs.AdcSampleRate;
            }
        }

        public static readonly double RenderFramerate = 15;

        public static readonly PixelFormat BitmapPixelFormat = PixelFormats.Bgra32;

        public DataRepository CurrentRepo;

        public readonly int RenderBitmapWidth = 500;
        public readonly int RenderBitmapHeight = 500;

        public readonly AdcChannelInfo[] Channels;


        public static UiState Instance = new UiState();

        private UiState()
        {
            CurrentRepo = new DataRepository();
            //CurrentRepo.Channels.Add(new ChannelData(DataRepository.RepoLength));

            Channels = InitChannels();
        }



        public static AdcChannelInfo[] InitChannels()
        {
            var lst = new List<AdcChannelInfo>();

            var ids = new RpiPicoDaqInterface.Rp2040AdcChannels[]{
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio28,
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio26,
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio27,
                    //RpiPicoDaqInterface.Rp2040AdcChannels.InternalReference,
                    //RpiPicoDaqInterface.Rp2040AdcChannels.InternalTempratureSensor
                    };


            var SwPins = new int[] { 19, 21, 18 };
            var AcDcPins = new int[] { 20, -1, -1 };
            //var adcPins = new byte[] { 28, 26, 27 };

            var pins = AdcPins();

            //var g26 = pins.FindFirstIndexOf(i => i == 26);
            //var g27 = pins.FindFirstIndexOf(i => i == 27);
            //var g28 = pins.FindFirstIndexOf(i => i == 28);

            var titles = new string[] {
                        "1",
                        "2",
                        "3",
                        "VRef ",
                        "Tmpr"
                    };

            var descs = new string[] {
                        "ADC Probe",
                        "ADC Probe",
                        "ADC Probe",
                        "Internal Voltage Reference",
                        "Internal Temprature Sensor" };

            for (var i = 0; i < 3; i++)
            {
                var ch = InitChannel(i, ids[i], SwPins[i], AcDcPins[i]);

                ch.Title= titles[i];
                ch.Description = descs[i];

                lst.Add(ch);
            }


            var tmp = lst.ToArray();//.OrderBy(i => i.Title).ToArray();

            return tmp;
        }


        public static byte[] AdcPins()
        {
            return new byte[] { 28, 26, 27 };
        }

        private static AdcChannelInfo InitChannel(int index, Rp2040AdcChannels chn, int swPin, int acdcPin)
        {

            //var AdcPins = new int[] { 26, 27, 28 };
            //var SwPins = new int[] { 19, 21, 18 };
            //var AcDcPins = new int[] { 20, -1, -1 };

            double normalAlpha, normalBeta;
            double _10xAlpha, _10xBeta;

            normalAlpha = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_alpha_off"]);
            normalBeta = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_beta_off"]);

            _10xAlpha = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_alpha_on"]);
            _10xBeta = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_beta_on"]);

            //var pns = AdcPins();
            //var adcPin = pns[chnId];
            //var swPin = SwPins[chnId];
            //var acdcPin = AcDcPins[chnId];

            var ch1 = new AdcChannelInfo(index, swPin, acdcPin, 
                normalAlpha, normalBeta,
                _10xAlpha, _10xBeta, chn);

            return ch1;
        }


    }



    public static class Temps
    {
        public static double Temp;
    }
}
