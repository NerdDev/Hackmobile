using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ProbabilityItem<T> : IProbabilityItem
{
    public double Multiplier { get; set; }
    public bool Unique { get; set; }
    public T Item;

    public ProbabilityItem(T item, double multiplier, bool unique = false)
    {
        this.Multiplier = multiplier;
        this.Unique = unique;
        this.Item = item;
    }

    public override string ToString()
    {
        return "Item: " + Item + ", Multiplier: " + Multiplier + ", Unique: " + Unique;
    }
}

