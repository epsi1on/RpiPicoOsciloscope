using SimpleOsciloscope.UI;
using SimpleOsciloscope.UI.FrequencyDetection;
using SimpleOsciloscope.UI.HardwareInterface;
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
using NAudio.Wave;
using System.Runtime.Remoting.Channels;
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;

namespace SimpleOsciloscope.TestConsole
{
    [StructLayout(LayoutKind.Explicit, Size = 13, Pack = 1)]
    public struct StmAdcConfig
    {
        public static StmAdcConfig Default()
        {
            var buf = new StmAdcConfig();
            buf.resolution = AdcResolution.ADC_RESOLUTION_12B;
            buf.chnCount = 1;
            buf.clockPrescaler = AdcClockPrescaler.ADC_CLOCK_SYNC_PCLK_DIV2;

            return buf;
        }

        [FieldOffset(0)]
        AdcResolution resolution;
        [FieldOffset(4)]
        byte chnCount;
        [FieldOffset(5)]
        AdcClockPrescaler clockPrescaler;
        [FieldOffset(9)]
        uint numberOfConvertion;
    }

    public enum AdcResolution:uint
    {
        ADC_RESOLUTION_12B = 0x00000000U,
        ADC_RESOLUTION_10B = 0x01000000,
        ADC_RESOLUTION_8B = 0x02000000,
        ADC_RESOLUTION_6B = 0x03000000
    }

    public enum AdcClockPrescaler : uint
    {
        ADC_CLOCK_SYNC_PCLK_DIV2 = 0x00000000U,
        ADC_CLOCK_SYNC_PCLK_DIV4 = 0x00010000,
        ADC_CLOCK_SYNC_PCLK_DIV6 = 0x00020000,
        ADC_CLOCK_SYNC_PCLK_DIV8 = 0x00030000
    }


    internal class Program
    {
        [STAThread]
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
            //TestBitmap();
            //TestAudio();
            //TestBlackpillUsb();
            //TestBlackpillUsbFft();
            //TestAudio();
            //testFreq4();
            //testFreq3();
            //ReadDirect();
            //TestAdcRead();
            //TestAdcRead2();
            TestAdcSamplerGui();
            Console.ReadKey();


        }


        static void TestAdcSamplerGui()
        {
            new Application();

            //var res = AdcSampler.GetAdcMedian("COM10", 0);
        }

        static void TestBlackpillUsbFft()
        {
            var port = new SnifferSerial("COM3", 1) { LogToConsole = false };

            port.Open();

            var headerLen = 32;

            var bodyLen = 3 * 4 * 7 * 1024;//in byte

            //var buf = new byte[l];

            var sp = Stopwatch.StartNew();


            var cfg = StmAdcConfig.Default();


            var cmdBin = StructTools.RawSerialize(cfg);//serialize into 9 byte binary

            port.Write(cmdBin, 0, cmdBin.Length);

            var hdr = port.ReadExplicitLength(headerLen);

            var dt1 = port.ReadExplicitLength(bodyLen / 2);
            var dt2 = port.ReadExplicitLength(bodyLen / 2);

            var t = dt1.Concat(dt2).Select(i => (double)i).ToArray();

            var dtr = new FftFrequencyDetector();
            double f, d;

            //dtr.TryGetFrequency(t, 700_000, out f, out d);

        }

        static void TestBlackpillUsb()
        {
            var port = new SnifferSerial("COM3", 1) { LogToConsole = false };

            port.Open();

            var headerLen = 32;

            var bodyLen = 10 * 1024;//in byte

            //var buf = new byte[l];

            var sp = Stopwatch.StartNew();

            var cfg = StmAdcConfig.Default();
            
            var cmdBin = StructTools.RawSerialize(cfg);//serialize into 9 byte binary

            port.Write(cmdBin, 0, cmdBin.Length);

            while (true)
            {
                var rc = 0;


                var hdr = port.ReadExplicitLength(headerLen);

                var dt1 = port.ReadExplicitLength(bodyLen / 2);
                var dt2 = port.ReadExplicitLength(bodyLen / 2);

                var c1 = dt1.Count(i => i == 0);
                var c2 = dt2.Count(i => i == 0);

                var c = BitConverter.ToUInt32(hdr, 0);
                var m1 = BitConverter.ToUInt16(hdr, 4);
                var m2 = BitConverter.ToUInt16(hdr, 8);

                //var c2 = BitConverter.ToUInt16(dt1, 0);

                Console.WriteLine($"{c},{m1},{m2},{c1}/{bodyLen / 2}, {c2}/{bodyLen / 2}, {(100*(c1+c2))/bodyLen}% zeros");

                //Console.WriteLine(m1);
                //Console.WriteLine(m2);

                var byteCount = c * bodyLen ;

                //Console.WriteLine(c);

                if (sp.ElapsedMilliseconds != 0)
                {
                    var spe = (double)byteCount / (double)sp.Elapsed.TotalMilliseconds;

                    Console.WriteLine(spe.ToString("0.0") + " KSps");
                    
                }
            }

        }


        static void TestAudio()
        {
            var sampleRate = 250_000;

            var audio = new SimpleOsciloscope.UI.Audio.NAudioPlayer(sampleRate, 1);
            var daq =
                //new SimpleOsciloscope.UI.HardwareInterface.RpiPicoDaqInterface("COM8", sampleRate);
                //new SimpleOsciloscope.UI.HardwareInterface.FakeDaqInterface() { Frequency = 10000 };
                new SimpleOsciloscope.UI.HardwareInterface.Stm32Interface("COM3", 0);

            var rep = new DataRepository();

            rep.Samples = audio;

            daq.TargetRepository = rep;

            new Thread(daq.StartSync).Start();

            new Thread(new ThreadStart(new Action(() => {
                while (true)
                {
                    Thread.Sleep(1000);
                    //Console.WriteLine(daq.TotalReads);
                }
            
            }))).Start();

            var waveOut = new WaveOut();

            waveOut.Init(audio);
            waveOut.Play();
            //waveOut.OutputWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);

            waveOut.Volume = 1f;
            
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
            var samples = System.IO.File.ReadAllLines("C:\\Users\\epsi1on\\Documents\\temp.csv");

            var arr = new double[samples.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = double.Parse(samples[i]);
            }

            var fs = 850_000;

            double F, phi;

            

            var avg = arr.KahanSum() / arr.Length;

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] -= avg;
            }


            var dtr = new FftFrequencyDetector();
            //dtr.MaxCrosses = 10;

            //var t2 = dtr.TryGetFrequency(arr, fs, out F, out phi);
        }

        static void TestDaqInterface()
        {
            var ifs = new Rp2DaqInterface("COM8",50_000);
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

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        static void ReadDirect()
        {
            var sport = new SerialPort("COM10", 268435456);

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
                var dts = "0e040200200004000000e001ff00";
                
                var dt = StringToByteArray(dts);
                //var dt = new byte[] { 0x0A, 0x04, 0x10, 0xe8, 0x03, 0x01, 0x01, 0x00,96, 0x00 };

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


        static void TestAdcRead2()
        {
            var rate = 100_000;

            var ifs = new Rp2DaqInterface("COM10",rate);

            //RpiPicoDaqInterface.ChannelMask = 4;
            //RpiPicoDaqInterface.blockSize = 100;
            //RpiPicoDaqInterface.blocksToSend = 10;
            //RpiPicoDaqInterface.infiniteBlocks = false;

            var repo = new DataRepository();

            //repo.Samples = new FixedLengthListRepo<short>(rate*5);

            ifs.TargetRepository = repo;

            repo.Init(rate);
            //ifs.PortName = "Com10";
            //ifs.AdcSampleRate = rate;


            //ifs.StartSync();


            new Thread(ifs.StartSync).Start();

            Task.Run(() =>
            {

                var arr = new short[rate];

                while(true)
                {

                    
                    Thread.Sleep(1000);

                    var tmp = repo.Samples as FixedLengthListRepo<short>;

                    if (tmp.TotalWrites < rate)
                        continue;

                    tmp.CopyTo(arr);


                    var sum = 0l;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        sum += arr[i];
                    }

                    var avg = sum / (double)(arr.Length);

                    var volt = avg * 3.3 / 4096;
                    //var lastVal = tmp[(int)tmp.TotalWrites - 1];

                    Console.WriteLine("{0:0.000000} V, (ADC: {1:0.000000}, !ADC: {2:0.000000})", volt, avg, 4096 - avg);
                }
               
            });

            Console.ReadKey();

        }

        static void TestAdcRead()
        {
            var rate = 50_000;

            var ifs = new Rp2DaqInterface("COM10", rate);


            var repo = new DataRepository();

            //repo.Samples = new FixedLengthListRepo<short>(rate*5);

            ifs.TargetRepository = repo;

            repo.Init(rate);
            //ifs.PortName = "Com10";
            //ifs.AdcSampleRate = rate;

            new Thread(ifs.StartSync).Start();

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    Console.WriteLine(repo.Samples.TotalWrites);
                }

            });

            Console.ReadKey();

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

            double F, phi;

            var dtr = new CorrelationBasedFrequencyDetector();
            dtr.MaxCrosses = 10;

           // var t2 = dtr.TryGetFrequency(ys, fs, out F, out phi);

           // var ratio = f / (F);

           // var round = Math.Round(ratio);

            //var diff = Math.Abs(round - ratio);



        }


        static void testFreq4()
        {
            //https://stackoverflow.com/a/59267175

            double fs = 1000;//sampling
            double f = 21.234567890;//frequency
            var n = 10000;

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

            double F, phi;

            var dtr = new FftFrequencyDetector();

            F = 1.0;
            var t2 = 1.0;// dtr.TryGetFrequency(ys, fs, out F, out phi);

            var ratio = f / (F);

            var round = Math.Round(ratio);

            var diff = Math.Abs(round - ratio);



        }

        static void t2()
        {
            var ifs = new Rp2DaqInterface("COM8",50_000);

            ifs.TargetRepository = UiState.Instance.CurrentRepo;

            ifs.StartSync();

        }

        static void test()
        {
            var l = 100;
            var t1 = new FixedLengthListRepo<int>(l);
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
