using Accord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public class DataRepository
    {

        public static readonly int RepoLength =
            //5_000_000 * 3;//1.5 M sample capacity, 3 sec for 500ksps
            5_00_000;//1.5 M sample capacity, 3 sec for 500ksps
        //public static readonly int ChannelCount = 1;
        public int SampleRate=500_000;//Sps
        //public List<ChannelData> Channels = new List<ChannelData>();// [ChannelCount];
        
        //public ChannelData Channel1 = new ChannelData(RepoLength);
        //public ChannelData Channel2 = new ChannelData(RepoLength);
        public FixedLengthList<short> Samples = new FixedLengthList<short>(RepoLength);

    }

    public class PointF
    {
        public float X, Y;
    }


    public class ChannelData_old
    {
        public double SampleRate;//Sample per second

        //FixedLengthList<double> Xs;//time
        FixedLengthList<short> Ys;//voltage

        public readonly int Length;

        public ChannelData_old(int l)
        {
            //Xs = new FixedLengthList<double>(l);
            Ys = new FixedLengthList<short>(l);
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

    public class FixedLengthList<T>
    {
        //will use a single array as circular

        public readonly int Count;

        long totalWrites;

        public long TotalWrites { get => totalWrites; private set => totalWrites = value; }

        private object lc = new object();

        private T[] arr;

        public FixedLengthList(int l)
        {
            arr = new T[l];
            Count= l;
        }

        
        public int Index = 0;
        

        public void Add(T item)
        {
            lock (lc)
            {
                //https://stackoverflow.com/q/33781853
                

                if (Index >= Count)
                    Index = 0;

                arr[Index] = item;

                Index++; // increment index

                totalWrites++;
            }
        }

        public void CopyTo(T[] other)
        {
            if (other.Length != Count)
                throw new Exception();

            lock (lc)
            {
                var L = this.Count;
                var idx = this.Index;
                var t = Index % L;

                var thisArr = this.arr;

                {
                    var def = default(T);
                    for (var i = 0; i < other.Length; i++)
                        other[i] = def;
                }

                for (int i = 0; i < t; i++)
                {
                    other[L - t + i] = thisArr[i];
                }


                for (int i = t; i < L; i++)
                {
                    other[i - t] = thisArr[i];
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if(index < 0 )
                    throw new Exception();
                return arr[(index + this.Index) % Count];
            }
        }
    }
}
