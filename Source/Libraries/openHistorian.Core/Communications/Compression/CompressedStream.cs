﻿//******************************************************************************************************
//  CompressedStream.cs - Gbtc
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
//  8/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO;
using openHistorian.Collections;
using openHistorian.Communications.Initialization;

namespace openHistorian.Communications.Compression
{
    public class CompressedStream<TKey, TValue>
        : KeyValueStreamCompressionBase<TKey, TValue>
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {
        TKey prevKey;
        TValue prevValue;

        public CompressedStream()
        {
            prevKey = new TKey();
            prevValue = new TValue();
        }

        public override Guid CompressionType
        {
            get
            {
                return CreateCompressedStream.TypeGuid;
            }
        }

        public override void WriteEndOfStream(BinaryStreamBase stream)
        {
            stream.Write(false);
        }

        public override void Encode(BinaryStreamBase stream, TKey currentKey, TValue currentValue)
        {
            stream.Write(true);
            currentKey.WriteCompressed(stream, prevKey);
            currentValue.WriteCompressed(stream, prevValue);

            currentKey.CopyTo(prevKey);
            currentValue.CopyTo(prevValue);
        }

        public override unsafe bool TryDecode(BinaryStreamBase stream, TKey key, TValue value)
        {
            if (!stream.ReadBoolean())
                return false;
            key.ReadCompressed(stream, prevKey);
            value.ReadCompressed(stream, prevValue);

            key.CopyTo(prevKey);
            value.CopyTo(prevValue);

            return true;
        }

        public override void ResetEncoder()
        {
            prevKey.Clear();
            prevValue.Clear();
        }
    }
}