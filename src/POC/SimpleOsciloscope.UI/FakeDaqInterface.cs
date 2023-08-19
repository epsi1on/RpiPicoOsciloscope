using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public interface IDaqInterface
    {
        void StartSync();

        DataRepository TargetRepository { get; set; }
    }

    public class FakeDaqInterface : IDaqInterface
    {
        public DataRepository TargetRepository { get; set; }

        public double dataRate = 8_000_000;//8Ksps
        public double Frequency = 1000;//frequency of generated signal

        public void StartSync()
        {
            TargetRepository.SampleRate = dataRate;

            long cnt = 0;

            var c2 = 5;

            var chn = TargetRepository.Channel1;// Channels[0];

            SpinWait sw = new SpinWait();

            //var dx = 1 / dataRate;// 

            var sp = System.Diagnostics.Stopwatch.StartNew();

            var f = Frequency;
            var wl = 1 / f;

            while (true)
            {
                var time = sp.Elapsed.TotalSeconds;

                var x = 2 * Math.PI * Frequency * time;

                var y = (Math.Sin(5*x) + Math.Sin(3 * x));

                y = time % wl;

                chn.Add(time, y);

                //Thread.Sleep() not works for less than milisecond

                //sw.SpinOnce();

                cnt++;
            }
        }

        private void SleepMicrosecond(double microseconds)
        {

        }
    }
}
