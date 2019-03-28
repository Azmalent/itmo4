using System;

namespace alg2
{
    internal struct DHeap
    {
        private const int D = 7;

        public short[] Name;
        public int[] Key;
        public int[] Index;
        public int N;

        public DHeap(int n)
        {
            N  = n;
            Key  = new int[n];
            Name = new short[n];
            Index = new int[n];

            for (short i = 0; i < n; i++)
            {
                Name[i] = i;
                Key[i]  = Graph.INFINITY;
                Index[i] = i;
            }
        }

        public bool IsEmpty => N == 0;

        public void DeleteMin()
        {
            Transpose(0, N - 1);
            N--;
            if (N > 0) SiftDown (0);
        }

        public void Heapify()
        {
            for (int i = N - 1; i >= 0; i--) SiftDown(i);
        }

        public void SiftUp(int i)
        {
            int key0  = Key[i];
            short name0 = Name[i];
            int p = Parent (i);
            while (i != 0 && Key[p] > key0)
            {
                //todo: transpose
                Key[i]  = Key[p];
                Name[i] = Name[p];
                Index [Name[i]] = i;
                i = p;
                p = Parent( i );
            }
            Key[i]  = key0;
            Name[i] = name0;
            Index [Name[i]] = i;
        }

        private void SiftDown(int i)
        {
            int key0 = Key[i];

            int s = MinChild(i);
            while (s != i && key0 > Key[s])
            {
                Transpose(i, s);
                i = s;
                s = MinChild(i);
            }
        }

        private int MinChild(int i)
        {
            int s = FirstChild(i);
            if (s == 0) return i;

            int last = LastChild(i);
            int minKey = Key[s];
            for (int j = s + 1; j <= last; j ++)
            {
                if (Key[j] < minKey)
                {
                    minKey  = Key[j];
                    s = j;
                }
            }

            return s;
        }

        int FirstChild(int i)
        {
            int k = i * D + 1;
            if (k >= N) return 0;
            return k;
        }

        int LastChild(int i)
        {
            int k = FirstChild(i);
            if (k == 0) return 0;
            return Math.Min (k + D - 1, N - 1);
        }

        private int Parent(int i)
        {
            return (i - 1) / D;
        }

        private void Transpose(int a, int b)
        {
            int oldKey = Key[a];
            Key[a] = Key[b];
            Key[b] = oldKey;

            short oldValue = Name[a];
            Name[a] = Name[b];
            Name[b] = oldValue;
        }
    }
}