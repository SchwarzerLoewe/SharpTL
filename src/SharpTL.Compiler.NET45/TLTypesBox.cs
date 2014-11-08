// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLTypesBox.cs">
//   Copyright (c) 2013-2014 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace SharpTL.Compiler
{
    public class TLTypesBox : IEnumerable<TLType>
    {
        private readonly Dictionary<string, TLType> _tlTypes = new Dictionary<string, TLType> {{"void", new VoidTLType()}};

        public TLType this[string typeName]
        {
            get
            {
                TLType type;
                if (!_tlTypes.TryGetValue(typeName, out type))
                {
                    type = new TLType(typeName);
                    _tlTypes.Add(typeName, type);
                }
                return type;
            }
        }

        public void Add(TLType tlType)
        {
            if (_tlTypes.ContainsKey(tlType.OriginalName))
            {
                return;
            }

            _tlTypes.Add(tlType.OriginalName, tlType);
        }

        public List<TLType> GetAll()
        {
            return new List<TLType>(_tlTypes.Values);
        }

        public bool Contains(string typeName)
        {
            return _tlTypes.ContainsKey(typeName);
        }

        public bool Contains(TLType tlType)
        {
            return _tlTypes.ContainsValue(tlType);
        }

        public IEnumerator<TLType> GetEnumerator()
        {
            return _tlTypes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
