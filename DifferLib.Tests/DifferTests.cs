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
            var differ = new Differ<char>("a".ToCharArray(), "a".ToCharArray());
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestSameStringsMultichars()
        {
            var differ = new Differ<char>("abc".ToCharArray(), "abc".ToCharArray());
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestEmpty()
        {
            var differ = new Differ<char>("".ToCharArray(), "".ToCharArray());
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestLineMatching()
        {
            var differ = new Differ<char>("_\n*\n".ToCharArray(), "*\n".ToCharArray());
            var (deletes, _) = differ.Compute();
            Assert.AreEqual(1, deletes.Count);
            Assert.AreEqual(0, deletes[0].Start);
            Assert.AreEqual(2, deletes[0].Length);
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

        [TestMethod]
        public void TestLines() => Test(new[] { "abc", "def", "ghi" }, new[] { "abc", "def", "jkl" });

        private void Test(string from, string to, int? nonDiagonalCount = null) => TestInternal<char>(from.ToCharArray(), to.ToCharArray(), nonDiagonalCount);

        private void Test(string[] from, string[] to, int? nonDiagonalCount = null) => TestInternal<string>(from, to, nonDiagonalCount);

        private void TestInternal<T>(T[] from, T[] to, int? nonDiagonalCount = null)
        {
            var differ = new Differ<T>(from, to);
            var (deletes, inserts) = differ.Compute();
            var actual = ApplyOperations(from.ToList(), to.ToList(), deletes, inserts).ToArray();
            Assert.IsTrue(Enumerable.SequenceEqual(to, actual));
            if (nonDiagonalCount != null) Assert.AreEqual(nonDiagonalCount, deletes.Count + inserts.Count);
        }

        private List<T> ApplyOperations<T>(List<T> from, List<T> to, IReadOnlyList<SubstringDescriptor> deletes, IReadOnlyList<SubstringDescriptor> inserts)
        {
            var insertIndex = inserts.Count - 1;
            var deleteIndex = deletes.Count - 1;

            var insertedCount = 0;

            for (int i = from.Count; i >= 0; --i)
            {
                var shouldInsert = false;
                var shouldDelete = false;

                if (insertIndex >= 0)
                {
                    var aliveCount = to.Count - inserts[insertIndex].Start - (insertedCount + inserts[insertIndex].Length);

                    if (from.Count - aliveCount == i)
                    {
                        shouldInsert = true;
                        from.InsertRange(i, to.GetRange(inserts[insertIndex].Start, inserts[insertIndex].Length));
                        insertedCount += inserts[insertIndex].Length;
                        insertIndex -= 1;
                    }
                }

                if (deleteIndex >= 0 && deletes[deleteIndex].Start == i)
                {
                    shouldDelete = true;
                    from.RemoveRange(i, deletes[deleteIndex].Length);
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
