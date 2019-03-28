namespace alg3
{
    internal struct DisjointSet
    {
        private readonly int[] parent;
        private readonly int[] rank;

        public DisjointSet(int n)
        {
            parent = new int[n];
            rank   = new int[n];
        }

        public void MakeSet(int x)
        {
            parent[x] = x;
            rank[x]   = 0;
        }

        public int Find(int x)
        {
            if (x != parent[x]) parent[x] = Find(parent[x]);
            return parent[x];
        }

        public void Union(int xInit, int yInit)
        {
            int x = Find(xInit);
            int y = Find(yInit);
            if (x == y) return;

            if (rank[x] < rank[y]) parent[x] = y;
            else
            {
                parent[y] = x;
                if (rank[x] == rank[y]) rank[x]++;
            }
        }
    }
}