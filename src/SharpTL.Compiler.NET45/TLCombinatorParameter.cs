// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLCombinatorParameter.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using SharpTL.Compiler.Utils;

namespace SharpTL.Compiler
{
    /// <summary>
    ///     TL combinator parameter.
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public class TLCombinatorParameter : IEquatable<TLCombinatorParameter>
    {
        private int _lastHashCode;
        private string _text;

        public TLCombinatorParameter(string originalName)
        {
            OriginalName = originalName;
            Name = originalName.ToConventionalCase(Case.PascalCase);
        }

        public string OriginalName { get; set; }

        public string Name { get; set; }

        public TLType Type { get; set; }

        public int Order { get; set; }

        public string Text
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            int currentHashCode = GetHashCode();
            if (_lastHashCode != currentHashCode)
            {
                _lastHashCode = currentHashCode;
                _text = string.IsNullOrWhiteSpace(Name) ? Type.Name : string.Format("{0}:{1}", Name, Type.Name);
            }
            return _text;
        }

        #region Equality
        public bool Equals(TLCombinatorParameter other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Name, other.Name) && string.Equals(Type, other.Type) && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TLCombinatorParameter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Order;
                return hashCode;
            }
        }

        public static bool operator ==(TLCombinatorParameter left, TLCombinatorParameter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TLCombinatorParameter left, TLCombinatorParameter right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
