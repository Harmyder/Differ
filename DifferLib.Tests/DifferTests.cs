using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DifferLib.Tests
{
    [TestClass]
    public class DifferTests
    {
        [TestMethod]
        public void TestSameStringsSingleton()
        {
            var differ = new Differ("a", "a");
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestSameStringsMultichars()
        {
            var differ = new Differ("abc", "abc");
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestEmpty()
        {
            var differ = new Differ("", "");
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestLengthOneDelete() => Test("a", "");

        [TestMethod]
        public void TestLengthOneInsert() => Test("", "a");

        [TestMethod]
        public void TestLongInsert() => Test("", "aaa");

        [TestMethod]
        public void TestLongDelete() => Test("aaa", "");

        [TestMethod]
        public void TestOptimality() => Test("abceb", "acbd", 3);

        private void Test(string from, string to, int? nonDiagonalCount = null)
        {
            var differ = new Differ(from, to);
            var (deletes, inserts) = differ.Compute();
            var actual = ApplyOperations(from, to, deletes, inserts);
            Assert.AreEqual(to, actual);
            if (nonDiagonalCount != null) Assert.AreEqual(nonDiagonalCount, deletes.Count + inserts.Count);
        }

        private string ApplyOperations(string from, string to, IReadOnlyList<DeleteOperation> deletes, IReadOnlyList<InsertOperation> inserts)
        {
            var insertIndex = inserts.Count - 1;
            var deleteIndex = deletes.Count - 1;

            for (int i = from.Length; i >= 0; --i)
            {
                var shouldInsert = false;
                var shouldDelete = false;

                if (insertIndex >= 0 && inserts[insertIndex].StartOriginal == i)
                {
                    shouldInsert = true;
                    from = from.Insert(i, to.Substring(inserts[insertIndex].StartNew, inserts[insertIndex].Length));
                    insertIndex -= 1;
                }

                if (deleteIndex >= 0 && deletes[deleteIndex].StartOriginal == i)
                {
                    shouldDelete = true;
                    from = from.Remove(i, deletes[deleteIndex].Length);
                    deleteIndex -= 1;
                }

                if (shouldInsert && shouldDelete)
                {
                    throw new InvalidProgramException();
                }
            }

            return from;
        }
    }
}
