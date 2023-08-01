using System;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint64_t = System.UInt64;
using System.Runtime.InteropServices;

namespace SimpleOsciloscope.UI
{
    [StructLayout(LayoutKind.Explicit, Size = 24, Pack = 1)]
    public struct ADC_Report
    {
        [FieldOffset(0)]
        public uint8_t report_code;  // again this is 0x04
        [FieldOffset(1)]
        public uint16_t _data_count;
        [FieldOffset(3)]
        public uint8_t _data_bitwidth;
        [FieldOffset(4)]
        public uint64_t start_time_us;
        [FieldOffset(12)]
        public uint64_t end_time_us;
        [FieldOffset(20)]
        public uint8_t channel_mask;
        [FieldOffset(21)]
        public uint16_t blocks_to_send;
        [FieldOffset(23)]
        public uint8_t block_delayed_by_usb;
    }

    [StructLayout(LayoutKind.Explicit, Size = 9, Pack = 1)]
    public struct Command
    {
        public static Command Default()
        {
            var buf = new Command();
            buf.message_type = 0x04;
            buf.channel_mask = 1;
            buf.blocksize = 1000;
            buf.infinite = 0;
            buf.blocks_to_send = 1;
            buf.clkdiv = 96;

            return buf;
        }


        [FieldOffset(0)]
        public uint8_t message_type ; // FOR CURRENT FIRMWARE THIS IS 0x04
        [FieldOffset(1)]
        public uint8_t channel_mask;       // default=1		min=0		max=31 Masks 0x01, 0x02, 0x04 are GPIO26, 27, 28; mask 0x08 internal reference, 0x10 temperature sensor
        [FieldOffset(2)]
        public uint16_t blocksize;         // default=1000		min=1		max=8192 Number of sample points until a report is sent
        [FieldOffset(4)]
        public uint8_t infinite;           // default=0		min=0		max=1  Disables blocks_to_send countdown (reports keep coming until explicitly stopped)
        [FieldOffset(5)]
        public uint16_t blocks_to_send;    // default=1		min=0		         Number of reports to be sent (if not infinite)
        [FieldOffset(7)]
        public uint16_t clkdiv;            // default=96		min=96		max=65535 Sampling rate is 48MHz/clkdiv (e.g. 96 gives 500 ksps; 48000 gives 1000 sps etc.)

    }



    public static class SerializationHelper
    {
        public static byte[] Serialize(Command obj)
        {
            int size = 21;// Marshal.SizeOf(obj);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);

            var buf = new byte[size];

            Marshal.Copy(ptr, buf, 0, size);
            Marshal.FreeHGlobal(ptr);

            return buf;
        }
    }

    public static class StructTools
    {
        /// <summary>
        /// converts byte[] to struct
        /// </summary>
        public static T RawDeserialize<T>(byte[] rawData, int position)
        {
            int rawsize = Marshal.SizeOf(typeof(T));
            if (rawsize > rawData.Length - position)
                throw new ArgumentException("Not enough data to fill struct. Array length from position: " + (rawData.Length - position) + ", Struct length: " + rawsize);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            T retobj = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        /// <summary>
        /// converts a struct to byte[]
        /// </summary>
        public static byte[] RawSerialize(object anything)
        {
            int rawSize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawDatas = new byte[rawSize];
            Marshal.Copy(buffer, rawDatas, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawDatas;
        }
    }
}
