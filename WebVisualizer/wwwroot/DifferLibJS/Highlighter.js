class HighlightedLine
{
    get blocks() { return this._blocks; }
    get regular() { return this._blocks.filter((_, index) => index % 2 == 0); }
    get highlighted() { return this._blocks.filter((_, index) => index % 2 == 1); }

    get hasHighlighted() { return this._blocks.length > 1; }

    constructor(blocks)
    {
        if (blocks == null) throw "";
        this._blocks = blocks;
    }
}

class HighlightedLineFactory
{
    constructor()
    {
        this._blocks = new Array();
    }

    create() { return new HighlightedLine(this._blocks); }

    addHighlighted(source, start, length)
    {
        if (this._blocks.length == 0)
        {
            this._blocks.push("");
        }

        if (this._blocks.length % 2 == 0)
        {
            throw `Can't insert highlighted block. Blocks types must alternate. Current number of blocks is ${_blocks.length}.`;
        }

        this._blocks.push(source.substring(start, start + length));
        return start + length;
    }

    add(source, start, length)
    {
        if (this._blocks.length % 2 == 1)
        {
            throw `Can't insert retained block. Blocks types must alternate. Current number of blocks is ${_blocks.length}.`;
        }

        this._blocks.push(source.substring(start, start + length));
        return start + length;
    }
}

class Line
{
    get start() { return this._start; }
    get end() { return this._start + this._length; }
    get length() { return this._length; }

    constructor(start, length)
    {
        this._start = start;
        this._length = length;
    }
}

class Highlighter
{
    static highlight(before, after, deleteDescs, insertDescs)
    {
        if (before == null) throw "";
        if (after == null) throw "";
        if (deleteDescs == null) throw "";
        if (insertDescs == null) throw "";

        const highlightedBefore = Highlighter.computeLines(before, deleteDescs);
        const highlightedAfter = Highlighter.computeLines(after, insertDescs);

        const pairs = Highlighter.linesMatcher(highlightedBefore, highlightedAfter);
        return pairs;
    }

    static linesMatcher(before, after)
    {
        let pairs = new Array();

        let beforeIndex = 0;
        let afterIndex = 0;
        let beforeRetainedLineStart = 0;
        let afterRetainedLineStart = 0;

        while (beforeIndex < before.length || afterIndex < after.length)
        {
            while (Highlighter.isFullyHighlighted(before, beforeIndex))
            {
                let shouldGoInPair = false;
                if (Highlighter.isFullyHighlighted(after, afterIndex))
                {
                    shouldGoInPair = true;
                    afterIndex += 1;
                }

                pairs.push({
                    before: before[beforeIndex++],
                    after: shouldGoInPair ? after[afterIndex - 1] : null
                });
            }

            while (Highlighter.isFullyHighlighted(after, afterIndex))
            {
                pairs.push({
                    before: null,
                    after: after[afterIndex]
                });
                afterIndex += 1;
            }

            if (beforeRetainedLineStart != afterRetainedLineStart)
            {
                throw `After can't be different from before - ${afterRetainedLineStart} against ${beforeRetainedLineStart}`;
            }

            if (beforeIndex < before.length)
            {
                const retained = before[beforeIndex].regular.reduce((accum, block) => accum + block.length, 0);
                beforeRetainedLineStart += retained;

                if (afterIndex >= after.length)
                {
                    throw `There are not enough after lines. ${beforeIndex}:${before.length} - ${afterIndex}:${after.length}`;
                }

                afterRetainedLineStart += retained;
                pairs.push({
                    before: before[beforeIndex++],
                    after: after[afterIndex++]
                });
            }
            else
            {
                if (afterIndex < after.length)
                {
                    throw `There are not enough before lines. ${beforeIndex}:${before.length} - ${afterIndex}:${after.length}`;
                }
            }
        }

        return pairs;
    }

    static isFullyHighlighted(lines, index)
    {
        if (index < lines.length) {
            const regularLength = lines[index].regular.reduce((accum, block) => accum + block.length, 0);
            return regularLength == 0;
        }
        return false;
    }

    static computeLines(source, descs)
    {
        let sourceIndex = 0;
        let curDescIndex = 0;
        const lines = Highlighter.splitByLines(source);

        let highlightedLines = new Array(lines.length);

        for (let i = 0; i < lines.length; ++i)
        {
            const line = lines[i];

            const highlightedLineFactory = new HighlightedLineFactory();

            if (curDescIndex < descs.length && descs[curDescIndex].start < line.end)
            {
                do
                {
                    var isLeftover = descs[curDescIndex].start < line.start;
                    if (!isLeftover)
                    {
                        sourceIndex = highlightedLineFactory.add(source, sourceIndex, descs[curDescIndex].start - sourceIndex);
                    }

                    if (descs[curDescIndex].end <= line.end)
                    {
                        sourceIndex = highlightedLineFactory.addHighlighted(source, sourceIndex, descs[curDescIndex].end - sourceIndex);
                        curDescIndex += 1;
                    }
                    else
                    {
                        sourceIndex = highlightedLineFactory.addHighlighted(source, sourceIndex, line.end - sourceIndex);
                        break;
                    }
                }
                while (curDescIndex < descs.length && descs[curDescIndex].start < line.end);
            }

            if (line.end - sourceIndex != 0)
            {
                sourceIndex = highlightedLineFactory.add(source, sourceIndex, line.end - sourceIndex);
            }

            highlightedLines[i] = highlightedLineFactory.create();
        }

        return highlightedLines;
    }

    static splitByLines(input)
    {
        const ret = new Array();
        for (let i = 0; i < input.length;)
        {
            let nextLineStartIndex = input.indexOf('\n', i) + 1;
            if (nextLineStartIndex == 0) nextLineStartIndex = input.length;
            ret.push(new Line(i, nextLineStartIndex - i));
            i = nextLineStartIndex;
        }
        return ret;
    }
}
