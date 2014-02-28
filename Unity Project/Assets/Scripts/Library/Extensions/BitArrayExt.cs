using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class BitArrayExt
{
    public static bool EqualsReal(this BitArray a, BitArray rhs)
    {
        throw new NotImplementedException("BIT ARRAYS SUCK");
        BitArray tmp = new BitArray(a);
        tmp = tmp.Xor(rhs);
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

    public static int GetHashCodeReal(this BitArray array)
    {
        UInt32 hash = 17;
        int bitsRemaining = array.Length;
        foreach (int value in array.GetInternalValues())
        {
            UInt32 cleanValue = (UInt32)value;
            if (bitsRemaining < 32)
            {
                //clear any bits that are beyond the end of the array
                int bitsToWipe = 32 - bitsRemaining;
                cleanValue <<= bitsToWipe;
                cleanValue >>= bitsToWipe;
            }

            hash = hash * 23 + cleanValue;
            bitsRemaining -= 32;
        }
        return (int)hash;
    }
    
    static FieldInfo _internalArrayGetter = GetInternalArrayGetter();

    static FieldInfo GetInternalArrayGetter()
    {
        return typeof(BitArray).GetField("m_array", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    static int[] GetInternalArray(this BitArray array)
    {
        return (int[])_internalArrayGetter.GetValue(array);
    }

    public static IEnumerable<int> GetInternalValues(this BitArray array)
    {
        return GetInternalArray(array);
    }
}
