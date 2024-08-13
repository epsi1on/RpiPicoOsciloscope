using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public interface ISampleRepository<T> 
    {
        void Add(T item);

        void CopyTo(T[] other);

        //T this[int i] { get; }

        long TotalWrites { get; }


        int Read(T[] arr, int offset, int length);

    }
}
