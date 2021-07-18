using System;
using System.Collections.Generic;
using System.Linq;

namespace DifferLib
{
    public static class Highlighter
    {
        public static List<(HighlightedLine Before, HighlightedLine After)> Highlight(string before, string after, List<SubstringDescriptor> deleteDescs, List<SubstringDescriptor> insertDescs)
        {
            if (before == null) throw new ArgumentNullException(nameof(before));
            if (after == null) throw new ArgumentNullException(nameof(after));
            if (deleteDescs == null) throw new ArgumentNullException(nameof(deleteDescs));
            if (insertDescs == null) throw new ArgumentNullException(nameof(insertDescs));

            var highlightedBefore = ComputeLines(before, deleteDescs);
            var highlightedAfter = ComputeLines(after, insertDescs);

            var pairs = LinesMatcher(highlightedBefore, highlightedAfter);
            return pairs;
        }

        private static List<(HighlightedLine Before, HighlightedLine After)> LinesMatcher(HighlightedLine[] before, HighlightedLine[] after)
        {
            var pairs = new List<(HighlightedLine Before, HighlightedLine After)>();

            var beforeIndex = 0;
            var afterIndex = 0;
            var beforeRetainedLineStart = 0;
            var afterRetainedLineStart = 0;

            while (beforeIndex < before.Length || afterIndex < after.Length)
            {
                while (IsFullyHighlighted(before, beforeIndex))
                {
                    var shouldGoInPair = false;
                    if (IsFullyHighlighted(after, afterIndex))
                    {
                        shouldGoInPair = true;
                        afterIndex += 1;
                    }

                    pairs.Add((before[beforeIndex++], shouldGoInPair ? after[afterIndex - 1] : null));
                }

                while (IsFullyHighlighted(after, afterIndex))
                {
                    pairs.Add((null, after[afterIndex]));
                    afterIndex += 1;
                }

                if (beforeRetainedLineStart != afterRetainedLineStart)
                {
                    throw new InvalidProgramException($"After can't be different from before - {afterRetainedLineStart} against {beforeRetainedLineStart}");
                }

                if (beforeIndex < before.Length)
                {
                    var retained = before[beforeIndex].Regular.Sum(b => b.Length);
                    beforeRetainedLineStart += retained;

                    if (afterIndex >= after.Length)
                    {
                        throw new InvalidProgramException($"There are not enough after lines. {beforeIndex}:{before.Length} - {afterIndex}:{after.Length}");
                    }

                    afterRetainedLineStart += retained;
                    pairs.Add((before[beforeIndex++], after[afterIndex++]));
                }
                else
                {
                    if (afterIndex < after.Length)
                    {
                        throw new InvalidProgramException($"There are not enough before lines. {beforeIndex}:{before.Length} - {afterIndex}:{after.Length}");
                    }
                }
            }

            return pairs;
        }

        private static bool IsFullyHighlighted(HighlightedLine[] lines, int index)
        {
            return index < lines.Length && lines[index].Regular.Sum(b => b.Length) == 0;
        }

        private static HighlightedLine[] ComputeLines(
            string source,
            List<SubstringDescriptor> descs)
        {
            var sourceIndex = 0;
            var curDescIndex = 0;
            var lines = SplitByLines(source);

            var highlightedLines = new HighlightedLine[lines.Count];

            for (int i = 0; i < lines.Count; ++i)
            {
                var line = lines[i];

                var highlightedLineFactory = new HighlightedLineFactory();

                if (curDescIndex < descs.Count && descs[curDescIndex].Start < line.End)
                {
                    do
                    {
                        var isLeftover = descs[curDescIndex].Start < line.Start;
                        if (!isLeftover)
                        {
                            highlightedLineFactory.Add(source, ref sourceIndex, descs[curDescIndex].Start - sourceIndex);
                        }

                        if (descs[curDescIndex].End <= line.End)
                        {
                            highlightedLineFactory.AddHighlighted(source, ref sourceIndex, descs[curDescIndex].End - sourceIndex);
                            curDescIndex += 1;
                        }
                        else
                        {
                            highlightedLineFactory.AddHighlighted(source, ref sourceIndex, line.End - sourceIndex);
                            break;
                        }
                    }
                    while (curDescIndex < descs.Count && descs[curDescIndex].Start < line.End);
                }

                if (line.End - sourceIndex != 0)
                {
                    highlightedLineFactory.Add(source, ref sourceIndex, line.End - sourceIndex);
                }

                highlightedLines[i] = highlightedLineFactory.Create();
            }

            return highlightedLines;
        }

        private static IReadOnlyList<Line> SplitByLines(string input)
        {
            var ret = new List<Line>();
            for (int i = 0; i < input.Length;)
            {
                var nextLineStartIndex = input.IndexOf('\n', i) + 1;
                if (nextLineStartIndex == 0) nextLineStartIndex = input.Length;
                ret.Add(new Line(i, nextLineStartIndex - i));
                i = nextLineStartIndex;
            }
            return ret;
        }

        private struct Line
        {
            public int Start { get; }
            public int End => Start + Length;
            public int Length { get; }
            public Line(int start, int length)
            {
                Start = start;
                Length = length;
            }
        }
    }
}
