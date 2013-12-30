﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLSerializersBucket.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharpTL.Serializers;

namespace SharpTL
{
    /// <summary>
    ///     TL serialization bucket.
    /// </summary>
    public class TLSerializersBucket
    {
        private static readonly Type _GenericListType = typeof (List<>);
        private static readonly Type _GenericTLVectorSerializerType = typeof (TLVectorSerializer<>);
        private readonly Dictionary<uint, ITLSerializer> _constructorNumberSerializersIndex = new Dictionary<uint, ITLSerializer>();
        private readonly Dictionary<Type, ITLSerializer> _serializersIndex = new Dictionary<Type, ITLSerializer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TLSerializersBucket"/> class.
        /// </summary>
        public TLSerializersBucket()
        {
            // Add base type serializers.
            foreach (var serializer in BuiltIn.BaseTypeSerializers)
            {
                Add(serializer);
            }
        }

        /// <summary>
        ///     Get TL serializer for an object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns>TL serializer.</returns>
        public ITLSerializer this[Type type]
        {
            get
            {
                PrepareSerializer(type);
                return _serializersIndex.ContainsKey(type) ? _serializersIndex[type] : null;
            }
        }

        /// <summary>
        ///     Get TL serializer for a constructor number.
        /// </summary>
        /// <param name="constructorNumber">Constructor number.</param>
        /// <returns>TL serializer.</returns>
        public ITLSerializer this[uint constructorNumber]
        {
            get
            {
                ITLSerializer serializer;
                if (_constructorNumberSerializersIndex.TryGetValue(constructorNumber, out serializer))
                {
                    return serializer;
                }
                return null;
            }
        }

        /// <summary>
        ///     Does the bucket contain serializer for a type.
        /// </summary>
        /// <param name="type">Type of an object.</param>
        public bool Contains(Type type)
        {
            return _serializersIndex.ContainsKey(type);
        }

        /// <summary>
        ///     Adds serializer.
        /// </summary>
        /// <param name="serializer">TL serializer.</param>
        public void Add(ITLSerializer serializer)
        {
            Type type = serializer.SupportedType;
            if (!_serializersIndex.ContainsKey(type))
            {
                _serializersIndex.Add(type, serializer);

                var singleConstructorSerializer = serializer as ITLSingleConstructorSerializer;
                if (singleConstructorSerializer != null)
                {
                    IndexType(singleConstructorSerializer.ConstructorNumber, serializer);
                }
                var multipleConstructorSerializer = serializer as ITLMultiConstructorSerializer;
                if (multipleConstructorSerializer != null)
                {
                    foreach (uint constructorNumber in multipleConstructorSerializer.ConstructorNumbers)
                    {
                        IndexType(constructorNumber, serializer);
                    }
                }
            }
        }

        /// <summary>
        ///     Prepare serializer for an object type.
        /// </summary>
        /// <param name="objType">Object type.</param>
        public void PrepareSerializer(Type objType)
        {
            if (Contains(objType))
            {
                return;
            }

            TypeInfo objTypeInfo = objType.GetTypeInfo();

            if (objTypeInfo.IsInterface)
            {
                var tlTypeAttribute = objTypeInfo.GetCustomAttribute<TLTypeAttribute>();
                if (tlTypeAttribute == null)
                {
                    return;
                }

                ITLSingleConstructorSerializer[] serializers =
                    tlTypeAttribute.ConstructorTypes.Where(type => objTypeInfo.IsAssignableFrom(type.GetTypeInfo()))
                        .Select(type => this[type] as ITLSingleConstructorSerializer)
                        .Where(s => s != null)
                        .ToArray();
                Add(new TLMultiConstructorObjectSerializer(objType, serializers));

                return;
            }

            if (objTypeInfo.IsAbstract)
            {
                return;
            }

            var tlObjectAttribute = objTypeInfo.GetCustomAttribute<TLObjectAttribute>();
            if (tlObjectAttribute != null)
            {
                // There is a TLObjectAttribute, then use this meta-info to create properties map for object serialization.
                List<TLPropertyInfo> props =
                    objTypeInfo.DeclaredProperties.Zip(objTypeInfo.DeclaredProperties.Select(info => info.GetCustomAttribute<TLPropertyAttribute>()),
                        (info, attribute) => new Tuple<PropertyInfo, TLPropertyAttribute>(info, attribute))
                        .Where(tuple => tuple.Item2 != null)
                        .Select(tuple => new TLPropertyInfo(tuple.Item2.Order, tuple.Item1, tuple.Item2.SerializationModeOverride))
                        .ToList();

                Add(new TLCustomObjectSerializer(tlObjectAttribute.ConstructorNumber, objType, props, tlObjectAttribute.SerializationMode));

                foreach (TLPropertyInfo tlPropertyInfo in props)
                {
                    PrepareSerializer(tlPropertyInfo.PropertyInfo.PropertyType /*, tlPropertyInfo.PropertyInfo.GetCustomAttributes()*/);
                }
            }
            else
            {
                // Otherwise check for base supported types.
                // List<> will be serialized as built-in type 'vector'.
                if (objType.GetTypeInfo().IsGenericType && objType.GetGenericTypeDefinition() == _GenericListType)
                {
                    Type genericVectorSerializerType = _GenericTLVectorSerializerType.MakeGenericType(objTypeInfo.GenericTypeArguments[0]);
                    var serializer = (ITLSerializer) Activator.CreateInstance(genericVectorSerializerType);
                    Add(serializer);
                }
                else
                {
                    throw new NotSupportedException(string.Format("'{0}' is not supported. Only base types and objects with TLObject attribute are supported.", objType));
                }
            }
        }

        private void IndexType(uint constructorNumber, ITLSerializer serializer)
        {
            if (!_constructorNumberSerializersIndex.ContainsKey(constructorNumber))
            {
                _constructorNumberSerializersIndex.Add(constructorNumber, serializer);
            }
        }
    }
}
