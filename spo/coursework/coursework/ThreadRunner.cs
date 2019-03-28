using System;
using System.Threading;

namespace coursework
{
    internal abstract class ThreadRunner
    {
        public int[] M;
        public volatile int[] M1;

        //Элемент для поиска
        protected readonly int SearchElement;

        //Количество итераций между обновлениями
        public readonly int ItersToUpdate;

        protected readonly Thread ThreadA;
        protected readonly Thread ThreadB;
        protected readonly Thread ThreadC;

        public volatile bool ShellFinished;
        public volatile bool MergeFinished;

        protected AutoResetEvent[] AutoEvents;
        protected AutoResetEvent SearchEvent;

        protected ThreadRunner(int[] m, int x)
        {
            M = m;
            SearchElement = x;
            ItersToUpdate = m.Length / 100;

            M1 = new int[m.Length];
            ShellFinished = false;
            MergeFinished = false;

            AutoEvents = new AutoResetEvent[3];
            for (int i = 0; i < 3; i++)
                AutoEvents[i] = new AutoResetEvent(false);

            ThreadA = new Thread(ThreadAProc);
            ThreadB = new Thread(ThreadBProc);
            ThreadC = new Thread(ThreadCProc);
            ThreadA.Name = "Поток А";
            ThreadB.Name = "Поток В";
            ThreadC.Name = "Поток С";

            SearchEvent = new AutoResetEvent(false);
        }

        public TimeSpan Start()
        {
            var startTime = DateTime.Now;

            ThreadA.Start();
            ThreadB.Start();
            ThreadC.Start();

            WaitHandle.WaitAll(AutoEvents);

            return DateTime.Now - startTime;
        }

        public abstract void UpdateArray(ref int counter, int[] m1);

        private void ThreadAProc()
        {
            Sorter.ShellSort(M, this);
            AutoEvents[0].Set();
        }

        private void ThreadBProc()
        {
            Sorter.MergeSort(M, this);
            AutoEvents[1].Set();
        }

        private void ThreadCProc()
        {
            while (!ShellFinished && !MergeFinished)
            {
                if (!SearchEvent.WaitOne(500)) continue;

                int index = Array.IndexOf(M1, SearchElement);
                Console.WriteLine($"Элемент {SearchElement} {(index == -1 ? "не найден" : $"найден на позиции {index}")}.");
            }

            AutoEvents[2].Set();
        }
    }
}
