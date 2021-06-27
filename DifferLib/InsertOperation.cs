using System.Diagnostics;

namespace DifferLib
{
    [DebuggerDisplay("Original:{StartOriginal}, New:{StartNew}, Length:{Length}")]
    public struct InsertOperation
    {
        public int StartOriginal { get; }
        public int StartNew { get; }
        public int Length { get; }

        public InsertOperation(int startOriginal, int startNew, int length)
        {
            StartOriginal = startOriginal;
            StartNew = startNew;
            Length = length;
        }
    }
}
