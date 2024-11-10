using System;

namespace SimpleOsciloscope.UI
{
    public class FixedLengthListRepo<T>: ISampleRepository<T>
    {
        //will use a single array as circular

        public readonly int FixedLength;

        long totalWrites;

        public long TotalWrites { get => totalWrites; private set => totalWrites = value; }

        private object lc = new object();

        private T[] arr;


        public FixedLengthListRepo(int l)
        {
            arr = new T[l];
            FixedLength= l;
        }

        
        public int Index = 0;
        

        public void Add(T item)
        {
            lock (lc)
            {
                //https://stackoverflow.com/q/33781853
                

                if (Index >= FixedLength)
                    Index = 0;

                arr[Index] = item;

                Index++; // increment index

                totalWrites++;
            }
        }

        public void CopyToOld(T[] other)
        {
            if (other.Length != FixedLength)
                throw new Exception();

            lock (lc)
            {
                var L = this.FixedLength;
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

        public void CopyTo(T[] other)
        {
            if (other.Length != FixedLength)
                throw new Exception();

            Array.Clear(other, 0, other.Length);

            var tmp = new T[1000];

            int sz;

            unsafe
            {
                sz = sizeof(T);
            }

            lock (lc)
            {
                var L = this.FixedLength;
                var idx = this.Index;
                var t = Index % L;

                var thisArr = this.arr;

                Buffer.BlockCopy(thisArr, sz * t, other, 0, sz * (L - t));

                Buffer.BlockCopy(thisArr, 0, other, sz * (L - t), sz * t);

            }
        }

        long readIndex = 0;

        public int Read(T[] arr, int offset, int length)
        {
            var buf = 0;

            for (int i = 0; i < length; i++)
            {
                //arr[i + offset] = this[readIndex+i]
            }

            readIndex += buf;
            return buf;
        }

        public T this[int index]
        {
            get
            {
                if(index < 0 )
                    throw new Exception();
                return arr[(index + this.Index) % FixedLength];
            }
        }
    }
}
