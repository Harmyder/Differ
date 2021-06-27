using System.Diagnostics;

namespace DifferLib
{
    [DebuggerDisplay("Original:{StartOriginal}, Length:{Length}")]
    public struct DeleteOperation
    {
        public int StartOriginal { get; }
        public int Length { get; }

        public DeleteOperation(int startOriginal, int length)
        {
            StartOriginal = startOriginal;
            Length = length;
        }
    }
}
