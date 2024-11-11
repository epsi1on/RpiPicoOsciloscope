using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

namespace SimpleOsciloscope.UI.Audio
{
    /*
    public class NAudioPlayer : ISampleRepository<short>, IWaveProvider
    {

        private ConcurrentQueue<short> Data = new ConcurrentQueue<short>();

        long index;

        public short this[int i]
        {
            get
            {
                if (index < 0)
                    throw new Exception();


                return 0;// arr[(index + this.Index) % FixedLength];
            }
        }

        public void Add(short item)
        {
            //lock(lc)
            {
                Data.Enqueue(item);
                index++;
            }
        }

        public void CopyTo(short[] other)
        {
            
        }


        private WaveFormat waveFormat;

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }


        public NAudioPlayer()
            : this(44100, 1)
        {
        }

        public NAudioPlayer(int sampleRate, int channels)
        {
            SetWaveFormat(sampleRate, channels);

            Amplitude = 0.25f;
        }

        public void SetWaveFormat(int sampleRate, int channels)
        {
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            WaveBuffer waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = ReadInternal(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        int sample;
        public float Amplitude { get; set; } = 1.0f;

        public long TotalWrites => throw new NotImplementedException();


        object lc = new object();

        private int ReadInternal (float[] buffer, int offset, int sampleCount)
        {
            var sampleRate = 0.0;// WaveFormat.SampleRate;

            short dt;

            int n ;

            var sc = 100f;

            //lock(lc)
            {

                for ( n = 0; n < sampleCount; n++)
                {
                    if (Data.Count == 0 || !Data.TryDequeue(out dt))
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    float sig = dt/4096f;

                    buffer[n + offset] = (float)(sc * Amplitude * sig);
                }
            }


            if (n == 0)
                Guid.NewGuid();


            return n;
        }

        public int Read(short[] arr, int offset, int length)
        {
            throw new NotImplementedException();
        }
    }

    */
}
