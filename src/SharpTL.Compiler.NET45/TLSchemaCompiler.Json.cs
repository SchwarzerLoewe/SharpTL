// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLSchemaCompiler.Json.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;

namespace SharpTL.Compiler
{
    public partial class TLSchemaCompiler
    {
        public static string CompileFromJson(string json, string defaultNamespace)
        {
            var compiler = new TLSchemaCompiler(defaultNamespace);
            TLSchema schema = compiler.GetTLSchemaFromJson(json);
            return compiler.Compile(schema);
        }

        public TLSchema GetTLSchemaFromJson(string json)
        {
            JsonObject tlSchemaJsonObject = JsonObject.Parse(json);

            List<TLCombinator> constructors = CreateConstructorsFromJsonArrayObjects(tlSchemaJsonObject.ArrayObjects("constructors"));
            List<TLCombinator> methods = CreateMethodsFromJsonArrayObjects(tlSchemaJsonObject.ArrayObjects("methods"));
            List<TLType> types = UpdateAndGetTLTypes(constructors);

            var schema = new TLSchema {Constructors = constructors, Methods = methods, Types = types};

            return schema;
        }

        private List<TLCombinator> CreateConstructorsFromJsonArrayObjects(JsonArrayObjects objects)
        {
            return CreateCombinatorsFromJsonArrayObjects(objects, "predicate");
        }

        private List<TLCombinator> CreateMethodsFromJsonArrayObjects(JsonArrayObjects objects)
        {
            return CreateCombinatorsFromJsonArrayObjects(objects, "method");
        }

        private List<TLCombinator> CreateCombinatorsFromJsonArrayObjects(JsonArrayObjects objects, string nameKey)
        {
            return
                objects.ConvertAll(
                    x =>
                        new TLCombinator(x.Get(nameKey))
                        {
                            Number = (uint) x.JsonTo<int>("id"),
                            Parameters = x.ArrayObjects("params").ConvertAll(param => new TLCombinatorParameter(param.Get("name")) {Type = GetTLType(param.Get("type"))}),
                            Type = GetTLType(x.Get("type"))
                        }).Where(combinator => !HasBuiltInSerializer(combinator.Number)).ToList();
        }
    }
}
