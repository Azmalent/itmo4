using System.Diagnostics;
using System.Linq;

namespace alg3
{
    internal partial struct Graph
    {
        private AdjacencyMatrix matrix;
        public int VertexCount { get; private set; }
        public int EdgeCount { get; private set; }


        private Graph(int vertices, int edges)
        {
            matrix = new AdjacencyMatrix(vertices);
            VertexCount = vertices;
            EdgeCount = edges;
        }

        public static Graph GenerateTree(int vertices, int maxWeight)
        {
            var graph = new Graph(vertices, vertices - 1);

            int[] vertexSequence = Enumerable.Range(0, vertices)
                                             .OrderBy(n => RNG.Random())
                                             .ToArray();

            for (int i = 1; i < vertices; i++)
            {
                int a = vertexSequence[i - 1];
                int b = vertexSequence[i];

                int weight = RNG.Random(1, maxWeight);

                graph.Connect(a, b, weight);
            }

            return graph;
        }

        public static Graph Generate(int vertices, int edges, int maxWeight)
        {
            Debug.Assert(edges >= vertices - 1);
            Debug.Assert(edges <= vertices * (vertices - 1) / 2);

            var graph = GenerateTree(vertices, maxWeight);
            edges = edges - graph.EdgeCount;

            for (int i = 0; i < edges; i++)
            {
                int weight = RNG.Random(1, maxWeight);

                int vertexA, vertexB;
                do
                {
                    vertexA = RNG.Random(0, vertices - 1);
                    vertexB = RNG.Random(0, vertices - 1);
                } while (vertexA == vertexB || graph.HasEdge(vertexA, vertexB));

                graph.Connect(vertexA, vertexB, weight);
            }

            return graph;
        }

        public Edge[] Edges
        {
            get {
                var edges = new Edge[EdgeCount];
                int k = 0;
                for (int i = 0; i < VertexCount - 1; i++)
                {
                    for (int j = i + 1; j < VertexCount; j++)
                    {
                        if (!HasEdge(i, j)) continue;

                        edges[k] = new Edge(i, j, matrix[i, j]);
                        k++;
                    }
                }

                return edges;
            }
        }

        public bool HasEdge(int a, int b)
        {
            return matrix[a, b] > 0;
        }

        private void Connect(int a, int b, int weight)
        {
            Debug.Assert(a != b);
            Debug.Assert(weight > 0);

            EdgeCount++;
            matrix[a, b] = weight;
        }
    }
}