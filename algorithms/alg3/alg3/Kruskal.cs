using System;

namespace alg3
{
    internal partial struct Graph
    {
        public static Edge[] Kruskal(Graph graph)
        {
            var mst = new Edge[graph.VertexCount - 1];
            var set = new DisjointSet(graph.VertexCount);

            for (int v = 0; v < graph.VertexCount; v++)
                set.MakeSet(v);

            var edges = graph.Edges;
            var comparer = new EdgeComparer();
            Array.Sort(edges, comparer);

            int mstIndex = 0;
            for (int i = 0; i < edges.Length; i++)
            {
                if (set.Find(edges[i].U) == set.Find(edges[i].V)) continue;

                mst[mstIndex] = edges[i];
                set.Union(edges[i].U, edges[i].V);
                mstIndex++;
            }

            return mst;
        }
    }
}
