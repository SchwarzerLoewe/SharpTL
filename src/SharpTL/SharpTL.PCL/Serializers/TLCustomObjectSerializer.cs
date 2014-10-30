// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLCustomObjectSerializer.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpTL.Serializers
{
    /// <summary>
    ///     Serializer for TL custom object.
    /// </summary>
    public class TLCustomObjectSerializer : TLSerializerBase
    {
        private readonly uint _constructorNumber;
        private readonly Type _objectType;
        private readonly TLPropertyInfo[] _properties;

        public TLCustomObjectSerializer(uint constructorNumber, Type objectType, IEnumerable<TLPropertyInfo> properties,
            TLSerializationMode serializationMode = TLSerializationMode.Boxed)
        {
            _constructorNumber = constructorNumber;
            _objectType = objectType;
            _properties = properties.OrderBy(info => info.Order).ToArray();
            SerializationMode = serializationMode;
        }

        public override uint ConstructorNumber
        {
            get { return _constructorNumber; }
        }

        public override Type SupportedType
        {
            get { return _objectType; }
        }

        protected override void WriteBody(object obj, TLSerializationContext context)
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                TLPropertyInfo tlPropertyInfo = _properties[i];
                PropertyInfo propertyInfo = tlPropertyInfo.PropertyInfo;

                Type propType = propertyInfo.PropertyType;

                object propertyValue = propertyInfo.GetValue(obj);

                if (propType == typeof (object))
                {
                    /*
                     * https://core.telegram.org/mtproto/serialize#object-pseudotype
                     * Object Pseudotype
                     * The Object pseudotype is a “type” which can take on values that belong to any boxed type in the schema.
                     */
                    TLRig.Serialize(propertyValue, context, TLSerializationMode.Boxed);
                }
                else
                {
                    ITLSerializer tlSerializer = context.Rig.GetSerializerByObjectType(propType);

                    if (tlSerializer is ITLVectorSerializer)
                    {
                        var vectorSerializer = tlSerializer as ITLVectorSerializer;
                        TLSerializationMode? itemsSerializationModeOverride = GetVectorItemsSerializationModeOverride(vectorSerializer, propertyInfo, context);
                        vectorSerializer.Write(propertyValue, context, tlPropertyInfo.SerializationModeOverride, itemsSerializationModeOverride);
                    }
                    else
                    {
                        tlSerializer.Write(propertyValue, context, tlPropertyInfo.SerializationModeOverride);
                    }
                }
            }
        }

        protected override object ReadBody(TLSerializationContext context)
        {
            object obj = Activator.CreateInstance(_objectType);
            for (int i = 0; i < _properties.Length; i++)
            {
                TLPropertyInfo tlPropertyInfo = _properties[i];
                PropertyInfo propertyInfo = tlPropertyInfo.PropertyInfo;

                object propertyValue;

                Type propType = propertyInfo.PropertyType;
                if (propType == typeof (object))
                {
                    /*
                     * https://core.telegram.org/mtproto/serialize#object-pseudotype
                     * Object Pseudotype
                     * The Object pseudotype is a “type” which can take on values that belong to any boxed type in the schema.
                     */
                    propertyValue = TLRig.Deserialize<object>(context, TLSerializationMode.Boxed);
                }
                else
                {
                    ITLSerializer tlSerializer = context.Rig.GetSerializerByObjectType(propType);
                    
                    if (tlSerializer is ITLVectorSerializer)
                    {
                        var vectorSerializer = tlSerializer as ITLVectorSerializer;
                        TLSerializationMode? itemsSerializationModeOverride = GetVectorItemsSerializationModeOverride(vectorSerializer, propertyInfo, context);
                        propertyValue = vectorSerializer.Read(context, tlPropertyInfo.SerializationModeOverride, itemsSerializationModeOverride);
                    }
                    else
                    {
                        propertyValue = tlSerializer.Read(context, tlPropertyInfo.SerializationModeOverride);
                    }
                }
                tlPropertyInfo.PropertyInfo.SetValue(obj, propertyValue);
            }
            return obj;
        }

        private TLSerializationMode? GetVectorItemsSerializationModeOverride(ITLVectorSerializer vectorSerializer, PropertyInfo propertyInfo,
            TLSerializationContext context)
        {
            Type propType = propertyInfo.PropertyType;

            if (vectorSerializer.SupportedType != propType)
            {
                throw new NotSupportedException(string.Format("Current vector serializer doesn't support type: {0}. It supports: {1}", propType,
                    vectorSerializer.SupportedType));
            }

            TLSerializationMode? itemsSerializationModeOverride = TLSerializationMode.Bare;

            // Check for items serializer.
            // If items have multiple constructors or have a TLTypeAttribute (in other words it is TL type),
            // then items must be serialized as boxed.
            Type itemsType = vectorSerializer.ItemsType;
            ITLSerializer vectorItemSerializer = context.Rig.GetSerializerByObjectType(itemsType);
            if (vectorItemSerializer is ITLMultiConstructorSerializer || itemsType.GetTypeInfo().GetCustomAttribute<TLTypeAttribute>() != null)
            {
                itemsSerializationModeOverride = TLSerializationMode.Boxed;
            }
            else
            {
                // Check for TLVector attribute with items serialization mode override.
                var tlVectorAttribute = propertyInfo.GetCustomAttribute<TLVectorAttribute>();
                if (tlVectorAttribute != null)
                {
                    itemsSerializationModeOverride = tlVectorAttribute.ItemsModeOverride;
                }
            }
            return itemsSerializationModeOverride;
        }
    }
}
