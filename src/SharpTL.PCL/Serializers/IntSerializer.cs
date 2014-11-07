// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntSerializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SharpTL.Serializers
{
    public class IntSerializer : TLBareTypeSerializerBase
    {
        private static readonly Type _SupportedType = typeof (int);

        public override uint ConstructorNumber
        {
            get { return 0xA8509BDAu; }
        }

        public override Type SupportedType
        {
            get { return _SupportedType; }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            return context.Streamer.ReadInt32();
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            context.Streamer.WriteInt32((int) obj);
        }
    }
}
