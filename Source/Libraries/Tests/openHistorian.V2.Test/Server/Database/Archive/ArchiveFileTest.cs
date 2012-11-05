﻿using openHistorian.V2.Server.Database.Archive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using openHistorian.V2;

namespace openHistorian.V2.Test
{
    /// <summary>
    ///This is a test class for PartitionFileTest and is intended
    ///to contain all PartitionFileTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArchiveFileTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for PartitionFile Constructor
        ///</summary>
        [TestMethod()]
        public void PartitionFileConstructorTest()
        {
            using (ArchiveFile target = ArchiveFile.CreateInMemory())
            {
            }
        }

        /// <summary>
        ///A test for AddPoint
        ///</summary>
        [TestMethod()]
        public void AddPointTest()
        {
            using (ArchiveFile target = ArchiveFile.CreateInMemory())
            {
                ulong date = 0;
                ulong pointId = 0;
                ulong value1 = 0;
                ulong value2 = 0;
                using (var fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(date, pointId, value1, value2);
                    fileEditor.Commit();
                }
            }
        }

        /// <summary>
        ///A test for AddPoint
        ///</summary>
        [TestMethod()]
        public void EnduranceTest()
        {
            using (ArchiveFile target = ArchiveFile.CreateInMemory())
            {
                for (uint x = 0; x < 100; x++)
                {
                    using (var fileEditor = target.BeginEdit())
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            fileEditor.AddPoint(x, x, x, x);
                            x++;
                        }
                        fileEditor.Commit();
                    }
                    Assert.AreEqual(target.FirstKey, (ulong)0);
                    Assert.AreEqual(target.LastKey, (ulong)(x-1));
                }
                
            }
        }

        /// <summary>
        ///A test for CreateSnapshot
        ///</summary>
        [TestMethod()]
        public void CreateSnapshotTest()
        {
            using (ArchiveFile target = ArchiveFile.CreateInMemory())
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                ArchiveFileSnapshot snap1;
                using (var fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(date, pointId, value1, value2);
                    fileEditor.AddPoint(date + 1, pointId, value1, value2);
                    snap1 = target.CreateSnapshot();
                    fileEditor.Commit();
                }
                var snap2 = target.CreateSnapshot();

                using (var instance = snap1.OpenInstance())
                {
                    var scanner = instance.GetDataRange();
                    scanner.SeekToKey(0, 0);
                    Assert.AreEqual(false, scanner.GetNextKey(out date, out pointId, out value1, out value2));
                }
                using (var instance = snap2.OpenInstance())
                {
                    var scanner = instance.GetDataRange();
                    scanner.SeekToKey(0, 0);
                    Assert.AreEqual(true, scanner.GetNextKey(out date, out pointId, out value1, out value2));
                    Assert.AreEqual(1uL, date);
                    Assert.AreEqual(2uL, pointId);
                    Assert.AreEqual(3uL, value1);
                    Assert.AreEqual(4uL, value2);
                }
                Assert.AreEqual(1uL, target.FirstKey);
                Assert.AreEqual(2uL, target.LastKey);

            }
        }

        /// <summary>
        ///A test for RollbackEdit
        ///</summary>
        [TestMethod()]
        public void RollbackEditTest()
        {
            using (ArchiveFile target = ArchiveFile.CreateInMemory())
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                ArchiveFileSnapshot snap1;
                using (var fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(date, pointId, value1, value2);
                    snap1 = target.CreateSnapshot();
                    fileEditor.Rollback();
                }
                var snap2 = target.CreateSnapshot();

                using (var instance = snap1.OpenInstance())
                {
                    var scanner = instance.GetDataRange();
                    scanner.SeekToKey(0, 0);
                    Assert.AreEqual(false, scanner.GetNextKey(out date, out pointId, out value1, out value2));
                }
                using (var instance = snap2.OpenInstance())
                {
                    var scanner = instance.GetDataRange();
                    scanner.SeekToKey(0, 0);
                    Assert.AreEqual(false, scanner.GetNextKey(out date, out pointId, out value1, out value2));
                }
            }
        }



    }
}
