using UnityEngine;
using System.Collections;

public interface IManager {
    bool Initialized { get; set; }
    void Initialize();
}
