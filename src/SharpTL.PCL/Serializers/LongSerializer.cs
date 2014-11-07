// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongSerializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SharpTL.Serializers
{
    public class LongSerializer : TLBareTypeSerializerBase
    {
        private static readonly Type _SupportedType = typeof (long);

        public override uint ConstructorNumber
        {
            get { return 0x22076CBAu; }
        }

        public override Type SupportedType
        {
            get { return _SupportedType; }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            return context.Streamer.ReadInt64();
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            context.Streamer.WriteInt64((long) obj);
        }
    }
}
