using System;
using System.Collections.Generic;
using System.Linq;

namespace DifferLib.Highlight
{
    public sealed class HighlightedLine
    {
        public List<string> Blocks;
        public IEnumerable<string> Regular => Blocks.Where((_, index) => index % 2 == 0);
        public IEnumerable<string> Highlighted => Blocks.Where((_, index) => index % 2 == 1);

        public bool HasHighlighted => Blocks.Count > 1;

        public HighlightedLine(List<string> blocks)
        {
            Blocks = blocks ?? throw new ArgumentNullException(nameof(blocks));
        }
    }
}
