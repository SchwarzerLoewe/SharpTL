// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLSerializersBucketFacts.cs">
//   Copyright (c) 2013-2014 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using NUnit.Framework;
using SharpTL.Serializers;

namespace SharpTL.Tests
{
    [TestFixture]
    public class TLSerializersBucketFacts
    {
        [Test]
        public void Should_prepare_serializer_for_TLObject()
        {
            var serializersBucket = new TLSerializersBucket();
            serializersBucket.PrepareSerializer<CustomObject1>();

            ITLSerializer serializer = serializersBucket[CustomObject1.ConstructorNumber];
            serializer.Should().NotBeNull().And.BeAssignableTo<ITLSingleConstructorSerializer>();

            serializer.SupportedType.Should().Be<CustomObject1>();

            var scSerializer = (ITLSingleConstructorSerializer) serializer;
            scSerializer.ConstructorNumber.Should().Be(CustomObject1.ConstructorNumber);

            serializersBucket[typeof (CustomObject1)].Should().NotBeNull().And.BeSameAs(serializer);
        }

        [Test]
        public void Should_prepare_serializer_for_TLObject_with_custom_serializer()
        {
            const uint serConstructorNumber = CustomObject2Serializer.DefaultConstructorNumber;

            var serializersBucket = new TLSerializersBucket();
            serializersBucket.PrepareSerializer<CustomObject2>();

            ITLSerializer serializer = serializersBucket[serConstructorNumber];
            serializer.Should().NotBeNull().And.BeAssignableTo<CustomObject2Serializer>();

            serializer.SupportedType.Should().Be<CustomObject2>();

            var scSerializer = (CustomObject2Serializer) serializer;
            scSerializer.ConstructorNumber.Should().Be(serConstructorNumber);

            serializersBucket[typeof (CustomObject2)].Should().NotBeNull().And.BeSameAs(serializer);
        }

        [Test]
        public void Should_prepare_serializer_for_TLObject_with_custom_serializer_and_with_constructor_number_override()
        {
            const uint serConstructorNumber = CustomObject3Serializer.DefaultConstructorNumber;

            CustomObject3.ConstructorNumber.Should().NotBe(serConstructorNumber);

            var serializersBucket = new TLSerializersBucket();
            serializersBucket.PrepareSerializer<CustomObject3>();

            ITLSerializer serializer1 = serializersBucket[CustomObject3.ConstructorNumber];
            serializer1.Should().NotBeNull().And.BeAssignableTo<CustomObject3Serializer>();

            serializer1.SupportedType.Should().Be<CustomObject3>();

            var scSerializer = (CustomObject3Serializer) serializer1;
            scSerializer.ConstructorNumber.Should().Be(CustomObject3.ConstructorNumber);

            serializersBucket[typeof (CustomObject3)].Should().NotBeNull().And.BeSameAs(serializer1);
        }
    }

    [TLObject(ConstructorNumber)]
    public class CustomObject1 : ICustomObject
    {
        public const uint ConstructorNumber = 0x1;

        [TLProperty(1)]
        public int Number { get; set; }
    }

    [TLObjectWithCustomSerializer(typeof (CustomObject2Serializer))]
    public class CustomObject2 : ICustomObject
    {
        public int Number { get; set; }
    }

    [TLObject(ConstructorNumber)] // Override default constructor number from custom serializer.
    [TLObjectWithCustomSerializer(typeof (CustomObject3Serializer))]
    public class CustomObject3 : ICustomObject
    {
        public const uint ConstructorNumber = 0x3;

        public int Number { get; set; }
    }

    public interface ICustomObject
    {
        int Number { get; set; }
    }

    public class CustomObject2Serializer : CustomObjectSerializer<CustomObject2>
    {
        public const uint DefaultConstructorNumber = 0xC2;

        public CustomObject2Serializer() : base(DefaultConstructorNumber)
        {
        }
    }

    public class CustomObject3Serializer : CustomObjectSerializer<CustomObject3>
    {
        public const uint DefaultConstructorNumber = 0xC3;

        public CustomObject3Serializer() : base(DefaultConstructorNumber)
        {
        }
    }

    public abstract class CustomObjectSerializer<T> : TLSerializer<T> where T : ICustomObject, new()
    {
        protected CustomObjectSerializer(uint constructorNumber) : base(constructorNumber)
        {
        }

        protected override T ReadTypedBody(TLSerializationContext context)
        {
            return new T {Number = context.Streamer.ReadInt32()};
        }

        protected override void WriteTypedBody(T obj, TLSerializationContext context)
        {
            context.Streamer.WriteInt32(obj.Number);
        }
    }
}
