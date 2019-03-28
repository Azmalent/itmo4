using System.Collections.Generic;

namespace alg2
{
    internal partial struct Graph
    {
        public const int INFINITY = 0x3f3f3f3f;

        public readonly int VertexCount;

        public readonly List<Edge>[] Edges;

        private readonly int[] dist;
        private readonly short[] prev;

        private Graph(int vertices)
        {
            VertexCount = vertices;
            Edges = new List<Edge>[vertices];
            for (int i = 0; i < vertices; i++) Edges[i] = new List<Edge>();

            dist = new int[vertices];
            prev = new short[vertices];
        }

        public static Graph Generate(int vertices, int edges, int maxWeight)
        {
            var matrix = new int[vertices, vertices];
            var graph = new Graph(vertices);

            for (int i = 0; i < edges; i++)
            {
                int weight = RNG.Random(1, maxWeight);

                int vertexA, vertexB;
                do
                {
                    vertexA = RNG.Random(0, vertices - 1);
                    vertexB = RNG.Random(0, vertices - 1);
                } while (vertexA == vertexB || matrix[vertexA, vertexB] > 0);

                matrix[vertexA, vertexB] = weight;

                graph.Edges[vertexA].Insert(0, new Edge(vertexB, weight));
            }

            return graph;
        }
    }
}