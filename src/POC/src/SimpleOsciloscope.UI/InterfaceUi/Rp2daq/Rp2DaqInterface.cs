using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows.Media.TextFormatting;
using System.Threading;
using System.IO;
using System.Windows.Markup;
using System.Linq;
using System.CodeDom;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using FftSharp;
using System.Windows.Markup.Localizer;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using System.Configuration;
using System.Net.NetworkInformation;
using SimpleOsciloscope.UI.InterfaceUi;
using SimpleOsciloscope.UI.InterfaceUi.Rp2daq;

namespace SimpleOsciloscope.UI.HardwareInterface
{
    /// <summary>
    /// config in UI level (no firmware) for channel
    /// </summary>
    [Obsolete("do not use a global channel info, todo detach and delete")]
    public class AdcChannelInfo
    {
        public readonly int Id = -1;// 1 or 2 or 3 or 4 etc
        public readonly int Pin10x = -1;//gpio# for 10x button
        public readonly int PinAcDc = -1;//ac coupling cap button
        public readonly int PinAdc = -1;//gpio# for adc

        public readonly Rp2DaqInterface.Rp2040AdcChannels RpChannel;


        //public double NormalPullupResistor = double.MaxValue;
        //public double NormalPulldownResistor = double.MaxValue;

        //public double _10xPullupResistor = double.MaxValue;
        //public double _10xPulldownResistor = double.MaxValue;

        public readonly double NormalAlpha = double.NaN;
        public readonly double NormalBeta = double.NaN;

        public readonly double _10xAlpha = double.NaN;
        public readonly double _10xBeta = double.NaN;

        public string Title { get; set; }
        public string Description{ get; set; }


        public bool AcDcMode { get; set; }// ac/dc button is pressed
        public bool _10xMode { get; set; }// 10x button is active

        public AdcChannelInfo(int id, int pin10x, int pinAcDc, double normalAlpha, double normalBeta, double _10xAlpha, double _10xBeta, Rp2DaqInterface.Rp2040AdcChannels rpChannel)
        {
            Id = id;
            Pin10x = pin10x;
            PinAcDc = pinAcDc;
            //PinAdc = pinAdc;
            NormalAlpha = normalAlpha;
            NormalBeta = normalBeta;
            this._10xAlpha = _10xAlpha;
            this._10xBeta = _10xBeta;
            RpChannel = rpChannel;
        }
    }


    public class Rp2DaqInterface: IDaqInterface
    {
        //public static AdcChannelInfo[] Channels;//= InitChannels();

        //static readonly int[] AdcPins = new int[] { 28, 27, 29 };//adc pin for each channel
        //static readonly int[] SwPins = new int[] { 19, 21, 22 };//switch pin for each channel
        //static readonly int[] AcDcPins = new int[] { 20, -1, -1 };//switch pin for each channel

       
        [Flags]
        public enum Rp2040AdcChannels
        {
            //channel_mask : Masks 0x01, 0x02, 0x04 are GPIO26, 27, 28; mask 0x08 internal reference, 0x10 temperature sensor
            None = 0,
            Gpio26 = 1,
            Gpio27 = 2,
            Gpio28 = 4,
            InternalReference = 8,
            InternalTempratureSensor = 16,
        }

        public static int GetChannelMask(params Rp2040AdcChannels[] gpios)
        {
            if (gpios.Contains(Rp2040AdcChannels.None))
                throw new Exception();

            var buf = gpios.Sum(i => (int)i);

            return buf;
        }

        private AdcChannelInfo Channel;
        public Rp2daqCalibrationData CalibrationData;
        public Rp2daqUserSettings UserSettings;


        private bool ch1X10 = false;
        private bool ch1XAcDc = false;
        private bool ch2X10 = false;
        private bool ch3X10 = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1">voltage</param>
        /// <param name="w1">adc value</param>
        /// <param name="v2"></param>
        /// <param name="w2"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        public static void GetCalibrationParameters(double v1,double w1,double v2,double w2,out double alpha,out double beta)
        {
            // y = y0 +  m * (x - x0)
            // m = (y1 - y0) / (x1 - x0)
            // y = a * x + b
            //a = m
            //b = y0 - m * x0

            var y0 = v1;
            var y1 = v2;
            var x0 = w1;
            var x1 = w2;

            var m = (y1 - y0) / (x1 - x0);
            var b = y0 - m * x0;

            alpha = m;
            beta = b;

            var d0 = m * x0 + b - y0;
        }
        /*
        static readonly float vcc_adc = 3.3f;
        /*
        static readonly byte Ch1_10x_pin = 19;//10x button
        static readonly byte Ch1_acdc_pin = 19;//ac coupling cap
        static readonly byte Ch1_adc_pin = 28;//gpio# for adc pin

        static readonly double Ch1_exp_pullup_val = 10e3;//channel1 expand domain resistor (see schematics)
        static readonly double Ch1_exp_pulldown_val = 10e3;//channel1 expand domain resistor (see schematics)
        static readonly double Ch1_adc_pulldown_val = 2e6;//channel1 builtin rp2040 pulldown resistor
        static readonly double Ch1_10xexp_pullup_val = 10e3;//channel1 expand domain resistor (see schematics)
        static readonly double Ch1_10xexp_pulldown_val = 10e3;//channel1 expand domain resistor (see schematics)

        */
        static readonly byte Sop = 0x03;//start of package for rp2daq

        SerialPort Port;

        public bool IsConnected = false;


        public Rp2DaqInterface(BaseDeviceUserSettingsData setts, BaseDeviceCalibrationData calib)
        {
            //AdcResolutionBits = adcResolutionBits;
            //AdcSampleRate = adcSampleRate;
            //PortName = portName;

            this.UserSettings = setts as Rp2daqUserSettings;
            this.CalibrationData = calib as Rp2daqCalibrationData;
        }

        public Rp2DaqInterface(string portName, long adcSampleRate)
        {
            //AdcResolutionBits = adcResolutionBits;
            AdcSampleRate = adcSampleRate;
            PortName = portName;
        }


        
        

        

        public double AdcMaxVoltage { get { return 3.3; } }
        public int AdcResolutionBits
        {
            get { return resolutionBits; }
            set
            {
                resolutionBits = value;
            }
        }
        private int resolutionBits = 12;


        public long AdcSampleRate { get; set; }


        

        //public int SampleRate ;
        public string PortName;

        public long TotalReads;

        //public byte BitWidth = 12;


        public static ushort blockSize = 1_000;
        public static ushort blocksToSend = 10;
        public static bool infiniteBlocks = true;
        //public int ChannelMask = 4;


        public bool Stopped = false;

        public DataRepository TargetRepository { get; set; }


        //private Queue<byte[]> Readed = new Queue<byte[]>();//those are filled with data
        //private Queue<byte[]> Emptied = new Queue<byte[]>();//those that content are used and ready to be reused
        //private object RLock = new object();//for Readed
        //private object ELock = new object();//for ELock


        public void DisConnect(bool log = false)
        {
            if (Port != null)
                Port.Close();

            this.IsConnected = false;
        }

        public void Connect(bool log = false)
        {
            var sport = new SnifferSerial(UserSettings.ComPortName, 268435456);

            sport.LogToConsole = log;

            {//https://stackoverflow.com/a/73668856
                sport.Handshake = Handshake.None;
                sport.DtrEnable = true;
                sport.RtsEnable = true;
                sport.StopBits = StopBits.One;
                sport.DataBits = 8;
                sport.Parity = Parity.None;
                sport.ReadBufferSize = 1024 * 1000;//1MB
            }

            sport.Open();

            Port = sport;
            this.IsConnected = true;
        }

        public void StopAdc()
        {

            if (Port == null)
                return;
            var port = Port;
            Stopped = true;

            var dt = new byte[] { Sop,12, 1 };//12: ADC_Stop id, 1:https://github.com/FilipDominec/rp2daq/blob/09a00b00b7f1d8f63583e23a4ced26a01f095c3d/include/adc_builtin.c#L100

            port.Write(dt);
            port.BaseStream.Flush();

            do
            {
                var ret = port.ReadAvailable();
                Thread.Sleep(100);

            } while (port.BytesToRead != 0);//read existing data
        }

        public string GetDeviceIdentifier()
        {
            var dt = new byte[] { 1, 0 };

            var sport = Port;
            sport.Write(dt);

            Thread.Sleep(200);

            var l = 34;

            var t1 = sport.BytesToRead;

            if (t1 != l)
                throw new Exception("Unexpected resonse length, try unplug and replug the PICO");

            var buf = sport.ReadExplicitLength(l);
            var pass = 4;

            var ver = Encoding.ASCII.GetString(buf, pass, buf.Length - pass);
            return ver;
        }

        /*
        public void SetHighZ()
        {
            var adcPins = new byte[] { 26, 27, 28 };
            //adcPins = new byte[] { 1, 2, 3 };

            foreach (var pin in adcPins)
            {

                var dt = new byte[] { Sop,2, pin };//

                Port.Write(dt);

                Port.BaseStream.FlushAsync();

                //Thread.Sleep(100);

                var tt = Port.ReadExplicitLength(3); //??
            }
        }
        */


        public void SetupAdc()
        {
            //var blockSize = blockSize;//samples per block
            var blockCount = 0;
            var bitwidth = this.UserSettings.BitWidth;
            var sampleRate = (int)this.UserSettings.SampleRate;
            var channel = this.UserSettings.ChannelId;

            Stopped = false;

            var cmd = AdcConfig.Default();

            {
                //https://github.com/FilipDominec/rp2daq/blob/main/docs/PYTHON_REFERENCE.md#adc
                cmd.channel_mask = (byte)GetChannelMask((Rp2040AdcChannels)channel);
                cmd.blocksize = (ushort)blockSize;
                cmd.blocks_to_send = (ushort)blockCount;
                cmd.infinite = infiniteBlocks ? (byte)1 : (byte)0;
                cmd.clkdiv = (ushort)(48_000_000 / sampleRate); //rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)
            }

            var cmdBin = cmd.ToArray();// StructTools.RawSerialize(cmd);//serialize into 9 byte binary

            var tmp = BitConverter.ToString(cmdBin);

            byte sop = 0x0e;//not sure why sop is 0x0e here!

            Port.Write(new byte[] { sop, 4 }, cmdBin);
            Port.BaseStream.Flush();

            Thread.Sleep(10);
        }

        /*
        public void ReadAdcDataFake()
        {
            var dt = new List<byte>();

            while(true)
            {
                dt.AddRange(Port.ReadAvailable());

                Thread.Sleep(10);
            }

        }
        */

        public Dictionary<byte, bool> LastGpioValues = new Dictionary<uint8_t, bool>();

        public event EventHandler<EventArgs> OnGpioChange;

        public void HandleGpioChange(byte pin,bool newValue)
        {
            LastGpioValues[pin] = newValue;
            /*
            foreach (var ch in UiState.Instance.Channels)
            {
                if (pin == ch.Pin10x)
                {
                    ch._10xMode = newValue;
                    return;
                }

                if (pin == ch.PinAcDc)
                {
                    ch.AcDcMode = newValue;
                    return;
                }
            }
            */

            if (OnGpioChange != null)
                OnGpioChange(this, EventArgs.Empty);
        }

        public void ReadGpioChangeData(byte[] data)
        {
            var pin = data[0];
            var newVal = data[4];

            if (newVal != 4 && newVal != 8)
                throw new Exception("gpio report parse failure");

            var isPressed = newVal == 8;

            HandleGpioChange(pin, isPressed);
        }

        public void ReadAdcValues(byte[] buff)
        {
            var arrLength = buff.Length;

            byte a, b, c;
            int v1, v2, newVal;
            float volt1, volt2;

            var arr = TargetRepository.Samples;
            var arrF = TargetRepository.SamplesF;
            //this is only single channel


            var ch = this.Channel;// UiState.Instance.Channels[0];

            double alpha, beta;


            alpha = 3.3 / 4096;
            beta = 0;

            /*
            if (ch._10xMode)
            {
                alpha = ch._10xAlpha;
                beta = ch._10xBeta;
            }
            else
            {
                alpha = ch.NormalAlpha;
                beta = ch.NormalBeta;
            }
            */

            UiState.Instance.CurrentRepo.LastAlpha = alpha;
            UiState.Instance.CurrentRepo.LastBeta = beta;


            for (var j = 0; j < arrLength; j += 3)
            {
                a = buff[j + 0];
                b = buff[j + 1];
                c = buff[j + 2];

                v1 = a + ((b & 0xF0) << 4);
                //v2 = ((c & 0xF0) / 16) + (b & 0x0F) * 16 + (c & 0x0F) * 256;//ImproveMe: replace / 16 and * 16 and * 256 etc with bitwise operators
                v2 = ((c & 0xF0) >> 4) + ((b & 0x0F) << 4) + ((c & 0x0F) << 8);

                arr.Add((short)v1);
                arr.Add((short)v2);

                volt1 = (float)(v1 * alpha + beta);
                volt2 = (float)(v2 * alpha + beta);

                arrF.Add(volt1);
                arrF.Add(volt2);


                TotalReads += 2;
            }
        }


        public void SetRefPwm()
        {
            //todo: fixme
            {//pwm config pair

                //RP2040 PWM Frequency and Duty cycle set algorithm.
                //https://medium.com/@pranjalchanda08/rp2040-pwm-frequency-and-duty-cycle-set-algorithm-2eb953b83dd4


                byte gpioPin = 0;
                uint8_t gpio = gpioPin;         // default=0		min=0		max=25
                uint16_t wrap_value = (uint16_t)125_00;        // default=999		min=1		max=65535
                uint16_t clkdiv = 0;            // default=1		min=1		max=255
                uint8_t clkdiv_int_frac = 0;    // default=0		min=0		max=15

                byte[] arr;

                using (var str = new MemoryStream())
                {
                    var rwtr = new BinaryWriter(str);

                    rwtr.Write(Sop);
                    rwtr.Write(5);//pwm config pair
                    rwtr.Write(gpio);
                    rwtr.Write(wrap_value);
                    rwtr.Write(clkdiv);
                    rwtr.Write(clkdiv_int_frac);

                    arr = str.ToArray();
                }

                Port.Write(arr, 0, arr.Length);

                var reportCode = Port.ReadExplicitLength(1)[0];

                if (reportCode != 5)
                    throw new Exception();

            }
        }


        public bool[] GetGpioValues(params byte[] pins)
        {
            var buf = new bool[pins.Length];

            for (int i = 0; i < pins.Length; i++)
            {
                byte pin = pins[i];
                var dt = new byte[] { Sop, 2, pin };

                Port.Write(dt);

                Port.BaseStream.FlushAsync();

                var tt = Port.ReadExplicitLength(3);

                var code = tt[0];

                if (code != 2)
                    throw new Exception();

                var gpio = tt[1];
                var val = tt[2];

                buf[i] = val == 1;
            }

            return buf;
        }

        private double Alpha, Beta;


        public void SetLocalCalibParams()
        {
            var g26_10x = 19;
            var g27_10x = 20;
            var g28_10x = 21;

            /*
            if (UserSettings.ChannelId == Rp2040AdcChannels.Gpio26)
            {
                if (LastGpioValues[g26_10x])//channel1, 10x pressed
                {
                    Alpha = CalibrationData.AlphaA1;
                    Beta = CalibrationData.BetaA1;
                }
                else
                {
                    Alpha = CalibrationData.AlphaB1;
                    Beta = CalibrationData.BetaB1;
                }
            }*/


            if (UserSettings.ChannelId == Rp2040AdcChannels.Gpio27)
            {
                if (LastGpioValues[20])//channel1, 10x pressed
                {
                    Alpha = CalibrationData.AlphaA2;
                    Beta = CalibrationData.BetaA2;
                }
                else
                {
                    Alpha = CalibrationData.AlphaB2;
                    Beta = CalibrationData.BetaB2;
                }
            }


            if (UserSettings.ChannelId == Rp2040AdcChannels.Gpio28)
            {
                if (LastGpioValues[19])//channel1, 10x pressed
                {
                    Alpha = CalibrationData.AlphaA2;
                    Beta = CalibrationData.BetaA2;
                }
                else
                {
                    Alpha = CalibrationData.AlphaB2;
                    Beta = CalibrationData.BetaB2;
                }
            }
        }


        public void ReadGpioInitialValues()
        {
            var pinsList = new List<byte>();

            foreach (var item in UiState.Instance.Channels)
            {
                var x10 = item.Pin10x;
                var acdc = item.PinAcDc;

                pinsList.Add((byte)x10);

                if (acdc != null)
                    pinsList.Add((byte)acdc);
            }

            var vals = GetGpioValues(pinsList.ToArray());

            for (var i = 0; i < pinsList.Count; i++)
            {
                byte pin = pinsList[i];
                var val = vals[i];

                HandleGpioChange(pin, val);
            }
                //HandleGpioChange(gpio, val == 1); not right
                
        }

        

        public void ReadAdcData()
        {
            int bitwidthCurr;
            var sport = Port;

            var bw = this.UserSettings.BitWidth;//faster access

            var arrLength = (blockSize * bw) / 8;

            var arr = TargetRepository.Samples;

            var arrF = TargetRepository.SamplesF;

            var _4Count = 0;

            {//reading data

                byte[] buf = new byte[arrLength];

                var cnt = 0;

                var adcHeaderLength = 25;
                var gpioHeaderLength = 16;

                var adcReportHeader = new byte[adcHeaderLength];

                var gpioReportHeader = new byte[gpioHeaderLength];

                //double adc1_alpha = 1.0f;
                //double adc1_beta = 1.0f;   //volt = adc1*adc1_alpha + adc1_beta

                //var tmp = Emptied.Dequeue();

                byte[] buff;

                //byte a, b, c;
                //int v1, v2;

                //Console.WriteLine("Starting ADC read");

                var tmp = new byte[1];

                //float volt1, volt2;

                while (!Stopped)//read while true
                {
                    var tmpii = TotalReads;

                    //read next block
                    if (sport.BytesToRead == 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    byte packageCode;

                    {
                        sport.BaseStream.ReadArray(tmp);
                        packageCode = tmp[0];
                    }

                    //Debug.WriteLine("Package :" + packageCode);

                    //packageCode = 4;

                    if (packageCode == 0x03)//gpio change
                    {
                        sport.BaseStream.ReadArray(gpioReportHeader);

                        ReadGpioChangeData(gpioReportHeader);
                        /*
                        if (pin == Ch1_10x_pin)
                        {
                            if (!isPressed)
                            {
                                GetCalibrationParameters(3.3, 2626, 0, 1345, out adc1_alpha, out adc1_beta);
                            }
                            else
                            {
                                GetCalibrationParameters(3.3, 3853, 0, 78.9, out adc1_alpha, out adc1_beta);
                            }

                            Debug.WriteLine($"{adc1_alpha},{adc1_beta}");
                        }

                        if (pin == Ch1_acdc_pin)
                        {

                        }
                        */
                    }


                    else if (packageCode == 0x04)//adc report
                    {
                        //_4Count++;
                        sport.BaseStream.ReadArray(adcReportHeader);//adc_report binary

                        bitwidthCurr = adcReportHeader[2];// report._data_bitwidth;

                        //var counter = BitConverter.ToUInt32(adcReportHeader, 20);
                        //var c2 = BitConverter.ToUInt16(adcReportHeader, 1);

                        //Console.WriteLine("Counter " + counter);

                        if (bitwidthCurr != bw)
                            throw new Exception("Packet Loss! Try Reconnect...");//kind of checking!

                        buff = buf;

                        sport.BaseStream.ReadArray(buf);

                        {
                            //var tm = new byte[2];
                            //sport.BaseStream.ReadArray(tm);

                            //if (tm[0] != 4) throw new Exception();
                        }

                        ReadAdcValues(buff);
                        /*
                        for (var j = 0; j < arrLength; j += 3)
                        {
                            a = buff[j + 0];
                            b = buff[j + 1];
                            c = buff[j + 2];

                            v1 = a + ((b & 0xF0) << 4);
                            v2 = (c & 0xF0) / 16 + (b & 0x0F) * 16 + (c & 0x0F) * 256;//ImproveMe: replace / 16 and * 16 and * 256 etc with bitwise operators

                            arr.Add((short)v1);
                            arr.Add((short)v2);

                            volt1 = (float) (v1  * adc1_alpha + adc1_beta);
                            volt2 = (float)(v2  * adc1_alpha + adc1_beta);

                            arrF.Add(volt1);
                            arrF.Add(volt2);


                            TotalReads += 2;
                        }
                        */
                    }
                    else
                    {
                        /*
                        if (OnPacketLoss != null)
                        {
                            OnPacketLoss.Invoke(this, new EventArgs());
                        }
                        return;
                        */
                        throw new Exception("packet loss");
                    }

                    cnt++;
                }


            }
        }


        //public event EventHandler<EventArgs> OnPacketLoss;

        public void StartSync()
        {

            Connect(false);

            var sport = Port;


            int bitwidthCurr;
            var blockSize = 100;//samples per block
            var blockCount = 1;
            var bitwidth = 12;
            var sampleRate = (int)this.AdcSampleRate;
            var infinite = true;

            {//send command for ADC stop
                StopAdc();
            }

            string ver = GetDeviceIdentifier();

            if (!ver.StartsWith("rp2daq_240715"))
                throw new Exception("Invalid firmware version");


            {//send command for set ADC pins to high impedance (remove pull up/down resistors), no need anymore, fixed on firmware
                //SetHighZ();
            }
           
            {//set gpio push button callback
                var pins = new byte[] { 19, 20 };

                foreach (var pin in pins)
                {
                    var cmd = new byte[] { 0x05, 0x03, pin, 0x01, 0x01 };
                    sport.Write(cmd);
                }
            }

            {//read gpio initial values
                ReadGpioInitialValues();
            }

            {//set calibration parameters
                SetLocalCalibParams();
            }

            {//send command for ADC
                SetupAdc();
            }

            
            ReadAdcData();

            //Enumerable.Repeat(1, 100).Select(i => new byte[arrLength]).ToList().ForEach(i => Emptied.Enqueue(i));

            

            

            //throw new NotImplementedException();
        }

        public void DisConnect()
        {
            StopAdc();

            if (Port != null)
                Port.Close();

            Port = null;

            IsConnected = false;
        }
    }
}
