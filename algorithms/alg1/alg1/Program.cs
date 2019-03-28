using System;
using System.IO;

namespace alg1
{
    class Program
    {
        static void Main(string[] args)
        {
            #if DEBUG
                args = new[] { @"..\..\lorem.txt" };
            #endif

            if (args.Length != 1)
            {
                Console.WriteLine("Использование:\nalg1.exe <filepath>");
                Console.ReadKey();
                return;
            }

            try
            {
                var hashtable = MyHashtable.FromFile(args[0], true);
                Console.WriteLine("Идентификаторы: " + hashtable.ElementCount);
                Console.WriteLine("Хэш-коды: " + hashtable.HashCount);
                Console.WriteLine("\nКоллизии: {0} ({1:P0})", hashtable.CollisionCount, hashtable.AverageCollisions);
                Console.WriteLine("Средний размер коллизии: " + hashtable.AverageCollisionSize);
                Console.WriteLine("Среднее кол-во сравнений при поиске: " + hashtable.AverageComparisons);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Ошибка: файл не найден.");
            }

            Console.ReadKey();
        }
    }
}
