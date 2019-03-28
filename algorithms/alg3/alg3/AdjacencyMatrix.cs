using System.Diagnostics;

namespace alg3
{
    internal struct AdjacencyMatrix
    {
        private readonly int[][] matrix;

        public AdjacencyMatrix(int size)
        {
            matrix = new int[size - 1][];

            for (int i = 0; i < size - 1; i++)
            {
                matrix[i] = new int[size - 1 - i];
            }
        }

        public int this[int a, int b]
        {
            get {
                Debug.Assert(a != b);
                if (b > a) return this[b, a];
                return matrix[b][a - b - 1];
            }

            set {
                Debug.Assert(a != b);
                if (a > b) matrix[b][a - b - 1] = value;
                else this[b, a] = value;
            }
        }
    }
}
