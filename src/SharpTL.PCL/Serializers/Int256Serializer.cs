// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Int256Serializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using BigMath;

namespace SharpTL.Serializers
{
    /// <summary>
    ///     Serializer for 256-bit integer.
    /// </summary>
    public class Int256Serializer : TLBareTypeSerializerBase
    {
        private static readonly Type _SupportedType = typeof (Int256);

        public override uint ConstructorNumber
        {
            get { return 0x7BEDEB5B; }
        }

        public override Type SupportedType
        {
            get { return _SupportedType; }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            return context.Streamer.ReadInt256();
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            context.Streamer.WriteInt256((Int256)obj);
        }
    }
}
