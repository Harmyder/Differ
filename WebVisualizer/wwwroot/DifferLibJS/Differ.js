class SubstringDescriptor
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

class Differ {
    static compute(textFrom, textTo)
    {
        const steps = computeInternal(textFrom, textTo);

        const deletes = new Array();
        const inserts = new Array();

        let curX = 0;
        let curY = 0;

        for (let i = 0; i < steps.length;)
        {
            let span = 0;
            switch (steps[i])
            {
                case StepId.Diagonal:
                    ++curX;
                    ++curY;
                    ++span;
                    break;
                case StepId.Horizontal:
                    while (steps.length > i + ++span && steps[i + span] == StepId.Horizontal) ;
                    deletes.push(new SubstringDescriptor(curX, span));
                    curX += span;
                    break;
                case StepId.Vertical:
                    while (steps.length > i + ++span && steps[i + span] == StepId.Vertical) ;
                    inserts.push(new SubstringDescriptor(curY, span));
                    curY += span;
                    break;
            }

            i += span;
        }

        return [deletes, inserts];
    }
}

const StepId = {
    None: "none",
    Horizontal: "horizontal",
    Diagonal: "diagonal",
    Vertical: "vertical",
}

class Path
{
    constructor(x, steps)
    {
        this._x = x ?? 0;
        this._steps = steps ?? new Array();
    }

    get x()
    {
        return this._x;
    }

    get steps()
    {
        return this._steps;
    }

    appendHorizontal()
    {
        this._x += 1;
        this._steps.push(StepId.Horizontal);
    }

    appendVertical()
    {
        this._steps.push(StepId.Vertical);
    }

    appendDiagonal(length)
    {
        for (let i = 0; i < length; ++i)
        {
            this._steps.push(StepId.Diagonal);
        }
        this._x += length;
    }
}

class PathsFront
{
    constructor(x, y)
    {
        this.offset = x + y;
        this.paths = Array(2 * this.offset + 1);
        for (let i = 0; i < this.paths.length; i++) this.paths[i] = new Path();

    }

    getPath(i)
    {
        return this.paths[i + this.offset];
    }

    setPath(i, path) {
        this.paths[i + this.offset] = path;
    }
}

function computeInternal(textFrom, textTo)
{
    const paths = new PathsFront(textFrom.length, textTo.length);

    const prefixSnakeLength = calculateSnakeLength(0, 0);
    paths.getPath(0).appendDiagonal(prefixSnakeLength);

    if (isTheEnd(paths.getPath(0).x, 0))
    {
        return paths.getPath(0).steps;
    }

    for (let nonDiagonalCount = 1; nonDiagonalCount <= textFrom.length + textTo.length; nonDiagonalCount++)
    {
        let minDiagonalIndex = -nonDiagonalCount;
        let maxDiagonalIndex = nonDiagonalCount;

        for (let curDiagonal = minDiagonalIndex; curDiagonal <= maxDiagonalIndex; curDiagonal += 2)
        {
            let curDiagPath = new Path();
            if (curDiagonal != minDiagonalIndex && (curDiagonal == maxDiagonalIndex || paths.getPath(curDiagonal - 1).x >= paths.getPath(curDiagonal + 1).x))
            {
                curDiagPath = paths.getPath(curDiagonal - 1);
                curDiagPath.appendHorizontal();
            }
            else
            {
                curDiagPath = new Path(paths.getPath(curDiagonal + 1).x, paths.getPath(curDiagonal + 1).steps.slice());
                curDiagPath.appendVertical();
            }

            if (curDiagonal != minDiagonalIndex)
            {
                paths.setPath(curDiagonal - 1, new Path());
            }

            const snakeLength = calculateSnakeLength(curDiagPath.x, curDiagonal);
            curDiagPath.appendDiagonal(snakeLength);

            if (isTheEnd(curDiagPath.x, curDiagonal))
            {
                return curDiagPath.steps;
            }

            paths.setPath(curDiagonal, curDiagPath);
        }
    }

    throw "";

    function isTheEnd(x, diagonal)
    {
        return x == textFrom.length && x - diagonal == textTo.length;
    }

    function calculateSnakeLength(x, diagonal)
    {
        let snakeLength = 0;
        let y = x - diagonal;
        while (0 <= x && x < textFrom.length
            && 0 <= y && y < textTo.length
            && textFrom[x] == textTo[y])
        {
            ++snakeLength;
            ++x;
            ++y;
        }

        return snakeLength;
    }
}
