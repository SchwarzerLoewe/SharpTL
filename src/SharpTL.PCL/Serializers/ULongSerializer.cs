// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ULongSerializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SharpTL.Serializers
{
    public class ULongSerializer : TLBareTypeSerializerBase
    {
        private static readonly Type _SupportedType = typeof (ulong);

        public override Type SupportedType
        {
            get { return _SupportedType; }
        }

        public override uint ConstructorNumber
        {
            get { return 0x22076CBAu; }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            return context.Streamer.ReadUInt64();
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            context.Streamer.WriteUInt64((ulong) obj);
        }
    }
}
