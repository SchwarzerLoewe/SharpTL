// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLRigFacts.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using SharpTL.Tests.TestData;

namespace SharpTL.Tests
{
    [TestFixture]
    public class TLRigFacts
    {
        [Test, TestCaseSource(typeof (TestCases), "DeserializationTestCasesData")]
        public object Should_deserialize_object(byte[] objBytes, Type objType)
        {
            return TLRig.Default.Deserialize(objBytes, objType);
        }

        [Test]
        //[Ignore("There is a problem in distinguishing of 'string' and 'byte[]' types in Durov mode")]
        public void Should_serialize_and_deserialize_heterogeneous_vector()
        {
            TLRig.Default.PrepareSerializersForAllTLObjectsInAssembly(Assembly.GetExecutingAssembly());
            var vector = new List<object>
            {
                new List<object> {new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9}, new List<object> {"Test", 0x4321}, 0x1234},
                new User {Id = 9, FirstName = "Alexander", LastName = "L", Key = new byte[] {0, 1, 2, 3, 4, 5}},
                "Something here. Привет.",
                0x100500,
                new List<string> {"One.", "Two.", "Three."},
                new byte[] {0, 255, 1, 2, 3, 4, 5},
                new NoUser {Id = 500},
                new List<IUser>
                {
                    new User {Id = 1, FirstName = "John", LastName = "Doe", Key = new byte[] {0, 1, 2, 3, 4, 5}},
                    new User {Id = 2, FirstName = "Peter", LastName = "Parker", Key = new byte[] {0, 1, 2, 3, 4, 5}},
                    new NoUser {Id = 3}
                }
            };
            byte[] bytes = TLRig.Default.Serialize(vector);

            var deserializedVector = TLRig.Default.Deserialize(bytes) as List<object>;
            deserializedVector.Should().NotBeNull();

            Assert.AreEqual(vector, deserializedVector);
        }

        [Test]
        public void Should_serialize_and_deserialize_list_of_objects_with_custom_serializer()
        {
            var obj = new List<TestCustomSerializerObject>
            {
                new TestCustomSerializerObject(1, 1, 1),
                new TestCustomSerializerObject(2, 2, 2),
                new TestCustomSerializerObject(3, 3, 3)
            };
            Should_serialize_and_deserialize(obj);
        }

        [Test]
        public void Should_serialize_and_deserialize_object_with_custom_serializer()
        {
            var obj = new TestCustomSerializerObject(100500, 9, "Does anybody really know the secret?");
            Should_serialize_and_deserialize(obj);
        }

        private static void Should_serialize_and_deserialize<T>(T obj)
        {
            byte[] objBytes = TLRig.Default.Serialize(obj);
            var deserializedObj = TLRig.Default.Deserialize<T>(objBytes);
            deserializedObj.Should().NotBeNull();
            deserializedObj.ShouldBeEquivalentTo(obj);
        }

        [Test, TestCaseSource(typeof (TestCases), "SerializationTestCasesData")]
        public byte[] Should_serialize_object(object obj)
        {
            return TLRig.Default.Serialize(obj);
        }

        [Test]
        public void Should_throw_not_supported_exception()
        {
            new object().Invoking(o => TLRig.Default.Serialize(o, new MemoryStream(0))).ShouldThrow<TLSerializerNotFoundException>();
        }
    }
}
