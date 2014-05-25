using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FireWall : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding with fire wall");

        Debug.Log(other.gameObject.name);
    }
}
