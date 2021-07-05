using DifferLib.Utils;
using System.Collections.Generic;
using System.Text;

namespace DifferLib
{
    public sealed class Highlighter
    {
        private readonly HighlighterSettings _deleteSettings;
        private readonly HighlighterSettings _insertSettings;

        public Highlighter(HighlighterSettings deleteSettings, HighlighterSettings insertSettings)
        {
            _deleteSettings = deleteSettings;
            _insertSettings = insertSettings;
        }

        public string HighlightDelete(string source, List<SubstringDescriptor> descriptors) => HighlightInternal(source, descriptors, _deleteSettings);
        public string HighlightInsert(string source, List<SubstringDescriptor> descriptors) => HighlightInternal(source, descriptors, _insertSettings);


        public static string HighlightInternal(string source, List<SubstringDescriptor> descriptors, HighlighterSettings settings)
        {
            var lines = SplitByLines(source);
            var builder = new StringBuilder(source.Length + descriptors.Count * (settings.GetTotalLength()));
            var sourceIndex = 0;
            var curDeleteIndex = 0;
            foreach (var line in lines)
            {
                var lineEndIndex = line.Start + line.Length;
                if (curDeleteIndex < descriptors.Count && descriptors[curDeleteIndex].Start < lineEndIndex)
                {
                    builder.Append(settings.LineStart);

                    do
                    {
                        var isLeftover = descriptors[curDeleteIndex].Start < line.Start;
                        if (!isLeftover)
                        {
                            builder.AppendWithAccounting(source, ref sourceIndex, descriptors[curDeleteIndex].Start - sourceIndex);
                        }

                        builder.Append(settings.BlockStart);

                        if (descriptors[curDeleteIndex].End <= lineEndIndex)
                        {
                            builder.AppendWithAccounting(source, ref sourceIndex, descriptors[curDeleteIndex].End - sourceIndex);
                            builder.Append(settings.BlockEnd);
                            curDeleteIndex += 1;
                        }
                        else
                        {
                            builder.AppendWithAccounting(source, ref sourceIndex, lineEndIndex - sourceIndex);
                            builder.Append(settings.BlockEnd);
                            break;
                        }
                    }
                    while (curDeleteIndex < descriptors.Count && descriptors[curDeleteIndex].Start < lineEndIndex);

                    builder.AppendWithAccounting(source, ref sourceIndex, lineEndIndex - sourceIndex);

                    builder.Append(settings.LineEnd);
                }
                else
                {
                    builder.AppendWithAccounting(source, ref sourceIndex, lineEndIndex - sourceIndex);
                }
            }

            return builder.ToString();
        }

        private static IReadOnlyList<(int Start, int Length)> SplitByLines(string input)
        {
            var ret = new List<(int Start, int Length)>();
            for (int i = 0; i < input.Length;)
            {
                var nextLineStartIndex = input.IndexOf('\n', i) + 1;
                if (nextLineStartIndex == 0) nextLineStartIndex = input.Length;
                ret.Add((i, nextLineStartIndex - i));
                i = nextLineStartIndex;
            }
            return ret;
        }
    }
}
