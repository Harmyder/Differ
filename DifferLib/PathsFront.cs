namespace DifferLib
{
    internal class PathsFront
    {
        Path[] _paths;
        int _offset;

        public PathsFront(int x, int y)
        {
            _offset = x + y;
            _paths = new Path[2 * _offset + 1];
            for (int i = 0; i < _paths.Length; i++) _paths[i] = new Path();
        }

        public Path this[int i]
        {
            get => _paths[i + _offset];
            set => _paths[i + _offset] = value;
        }
    }
}
