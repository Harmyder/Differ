using System.Collections.Generic;

namespace DifferLib.Highlight
{
    internal sealed class HighlightedLineFactory
    {
        private List<string> _blocks = new List<string>();

        public HighlightedLine Create() => new HighlightedLine(_blocks);

        public void AddHighlighted(string source, ref int start, int length)
        {
            if (_blocks.Count == 0)
            {
                _blocks.Add(string.Empty);
            }

            if (_blocks.Count % 2 == 0)
            {
                throw new System.InvalidProgramException($"Can't insert highlighted block. Blocks types must alternate. Current number of blocks is {_blocks.Count}.");
            }

            _blocks.Add(source.Substring(start, length));
            start += length;
        }

        public void Add(string source, ref int start, int length)
        {
            if (_blocks.Count % 2 == 1)
            {
                throw new System.InvalidProgramException($"Can't insert retained block. Blocks types must alternate. Current number of blocks is {_blocks.Count}.");
            }

            _blocks.Add(source.Substring(start, length));
            start += length;
        }
    }
}
