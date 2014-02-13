using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StartManager : MonoBehaviour, IManager
{

    public bool Initialized { get; set; }
    public void Initialize()
    {

    }

    public void Start()
    {
        BigBoss.Gooey.OpenMenuGUI();
    }

    public IEnumerator StartGame(List<GameObject> objectsToDestroy)
    {
        BigBoss.Gooey.DisplayLoading();
        PathTree tree = new PathTree();
        foreach (GameObject go in objectsToDestroy)
        {
            Destroy(go);
        }
        yield return new WaitForSeconds(.01f);
        BigBoss.Gooey.OpenInventoryGUI();
        BigBoss.Levels.SetCurLevel(0);

        // Temp (will move eventually)
        BigBoss.PlayerInfo.Rendering(true);
        BigBoss.Gooey.CloseLoading();
    }
}
