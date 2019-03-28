using System;
using System.Runtime.InteropServices;

namespace spo3
{
    class Program
    {
        [DllImport("mydll.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DllCopy();

        [DllImport("mydll.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DllMemory();

        [DllImport("mydll.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void DllSeries();

        [DllImport("mydll.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void DllWindows();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("\n#1: Методы копирования\n");
            DllCopy();

            Console.WriteLine("\n#2: Методы управления памятью\n");
            DllMemory();

            Console.WriteLine("\n#3: Сумма ряда\n");
            DllSeries();

            Console.WriteLine("\n\n#4: Оконное приложение");
            Console.WriteLine("\nНажмите любую клавишу, чтобы открыть окна");
            Console.ReadKey();

            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            DllWindows();
        }
    }
}