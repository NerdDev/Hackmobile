using System;
using System.Collections;
using System.Collections.Generic;

public static class BitArrayExt
{
    public static bool EqualsReal(this BitArray a, BitArray rhs)
    {
        BitArray tmp = a.Xor(rhs);
        foreach (bool b in tmp)
        {
            if (b)
                return false;
        }
        return true;
    }

    public static bool Contains(this BitArray a, BitArray rhs)
    {
        BitArray tmp = a.And(rhs);
        return tmp.EqualsReal(rhs);
    }
}
