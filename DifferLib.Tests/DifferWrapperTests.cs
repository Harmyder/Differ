using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace DifferLib.Tests
{
    [TestClass]
    public sealed class DifferWrapperTests
    {
        [DataRow(true)]
        [DataRow(false)]
        [DataTestMethod]
        public void TestDifference(bool isWindows)
        {
            var before = CombineLines(isWindows, "b", "C", "C");
            var after = CombineLines(isWindows, "c", "c", "b");

            var (deletesPreferLines, insertsPreferLines) = DifferWrapper.Compute(before, after, true);
            var (deletes, inserts) = DifferWrapper.Compute(before, after, false);

            Assert.AreEqual(1, deletesPreferLines.Count);
            Assert.AreEqual(1, insertsPreferLines.Count);
            Assert.IsTrue(1 < deletes.Count);
            Assert.IsTrue(1 < inserts.Count);
        }

        [TestMethod]
        public void TestDifferentEols()
        {
            var before = "a\r\n";
            var after = "a\n";

            var (deletes, inserts) = DifferWrapper.Compute(before, after, true);

            // For now ignores EOLs for lines
            Assert.IsFalse(0 == deletes.Count + inserts.Count);
        }

        [TestMethod]
        public void TestWithAndWithoutEol()
        {
            var before = "a\n";
            var after = "a";

            var (deletes, inserts) = DifferWrapper.Compute(before, after, true);

            // For now ignores EOLs for lines
            Assert.IsFalse(0 == deletes.Count + inserts.Count);
        }

        private string CombineLines(bool isWindows, params string[] lines)
        {
            var eol = isWindows ? "\r\n" : "\n";
            var builder = new StringBuilder();
            foreach (var line in lines)
            {
                builder.Append(line);
                builder.Append(eol);
            }
            return builder.ToString();
        }
    }
}
