using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.InterfaceUi.FakeDaq
{
    public class FakeDaqInterface : IDaqInterface
    {

        public FakeDaqUserSettings UserSettings;
        public FakeDaqCalibrationData CalibrationData;


        public DataRepository TargetRepository { get ; set ; }

        public int AdcResolutionBits => 12 ;

        public double AdcMaxVoltage => 3.3;

        public long AdcSampleRate
        {
            get
            {
                return UserSettings.SampleRate;
            }
        }


        public bool StopFlag = false;

        public void DisConnect()
        {
            StopFlag = true;
        }

        public void StartSync()
        {
            var rnd = new Random();

            var resolution = 12;
            short adcMax = (short)(1 << resolution);//2 ^ resolution
            var halfMax = adcMax / 2;

            adcMax--;//just for safety where sin=1.0

            UiState.AdcConfig.Set(this);

            StopFlag = false;

            var tmr = Stopwatch.StartNew();

            var sampleRate = UserSettings.SampleRate;
            var freq = UserSettings.Frequency;
            var offset = UserSettings.Offset;
            var ampl = UserSettings.Amplitude;
            var noise = UserSettings.Noise;


            var alpha = UiState.Instance.CurrentRepo.LastAlpha = 2 * offset / adcMax;
            var beta = UiState.Instance.CurrentRepo.LastBeta = ampl - offset;

            var rr = TargetRepository.Samples;
            //var rrf = TargetRepository.SamplesF;

            var l = 100_000;

            long counter = 0;

            var dt = 1.0 / sampleRate;

            var omega = 2 * Math.PI * freq;

            var noiseAdc = (short)(noise / alpha);

            var haveNoise = noiseAdc != 0;

            while (!StopFlag)
            {
                counter++;

                tmr.Restart();

                var cnt2 = 0;

                while (cnt2++ < l)
                {
                    var t = counter * dt + cnt2 * dt;

                    var sin = Math.Sin(omega * t);

                    short res = (short)(sin * halfMax + halfMax);

                    //rrf.Add((float)(res * alpha + beta));
                    if (haveNoise)
                        res = (short)(res + rnd.Next(-noiseAdc, noiseAdc));

                    if (res > adcMax)
                        res = adcMax;

                    if (res < 0)
                        res = 0;

                    rr.Add(res);
                }

                tmr.Stop();

                var time = tmr.Elapsed.TotalSeconds;
                var length = l;

                var expectedMilis = (1000 * l) / sampleRate;

                var current = time*1000;

                var wait = expectedMilis - current;

                if(wait > 0)
                {
                    Thread.Sleep((int)(wait ));
                }

                counter += cnt2;

                
            }

            Guid.NewGuid().ToString();
        }

        public void StopAdc()
        {
            StopFlag = true;
        }
    }
}
