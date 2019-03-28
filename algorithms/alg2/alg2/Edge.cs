namespace alg2
{
    internal struct Edge
    {
        public readonly int Destination;
        public readonly int Weight;

        public Edge(int destination, int weight)
        {
            Destination = destination;
            Weight      = weight;
        }
    }
}