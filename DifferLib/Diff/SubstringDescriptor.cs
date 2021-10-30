using System.Diagnostics;

namespace DifferLib.Diff
{
    [DebuggerDisplay("Start:{Start}, Length:{Length}")]
    public struct SubstringDescriptor
    {
        public int Start { get; }
        public int End => Start + Length;
        public int Length { get; }

        public SubstringDescriptor(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}
