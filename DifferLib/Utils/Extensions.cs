using System.Text;

namespace DifferLib.Utils
{
    internal static class Extensions
    {
        public static void AppendWithAccounting(this StringBuilder builder, string source, ref int start, int length)
        {
            builder.Append(source.Substring(start, length));
            start += length;
        }
    }
}
