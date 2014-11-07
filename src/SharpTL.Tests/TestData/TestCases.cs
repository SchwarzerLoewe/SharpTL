// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCases.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BigMath;
using NUnit.Framework;

namespace SharpTL.Tests.TestData
{
    public class TestCases
    {
        public static IEnumerable SerializationTestCasesData
        {
            get { return GetData().Select(dataUnit => new TestCaseData(dataUnit.Object).Returns(dataUnit.Bytes).SetName(dataUnit.Name)); }
        }

        public static IEnumerable DeserializationTestCasesData
        {
            get { return GetData().Select(dataUnit => new TestCaseData(dataUnit.Bytes, dataUnit.Object.GetType()).Returns(dataUnit.Object).SetName(dataUnit.Name)); }
        }

        private static IEnumerable<DataUnit> GetData()
        {
            return new List<DataUnit>
            {
                new DataUnit(int.MinValue, GetBytes(int.MinValue)),
                new DataUnit(int.MaxValue, GetBytes(int.MaxValue)),
                new DataUnit(uint.MinValue, GetBytes(uint.MinValue)),
                new DataUnit(uint.MaxValue, GetBytes(uint.MaxValue)),
                new DataUnit(long.MinValue, GetBytes(long.MinValue)),
                new DataUnit(long.MaxValue, GetBytes(long.MaxValue)),
                new DataUnit(ulong.MinValue, GetBytes(ulong.MinValue)),
                new DataUnit(ulong.MaxValue, GetBytes(ulong.MaxValue)),
                new DataUnit(double.MinValue, GetBytes(double.MinValue)),
                new DataUnit(double.MaxValue, GetBytes(double.MaxValue)),
                new DataUnit(double.Epsilon, GetBytes(double.Epsilon)),
                new DataUnit(double.NaN, GetBytes(double.NaN)),
                new DataUnit(double.PositiveInfinity, GetBytes(double.PositiveInfinity)),
                new DataUnit(double.NegativeInfinity, GetBytes(double.NegativeInfinity)),
                new DataUnit(true, GetBytes(0x997275b5)),
                new DataUnit(false, GetBytes(0xbc799737)),
                new DataUnit("P", GetBytes(0x5001)),
                new DataUnit("Pa", GetBytes(0x615002)),
                new DataUnit("Pav", GetBytes(0x76615003)),
                new DataUnit("Pave", GetBytes(0x76615004, 0x65)),
                new DataUnit("Pavel", GetBytes(0x76615005, 0x6c65)),
                new DataUnit(new string('P', 500), GetBytes(0x1F4FE, Enumerable.Repeat((byte) 0x50, 500))),
                new DataUnit(new string('P', 501), GetBytes(0x1F5FE, Enumerable.Repeat((byte) 0x50, 501), new byte[] {0, 0, 0})),
                new DataUnit(new string('P', 502), GetBytes(0x1F6FE, Enumerable.Repeat((byte) 0x50, 502), new byte[] {0, 0})),
                new DataUnit(new string('P', 503), GetBytes(0x1F7FE, Enumerable.Repeat((byte) 0x50, 503), new byte[] {0})),
                new DataUnit(
                    new TestObject
                    {
                        TestBoolean = true,
                        TestDouble = Double.Epsilon,
                        TestInt = Int32.MaxValue,
                        TestIntVector = new List<int> {1, 2, 3, 4, 5},
                        TestLong = Int64.MaxValue,
                        TestString = "PPP",
                        TestInt128 = Int128.Parse("0x0102030405060708090A0B0C0D0E0F10"),
                        TestInt256 = Int256.Parse("0x0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20"),
                        TestUsersVector =
                            new List<IUser>
                            {
                                new User {Id = 2, FirstName = "Pavel", LastName = "Durov", Key = new byte[] {1, 2, 3, 4, 5}},
                                new NoUser {Id = 3},
                                new User {Id = 4, FirstName = "Nikolay", LastName = "Durov", Key = new byte[] {6, 7, 8, 9, 10}}
                            },
                        TestIntBareVector = new List<int> {9, 99, 999, 9999, 99999, 999999},
                        TestInnerObject = 9
                    },
                    GetBytes(0xA1B2C3D4, 0x997275b5, Double.Epsilon, Int32.MaxValue, 0x1CB5C415, 5, 1, 2, 3, 4, 5, Int64.MaxValue, 0x50505003, 0x090A0B0C0D0E0F10,
                        0x0102030405060708UL, 0x191A1B1C1D1E1F20, 0x1112131415161718, 0x090A0B0C0D0E0F10, 0x0102030405060708UL, 0x1cb5c415, 0x3, 0xd23c81a3, 0x2, 0x76615005, 0x6c65,
                        0x72754405, 0x766f, 0x03020105, 0x0504, 0xc67599d1, 0x3, 0xd23c81a3, 0x4, 0x6b694e07, 0x79616c6f, 0x72754405, 0x766f, 0x08070605, 0x0A09,
                        6, 9, 99, 999, 9999, 99999, 999999, 
                        0xA8509BDAu, 9), "TestObject"),

                // getUsers([2,3,4])
                new DataUnit(new GetUsersFunction {Arg1 = new List<int> {2, 3, 4}}, GetBytes(0x2d84d5f5, 0x1cb5c415, 0x3, 0x2, 0x3, 0x4), "GetUsersFunction"),

                // Response to getUsers([2,3,4])
                new DataUnit(
                    new List<IUser>
                    {
                        new User {Id = 2, FirstName = "Pavel", LastName = "Durov", Key = new byte[] {1, 2, 3, 4, 5}},
                        new NoUser {Id = 3},
                        new User {Id = 4, FirstName = "Nikolay", LastName = "Durov", Key = new byte[] {6, 7, 8, 9, 10}}
                    },
                    GetBytes(0x1cb5c415, 0x3, 0xd23c81a3, 0x2, 0x76615005, 0x6c65, 0x72754405, 0x766f, 0x03020105, 0x0504, 0xc67599d1, 0x3, 0xd23c81a3, 0x4, 0x6b694e07, 0x79616c6f,
                        0x72754405, 0x766f, 0x08070605, 0x0A09), "List<IUser>"),
            };
        }

        private static byte[] GetBytes(params object[] values)
        {
            using (var stream = new MemoryStream())
            {
                using (var streamer = new TLStreamer(stream))
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        object value = values[i];
                        Type type = value.GetType();
                        if (type == typeof (byte))
                        {
                            streamer.WriteByte((byte) value);
                        }
                        else if (type == typeof (int))
                        {
                            streamer.WriteInt32((int) value);
                        }
                        else if (type == typeof (uint))
                        {
                            streamer.WriteUInt32((uint) value);
                        }
                        else if (type == typeof (long))
                        {
                            streamer.WriteInt64((long) value);
                        }
                        else if (type == typeof (ulong))
                        {
                            streamer.WriteUInt64((ulong) value);
                        }
                        else if (type == typeof (double))
                        {
                            streamer.WriteDouble((double) value);
                        }
                        else if (type == typeof (Array))
                        {
                            byte[] v = GetBytes(value);
                            streamer.Write(v, 0, v.Length);
                        }
                        else
                        {
                            var enumerable = value as IEnumerable;
                            if (enumerable != null)
                            {
                                byte[] v = GetBytes(enumerable.Cast<object>().ToArray());
                                streamer.Write(v, 0, v.Length);
                            }
                        }
                    }
                    return stream.ToArray();
                }
            }
        }

        private class DataUnit
        {
            public DataUnit(object o, byte[] bytes, string name = null)
            {
                Object = o;
                Bytes = bytes;
                Name = name ?? o.ToString();
            }

            public object Object { get; set; }
            public byte[] Bytes { get; set; }
            public string Name { get; set; }
        }
    }
}
