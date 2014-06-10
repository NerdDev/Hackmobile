using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class FireballAnimationSlow : MonoBehaviour
{
    void Start()
    {
        animation["ScaleFire"].speed = .4f;
    }
}
