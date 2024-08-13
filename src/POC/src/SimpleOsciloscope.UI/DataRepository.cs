using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public class DataRepository
    {

        public static readonly double RepoLengthSecs = 1;

        //public static readonly int RepoLength =
        //5_000_000 * 3;//1.5 M sample capacity, 3 sec for 500ksps
        //    5_00_000;//1.5 M sample capacity, 3 sec for 500ksps
        //public static readonly int ChannelCount = 1;
        //public int AdcSampleRate = 500_000;//Sps
        //public List<ChannelData> Channels = new List<ChannelData>();// [ChannelCount];

        //public int AdcMaxValue = 4096;//rpi pico: 4096, arduino nano 1024
        //public double AdcMaxVoltage = 3.3;//rpi pico: 3.3, arduino nano 1024


        //public ChannelData Channel1 = new ChannelData(RepoLength);
        //public ChannelData Channel2 = new ChannelData(RepoLength);
        public ISampleRepository<short> Samples;//= new FixedLengthList<short>(RepoLength);

        public ISampleRepository<float> SamplesF;//= new FixedLengthList<short>(RepoLength);

        //public ISampleRepository<short> Samples;//= new FixedLengthList<short>(RepoLength);

        public void Init(int sampleRate)
        {
            UiState.AdcConfig.SampleRate = sampleRate;
            //AdcSampleRate = sampleRate;

            var lng = (int)(sampleRate * RepoLengthSecs);

            Samples = new FixedLengthListRepo<short>(lng);
            SamplesF = new FixedLengthListRepo<float>(lng);
        }
    }

    public class PointF
    {
        public float X, Y;
    }


    public class ChannelData_old
    {
        public double SampleRate;//Sample per second

        //FixedLengthList<double> Xs;//time
        FixedLengthListRepo<short> Ys;//voltage

        public readonly int Length;

        public ChannelData_old(int l)
        {
            //Xs = new FixedLengthList<double>(l);
            Ys = new FixedLengthListRepo<short>(l);
            Length= l;
        }

        internal void Add(double x, double y)
        {
            lock (lc)
            {
                //Xs.Add(x);
                Ys.Add((short)y);
                Sets++;
            }
        }

        internal void Add(short x, short y)
        {
            lock (lc)
            {
                //Xs.Add(x);
                Ys.Add(y);
                Sets++;
            }
        }


        internal void Add(double y)
        {
            lock (lc)
            {
                //Xs.Add(x);
                Ys.Add((short)y);
                Sets++;
            }
        }

        internal void Add(short y)
        {
            lock (lc)
            {
                //Xs.Add(x);
                Ys.Add(y);
                Sets++;
            }
        }

        public void Copy(double[] xs, double[] ys)
        {
            lock (lc)
            {
                //Xs.CopyTo(xs);
                Copy(ys);
            }
        }

        public void Copy(double[] ys)
        {

            //throw new NotImplementedException();
            lock (lc)
            {
                //Xs.CopyTo(xs);
                //Ys.CopyTo(ys);
            }
        }

        public void Copy(short[] ys)
        {
            lock (lc)
            {
                //Xs.CopyTo(xs);
                Ys.CopyTo(ys);
            }
        }

        object lc = new object();

        public long Sets { get; private set; }
    }
}
