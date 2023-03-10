using System;
using System.Collections.Generic;

namespace Carotaa.Code
{
    public static class LinkedListExtension
    {
        public static LinkedListNode<T> Find<T>(this LinkedList<T> list, Predicate<T> predicate)
        {
            var prt = list.First;
            while (prt != null)
            {
                if (predicate.Invoke(prt.Value)) return prt;

                prt = prt.Next;
            }

            return null;
        }
    }
}