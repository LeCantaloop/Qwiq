using System;

namespace Qwiq.Mocks
{
    public class Randomizer : Random
    {
        private static Randomizer random;

        public static Randomizer Instance => random ?? (random = new Randomizer());
    }
}