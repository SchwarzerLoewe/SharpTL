// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLStreamerFacts.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using BigMath;
using FluentAssertions;
using NUnit.Framework;

namespace SharpTL.Tests
{
    [TestFixture]
    public class TLStreamerFacts
    {
        private static readonly byte[] TestIntBytesInBigEndian = BytesRange(1, 4);
        private static readonly byte[] TestLongBytesInBigEndian = BytesRange(1, 8);
        private static readonly byte[] TestDoubleBytesInBigEndian = TestLongBytesInBigEndian;
        private static readonly byte[] TestInt128BytesInBigEndian = BytesRange(1, 16);
        private static readonly byte[] TestInt256BytesInBigEndian = BytesRange(1, 32);

        private static readonly byte[] TestIntBytesInLittleEndian = TestIntBytesInBigEndian.Reverse().ToArray();
        private static readonly byte[] TestLongBytesInLittleEndian = TestLongBytesInBigEndian.Reverse().ToArray();
        private static readonly byte[] TestDoubleBytesInLittleEndian = TestLongBytesInLittleEndian;
        private static readonly byte[] TestInt128BytesInLittleEndian = TestInt128BytesInBigEndian.Reverse().ToArray();
        private static readonly byte[] TestInt256BytesInLittleEndian = TestInt256BytesInBigEndian.Reverse().ToArray();

        private const int TestInt = 0x01020304;
        private const long TestLong = 0x0102030405060708;
        private static readonly double TestDouble = BitConverter.Int64BitsToDouble(TestLong);
        private static readonly Int128 TestInt128 = Int128.Parse("0x0102030405060708090A0B0C0D0E0F10");
        private static readonly Int256 TestInt256 = Int256.Parse("0x0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20");

        [TestCase(true)]
        [TestCase(false)]
        public void Should_write_int(bool streamAsLittleEndian)
        {
            CheckWriteToStream(stream => stream.WriteInt32(TestInt), TestIntBytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_write_long(bool streamAsLittleEndian)
        {
            CheckWriteToStream(stream => stream.WriteInt64(TestLong), TestLongBytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_write_double(bool streamAsLittleEndian)
        {
            CheckWriteToStream(stream => stream.WriteDouble(TestDouble), TestLongBytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_write_int128(bool streamAsLittleEndian)
        {
            CheckWriteToStream(stream => stream.WriteInt128(TestInt128), TestInt128BytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_read_int(bool streamAsLittleEndian)
        {
            CheckReadFromStream(stream => stream.ReadInt32(), TestInt, streamAsLittleEndian ? TestIntBytesInLittleEndian : TestIntBytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_read_long(bool streamAsLittleEndian)
        {
            CheckReadFromStream(stream => stream.ReadInt64(), TestLong, streamAsLittleEndian ? TestLongBytesInLittleEndian : TestLongBytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_read_double(bool streamAsLittleEndian)
        {
            CheckReadFromStream(stream => stream.ReadDouble(), TestDouble, streamAsLittleEndian ? TestDoubleBytesInLittleEndian : TestDoubleBytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_read_int128(bool streamAsLittleEndian)
        {
            CheckReadFromStream(stream => stream.ReadInt128(), TestInt128, streamAsLittleEndian ? TestInt128BytesInLittleEndian : TestInt128BytesInBigEndian, streamAsLittleEndian);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_read_int256(bool streamAsLittleEndian)
        {
            CheckReadFromStream(stream => stream.ReadInt256(), TestInt256, streamAsLittleEndian ? TestInt256BytesInLittleEndian : TestInt256BytesInBigEndian, streamAsLittleEndian);
        }

        private static void CheckWriteToStream(Action<TLStreamer> write, byte[] testBytesInBigEndian, bool streamAsLittleEndian)
        {
            int bytesCount = testBytesInBigEndian.Length;

            using (var streamer = new TLStreamer {StreamAsLittleEndian = streamAsLittleEndian})
            {
                write(streamer);
                streamer.Length.Should().Be(bytesCount);

                streamer.Position = 0;

                var bytes = new byte[bytesCount];
                streamer.Read(bytes, 0, bytesCount);

                if (streamAsLittleEndian)
                {
                    Array.Reverse(bytes);
                }

                for (byte i = 0; i < bytesCount; i++)
                {
                    bytes[i].Should().Be(testBytesInBigEndian[i]);
                }
            }
        }

        private static void CheckReadFromStream<T>(Func<TLStreamer, T> read, T testValue, byte[] streamBuffer, bool streamAsLittleEndian) where T : struct
        {
            using (var stream = new TLStreamer(streamBuffer) {StreamAsLittleEndian = streamAsLittleEndian})
            {
                T value = read(stream);
                value.Should().Be(testValue);
            }
        }

        private static byte[] BytesRange(int start, int count)
        {
            return Enumerable.Range(start, count).Select(i => (byte) i).ToArray();
        }
    }
}
