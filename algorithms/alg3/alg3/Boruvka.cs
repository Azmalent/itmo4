namespace alg3
{
    internal partial struct Graph
    {
        public static Edge[] Boruvka(Graph graph)
        {
            var mst = new Edge[graph.VertexCount - 1];
            var set = new DisjointSet(graph.VertexCount);
            var cheapest = new int[graph.VertexCount];

            for (int v = 0; v < graph.VertexCount; v++)
                set.MakeSet(v);

            var edges = graph.Edges;

            int treeCount = graph.VertexCount;
            int mstIndex = 0;
            while (treeCount > 1)
            {
                for (int v = 0; v < graph.VertexCount; v++)
                    cheapest[v] = -1;

                for (int e = 0; e < edges.Length; e++)
                {
                    int i = set.Find(edges[e].U);
                    int j = set.Find(edges[e].V);
                    if (i == j) continue;

                    if (cheapest[i] == -1 || edges[cheapest[i]].Weight > edges[e].Weight)
                        cheapest[i] = e;

                    if (cheapest[j] == -1 || edges[cheapest[j]].Weight > edges[e].Weight)
                        cheapest[j] = e;
                }

                for (int v = 0; v < graph.VertexCount; v++)
                {
                    if (cheapest[v] != -1)
                    {
                        int i = set.Find(edges[cheapest[v]].U);
                        int j = set.Find(edges[cheapest[v]].V);
                        if (i == j) continue;

                        mst[mstIndex] = edges[cheapest[v]];
                        set.Union(i, j);
                        mstIndex++;
                        treeCount--;
                    }
                }
            }

            return mst;
        }
    }
}
