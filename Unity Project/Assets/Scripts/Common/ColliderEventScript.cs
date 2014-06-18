using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ColliderEventScript : MonoBehaviour
{
    public event Action<Collision> OnCollisionEnterEvent;

    public void OnCollisionEnter(Collision collision)
    {
        if (OnCollisionEnterEvent != null)
        {
            OnCollisionEnterEvent(collision);
        }
    }
}

