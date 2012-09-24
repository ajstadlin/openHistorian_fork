﻿//******************************************************************************************************
//  BufferedFileStream_IoStream.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.IO.Unmanaged
{
    public partial class BufferedFileStream : ISupportsBinaryStreamSizing
    {
        // Nested Types
        class IoSession : IBinaryStreamIoSession
        {
            bool m_disposed;
            BufferedFileStream m_stream;
            LeastRecentlyUsedPageReplacement.IoSession m_ioSession;

            public IoSession(BufferedFileStream stream, LeastRecentlyUsedPageReplacement.IoSession ioSession)
            {
                m_stream = stream;
                m_ioSession = ioSession;
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.
                        m_ioSession.Dispose();
                    }
                    finally
                    {
                        m_disposed = true;  // Prevent duplicate dispose.
                    }
                }
            }

            public void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
            {
                m_stream.GetBlock(m_ioSession, position, isWriting, out firstPointer, out firstPosition, out length, out supportsWriting);
            }

            public void Clear()
            {
                m_ioSession.Clear();
            }

            public bool IsDisposed
            {
                get
                {
                    return m_disposed;
                }
            }
        }

    }
}
