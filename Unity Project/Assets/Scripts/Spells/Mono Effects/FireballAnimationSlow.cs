using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FireballAnimationSlow : MonoBehaviour
{
    void Start()
    {
        animation["ScaleFire"].speed = .4f;
    }
}
