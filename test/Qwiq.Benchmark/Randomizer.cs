using System;

namespace Qwiq.Benchmark
{
    public class Randomizer : Random
    {
        private static Randomizer random;

        public static Randomizer Instance => random ?? (random = new Randomizer());

        public static bool ShouldEnter()
        {
            return Instance.NextDouble() < 0.5;
        }

        public static int NextInt32()
        {
            unchecked
            {
                var firstBits = Instance.Next(0, 1 << 4) << 28;
                var lastBits = Instance.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

        /// <summary>
        /// Taken from http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp Jon Skeet's answer
        /// </summary>
        public static decimal NextDecimal()
        {
            var scale = (byte)Instance.Next(29);
            var sign = Instance.Next(2) == 1;
            return new decimal(NextInt32(), NextInt32(), NextInt32(), sign, scale);
        }
    }
}