using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DifferLib.Diff
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Path
    {
        public int X { get; private set; }
        public List<StepId> Steps { get; private set; }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"X = {X}, {Steps.Count}:{string.Join("", Steps.Select(s => s.ToString().Substring(0,1)))}");
            }
        }

        public Path()
        {
            X = 0;
            Steps = new List<StepId>();
        }

        public Path(int x, List<StepId> steps)
        {
            X = x;
            Steps = steps;
        }

        public void AppendHorizontal()
        {
            X += 1;
            Steps.Add(StepId.Horizontal);
        }

        public void AppendVertical()
        {
            Steps.Add(StepId.Vertical);
        }

        public void AppendDiagonal(int length)
        {
            Steps.AddRange(Enumerable.Repeat(StepId.Diagonal, length));
            X += length;
        }
    }
}
