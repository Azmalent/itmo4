using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace alg1
{
    internal class MyHashtable
    {
        private const int HASHTABLE_SIZE = char.MaxValue * 3;

        private struct HashtableEntry
        {
            public int CollisionSize;
            public Element Element;
        }

        private class Element
        {
            public readonly string Value;
            public Element NextElement;

            public Element(string value)
            {
                Value = value;
                NextElement = null;
            }
        }
        private readonly HashtableEntry[] table;

        public int ElementCount    { get; private set; }
        public int HashCount       { get; private set; }
        public int CollisionCount  { get; private set; }
        public int ComparisonCount { get; private set; }
        public int MaxCollisionSize => table.Max(e => e.CollisionSize);
        
        public double AverageCollisions  => HashCount == 0 ? 0 : (double)CollisionCount / HashCount;
        public double AverageComparisons;

        public int AverageCollisionSize => (int)table.Where(e => e.Element != null).Select(e => e.CollisionSize).Average();

        public MyHashtable()
        {
            table = new HashtableEntry[HASHTABLE_SIZE + 1];
            CollisionCount = 0;
            HashCount = 0;
            ElementCount = 0;
            ComparisonCount = 0;
        }
        public static MyHashtable FromFile(string filename, bool countComparisons = false)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException();

            var hashtable = new MyHashtable();
            using (var reader = new StreamReader(filename))
            {
                var separators = new[] {' ', '\n', '\t'};
                var words = reader.ReadToEnd().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words) hashtable.Add(word);

                if (countComparisons)
                {
                    var list = new List<int>();
                    foreach (var word in words)
                    {
                        int c;
                        Debug.Assert(hashtable.Contains(word, out c));
                        list.Add(c);
                    }

                    hashtable.AverageComparisons = list.Max();
                }
            }

            return hashtable;
        }

        private static int Hash(string value)
        {
            int startIndex = value.Length - Math.Min(3, value.Length);

            int sum = 0;
            for (int index = startIndex; index < value.Length; index++)
                sum += value[index];

            Debug.Assert(sum <= HASHTABLE_SIZE);
            return sum;
        }

        public void Add(string value)
        {
            int hashcode = Hash(value);
            var element = new Element(value);

            if (table[hashcode].Element == null)
            {
                table[hashcode].Element = element;
                HashCount++;
            }
            else
            {
                int collisionSize = 2;
                Element target = table[hashcode].Element;
                if (target.NextElement == null) CollisionCount++;
                while (target.NextElement != null)
                {
                    collisionSize++;
                    target = target.NextElement;
                }
                target.NextElement = element;
                table[hashcode].CollisionSize = collisionSize;
            }

            ElementCount++;
        }
        public bool Contains(string value, out int comparisons)
        {
            comparisons = 0;
            int hashcode = Hash(value);
            Element element = table[hashcode].Element;

            while (element != null)
            {
                comparisons++;
                if (element.Value == value) return true;
                element = element.NextElement;
            }

            return false;
        }
    }
}