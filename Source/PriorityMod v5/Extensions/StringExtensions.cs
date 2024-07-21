using System;
using System.Collections.Generic;

namespace PriorityMod.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<String> SplitInParts(this String value, int length)
        {
            return SplitInParts(value, length, -1);
        }
        public static IEnumerable<String> SplitInParts(this String value, int length, int limit)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (length <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(length));

            if (limit > 0)
                for (int index = 0, attempt = 1; index < value.Length; index += length, attempt++)
                {
                    if (attempt != limit)
                    {
                        yield return value.Substring(index, Math.Min(length, value.Length - index));
                        continue;
                    }
                    yield return value.Substring(index, value.Length);
                    break;
                }
            else
                for (int index = 0; index < value.Length; index += length)
                    yield return value.Substring(index, Math.Min(length, value.Length - index));

        }

    }
}
