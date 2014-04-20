using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIActionSet
{
    List<AIAction> set = new List<AIAction>();

    public void AddAction(AIAction action)
    {
        set.Add(action);
    }
}
