namespace DifferLib
{
    public sealed class HighlighterSettings
    {
        public string LineStart { get; }
        public string LineEnd { get; }
        public string BlockStart { get; }
        public string BlockEnd { get; }

        public HighlighterSettings(
            string lineStart,
            string lineEnd,
            string blockStart,
            string blockEnd)
        {
            LineStart = lineStart;
            LineEnd = lineEnd;
            BlockStart = blockStart;
            BlockEnd = blockEnd;
        }

        public int GetTotalLength() => LineStart.Length + LineEnd.Length + BlockStart.Length + BlockEnd.Length;
    }
}
