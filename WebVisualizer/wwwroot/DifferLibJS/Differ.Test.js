function testSameStringsSingleton()
{
    [deletes, inserts] = Differ.compute("a", "a");
    Assert.AreEqual(0, deletes.length);
    Assert.AreEqual(0, inserts.length);
}

function testSameStringsMultichars()
{
    [deletes, inserts] = Differ.compute("abc", "abc");
    Assert.AreEqual(0, deletes.length);
    Assert.AreEqual(0, inserts.length);
}

function testEmpty()
{
    [deletes, inserts] = Differ.compute("", "");
    Assert.AreEqual(0, deletes.length);
    Assert.AreEqual(0, inserts.length);
}

function testLineMatching()
{
    [deletes, _] = Differ.compute("_\n*\n", "*\n");
    Assert.AreEqual(1, deletes.length);
    Assert.AreEqual(0, deletes[0].start);
    Assert.AreEqual(2, deletes[0].length);
}

function testLengthOneDelete() { testInternal("a", ""); }

function testLengthOneInsert() { testInternal("", "a"); }

function TestMoreDiagonalPreferable() { testInternal("abc", "axb", 2); }

function testLongInsert() { testInternal("", "aaa"); }

function testLongDelete() { testInternal("aaa", ""); }

function testOptimality() { testInternal("abceb", "acbd", 3); }

function testInternal(from, to, nonDiagonalCount)
{
    [deletes, inserts] = Differ.compute(from, to);
    AssertConsistency(from, to, deletes, inserts);
    if (nonDiagonalCount) Assert.AreEqual(nonDiagonalCount, deletes.length + inserts.length);
}

function AssertConsistency(from, to, deletes, inserts)
{
    for (let i = deletes.length - 1; i >= 0; --i) {
        from = from.substring(0, deletes[i].start) + from.substring(deletes[i].end);
    }

    for (let i = inserts.length - 1; i >= 0; --i) {
        to = to.substring(0, inserts[i].start) + to.substring(inserts[i].end);
    }

    Assert.AreEqual(from, to);
}
