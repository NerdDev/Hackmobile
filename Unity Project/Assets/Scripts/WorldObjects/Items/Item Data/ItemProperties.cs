using System;

public class ItemProperties 
{
    //Properties
    public BUC BUC { get; set; }

    public int GetHash()
    {
        int hash = 11;
        hash += BUC.GetHashCode();
        return hash;
    }
}