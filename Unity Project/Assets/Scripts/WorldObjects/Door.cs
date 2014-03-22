using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int RotationAngle;
    public Vector3 OpenPosition;

    public bool open = false;

    public List<GameObject> ObjectsToDisable;

    private GameObject invisObject;
    private Point doorLoc;

    void OnMouseDown() //player's input only here
    {
        doorLoc = new Point(transform.parent.position.x, transform.parent.position.z);
        if (BigBoss.Player.IsNextToTarget(BigBoss.Levels.Level[doorLoc.x, doorLoc.y]))
        {
            OpenDoor();
            BigBoss.Time.PassTurn(60);
        }
    }

    public void OpenDoor()
    {
        doorLoc = new Point(transform.parent.position.x, transform.parent.position.z);
        if (open)
        {
            open = !open;

            transform.Rotate(Vector3.up, RotationAngle);
            transform.position += OpenPosition;

            if (ObjectsToDisable != null && ObjectsToDisable.Count > 0)
            {
                foreach (GameObject go in ObjectsToDisable)
                {
                    go.gameObject.layer = 12;
                }
            }
            FOWSystem.instance.ModifyGrid(new Vector3(doorLoc.x, 0f, doorLoc.y), 0, 2, 0);
        }
        else
        {
            open = !open;

            transform.Rotate(Vector3.up, -RotationAngle);
            transform.position -= OpenPosition;

            if (ObjectsToDisable != null && ObjectsToDisable.Count > 0)
            {
                foreach (GameObject go in ObjectsToDisable)
                {
                    go.gameObject.layer = 0;
                }
            }
            FOWSystem.instance.ModifyGrid(new Vector3(doorLoc.x, 0f, doorLoc.y), 0, 2, 0);
        }
    }
}
