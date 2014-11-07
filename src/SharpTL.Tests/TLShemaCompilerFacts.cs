// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLShemaCompilerFacts.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SharpTL.Compiler;

namespace SharpTL.Tests
{
    [TestFixture]
    public class TLShemaCompilerFacts
    {
        private static string GetTestTLSchema()
        {
            return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TL-schemas", "Test.tl"));
        }

        private static string GetTestTLJsonSchema()
        {
            return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TL-schemas", "mtproto.json"));
        }

        [Test]
        public void Should_compile_TL_schema()
        {
            string sharpTLSchemaCode = TLSchemaCompiler.CompileFromJson(GetTestTLJsonSchema(), "SharpTL.TestNamespace");
            sharpTLSchemaCode.Should().NotBeNullOrEmpty();
        }
    }
}
