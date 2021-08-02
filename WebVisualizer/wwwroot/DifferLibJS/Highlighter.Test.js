function testEmptyDescriptors()
{
    const descsEmpty = new Array();
    const highlighted = Highlighter.highlight("_", "_", descsEmpty, descsEmpty);
    Assert.AreEqual(1, highlighted.length);
    Assert.AreEqual(new Array("_"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("_"), highlighted[0].after.blocks);
}

function testNoOperationsOnALineInTheMiddle()
{
    const del = new Array(new SubstringDescriptor(2, 1));
    const ins = new Array(new SubstringDescriptor(2, 1));
    const highlighted = Highlighter.highlight("_\n_", "_\n*", del, ins);
    Assert.AreEqual(2, highlighted.length);
    Assert.AreEqual(new Array("_\n"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("_\n"), highlighted[0].after.blocks);
    Assert.AreEqual(new Array("", "_"), highlighted[1].before.blocks);
    Assert.AreEqual(new Array("", "*"), highlighted[1].after.blocks);
}

function testLineEndings()
{
    const del = new Array(new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1));
    const ins = new Array(new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1));
    const highlighted = Highlighter.highlight("_\r\n_", "*\r\n*", del, ins);
    Assert.AreEqual(2, highlighted.length);
    Assert.AreEqual(new Array("", "_", "\r\n"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("", "*", "\r\n"), highlighted[0].after.blocks);
    Assert.AreEqual(new Array("", "_"), highlighted[1].before.blocks);
    Assert.AreEqual(new Array("", "*"), highlighted[1].after.blocks);
}

function testDifferentLineEndings()
{
    const del = new Array(new SubstringDescriptor(0, 1), new SubstringDescriptor(3, 1), new SubstringDescriptor(5, 1));
    const ins = new Array(new SubstringDescriptor(3, 1));
    const highlighted = Highlighter.highlight("_\r\n_\n_", "\r\n\n*", del, ins);
    Assert.AreEqual(3, highlighted.length);
    Assert.AreEqual(new Array("", "_", "\r\n"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("\r\n"),          highlighted[0].after.blocks);
    Assert.AreEqual(new Array("", "_", "\n"),   highlighted[1].before.blocks);
    Assert.AreEqual(new Array("\n"),            highlighted[1].after.blocks);
    Assert.AreEqual(new Array("", "_"),         highlighted[2].before.blocks);
    Assert.AreEqual(new Array("", "*"),         highlighted[2].after.blocks);
}

function testFullLineDelete()
{
    const highlighted = Highlighter.highlight("_", "", new Array(new SubstringDescriptor(0, 1)), new Array());
    Assert.AreEqual(1, highlighted.length);
    Assert.AreEqual(new Array("", "_"), highlighted[0].before.blocks);
    Assert.AreEqual(null, highlighted[0].After);
}

function testInterlineDelete()
{
    const del = new Array(new SubstringDescriptor(2, 3));
    const ins = new Array();
    const highlighted = Highlighter.highlight("_\n.\n_*", "_\n*", del, ins);
    Assert.AreEqual(3, highlighted.length);
    Assert.AreEqual(new Array("_\n"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("_\n"), highlighted[0].after.blocks);
    Assert.AreEqual(new Array("", ".\n"), highlighted[1].before.blocks);
    Assert.AreEqual(null, highlighted[1].After);
    Assert.AreEqual(new Array("", "_", "*"), highlighted[2].before.blocks);
    Assert.AreEqual(new Array("*"), highlighted[2].after.blocks);
}

function testdelFirstLine()
{
    const del = new Array(new SubstringDescriptor(0, 1));
    const ins = new Array();
    const highlighted = Highlighter.highlight("\n_", "_", del, ins);
    Assert.AreEqual(2, highlighted.length);
    Assert.AreEqual(new Array("", "\n"), highlighted[0].before.blocks);
    Assert.AreEqual(null, highlighted[0].After);
    Assert.AreEqual(new Array("_"), highlighted[1].before.blocks);
    Assert.AreEqual(new Array("_"), highlighted[1].after.blocks);
}

function testChangeFullLine()
{
    const del = new Array(new SubstringDescriptor(0, 1));
    const ins = new Array(new SubstringDescriptor(0, 1));
    const highlighted = Highlighter.highlight("_\n_", "*\n_", del, ins);
    Assert.AreEqual(2, highlighted.length);
    Assert.AreEqual(new Array("", "_\n"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("", "*\n"), highlighted[0].after.blocks);
    Assert.AreEqual(new Array("_"), highlighted[1].before.blocks);
    Assert.AreEqual(new Array("_"), highlighted[1].after.blocks);
}

function testChangeFullLastLine()
{
    const del = new Array(new SubstringDescriptor(2, 1));
    const ins = new Array(new SubstringDescriptor(2, 1));
    const highlighted = Highlighter.highlight("_\n_", "_\n*", del, ins);
    Assert.AreEqual(2, highlighted.length);
    Assert.AreEqual(new Array("_\n"), highlighted[0].before.blocks);
    Assert.AreEqual(new Array("_\n"), highlighted[0].after.blocks);
    Assert.AreEqual(new Array("", "_"), highlighted[1].before.blocks);
    Assert.AreEqual(new Array("", "*"), highlighted[1].after.blocks);
}

function testInsertLine()
{
    const del = new Array();
    const ins = new Array(new SubstringDescriptor(0, 2));
    const highlighted = Highlighter.highlight("_\n", "*\n_\n", del, ins);
    Assert.AreEqual(2, highlighted.length);
    Assert.AreEqual(null, highlighted[0].Before);
    Assert.AreEqual(new Array("", "*\n"), highlighted[0].after.blocks);
    Assert.AreEqual(new Array("_\n"), highlighted[1].before.blocks);
    Assert.AreEqual(new Array("_\n"), highlighted[1].after.blocks);
}
