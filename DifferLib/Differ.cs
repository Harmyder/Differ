using System;
using System.Collections.Generic;

namespace DifferLib
{
    public sealed class Differ<T>
    {
        private T[] TextFrom { get; }
        private T[] TextTo { get; }

        public Differ(T[] textFrom, T[] textTo)
        {
            TextFrom = textFrom;
            TextTo = textTo;
        }

        public (List<SubstringDescriptor> Deletes, List<SubstringDescriptor> Inserts) Compute()
        {
            var steps = ComputeInternal();

            var deletes = new List<SubstringDescriptor>();
            var inserts = new List<SubstringDescriptor>();

            var curX = 0;
            var curY = 0;

            for (var i = 0; i < steps.Count;)
            {
                var span = 0;
                switch (steps[i])
                {
                    case StepId.Diagonal:
                        ++curX;
                        ++curY;
                        ++span;
                        break;
                    case StepId.Horizontal:
                        while (steps.Count > i + ++span && steps[i + span] == StepId.Horizontal) ;
                        deletes.Add(new SubstringDescriptor(curX, span));
                        curX += span;
                        break;
                    case StepId.Vertical:
                        while (steps.Count > i + ++span && steps[i + span] == StepId.Vertical) ;
                        inserts.Add(new SubstringDescriptor(curY, span));
                        curY += span;
                        break;
                }

                i += span;
            }

            return (deletes, inserts);
        }

        private int CalculateSnakeLength(int x, int diagonal)
        {
            var snakeLength = 0;
            var y = x - diagonal;
            while (0 <= x && x < TextFrom.Length
                && 0 <= y && y < TextTo.Length
                && EqualityComparer<T>.Default.Equals(TextFrom[x], TextTo[y]))
            {
                ++snakeLength;
                ++x;
                ++y;
            }

            return snakeLength;
        }

        private IReadOnlyList<StepId> ComputeInternal()
        {
            var paths = new PathsFront(TextFrom.Length, TextTo.Length);

            var prefixSnakeLength = CalculateSnakeLength(0, 0);
            paths[0].AppendDiagonal(prefixSnakeLength);

            if (IsTheEnd(paths[0].X, 0))
            {
                return paths[0].Steps;
            }

            for (int nonDiagonalCount = 1; nonDiagonalCount <= TextFrom.Length + TextTo.Length; nonDiagonalCount++)
            {
                var minDiagonalIndex = -nonDiagonalCount;
                var maxDiagonalIndex = nonDiagonalCount;

                for (int curDiagonal = minDiagonalIndex; curDiagonal <= maxDiagonalIndex; curDiagonal += 2)
                {
                    Path curDiagPath;
                    // If both neighboring paths have the same X, then the -1 one has more diagonal elements
                    if (curDiagonal != minDiagonalIndex && (curDiagonal == maxDiagonalIndex || paths[curDiagonal - 1].X >= paths[curDiagonal + 1].X))
                    {
                        curDiagPath = paths[curDiagonal - 1];
                        curDiagPath.AppendHorizontal();
                    }
                    else
                    {
                        curDiagPath = new Path(paths[curDiagonal + 1].X, new List<StepId>(paths[curDiagonal + 1].Steps));
                        curDiagPath.AppendVertical();
                    }

                    if (curDiagonal != minDiagonalIndex)
                    {
                        paths[curDiagonal - 1] = new Path();
                    }

                    var snakeLength = CalculateSnakeLength(curDiagPath.X, curDiagonal);
                    curDiagPath.AppendDiagonal(snakeLength);

                    if (IsTheEnd(curDiagPath.X, curDiagonal))
                    {
                        return curDiagPath.Steps;
                    }

                    paths[curDiagonal] = curDiagPath;
                }
            }

            throw new InvalidProgramException();
        }

        private bool IsTheEnd(int x, int diagonal)
        {
            return x == TextFrom.Length && x - diagonal == TextTo.Length;
        }
    }
}
