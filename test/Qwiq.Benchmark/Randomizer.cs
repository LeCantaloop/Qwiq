using System;

namespace Microsoft.Qwiq.Benchmark
{
    public class Randomizer : Random
    {
        private static Randomizer random;

        public static Randomizer Instance => random ?? (random = new Randomizer());

       
    }

    public static class RandomizerExtensions
    {
        public static int NextSystemId(this Randomizer instance)
        {
            return instance.NextSystemId(int.MaxValue);
        }

        public static int NextSystemId(this Randomizer instance, int max)
        {
            return instance.NextSystemId(1, max);
        }

        public static int NextSystemId(this Randomizer instance, int min, int max)
        {
            return instance.Next(min, max);
        }

        /// <summary>
        /// Taken from http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp Jon Skeet's answer
        /// </summary>
        public static decimal NextDecimal(this Randomizer instance)
        {
            var scale = (byte)instance.Next(29);
            var sign = instance.Next(2) == 1;
            return new decimal(instance.NextInt32(), instance.NextInt32(), instance.NextInt32(), sign, scale);
        }

        public static int NextInt32(this Randomizer instance)
        {
            unchecked
            {
                var firstBits = instance.Next(0, 1 << 4) << 28;
                var lastBits = instance.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

        public static bool ShouldEnter(this Randomizer instance)
        {
            return instance.NextDouble() < 0.5;
        }
    }
}