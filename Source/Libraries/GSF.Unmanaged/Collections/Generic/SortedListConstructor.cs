﻿//******************************************************************************************************
//  SortedListConstructor.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  8/20/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace GSF.Collections.Generic
{

    public static class SortedListConstructor
    {
        public static SortedList<TKey, TValue> Create<TKey, TValue>(ICollection<TKey> keys, ICollection<TValue> values)
        {
            return new DictionaryWrapper<TKey, TValue>(keys, values).ToSortedList();
        }

        private class DictionaryWrapper<TKey,TValue> 
            : IDictionary<TKey,TValue>
        {

            public DictionaryWrapper(ICollection<TKey> keys, ICollection<TValue> values)
            {
                Keys = keys;
                Values = values;
            }

            public SortedList<TKey, TValue> ToSortedList()
            {
                return new SortedList<TKey, TValue>(this);
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                using (var keys = Keys.GetEnumerator())
                using (var values = Values.GetEnumerator())
                {
                    while (keys.MoveNext() && values.MoveNext())
                        yield return new KeyValuePair<TKey, TValue>(keys.Current, values.Current);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get
                {
                    return Keys.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public bool ContainsKey(TKey key)
            {
                throw new NotImplementedException();
            }

            public void Add(TKey key, TValue value)
            {
                throw new NotImplementedException();
            }

            public bool Remove(TKey key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                throw new NotImplementedException();
            }

            public TValue this[TKey key]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<TKey> Keys { get; private set; }
            public ICollection<TValue> Values { get; private set; }
        }

    }
}
