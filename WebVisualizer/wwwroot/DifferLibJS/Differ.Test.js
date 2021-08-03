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

function testLongInsert() { testInternal("", "aaa"); }

function testLongDelete() { testInternal("aaa", ""); }

function testOptimality() { testInternal("abceb", "acbd", 3); }

function testInternal(from, to, nonDiagonalCount)
{
    [deletes, inserts] = Differ.compute(from, to);
    var actual = applyOperations(from, to, deletes, inserts);
    Assert.AreEqual(to, actual);
    if (nonDiagonalCount) Assert.AreEqual(nonDiagonalCount, deletes.length + inserts.length);
}

function applyOperations(from, to, deletes, inserts)
{
    let insertIndex = inserts.length - 1;
    let deleteIndex = deletes.length - 1;

    let insertedCount = 0;

    for (let i = from.length; i >= 0; --i)
    {
        let shouldInsert = false;
        let shouldDelete = false;

        if (insertIndex >= 0)
        {
            const aliveCount = to.length - inserts[insertIndex].start - (insertedCount + inserts[insertIndex].length);

            if (from.length - aliveCount == i)
            {
                shouldInsert = true;
                from = from.slice(0, i) + to.slice(inserts[insertIndex].start, inserts[insertIndex].end) + from.slice(i);
                insertedCount += inserts[insertIndex].length;
                insertIndex -= 1;
            }
        }

        if (deleteIndex >= 0 && deletes[deleteIndex].start == i)
        {
            shouldDelete = true;
            from = from.slice(0, i) + from.slice(i + deletes[deleteIndex].length);
            deleteIndex -= 1;
        }

        if (shouldInsert && shouldDelete)
        {
            throw "";
        }
    }

    return from;
}
