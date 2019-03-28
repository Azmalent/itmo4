using System;
using NUnit.Framework;

namespace alg2
{
    internal class UnitTests
    {
        [Test, TestCase(arg1: 25, arg2: 100, arg3: 10000)]
        public void PathfindingTest(int vertices, int edges, int maxWeight)
        {
            var graph = Graph.Generate(vertices, edges, maxWeight);

            var dijkstra = Graph.Dijkstra(graph, 1);

            var fordBellman = Graph.FordBellman(graph, 1);

            CollectionAssert.AreEqual(dijkstra.Item1, fordBellman.Item1);
            CollectionAssert.AreEqual(dijkstra.Item2, fordBellman.Item2);
        }

    }
}
