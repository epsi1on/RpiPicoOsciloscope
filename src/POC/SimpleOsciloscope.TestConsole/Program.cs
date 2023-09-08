using SimpleOsciloscope.UI;
using SimpleOsciloscope.UI.FrequencyDetection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestInterpolate();
            //testCrossColl();
            //testSimpleCrossColl();
            //testFreq2();
            //testHistogramRegions();
            //TestAdcRead();
            //TestDirect();
            //TestDaqInterface();
            //testFreq3();
            TestBitmap();
            Console.ReadKey();


        }

        static void TestBitmap()
        {
            var w = 500;
            var h = 500;

            var bmp = new RgbBitmap(w,h);
            var b2 = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgr24, null);

            ImageUtil.CopyToBitmap(bmp, b2);
        }

        static void testFreq3()
        {
            var samples = System.IO.File.ReadAllLines("C:\\temp\\samples.csv");

            var arr = new short[samples.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = short.Parse(samples[i]);
            }

            var fs = 500_000;

            double F, phi;

            var dtr = new CorrelationBasedFrequencyDetector();
            dtr.MaxCrosses = 10;

            var t2 = dtr.TryGetFrequency(arr, fs, out F,out phi);


        }

        static void TestDaqInterface()
        {
            var ifs = new RpiPicoDaqInterface();
            ifs.StartSync();
        }


        static void TestDirect()
        {
            new Thread(ReadDirect).Start();
            new Thread(PrintDirect).Start();
        }

        static ConcurrentStack<string> LastHeaders = new ConcurrentStack<string>();

        static void PrintDirect()
        {
            string header;

            while(true)
            {
                if (LastHeaders.TryPop(out header))
                {
                    Console.WriteLine(header);
                }
                Thread.Sleep(10);
            }
           
        }

        static void ReadDirect()
        {
            var sport = new SerialPort("COM6", 268435456);

            {//https://stackoverflow.com/a/73668856
                sport.Handshake = Handshake.None;
                sport.DtrEnable = true;
                sport.RtsEnable = true;
                sport.StopBits = StopBits.One;
                sport.DataBits = 8;
                sport.Parity = Parity.None;
                sport.ReadBufferSize = 1024 * 10000;
            }

            sport.Open();
            
            string ver;

            //read device identifier
            {
                var dt = new byte[] { 1, 0, 1 };

                sport.Write(dt, 0, dt.Length);
                
                Thread.Sleep(100);

                var l = 34;

                if (sport.BytesToRead != l)
                    throw new Exception("Unexpected Resonse Length");

                dt = new byte[34];

                sport.Read(dt, 0, dt.Length);
            }

            {
                var dt = new byte[] { 0x0A, 0x04, 0x10, 0xe8, 0x03, 0x01, 0x01, 0x00,96, 0x00 };

                sport.Write(dt, 0, dt.Length);
            }

            var str = sport.BaseStream;

            var header = new byte[24];
            var samples = new byte[1500];

            while (true)
            {
                //ReadExactAmount(sport, header.Length, header);
                //ReadExactAmount(sport, samples.Length, samples);
                Extensions.ReadArray(sport.BaseStream, header);
                Extensions.ReadArray(sport.BaseStream, samples);

                LastHeaders.Push(BitConverter.ToString(header));
            }

            
        }


        static void ReadExactAmount(SerialPort stream,int count, byte[] buffer)
        {
            int offset = 0;

            while (offset < count)
            {
                int read = stream.BaseStream.Read(buffer, offset, count - offset);

                if (read == 0)
                    Thread.Sleep(1);
                
                offset += read;
            }
        }

        static void Write(SerialPort stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }


        static void TestAdcRead()
        {
            var ifs = new RpiPicoDaqInterface();

            //ifs.AdcReadTest();
        }

        static void testSimpleCrossColl()
        {
            double fs = 10;//sampling
            double f = 1;//frequency
            var n = 10000;

            var cnt = n * (long)fs;

            var ys = new double[n + 100];
            var xs = new double[n + 100];

            var dt = 1.0 / fs;

            for (var i = 0; i < n; i++)
            {
                var t = i * dt;

                var x = Math.Sin(2 * Math.PI * f* t);

                ys[i] = x;
                xs[i] = t;
            }

            var cc = new SimpleCrossCorrelate();

            var tau = 0 * 0.3 * dt;

            cc.ys = ys;
            cc.n = n;
            
            cc.Dx = 1 / fs;


            for (var i = 0; i < 100; i+=10)
            {
                cc.Tau = (i + 0.50) / fs;

                var I = cc.CrossCollerate();
                Console.WriteLine("{0}, {1}", cc.Tau, I);
            }


            Console.ReadKey();
        }

        static void testHistogramRegions()
        {
            var rnd = new Random();

            var n = 10000;
            var m = 100000;

            var data = new double[m];

            for (int i = 0; i < n; i++)
            {
                var rn = 0.1 * (0.5 - rnd.NextDouble());
                data[i] = 27 + rn;
            }

            data[n - 1] = 1;

            {

                data[10] = 17;
                data[11] = 20;
            }

            var hist = HistogramData.Generate(data, n, 10);
            
            var regions = HistogramUtil.ClusterSimple(hist);

            var walls = hist.GetWalls();

            //foreach (var item in regions)
            {
                for (int i = 0; i < hist.Groups; i++)
                {
                    var st = walls[i];
                    var en = walls[i + 1];
                    var cnt = hist[i];

                    Console.WriteLine("[{0} - {1}) : {2}", st, en, cnt);
                }
            }

            Console.WriteLine("");

            foreach (var item in regions)
            {
                var st = walls[item.MinGroup];
                var en = walls[item.MaxGroup];
                var cnt = Enumerable.Range(item.MinGroup, item.MaxGroup - item.MinGroup).Sum(ii => hist[ii]);

                Console.WriteLine("from {0} to == {1}: {2}", st, en, cnt);
            }
        }


        static void TestInterpolate()
        {
            var n = 100;
            var ts = new double[2 * n];
            var ys = new double[2 * n];

            var eps = 1e-4;

            for (var i = 0; i < n; i++)
            {
                ts[2 * i] = i ;
                ts[2 * i + 1] = i + eps;

                ys[2 * i] = 0;
                ys[2 * i + 1] = 1;
            }

            var d = 10;

            var t2s = new double[n];

            for (var i = 0; i < n; i++)
            {
                t2s[i] = i / 10.0;
            }

            var merged = new double[3 * n];

            int N;
            CrossCorrelate.Merge(ts, t2s, ts.Length, t2s.Length, merged, out N);

            var y2s = new double[merged.Length];

            CrossCorrelate.Interpolate(ts, ys, ts.Length, merged, y2s, N);

        }


        static void testCrossColl()
        {
            var fs = 10;//sampling
            var f = 1;//frequency
            var n = 100;

            var cnt = n * (long)fs;

            var ys = new double[n+100];
            var xs = new double[n+100];

            var dt = 1.0 / fs;

            for (var i = 0; i < n; i++)
            {
                var t = i * dt;

                var x = Math.Sin(2 * Math.PI * t);

                ys[i] = x;
                xs[i] = t;
            }

            var cc = new CrossCorrelate();

            var tau = 0*0.3 * dt;

            var xs2 = (double[])xs.Clone();
            
            for (var i = 0; i < n; i++)
                xs2[i] += tau;


            cc.xs1 = xs;
            cc.ys1 = ys;
            cc.xs2 = xs2;
            cc.ys2 = ys;
            cc.n1 = cc.n2 = n;

            cc.CrossCollerate();
        }

        static void testFreq2()
        {
            //https://stackoverflow.com/a/59267175

            double fs = 10;//sampling
            double f = 1.856;//frequency
            var n = 100000;

            var ys = new double[n];
            var xs = new double[n];

            var dt = 1.0 / fs;

            var omega = 2 * Math.PI * f;

            for (var i = 0; i < n; i++)
            {
                var t = i * dt;

                var x = Math.Sin(omega * t);

                ys[i] = x;
                xs[i] = t;
            }

            double F,phi;

            var dtr = new CorrelationBasedFrequencyDetector();
            dtr.MaxCrosses = 10;

            var t2 = dtr.TryGetFrequency(ys, fs, out F,out phi);

            var ratio = f / ( F);

            var round = Math.Round(ratio);

            var diff = Math.Abs(round - ratio);

            

        }

        static void t2()
        {
            var ifs = new RpiPicoDaqInterface();

            ifs.TargetRepository = UiState.Instance.CurrentRepo;

            ifs.StartSync();

        }

        static void test()
        {
            var l = 100;
            var t1 = new FixedLengthList<int>(l);
            var ts = ArrayPool.Int32(l);

            for (int i = 0; i < 10.5*l; i++)
            {
                t1.Add(i);
            }

            t1.CopyTo(ts);
        }



        static void testFreq()
        {
            var l = 200_000;

            var a1 = new double[l];
            var a2 = new double[l];

            var fs = 20;

            var f = 1;

            var pi = Math.PI;

            var dt = 1.0;

            for (var i = 0; i < l; i++)
            {
                var x = i * dt;

                var y = Math.Sin(2 * pi * f * x);// + Math.Sin(b * x);

                a1[i] = x;
                a2[i] = y;
            }

            double t=0, phi;
            //var t2 = new FftFrequencyDetector().TryGetFrequency(a2, 1, out t, out phi);

            var tt = 1 / (t);

            var lcm = 2 * Math.PI;

            var diff = Math.Abs(tt - lcm);

            if (diff > 1e-3)
                throw new Exception();
        }

        //https://helloacm.com/the-one-line-c-linq-implementation-of-linespace-algorithm/
        private static IEnumerable<double> LineSpace(double start, double end, long partitions)
        {
            for (int i = 0; i < partitions; i++)
            {
                var x = start + (end - start) / partitions * i;
                yield return x;
            }

            yield return end;
        }

        public static int findLCM(int a, int b) //method for finding LCM with parameters a and b
        {
            int num1, num2;                         //taking input from user by using num1 and num2 variables
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (int i = 1; i <= num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num2;
        }


    }
}
