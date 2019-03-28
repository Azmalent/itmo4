using System;

using PathResult = System.Tuple<int[], short[]>;

namespace alg2
{
    internal partial struct Graph
    {
        public static PathResult FordBellman(Graph graph, int source)
        {
            int[] dist = graph.dist;
            short[] prev = graph.prev;

            for (short i = 0; i < graph.VertexCount; i++)
            {
                dist[i] = INFINITY;
                prev[i] = -1;
            }

            dist[source] = 0;

            bool shouldRetry;
            do
            {
                shouldRetry = false;
                for (short i = 0; i < graph.VertexCount; i++)
                {
                    foreach (Edge e in graph.Edges[i])
                    {
                        int j = e.Destination;
                        if (j != source && dist[j] > dist[i] + e.Weight)
                        {
                            dist[j] = dist[i] + e.Weight;
                            prev[j] = i;
                            shouldRetry = true;
                        }
                    }
                }
            } while (shouldRetry);

            return Tuple.Create(dist, prev);
        }
    }
}
