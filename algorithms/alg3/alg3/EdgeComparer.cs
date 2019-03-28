using System.Collections.Generic;

namespace alg3
{
    class EdgeComparer : IComparer<Edge>
    {
        public int Compare(Edge x, Edge y)
        {
            if (x.Weight > y.Weight) return 1;
            if (x.Weight < y.Weight) return -1;
            return 0;
        }
    }
}
