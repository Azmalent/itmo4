using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace coursework
{
    internal class Table
    {
        private string[] head;
        private List<string[]> body;
        private int[] width;

        private ConsoleColor headerColor;
        private ConsoleColor borderColor;

        public Table(params string[] head)
        {
            this.head = head;
            width = new int[head.Length];
            body = new List<string[]>();

            headerColor = ConsoleColor.Green;
            borderColor = ConsoleColor.White;

            for (int i = 0; i < head.Length; i++)
            {
                width[i] = head[i].Length;
            }
        }

        private void UpdateWidth()
        {
            foreach (var row in body)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    row[i] = row[i].PadRight(width[i], ' ');
                }
            }
        }

        public void AddRow(params string[] values)
        {
            Debug.Assert(values.Length == head.Length);

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Length > width[i])
                {
                    int widthIncrease = values[i].Length - width[i];
                    if (widthIncrease % 2 == 1) widthIncrease++;

                    width[i] += widthIncrease;

                    int pad = widthIncrease / 2;
                    head[i] = head[i].PadLeft(width[i] - pad, ' ').PadRight(width[i], ' ');
                }
            }

            body.Add(values);

            UpdateWidth();
        }

        private void DrawHeader()
        {
            Console.ForegroundColor = borderColor;
            Console.Write('╔');
            for (int i = 0; i < head.Length; i++)
            {
                for (int j = 0; j < width[i]; j++) Console.Write('═');
                if (i < head.Length - 1) Console.Write('╦');
            }
            Console.WriteLine('╗');

            Console.Write('║');
            for (int i = 0; i < head.Length; i++)
            {
                Console.ForegroundColor = headerColor;
                Console.Write(head[i]);
                Console.ForegroundColor = borderColor;
                Console.Write('║');
            }
            Console.WriteLine();

            Console.Write('╠');
            for (int i = 0; i < head.Length; i++)
            {
                for (int j = 0; j < width[i]; j++) Console.Write('═');
                if (i < head.Length - 1) Console.Write('╬');
            }
            Console.WriteLine('╣');
        }

        private void DrawBody()
        {
            int rowIndex = 0;
            int rowCount = body.Count;

            foreach (var row in body)
            {
                rowIndex++;
                bool isLast = rowIndex == rowCount;

                Console.Write('║');
                for (int i = 0; i < row.Length; i++)
                {
                    Console.Write(row[i]);
                    Console.Write('║');
                }
                Console.WriteLine();

                Console.Write(isLast ? '╚' : '╠');
                for (int i = 0; i < row.Length; i++)
                {
                    for (int j = 0; j < width[i]; j++) Console.Write('═');
                    if (i < row.Length - 1) Console.Write(isLast ? '╩' : '╬');
                }
                Console.WriteLine(isLast ? '╝' : '╣');
            }
        }

        public void Draw()
        {
            DrawHeader();
            DrawBody();
        }
    }
}
