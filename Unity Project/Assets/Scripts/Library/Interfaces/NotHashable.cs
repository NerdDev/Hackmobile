using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

[AttributeUsage(AttributeTargets.Field)]
public sealed class NotHashable : Attribute
{
}
