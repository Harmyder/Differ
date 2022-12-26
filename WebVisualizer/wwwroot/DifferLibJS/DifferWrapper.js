class DifferWrapper {
    static compute(before, after, shouldPreferLines) {
        if (shouldPreferLines) {
            const [beforeLines, beforeEols] = split(before);
            const [afterLines, afterEols] = split(after);

            const [lineDeletes, lineInserts] = PrimitiveDiffer.compute(beforeLines, afterLines);

            const charDeletes = linesToChars(beforeLines, beforeEols, lineDeletes);
            const charInserts = linesToChars(afterLines, afterEols, lineInserts);

            let deletes = charDeletes.slice();
            let inserts = charInserts.slice();

            let linesDeleted = 0;
            let linesInserted = 0;
            let currLineDeleteIndex = 0;
            let currLineInsertIndex = 0;
            while (currLineDeleteIndex < lineDeletes.length && currLineInsertIndex < lineInserts.length) {
                var commonIndexFromDeletes = lineDeletes[currLineDeleteIndex].start + linesDeleted;
                var commonIndexFromInserts = lineInserts[currLineInsertIndex].start - linesInserted;

                if (commonIndexFromDeletes == commonIndexFromInserts) {
                    const from = before.substring(charDeletes[currLineDeleteIndex].start, charDeletes[currLineDeleteIndex].end);
                    const to = after.substring(charInserts[currLineInsertIndex].start, charInserts[currLineInsertIndex].end);
                    const [localDeletes, localInserts] = PrimitiveDiffer.compute(from, to);

                    const localDeletesWithOffset = localDeletes.map(x => new SubstringDescriptor(x.start + charDeletes[currLineDeleteIndex].start, x.end));
                    const localInsertsWithOffset = localInserts.map(x => new SubstringDescriptor(x.start + charInserts[currLineInsertIndex].start, x.end));

                    deletes.splice(currLineDeleteIndex, 1);
                    inserts.splice(currLineInsertIndex, 1);
                    deletes.splice(currLineDeleteIndex, 0, ...localDeletesWithOffset);
                    inserts.splice(currLineInsertIndex, 0, ...localInsertsWithOffset);
                }

                if (commonIndexFromDeletes <= commonIndexFromInserts) {
                    ++currLineDeleteIndex;
                }
                else {
                    ++currLineInsertIndex;
                }

                return [deletes, inserts];
            }
        }
        else {
            return PrimitiveDiffer.compute(before, after);
        }
    }
}

function split(text) {
    let lines = [];
    let eols = [];

    let prevLineEnd = 0;
    let isWindowsEol = false;

    for (let i = 0; i < text.length; ++i)
    {
        if (text[i] == '\n') {
            if (isWindowsEol) {
                eols.push(2);
                lines.push(text.substring(prevLineEnd, i - 1));
            }
            else {
                eols.push(1);
                lines.push(text.substring(prevLineEnd, i));
            }

            prevLineEnd = i + 1;
        }
        isWindowsEol = text[i] == '\r';
    }

    if (prevLineEnd != text.length) {
        eols.push(0);
        lines.push(text.substring(prevLineEnd));
    }

    return [lines, eols];
}

function linesToChars(lines, eols, descriptors)
{
    var beforeLinesIndices = new Array(lines.length + 1);
    beforeLinesIndices[0] = 0;
    for (var i = 0; i < lines.length; ++i) {
        beforeLinesIndices[i + 1] = beforeLinesIndices[i] + lines[i].length + eols[i];
    }
    //beforeLinesIndices[^1] -= eols[^1];

    let charDeletes = [];
    for (let i = 0; i < descriptors.length; ++i) {
        charDeletes.push(new SubstringDescriptor(
            beforeLinesIndices[descriptors[i].start],
            beforeLinesIndices[descriptors[i].end] - beforeLinesIndices[descriptors[i].start]));
    }

    return charDeletes;
}
