﻿using System;
using System.Diagnostics;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Configuration;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using NUnit.Framework;
using openHistorian.Scada.AMI;

namespace openHistorian.Scada.Test
{
    [TestFixture]
    public class WriteAFile
    {

        [Test]
        public void TestFixed()
        {
            int count = 10000000;
            Stopwatch sw = new Stopwatch();
            using (var af = SortedTreeFile.CreateInMemory())
            using (var table = af.OpenOrCreateTable<AmiKey, AmiKey>(SortedTree.FixedSizeNode))
            {
                var key = new AmiKey();
                var value = new AmiKey();
                using (var edit = table.BeginEdit())
                {
                    sw.Start();


                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.ArchiveSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }

                using (var read = table.BeginRead())
                using (var scan = read.GetTreeScanner())
                {
                    scan.SeekToStart();
                    while (scan.Read(key, value))
                        count--;
                }
                Console.WriteLine(count);
            }
        }

        [Test]
        public void TestRollover()
        {
            int count = 1000000;
            Stopwatch sw = new Stopwatch();
            using (var af = SortedTreeFile.CreateInMemory())
            using (var table = af.OpenOrCreateTable<AmiKey, AmiKey>(SortedTree.FixedSizeNode))
            {
                using (var edit = table.BeginEdit())
                {
                    sw.Start();
                    var key = new AmiKey();
                    var value = new AmiKey();

                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.ArchiveSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }

                using (var af2 = SortedTreeFile.CreateInMemory())
                using (var table2 = af2.OpenOrCreateTable<AmiKey, AmiKey>(new EncodingDefinition(SortedTree.FixedSizeNode.KeyValueEncodingMethod, SortedTree.FixedSizeNode.KeyValueEncodingMethod)))
                using (var edit = table2.BeginEdit())
                {
                    using (var read = table.BeginRead())
                    using (var scan = read.GetTreeScanner())
                    {
                        scan.SeekToStart();
                        edit.AddPoints(scan);
                    }
                }

                Console.WriteLine(count);
            }
        }

        [Test]
        public void TestArchiveWriter()
        {
            Random r = new Random(3);
            var KV2CEncoding = new EncodingDefinition(CreateFixedSizeSingleEncoding.TypeGuid, CreateFixedSizeSingleEncoding.TypeGuid);
            var config = new AdvancedServerDatabaseConfig<AmiKey, AmiKey>("KV2CPQ", "C:\\Temp\\AMI", true);
            using (var server = new Server(config))
            {
                using (var client = Client.Connect(server))
                using (var db = client.GetDatabase<AmiKey, AmiKey>("KV2CPQ"))
                {
                    int count = 10000000;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var key = new AmiKey();
                    var value = new AmiKey();

                    for (int x = count; x >= 0; x--)
                    {
                        key.Timestamp = (ulong)r.Next();
                        key.TableId = r.Next();
                        db.Write(key, value);
                    }

                    sw.Stop();

                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);

                    Console.WriteLine(count);
                }
            }
        }

        //[Test]
        //public void TestFixed()
        //{
        //    int count = 10000000;
        //    Stopwatch sw = new Stopwatch();
        //    using (var af = SortedTreeFile.CreateInMemory())
        //    using (var table = af.OpenOrCreateTable<AmiKey, AmiKey>(SortedTree.FixedSizeNode))
        //    {
        //        using (var edit = table.BeginEdit())
        //        {
        //            sw.Start();
        //            var key = new AmiKey();
        //            var value = new AmiKey();

        //            for (int x = 0; x < count; x++)
        //            {
        //                key.Timestamp = (uint)x;
        //                edit.AddPoint(key, value);
        //            }

        //            edit.Commit();
        //            sw.Stop();

        //            Console.WriteLine(af.FileSize / 1024.0 / 1024.0);
        //            Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
        //        }

        //        using (var read = table.BeginRead())
        //        using (var scan = read.GetTreeScanner())
        //        {
        //            scan.SeekToStart();
        //            while (scan.Read())
        //                count--;
        //        }
        //        Console.WriteLine(count);
        //    }
        //}
        [Test]
        public void TestCompressed()
        {
            int count = 10000000;
            Stopwatch sw = new Stopwatch();
            using (var af = SortedTreeFile.CreateInMemory())
            using (var table = af.OpenOrCreateTable<AmiKey, AmiValue>(SortedTree.FixedSizeNode))
            {
                using (var edit = table.BeginEdit())
                {
                    sw.Start();
                    var key = new AmiKey();
                    var value = new AmiValue();

                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.ArchiveSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }
            }
        }
    }
}
