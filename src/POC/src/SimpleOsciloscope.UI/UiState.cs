using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

        public static readonly double RenderFramerate = 10;

        public static PixelFormat BitmapPixelFormat = PixelFormats.Bgr24;

        public DataRepository CurrentRepo;

        public int RenderBitmapWidth = 500;
        public int RenderBitmapHeight = 500;


        public static UiState Instance = new UiState();

        private UiState()
        {
            CurrentRepo = new DataRepository();
            //CurrentRepo.Channels.Add(new ChannelData(DataRepository.RepoLength));
        }

    }



    public static class Temps
    {
        public static double Temp;
    }
}
