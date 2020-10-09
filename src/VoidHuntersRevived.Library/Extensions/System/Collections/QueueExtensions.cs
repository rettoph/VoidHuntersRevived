using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.System.Collections
{
    public static class QueueExtensions
    {
        public static void Remove<T>(this Queue<T> queue, T item)
            where T : class
        {
            T target;
            if(queue.Contains(item))
            {
                while ((target = queue.Dequeue()) != item)
                    queue.Enqueue(target);
            }
        }
    }
}
