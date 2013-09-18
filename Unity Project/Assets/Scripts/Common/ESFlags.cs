using System;
using System.Collections.Generic;

public class ESFlags<T> where T : IComparable, IConvertible
{
    GenericFlags<T> flags = new GenericFlags<T>();
    HashSet<string> strings = new HashSet<string>();
}
