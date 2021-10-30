using DifferLib.Diff;
using DifferLib.Highlight;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DifferLib.Tests.Highlight
{
    [TestClass]
    public sealed class HighlighterTests
    {
        [TestMethod]
        public void TestEmptyDescriptors()
        {
            var descsEmpty = new List<SubstringDescriptor>();
            var highlighted = Highlighter.Highlight("_", "_", descsEmpty, descsEmpty);
            Assert.AreEqual(1, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "_" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_" }, highlighted[0].After.Blocks);
        }

        [TestMethod]
        public void TestNoOperationsOnALineInTheMiddle()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(2, 1) };
            var insert = new List<SubstringDescriptor>() { new SubstringDescriptor(2, 1) };
            var highlighted = Highlighter.Highlight("_\n_", "_\n*", delete, insert);
            Assert.AreEqual(2, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "", "_" }, highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "", "*" }, highlighted[1].After.Blocks);
        }

        [TestMethod]
        public void TestLineEndings()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1) };
            var insert = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1) };
            var highlighted = Highlighter.Highlight("_\r\n_", "*\r\n*", delete, insert);
            Assert.AreEqual(2, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "", "_", "\r\n" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "", "*", "\r\n" }, highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "", "_" }, highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "", "*" }, highlighted[1].After.Blocks);
        }

        [TestMethod]
        public void TestDifferentLineEndings()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1), new SubstringDescriptor(5, 1) };
            var insert = new List<SubstringDescriptor>() { new SubstringDescriptor(3, 1) };
            var highlighted = Highlighter.Highlight("_\r\n_\n_", "\r\n\n*", delete, insert);
            Assert.AreEqual(3, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "", "_", "\r\n" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "\r\n" },          highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "", "_", "\n" },   highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "\n" },            highlighted[1].After.Blocks);
            CollectionAssert.AreEqual(new[] { "", "_" },         highlighted[2].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "", "*" },         highlighted[2].After.Blocks);
        }

        [TestMethod]
        public void TestFullLineDelete()
        {
            var highlighted = Highlighter.Highlight("_", "", new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1) }, new List<SubstringDescriptor>());
            Assert.AreEqual(1, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "", "_" }, highlighted[0].Before.Blocks);
            Assert.AreEqual(null, highlighted[0].After);
        }

        [TestMethod]
        public void TestInterlineDelete()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(2, 3) };
            var insert = new List<SubstringDescriptor>() { };
            var highlighted = Highlighter.Highlight("_\n.\n_*", "_\n*", delete, insert);
            Assert.AreEqual(3, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "", ".\n" }, highlighted[1].Before.Blocks);
            Assert.AreEqual(null, highlighted[1].After);
            CollectionAssert.AreEqual(new[] { "", "_", "*" }, highlighted[2].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "*" }, highlighted[2].After.Blocks);
        }

        [TestMethod]
        public void TestDeleteFirstLine()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1) };
            var insert = new List<SubstringDescriptor>() { };
            var highlighted = Highlighter.Highlight("\n_", "_", delete, insert);
            Assert.AreEqual(2, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "", "\n" }, highlighted[0].Before.Blocks);
            Assert.AreEqual(null, highlighted[0].After);
            CollectionAssert.AreEqual(new[] { "_" }, highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_" }, highlighted[1].After.Blocks);
        }

        [TestMethod]
        public void TestChangeFullLine()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1) };
            var insert = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 1) };
            var highlighted = Highlighter.Highlight("_\n_", "*\n_", delete, insert);
            Assert.AreEqual(2, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "", "_\n" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "", "*\n" }, highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "_" }, highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_" }, highlighted[1].After.Blocks);
        }

        [TestMethod]
        public void TestChangeFullLastLine()
        {
            var delete = new List<SubstringDescriptor>() { new SubstringDescriptor(2, 1) };
            var insert = new List<SubstringDescriptor>() { new SubstringDescriptor(2, 1) };
            var highlighted = Highlighter.Highlight("_\n_", "_\n*", delete, insert);
            Assert.AreEqual(2, highlighted.Count);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[0].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "", "_" }, highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "", "*" }, highlighted[1].After.Blocks);
        }

        [TestMethod]
        public void TestInsertLine()
        {
            var delete = new List<SubstringDescriptor>() { };
            var insert = new List<SubstringDescriptor>() { new SubstringDescriptor(0, 2) };
            var highlighted = Highlighter.Highlight("_\n", "*\n_\n", delete, insert);
            Assert.AreEqual(2, highlighted.Count);
            Assert.AreEqual(null, highlighted[0].Before);
            CollectionAssert.AreEqual(new[] { "", "*\n" }, highlighted[0].After.Blocks);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[1].Before.Blocks);
            CollectionAssert.AreEqual(new[] { "_\n" }, highlighted[1].After.Blocks);
        }
    }
}
