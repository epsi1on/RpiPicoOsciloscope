using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.HardwareInterface
{

    public class FakeDaqInterface : IDaqInterface
    {
        public double AdcMaxVoltage { get; set; }
        public int AdcResolutionBits { get; set; }

        public long AdcSampleRate { get; set; }

        public DataRepository TargetRepository { get; set; }

        public int dataRate = 5;//sample per second
        public double Frequency = 10.234567;//frequency of generated signal

        public void StartSync()
        {
            //TargetRepository.AdcSampleRate = dataRate;

            long cnt = 0;

            var c2 = 5;

            //var chn = TargetRepository.Channel1;// Channels[0];
            //TargetRepository.AdcSampleRate = dataRate;

            SpinWait sw = new SpinWait();

            //var dx = 1 / dataRate;// 

            var sp = System.Diagnostics.Stopwatch.StartNew();

            

            var fs = dataRate;//sampling
            var f = Frequency;//frequency

            var dt = 1.0 / fs;

            var rnd = new Random();

            var arr = TargetRepository.Samples;

            while (true)
            {
                var time = cnt * dt;// sp.Elapsed.TotalSeconds*1e6;

                //var x = start + (end - start) / partitions * i;

                var x = Math.E + Math.PI * f * time;

                //var y = Math.Sin(x);// +
                                    //(Math.Sin(5 * x) + Math.Sin(3 * x));
                var b = Evaluate(time);
                //y = time % wl;

                var y = 2048 * b + 2048;

                var rn = 0.5 - rnd.NextDouble();

                //y += rn * 0.01;
                arr.Add((short)y);

                //Thread.Sleep() not works for less than milisecond

                //if (cnt > chn.Length)
                    //sw.SpinOnce();

                cnt++;
            }
        }


        private double Evaluate(double t)
        {
            return Math.Sin(t);
        }

        private void SleepMicrosecond(double microseconds)
        {

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
