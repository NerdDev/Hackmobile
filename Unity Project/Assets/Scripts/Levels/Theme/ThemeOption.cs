using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public abstract class ThemeOption : MonoBehaviour
{
    public abstract Theme GetTheme(System.Random rand);
    public abstract double AvgRoomRadius { get; }
}
