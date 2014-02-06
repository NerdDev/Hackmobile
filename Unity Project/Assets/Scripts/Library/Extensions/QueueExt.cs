﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class QueueExt
{
    public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> rhs)
    {
        foreach (T t in rhs)
        {
            queue.Enqueue(t);
        }
    }
}

