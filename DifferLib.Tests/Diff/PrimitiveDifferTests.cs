using DifferLib.Diff;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DifferLib.Tests.Diff
{
    [TestClass]
    public class PrimitiveDifferTests
    {
        [TestMethod]
        public void TestSameStringsSingleton()
        {
            var differ = new PrimitiveDiffer<char>("a".ToCharArray(), "a".ToCharArray());
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestSameStringsMultichars()
        {
            var differ = new PrimitiveDiffer<char>("abc".ToCharArray(), "abc".ToCharArray());
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestEmpty()
        {
            var differ = new PrimitiveDiffer<char>("".ToCharArray(), "".ToCharArray());
            var (deleted, inserted) = differ.Compute();
            Assert.IsFalse(deleted.Any());
            Assert.IsFalse(inserted.Any());
        }

        [TestMethod]
        public void TestLineMatching()
        {
            var differ = new PrimitiveDiffer<char>("_\n*\n".ToCharArray(), "*\n".ToCharArray());
            var (deletes, _) = differ.Compute();
            Assert.AreEqual(1, deletes.Count);
            Assert.AreEqual(0, deletes[0].Start);
            Assert.AreEqual(2, deletes[0].Length);
        }

        [TestMethod]
        public void TestLengthOneDelete() => Test("a", "");

        [TestMethod]
        public void TestMoreDiagonalPreferable() => Test("abc", "axb", 2);

        [TestMethod]
        public void TestLongDelete() => Test("aaa", "");

        [TestMethod]
        public void TestOptimality() => Test("abceb", "acbd", 3);

        [TestMethod]
        public void TestLines() => Test(new[] { "abc", "def", "ghi" }, new[] { "abc", "def", "jkl" });

        [TestMethod]
        public void TestIterations() => Test("abcdefgh", "a1b2c3d4e5f6g7h8");

        private void Test(string from, string to, int? nonDiagonalCount = null)
        {
            TestInternal(from.ToCharArray(), to.ToCharArray(), nonDiagonalCount);
            TestInternal(to.ToCharArray(), from.ToCharArray(), nonDiagonalCount);
        }

        private void Test(string[] from, string[] to, int? nonDiagonalCount = null) => TestInternal(from, to, nonDiagonalCount);

        private void TestInternal<T>(T[] from, T[] to, int? nonDiagonalCount = null)
        {
            var differ = new PrimitiveDiffer<T>(from, to);
            var (deletes, inserts) = differ.Compute();
            AssertConsistency(from.ToList(), to.ToList(), deletes, inserts);
            if (nonDiagonalCount != null) Assert.AreEqual(nonDiagonalCount, deletes.Count + inserts.Count);
        }

        private void AssertConsistency<T>(List<T> from, List<T> to, IReadOnlyList<SubstringDescriptor> deletes, IReadOnlyList<SubstringDescriptor> inserts)
        {
            foreach (var delete in deletes.Reverse())
            {
                from.RemoveRange(delete.Start, delete.Length);
            }

            foreach (var insert in inserts.Reverse())
            {
                to.RemoveRange(insert.Start, insert.Length);
            }

            Assert.AreEqual(string.Join("", from), string.Join("", to.ToArray()));
        }
    }
}
