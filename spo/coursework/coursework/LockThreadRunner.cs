using System;
using System.Threading;

namespace coursework
{
    /// <summary>
    /// Запускает потоки А, В и С, используя для синхронизации критическую секцию.
    /// </summary>
    class LockThreadRunner : ThreadRunner
    {
        private readonly object locker;

        public LockThreadRunner(int[] m, int x) : base(m, x)
        {
            locker = new object();
        }

        public override void UpdateArray(ref int counter, int[] m1)
        {
            counter++;
            if (counter < ItersToUpdate) return;

            counter = 0;

            lock(locker)
            {
                Console.WriteLine(Thread.CurrentThread.Name + " обновляет массив...");
                Array.Copy(m1, M1, m1.Length);
            }

            SearchEvent.Set();
        }
    }
}
