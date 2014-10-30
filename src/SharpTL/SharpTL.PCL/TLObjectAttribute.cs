// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLObjectAttribute.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace SharpTL
{
    /// <summary>
    ///     TL object attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TLObjectAttribute : Attribute
    {
        private static readonly TypeInfo _TLSerializerTypeInfo = typeof (ITLSingleConstructorSerializer).GetTypeInfo();

        /// <summary>
        ///     Initializes a new instance of the <see cref="TLObjectAttribute" /> class.
        /// </summary>
        /// <param name="constructorNumber">Constructor number.</param>
        public TLObjectAttribute(uint constructorNumber) : this()
        {
            ConstructorNumber = constructorNumber;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TLObjectAttribute" /> class.
        /// </summary>
        /// <param name="customSerializerType">
        ///     Custom serializer type must be a non abstract class which implements
        ///     <see cref="ITLSingleConstructorSerializer" />.
        /// </param>
        public TLObjectAttribute(Type customSerializerType) : this()
        {
            TypeInfo serTypeInfo = customSerializerType.GetTypeInfo();
            if (!_TLSerializerTypeInfo.IsAssignableFrom(serTypeInfo) || serTypeInfo.IsAbstract || serTypeInfo.IsGenericType)
            {
                throw new TLSerializationException(String.Format("Invalid custom serializer type {0}.", customSerializerType));
            }

            CustomSerializerType = customSerializerType;
        }

        private TLObjectAttribute()
        {
            SerializationMode = TLSerializationMode.Boxed;
        }

        public Type CustomSerializerType { get; private set; }

        /// <summary>
        ///     Constructor number.
        /// </summary>
        public uint ConstructorNumber { get; private set; }

        /// <summary>
        ///     Serialization mode.
        /// </summary>
        public TLSerializationMode SerializationMode { get; set; }
    }
}
