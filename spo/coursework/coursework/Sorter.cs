using System;

namespace coursework
{
    internal static class Sorter
    {
        public static void ShellSort(int[] input, ThreadRunner runner)
        {
            int iterationCount = 0;
            int n  = input.Length;

            var m1 = new int[n];
            Array.Copy(input, m1, n);

            for (int gap = n/100; gap > 0; gap /= 10)
            {
                for (int i = gap; i < n; i += 1)
                {
                    int temp = m1[i];

                    int j = i;
                    while (j >= gap && m1[j - gap] > temp)
                    {
                        m1[j] = m1[j - gap];
                        j -= gap;
                    }

                    m1[j] = temp;

                    if (runner.MergeFinished) return;
                    runner.UpdateArray(ref iterationCount, m1);
                }
            }

            runner.ShellFinished = true;
        }

        public static void MergeSort(int[] input, ThreadRunner runner)
        {
            int iterationCount = 0;
            int n = input.Length;

            var m1 = new int[n];
            Array.Copy(input, m1, n);

            for (int i = 1; i < n; i *= 2)
            {
                for (int j = 0; j < n - i; j += 2 * i)
                {
                    int right = j + 2 * i;
                    if (n < right) right = n;
                    Merge(m1, j, j + i, right);

                    if (runner.ShellFinished) return;
                    runner.UpdateArray(ref iterationCount, m1);
                }
            }

            runner.MergeFinished = true;
        }

        private static void Merge(int[] input, int left, int mid, int right)
        {
            int a = 0;
            int b = 0;
            int[] result = new int[right - left];

            while (left + a < mid && mid + b < right)
            {
                if (input[left + a] < input[mid + b])
                {
                    result[a + b] = input[left + a];
                    a++;
                }
                else
                {
                    result[a + b] = input[mid + b];
                    b++;
                }
            }
            while (left + a < mid)
            {
                result[a + b] = input[left + a];
                a++;
            }
            while (mid + b < right)
            {
                result[a + b] = input[mid + b];
                b++;
            }

            for (int i = 0; i < a + b; i++)
                input[left + i] = result[i];
        }
    }
}
