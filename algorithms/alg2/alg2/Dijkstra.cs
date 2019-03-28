using System;

using PathResult = System.Tuple<int[], short[]>;

namespace alg2
{
    internal partial struct Graph
    {
        public static PathResult Dijkstra(Graph graph, int source)
        {
            int[]   dist = graph.dist;
            short[] prev = graph.prev;

            for (short i = 0; i < graph.VertexCount; i++)
            {
                dist[i] = INFINITY;
                prev[i] = -1;
            }

            var heap = new DHeap(graph.VertexCount);
            heap.Key[source] = 0;
            heap.Heapify();

            while (!heap.IsEmpty)
            {
                heap.DeleteMin();
                short i = heap.Name[heap.N];
                dist[i] = heap.Key[heap.N];

                foreach (Edge e in graph.Edges[i])
                {
                    int j = e.Destination;
                    if (dist[j] >= INFINITY)
                    {
                        int jq = heap.Index[j];
                        if (heap.Key[jq] > dist[i] + e.Weight)
                        {
                            heap.Key[jq] = dist[i] + e.Weight;
                            heap.SiftUp(jq);
                            prev[j] = i;
                        }
                    }
                }
            }

            return Tuple.Create(dist, prev);
        }
    }
}
