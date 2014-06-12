using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class ParticleResizer : MonoBehaviour
{
    public GameObject sizeObj;

    void Start()
    {
        particleSystem.startSize = Math.Max(sizeObj.transform.localScale.x, sizeObj.transform.localScale.z) * 5;
    }
}
