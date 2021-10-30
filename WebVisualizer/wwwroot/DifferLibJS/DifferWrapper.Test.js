function testDifference() {
    const before = combineLines(true, ["b", "C", "C"]);
    const after = combineLines(true, ["c", "c", "b"]);

    const [deletesPreferLines, insertsPreferLines] = DifferWrapper.compute(before, after, true);
    const [deletes, inserts] = DifferWrapper.compute(before, after, false);

    Assert.AreEqual(1, deletesPreferLines.length);
    Assert.AreEqual(1, insertsPreferLines.length);
    Assert.IsTrue(1 < deletes.length);
    Assert.IsTrue(1 < inserts.length);
}

function combineLines(isWindows, lines)
{
    const eol = isWindows ? "\r\n" : "\n";
    let res = "";
    lines.forEach(line => {
        res += line + eol;
    });
    return res;
}
