using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PriorityMod.Extensions
{
    public static class ListExtensions
    {

        public static void AppendList<E>(this List<E> list, List<E> otherList, int startIndex = 0, int endIndex = -1, int offset = -1)
        {
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            if (endIndex <= -1)
            {
                endIndex = otherList.Count;
            }
            if (offset <= -1)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    list.Add(otherList[i]);
                }
                return;
            }
            for (int i = startIndex; i < endIndex; i++)
            {
                list.Insert(offset + (i - startIndex), otherList[i]);
            }
        }

    }
}
