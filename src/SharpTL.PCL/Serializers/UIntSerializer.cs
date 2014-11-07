// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIntSerializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SharpTL.Serializers
{
    public class UIntSerializer : TLBareTypeSerializerBase
    {
        private static readonly Type _SupportedType = typeof (uint);

        public override Type SupportedType
        {
            get { return _SupportedType; }
        }

        public override uint ConstructorNumber
        {
            get { return 0xA8509BDAu; }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            return context.Streamer.ReadUInt32();
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            context.Streamer.WriteUInt32((uint) obj);
        }
    }
}
