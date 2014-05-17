using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ReferenceEqualityComparer : EqualityComparer<Object>
{
    public static ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

    private ReferenceEqualityComparer()
    {

    }

    public override bool Equals(object x, object y)
    {
        return ReferenceEquals(x, y);
    }
    public override int GetHashCode(object obj)
    {
        if (obj == null) return 0;
        return obj.GetHashCode();
    }
}