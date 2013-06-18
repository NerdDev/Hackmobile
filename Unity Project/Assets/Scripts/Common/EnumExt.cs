using System;

/**
* Extension methods to make working with Enum values easier.
*/
public static class EnumExt
{

    #region Extension Methods

    /**
    * Includes an enumerated type and returns the new value.
    */
    public static T Include<T>(this Enum value, T append)
    {
        Type type = value.GetType();

        //determine the values
        object result = value;
        _Value parsed = new _Value(append, type);
        if (parsed.Signed is long)
        {
            result = Convert.ToInt64(value) | (long) parsed.Signed;
        }
        else if (parsed.Unsigned is ulong)
        {
            result = Convert.ToUInt64(value) | (ulong) parsed.Unsigned;
        }

        //return the final value
        return (T) Enum.Parse(type, result.ToString());
    }

    /**
     * Added these for the sake of not needing to do an assignation.
     */
    public static void Add<T>(this Enum append, ref T value)
    {
        value = Include(append, value);
    }

    public static void Remove<T>(this Enum remove, ref T value)
    {
        value = Expel(remove, value);
    }

    /**
    * Takes out an enumerated type and returns the new value.
    */
    public static T Expel<T>(this Enum value, T remove)
    {
        Type type = value.GetType();

        //determine the values
        object result = value;
        _Value parsed = new _Value(remove, type);
        if (parsed.Signed is long)
        {
            result = Convert.ToInt64(value) & ~(long) parsed.Signed;
        }
        else if (parsed.Unsigned is ulong)
        {
            result = Convert.ToUInt64(value) & ~(ulong) parsed.Unsigned;
        }

        //return the final value
        return (T) Enum.Parse(type, result.ToString());
    }

    /**
    * Checks if an enumerated type contains a value.
    */
    public static bool Has<T>(this Enum value, T check)
    {
        Type type = value.GetType();

        //determine the values
        object result = value;
        _Value parsed = new _Value(check, type);
        if (parsed.Signed is long)
        {
            return (Convert.ToInt64(value) & (long) parsed.Signed) == (long) parsed.Signed;
        }
        else if (parsed.Unsigned is ulong)
        {
            return (Convert.ToUInt64(value) & (ulong) parsed.Unsigned) == (ulong) parsed.Unsigned;
        }
        else
        {
            return false;
        }
    }

    /**
    * Checks if an enumerated type is missing a value.
    */
    public static bool Missing<T>(this Enum obj, T value)
    {
        return !EnumExt.Has<T>(obj, value);
    }

    #endregion

    #region Helper Classes

    //class to simplfy narrowing values between 
    //a ulong and long since either value should
    //cover any lesser value
    private class _Value
    {
        //cached comparisons for type to use
        private static Type _UInt64 = typeof(ulong);
        private static Type _UInt32 = typeof(long);

        public long? Signed;
        public ulong? Unsigned;

        public _Value(object value, Type type)
        {
            //make sure it is even an enum to work with
            if (!type.IsEnum)
            {
                throw new ArgumentException("Value provided is not an enumerated type!");
            }

            //then check for the enumerated value
            Type compare = Enum.GetUnderlyingType(type);

            //if this is an unsigned long then the only
            //value that can hold it would be a ulong
            if (compare.Equals(_Value._UInt32) || compare.Equals(_Value._UInt64))
            {
                this.Unsigned = Convert.ToUInt64(value);
            }
            //otherwise, a long should cover anything else
            else
            {
                this.Signed = Convert.ToInt64(value);
            }
        }
    }
    #endregion
}

