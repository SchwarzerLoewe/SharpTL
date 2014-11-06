// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceTests.cs">
//   Copyright (c) 2013-2014 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using BigMath.Utils;
using NUnit.Framework;
using ProtoBuf;

namespace SharpTL.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        [Explicit]
        [Category("Performance")]
        public void Measure_performance_with_complex_object()
        {
            const int loops = 5000000;
            var sw = new Stopwatch();
            PerormanceTestObject perormanceTestObject = CreateTestObject();

            var b = new byte[0];
            var bytes = new byte[1000];
            using (var memoryStream = new MemoryStream(bytes))
            {
                memoryStream.SetLength(0);

                Console.WriteLine("Test serialization of a complex object with {0} iterations...", loops);
                Console.WriteLine();

                Console.WriteLine("Starting protobuf-net test...");
                double protobufNetTime;
                {
                    sw.Restart();
                    Serializer.PrepareSerializer<PerormanceTestObject>();
                    sw.Stop();
                    Console.WriteLine("Serializer prepared in {0} ms.", sw.ElapsedMilliseconds);
                
                    sw.Restart();

                    for (int i = 0; i < loops; i++)
                    {
                        Serializer.Serialize(memoryStream, perormanceTestObject);
                        b = memoryStream.ToArray();

                        memoryStream.Position = 0;
                        memoryStream.SetLength(0);
                    }
                    sw.Stop();
                    protobufNetTime = sw.ElapsedMilliseconds;
                    Console.WriteLine("Bytes ({0}): {1}.", b.Length, b.ToHexString());
                    Console.WriteLine("Elapsed {0} ms.", protobufNetTime);
                    Console.WriteLine();
                }

                Console.WriteLine("Starting SharpTL test...");
                double sharpTLTime;
                {
                    sw.Restart();
                    TLRig.Default.PrepareSerializer<PerormanceTestObject>();
                    sw.Stop();
                    Console.WriteLine("Serializer prepared in {0} ms.", sw.ElapsedMilliseconds);

                    sw.Restart();

                    for (int i = 0; i < loops; i++)
                    {
                        TLRig.Default.Serialize(perormanceTestObject, memoryStream);
                        b = memoryStream.ToArray();

                        memoryStream.Position = 0;
                        memoryStream.SetLength(0);
                    }
                    sw.Stop();
                    sharpTLTime = sw.ElapsedMilliseconds;
                    Console.WriteLine("Bytes ({0}): {1}.", b.Length, b.ToHexString());
                    Console.WriteLine("Elapsed {0} ms.", sharpTLTime);
                    Console.WriteLine();
                }

                if (sharpTLTime > protobufNetTime)
                {
                    Console.WriteLine("SharpTL is {0:N2}x slower than protobuf-net.", sharpTLTime / protobufNetTime);
                }
                else
                {
                    Console.WriteLine("SharpTL is {0:N2}x faster than protobuf-net.", protobufNetTime / sharpTLTime);
                }
            }
        }

        private static PerormanceTestObject CreateTestObject()
        {
            return new PerormanceTestObject
            {
                TestBoolean = true,
                TestDouble = Double.Epsilon,
                TestInt = Int32.MaxValue,
                TestIntVector = new List<int> {1, 2, 3, 4, 5},
                TestLong = Int64.MaxValue,
                TestString = "PPP",
                TestIntBareVector = new List<int> {9, 99, 999, 9999, 99999, 999999}
            };
        }
    }

    [TLObject(0xA9B9C9D9)]
    [ProtoContract]
    public class PerormanceTestObject : IEquatable<PerormanceTestObject>
    {
        [TLProperty(1)]
        [ProtoMember(1)]
        public bool TestBoolean { get; set; }

        [TLProperty(2)]
        [ProtoMember(2)]
        public double TestDouble { get; set; }

        [TLProperty(3)]
        [ProtoMember(3)]
        public int TestInt { get; set; }

        [TLProperty(4)]
        [ProtoMember(4)]
        public List<int> TestIntVector { get; set; }

        [TLProperty(5)]
        [ProtoMember(5)]
        public long TestLong { get; set; }

        [TLProperty(6)]
        [ProtoMember(6)]
        public string TestString { get; set; }

        [TLProperty(7, TLSerializationMode.Bare)]
        [ProtoMember(7)]
        public List<int> TestIntBareVector { get; set; }

        #region Equality
        public bool Equals(PerormanceTestObject other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return TestBoolean.Equals(other.TestBoolean) && TestDouble.Equals(other.TestDouble) && TestInt == other.TestInt &&
                TestIntVector.SequenceEqual(other.TestIntVector) && TestLong == other.TestLong && string.Equals(TestString, other.TestString) &&
                TestIntBareVector.SequenceEqual(other.TestIntBareVector);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((PerormanceTestObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TestBoolean.GetHashCode();
                hashCode = (hashCode*397) ^ TestDouble.GetHashCode();
                hashCode = (hashCode*397) ^ TestInt;
                hashCode = (hashCode*397) ^ (TestIntVector != null ? TestIntVector.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ TestLong.GetHashCode();
                hashCode = (hashCode*397) ^ (TestString != null ? TestString.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TestIntBareVector != null ? TestIntBareVector.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PerormanceTestObject left, PerormanceTestObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PerormanceTestObject left, PerormanceTestObject right)
        {
            return !Equals(left, right);
        }
        #endregion

        public override string ToString()
        {
            return "PerormanceTestObject #" + GetHashCode().ToString(CultureInfo.InvariantCulture);
        }
    }
}
