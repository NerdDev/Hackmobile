using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class StairsUp : MonoBehaviour
{
    void OnMouseDown()
    {
        BigBoss.Levels.SetCurLevel(true);
    }
}

