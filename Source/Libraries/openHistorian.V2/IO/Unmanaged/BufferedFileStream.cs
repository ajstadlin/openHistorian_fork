﻿//******************************************************************************************************
//  BufferedFileStream.cs - Gbtc
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
using System.IO;
using System.Linq;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// A buffered file stream utilizes the buffer pool to intellectually cache
    /// the contents of files.  
    /// </summary>
    /// <remarks>
    /// The cache algorithm is a least recently used algorithm.
    /// but will place more emphysis on object that are repeatidly accessed over 
    /// ones that are rarely accessed. This is accomplised by incrementing a counter
    /// every time a page is accessed and dividing by 2 every time a collection occurs from the buffer pool.
    /// </remarks>
    unsafe public partial class BufferedFileStream : ISupportsBinaryStreamSizing
    {
        object m_syncRoot;
        object m_syncFlush;


        BufferPool m_pool;

        int m_dirtyPageSize;

        /// <summary>
        /// The file stream use by this class.
        /// </summary>
        FileStream m_baseStream;

        LeastRecentlyUsedPageReplacement m_pageReplacementAlgorithm;

        bool m_disposed;

        IoQueue m_queue;


        public BufferedFileStream(FileStream stream)
            : this(stream, Globals.BufferPool, 4096)
        {

        }

        /// <summary>
        /// Creates a file backed memory stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="pool"></param>
        /// <param name="dirtyPageSize"></param>
        public BufferedFileStream(FileStream stream, BufferPool pool, int dirtyPageSize)
        {
            m_pool = pool;
            m_dirtyPageSize = dirtyPageSize;
            m_queue = new IoQueue(stream, pool.PageSize, dirtyPageSize);

            m_syncRoot = new object();
            m_syncFlush = new object();
            m_pageReplacementAlgorithm = new LeastRecentlyUsedPageReplacement(dirtyPageSize, pool);
            m_baseStream = stream;
            Globals.BufferPool.RequestCollection += BufferPool_RequestCollection;
        }

        public int RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void Flush(bool waitForWriteToDisk = false, bool skipPagesInUse = true)
        {
            lock (m_syncFlush)
            {
                PageMetaDataList.PageMetaData[] dirtyPages;
                lock (m_syncRoot)
                {
                    dirtyPages = m_pageReplacementAlgorithm.GetDirtyPages(skipPagesInUse).ToArray();

                    foreach (var block in dirtyPages)
                    {
                        m_pageReplacementAlgorithm.ClearDirtyBits(block);
                    }
                }
                m_queue.Write(dirtyPages, waitForWriteToDisk);
            }
        }

        void GetBlock(LeastRecentlyUsedPageReplacement.IoSession ioSession, long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            LeastRecentlyUsedPageReplacement.SubPageMetaData subPage;

            lock (m_syncRoot)
            {
                if (ioSession.TryGetSubPage(position, isWriting, out subPage))
                {
                    firstPointer = (IntPtr)subPage.Location;
                    length = subPage.Length;
                    firstPosition = subPage.Position;
                    supportsWriting = subPage.IsDirty;
                    return;
                }
            }

            Action<byte[]> callback = data =>
                {
                    lock (m_syncRoot)
                    {
                        subPage = ioSession.CreateNew(position, isWriting, data, 0);
                    }
                };
            m_queue.Read(position, callback);

            firstPointer = (IntPtr)subPage.Location;
            length = subPage.Length;
            firstPosition = subPage.Position;
            supportsWriting = subPage.IsDirty;
            return;

        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                Globals.BufferPool.RequestCollection -= BufferPool_RequestCollection;
                m_pageReplacementAlgorithm.Dispose();
            }
        }

        void BufferPool_RequestCollection(object sender, CollectionEventArgs e)
        {
            if (e.CollectionMode == BufferPoolCollectionMode.Critical)
            {
                Flush();
            }
            lock (m_syncRoot)
            {
                m_pageReplacementAlgorithm.DoCollection();
            }
        }

        IBinaryStreamIoSession ISupportsBinaryStream.GetNextIoSession()
        {
            lock (m_syncRoot)
            {
                return new IoSession(this, m_pageReplacementAlgorithm.CreateNewIoSession());
            }
        }

        public IBinaryStream CreateBinaryStream()
        {
            return new BinaryStream(this);
        }

        long ISupportsBinaryStreamSizing.Length
        {
            get
            {
                return m_baseStream.Length;
            }
        }

        long ISupportsBinaryStreamSizing.SetLength(long length)
        {
            lock (m_syncRoot)
            {
                //if (m_baseStream.Length < length)
                m_baseStream.SetLength(length);
                return m_baseStream.Length;
            }
        }

        public int BlockSize
        {
            get
            {
                return Globals.BufferPool.PageSize;
            }
        }

        public void Flush()
        {
            Flush(false, true);
        }

        public void TrimEditsAfterPosition(long position)
        {
            lock (m_syncFlush)
            {

            }
        }

        public bool IsReadOnly
        {
            get
            {
                return m_baseStream.CanWrite;
            }
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
