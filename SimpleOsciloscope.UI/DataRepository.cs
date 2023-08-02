using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public class DataRepository
    {
        public static readonly int RepoLength = 500000 * 3;//1.5 M sample capacity, 3 sec for 500ksps
        public static readonly int ChannelCount = 1;

        public FixedLengthList<short>[] Channels = new FixedLengthList<short>[ChannelCount];

    }

    public class FixedLengthList<T>
    {
        //will use a single array as circular

        public readonly int Count;

        private long totalWrites;

        public long TotalWrites { get => totalWrites; set => totalWrites = value; }


        private T[] arr;

        public FixedLengthList(int l)
        {
            arr = new T[l];
        }

        int Index = 0;

        

        public void Add(T item)
        {
            //https://stackoverflow.com/q/33781853
            Index++; // increment index

            if (Index >= Count)
                Index = 0;

            arr[Index] = item;

            
            totalWrites++;
        }

        public T this[int index]
        {
            get
            {

                return arr[(index + Index) % Count];
            }
        }
    }
}
