// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using PowerArgs;
using SharpTL.Compiler.Annotations;

namespace SharpTL.Compiler.CLI
{
    public enum SchemaSourceType
    {
        TL,
        JSON
    }

    [ArgExample(@"SharpTL.Compiler.CLI compile json C:\schema1.json C:\schema1.cs", "Compile TL-schema described in JSON file."), UsedImplicitly]
    public class CompilerArgs
    {
        [ArgRequired]
        [ArgPosition(0)]
        public string Action { get; set; }

        public CompileArgs CompileArgs { get; set; }

        public static void Compile(CompileArgs args)
        {
            Console.WriteLine("Compiling...");
            string source = File.ReadAllText(args.Source);
            string compiled;
            switch (args.SourceType)
            {
                case SchemaSourceType.TL:
                    compiled = TLSchemaCompiler.CompileFromTL(source, args.Namespace);
                    break;
                case SchemaSourceType.JSON:
                    compiled = TLSchemaCompiler.CompileFromJson(source, args.Namespace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            File.WriteAllText(args.Destination, compiled);
            Console.WriteLine("Compilation done successfully.");
        }
    }

    [UsedImplicitly]
    public class CompileArgs
    {
        [ArgRequired]
        [ArgDescription("The type of the TL schema.")]
        [ArgPosition(1)]
        public SchemaSourceType SourceType { get; set; }

        [ArgRequired]
        [ArgDescription("The path to schema file.")]
        [ArgPosition(2)]
        public string Source { get; set; }

        [ArgRequired]
        [ArgDescription("The path to a file where to drop a compiled schema.")]
        [ArgPosition(3)]
        public string Destination { get; set; }

        [ArgRequired]
        [ArgDescription("Namespace for compiled C# code.")]
        [ArgPosition(4)]
        public string Namespace { get; set; }
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var parsed = Args.InvokeAction<CompilerArgs>(args);
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                ArgUsage.GenerateUsageFromTemplate<CompilerArgs>().Write();
            }
        }
    }
}
