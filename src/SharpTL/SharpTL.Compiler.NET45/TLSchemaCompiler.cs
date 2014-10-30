// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLSchemaCompiler.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BigMath;
using SharpTL.Serializers;

namespace SharpTL.Compiler
{
    /// <summary>
    ///     TL-schema.
    /// </summary>
    public partial class TLSchemaCompiler
    {
        private static readonly Regex VectorRegex = new Regex(@"^(?:(?<Boxed>V)|(?<Bare>v))ector<(?<ItemsType>%?\w[\w\W-[\s]]*)>$", RegexOptions.Compiled);
        private static readonly Regex BareTypeRegex = new Regex(@"^%(?<Type>\w+)$", RegexOptions.Compiled);
        private static readonly Regex Int32Regex = new Regex(@"^int$", RegexOptions.Compiled);
        private static readonly Regex Int64Regex = new Regex(@"^long$", RegexOptions.Compiled);
        private static readonly Regex Int128Regex = new Regex(@"^int128$", RegexOptions.Compiled);
        private static readonly Regex Int256Regex = new Regex(@"^int256$", RegexOptions.Compiled);
        private static readonly Regex TLBytesRegex = new Regex(@"^bytes$", RegexOptions.Compiled);
        private static readonly Regex TLObjectRegex = new Regex(@"^Object$", RegexOptions.Compiled);
        private readonly string _defaultNamespace;
        private readonly Dictionary<string, TLType> _tlTypesCache = new Dictionary<string, TLType> {{"void", new VoidTLType()}};

        /// <summary>
        ///     Initializes a new instance of the <see cref="TLSchemaCompiler" /> class.
        /// </summary>
        /// <param name="defaultNamespace">Default namespace.</param>
        protected TLSchemaCompiler(string defaultNamespace)
        {
            _defaultNamespace = defaultNamespace;
        }

        protected string Compile(TLSchema schema)
        {
            SetBuiltInTypeNames(schema);
            FixVoidReturns(schema);

            var template = new SharpTLDefaultTemplate(new TemplateVars {Schema = schema, Namespace = _defaultNamespace});
            return template.TransformText();
        }

        private void FixVoidReturns(TLSchema schema)
        {
            foreach (var method in schema.Methods.Where(method => !schema.Types.Contains(method.Type)))
            {
                method.Type = _tlTypesCache["void"];
            }
        }

        private List<TLType> UpdateAndGetTLTypes(IEnumerable<TLCombinator> constructors)
        {
            var types = new List<TLType>();
            foreach (TLCombinator constructor in constructors)
            {
                TLType type = constructor.Type;
                if (!type.Constructors.Contains(constructor))
                {
                    type.Constructors.Add(constructor);
                }
                if (!types.Contains(type))
                {
                    types.Add(type);
                }
            }
            return types;
        }

        private static string GetBuiltInTypeName(uint constructorNumber)
        {
            return (from serializer in BuiltIn.BaseTypeSerializers
                let singleConstructorSerializer = serializer as ITLSingleConstructorSerializer
                let multiConstructorSerializer = serializer as ITLMultiConstructorSerializer
                where
                    (singleConstructorSerializer != null && constructorNumber == singleConstructorSerializer.ConstructorNumber) ||
                        multiConstructorSerializer != null && multiConstructorSerializer.ConstructorNumbers.Contains(constructorNumber)
                select serializer.SupportedType.FullName).FirstOrDefault();
        }

        private static bool HasBuiltInSerializer(uint constructorNumber)
        {
            return GetBuiltInTypeName(constructorNumber) != null;
        }

        private TLType GetTLType(string typeName)
        {
            TLType type;
            if (!_tlTypesCache.TryGetValue(typeName, out type))
            {
                type = new TLType(typeName);
                _tlTypesCache.Add(typeName, type);
            }
            return type;
        }

        private void SetBuiltInTypeNames(TLSchema schema)
        {
            foreach (TLCombinatorParameter parameter in schema.Constructors.SelectMany(constructor => constructor.Parameters))
            {
                FixType(parameter.Type, schema);
            }
        }

        private void FixType(TLType type, TLSchema schema)
        {
            List<TLCombinator> constructors = schema.Constructors;
            string typeName = type.OriginalName;

            // Vector.
            Match match = VectorRegex.Match(typeName);
            if (match.Success)
            {
                TLType itemsType = GetTLType(match.Groups["ItemsType"].Value);
                FixType(itemsType, schema);
                type.Name = string.Format("System.Collections.Generic.List<{0}>", itemsType.Name);
                if (match.Groups["Bare"].Success)
                {
                    type.SerializationModeOverride = TLSerializationMode.Bare;
                }
                return;
            }

            // int.
            match = Int32Regex.Match(typeName);
            if (match.Success)
            {
                type.ClrType = typeof(UInt32);
                return;
            }

            // long.
            match = Int64Regex.Match(typeName);
            if (match.Success)
            {
                type.ClrType = typeof(UInt64);
                return;
            }

            // int128.
            match = Int128Regex.Match(typeName);
            if (match.Success)
            {
                type.ClrType = typeof(Int128);
                return;
            }

            // int256.
            match = Int256Regex.Match(typeName);
            if (match.Success)
            {
                type.ClrType = typeof(Int256);
                return;
            }

            // bytes.
            match = TLBytesRegex.Match(typeName);
            if (match.Success)
            {
                type.ClrType = typeof(byte[]);
                return;
            }

            // % bare types.
            match = BareTypeRegex.Match(typeName);
            if (match.Success)
            {
                typeName = match.Groups["Type"].Value;
                type.Name = constructors.Where(combinator => combinator.Type.Name == typeName).Select(combinator => combinator.Name).SingleOrDefault() ?? typeName;
                type.SerializationModeOverride = TLSerializationMode.Bare;
                return;
            }

            // Object.
            match = TLObjectRegex.Match(typeName);
            if (match.Success)
            {
                type.ClrType = typeof (Object);
                if (schema.Types.Contains(type))
                {
                    schema.Types.Remove(type);
                }
                return;
            }
        }
    }
}
