using System;

namespace alg3
{
    internal static class RNG
    {
        private static readonly Random random;

        static RNG()
        {
            random = new Random(DateTime.Now.Millisecond);
        }

        public static int Random()
        {
            return random.Next();
        }

        public static int Random(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }
}
