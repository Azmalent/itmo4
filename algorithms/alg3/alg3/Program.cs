using System;
using System.IO;
using System.Linq;

using ExperimentResult = System.Tuple<long, long[], long[]>;

namespace alg3
{
    class Program
    {
        private const int N = 10_000;
        private const int M_LARGE = 100_000;
        private const int M_SMALL = 10_000;
        private const int R = 1000000;

        private static ExperimentResult Experiment(int mInit)
        {
            long graphTime = 0;
            var boruvkaTime = new long[100];
            var kruskalTime = new long[100];

            int m = mInit;
            for (int i = 0; i < 100; i++)
            {
                var startTime = DateTime.Now;
                var graph = Graph.Generate(N, m, R);
                graphTime += (DateTime.Now - startTime).Ticks;

                startTime = DateTime.Now;
                Graph.Boruvka(graph);
                var boruvkaTimespan = DateTime.Now - startTime;
                boruvkaTime[i] = boruvkaTimespan.Ticks;

                startTime = DateTime.Now;
                Graph.Kruskal(graph);
                var kruskalTimespan = DateTime.Now - startTime;
                kruskalTime[i] = kruskalTimespan.Ticks;

                m += mInit;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Итерация " + i);
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Борувка: " + (int)boruvkaTimespan.TotalMilliseconds + " мс");
                Console.WriteLine("Краскал: " + (int)kruskalTimespan.TotalMilliseconds + " мс");
                Console.Write("\n\n");
            }

            Console.WriteLine("\nЭксперимент завершён.\n");

            return Tuple.Create(graphTime, boruvkaTime, kruskalTime);
        }

        private static void SaveResults(string filename, int mInit, long[] results1, long[] results2)
        {
            using (var writer = new StreamWriter(filename))
            {
                int m = mInit;
                for (int i = 0; i < 100; i++)
                {
                    long ms1 = results1[i] / TimeSpan.TicksPerMillisecond;
                    long ms2 = results2[i] / TimeSpan.TicksPerMillisecond;
                    writer.WriteLine(m + "\t" + ms1 + "\t" + ms2);

                    m += mInit;
                }
            }
        }

        private static void SingleExperiment(int m)
        {
            var results = Experiment(m);

            var graphTimespan = TimeSpan.FromTicks(results.Item1);
            var boruvkaTimespan  = TimeSpan.FromTicks(results.Item2.Sum());
            var kruskalTimespan  = TimeSpan.FromTicks(results.Item3.Sum());
            Console.WriteLine("Генерация графов:  {0:mm\\:ss}", graphTimespan);
            Console.WriteLine("Алгоритм Борувки:  {0:mm\\:ss}", boruvkaTimespan);
            Console.WriteLine("Алгоритм Краскала: {0:mm\\:ss}", kruskalTimespan);
            SaveResults("results.txt", m, results.Item2, results.Item3);
        }

        static void Main(string[] args)
        {
            ConsoleKey key;
            int menuChoice = 0;
            do
            {
                Console.WriteLine("Выберите эксперимент:");
                Console.WriteLine("{0} №1: {1}...{2} рёбер", menuChoice == 0 ? ">" : " ", M_LARGE, M_LARGE * 100);
                Console.WriteLine("{0} №2: {1}...{2} рёбер", menuChoice == 1 ? ">" : " ", M_SMALL, M_SMALL * 100);

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow) menuChoice++;
                else if (key == ConsoleKey.UpArrow) menuChoice--;

                if (menuChoice < 0) menuChoice = 1;
                else if (menuChoice > 1) menuChoice = 0;

                Console.Clear();
            } while (key != ConsoleKey.Enter);

            switch (menuChoice)
            {
                case 0:
                    SingleExperiment(M_LARGE);
                    break;

                case 1:
                    SingleExperiment(M_SMALL);
                    break;
            }

            Console.ReadKey();
        }
    }
}
