// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLSchema.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SharpTL.Compiler
{
    /// <summary>
    ///     TL-schema.
    /// </summary>
    public class TLSchema
    {
        public List<TLCombinator> Constructors { get; set; }
        
        public List<TLCombinator> Methods { get; set; }

        public List<TLType> Types { get; set; }
    }
}
