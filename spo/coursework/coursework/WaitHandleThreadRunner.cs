using System;
using System.Threading;

namespace coursework
{
    /// <summary>
    /// Запускает потоки А, В и С, используя для синхронизации производный от WaitHandle класс.
    /// </summary>
    class WaitHandleThreadRunner : ThreadRunner
    {
        private readonly WaitHandle waitHandle;

        public WaitHandleThreadRunner(int[] m, int x, WaitHandle waitHandle) : base(m, x)
        {
            this.waitHandle = waitHandle;
        }

        private void Exit()
        {
            switch (waitHandle)
            {
                case Semaphore s:
                    s.Release(1);
                    break;
                case Mutex m:
                    m.ReleaseMutex();
                    break;
                case EventWaitHandle e:
                    e.Set();
                    break;
            }
        }

        public override void UpdateArray(ref int counter, int[] m1)
        {
            counter++;
            if (counter < ItersToUpdate) return;

            counter = 0;

            waitHandle.WaitOne();

            Console.WriteLine(Thread.CurrentThread.Name + " обновляет массив...");
            Array.Copy(m1, M1, m1.Length);

            Exit();

            SearchEvent.Set();
        }
    }
}
