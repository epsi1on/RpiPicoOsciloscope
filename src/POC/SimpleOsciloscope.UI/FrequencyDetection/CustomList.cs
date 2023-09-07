using System;

namespace SimpleOsciloscope.UI.FrequencyDetection
{
    public class CustomList<T>
    {
        public int Count { get; set; }

        T[] Array;

        public CustomList(T[] array)
        {
            Array = array;
        }

        public T this[int i]
        {
            get { return Array[i]; }
            set
            {
                if (i >= Count)
                    throw new Exception();

                Array[i] = value;
            }

        }
    }
}
