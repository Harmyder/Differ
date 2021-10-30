using DifferLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebVisualizer.Content
{
    public class IndexModel : PageModel
    {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";

        public (List<string> Before, List<string> After)[] Highlighted = new (List<string> Before, List<string> After)[] { };

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Before = Request.Cookies["Before"];
            After = Request.Cookies["After"];
        }

        public void OnPost([FromForm] bool shouldPreferLines, [FromForm] string before, [FromForm] string after)
        {
            Before = before;
            After = after;

            Response.Cookies.Append("Before", Before);
            Response.Cookies.Append("After", After);

            List<SubstringDescriptor> deletes;
            List<SubstringDescriptor> inserts;
            if (shouldPreferLines)
            {
                var (beforeLines, beforeEols) = Split(Before);
                var (afterLines, afterEols) = Split(After);

                var differ = new PrimitiveDiffer<string>(beforeLines, afterLines);
                var (lineDeletes, lineInserts) = differ.Compute();

                var charDeletes = LinesToChars(beforeLines, beforeEols, lineDeletes);
                var charInserts = LinesToChars(afterLines, afterEols, lineInserts);

                deletes = new List<SubstringDescriptor>(charDeletes);
                inserts = new List<SubstringDescriptor>(charInserts);

                var linesDeleted = 0;
                var linesInserted = 0;
                var currLineDeleteIndex = 0;
                var currLineInsertIndex = 0;
                while (currLineDeleteIndex < lineDeletes.Count && currLineInsertIndex < lineInserts.Count)
                {
                    var commonIndexFromDeletes = lineDeletes[currLineDeleteIndex].Start + linesDeleted;
                    var commonIndexFromInserts = lineInserts[currLineInsertIndex].Start - linesInserted;

                    if (commonIndexFromDeletes == commonIndexFromInserts)
                    {
                        var from = Before.Substring(charDeletes[currLineDeleteIndex].Start, charDeletes[currLineDeleteIndex].Length);
                        var to = After.Substring(charInserts[currLineInsertIndex].Start, charInserts[currLineInsertIndex].Length);
                        var localDiffer = new PrimitiveDiffer<char>(from.ToCharArray(), to.ToCharArray());
                        var (localDeletes, localInserts) = localDiffer.Compute();

                        var localDeletesWithOffset = localDeletes.Select(x => new SubstringDescriptor(x.Start + charDeletes[currLineDeleteIndex].Start, x.Length)).ToList();
                        var localInsertsWithOffset = localInserts.Select(x => new SubstringDescriptor(x.Start + charInserts[currLineInsertIndex].Start, x.Length)).ToList();

                        deletes.RemoveAt(currLineDeleteIndex);
                        inserts.RemoveAt(currLineInsertIndex);
                        deletes.InsertRange(currLineDeleteIndex, localDeletesWithOffset);
                        inserts.InsertRange(currLineInsertIndex, localInsertsWithOffset);
                    }

                    if (commonIndexFromDeletes <= commonIndexFromInserts)
                    {
                        ++currLineDeleteIndex;
                    }
                    else
                    {
                        ++currLineInsertIndex;
                    }
                }
            }
            else
            {
                var differ = new PrimitiveDiffer<char>(Before.ToCharArray(), After.ToCharArray());
                (deletes, inserts) = differ.Compute();
            }

            var highlighted = Highlighter.Highlight(Before, After, deletes, inserts);

            Highlighted = highlighted.Select(h => (h.Before?.Blocks, h.After?.Blocks)).ToArray();
        }

        private (string[] Lines, int[] Eols) Split(string text)
        {
            var lines = new List<string>();
            var eols = new List<int>();

            var prevLineEnd = 0;
            var isWindowsEol = false;

            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == '\n')
                {
                    if (isWindowsEol)
                    {
                        eols.Add(2);
                        lines.Add(text.Substring(prevLineEnd, i - 1 - prevLineEnd));
                    }
                    else
                    {
                        eols.Add(1);
                        lines.Add(text.Substring(prevLineEnd, i - prevLineEnd));
                    }

                    prevLineEnd = i + 1;
                }
                isWindowsEol = text[i] == '\r';
            }

            if (prevLineEnd != text.Length)
            {
                eols.Add(0);
                lines.Add(text.Substring(prevLineEnd));
            }

            return (lines.ToArray(), eols.ToArray());
        }

        private List<SubstringDescriptor> LinesToChars(string[] lines, int[] eols, List<SubstringDescriptor> descriptors)
        {
            var beforeLinesIndices = new int[lines.Length + 1];
            for (var i = 0; i < lines.Length; ++i)
            {
                beforeLinesIndices[i + 1] = beforeLinesIndices[i] + lines[i].Length + eols[i];
            }
            //beforeLinesIndices[^1] -= eols[^1];

            var charDeletes = new List<SubstringDescriptor>();
            for (var i = 0; i < descriptors.Count; ++i)
            {
                charDeletes.Add(new SubstringDescriptor(
                    beforeLinesIndices[descriptors[i].Start],
                    beforeLinesIndices[descriptors[i].End] - beforeLinesIndices[descriptors[i].Start]));
            }

            return charDeletes;
        }
    }
}
