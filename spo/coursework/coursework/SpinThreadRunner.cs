using System;
using System.Threading;

namespace coursework
{
    internal class SpinThreadRunner : ThreadRunner
    {
        private volatile bool locked;

        public SpinThreadRunner(int[] m, int x) : base(m, x)
        {
            locked = false;
        }

        public override void UpdateArray(ref int counter, int[] m1)
        {
            counter++;
            if (counter < ItersToUpdate) return;

            counter = 0;

            var spinWait = new SpinWait();
            while (locked)
            {
                Thread.MemoryBarrier();
                spinWait.SpinOnce();
            }

            locked = true;

            Console.WriteLine(Thread.CurrentThread.Name + " обновляет массив...");
            Array.Copy(m1, M1, m1.Length);

            locked = false;

            SearchEvent.Set();
        }
    }
}