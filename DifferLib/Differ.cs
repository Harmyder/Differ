﻿using System;
using System.Collections.Generic;

namespace DifferLib
{
    public sealed class Differ
    {
        private string TextFrom { get; }
        private string TextTo { get; }

        public Differ(string textFrom, string textTo)
        {
            TextFrom = textFrom;
            TextTo = textTo;
        }

        public (List<DeleteOperation> Deletes, List<InsertOperation> Inserts) Compute()
        {
            var steps = ComputeInternal();

            var deletes = new List<DeleteOperation>();
            var inserts = new List<InsertOperation>();

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
                        deletes.Add(new DeleteOperation(curX, span));
                        curX += span;
                        break;
                    case StepId.Vertical:
                        while (steps.Count > i + ++span && steps[i + span] == StepId.Vertical) ;
                        inserts.Add(new InsertOperation(curX, curY, span));
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
                && TextFrom[x] == TextTo[y])
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
                //var minDiagonalIndex = Math.Max(-nonDiagonalCount, -TextTo.Length + (nonDiagonalCount - TextTo.Length) % 2);
                //var maxDiagonalIndex = Math.Min(nonDiagonalCount, TextFrom.Length - (nonDiagonalCount - TextFrom.Length) % 2);
                var minDiagonalIndex = -nonDiagonalCount;
                var maxDiagonalIndex = nonDiagonalCount;

                for (int curDiagonal = minDiagonalIndex; curDiagonal <= maxDiagonalIndex; curDiagonal += 2)
                {
                    Path curDiagPath;
                    if (curDiagonal != minDiagonalIndex && (curDiagonal == maxDiagonalIndex || paths[curDiagonal - 1].X > paths[curDiagonal + 1].X))
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
