using System;
using System.Collections.Generic;
using System.Linq;

namespace Maniac.Utils.Extension
{
    public static class LinQExtension
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (rng == null) throw new ArgumentNullException(nameof(rng));

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        public static T TakeRandom<T>(this IEnumerable<T> list)
        {
            return list.Shuffle().FirstOrDefault();
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> list, int numberOfItems)
        {
            return list.Shuffle().Take(numberOfItems).ToList();
        }
    }
}