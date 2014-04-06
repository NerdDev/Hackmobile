using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IEnsureType
{
    void EnsureType(Type target);
}

public static class IEnsureTypeExt
{
    public static void EnsureType(this IEnsureType caller, Type target, object obj)
    {
        if (!target.IsAssignableFrom(obj.GetType()))
        {
            throw new ArgumentException("Ensure Type Failed. (" + obj + ") of type " + obj.GetType() + " needed to be of type " + target);
        }
    }
}
