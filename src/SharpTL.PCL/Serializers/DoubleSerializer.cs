// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleSerializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SharpTL.Serializers
{
    public class DoubleSerializer : TLBareTypeSerializerBase
    {
        private static readonly Type _SupportedType = typeof (double);

        public override uint ConstructorNumber
        {
            get { return 0x2210C154u; }
        }

        public override Type SupportedType
        {
            get { return _SupportedType; }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            return context.Streamer.ReadDouble();
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            context.Streamer.WriteDouble((double) obj);
        }
    }
}
