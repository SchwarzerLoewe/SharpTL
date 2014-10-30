// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCustomSerializerObjects.cs">
//   Copyright (c) 2013-2014 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SharpTL.Serializers;

namespace SharpTL.Tests.TestData
{
    [TLObject(typeof (TestCustomSerializer))]
    public class TestCustomSerializerObject : IEquatable<TestCustomSerializerObject>
    {
        public TestCustomSerializerObject()
        {
        }

        public TestCustomSerializerObject(ulong msgId, uint seqno, object body)
        {
            MsgId = msgId;
            Seqno = seqno;
            Body = body;
        }

        public UInt64 MsgId { get; set; }

        public UInt32 Seqno { get; set; }

        public Object Body { get; set; }

        public bool Equals(TestCustomSerializerObject other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return MsgId == other.MsgId && Seqno == other.Seqno && Equals(Body, other.Body);
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((TestCustomSerializerObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = MsgId.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Seqno;
                hashCode = (hashCode*397) ^ (Body != null ? Body.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(TestCustomSerializerObject left, TestCustomSerializerObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestCustomSerializerObject left, TestCustomSerializerObject right)
        {
            return !Equals(left, right);
        }
    }

    public class TestCustomSerializer : TLSerializer<TestCustomSerializerObject>
    {
        public override uint ConstructorNumber
        {
            get { return 0xF4FD58C6; }
        }

        protected override TestCustomSerializerObject ReadTypedBody(TLSerializationContext context)
        {
            TLStreamer streamer = context.Streamer;

            ulong msgId = streamer.ReadUInt64();
            uint seqNo = streamer.ReadUInt32();
            int bodyLength = streamer.ReadInt32();

            if (streamer.BytesTillEnd < bodyLength)
            {
                throw new TLSerializationException(String.Format("Body length ({0}) is greated than available to read bytes till end ({1}).", bodyLength,
                    streamer.BytesTillEnd));
            }

            object body = TLRig.Deserialize(context);

            return new TestCustomSerializerObject(msgId, seqNo, body);
        }

        protected override void WriteTypedBody(TestCustomSerializerObject obj, TLSerializationContext context)
        {
            TLStreamer streamer = context.Streamer;

            streamer.WriteUInt64(obj.MsgId);
            streamer.WriteUInt32(obj.Seqno);

            // Skip 4 bytes for a body length.
            streamer.Position += 4;

            long bodyStartPosition = streamer.Position;
            TLRig.Serialize(obj.Body, context, TLSerializationMode.Boxed);
            long bodyEndPosition = streamer.Position;

            long bodyLength = bodyEndPosition - bodyStartPosition;
            
            streamer.Position = bodyStartPosition - 4;

            // Write a body length.
            streamer.WriteInt32((int) bodyLength);
        }
    }
}
