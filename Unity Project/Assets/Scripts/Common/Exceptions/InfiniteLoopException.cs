using UnityEngine;
using System;

public class InfiniteLoopException: System.Exception
{

   public InfiniteLoopException()
   {
   }

   public InfiniteLoopException(string message): base(message)
   {
   }
}

