using UnityEngine;
using System.Collections;

public interface IProbabilityItem
{
    double Multiplier { get; }
    bool Unique { get; }
}
