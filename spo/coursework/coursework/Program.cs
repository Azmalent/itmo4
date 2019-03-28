using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace coursework
{
    internal class Program
    {
        private const string ARRAY_FILENAME = "../../../array.txt";

        private static int[] LoadArray(string filename)
        {
            var list = new List<int>();
            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    bool isNumber = int.TryParse(line, out int number);
                    if (isNumber) list.Add(number);
                }
            }

            return list.ToArray();
        }

        private static void WriteRedLine(string str)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(1000);
        }

        private static string GetSortingMethod(ThreadRunner runner, bool titleCase = false)
        {
            string sort = runner.MergeFinished ? "слиянием" : "Шелла";
            if (titleCase) return char.ToUpper(sort[0]) + sort.Substring(1);
            return sort;
        }

        private static void DoneMessage(ThreadRunner runner)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nГотово.");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"Первой завершилась сортировка {GetSortingMethod(runner)}.");

            Thread.Sleep(3000);
        }

        private static void Main()
        {
            Console.WriteLine("Считывание массива...");
            var m = LoadArray(ARRAY_FILENAME);
            var random = new Random(DateTime.Now.Millisecond);
            int x = m[random.Next(0, m.Length)];

            Console.WriteLine("Размер массива: " + m.Length);
            Thread.Sleep(3000);

            WaitHandleThreadRunner semaphoreRunner, mutexRunner, eventRunner;
            TimeSpan semaphoreTime, mutexTime, eventTime;

            WriteRedLine("\nСинхронизация с помощью семафоров");
            using (var semaphore = new Semaphore(1, 1))
            {
                semaphoreRunner = new WaitHandleThreadRunner(m, x, semaphore);
                semaphoreTime = semaphoreRunner.Start();
                DoneMessage(semaphoreRunner);
            }

            WriteRedLine("\nСинхронизация с помощью мьютексов");
            using (var mutex = new Mutex())
            {
                mutexRunner = new WaitHandleThreadRunner(m, x, mutex);
                mutexTime = mutexRunner.Start();
            }


            WriteRedLine("\nСинхронизация с помощью событий");
            using (var eventWaitHandle = new AutoResetEvent(true))
            {
                eventRunner = new WaitHandleThreadRunner(m, x, eventWaitHandle);
                eventTime = eventRunner.Start();
                DoneMessage(eventRunner);
            }

            WriteRedLine("\nСинхронизация с помощью критических секций");
            var lockRunner = new LockThreadRunner(m, x);
            var lockTime = lockRunner.Start();
            DoneMessage(lockRunner);

            WriteRedLine("\nСинхронизация с помощью флага и SpinWait");
            var spinRunner = new SpinThreadRunner(m, x);
            var spinTime = spinRunner.Start();
            DoneMessage(spinRunner);

            Console.Clear();

            Console.WriteLine("\nРезультаты\n");

            var table = new Table("Метод синхронизации", "Время", "Сортировка");
            table.AddRow("Семафоры", $"{semaphoreTime:ss\\.ff} с", GetSortingMethod(semaphoreRunner, true));
            table.AddRow("Мьютексы", $"{mutexTime:ss\\.ff} с", GetSortingMethod(mutexRunner, true));
            table.AddRow("События", $"{eventTime:ss\\.ff} с", GetSortingMethod(eventRunner, true));
            table.AddRow("Критические секции", $"{lockTime:ss\\.ff} с", GetSortingMethod(lockRunner, true));
            table.AddRow("SpinWait", $"{spinTime:ss\\.ff} с", GetSortingMethod(spinRunner, true));
            table.Draw();

            Console.ReadKey();
        }
    }
}
