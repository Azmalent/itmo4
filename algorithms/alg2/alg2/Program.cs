using System;
using System.IO;
using System.Linq;
using ExperimentResult = System.Tuple<long, long[], long[]>;

namespace alg2
{
    internal class Program
    {
        private const int N = 10_000;
        private const int M_LARGE = 100_000;
        private const int M_SMALL = 1000;
        private const int R = 1_000_000;

        private const int MEGABYTE = 1024 * 1024;
        private const int GC_MIN_SPACE = 128 * MEGABYTE;

        private static ExperimentResult Experiment(int mInit)
        {
            long graphTime = 0;
            var dijkstraTime = new long[100];
            var fordBellmanTime = new long[100];

            int m = mInit;
            for (int i = 0; i < 100; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Итерация " + i);
                Console.ForegroundColor = ConsoleColor.White;

                var startTime = DateTime.Now;
                var graph = Graph.Generate(N, m, R);
                graphTime += (DateTime.Now - startTime).Ticks;

                startTime = DateTime.Now;
                Graph.Dijkstra(graph, 0);
                var dijkstraTimespan = DateTime.Now - startTime;
                dijkstraTime[i] = dijkstraTimespan.Ticks;

                Console.WriteLine($"Дейкстра: {(int)dijkstraTimespan.TotalMilliseconds} мс");

                startTime = DateTime.Now;
                Graph.FordBellman(graph, 0);
                var fordBellmanTimespan = DateTime.Now - startTime;
                fordBellmanTime[i] = fordBellmanTimespan.Ticks;

                Console.WriteLine($"Форд-Беллман: {(int)fordBellmanTimespan.TotalMilliseconds} мс");
                Console.Write("\n\n");

                m += mInit;
            }

            Console.WriteLine("\nЭксперимент завершён.\n");

            return Tuple.Create(graphTime, dijkstraTime, fordBellmanTime);
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

            var graphTimespan       = TimeSpan.FromTicks(results.Item1);
            var dijkstraTimespan    = TimeSpan.FromTicks(results.Item2.Sum());
            var fordBellmanTimespan = TimeSpan.FromTicks(results.Item3.Sum());
            Console.WriteLine("Генерация графов:  {0:mm\\:ss}", graphTimespan);
            Console.WriteLine("Алгоритм Дейкстры:  {0:mm\\:ss}", dijkstraTimespan);
            Console.WriteLine("Алгоритм Форда-Беллмана: {0:mm\\:ss}", fordBellmanTimespan);
            SaveResults("results.txt", m, results.Item2, results.Item3);
        }

        static void Main(string[] args)
        {
            ConsoleKey key;
            int        menuChoice = 0;
            do
            {
                Console.WriteLine("Выберите эксперимент:");
                Console.WriteLine("{0} №1: {1}...{2} рёбер", menuChoice == 0 ? ">" : " ", M_LARGE, M_LARGE * 100);
                Console.WriteLine("{0} №2: {1}...{2} рёбер", menuChoice == 1 ? ">" : " ", M_SMALL, M_SMALL * 100);

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow) menuChoice++;
                else if (key == ConsoleKey.UpArrow) menuChoice--;

                if (menuChoice < 0) menuChoice      = 1;
                else if (menuChoice > 1) menuChoice = 0;

                Console.Clear();
            } while (key != ConsoleKey.Enter);

            SingleExperiment(menuChoice == 1 ? M_SMALL : M_LARGE);

            Console.ReadKey();
        }
    }
}
